using System;
using Conwid.Core;

namespace Demo
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            var t1 = new Transmitter();
            var t2 = new Transmitter();
            t1.Friend = t2;
            t2.Friend = t1;

            t1.PostMessage( new PingMessage() );

            MessageLoop.Instance.Run();
        }
    }
}
