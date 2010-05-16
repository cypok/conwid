using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Conwid.Core
{
    public interface IUIElement : IMessageHandler
    {
        IUIElement Parent { get; }
        Rectangle Area{ get; }

        void Invalidate();
        void Draw(DrawSpace ds);
    }
}
