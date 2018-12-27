using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;

namespace HATE.GTK
{
    public class Program
    {
        static App _app = null;
        static FormsWindow formsWindow = null;

        [STAThread]
        public static void Main(string[] args)
        {
            Gtk.Application.Init();
            Forms.Init();

            _app = new App(false);
            MessageBoxTask();
            LoadWindow(_app);
            Gtk.Application.Run();
        }

        public static async Task<FormsWindow> LoadWindow(App app, int Width = 220, int Height = 485)
        {
            var window = new FormsWindow();
            window.LoadApplication(app);
            window.SetApplicationTitle("HATE");
            window.SetApplicationIcon("hateicon.png");

            if(App.OperatingSystem == App.OS.Linux)
                window.WidthRequest = Width;
            else if (App.OperatingSystem == App.OS.Windows)
                window.WidthRequest = Width - 15;
            window.DefaultWidth = window.WidthRequest;

            if(App.OperatingSystem == App.OS.Linux)
                window.HeightRequest = Height;
            else if (App.OperatingSystem == App.OS.Windows)
                window.HeightRequest = Height + 15;
            window.DefaultHeight = window.HeightRequest;

            window.AllowGrow = false;
            window.AllowShrink = false;
            window.Show();
            return window;
        }

        public static async Task MessageBoxTask()
        {
            while (true)
            {
                try
                {
                    while (!App.NeedMessageBox)
                    {
                        await Task.Delay(250);
                    }
                    if (formsWindow == null)
                    {
                        App app = new App(true);
                        formsWindow = await LoadWindow(app, 585, 150);
                        formsWindow.DestroyWithParent = true;
                    }
                    formsWindow.Show();
                    if (!string.IsNullOrWhiteSpace(MessageBox._Title))
                        formsWindow.SetApplicationTitle(MessageBox._Title);
                    while (App.NeedMessageBox)
                    {
                        await Task.Delay(250);
                    }
                    formsWindow.Destroy();
                }
                catch
                {
                    formsWindow.Destroy();
                }
            }
        }
    }
}
