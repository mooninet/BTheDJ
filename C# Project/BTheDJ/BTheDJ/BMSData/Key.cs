using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTheDJ.BMSData
{
    public class Key
    {
        int channelfirst;
        int channelsecond;
        string data;

        public Key(int ChannelFirst, int ChannelSecond, string Data)
        {
            channelfirst = ChannelFirst;
            channelsecond = ChannelSecond;
            data = Data;
        }
    }
}
