using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;


namespace HATE
{
    public partial class HATE : Form
    {
        public static Random RNG;
        public static bool ShuffleGFX = false;
        public static bool ShuffleText = false;
        public static bool HitboxFix = false;
        public static bool ShuffleFont = false;
        public static bool ShuffleBG = false;
        public static bool ShuffleAudio = false;
        public static bool Corrupt = false;
        public static bool ShowSeed = false;
        public static bool FriskMode = false;
        public static float TruePower = 0;
        public static string DefaultPowerText = "0 - 255";

        public static string[] FriskSpriteHandles = { "spr_maincharal", "spr_maincharau", "spr_maincharar", "spr_maincharad", "spr_maincharau_stark", "spr_maincharar_stark", "spr_maincharal_stark", "spr_maincharad_pranked", "spr_maincharal_pranked", "spr_maincharad_umbrellafall", "spr_maincharau_umbrellafall", "spr_maincharar_umbrellafall", "spr_maincharal_umbrellafall", "spr_maincharad_umbrella", "spr_maincharau_umbrella", "spr_maincharar_umbrella", "spr_maincharal_umbrella", "spr_charad", "spr_charad_fall", "spr_charar", "spr_charar_fall", "spr_charal", "spr_charal_fall", "spr_charau", "spr_charau_fall", "spr_maincharar_shadow", "spr_maincharal_shadow", "spr_maincharau_shadow", "spr_maincharad_shadow", "spr_maincharal_tomato", "spr_maincharal_burnt", "spr_maincharal_water", "spr_maincharar_water", "spr_maincharau_water", "spr_maincharad_water", "spr_mainchara_pourwater", "spr_maincharad_b", "spr_maincharau_b", "spr_maincharar_b", "spr_maincharal_b", "spr_doorA", "spr_doorB", "spr_doorC", "spr_doorD", "spr_doorX" };

        private void button_Corrupt_Clicked(object sender, EventArgs e)
        {
            SetInput(false);
            if (!Setup()) { goto End; };
            if (HitboxFix && !HitboxFix_Func(TruePower)) { goto End; }
            if (ShuffleGFX && !ShuffleGFX_Func(TruePower)) { goto End; }
            if (ShuffleText && !ShuffleText_Func(TruePower)) { goto End; }
            if (ShuffleFont && !ShuffleFont_Func(TruePower)) { goto End; }
            if (ShuffleBG && !ShuffleBG_Func(TruePower)) { goto End; }
            if (ShuffleAudio && !ShuffleAudio_Func(TruePower)) { goto End; }
            End:
            SetInput(true);
        }

        public void SetInput(bool state)
        {
            button_Corrupt.Enabled = state;
            checkBox_ShuffleText.Enabled = state;
            checkBox_ShuffleGFX.Enabled = state;
            checkBox_HitboxFix.Enabled = state;
            checkBox_ShuffleFont.Enabled = state;
            checkBox_ShuffleBG.Enabled = state;
            checkBox_ShuffleAudio.Enabled = state;
            checkBox_ShowSeed.Enabled = state;
            label1.Enabled = state;
            label2.Enabled = state;
            textBox_Power.Enabled = state;
            textBox_Seed.Enabled = state;
        }

        public void UpdateCorrupt()
        {
            Corrupt = ShuffleGFX || ShuffleText || HitboxFix || ShuffleFont || ShuffleAudio || ShuffleBG;
            button_Corrupt.Text = Corrupt ? "💀 CORRUPT" : "⚘ RESTORE";
            button_Corrupt.ForeColor = Corrupt ? Color.Coral : Color.LimeGreen;
        }

        public HATE()
        {
            InitializeComponent();
            UpdateCorrupt();
        }

