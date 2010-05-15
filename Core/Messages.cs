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

    public sealed class KeyPressedMessage : SystemMessage
    {
        public ConsoleKeyInfo KeyInfo { get; private set; }
        public KeyPressedMessage(ConsoleKeyInfo ki) { KeyInfo = ki; }
    }

    public sealed class SwitchWidgetMessage : SystemMessage
    {
        public bool Next { get; private set; }
        public SwitchWidgetMessage(bool next = true) { Next = next; }
    }
}
