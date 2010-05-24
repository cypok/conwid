using System;

namespace Conwid.Core
{
    public interface IMessageHandler
    {
        bool IsEnabled {get;}
        void Handle(IMessage msg);
    }

    public static class MessageHandlerExtensions
    {
        public static void PostMessage(this IMessageHandler rcv, IMessage msg)
        {
            MessageLoop.Instance.PostMessage(rcv, msg);
        }

        public static void SendMessage(this IMessageHandler rcv, IMessage msg)
        {
            MessageLoop.Instance.SendMessage(rcv, msg);
        }
    }
}
