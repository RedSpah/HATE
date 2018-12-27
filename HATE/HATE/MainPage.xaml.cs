using System;
using Xamarin.Forms;
using HATE.Core;
using HATE.Core.Logging;
using System.IO;
using System.Diagnostics;

namespace HATE
{
    public partial class MainPage : ContentPage
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

        //Code from https://stackoverflow.com/questions/38790802/determine-operating-system-in-net-core
        //Was needed because with what we get isn't the best (only shows if we are on windows in Unix which could be macOS or Linix)
        private OS OperatingSystem
        {
            get {
                string windir = Environment.GetEnvironmentVariable("windir");
                if (!string.IsNullOrEmpty(windir) && windir.Contains(@"\") && Directory.Exists(windir))
                {
                    return OS.Windows;
                }
                else if (File.Exists(@"/proc/sys/kernel/ostype"))
                {
                    string osType = File.ReadAllText(@"/proc/sys/kernel/ostype");
                    if (osType.StartsWith("Linux", StringComparison.OrdinalIgnoreCase))
                    {
                        // Note: Android gets here too
                        return OS.Linux;
                    }
                    else
                    {
                        return OS.Unknown;
                    }
                }
                else if (File.Exists(@"/System/Library/CoreServices/SystemVersion.plist"))
                {
                    // Note: iOS gets here too
                    return OS.macOS;
                }
                else
                {
                    return OS.Unknown;
                }
            }
        }
        private bool _shuffleGFX = false;
        private bool _shuffleText = false;
        private bool _hitboxFix = false;
        private bool _shuffleFont = false;
        private bool _shuffleBG = false;
        private bool _shuffleAudio = false;
        private bool _garbleText = false;
        private bool _corrupt = false;
        private bool _showSeed = false;
        private float _truePower = 0;
        private readonly string _defaultPowerText = "0 - 255";

        private readonly string[] _friskSpriteHandles = { "spr_maincharal", "spr_maincharau", "spr_maincharar", "spr_maincharad", "spr_maincharau_stark", "spr_maincharar_stark", "spr_maincharal_stark", "spr_maincharad_pranked", "spr_maincharal_pranked", "spr_maincharad_umbrellafall", "spr_maincharau_umbrellafall", "spr_maincharar_umbrellafall", "spr_maincharal_umbrellafall", "spr_maincharad_umbrella", "spr_maincharau_umbrella", "spr_maincharar_umbrella", "spr_maincharal_umbrella", "spr_charad", "spr_charad_fall", "spr_charar", "spr_charar_fall", "spr_charal", "spr_charal_fall", "spr_charau", "spr_charau_fall", "spr_maincharar_shadow", "spr_maincharal_shadow", "spr_maincharau_shadow", "spr_maincharad_shadow", "spr_maincharal_tomato", "spr_maincharal_burnt", "spr_maincharal_water", "spr_maincharar_water", "spr_maincharau_water", "spr_maincharad_water", "spr_mainchara_pourwater", "spr_maincharad_b", "spr_maincharau_b", "spr_maincharar_b", "spr_maincharal_b", "spr_doorA", "spr_doorB", "spr_doorC", "spr_doorD", "spr_doorX" };

        private static readonly DateTime _unixTimeZero = new DateTime(1970, 1, 1);
        private Random _random;

        //TODO: Find a way to get labels to have a Click event
        public MainPage()
        {
            InitializeComponent();
            Logger.MessageHandle += Logger_SecondChange;

            if (!File.Exists(Main._dataWin))
            {
                if (File.Exists("game.ios"))
                    Main._dataWin = "game.ios";
                else if (File.Exists("game.unx"))
                    Main._dataWin = "game.unx";
            }

            if (File.Exists("DELTARUNE.exe") || File.Exists("../../SURVEY_PROGRAM.app")) { labGameName.Text = "Deltarune"; }
            else if (File.Exists("UNDERTALE.exe") || File.Exists("../../UNDERTALE.app")) { labGameName.Text = "Undertale"; }
            else
            {
                labGameName.Text = GetGame().Replace(".exe", "");
                if (!string.IsNullOrWhiteSpace(labGameName.Text))
                {
                    Logger.Log(MessageType.Warning, $"We couldn't find Deltarune or Undertale in this folder, if you're using this for another game then as long there is a {Main._dataWin} file and the game was made with GameMaker then this program should work but there are no guarantees that it will.");
                }
                else
                {
                    Logger.Log(MessageType.Warning, "We couldn't find any game in this folder, check that this is in the right folder.");
                }
            }

            //This is so it doesn't keep starting the program over and over in case something messes up
            if ((Environment.OSVersion).Platform == PlatformID.Win32NT && Process.GetProcessesByName("HATE").Length == 1)
            {
                DisplayActionSheet("Title", "cancel", "What?", "Ok").ConfigureAwait(true).GetAwaiter().GetResult();
                if (Directory.GetCurrentDirectory().Contains(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)) || Directory.GetCurrentDirectory().Contains(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)) && !IsElevated)
                {
                    DisplayActionSheet("Title","cancel","What?","Ok").ConfigureAwait(true).GetAwaiter().GetResult();
                    //DialogResult dialogResult = MessageBox.Show($"The game is in a system protected folder and we need elevated permissions in order to mess with {_dataWin}, Do you allow us to get elevated permissions (if you press no this will just close the program as we can't do anything)", "HATE", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    //if (dialogResult == DialogResult.Yes)
                    //{
                    //    // Restart program and run as admin
                    //    var exeName = Process.GetCurrentProcess().MainModule.FileName;
                    //    ProcessStartInfo startInfo = new ProcessStartInfo(exeName);
                    //    startInfo.Arguments = "true";
                    //    startInfo.Verb = "runas";
                    //    Process.Start(startInfo);
                    //    Application.Current.Quit();
                    //}
                    //else
                    //{
                    //    Application.Current.Quit();
                    //}
                }
            }

            _random = new Random();

            UpdateCorrupt();
        }

