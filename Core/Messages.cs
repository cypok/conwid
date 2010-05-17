using System;
using System.Drawing;

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

    internal class UIElementMessage<Element> : SystemMessage 
        where Element : UIElement
    {
        public Element UIElement { get; private set; }
        public UIElementMessage(Element e) { UIElement = e; }
    }
    internal sealed class AddUIElementMessage<Element> : UIElementMessage<Element>
        where Element : UIElement
    {
        public AddUIElementMessage(Element e) : base(e) {}
    }
    internal sealed class RemoveUIElementMessage<Element> : UIElementMessage<Element>
        where Element : UIElement
    {
        public RemoveUIElementMessage(Element e) : base(e) {}
    }
    internal sealed class InvalidateUIElementMessage<Element> : UIElementMessage<Element>
        where Element : UIElement
    {
        public Rectangle? Rect { get; private set; }
        public InvalidateUIElementMessage(Element e, Rectangle? rect = null) : base(e) { Rect = rect; }
    }
    internal sealed class GlobalRedrawMessage : SystemMessage
    {
        public Rectangle? Rect { get; private set; }
        public GlobalRedrawMessage(Rectangle? rect = null) { Rect = rect; }
    }
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
