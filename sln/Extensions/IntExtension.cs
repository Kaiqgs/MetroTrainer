using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metrotrainer
{
    public static class IntExtension
    {
        public static string ToTimeFormat(this int me) => $"{me / 60,2:D2}:{me % 60,2:D2}";
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static int Mod(this int x, int m)
        {
            return (x % m + m) % m;
        }
    }
}
