using System;

namespace Conwid.Core.Messages
{
    public class SystemMessage : IMessage {}
            
    public sealed class QuitMessage : SystemMessage
    {
        public int Code { get; private set; }
        public QuitMessage(int code = 0) { Code = code; }
    }
    public sealed class SetTitleMessage : SystemMessage
    {
        public string Title { get; private set; }
        public SetTitleMessage(string title) { Title = title; }
    }

    internal class UIElementMessage<Element> : SystemMessage  where Element : IUIElement
    {
        public Element UIElement { get; private set; }
        public UIElementMessage(Element e) { UIElement = e; }
    }
    internal sealed class AddUIElementMessage<Element> : UIElementMessage<Element>  where Element : IUIElement
    {
        public AddUIElementMessage(Element e) : base(e) {}
    }
    internal sealed class RemoveUIElementMessage<Element> : UIElementMessage<Element>  where Element : IUIElement
    {
        public RemoveUIElementMessage(Element e) : base(e) {}
    }
    internal sealed class RedrawUIElementMessage<Element> : UIElementMessage<Element>  where Element : IUIElement
    {
        public RedrawUIElementMessage(Element e) : base(e) {}
    }
    internal sealed class GlobalRedrawMessage : SystemMessage {}
    internal sealed class SwitchUIElementMessage : SystemMessage
    {
        public bool Next { get; private set; }
        public SwitchUIElementMessage(bool next = true) { Next = next; }
    }

    public sealed class KeyPressedMessage : SystemMessage
    {
        public ConsoleKeyInfo KeyInfo { get; private set; }
        public KeyPressedMessage(ConsoleKeyInfo ki) { KeyInfo = ki; }
    }
}
