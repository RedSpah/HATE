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
        public MessageButton MessageButtons { get; set; }
        public MessageResult Result { get; set; }

        public MessageBox ()
		{
			InitializeComponent();
		}

        public enum MessageResult
        {
            Yes,
            No,
            Ok
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