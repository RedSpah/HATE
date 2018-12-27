using System;

namespace HATE.Core.Logging
{
    //Without this website https://www.akadia.com/services/dotnet_delegates_and_events.html I wouldn't of been able to do this *The more you know* ✨
    public class Logger
    {
        public delegate void MessageHandler(MessageEventArgs messageType);
        
        public static event MessageHandler MessageHandle;

        public static void Log(MessageType messageType, string message)
        {
            MessageHandle?.Invoke(new MessageEventArgs(messageType, message));
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
        public MessageEventArgs(MessageType messageType, string message)
        {
            MessageType = messageType;
            Message = message;
        }
        public readonly MessageType MessageType;
        public readonly string Message;
    }
}
