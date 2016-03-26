using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Security;

namespace HATE
{
    static class Safe
    {
        public static List<string> GetDirectories(string dirname, string searchstring)
        {
            List<string> output = new List<string>();
            try
            {
                output = Directory.GetDirectories(dirname, searchstring, SearchOption.AllDirectories).ToList();
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException)
                {
                    MessageBox.Show(
                        $"UnauthorizedAccessException has occured while attempting to get the list of directories in {dirname}. The directory requires permissions which this application does not have to access. Please remove permission requirement from the directory and try again.");
                }
                else if (ex is ArgumentException)
                {
                    MessageBox.Show(
                        $"ArgumentException has occured while attempting to get the list of directories in {dirname}. Path of the directory is invalid. This should never happen outside of debugging.");
                }
                else if (ex is ArgumentNullException)
                {
                    MessageBox.Show(
                        $"ArgumentNullException has occured while attempting to get the list of directories in {dirname}. Path is null. This should never happen.");
                }
                else if (ex is PathTooLongException)
                {
                    MessageBox.Show(
                        $"PathTooLongException has occured while attempting to get the list of directories in {dirname}. Absolute path to the directory is too long. Please ensure that file at the specified address has a shorter absolute path and try again.");
                }
                else if (ex is DirectoryNotFoundException)
                {
                    MessageBox.Show(
                        $"DirectoryNotFoundException has occured while attempting to get the list of directories in {dirname}. Directory can not be found. Please ensure that the directory exists and try again.");
                }
                else if (ex is IOException)
                {
                    MessageBox.Show($"IOException has occured while attempting to get the list of directories in {dirname}. Please ensure that the directory is not actually a file and try again.");
                }
                else
                {
                    MessageBox.Show("Exception " + ex + $" has occured while attempting to get the list of directories in {dirname}.");
                }
                return null;
            }
            return output;
        }

