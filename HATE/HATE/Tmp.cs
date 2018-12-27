using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Security.Principal;

namespace HATE
{
    public partial class MainForm : Form
    {
        private static class Style
        {
            private static readonly Color _btnRestoreColor = Color.LimeGreen;
            private static readonly Color _btnCorruptColor = Color.Coral;
            private static readonly string _btnRestoreLabel = " -RESTORE- ";
            private static readonly string _btnCorruptLabel = " -CORRUPT- ";
            private static readonly Color _optionSet = Color.Yellow;
            private static readonly Color _optionUnset = Color.White;

            public static Color GetOptionColor(bool b) { return b ? _optionSet : _optionUnset; }
            public static Color GetCorruptColor(bool b) { return b ? _btnCorruptColor : _btnRestoreColor; }
            public static string GetCorruptLabel(bool b) { return b ? _btnCorruptLabel : _btnRestoreLabel; }
        }

        private StreamWriter _logWriter;

        private bool _shuffleGFX = false;
        private bool _shuffleText = false;
        private bool _hitboxFix = false;
        private bool _shuffleFont = false;
        private bool _shuffleBG = false;
        private bool _shuffleAudio = false;
        private bool _corrupt = false;
        private bool _showSeed = false;
        private bool _friskMode = false;
        private float _truePower = 0;
        private readonly string _defaultPowerText = "0 - 255";
        private readonly string _dataWin = "data.win";

        private readonly string[] _friskSpriteHandles = { "spr_maincharal", "spr_maincharau", "spr_maincharar", "spr_maincharad", "spr_maincharau_stark", "spr_maincharar_stark", "spr_maincharal_stark", "spr_maincharad_pranked", "spr_maincharal_pranked", "spr_maincharad_umbrellafall", "spr_maincharau_umbrellafall", "spr_maincharar_umbrellafall", "spr_maincharal_umbrellafall", "spr_maincharad_umbrella", "spr_maincharau_umbrella", "spr_maincharar_umbrella", "spr_maincharal_umbrella", "spr_charad", "spr_charad_fall", "spr_charar", "spr_charar_fall", "spr_charal", "spr_charal_fall", "spr_charau", "spr_charau_fall", "spr_maincharar_shadow", "spr_maincharal_shadow", "spr_maincharau_shadow", "spr_maincharad_shadow", "spr_maincharal_tomato", "spr_maincharal_burnt", "spr_maincharal_water", "spr_maincharar_water", "spr_maincharau_water", "spr_maincharad_water", "spr_mainchara_pourwater", "spr_maincharad_b", "spr_maincharau_b", "spr_maincharar_b", "spr_maincharal_b", "spr_doorA", "spr_doorB", "spr_doorC", "spr_doorD", "spr_doorX" };

        private static readonly DateTime _unixTimeZero = new DateTime(1970, 1, 1);
        private Random _random;

        public MainForm()
        {
            InitializeComponent();

            if (!File.Exists(_dataWin))
            {
                if (File.Exists("game.ios"))
                    _dataWin = "game.ios";
                else if (File.Exists("game.unx"))
                    _dataWin = "game.unx";
            }

            if (File.Exists("DELTARUNE.exe") || File.Exists("../../SURVEY_PROGRAM.app")) { lblGameName.Text = "Deltarune"; }
            else if (File.Exists("UNDERTALE.exe") || File.Exists("../../UNDERTALE.app")) { lblGameName.Text = "Undertale"; }
            else
            {
                lblGameName.Text = GetGame().Replace(".exe", "");
                if (!string.IsNullOrWhiteSpace(lblGameName.Text))
                {
                    MessageBox.Show($"We couldn't find Deltarune or Undertale in this folder, if you're using this for another game then as long there is a {_dataWin} file and the game was made with GameMaker then this program should work but there are no guarantees that it will.", "HATE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("We couldn't find any game in this folder, check that this is in the right folder.");
                }
            }

            //This is so it doesn't keep starting the program over and over in case something messes up
            if ((Environment.OSVersion).Platform == PlatformID.Win32NT && Process.GetProcessesByName("HATE").Length == 1)
            {
                if (Directory.GetCurrentDirectory().Contains(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)) || Directory.GetCurrentDirectory().Contains(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)) && !IsElevated)
                {
                    DialogResult dialogResult = MessageBox.Show($"The game is in a system protected folder and we need elevated permissions in order to mess with {_dataWin}, Do you allow us to get elevated permissions (if you press no this will just close the program as we can't do anything)", "HATE", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (dialogResult == DialogResult.Yes)
                    {
                        // Restart program and run as admin
                        var exeName = Process.GetCurrentProcess().MainModule.FileName;
                        ProcessStartInfo startInfo = new ProcessStartInfo(exeName);
                        startInfo.Arguments = "true";
                        startInfo.Verb = "runas";
                        Process.Start(startInfo);
                        Close();
                    }
                    else
                    {
                        Close();
                    }
                }
            }

            _random = new Random();

            UpdateCorrupt();
        }

