using System;
using System.Collections.Generic;
using Tavstal.TAdvancedHealth.Managers;
using System.Globalization;

namespace Tavstal.TAdvancedHealth.Compatibility
{
    public static class DecimalExtensions
    {
        public static decimal Clamp(this decimal value, decimal maxValue)
        {
            return maxValue < value ? maxValue : value;
        }

        public static decimal Clamp(this decimal value, decimal minValue, decimal maxValue)
        {
            return minValue > value ? minValue : maxValue < value ? maxValue : value;
        }
    }
}
