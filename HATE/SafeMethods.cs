using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Security;
using Optional;

namespace HATE
{
    static class Safe
    {
        private static bool IsValidPath(string path)
        {
            try { Path.GetFullPath(path); } catch (Exception) { return false; } return true;
        }

        public static Option<List<string>> GetDirectories(string dirname, string searchstring)
        {
            if (!IsValidPath(dirname) || string.IsNullOrWhiteSpace(searchstring) || !Directory.Exists(dirname) ) { return Option.None<List<string>>(); }
            List<string> output = new List<string>();
            try
            {
                output = Directory.GetDirectories(dirname, searchstring, SearchOption.AllDirectories).ToList();
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException)
                {
                    MessageBox.Show($"UnauthorizedAccessException has occured while attempting to get the list of directories in {dirname}. The directory requires permissions which this application does not have to access. Please remove permission requirement from the directory and try again.");
                }
                else if (ex is IOException)
                {
                    MessageBox.Show($"IOException has occured while attempting to get the list of directories in {dirname}. Please ensure that the directory is not in use and try again.");
                }
                else
                {
                    MessageBox.Show(ex.ToString() + $" has occured while attempting to get the list of directories in {dirname}.");
                }
                return Option.None<List<string>>();
            }
            return Option.Some(output);
        }

