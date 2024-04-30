using LancerUI.Controls.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LancerUI.Extensions
{
    public static class IconSymbolExtensions
    {
        public static string String(this IconSymbol symbol_)
        {
            return Encoding.Unicode.GetString(BitConverter.GetBytes((int)symbol_)).TrimEnd('\0');
        }
    }
}
