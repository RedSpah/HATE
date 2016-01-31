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

        private void button_Corrupt_Clicked(object sender, EventArgs e)
        {
            SetInput(false);
            if (!Setup()) { goto End; };
            if (HitboxFix && !HitboxFix_Func()) { goto End; }
            if (ShuffleGFX && !ShuffleGFX_Func()) { goto End; }
            if (ShuffleText && !ShuffleText_Func()) { goto End; }
            if (ShuffleFont && !ShuffleFont_Func()) { goto End; }
            if (ShuffleBG && !ShuffleBG_Func()) { goto End; }
            if (ShuffleAudio && !ShuffleAudio_Func()) { goto End; }
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
            RNG = new Random((int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);

            if (!File.Exists("./splash.png") || Directory.Exists("./Undertale"))
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
    }
}
