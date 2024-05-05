using System;

namespace Tavstal.TAdvancedHealth.Utils.Managers
{
    /// <summary>
    /// Provides functionality for managing and dispatching events within the application.
    /// </summary>
    public static class EventManager
    {
        public delegate void BaseHealthUpdated(string id, float newHealth);
        public static event BaseHealthUpdated OnBaseHealthUpdated;
        internal static void FCallBaseHealthUpdated(string id, float newHealth)
        {
            if (OnBaseHealthUpdated != null)
                OnBaseHealthUpdated.Invoke(id, newHealth);
        }

        public delegate void HeadHealthUpdated(string id, float newHealth);
        public static event HeadHealthUpdated OnHeadHealthUpdated;
        internal static void FCallHeadHealthUpdated(string id, float newHealth)
        {
            if (OnHeadHealthUpdated != null)
                OnHeadHealthUpdated.Invoke(id, newHealth);
        }

        public delegate void BodyHealthUpdated(string id, float newHealth);
        public static event BodyHealthUpdated OnBodyHealthUpdated;
        internal static void FCallBodyHealthUpdated(string id, float newHealth)
        {
            if (OnBodyHealthUpdated != null)
                OnBodyHealthUpdated.Invoke(id, newHealth);
        }

        public delegate void RightArmHealthUpdated(string id, float newHealth);
        public static event RightArmHealthUpdated OnRightArmHealthUpdated;
        internal static void FCallRightArmHealthUpdated(string id, float newHealth)
        {
            if (OnRightArmHealthUpdated != null)
                OnRightArmHealthUpdated.Invoke(id, newHealth);
        }

        public delegate void LeftArmHealthUpdated(string id, float newHealth);
        public static event LeftArmHealthUpdated OnLeftArmHealthUpdated;
        internal static void FCallLeftArmHealthUpdated(string id, float newHealth)
        {
            if (OnLeftArmHealthUpdated != null)
                OnLeftArmHealthUpdated.Invoke(id, newHealth);
        }

        public delegate void RightLegHealthUpdated(string id, float newHealth);
        public static event RightLegHealthUpdated OnRightLegHealthUpdated;
        internal static void FCallRightLegHealthUpdated(string id, float newHealth)
        {
            if (OnRightLegHealthUpdated != null)
                OnRightLegHealthUpdated.Invoke(id, newHealth);
        }

        public delegate void LeftLegHealthUpdated(string id, float newHealth);
        public static event LeftLegHealthUpdated OnLeftLegHealthUpdated;
        internal static void FCallLeftLegHealthUpdated(string id, float newHealth)
        {
            if (OnLeftLegHealthUpdated != null)
                OnLeftLegHealthUpdated.Invoke(id, newHealth);
        }

        public delegate void InjuredStateUpdated(string id, bool isInjured, DateTime bleedDate);
        public static event InjuredStateUpdated OnInjuredStateUpdated;
        internal static void FCallInjuredStateUpdated(string id, bool isInjured, DateTime bleedDate)
        {
            if (OnInjuredStateUpdated != null)
                OnInjuredStateUpdated.Invoke(id, isInjured, bleedDate);
        }
    }
}
