using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Optional;

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
        private readonly string _runUtScript = "Undertale.bat";

        private readonly string[] _friskSpriteHandles = { "spr_maincharal", "spr_maincharau", "spr_maincharar", "spr_maincharad", "spr_maincharau_stark", "spr_maincharar_stark", "spr_maincharal_stark", "spr_maincharad_pranked", "spr_maincharal_pranked", "spr_maincharad_umbrellafall", "spr_maincharau_umbrellafall", "spr_maincharar_umbrellafall", "spr_maincharal_umbrellafall", "spr_maincharad_umbrella", "spr_maincharau_umbrella", "spr_maincharar_umbrella", "spr_maincharal_umbrella", "spr_charad", "spr_charad_fall", "spr_charar", "spr_charar_fall", "spr_charal", "spr_charal_fall", "spr_charau", "spr_charau_fall", "spr_maincharar_shadow", "spr_maincharal_shadow", "spr_maincharau_shadow", "spr_maincharad_shadow", "spr_maincharal_tomato", "spr_maincharal_burnt", "spr_maincharal_water", "spr_maincharar_water", "spr_maincharau_water", "spr_maincharad_water", "spr_mainchara_pourwater", "spr_maincharad_b", "spr_maincharau_b", "spr_maincharar_b", "spr_maincharal_b", "spr_doorA", "spr_doorB", "spr_doorC", "spr_doorD", "spr_doorX" };

        private static readonly DateTime _unixTimeZero = new DateTime(1970, 1, 1);
        private Random _random;

        public MainForm()
        {
            InitializeComponent();

            

            _random = new Random();
            
            UpdateCorrupt();

            if (!File.Exists("./" + _dataWin))
            {
                if (File.Exists("./game.ios"))
                {
                    _dataWin = "game.ios";
                }
                else if (File.Exists("./game.unx"))
                {
                    _dataWin = "game.unx";
                }
            }
        }



        private void btnLaunch_Clicked(object sender, EventArgs e)
        {
            if (!File.Exists(_runUtScript))
            {
                MessageBox.Show($"ERROR: {_runUtScript} is not present in the folder. It will not be possible to launch Undertale from HATE without it present.");
                return;
            }

            EnableControls(false);

            ProcessStartInfo processStartInfo = new ProcessStartInfo(_runUtScript, "test")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            string output;
            using (Process scriptProcess = Process.Start(processStartInfo))
            {
                output = scriptProcess.StandardOutput.ReadToEnd();
                scriptProcess.WaitForExit();
            }
                    
            EnableControls(true);
            _logWriter.WriteLine(output);
        }

        private void button_Corrupt_Clicked(object sender, EventArgs e)
        {
            EnableControls(false);

            try
            {
                _logWriter = new StreamWriter("HATE.log", true);
            }
            catch (Exception) { MessageBox.Show("Could not set up the log file."); }

            if (!Setup()) { goto End; };
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

            if (!byte.TryParse(txtPower.Text, out power))
            {
                MessageBox.Show("Please set Power to a number between 0 and 255 and try again.");
                return false;
            }

            _truePower = (float)power / 255;

            int timeSeed = 0;
            string seed = txtSeed.Text.Trim();
            bool textSeed = false;

            if (seed == "")
            {
                timeSeed = (int)DateTime.Now.Subtract(_unixTimeZero).TotalSeconds;

                if (_showSeed)
                {
                    txtSeed.Text = $"#{timeSeed}";
                }
            }
            else if (txtSeed.Text[0] == '#' && int.TryParse(txtSeed.Text.Substring(1), out int tmpSeed))
            {
                timeSeed = tmpSeed;
                _logWriter.WriteLine("# seed - " + tmpSeed);
            }
            else if (txtSeed.Text.ToUpper() == "FRISK")
            {
                _friskMode = true;
            }
            else
            {
                _logWriter.WriteLine("Text seed - " + txtSeed.Text.GetHashCode());
                _random = new Random(txtSeed.Text.GetHashCode());
                textSeed = true;
            }

            if (!textSeed)
            {
                _random = new Random(timeSeed);
                _logWriter.WriteLine("Time seed - " + timeSeed);
                _logWriter.WriteLine("Power - " + power);
                _logWriter.WriteLine("TruePower - " + _truePower);
            }   

            /** ENVIRONMENTAL CHECKS **/

            if (!File.Exists("./mus_barrier.ogg"))
            {
                MessageBox.Show("ERROR:\nIt seems you've either placed HATE.exe in the wrong location or are using an old version of the game. Solutions to both problems are given in the README.txt file included in the download.");
                return false;
            }

            if (!File.Exists("./" + _dataWin))
            {
                MessageBox.Show($"You seem to be missing your resource file, {_dataWin}. Make sure you've placed HATE.exe in the proper location.");
                return false;
            }

            else if (!Directory.Exists("./Data"))
            {
                if (!Safe.CreateDirectory("./Data")) { return false; }
                if (!Safe.CopyFile("./" + _dataWin, "./Data/" + _dataWin)) { return false; }
            }

            if (!Safe.DeleteFile("./" + _dataWin)) { return false; }
            _logWriter.WriteLine("Deleted " + _dataWin + ".");
            if (!Safe.CopyFile("./Data/" + _dataWin, "./" + _dataWin)) { return false; }
            _logWriter.WriteLine("Copied " + _dataWin + ".");
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
            txtPower.Text = (txtPower.Text == "") ? _defaultPowerText : txtPower.Text;
        }

        private void btnBrowse_Clicked(object sender, EventArgs e)
        {

        }
    }
}
