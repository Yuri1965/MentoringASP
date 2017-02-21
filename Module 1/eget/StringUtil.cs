using System;

namespace Eget
{
    public static class StringUtil
    {
        public static bool EqualsIgnoringCase(this string that, string other)
        {
            return that.Equals(other, StringComparison.OrdinalIgnoreCase);
        }
    }
}