        public static bool CreateDirectory(string dirname)
        {
            if (!IsValidPath(dirname)) { return false; }

            try
            {
                Directory.CreateDirectory(dirname);
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException)
                {
                    MessageBox.Show($"UnauthorizedAccessException has occured while attempting to create {dirname}. Creation of the specified directory requires permissions which this application does not have. Please remove permission requirement from the parent directory and try again.");
                }
                else
                {
                    MessageBox.Show(ex.ToString() + $" has occured while attempting to create {dirname}.");
                }
                return false;
            }
            return true;
        }

        public static bool CopyFile(string from, string to)
        {
            if (!IsValidPath(from) || !IsValidPath(to) || !File.Exists(from)) { return false; }

            try
            {
                File.Copy(from, to, true);
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException)
                {
                    MessageBox.Show( $"UnauthorizedAccessException has occured while attempting to copy {from} to {to}. Please ensure that the source file doesn't require permissions to access and that destination file is not read-only.");
                }
                else if (ex is IOException)
                {
                    MessageBox.Show($"IOException has occured while attempting to copy {from} to {to}. Please ensure that the files are not in use and try again.");
                }
                else
                {
                    MessageBox.Show("Exception " + ex + $" has occured while attempting to copy {from} to {to}.");
                }
                return false;
            }
            return true;
        }

        public static Option<List<string>> GetFiles(string dirname, bool alldirs = true, string format = "*.*")
        {
            if (!IsValidPath(dirname) || !Directory.Exists(dirname)) { return Option.None<List<string>>(); }
            
            List<string> output = new List<string>();
            try
            {
                output = Directory.GetFiles(dirname, format, alldirs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).ToList();
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException)
                {
                    MessageBox.Show($"UnauthorizedAccessException has occured while attempting to get the list of files in {dirname}. The directory requires permissions which this application does not have to access. Please remove permission requirement from the directory and try again.");
                }
                else if (ex is IOException)
                {
                    MessageBox.Show($"IOException has occured while attempting to get the list of files in {dirname}. Please ensure that the directory is not in use and try again.");
                }
                else
                {
                    MessageBox.Show(ex.ToString() + $" has occured while attempting to get the list of files in {dirname}.");
                }
                return Option.None<List<string>>();
            }
            return Option.Some(output);
        }

        public static bool DeleteDirectory(string dirname)
        {
            if (!IsValidPath(dirname) || !Directory.Exists(dirname)) { return false; }

            try
            {
                Directory.Delete(dirname, true);
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException)
                {
                    MessageBox.Show(
                        $"UnauthorizedAccessException has occured while attempting to delete {dirname}. The directory requires permissions which this application does not have to access. Please remove permission requirement from the directory and try again.");
                }
                else if (ex is IOException)
                {
                    MessageBox.Show($"IOException has occured while attempting to delete {dirname}. Please ensure that the directory is not in use and doesn't contain a read-only file and try again.");
                }
                else
                {
                    MessageBox.Show(ex.ToString() + $" has occured while attempting to delete {dirname}.");
                }
                return false;
            }
            return true;
        }

        public static bool DeleteFile(string filename)
        {
            if (!IsValidPath(filename) || !File.Exists(filename)) { return false; }

            try
            {
                File.Delete(filename);
            }
            catch (Exception ex)
            {
               if (ex is UnauthorizedAccessException)
                {
                    MessageBox.Show($"UnauthorizedAccessException has occured while attempting to delete {filename}. Please ensure that the file is neither read-only, requiring permissions to access, actually a directory all along, or a executable currently in use.");
                }              
                else if (ex is IOException)
                {
                    MessageBox.Show($"IOException has occured while attempting to delete {filename}. Please ensure that the file is not in use and try again.");
                }
                else
                {
                    MessageBox.Show(ex.ToString() + $" has occured while attempting to delete {filename}.");
                }
                return false;
            }
            return true;
        }

        public static bool MoveFile(string from, string to)
        {
            if (!IsValidPath(from) || !IsValidPath(to) || !File.Exists(from)) { return false; }

            try
            {
                File.Move(from, to);
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException)
                {
                    MessageBox.Show($"UnauthorizedAccessException has occured while attempting to move {from} to {to}. Please ensure that the source file doesn't require permissions to access.");
                }               
                else if (ex is IOException)
                {
                    MessageBox.Show($"IOException has occured while attempting to move {from} to {to}. Please ensure that the destination file doesn't exist and that the source file does.");
                }
                else
                {
                    MessageBox.Show(ex.ToString() + $" has occured while attempting to move {from} to {to}.");
                }
                return false;
            }
            return true;
        }

        public static Option<StreamWriter> OpenStreamWriter(string filename)
        {
            if (!IsValidPath(filename) || !File.Exists(filename)) { return Option.None<StreamWriter>(); } 

            StreamWriter TXW;
            try
            {
                TXW = new StreamWriter(filename);
            }
            catch (Exception ex)
            {
                if (ex is SecurityException)
                {
                    MessageBox.Show($"SecurityException has occured while opening {filename} with a StreamWriter. File requires permissions to access which this program does not have.");
                }
                else if (ex is IOException)
                {
                    MessageBox.Show($"IOException has occured while opening {filename} with a StreamWriter. Please ensure that the path is correct and the file is not in use and try again.");
                }
                else if (ex is UnauthorizedAccessException)
                {
                    MessageBox.Show($"UnauthorizedAccessException has occured while opening {filename} with a StreamWriter. The file requires permissions which this program doesn't have to open. Please ensure that file at the specified address requires no permissions to open and try again.");
                }
                else
                {
                    MessageBox.Show(ex.ToString() + $" has occured when while opening {filename} with a StreamWriter.");
                }
                return Option.None<StreamWriter>();
            }
            return Option.Some(TXW); 
        }

        public static Option<FileStream> OpenFileStream(string filename)
        {
            if (!IsValidPath(filename) || !File.Exists(filename)) { return Option.None<FileStream>(); }

            FileStream TXW;
            try
            {
                TXW = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite);
            }
            catch (Exception ex)
            {
                if (ex is SecurityException)
                {
                    MessageBox.Show($"SecurityException has occured while opening {filename} with a FileStream. File requires permissions to access which this program does not have.");
                }
                else if (ex is IOException)
                {
                    MessageBox.Show($"IOException has occured while opening {filename} with a FileStream. Please ensure that the path is correct and the file isn't in use and try again.");
                }
                else if (ex is UnauthorizedAccessException)
                {
                    MessageBox.Show($"UnauthorizedAccessException has occured while opening {filename} with a FileStream. The file requires permissions which this program doesn't have to open. Please ensure that file at the specified address requires no permissions to open and try again.");
                }              
                else
                {
                    MessageBox.Show(ex.ToString() + $" has occured when while opening {filename} with a FileStream.");
                }
                return Option.None<FileStream>();
            }
            return Option.Some(TXW);
        }
    }
}