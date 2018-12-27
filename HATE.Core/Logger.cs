using System;

namespace HATE.Core.Logging
{
    //Without this website https://www.akadia.com/services/dotnet_delegates_and_events.html I wouldn't of been able to do this *The more you know* ✨
    public class Logger
    {
        public delegate void MessageHandler(MessageEventArgs messageType);
        
        public static event MessageHandler MessageHandle;

        public static void Log(MessageType messageType, string message, bool wait = false)
        {
            MessageHandle?.Invoke(new MessageEventArgs(messageType, message, wait));
        }
    }

    public enum MessageType
    {
        Error,
        Warning,
        Debug
    }

    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(MessageType messageType, string message, bool waitForAnything)
        {
            MessageType = messageType;
            Message = message;
            WaitForAnything = waitForAnything;
        }
        public readonly MessageType MessageType;
        public readonly string Message;
        public readonly bool WaitForAnything;
    }
}
