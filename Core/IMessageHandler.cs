using System;

namespace Conwid
{
    namespace Core
    {
        public interface IMessageHandler
        {
            void Handle(IMessage msg);
        }
    }
}
