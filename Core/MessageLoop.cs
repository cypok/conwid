using System;
using System.Collections.Generic;
using System.Linq;

namespace Conwid.Core
{
    using Messages;
    using System.Drawing;
    
    internal class MesssageContainer
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

        // Don't create instances of this class directly.
        // Use MessageLoop.Instance instead.
        private MessageLoop() { }
            
        #endregion // Singleton implementation

        readonly ConsoleKeyInfo ExitKeyInfo = new ConsoleKeyInfo('_', ConsoleKey.Q, control: true, shift: false, alt: false);

        List<MesssageContainer> queue = new List<MesssageContainer>();
        UIManager<UIManager<Widget>> widgetManager = new UIManager<UIManager<Widget>>();
        bool stopped;
        int retcode;

        public UIManager<UIManager<Widget>> WidgetManager{ get { return widgetManager; } }

        public void PostMessage(IMessageHandler receiver, IMessage msg)
        {
            if(receiver == null)
                throw new ArgumentNullException("receiver");
            
            if( msg is GlobalRedrawMessage )
            {
                var new_msg = (GlobalRedrawMessage)msg;
                // Find another GlobalRedrawMessage message
                var another_mc = queue.Find( mc => mc.receiver == receiver && mc.message is GlobalRedrawMessage );
                if(another_mc != null)
                {
                    queue.Remove(another_mc);
                    var old_msg = (GlobalRedrawMessage)another_mc.message;
                    // if one of the messages has empty Rects collection, it means full invalidate,
                    // so we should leave it empty, otherwise we should concatenate collections
                    var rects = old_msg.Rects.IsEmpty() || new_msg.Rects.IsEmpty() ?
                                new Rectangle[0] : old_msg.Rects.Concat(new_msg.Rects); 
                    
                    msg = new GlobalRedrawMessage(rects);
                }
            }
            queue.PushBack( new MesssageContainer(receiver, msg) );
        }

        public void SendMessage(IMessageHandler receiver, IMessage msg)
        {
            if(receiver == null)
                throw new ArgumentNullException("receiver");

            receiver.Handle(msg);
        }

        
        // Handles:
        // * QuitMessage
        // * KeyPressedMessage
        // * SetTitleMessage
        public void Handle(IMessage msg)
        {
            if(msg is QuitMessage)
            {
                stopped = true;
                retcode = (msg as QuitMessage).Code;
            }
            else if(msg is KeyPressedMessage)
            {
                var keyInfo = (msg as KeyPressedMessage).KeyInfo;
                if( keyInfo.EqualsTo(ExitKeyInfo) )
                    this.SendMessage(new QuitMessage());
                else
                    WidgetManager.SendMessage(msg);

            }
            else if(msg is SetTitleMessage)
            {
                Console.Title = (msg as SetTitleMessage).Title;
            }
        }
        public bool IsEnabled { get { return true; } }

        public int Run()
        {
            stopped = false;
            var oldCursorVisible = Console.CursorVisible;
            Console.CursorVisible = false;

            do
            {
                if(Console.KeyAvailable)
                    SendMessage(this, new KeyPressedMessage( Console.ReadKey(intercept: true) ));

                if(queue.IsEmpty())
                    continue;

                var mc = queue.PopFront();
                if(mc.receiver.IsEnabled)
                    SendMessage(mc.receiver, mc.message);
            }
            while(!stopped);

            queue.Clear();
            Console.Clear();
            Console.CursorVisible = oldCursorVisible;
            return retcode;
        }
    }
}

