using System;

namespace Conwid
{
    namespace Core
    {
        public interface IMessageHandler
        {
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
}
