using System;
using System.Collections.Generic;
using Conwid.Core.Messages;

namespace Conwid
{
    namespace Core
    {
        struct MesssageContainer
        {
            public IMessage message;
            public IMessageHandler receiver;

            public MesssageContainer(IMessageHandler rcv, IMessage msg)
            {
                message = msg;
                receiver = rcv;
            }
        }

        public sealed class MessageLoop : IMessageHandler
        {
            static readonly MessageLoop instance = new MessageLoop();

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static MessageLoop() { }

            static public MessageLoop Instance
            {
                get { return instance; }
            }

            Queue<MesssageContainer> queue = new Queue<MesssageContainer>();
            bool stopped;
            int retcode;

            public void PostMessage(IMessageHandler receiver, IMessage msg)
            {
                if(receiver == null)
                    return;

                queue.Enqueue( new MesssageContainer(receiver, msg) );
            }

            public void SendMessage(IMessageHandler receiver, IMessage msg)
            {
                if(receiver == null)
                    return;

                receiver.Handle(msg);
            }
            
            public void Handle(IMessage msg)
            {
                if(msg is QuitMessage)
                {
                    stopped = true;
                    retcode = (msg as QuitMessage).Code;
                }
            }

            public int Run()
            {
                stopped = false;

                do
                {
                    if(queue.Count == 0)
                        continue;

                    var mc = queue.Dequeue();
                    SendMessage(mc.receiver, mc.message);
                }
                while(!stopped);

                queue.Clear();
                return retcode;
            }
        }
    }
}
