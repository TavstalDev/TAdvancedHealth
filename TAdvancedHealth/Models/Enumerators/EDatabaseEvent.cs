using System;

namespace Tavstal.TAdvancedHealth.Models.Enumerators
{
    [Flags]
    public enum EDatabaseEvent
    {
       None = 0,
       All = 1,
       Base = 2,
       Head = 3,
       Body = 4,
       LeftARM = 5,
       RightARM = 6,
       LeftLeg = 7,
       RightLeg = 8,
       Injured = 9
    }
}
