using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HealthBot
{
    public static class Phrases
    {
        internal static string[,] lookup = new string[,] {
            { "hyperten", "hypertension", "ailment" },
            { "diabet", "diabetes", "ailment" },
            { "asthm", "asthma", "ailment" },
            { "kidney", "kidney disease", "ailment" },
            { "tubercul", "tuberculosis", "ailment" },
            { "alcoh", "alcohol", "food" },
            { "beer", "alcohol", "food" },
            { "beans", "beans", "food" },
            { "citrus", "citrus fruits", "food" },
            { "milk", "milk", "food" },
            { "pepper", "pepper", "food" },
            { "plantain", "plantain", "food" },
            { "rice", "rice", "food" },
            { "salt", "salt", "food" },
            { "vegetables", "vegetables", "food" },
            { "yam", "yam", "food" },
            };

        internal static List<string> lookuplist()
        {
            List<string> l = new List<string>();
            for (int i = 0; i < lookup.GetLength(0); i++)
            {
                l.Add(lookup[i, 0]);
            }
            return l;
        }
    }
}