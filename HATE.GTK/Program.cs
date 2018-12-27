using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;

namespace HATE.GTK
{
    public class Program
    {
        static App _app = null;

        [STAThread]
        public static void Main(string[] args)
        {
            Gtk.Application.Init();
            Forms.Init();

            _app = new App();
            MessageBoxTask();
            LoadWindow(_app);
            Gtk.Application.Run();
        }

        public static async Task LoadWindow(App app)
        {
            var window = new FormsWindow();
            window.LoadApplication(app);
            window.SetApplicationTitle("HATE");
            window.SetApplicationIcon("hateicon.png");
            window.WidthRequest = 205;
            window.DefaultWidth = window.WidthRequest;
            window.HeightRequest = 500;
            window.DefaultHeight = window.HeightRequest;
            window.AllowGrow = false;
            window.AllowShrink = false;
            window.Show();
        }

        public static async Task MessageBoxTask()
        {
            while (true)
            {
                while (!App.NeedMessageBox)
                {
                    await Task.Delay(250);
                }
                App app = new App();
                app.IsMessageBox = true;
                LoadWindow(app);
            }
        }
    }
}
