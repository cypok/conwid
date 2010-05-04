using System;

namespace Conwid
{
    namespace Core
    {
        namespace Messages
        {        
            public class SystemMessage : IMessage
            {
            }
            
            public sealed class QuitMessage : SystemMessage
            {
                public int Code { get; private set; }
                public QuitMessage(int code) { Code = code; }
                public QuitMessage () { Code = 0; }
            }
        }
    }
}
