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
        public static class Style
        {
            public readonly static Color OptionSet = Color.Yellow;
            public readonly static Color OptionUnset = Color.White;
            public static Color GetOptionColor(bool b) { return b ? OptionSet : OptionUnset; }
        }

        public StreamWriter LogWriter;

        public Random RNG;
        public bool ShuffleGFX = false;
        public bool ShuffleText = false;
        public bool HitboxFix = false;
        public bool ShuffleFont = false;
        public bool ShuffleBG = false;
        public bool ShuffleAudio = false;
        public bool Corrupt = false;
        public bool ShowSeed = false;
        public bool FriskMode = false;
        public float TruePower = 0;
        public string DefaultPowerText = "0 - 255";
        public string DataWin = "data.win";
        public string RunUTScript = "Undertale.bat";

        public string[] FriskSpriteHandles = { "spr_maincharal", "spr_maincharau", "spr_maincharar", "spr_maincharad", "spr_maincharau_stark", "spr_maincharar_stark", "spr_maincharal_stark", "spr_maincharad_pranked", "spr_maincharal_pranked", "spr_maincharad_umbrellafall", "spr_maincharau_umbrellafall", "spr_maincharar_umbrellafall", "spr_maincharal_umbrellafall", "spr_maincharad_umbrella", "spr_maincharau_umbrella", "spr_maincharar_umbrella", "spr_maincharal_umbrella", "spr_charad", "spr_charad_fall", "spr_charar", "spr_charar_fall", "spr_charal", "spr_charal_fall", "spr_charau", "spr_charau_fall", "spr_maincharar_shadow", "spr_maincharal_shadow", "spr_maincharau_shadow", "spr_maincharad_shadow", "spr_maincharal_tomato", "spr_maincharal_burnt", "spr_maincharal_water", "spr_maincharar_water", "spr_maincharau_water", "spr_maincharad_water", "spr_mainchara_pourwater", "spr_maincharad_b", "spr_maincharau_b", "spr_maincharar_b", "spr_maincharal_b", "spr_doorA", "spr_doorB", "spr_doorC", "spr_doorD", "spr_doorX" };

        public MainForm()
        {
            try
            {
                LogWriter = new StreamWriter("HATE.log");
            }
            catch (Exception) { MessageBox.Show("Could not set up the log file."); }

            InitializeComponent();
            UpdateCorrupt();

            if (!File.Exists("./" + DataWin))
            {
                if (File.Exists("./game.ios"))
                {
                    DataWin = "game.ios";
                }
                else if (File.Exists("./game.unx"))
                {
                    DataWin = "game.unx";
                }
            }
        }



        private void btnLaunch_Clicked(object sender, EventArgs e)
        {
            if (!File.Exists(RunUTScript))
            {
                MessageBox.Show($"ERROR: {RunUTScript} is not present in the folder. It will not be possible to launch Undertale from HATE without it present.");
                return;
            }

            SetInput(false);
            ProcessStartInfo UTScript = new ProcessStartInfo(RunUTScript, "test");
            UTScript.UseShellExecute = false;
            UTScript.RedirectStandardOutput = true;
            Process UTProcess = Process.Start(UTScript);
            string Output = UTProcess.StandardOutput.ReadToEnd();
            UTProcess.WaitForExit();
            SetInput(true);
            Console.WriteLine(Output);
        }

        private void button_Corrupt_Clicked(object sender, EventArgs e)
        {
            

           


                SetInput(false);
                if (!Setup()) { goto End; };
                if (HitboxFix && !HitboxFix_Func(RNG, TruePower)) { goto End; }
                if (ShuffleGFX && !ShuffleGFX_Func(RNG, TruePower)) { goto End; }
                if (ShuffleText && !ShuffleText_Func(RNG, TruePower)) { goto End; }
                if (ShuffleFont && !ShuffleFont_Func(RNG, TruePower)) { goto End; }
                if (ShuffleBG && !ShuffleBG_Func(RNG, TruePower)) { goto End; }
                if (ShuffleAudio && !ShuffleAudio_Func(RNG, TruePower)) { goto End; }

                End:
                LogWriter.Close();
                SetInput(true);
            
        }

        public void SetInput(bool state)
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

        public void UpdateCorrupt()
        {
            Corrupt = ShuffleGFX || ShuffleText || HitboxFix || ShuffleFont || ShuffleAudio || ShuffleBG;
            btnCorrupt.Text = Corrupt ? " -CORRUPT- " : " -RESTORE- ";
            btnCorrupt.ForeColor = Corrupt ? Color.Coral : Color.LimeGreen;
        }

        

        public bool Setup()
        {

            /** SEED PARSING AND RNG SETUP **/
            int Seed = 0;
            FriskMode = false;
            byte Power = 0;

            if (File.Exists("./" + DataWin))

            if (!byte.TryParse(txtPower.Text, out Power))
            {
                MessageBox.Show("Please set Power to a number between 0 and 255 and try again.");
                return false;
            }

            TruePower = (float)Power / 255;

            if (txtSeed.Text == "")
            {
                RNG = new Random((int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                if (ShowSeed)
                {
                    LogWriter.WriteLine("Time seed - " + ((int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString());
                    txtSeed.Text = "#" + ((int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();
                }
            }
            else if (txtSeed.Text[0] == '#' && int.TryParse(txtSeed.Text.Substring(1), out Seed))
            {
                LogWriter.WriteLine("# seed - " + Seed.ToString());
                RNG = new Random(Seed);
            }
            else if (txtSeed.Text.ToUpper() == "FRISK")
            {
                FriskMode = true;
                LogWriter.WriteLine("Time seed - " + ((int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString());
                RNG = new Random((int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
            }
            else
            {
                LogWriter.WriteLine("Text seed - " + txtSeed.Text.GetHashCode());
                RNG = new Random(txtSeed.Text.GetHashCode());
            }

            LogWriter.WriteLine("Power - " + Power);
            LogWriter.WriteLine("TruePower - " + TruePower);

            /** ENVIRONMENTAL CHECKS **/

            if (!File.Exists("./mus_barrier.ogg"))
            {
                MessageBox.Show("ERROR:\nIt seems you've either placed HATE.exe in the wrong location or are using an old version of the game. Solutions to both problems are given in the README.txt file included in the download.");
                return false;
            }

            if (!File.Exists("./" + DataWin))
            {
                MessageBox.Show($"You seem to be missing your resource file, {DataWin}. Make sure you've placed HATE.exe in the proper location.");
                return false;
            }

            else if (!Directory.Exists("./Data"))
            {
                if (!Safe.CreateDirectory("./Data"))
                {
                    return false;
                }

                if (!Safe.CopyFile("./" + DataWin, "./Data/" + DataWin))
                {
                    return false;
                }
            }

            if (!Safe.DeleteFile("./" + DataWin))
            {
                return false;

            }
            LogWriter.WriteLine("Deleted " + DataWin + ".");
            if (!Safe.CopyFile("./Data/" + DataWin, "./" + DataWin))
            {
                return false;

            }
            LogWriter.WriteLine("Copied " + DataWin + ".");
            return true;
        }



        private void chbShuffleText_CheckedChanged(object sender, EventArgs e)
        {
            ShuffleText = chbShuffleText.Checked;
            chbShuffleText.ForeColor = Style.GetOptionColor(ShuffleText);
            UpdateCorrupt();
        }

        private void chbShuffleGFX_CheckedChanged(object sender, EventArgs e)
        {
            ShuffleGFX = chbShuffleGFX.Checked;
            chbShuffleGFX.ForeColor = Style.GetOptionColor(ShuffleGFX);
            UpdateCorrupt();
        }

        private void chbHitboxFix_CheckedChanged(object sender, EventArgs e)
        {
            HitboxFix = chbHitboxFix.Checked;
            chbHitboxFix.ForeColor = Style.GetOptionColor(HitboxFix);
            UpdateCorrupt();
        }

        private void chbShuffleFont_CheckedChanged(object sender, EventArgs e)
        {
            ShuffleFont = chbShuffleFont.Checked;
            chbShuffleFont.ForeColor = Style.GetOptionColor(ShuffleFont);
            UpdateCorrupt();
        }

        private void chbShuffleBG_CheckedChanged(object sender, EventArgs e)
        {
            ShuffleBG = chbShuffleBG.Checked;
            chbShuffleBG.ForeColor = Style.GetOptionColor(ShuffleBG);
            UpdateCorrupt();
        }

        private void chbShuffleAudio_CheckedChanged(object sender, EventArgs e)
        {
            ShuffleAudio = chbShuffleAudio.Checked;
            chbShuffleAudio.ForeColor = Style.GetOptionColor(ShuffleAudio);
            UpdateCorrupt();
        }

        private void chbShowSeed_CheckedChanged(object sender, EventArgs e)
        {
            ShowSeed = chbShowSeed.Checked;
            chbShowSeed.ForeColor = Style.GetOptionColor(ShowSeed);
        }

        private void txtPower_Enter(object sender, EventArgs e)
        {
            txtPower.Text = (txtPower.Text == DefaultPowerText) ? "" : txtPower.Text;
        }

        private void txtPower_Leave(object sender, EventArgs e)
        {
            txtPower.Text = (txtPower.Text == "") ? DefaultPowerText : txtPower.Text;
        }

        private void btnBrowse_Clicked(object sender, EventArgs e)
        {

        }
    }
}
