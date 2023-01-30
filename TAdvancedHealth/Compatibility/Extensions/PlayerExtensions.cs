using System;
using System.Collections.Generic;
using Tavstal.TAdvancedHealth.Managers;
using System.Globalization;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using Rocket.API;
using SDG.Unturned;
using UnityEngine;
using Logger = Tavstal.TAdvancedHealth.Helpers.LoggerHelper;
using Steamworks;
using SDG.Framework.Utilities;
using System.Linq;
using Rocket.Unturned;
using Rocket.API.Serialisation;
using Tavstal.TAdvancedHealth.Modules;
using Tavstal.TAdvancedHealth.Compatibility;
using System.Reflection;
using Tavstal.TAdvancedHealth.Handlers;
using Tavstal.TAdvancedHealth.Helpers;

namespace Tavstal.TAdvancedHealth.Compatibility
{

    public static class PlayerExtensions
    {

        public static UnturnedPlayer GetPlayer(this CSteamID steamID)
        {
            return UnturnedPlayer.FromCSteamID(steamID);
        }
    }
}
