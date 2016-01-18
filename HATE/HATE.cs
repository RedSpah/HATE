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
        public const string UnpackBatchFile =
            "cd bin\n" +
            "peazip.exe -ext2folder ../Undertale.exe";

        public HATE()
        {
            InitializeComponent();

            // Console.WriteLine((int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
        }

        public static Random RNG;
        bool ShuffleGFX = false;
        bool ShuffleText = false;
        bool HitboxFix = false;
        bool ShuffleFont = false;
        bool ShuffleRooms = false;
        bool ShuffleAudio = false;

        private void BtnClicked_Go(object sender, EventArgs e)
        {
            SetInput(false);
            Setup();
            if (HitboxFix && !HitboxFix_Func()) { goto End; }
            if (ShuffleGFX && !ShuffleGFX_Func()) { goto End; }
            if (ShuffleText && !ShuffleStrings_Func()) { goto End; }
            if (ShuffleFont && !ShuffleFont_Func()) { goto End; }
            if (ShuffleRooms && !ShuffleRoom_Func()) { goto End; }
            if (ShuffleAudio && !ShuffleAudio_Func()) { goto End; }
            End:
            SetInput(true);
        }

        void SetInput(bool state)
        {
            button1.Enabled = state;
            checkBox1.Enabled = state;
            checkBox2.Enabled = state;
            checkBox3.Enabled = state;
            checkBox4.Enabled = state;
            checkBox5.Enabled = state;
            checkBox6.Enabled = state;
        }

        bool Setup()
        {
            RNG = new Random((int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
            if (!Directory.Exists("./Undertale") || !Directory.Exists("./Data"))
            {

                TextWriter unpackbatch; // creating _unpack.bat
                unpackbatch = Safe.OpenStreamWriter("./bin/_unpack.bat");

                if (unpackbatch == null)
                {
                    return false;
                }

                try
                {
                    unpackbatch.Write(UnpackBatchFile);
                }
                catch (Exception ex)
                {
                    if (ex is IOException)
                    {
                        MessageBox.Show($"IOException has occured while saving the ./bin/_unpack.bat. Please try again.");
                    }
                    else if (ex is ObjectDisposedException)
                    {
                        MessageBox.Show($"ObjectDisposedException has occured while saving the modified ./bin/_unpack.bat. Should never happen.");
                    }
                    else
                    {
                        MessageBox.Show("Exception " + ex + $" has occured while saving the modified ./bin/_unpack.bat as a text file. Should never happen.");
                    }
                    return false;
                }

                unpackbatch.Close();

                Process PR = new Process { StartInfo = new ProcessStartInfo(".\\bin\\_unpack.bat") };  // running _unpack.bat 

                try
                {
                    PR.Start();
                }
                catch (Exception ex)
                {
                    if (ex is InvalidOperationException)
                    {
                        MessageBox.Show(
                            "InvalidOperationException has occured while starting ./bin/_unpack.bat. Should never happen.");
                    }
                    else if (ex is ArgumentNullException)
                    {
                        MessageBox.Show(
                            "ArgumentNullException has occured while starting ./bin/_unpack.bat. Should never happen.");
                    }
                    else if (ex is ObjectDisposedException)
                    {
                        MessageBox.Show(
                            "ObjectDisposedException has occured while starting ./bin/_unpack.bat. Should never happen.");
                    }
                    else if (ex is FileNotFoundException)
                    {
                        MessageBox.Show(
                            "FileNotFoundException has occured while starting ./bin/_unpack.bat. Please ensure that the file ./bin/_unpack.bat exists and try again.");
                    }
                    else if (ex is Win32Exception)
                    {
                        MessageBox.Show(
                            "Win32Exception has occured while starting ./bin/_unpack.bat. Please try again.");
                    }
                    else
                    {
                        MessageBox.Show(
                            "Exception " + ex + " has occured while starting ./bin/_unpack.bat. Please try again.");
                    }
                    return false;
                }

                PR.WaitForExit();

                // Cleanup
                if (!Safe.DeleteFile("./bin/_unpack.bat"))
                {
                    return false;
                }

                if (Directory.Exists("./Data"))
                {
                    if (!Safe.DeleteDirectory("./Data"))
                    {
                        return false;
                    }
                }

                if (!Safe.CreateDirectory("./Data"))
                {
                    return false;
                }

                if (!Safe.CreateDirectory("./tmp"))
                {
                    return false;
                }

                if (!Safe.CopyFile("./Undertale/data.win", "./Data/data.win"))
                {
                    return false;
                }

                if (!Safe.MoveFile("./Undertale.exe", "./tmp/Undertale.exe"))
                {
                    return false;
                }

                if (!Safe.DeleteFile("./Undertale.exe"))
                {
                    return false;
                }

                List<string> fls = Safe.GetFiles("./Undertale");
                if (fls == null) { return false; }

                foreach (string a in fls)
                {
                    if (!Safe.MoveFile(a, a.Replace("/Undertale", ""))) { return false; }
                }

                foreach (string a in fls)
                {
                    if (!Safe.DeleteFile(a)) { return false; }
                }

                if (!Safe.MoveFile("./tmp/Undertale.exe", "./Undertale/Undertale.exe"))
                {
                    return false;
                }

                if (!Safe.DeleteDirectory("./tmp"))
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

        private void HATE_Load(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ShuffleText = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            ShuffleGFX = checkBox2.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            HitboxFix = checkBox3.Checked;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            ShuffleFont = checkBox4.Checked;
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            ShuffleRooms = checkBox5.Checked;
        }



        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            ShuffleAudio = checkBox6.Checked;
        }
    }
}
