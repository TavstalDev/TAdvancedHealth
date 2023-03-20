using Rocket.API;
using Rocket.API.Collections;
using Rocket.API.Serialisation;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TAdvancedHealth.Compatibility;
using Tavstal.TAdvancedHealth.Modules;
using UnityEngine;
using Logger = Tavstal.TAdvancedHealth.Helpers.LoggerHelper;

namespace Tavstal.TAdvancedHealth.Helpers
{
    public static class UnturnedHelper
    {
        private static IAsset<TAdvancedHealthConfig> Configuration => TAdvancedHealthMain.Instance.Configuration;

        private static string Translate(bool addPrefix, string key, params object[] args) => TAdvancedHealthMain.Instance.Translate(addPrefix, key, args);

        public static void ServerSendChatMessage(string text, string icon = null, SteamPlayer fromPlayer = null, SteamPlayer toPlayer = null, EChatMode mode = EChatMode.GLOBAL)
        => ChatManager.serverSendMessage(text, Color.white, fromPlayer, toPlayer, mode, icon, true);

        public static void SendCommandReply(object toPlayer, string translation, params object[] args)
        {
            string icon = "";
            if (toPlayer is SteamPlayer steamPlayer)
                ServerSendChatMessage(FormatHelper.FormatTextV2(Translate(true, translation, args)), icon, null, steamPlayer, EChatMode.GLOBAL);
            else
                LoggerHelper.LogRichCommand(Translate(false, translation, args));
        }

        public static void SendChatMessage(SteamPlayer toPlayer, string translation, params object[] args)
        {
            string icon = "";
            ServerSendChatMessage(FormatHelper.FormatTextV2(Translate(true, translation, args)), icon, null, toPlayer, EChatMode.GLOBAL);
        }

        public static void SendChatMessage(string translation, params object[] args)
        {
            string icon = "";
            ServerSendChatMessage(Translate(true, translation, args), icon, null, null, EChatMode.GLOBAL);
        }

        public static void SendChatMessageUntranslated(SteamPlayer toPlayer, string text)
        {
            string icon = "";
            ServerSendChatMessage(text, icon, null, toPlayer, EChatMode.GLOBAL);
        }

    }
}
