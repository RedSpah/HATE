using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace HATE
{
    public partial class App : Application
    {
        public static bool NeedMessageBox { get; set; }
        //Code from https://stackoverflow.com/questions/38790802/determine-operating-system-in-net-core
        //Was needed because what we get isn't the best when using .NET Standard (only shows if we are on windows or Unix (which could be macOS or Linix))
        public static OS OperatingSystem
        {
            get
            {
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

        public enum OS
        {
            Windows,
            Linux,
            macOS,
            Unknown
        }

        public App(bool IsMessageBox)
        {
            InitializeComponent();

            if(!IsMessageBox)
                MainPage = new MainPage();
            else
                MainPage = new MessageBox();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
