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
        
        readonly ConsoleKeyInfo NextWidgetKeyInfo =     new ConsoleKeyInfo('_', ConsoleKey.Tab, control: true, shift: false, alt: false);
        readonly ConsoleKeyInfo PreviousWidgetKeyInfo = new ConsoleKeyInfo('_', ConsoleKey.Tab, control: true, shift: true, alt: false);
        
        private List<Widget> widgets = new List<Widget>();

        private IEnumerable<Widget> LowerWidgets(Widget w)
        {
            return widgets.FindAll(x => widgets.IndexOf(x) > widgets.IndexOf(w));
        }
        private IEnumerable<Widget> UpperWidgets(Widget w)
        {
            return widgets.FindAll(x => widgets.IndexOf(x) < widgets.IndexOf(w));
        }
        public Widget ActiveWidget
        {
            get { return widgets.FirstOrDefault(); }
        }
        
        // Handles:
        // * AddWidgetMessage
        // * RemoveWidgetMessage
        // * RedrawWidgetMessage
        // * SwitchWidgetMessage
        // * KeyPressedMessage
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
            else if(msg is KeyPressedMessage)
            {
                var keyInfo = (msg as KeyPressedMessage).KeyInfo;

                if(keyInfo.IsEqualTo(NextWidgetKeyInfo))
                {
                    this.SendMessage(new SwitchWidgetMessage(next: true));
                }
                else if(keyInfo.IsEqualTo(PreviousWidgetKeyInfo))
                {
                    this.SendMessage(new SwitchWidgetMessage(next: false));
                }
                else if(ActiveWidget != null)
                {
                    ActiveWidget.SendMessage(msg);
                }
                // else unhandled key :(
            }
            else if (msg is SwitchWidgetMessage && !widgets.IsEmpty())
            {
                if((msg as SwitchWidgetMessage).Next)
                    widgets.MoveToEnding(0);
                else
                    widgets.MoveToBeginning(widgets.Count - 1);

                // cypok
                //widgets.ForEach( w => this.PostMessage(new RedrawWidgetMessage(w)) );

                // NIA
                foreach (var w in widgets)
                    this.PostMessage( new RedrawWidgetMessage(w) );

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
