using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KerberoShutdown
{
    public class Enums
    {
        [Flags]
        public enum PrintColor
        {
            YELLOW = 0,
            GREEN = 1,
            RED = 2
        }
    }
}