        public bool IsElevated
        {
            get
            {
                return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        public string GetGame()
        {
            if (File.Exists("DELTARUNE.exe")) { return $"DELTARUNE.exe"; }
            else if (File.Exists("../../SURVEY_PROGRAM.app")) { return "../../SURVEY_PROGRAM.app"; }
            else if (File.Exists("UNDERTALE.exe")) { return $"UNDERTALE.exe"; }
            else if (File.Exists("../../UNDERTALE.app")) { return "../../UNDERTALE.app"; }
            else
            {
                var files = Directory.EnumerateFiles(Directory.GetCurrentDirectory());
                foreach (string s in files)
                    if (!s.Remove(0, s.LastIndexOf("\\") + 1).Contains("HATE.exe") && s.Contains(".exe") || s.Contains(".app"))
                        return s.Remove(0, s.LastIndexOf("\\") + 1);

                return "";
            }
        }

        public string LinuxWine()
        {
            PlatformID pid = (Environment.OSVersion).Platform;
            switch (pid)
            {
                case PlatformID.Unix:
                    return "wine";
            }
            return "";
        }

        private void btnLaunch_Clicked(object sender, EventArgs e)
        {
            EnableControls(false);

            ProcessStartInfo processStartInfo = new ProcessStartInfo(LinuxWine(), arguments: GetGame())
            {
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            try
            {
                Process.Start(processStartInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            EnableControls(true);
        }

        private void button_Corrupt_Clicked(object sender, EventArgs e)
        {
            EnableControls(false);

            try { _logWriter = new StreamWriter("HATE.log", true); }
            catch (Exception) { MessageBox.Show("Could not set up the log file."); }

            if (!Setup()) { goto End; };
            //DebugListChunks(_dataWin, _logWriter);
            //Shuffle.LoadDataAndFind("STRG", _random, 0, _logWriter, _dataWin, Shuffle.ComplexShuffle(Shuffle.StringDumpAccumulator, Shuffle.SimpleShuffler, Shuffle.SimpleWriter));
            if (_hitboxFix && !HitboxFix_Func(_random, _truePower, _logWriter)) { goto End; }
            if (_shuffleGFX && !ShuffleGFX_Func(_random, _truePower, _logWriter)) { goto End; }
            if (_shuffleText && !ShuffleText_Func(_random, _truePower, _logWriter)) { goto End; }
            if (_shuffleFont && !ShuffleFont_Func(_random, _truePower, _logWriter)) { goto End; }
            if (_shuffleBG && !ShuffleBG_Func(_random, _truePower, _logWriter)) { goto End; }
            if (_shuffleAudio && !ShuffleAudio_Func(_random, _truePower, _logWriter)) { goto End; }

        End:
            _logWriter.Close();
            EnableControls(true);
        }

        public void EnableControls(bool state)
        {
            btnCorrupt.Enabled = state;
            btnLaunch.Enabled = state;
            chbShuffleText.Enabled = state;
            chbShuffleGFX.Enabled = state;
            chbHitboxFix.Enabled = state;
            chbShuffleFont.Enabled = state;
            chbShuffleBG.Enabled = state;
            chbShuffleAudio.Enabled = state;
            chbShowSeed.Enabled = state;
            label1.Enabled = state;
            label2.Enabled = state;
            txtPower.Enabled = state;
            txtSeed.Enabled = state;
        }

        public bool Setup()
        {
            _logWriter.WriteLine("-------------- Session at: " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + "\n");

            /** SEED PARSING AND RNG SETUP **/
            _friskMode = false;
            byte power = 0;
            _random = new Random();
            int timeSeed = (int)DateTime.Now.Subtract(_unixTimeZero).TotalSeconds;

            if (!byte.TryParse(txtPower.Text, out power) && _corrupt)
            {
                MessageBox.Show("Please set Power to a number between 0 and 255 and try again.");
                return false;
            }
            _truePower = (float)power / 255;

            if (txtSeed.Text.ToUpper() == "FRISK" && !File.Exists("DELTARUNE.exe"))
                _friskMode = true;

            if (_showSeed)
                txtSeed.Text = $"#{timeSeed}";
            else
                txtSeed.Text = "";

            _logWriter.WriteLine($"Time seed - {timeSeed}");
            _logWriter.WriteLine($"Power - {power}");
            _logWriter.WriteLine($"TruePower - {_truePower}");

            /** ENVIRONMENTAL CHECKS **/
            if (File.Exists("UNDERTALE.exe") && !File.Exists("./mus_barrier.ogg"))
            {
                MessageBox.Show("ERROR:\nIt seems you've either placed HATE.exe in the wrong location or are using an old version of Undertale. Solutions to both problems are given in the README.txt file included in the download.");
                return false;
            }

            if (!File.Exists(_dataWin))
            {
                MessageBox.Show($"You seem to be missing your resource file, {_dataWin}. Make sure you've placed HATE.exe in the proper location.");
                return false;
            }
            else if (!Directory.Exists("Data"))
            {
                if (!Safe.CreateDirectory("Data")) { return false; }
                if (!Safe.CopyFile(_dataWin, $"Data/{_dataWin}")) { return false; }
                if (lblGameName.Text == "Deltarune")
                {
                    if (!Safe.CopyFile("./lang/lang_en.json", "./Data/lang_en.json")) { return false; };
                    if (!Safe.CopyFile("./lang/lang_ja.json", "./Data/lang_ja.json")) { return false; };
                }
                _logWriter.WriteLine($"Finished setting up the Data folder.");
            }

            if (!Safe.DeleteFile(_dataWin)) { return false; }
            _logWriter.WriteLine($"Deleted {_dataWin}.");
            if (!Safe.DeleteFile("./lang/lang_en.json")) { return false; }
            _logWriter.WriteLine($"Deleted ./lang/lang_en.json.");
            if (!Safe.DeleteFile("./lang/lang_ja.json")) { return false; }
            _logWriter.WriteLine($"Deleted ./lang/lang_ja.json.");

            if (!Safe.CopyFile($"Data/{_dataWin}", _dataWin)) { return false; }
            _logWriter.WriteLine($"Copied {_dataWin}.");
            if (!Safe.CopyFile("./Data/lang_en.json", "./lang/lang_en.json")) { return false; }
            _logWriter.WriteLine($"Copied ./lang/lang_en.json.");
            if (!Safe.CopyFile("./Data/lang_ja.json", "./lang/lang_ja.json")) { return false; }
            _logWriter.WriteLine($"Copied ./lang/lang_ja.json.");

            return true;
        }

        public void UpdateCorrupt()
        {
            _corrupt = _shuffleGFX || _shuffleText || _hitboxFix || _shuffleFont || _shuffleAudio || _shuffleBG;
            btnCorrupt.Text = Style.GetCorruptLabel(_corrupt);
            btnCorrupt.ForeColor = Style.GetCorruptColor(_corrupt);
        }

        private void chbShuffleText_CheckedChanged(object sender, EventArgs e)
        {
            _shuffleText = chbShuffleText.Checked;
            chbShuffleText.ForeColor = Style.GetOptionColor(_shuffleText);
            UpdateCorrupt();
        }

        private void chbShuffleGFX_CheckedChanged(object sender, EventArgs e)
        {
            _shuffleGFX = chbShuffleGFX.Checked;
            chbShuffleGFX.ForeColor = Style.GetOptionColor(_shuffleGFX);
            UpdateCorrupt();
        }

        private void chbHitboxFix_CheckedChanged(object sender, EventArgs e)
        {
            _hitboxFix = chbHitboxFix.Checked;
            chbHitboxFix.ForeColor = Style.GetOptionColor(_hitboxFix);
            UpdateCorrupt();
        }

        private void chbShuffleFont_CheckedChanged(object sender, EventArgs e)
        {
            _shuffleFont = chbShuffleFont.Checked;
            chbShuffleFont.ForeColor = Style.GetOptionColor(_shuffleFont);
            UpdateCorrupt();
        }

        private void chbShuffleBG_CheckedChanged(object sender, EventArgs e)
        {
            _shuffleBG = chbShuffleBG.Checked;
            chbShuffleBG.ForeColor = Style.GetOptionColor(_shuffleBG);
            UpdateCorrupt();
        }

        private void chbShuffleAudio_CheckedChanged(object sender, EventArgs e)
        {
            _shuffleAudio = chbShuffleAudio.Checked;
            chbShuffleAudio.ForeColor = Style.GetOptionColor(_shuffleAudio);
            UpdateCorrupt();
        }

        private void chbShowSeed_CheckedChanged(object sender, EventArgs e)
        {
            _showSeed = chbShowSeed.Checked;
            chbShowSeed.ForeColor = Style.GetOptionColor(_showSeed);
        }

        private void txtPower_Enter(object sender, EventArgs e)
        {
            txtPower.Text = (txtPower.Text == _defaultPowerText) ? "" : txtPower.Text;
        }

        private void txtPower_Leave(object sender, EventArgs e)
        {
            txtPower.Text = string.IsNullOrWhiteSpace(txtPower.Text) ? _defaultPowerText : txtPower.Text;
        }

    }
}
