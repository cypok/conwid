using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conwid.Core
{
    using Messages;

    public sealed class WidgetManager : IMessageHandler
    {
        #region Singleton implementation

        static readonly WidgetManager instance = new WidgetManager();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static WidgetManager() { }

        static public WidgetManager Instance
        {
            get { return instance; }
        }

        #endregion //Singleton implementation
        
        private List<Widget> widgets = new List<Widget>();
        
        // Handles:
        // * AddWidgetMessage
        // * RemoveWidgetMessage
        public void Handle(IMessage msg)
        {
            if(msg is      AddWidgetMessage)
            {
                var w = (msg as AddWidgetMessage).Widget;
                if(widgets.Contains(w))
                    throw new InvalidOperationException("Widget already added to WidgetManager");
                widgets.Add(w);
            }
            else if(msg is RemoveWidgetMessage)
            {
                widgets.Remove( (msg as RemoveWidgetMessage).Widget );
            }
        }

        public string DebugDump()
        {
            var s = "";
            s += "Widgets:\n";
            var strings = widgets.Select( w => String.Format(" - {0} #{1}", w, w.GetHashCode()) );
            s += String.Join("\n", strings);
            return s;
        }
    }
}
