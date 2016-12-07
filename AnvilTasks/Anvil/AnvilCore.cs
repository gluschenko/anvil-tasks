//Anvil.Net v 0.1a (21.03.2015)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anvil
{
    public class AnvilCore
    {
        public static void Init() 
        {
            UI.Start();
            DataPrefs.Start();
        }
    }
}