        public MessageBox.MessageResult MessageResult()
        {
            return MessageBox.MessageResult.Ok;
        }

        private void Logger_SecondChange(MessageEventArgs messageType)
        {
            //Code TBA
        }

        public bool IsElevated
        {
            get
            {
                //                This is to make sure we are running on Windows
                return OperatingSystem == OS.Windows && new System.Security.Principal.WindowsPrincipal(System.Security.Principal.WindowsIdentity.GetCurrent()).IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
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
            if (OperatingSystem == OS.Linux)
                return "wine";
            else
                return "";
        }

        private void btnLaunch_Clicked(object sender, EventArgs e)
        {
            EnableControls(false);

            ProcessStartInfo processStartInfo = null;
            if (OperatingSystem == OS.Linux)
            {
                processStartInfo = new ProcessStartInfo(LinuxWine(), arguments: GetGame())
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };
            }
            else
            {
                processStartInfo = new ProcessStartInfo(GetGame())
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };
            }

            try
            {
                Process.Start(processStartInfo);
            }
            catch (Exception ex)
            {
                Logger.Log(MessageType.Error, $"Unable to launch {labGameName.Text}");
            }

            EnableControls(true);
        }

        private void button_Corrupt_Clicked(object sender, EventArgs e)
        {
            EnableControls(false);

            try { _logWriter = new StreamWriter("HATE.log", true); }
            catch (Exception) { Logger.Log( MessageType.Error, "Could not set up the log file."); }

            if (!Setup()) { goto End; };
            //DebugListChunks(_dataWin, _logWriter);
            //Shuffle.LoadDataAndFind("STRG", _random, 0, _logWriter, _dataWin, Shuffle.ComplexShuffle(Shuffle.StringDumpAccumulator, Shuffle.SimpleShuffler, Shuffle.SimpleWriter));
            if (_hitboxFix && !Main.HitboxFix_Func(_random, _truePower, _logWriter)) { goto End; }
            if (_shuffleGFX && !Main.ShuffleGFX_Func(_random, _truePower, _logWriter)) { goto End; }
            if (_shuffleText && !Main.ShuffleText_Func(_random, _truePower, _logWriter)) { goto End; }
            if (_shuffleFont && !Main.ShuffleFont_Func(_random, _truePower, _logWriter)) { goto End; }
            if (_shuffleBG && !Main.ShuffleBG_Func(_random, _truePower, _logWriter)) { goto End; }
            if (_shuffleAudio && !Main.ShuffleAudio_Func(_random, _truePower, _logWriter)) { goto End; }

            End:
            _logWriter.Close();
            EnableControls(true);
        }

        public void EnableControls(bool state)
        {
            btnCorrupt.IsEnabled = state;
            btnLaunch.IsEnabled = state;
            chbShuffleText.IsEnabled = state;
            chbShuffleGFX.IsEnabled = state;
            chbHitboxFix.IsEnabled = state;
            chbShuffleFonts.IsEnabled = state;
            chbShuffleSprites.IsEnabled = state;
            chbShuffleAudio.IsEnabled = state;
            chbShowSeed.IsEnabled = state;
            txtPower.IsEnabled = state;
            txtSeed.IsEnabled = state;
        }

        public bool Setup()
        {
            _logWriter.WriteLine("-------------- Session at: " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + "\n");

            /** SEED PARSING AND RNG SETUP **/
            Main._friskMode = false;
            byte power = 0;
            _random = new Random();
            int timeSeed = (int)DateTime.Now.Subtract(_unixTimeZero).TotalSeconds;

            if (!byte.TryParse(txtPower.Text, out power) && _corrupt)
            {
                Logger.Log(MessageType.Warning ,"Please set Power to a number between 0 and 255 and try again.");
                return false;
            }
            _truePower = (float)power / 255;

            if (txtSeed.Text.ToUpper() == "FRISK" && !File.Exists("DELTARUNE.exe"))
                Main._friskMode = true;

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
                Logger.Log(MessageType.Error, "ERROR:\nIt seems you've either placed HATE.exe in the wrong location or are using an old version of Undertale. Solutions to both problems are given in the README.txt file included in the download.");
                return false;
            }

            if (!File.Exists(Main._dataWin))
            {
                Logger.Log(MessageType.Error, $"You seem to be missing your resource file, {Main._dataWin}. Make sure you've placed HATE.exe in the proper location.");
                return false;
            }
            else if (!Directory.Exists("Data"))
            {
                if (!Safe.CreateDirectory("Data")) { return false; }
                if (!Safe.CopyFile(Main._dataWin, Path.Combine($"Data", Main._dataWin))) { return false; }
                if (labGameName.Text == "Deltarune")
                {
                    if (!Safe.CopyFile("./lang/lang_en.json", "./Data/lang_en.json")) { return false; };
                    if (!Safe.CopyFile("./lang/lang_ja.json", "./Data/lang_ja.json")) { return false; };
                }
                _logWriter.WriteLine($"Finished setting up the Data folder.");
            }

            if (!Safe.DeleteFile(Main._dataWin)) { return false; }
            _logWriter.WriteLine($"Deleted {Main._dataWin}.");
            if (!Safe.DeleteFile("./lang/lang_en.json")) { return false; }
            _logWriter.WriteLine($"Deleted ./lang/lang_en.json.");
            if (!Safe.DeleteFile("./lang/lang_ja.json")) { return false; }
            _logWriter.WriteLine($"Deleted ./lang/lang_ja.json.");

            if (!Safe.CopyFile($"Data/{Main._dataWin}", Main._dataWin)) { return false; }
            _logWriter.WriteLine($"Copied {Main._dataWin}.");
            if (!Safe.CopyFile("./Data/lang_en.json", "./lang/lang_en.json")) { return false; }
            _logWriter.WriteLine($"Copied ./lang/lang_en.json.");
            if (!Safe.CopyFile("./Data/lang_ja.json", "./lang/lang_ja.json")) { return false; }
            _logWriter.WriteLine($"Copied ./lang/lang_ja.json.");

            return true;
        }

        public void UpdateCorrupt()
        {
            _corrupt = _shuffleGFX || _shuffleText || _hitboxFix || _shuffleFont || _shuffleAudio || _shuffleBG || _garbleText;
            btnCorrupt.Text = Style.GetCorruptLabel(_corrupt);
            btnCorrupt.TextColor = Style.GetCorruptColor(_corrupt);
            btnCorrupt.BorderColor = btnCorrupt.TextColor;
        }

        private void chbShuffleAudio_Toggled(object sender, ToggledEventArgs e)
        {
            _shuffleAudio = chbShuffleAudio.IsToggled;
            labShuffleAudio.TextColor = Style.GetOptionColor(_shuffleAudio);
            UpdateCorrupt();
        }

        private void chbShuffleGFX_Toggled(object sender, ToggledEventArgs e)
        {
            _shuffleGFX = chbShuffleGFX.IsToggled;
            labShuffleGFX.TextColor = Style.GetOptionColor(_shuffleGFX);
            UpdateCorrupt();
        }

        private void chbShuffleFonts_Toggled(object sender, ToggledEventArgs e)
        {
            _shuffleFont = chbShuffleFonts.IsToggled;
            labShuffleFonts.TextColor = Style.GetOptionColor(_shuffleFont);
            UpdateCorrupt();
        }

        private void chbHitboxFix_Toggled(object sender, ToggledEventArgs e)
        {
            _hitboxFix = chbHitboxFix.IsToggled;
            labHitboxFix.TextColor = Style.GetOptionColor(_hitboxFix);
            UpdateCorrupt();
        }

        private void chbShuffleSprites_Toggled(object sender, ToggledEventArgs e)
        {
            _shuffleBG = chbShuffleSprites.IsToggled;
            labShuffleSprites.TextColor = Style.GetOptionColor(_shuffleBG);
            UpdateCorrupt();
        }

        private void chbShuffleText_Toggled(object sender, ToggledEventArgs e)
        {
            _shuffleText = chbShuffleText.IsToggled;
            labShuffleText.TextColor = Style.GetOptionColor(_shuffleText);
            UpdateCorrupt();
        }

        private void chbGarbleText_Toggled(object sender, ToggledEventArgs e)
        {
            _garbleText = chbGarbleText.IsToggled;
            labGarbleText.TextColor = Style.GetOptionColor(_garbleText);
            UpdateCorrupt();
        }

        private void chbShowSeed_Toggled(object sender, ToggledEventArgs e)
        {
            _showSeed = chbShowSeed.IsToggled;
            labShowSeed.TextColor = Style.GetOptionColor(_showSeed);
        }

        private enum OS
        {
            Windows,
            Linux,
            macOS,
            Unknown
        }
    }
}
