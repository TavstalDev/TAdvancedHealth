using System;
using System.Collections.Generic;
using Tavstal.TAdvancedHealth.Managers;
using System.Globalization;

namespace Tavstal.TAdvancedHealth.Compatibility
{
    public static class StringExtensions
    {

        public static bool EqualsIgnoreCase(this string str1, string str2)
        {
            return string.Compare(str1, str2, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static bool ContainsIgnoreCase(this string str, string part)
        {
            return CultureInfo.InvariantCulture.CompareInfo.IndexOf(str, part, CompareOptions.IgnoreCase) >= 0;
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static string Capitalize(this string str)
        {
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(str.ToLowerInvariant());
        }

    }
}
