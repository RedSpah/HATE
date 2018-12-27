using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HATE
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MessageBox : ContentPage
	{
        private static MessageResult _Result { get; set; }
        public MessageButton MessageButtons { get; set; }
        public MessageResult Result { get; set; }
        public MessageIcon MessageIcons { get; set; }

        public MessageBox ()
		{
			InitializeComponent();
		}

        private static void _Show() 
        {
            App.NeedMessageBox = true;
        }

        public static void Show(string Title, string Message, MessageIcon MessageIcon, MessageButton MessageButton = MessageButton.Ok)
        {
            _Show();
        }

        public static MessageResult Show(string Title, string Message, MessageButton MessageButton, MessageIcon MessageIcon)
        {
            _Show();
            return _Result;
        }

        public void SomeMagicalThing(object sender, EventArgs e)
        {
            Application.Current.Quit();
        }

        public enum MessageResult
        {
            Yes,
            No,
            Ok
        }

        public enum MessageIcon 
        {
            Warning
        }

        public enum MessageButton
        {
            Yes,
            No,
            YesAndNo,
            Ok
        }
    }
}