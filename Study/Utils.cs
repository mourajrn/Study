using System;

namespace Study
{
    public static class Utils
    {
        public static bool IsAlmostEquals(this double first, double second, int decimals = 3)
        {
            return Math.Round(first, decimals).Equals(Math.Round(second, decimals));
        }
    }
}
