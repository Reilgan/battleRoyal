using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace battleRoyalServer.Common
{
    public class PlayerTemlateEventArgs : EventArgs
    {
        public Dictionary<string, object> PlayerTemplate { get; set; }

        public PlayerTemlateEventArgs(Dictionary<string, object> playerTemplate)
        {
            PlayerTemplate = playerTemplate;
        }
    }
}