        public bool Setup()
        {
            int Seed = 0;
            FriskMode = false;
            byte Power = 0;

            if (!byte.TryParse(textBox_Power.Text, out Power))
            {
                MessageBox.Show("Please set Power to a number between 0 and 255 and try again.");
                return false;
            }

            TruePower = (float)Power / 255;

            if (textBox_Seed.Text == "")
            {
                RNG = new Random((int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
                if (ShowSeed)
                {
                    textBox_Seed.Text = "#" + ((int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();
                }
            }
            else if (textBox_Seed.Text[0] == '#' && int.TryParse(textBox_Seed.Text.Substring(1), out Seed))
            {
                RNG = new Random(Seed);
            }
            else if (textBox_Seed.Text.ToUpper() == "FRISK")
            {
                FriskMode = true;
                RNG = new Random((int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
            }
            else
            {
                RNG = new Random(textBox_Seed.Text.GetHashCode());
            }

            if (!File.Exists("./mus_barrier.ogg"))
            {
                MessageBox.Show("ERROR:\nIt seems you're using an old version of the game. Please force Steam to update it by wiping the Undertale folder clean and try again.");
                return false;
            }

            else if (!Directory.Exists("./Data"))
            {
                if (!Safe.CreateDirectory("./Data"))
                {
                    return false;
                }

                if (!Safe.CopyFile("./data.win", "./Data/data.win"))
                {
                    return false;
                }
            }

            if (!Safe.DeleteFile("./data.win"))
            {
                return false;
            }

            if (!Safe.CopyFile("./Data/data.win", "./data.win"))
            {
                return false;
            }

            return true;
        }

        private void checkBox_ShuffleText_CheckedChanged(object sender, EventArgs e)
        {
            ShuffleText = checkBox_ShuffleText.Checked;
            checkBox_ShuffleText.ForeColor = ShuffleText ? Color.Yellow : Color.White;
            UpdateCorrupt();
        }

        private void checkBox_ShuffleGFX_CheckedChanged(object sender, EventArgs e)
        {
            ShuffleGFX = checkBox_ShuffleGFX.Checked;
            checkBox_ShuffleGFX.ForeColor = ShuffleGFX ? Color.Yellow : Color.White;
            UpdateCorrupt();
        }

        private void checkBox_HitboxFix_CheckedChanged(object sender, EventArgs e)
        {
            HitboxFix = checkBox_HitboxFix.Checked;
            checkBox_HitboxFix.ForeColor = HitboxFix ? Color.Yellow : Color.White;
            UpdateCorrupt();
        }

        private void checkBox_ShuffleFont_CheckedChanged(object sender, EventArgs e)
        {
            ShuffleFont = checkBox_ShuffleFont.Checked;
            checkBox_ShuffleFont.ForeColor = ShuffleFont ? Color.Yellow : Color.White;
            UpdateCorrupt();
        }

        private void checkBox_ShuffleBG_CheckedChanged(object sender, EventArgs e)
        {
            ShuffleBG = checkBox_ShuffleBG.Checked;
            checkBox_ShuffleBG.ForeColor = ShuffleBG ? Color.Yellow : Color.White;
            UpdateCorrupt();
        }

        private void checkBox_ShuffleAudio_CheckedChanged(object sender, EventArgs e)
        {
            ShuffleAudio = checkBox_ShuffleAudio.Checked;
            checkBox_ShuffleAudio.ForeColor = ShuffleAudio ? Color.Yellow : Color.White;
            UpdateCorrupt();
        }

        private void checkBox_ShowSeed_CheckedChanged(object sender, EventArgs e)
        {
            ShowSeed = checkBox_ShowSeed.Checked;
            checkBox_ShowSeed.ForeColor = ShowSeed ? Color.Yellow : Color.White;
        }

        private void textBox_Power_Enter(object sender, EventArgs e)
        {
            textBox_Power.Text = textBox_Power.Text == DefaultPowerText ? "" : textBox_Power.Text;
        }

        private void textBox_Power_Leave(object sender, EventArgs e)
        {
            textBox_Power.Text = textBox_Power.Text == "" ? DefaultPowerText : textBox_Power.Text;
        }

    }
}
