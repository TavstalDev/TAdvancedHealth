using System;

namespace Tavstal.TAdvancedHealth.Utils.Managers
{
    /// <summary>
    /// Provides functionality for managing and dispatching events within the application.
    /// </summary>
    public static class EventManager
    {
        #region Base Health Update
        /// <summary>
        /// Represents a delegate for handling updates to base health.
        /// </summary>
        /// <param name="id">The identifier of the base whose health was updated.</param>
        /// <param name="newHealth">The new health value of the base.</param>
        public delegate void BaseHealthUpdated(string id, float newHealth);

        /// <summary>
        /// Event triggered when the health of a base is updated.
        /// </summary>
        /// <remarks>
        /// Subscribe to this event to be notified when a base's health changes.
        /// </remarks>
        public static event BaseHealthUpdated OnBaseHealthUpdated;

        /// <summary>
        /// Internally invokes the <see cref="OnBaseHealthUpdated"/> event.
        /// </summary>
        /// <param name="id">The identifier of the base whose health was updated.</param>
        /// <param name="newHealth">The new health value of the base.</param>
        /// <remarks>
        /// This method is called to notify subscribers about base health updates.
        /// </remarks>
        internal static void FCallBaseHealthUpdated(string id, float newHealth)
        {
            OnBaseHealthUpdated?.Invoke(id, newHealth);
        }
        #endregion
        #region Head Health Update
        /// <summary>
        /// Represents a delegate for handling updates to head health.
        /// </summary>
        /// <param name="id">The identifier of the head whose health was updated.</param>
        /// <param name="newHealth">The new health value of the head.</param>
        public delegate void HeadHealthUpdated(string id, float newHealth);

        /// <summary>
        /// Event triggered when the health of a head is updated.
        /// </summary>
        /// <remarks>
        /// Subscribe to this event to be notified when a head's health changes.
        /// </remarks>
        public static event HeadHealthUpdated OnHeadHealthUpdated;

        /// <summary>
        /// Internally invokes the <see cref="OnHeadHealthUpdated"/> event.
        /// </summary>
        /// <param name="id">The identifier of the head whose health was updated.</param>
        /// <param name="newHealth">The new health value of the head.</param>
        /// <remarks>
        /// This method is called to notify subscribers about head health updates.
        /// </remarks>
        internal static void FCallHeadHealthUpdated(string id, float newHealth)
        {
            OnHeadHealthUpdated?.Invoke(id, newHealth);
        }
        #endregion
        #region Body  Health Update
        /// <summary>
        /// Represents a delegate for handling updates to body health.
        /// </summary>
        /// <param name="id">The identifier of the body whose health was updated.</param>
        /// <param name="newHealth">The new health value of the body.</param>
        public delegate void BodyHealthUpdated(string id, float newHealth);

        /// <summary>
        /// Event triggered when the health of a body is updated.
        /// </summary>
        /// <remarks>
        /// Subscribe to this event to be notified when a body's health changes.
        /// </remarks>
        public static event BodyHealthUpdated OnBodyHealthUpdated;

        /// <summary>
        /// Internally invokes the <see cref="OnBodyHealthUpdated"/> event.
        /// </summary>
        /// <param name="id">The identifier of the body whose health was updated.</param>
        /// <param name="newHealth">The new health value of the body.</param>
        /// <remarks>
        /// This method is called to notify subscribers about body health updates.
        /// </remarks>
        internal static void FCallBodyHealthUpdated(string id, float newHealth)
        {
            OnBodyHealthUpdated?.Invoke(id, newHealth);
        }
        #endregion
        #region Right Arm Health Update
        /// <summary>
        /// Represents a delegate for handling updates to the health of the right arm.
        /// </summary>
        /// <param name="id">The identifier of the right arm whose health was updated.</param>
        /// <param name="newHealth">The new health value of the right arm.</param>
        public delegate void RightArmHealthUpdated(string id, float newHealth);

        /// <summary>
        /// Event triggered when the health of the right arm is updated.
        /// </summary>
        /// <remarks>
        /// Subscribe to this event to be notified when a right arm's health changes.
        /// </remarks>
        public static event RightArmHealthUpdated OnRightArmHealthUpdated;

        /// <summary>
        /// Internally invokes the <see cref="OnRightArmHealthUpdated"/> event.
        /// </summary>
        /// <param name="id">The identifier of the right arm whose health was updated.</param>
        /// <param name="newHealth">The new health value of the right arm.</param>
        /// <remarks>
        /// This method is called to notify subscribers about right arm health updates.
        /// </remarks>
        internal static void FCallRightArmHealthUpdated(string id, float newHealth)
        {
            OnRightArmHealthUpdated?.Invoke(id, newHealth);
        }

