using System;

namespace Conwid.Core
{
    public static class ConsoleKeyExtensions
    {
        public static bool EqualsTo(this ConsoleKeyInfo ki1, ConsoleKeyInfo ki2)
        {
            return ki1.Key == ki2.Key && ki1.Modifiers == ki2.Modifiers;
        }
    }
}
