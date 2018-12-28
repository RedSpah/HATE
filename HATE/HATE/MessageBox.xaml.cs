using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HATE
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessageBox : ContentPage
    {
        private static MessageResult _Result { get; set; }
        private static MessageIcon _Icon { get; set; }
        private static MessageButton _Buttons { get; set; }
        public static string _Title { get; set; }
        public static string _Message { get; set; }
        public MessageResult Result { get; set; }
        public static MessageBox _MessageBox { get; set; }

        public MessageBox()
        {
            InitializeComponent();
            Setup();
        }

        public async void Setup()
        {
            labMessage.Text = _Message;
            if (_Buttons == MessageButton.AbortRetryIgnore)
            {
                butAbort.IsVisible = true;
                butRetry.IsVisible = true;
                butIgnore.IsVisible = true;
            }
            else if (_Buttons == MessageButton.OK)
            {
                butOK.IsVisible = true;
            }
            else if (_Buttons == MessageButton.OKCancel)
            {
                butOK.IsVisible = true;
                butCancel.IsVisible = true;
            }
            else if (_Buttons == MessageButton.RetryCancel)
            {
                butRetry.IsVisible = true;
                butCancel.IsVisible = true;
            }
            else if (_Buttons == MessageButton.YesNo)
            {
                butYes.IsVisible = true;
                butNo.IsVisible = true;
            }
            else if (_Buttons == MessageButton.YesNoCancel)
            {
                butYes.IsVisible = true;
                butNo.IsVisible = true;
                butCancel.IsVisible = true;
            }
            //if ((int)_Icon == 64)
            //{
            //    imgIcon.Source = ImageSource.FromFile("Assets/alert-circle-outline.png");
            //}
            _MessageBox = this;
        }

        public async Task _Show(string Message, MessageButton MessageButton, MessageIcon MessageIcon, string Title) 
        {
            _Title = Title;
            _Message = Message;
            _Icon = MessageIcon;
            _Buttons = MessageButton;
            Setup();
            App.NeedMessageBox = true;
            while (App.NeedMessageBox)
            {
                await Task.Delay(250);
            }
        }

        public static async Task Show(string Message, MessageIcon MessageIcon, MessageButton MessageButton = MessageButton.OKCancel, string Title = "HATE")
        {
            if (_MessageBox == null)
                _MessageBox = new MessageBox();

            _MessageBox._Show(Message, MessageButton, MessageIcon, Title);
        }

        public static async Task<MessageResult> Show(string Message, MessageButton MessageButton, MessageIcon MessageIcon, string Title = "HATE")
        {
            if (_MessageBox == null)
                _MessageBox = new MessageBox();

            await _MessageBox._Show(Message, MessageButton, MessageIcon, Title);
            return _Result;
        }

        public void SomeMagicalThing(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            switch (button.Text)
            {
                case "Abort":
                    _Result = MessageResult.Abort;
                    break;
                case "Cancel":
                    _Result = MessageResult.Cancel;
                    break;
                case "Ignore":
                    _Result = MessageResult.Ignore;
                    break;
                case "No":
                    _Result = MessageResult.No;
                    break;
                case "OK":
                    _Result = MessageResult.OK;
                    break;
                case "Retry":
                    _Result = MessageResult.Retry;
                    break;
                case "Yes":
                    _Result = MessageResult.Yes;
                    break;
            }
            Result = _Result;

            App.NeedMessageBox = false;
        }

        public enum MessageResult
        {
            Abort = 3,
            Cancel = 2,
            Ignore = 5,
            No = 7,
            None = 0,
            OK = 1,
            Retry = 4,
            Yes = 6
        }

        public enum MessageIcon 
        {
            Asterisk = 64,
            Error = 16,
            Exclamation = 48,
            Hand = 16,
            Information = 64,
            None = 0,
            Question = 32,
            Stop = 16,
            Warning = 48
        }

        public enum MessageButton
        {
            AbortRetryIgnore = 2,
            OK = 0,
            OKCancel = 1,
            RetryCancel = 5,
            YesNo = 4,
            YesNoCancel = 3
        }
    }
}