        #endregion
        #region Left Arm Health Update
        /// <summary>
        /// Represents a delegate for handling updates to the health of the left arm.
        /// </summary>
        /// <param name="id">The identifier of the left arm whose health was updated.</param>
        /// <param name="newHealth">The new health value of the left arm.</param>
        public delegate void LeftArmHealthUpdated(string id, float newHealth);

        /// <summary>
        /// Event triggered when the health of the left arm is updated.
        /// </summary>
        /// <remarks>
        /// Subscribe to this event to be notified when a left arm's health changes.
        /// </remarks>
        public static event LeftArmHealthUpdated OnLeftArmHealthUpdated;

        /// <summary>
        /// Internally invokes the <see cref="OnLeftArmHealthUpdated"/> event.
        /// </summary>
        /// <param name="id">The identifier of the left arm whose health was updated.</param>
        /// <param name="newHealth">The new health value of the left arm.</param>
        /// <remarks>
        /// This method is called to notify subscribers about left arm health updates.
        /// </remarks>
        internal static void FCallLeftArmHealthUpdated(string id, float newHealth)
        {
            OnLeftArmHealthUpdated?.Invoke(id, newHealth);
        }

        #endregion
        #region Right Leg Health Update
        /// <summary>
        /// Represents a delegate for handling updates to the health of the right leg.
        /// </summary>
        /// <param name="id">The identifier of the right leg whose health was updated.</param>
        /// <param name="newHealth">The new health value of the right leg.</param>
        public delegate void RightLegHealthUpdated(string id, float newHealth);

        /// <summary>
        /// Event triggered when the health of the right leg is updated.
        /// </summary>
        /// <remarks>
        /// Subscribe to this event to be notified when a right leg's health changes.
        /// </remarks>
        public static event RightLegHealthUpdated OnRightLegHealthUpdated;

        /// <summary>
        /// Internally invokes the <see cref="OnRightLegHealthUpdated"/> event.
        /// </summary>
        /// <param name="id">The identifier of the right leg whose health was updated.</param>
        /// <param name="newHealth">The new health value of the right leg.</param>
        /// <remarks>
        /// This method is called to notify subscribers about right leg health updates.
        /// </remarks>
        internal static void FCallRightLegHealthUpdated(string id, float newHealth)
        {
            OnRightLegHealthUpdated?.Invoke(id, newHealth);
        }
        #endregion
        #region Left Leg Health Update
        /// <summary>
        /// Represents a delegate for handling updates to the health of the left leg.
        /// </summary>
        /// <param name="id">The identifier of the left leg whose health was updated.</param>
        /// <param name="newHealth">The new health value of the left leg.</param>
        public delegate void LeftLegHealthUpdated(string id, float newHealth);

        /// <summary>
        /// Event triggered when the health of the left leg is updated.
        /// </summary>
        /// <remarks>
        /// Subscribe to this event to be notified when a left leg's health changes.
        /// </remarks>
        public static event LeftLegHealthUpdated OnLeftLegHealthUpdated;

        /// <summary>
        /// Internally invokes the <see cref="OnLeftLegHealthUpdated"/> event.
        /// </summary>
        /// <param name="id">The identifier of the left leg whose health was updated.</param>
        /// <param name="newHealth">The new health value of the left leg.</param>
        /// <remarks>
        /// This method is called to notify subscribers about left leg health updates.
        /// </remarks>
        internal static void FCallLeftLegHealthUpdated(string id, float newHealth)
        {
            OnLeftLegHealthUpdated?.Invoke(id, newHealth);
        }
        #endregion
        #region Injured State Update
        /// <summary>
        /// Represents a delegate for handling updates to an entity's injured state.
        /// </summary>
        /// <param name="id">The identifier of the entity whose injured state is updated.</param>
        /// <param name="isInjured">Indicates whether the entity is injured or not.</param>
        /// <param name="bleedDate">The date and time when the bleeding started, if applicable.</param>
        public delegate void InjuredStateUpdated(string id, bool isInjured, DateTime bleedDate);

        /// <summary>
        /// Event triggered when an entity's injured state is updated.
        /// </summary>
        /// <remarks>
        /// Subscribe to this event to be notified when an entity's injured state changes, including injury status and bleeding information.
        /// </remarks>
        public static event InjuredStateUpdated OnInjuredStateUpdated;

        /// <summary>
        /// Internally invokes the <see cref="OnInjuredStateUpdated"/> event.
        /// </summary>
        /// <param name="id">The identifier of the entity whose injured state is updated.</param>
        /// <param name="isInjured">Indicates whether the entity is injured or not.</param>
        /// <param name="bleedDate">The date and time when the bleeding started, if applicable.</param>
        /// <remarks>
        /// This method is called to notify subscribers about an entity's injured state updates.
        /// </remarks>
        internal static void FCallInjuredStateUpdated(string id, bool isInjured, DateTime bleedDate)
        {
            OnInjuredStateUpdated?.Invoke(id, isInjured, bleedDate);
        }
        #endregion
    }
}
