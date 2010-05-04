using System;
using Conwid.Core;
using Conwid.Core.Messages;

namespace Demo
{
    class PingMessage : IMessage { }

    class Transmitter : IMessageHandler
    {
        int counter = 1000;

        public Transmitter Friend { private get; set; }

        public void Handle(IMessage msg)
        {
            if( msg is PingMessage )
            {
                Console.WriteLine("{0} received message", this.GetHashCode());

                if( counter --> 0)
                    Friend.PostMessage(msg);
                else
                    MessageLoop.Instance.PostMessage( new QuitMessage() );
            }
        }
    }
}
