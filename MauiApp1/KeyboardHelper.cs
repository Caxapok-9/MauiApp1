using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1
{
    public static class KeyboardHelper
    {
        public static Keyboard WordKeyboard { get; } = Keyboard.Create(KeyboardFlags.CapitalizeWord);
    }
}
