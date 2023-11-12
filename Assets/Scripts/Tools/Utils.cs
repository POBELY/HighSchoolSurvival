using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Tools
{
    static class Utils
    {
        public static string ToString<KeyType,ValueType>(Dictionary<KeyType,ValueType> dico)
        {
            string res = "{";
            foreach (KeyValuePair<KeyType, ValueType> pair in dico)
            {
                res += "(" + pair.Key + "," + pair.Value + "), ";
            }
            res += "}";
            return res;
        }

    }
}
