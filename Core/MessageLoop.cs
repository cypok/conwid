using System;
using System.Collections.Generic;

namespace Conwid.Core
{
    using Messages;
    
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
        #region Singleton implementation

        static readonly MessageLoop instance = new MessageLoop();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static MessageLoop() { }

        static public MessageLoop Instance
        {
            get { return instance; }
        }
            
        #endregion //Singleton implementation

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
            
        // Handles:
        // * QuitMessage
        // * KeyPressedMessage
        public void Handle(IMessage msg)
        {
            if(msg is QuitMessage)
            {
                stopped = true;
                retcode = (msg as QuitMessage).Code;
            }
            else if (msg is KeyPressedMessage)
            {
                var key = (msg as KeyPressedMessage).Key;
                switch(key)
                {
                    case ConsoleKey.Tab:
                        WidgetManager.Instance.PostMessage(new SwitchWidgetMessage());
                        break;
                    case ConsoleKey.Escape:
                        this.PostMessage(new QuitMessage());
                        break;
                }
            }
        }

        public int Run()
        {
            stopped = false;

            do
            {
                if(Console.KeyAvailable)
                {
                    var ki = Console.ReadKey();
                    var k = ki.Key;
                    SendMessage(this, new KeyPressedMessage(k));
                }

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

