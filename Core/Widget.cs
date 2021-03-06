﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Conwid.Core
{
    using Messages;
    
    public abstract class Widget : UIElement
    {
        public Widget(UIElement parent, Rectangle area) : base(area)
        {
            Parent = parent;
        }

        UIManager<Widget> parent;
        public override UIElement Parent
        {
            get { return parent; }
            set
            {
                if(value == null || value is UIManager<Widget>)
                {
                    var newParent = value as UIManager<Widget>;

                    if(newParent == parent) // nothing changed
                        return;
                
                    // remove itself from previous parent
                    if(parent != null)
                        parent.SendMessage( new RemoveUIElementMessage<Widget>(this) );
                
                    parent = newParent;

                    if(newParent != null)
                    {
                        newParent.SendMessage( new AddUIElementMessage<Widget>(this) );
                        IsEnabled = true;
                    }
                    else
                    {
                        IsEnabled = false;
                    }
                }
                else
                {
                    throw new InvalidCastException("Only UIManager<Widget> can be a parent of a Widget");
                }
            }
        }
        
        public override bool IsActive
        {
            get
            {
                if(parent == null)
                    return false;
                return parent.ActiveElement == this && parent.IsActive;
            }
        }
        
        private bool isFocusable = true;
        public override bool IsFocusable
        {
            get { return isFocusable;}
            set
            {
                if(value == isFocusable)
                    return; //nothing changed

                isFocusable = value;
                Parent.SendMessage( new IsFocusableChangedMessage<Widget>(this) );
            }
        }

        protected void Emit(MulticastDelegate d, params object[] objs)
        {
            if(d != null)
                d.DynamicInvoke(objs);
        }

        public override void Invalidate(Rectangle? rect = null)
        {
            Parent.PostMessage( new InvalidateUIElementMessage<Widget>(this, rect) );
        }
    }
}

