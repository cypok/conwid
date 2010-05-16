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
        public Widget(Rectangle area) : base(area) {}

        UIManager<Widget> parent;
        public override UIElement Parent
        {
            get { return parent; }
            set
            {
                if(value is UIManager<Widget>)
                {
                    var newParent = value as UIManager<Widget>;

                    if(newParent == parent) // nothing changed
                        return;
                
                    // remove itself from previous parent
                    if(parent != null)
                        parent.SendMessage( new RemoveUIElementMessage<Widget>(this) );
                
                    parent = newParent;
                    if(parent != null)
                        parent.SendMessage( new AddUIElementMessage<Widget>(this) );
                }
                else
                {
                    throw new InvalidCastException("Only UIManager<Widget> can be a parent of a Widget");
                }
            }
        }
        
        public bool IsActive()
        {
            if(parent == null)
                return false;
            return parent.ActiveElement == this;
        }

        public override void Invalidate(Rectangle? rect = null)
        {
            Parent.PostMessage( new InvalidateUIElementMessage<Widget>(this, rect) );
        }
    }
}