        public static bool CreateDirectory(string dirname)
        {
            try
            {
                Directory.CreateDirectory(dirname);
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException)
                {
                    MessageBox.Show(
                        $"UnauthorizedAccessException has occured while attempting to create {dirname}. The directory requires permissions which this application does not have to access. Please remove permission requirement from the directory and try again.");
                }
                else if (ex is ArgumentException)
                {
                    MessageBox.Show(
                        $"ArgumentException has occured while attempting to create {dirname}. Path of the directory is invalid. This should never happen outside of debugging.");
                }
                else if (ex is ArgumentNullException)
                {
                    MessageBox.Show(
                        $"ArgumentNullException has occured while attempting to create {dirname}. Path is null. This should never happen.");
                }
                else if (ex is PathTooLongException)
                {
                    MessageBox.Show(
                        $"PathTooLongException has occured while attempting to create {dirname}. Absolute path to the directory is too long. Please ensure that file at the specified address has a shorter absolute path and try again.");
                }
                else if (ex is DirectoryNotFoundException)
                {
                    MessageBox.Show(
                        $"DirectoryNotFoundException has occured while attempting to create {dirname}. Path is invalid. Please ensure that the path is valid and try again.");
                }
                else if (ex is IOException)
                {
                    MessageBox.Show($"IOException has occured while attempting to create {dirname}. Please ensure that the path is not actually a file and try again.");
                }
                else if (ex is NotSupportedException)
                {
                    MessageBox.Show(
                        $"NotSupportedException has occured while attempting to create {dirname}. Should never happen.");
                }
                else
                {
                    MessageBox.Show("Exception " + ex + $" has occured while attempting to create {dirname}.");
                }
                return false;
            }
            return true;
        }

        public static bool CopyFile(string from, string to)
        {
            try
            {
                File.Copy(from, to, true);
            }
            catch (Exception ex)
            {
                if (ex is NotSupportedException)
                {
                    MessageBox.Show(
                        $"NotSupportedException has occured while attempting to copy {from} to {to}. Please ensure that the files are in a proper format and try again.");
                }
                else if (ex is UnauthorizedAccessException)
                {
                    MessageBox.Show(
                        $"UnauthorizedAccessException has occured while attempting to copy {from} to {to}. Please ensure that the source file doesn't require permissions to access and that destination file is not read-only.");
                }
                else if (ex is ArgumentException)
                {
                    MessageBox.Show(
                        $"ArgumentException has occured while attempting to copy {from} to {to}. Paths of the files are invalid. This should never happen outside of debugging.");
                }
                else if (ex is ArgumentNullException)
                {
                    MessageBox.Show(
                        $"ArgumentNullException has occured while attempting to copy {from} to {to}. Paths are null. This should never happen.");
                }
                else if (ex is PathTooLongException)
                {
                    MessageBox.Show(
                        $"PathTooLongException has occured while attempting to copy {from} to {to}. Absolute paths to the files are too long. Please ensure that files at the specified addresses have a shorter absolute path and try again.");
                }
                else if (ex is DirectoryNotFoundException)
                {
                    MessageBox.Show(
                        $"DirectoryNotFoundException has occured while attempting to copy {from} to {to}. Paths are invalid. Please ensure that the paths are valid and try again.");
                }
                else if (ex is IOException)
                {
                    MessageBox.Show($"IOException has occured while attempting to copy {from} to {to}. Please try again.");
                }
                else
                {
                    MessageBox.Show("Exception " + ex + $" has occured while attempting to copy {from} to {to}.");
                }
                return false;
            }
            return true;
        }

        public static List<string> GetFiles(string dirname, bool alldirs = true, string format = "*.*")
        {
            List<string> output = new List<string>();
            try
            {
                output = alldirs ? Directory.GetFiles(dirname, format, SearchOption.AllDirectories).ToList() : Directory.GetFiles(dirname, "*.*", SearchOption.TopDirectoryOnly).ToList();
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException)
                {
                    MessageBox.Show(
                        $"UnauthorizedAccessException has occured while attempting to get the list of files in {dirname}. The directory requires permissions which this application does not have to access. Please remove permission requirement from the directory and try again.");
                }
                else if (ex is ArgumentException)
                {
                    MessageBox.Show(
                        $"ArgumentException has occured while attempting to get the list of files in {dirname}. Path of the directory is invalid. This should never happen outside of debugging.");
                }
                else if (ex is ArgumentNullException)
                {
                    MessageBox.Show(
                        $"ArgumentNullException has occured while attempting to get the list of files in {dirname}. Path is null. This should never happen.");
                }
                else if (ex is PathTooLongException)
                {
                    MessageBox.Show(
                        $"PathTooLongException has occured while attempting to get the list of files in {dirname}. Absolute path to the directory is too long. Please ensure that file at the specified address has a shorter absolute path and try again.");
                }
                else if (ex is DirectoryNotFoundException)
                {
                    MessageBox.Show(
                        $"DirectoryNotFoundException has occured while attempting to get the list of files in {dirname}. Directory can not be found. Please ensure that the directory exists and try again.");
                }
                else if (ex is IOException)
                {
                    MessageBox.Show($"IOException has occured while attempting to get the list of files in {dirname}. Please ensure that the directory is not actually a file and try again.");
                }
                else
                {
                    MessageBox.Show("Exception " + ex + $" has occured while attempting to get the list of files in {dirname}.");
                }
                return null;
            }
            return output;
        }

        public static bool DeleteDirectory(string dirname)
        {
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
                else if (ex is ArgumentException)
                {
                    MessageBox.Show(
                        $"ArgumentException has occured while attempting to delete {dirname}. Path of the directory is invalid. This should never happen outside of debugging.");
                }
                else if (ex is ArgumentNullException)
                {
                    MessageBox.Show(
                        $"ArgumentNullException has occured while attempting to delete {dirname}. Path is null. This should never happen.");
                }
                else if (ex is PathTooLongException)
                {
                    MessageBox.Show(
                        $"PathTooLongException has occured while attempting to delete {dirname}. Absolute path to the directory is too long. Please ensure that file at the specified address has a shorter absolute path and try again.");
                }
                else if (ex is DirectoryNotFoundException)
                {
                    MessageBox.Show(
                        $"DirectoryNotFoundException has occured while attempting to delete {dirname}. Path is invalid. Please ensure that the path is valid and try again.");
                }
                else if (ex is IOException)
                {
                    MessageBox.Show($"IOException has occured while attempting to delete {dirname}. Please ensure that the directory is not in use, doesn't contain a read-only file, and is not actually a file and try again.");
                }
                else
                {
                    MessageBox.Show("Exception " + ex + $" has occured while attempting to delete {dirname}.");
                }
                return false;
            }
            return true;
        }

        public static bool LoadXML(XmlDocument XML, string filename)
        {
            try
            {
                XML.Load(filename);
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException)
                {
                    MessageBox.Show(
                        $"FileNotFoundException has occured while attempting to load {filename} to an XMLDocument. File could not be found. Please ensure that file at the specified address exists and try again.");
                }
                else if (ex is NotSupportedException)
                {
                    MessageBox.Show(
                        $"NotSupportedException has occured while loading {filename} to an XMLDocument. File isn't in a text format. Please ensure that file at the specified address is a text file and try again.");
                }
                else if (ex is UnauthorizedAccessException)
                {
                    MessageBox.Show(
                        $"UnauthorizedAccessException has occured while attempting to load {filename} to an XMLDocument. Please ensure that the file is neither read-only, requiring permissions to access, actually a directory all along, and that opening it is supported on a current platform.");
                }
                else if (ex is ArgumentException)
                {
                    MessageBox.Show(
                        $"ArgumentException has occured while attempting to load {filename} to an XMLDocument. Path is invalid. This should never happen outside of debugging.");
                }
                else if (ex is ArgumentNullException)
                {
                    MessageBox.Show(
                        $"ArgumentNullException has occured while attempting to load {filename} to an XMLDocument. Path is null. This should never happen.");
                }
                else if (ex is PathTooLongException)
                {
                    MessageBox.Show(
                        $"PathTooLongException has occured while attempting to load {filename} to an XMLDocument. Absolute path to the file is too long. Please ensure that the file has a shorter absolute path and try again.");
                }
                else if (ex is DirectoryNotFoundException)
                {
                    MessageBox.Show(
                        $"DirectoryNotFoundException has occured while attempting to load {filename} to an XMLDocument. Path is invalid. Please ensure that the path is valid and try again.");
                }
                else if (ex is IOException)
                {
                    MessageBox.Show($"IOException has occured while reading from {filename} to an XMLDocument. Please try again.");
                }
                else if (ex is OutOfMemoryException)
                {
                    MessageBox.Show(
                        $"OutOfMemoryException has occured while reading from {filename} to an XMLDocument. Please ensure that the system has enough free memory left and try again.");
                }
                else if (ex is XmlException)
                {
                    MessageBox.Show(
                        $"XmlException has occured while parsing {filename} to an XMLDocument. Please ensure that the file has correct XML formatting and try again.");
                }
                else if (ex is SecurityException)
                {
                    MessageBox.Show(
                        $"SecurityException has occured while attempting to load {filename} to an XMLDocument. The file requires permissions which this program doesn't have to open. Please ensure that file at the specified path requires no permissions to open and try again.");
                }
                else
                {
                    MessageBox.Show("Exception " + ex + $" has occured while reading from {filename} to an XMLDocument.");
                }
                return false;
            }
            return true;
        }

        public static bool DeleteFile(string filename)
        {
            try
            {
                File.Delete(filename);
            }
            catch (Exception ex)
            {
                if (ex is NotSupportedException)
                {
                    MessageBox.Show(
                        $"NotSupportedException has occured while attempting to delete {filename}. Please ensure that file at the specified address is in a proper format and try again.");
                }
                else if (ex is UnauthorizedAccessException)
                {
                    MessageBox.Show(
                        $"UnauthorizedAccessException has occured while attempting to delete {filename}. Please ensure that the file is neither read-only, requiring permissions to access, actually a directory all along, or a executable currently in use.");
                }
                else if (ex is ArgumentException)
                {
                    MessageBox.Show(
                        $"ArgumentException has occured while attempting to delete {filename}. Path of the file is invalid. This should never happen outside of debugging.");
                }
                else if (ex is ArgumentNullException)
                {
                    MessageBox.Show(
                        $"ArgumentNullException has occured while attempting to delete {filename}. Path is null. This should never happen.");
                }
                else if (ex is PathTooLongException)
                {
                    MessageBox.Show(
                        $"PathTooLongException has occured while attempting to delete {filename}. Absolute path to the file is too long. Please ensure that file at the specified address has a shorter absolute path and try again.");
                }
                else if (ex is DirectoryNotFoundException)
                {
                    MessageBox.Show(
                        $"DirectoryNotFoundException has occured while attempting to delete {filename}. Path is invalid. Please ensure that the path is valid and try again.");
                }
                else if (ex is IOException)
                {
                    MessageBox.Show($"IOException has occured while attempting to delete {filename}. Please ensure that the file is not in use and try again.");
                }
                else
                {
                    MessageBox.Show("Exception " + ex + $" has occured while attempting to delete {filename}.");
                }
                return false;
            }
            return true;
        }

        public static bool MoveFile(string from, string to)
        {
            try
            {
                File.Move(from, to);
            }
            catch (Exception ex)
            {
                if (ex is NotSupportedException)
                {
                    MessageBox.Show(
                        $"NotSupportedException has occured while attempting to move {from} to {to}. Please ensure that the source file is in a proper format and try again.");
                }
                else if (ex is UnauthorizedAccessException)
                {
                    MessageBox.Show(
                        $"UnauthorizedAccessException has occured while attempting to move {from} to {to}. Please ensure that the source file doesn't require permissions to access.");
                }
                else if (ex is ArgumentException)
                {
                    MessageBox.Show(
                        $"ArgumentException has occured while attempting to move {from} to {to}. Paths of the files are invalid. This should never happen outside of debugging.");
                }
                else if (ex is ArgumentNullException)
                {
                    MessageBox.Show(
                        $"ArgumentNullException has occured while attempting to move {from} to {to}. Paths are null. This should never happen.");
                }
                else if (ex is PathTooLongException)
                {
                    MessageBox.Show(
                        $"PathTooLongException has occured while attempting to move {from} to {to}. Absolute paths to the files are too long. Please ensure that files at the specified addresses have a shorter absolute path and try again.");
                }
                else if (ex is DirectoryNotFoundException)
                {
                    MessageBox.Show(
                        $"DirectoryNotFoundException has occured while attempting to move {from} to {to}. Paths are invalid. Please ensure that the paths are valid and try again.");
                }
                else if (ex is IOException)
                {
                    MessageBox.Show($"IOException has occured while attempting to move {from} to {to}. Please ensure that the destination file doesn't exist and that the source file does.");
                }
                else
                {
                    MessageBox.Show("Exception " + ex + $" has occured while attempting to move {from} to {to}.");
                }
                return false;
            }
            return true;
        }

        public static TextReader OpenTextReader(string filename)
        {
            TextReader TX;
            try
            {
                TX = File.OpenText(filename);
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException)
                {
                    MessageBox.Show(
                        $"FileNotFoundException has occured while opening {filename} with a TextReader. File could not be found. Please ensure that file at the specified address exists and try again.");
                }
                else if (ex is NotSupportedException)
                {
                    MessageBox.Show(
                        $"NotSupportedException has occured while opening {filename} with a TextReader. The file isn't in a text format. Please ensure that file at the specified address is a text file and try again.");
                }
                else if (ex is UnauthorizedAccessException)
                {
                    MessageBox.Show(
                        $"UnauthorizedAccessException has occured while opening {filename} with a TextReader. The file requires permissions which this program doesn't have to open. Please ensure that file at the specified address requires no permissions to open and try again.");
                }
                else if (ex is ArgumentException)
                {
                    MessageBox.Show(
                        $"ArgumentException has occured while opening {filename} with a TextReader. Path is invalid. This should never happen outside of debugging.");
                }
                else if (ex is ArgumentNullException)
                {
                    MessageBox.Show(
                        $"ArgumentNullException has occured while opening {filename} with a TextReader. Path is null. This should never happen.");
                }
                else if (ex is PathTooLongException)
                {
                    MessageBox.Show(
                        $"PathTooLongException has occured while opening {filename} with a TextReader. Absolute path to the file is too long. Please ensure that file at the specified address has a shorter absolute path and try again.");
                }
                else if (ex is DirectoryNotFoundException)
                {
                    MessageBox.Show(
                        $"DirectoryNotFoundException has occured while opening {filename} with a TextReader. Path is invalid. Please ensure that the path is valid and try again.");
                }
                else
                {
                    MessageBox.Show("Exception " + ex + $" has occured when while opening {filename} with a TextReader.");
                }
                return null;
            }
            return TX;
        }

        public static StreamWriter OpenStreamWriter(string filename)
        {
            StreamWriter TXW;
            try
            {
                TXW = new StreamWriter(filename);
            }
            catch (Exception ex)
            {
                if (ex is SecurityException)
                {
                    MessageBox.Show(
                        $"SecurityException has occured while opening {filename} with a StreamWriter. File requires permissions to access which this program does not have.");
                }
                else if (ex is IOException)
                {
                    MessageBox.Show(
                        $"IOException has occured while opening {filename} with a StreamWriter. Please ensure that the path is correct and try again.");
                }
                else if (ex is UnauthorizedAccessException)
                {
                    MessageBox.Show(
                        $"UnauthorizedAccessException has occured while opening {filename} with a StreamWriter. The file requires permissions which this program doesn't have to open. Please ensure that file at the specified address requires no permissions to open and try again.");
                }
                else if (ex is ArgumentException)
                {
                    MessageBox.Show(
                        $"ArgumentException has occured while opening {filename} with a StreamWriter. Path is invalid. This should never happen outside of debugging.");
                }
                else if (ex is ArgumentNullException)
                {
                    MessageBox.Show(
                        $"ArgumentNullException has occured while opening {filename} with a StreamWriter. Path is null. This should never happen.");
                }
                else if (ex is PathTooLongException)
                {
                    MessageBox.Show(
                        $"PathTooLongException has occured while opening {filename} with a StreamWriter. Absolute path to the file is too long. Please ensure that the specified address has a shorter absolute path and try again.");
                }
                else if (ex is DirectoryNotFoundException)
                {
                    MessageBox.Show(
                        $"DirectoryNotFoundException has occured while opening {filename} with a StreamWriter. Path is invalid. Please ensure that the path is valid and try again.");
                }
                else
                {
                    MessageBox.Show("Exception " + ex + $" has occured when while opening {filename} with a StreamWriter.");
                }
                return null;
            }
            return TXW;
        }

        public static FileStream OpenFileStream(string filename)
        {
            FileStream TXW;
            try
            {
                TXW = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite);
            }
            catch (Exception ex)
            {
                if (ex is SecurityException)
                {
                    MessageBox.Show(
                        $"SecurityException has occured while opening {filename} with a FileStream. File requires permissions to access which this program does not have.");
                }
                else if (ex is IOException)
                {
                    MessageBox.Show(
                        $"IOException has occured while opening {filename} with a FileStream. Please ensure that the path is correct and try again.");
                }
                else if (ex is UnauthorizedAccessException)
                {
                    MessageBox.Show(
                        $"UnauthorizedAccessException has occured while opening {filename} with a FileStream. The file requires permissions which this program doesn't have to open. Please ensure that file at the specified address requires no permissions to open and try again.");
                }
                else if (ex is ArgumentException)
                {
                    MessageBox.Show(
                        $"ArgumentException has occured while opening {filename} with a FileStream. Path is invalid. This should never happen outside of debugging.");
                }
                else if (ex is ArgumentNullException)
                {
                    MessageBox.Show(
                        $"ArgumentNullException has occured while opening {filename} with a FileStream. Path is null. This should never happen.");
                }
                else if (ex is PathTooLongException)
                {
                    MessageBox.Show(
                        $"PathTooLongException has occured while opening {filename} with a FileStream. Absolute path to the file is too long. Please ensure that the specified address has a shorter absolute path and try again.");
                }
                else if (ex is DirectoryNotFoundException)
                {
                    MessageBox.Show(
                        $"DirectoryNotFoundException has occured while opening {filename} with a FileStream. Path is invalid. Please ensure that the path is valid and try again.");
                }
                else
                {
                    MessageBox.Show("Exception " + ex + $" has occured when while opening {filename} with a FileStream.");
                }
                return null;
            }
            return TXW;
        }
    }
}