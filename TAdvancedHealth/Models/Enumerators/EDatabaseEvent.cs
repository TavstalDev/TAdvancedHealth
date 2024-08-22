using System;

namespace Tavstal.TAdvancedHealth.Models.Enumerators
{
    [Flags]
    public enum EDatabaseEvent
    {
       NONE,
       ALL,
       BASE,
       HEAD,
       BODY,
       LEFT_ARM,
       RIGHT_ARM,
       LEFT_LEG,
       RIGHT_LEG,
       INJURED
    }
}
