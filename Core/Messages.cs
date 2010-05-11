using System;

namespace Conwid.Core.Messages
{
    public class SystemMessage : IMessage {}
            
    public sealed class QuitMessage : SystemMessage
    {
        public int Code { get; private set; }
        public QuitMessage(int code = 0) { Code = code; }
    }

    public class WidgetManipulationMessage : SystemMessage
    {
        public Widget Widget { get; private set; }
        public WidgetManipulationMessage(Widget w) { Widget = w; }
    }
    public sealed class AddWidgetMessage : WidgetManipulationMessage
    {
        public AddWidgetMessage(Widget w) : base(w) {}
    }
    public sealed class RemoveWidgetMessage : WidgetManipulationMessage
    {
        public RemoveWidgetMessage(Widget w) : base(w) {}
    }
    public sealed class RedrawWidgetMessage : WidgetManipulationMessage
    {
        public RedrawWidgetMessage(Widget w) : base(w) {}
    }
}
