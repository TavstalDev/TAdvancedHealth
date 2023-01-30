using System;
using System.Collections.Generic;
using Tavstal.TAdvancedHealth.Managers;
using System.Globalization;

namespace Tavstal.TAdvancedHealth.Compatibility
{

    public static class IntegerExtensions
    {
        public static int Clamp(this int value, int maxValue)
        {
            return maxValue < value ? maxValue : value;
        }

        public static int Clamp(this int value, int minValue, int maxValue)
        {
            return minValue > value ? minValue : maxValue < value ? maxValue : value;
        }
    }
}
