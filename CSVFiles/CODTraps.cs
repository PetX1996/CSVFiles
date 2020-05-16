using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IWDPacker
{
    public class CODTraps
    {
        public static string ReplacePathSeps(string path)
        {
            return path.Replace('/', '\\');
        }
    }
}
