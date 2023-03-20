using System;
using System.Collections.Generic;
using Tavstal.TAdvancedHealth.Managers;
using System.Globalization;

namespace Tavstal.TAdvancedHealth.Compatibility
{
    public static class ArrayExensions
    {
        public static bool isValidIndex<T>(this List<T> list, int index)
        {
            return list.Count - 1 >= index;
        }

        public static bool isValidIndex<T>(this T[] list, int index)
        {
            return list.Length - 1 >= index;
        }

        public static bool ContainsIgnoreCase(this List<string> stringList, string text)
        {
            foreach (string s in stringList)
                if (s.EqualsIgnoreCase(text))
                    return true;

            return false;
        }

        public static void Shuffle<T>(this T[] list)
        {
            int count = list.Length;
            while (count > 0)
            {
                count--;
                int index = MathHelper.Next(count + 1);
                T value = list[index];
                list[index] = list[count];
                list[count] = value;
            }
        }

        public static void Shuffle<T>(this List<T> list)
        {
            int count = list.Count;
            while (count > 0)
            {
                count--;
                int index = MathHelper.Next(count + 1);
                T value = list[index];
                list[index] = list[count];
                list[count] = value;
            }
        }
    }
}
