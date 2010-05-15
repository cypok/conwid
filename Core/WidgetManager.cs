using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

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

        private IEnumerable<Widget> LowerWidgets(Widget w)
        {
            return widgets.FindAll(x => widgets.IndexOf(x) > widgets.IndexOf(w));
        }
        private IEnumerable<Widget> UpperWidgets(Widget w)
        {
            return widgets.FindAll(x => widgets.IndexOf(x) < widgets.IndexOf(w));
        }
        
        // Handles:
        // * AddWidgetMessage
        // * RemoveWidgetMessage
        // * RedrawWidgetMessage
        // * SwitchWidgetMessage
        public void Handle(IMessage msg)
        {
            if(msg is WidgetManipulationMessage)
            {
                var w = (msg as WidgetManipulationMessage).Widget;
                
                
                if(msg is AddWidgetMessage)
                {
                    if(widgets.Contains(w))
                        throw new InvalidOperationException("Widget already added to WidgetManager");
                    widgets.Insert(0, w);
                }
                else if(msg is RemoveWidgetMessage)
                {
                    widgets.Remove(w);
                }
                else if(msg is RedrawWidgetMessage)
                {
                    if(!widgets.Contains(w))
                        throw new InvalidOperationException("Widget not added to WidgetManager being redrawed");
                    w.Draw( new DrawSpace(w.Area, UpperWidgets(w).Select(x => x.Area)) );
                }
            }
            else if (msg is SwitchWidgetMessage)
            {
                var top = widgets[0];
                widgets.RemoveAt(0);
                widgets.Add(top); // move top window to bottom
                foreach (var w in widgets)
                {
                    Console.Clear(); //TODO: oh oh oh!
                    this.PostMessage( new RedrawWidgetMessage(w) );
                }
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
