using System;
using Xamarin.Forms;
using HATE.Core;
using HATE.Core.Logging;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

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
                    Logger.Log(MessageType.Warning, $"We couldn't find Deltarune or Undertale in this folder, if you're using this for another game then as long there is a {Main._dataWin} file and the game was made with GameMaker then this program should work but there are no guarantees that it will.", true);
                }
                else
                {
                    Logger.Log(MessageType.Warning, "We couldn't find any game in this folder, check that this is in the right folder.");
                }
            }

            _random = new Random();

            UpdateCorrupt();

            //This is so it doesn't keep starting the program over and over in case something messes up
            if (App.OperatingSystem == App.OS.Windows && Process.GetProcessesByName("HATE.GTK").Length == 1)
            {
                var task = Task.Run(() => BecomeElevated()).ConfigureAwait(false);
            }
        }

        public async Task BecomeElevated()
        {
            if (Directory.GetCurrentDirectory().Contains(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)) || Directory.GetCurrentDirectory().Contains(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)) && !IsElevated)
            {
                var dialogResult = MessageBox.Show($"The game is in a system protected folder and we need elevated permissions in order to mess with {Main._dataWin}, Do you allow us to get elevated permissions (if you press no this will just close the program as we can't do anything)", MessageBox.MessageButton.YesNo, MessageBox.MessageIcon.Exclamation).ConfigureAwait(true).GetAwaiter().GetResult();
                if (dialogResult == MessageBox.MessageResult.Yes)
                {
                    //Restart program and run as admin
                    var exeName = Process.GetCurrentProcess().MainModule.FileName;
                    ProcessStartInfo startInfo = new ProcessStartInfo(exeName);
                    startInfo.Arguments = "true";
                    startInfo.Verb = "runas";
                    Process.Start(startInfo);
                    Application.Current.Quit();
                }
                else
                {
                    Application.Current.Quit();
                }
            }
        }

        private async void Logger_SecondChange(MessageEventArgs messageType)
        {
            if (messageType.MessageType == MessageType.Debug)
            {
                if(!messageType.WaitForAnything)
                    MessageBox.Show(messageType.Message, MessageBox.MessageButton.OK, MessageBox.MessageIcon.Information);
                else
                    await MessageBox.Show(messageType.Message, MessageBox.MessageButton.OK, MessageBox.MessageIcon.Information);
            }
            else if (messageType.MessageType == MessageType.Warning)
            {
                if (!messageType.WaitForAnything)
                    MessageBox.Show(messageType.Message, MessageBox.MessageButton.OK, MessageBox.MessageIcon.Exclamation);
                else
                    await MessageBox.Show(messageType.Message, MessageBox.MessageButton.OK, MessageBox.MessageIcon.Exclamation);
            }
            else if (messageType.MessageType == MessageType.Error)
            {
                if (!messageType.WaitForAnything)
                    MessageBox.Show(messageType.Message, MessageBox.MessageButton.OK, MessageBox.MessageIcon.Error);
                else
                    await MessageBox.Show(messageType.Message, MessageBox.MessageButton.OK, MessageBox.MessageIcon.Error);
            }
        }

        public bool IsElevated
        {
            get
            {
                //                This is to make sure we are running on Windows
                return App.OperatingSystem == App.OS.Windows && new System.Security.Principal.WindowsPrincipal(System.Security.Principal.WindowsIdentity.GetCurrent()).IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
        }

        public string GetGame()
        {
            if (File.Exists("DELTARUNE.exe")) { return $"DELTARUNE.exe"; }
            else if (Directory.Exists("SURVEY_PROGRAM.app")) { return "SURVEY_PROGRAM.app"; }
            else if (File.Exists("UNDERTALE.exe")) { return $"UNDERTALE.exe"; }
            else if (Directory.Exists("UNDERTALE.app")) { return "UNDERTALE.app"; }
            else
            {
                var files = Directory.EnumerateFiles(Directory.GetCurrentDirectory());
                foreach (string s in files)
                {
                    if (App.OperatingSystem == App.OS.Windows && !s.Remove(0, s.LastIndexOf("\\") + 1).Contains("HATE.exe") && s.Contains(".exe") || s.Contains(".app"))
                        return s.Remove(0, s.LastIndexOf("\\") + 1);
                    else if (s != "HATE.exe" && s.Contains(".exe") || s.Contains(".app"))
                        return s.Remove(0, s.LastIndexOf("/") + 1);
                }    
            }
            return "";
        }

        public string LinuxWine()
        {
            if (App.OperatingSystem == App.OS.Linux)
                return "wine";
            else
                return "";
        }

        private void btnLaunch_Clicked(object sender, EventArgs e)
        {
            EnableControls(false);

            ProcessStartInfo processStartInfo = null;
            if (App.OperatingSystem == App.OS.Linux)
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
                Logger.Log(MessageType.Warning ,"Please set Power to a number between 0 and 255 and try again.", true);
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
    }
}
