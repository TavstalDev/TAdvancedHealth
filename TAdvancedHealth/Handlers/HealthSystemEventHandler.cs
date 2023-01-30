using Rocket.API.Serialisation;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TAdvancedHealth.Compatibility;
using Tavstal.TAdvancedHealth.Managers;
using Tavstal.TAdvancedHealth.Modules;
using UnityEngine;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.API;
using Logger = Tavstal.TAdvancedHealth.Helpers.LoggerHelper;
using SDG.Framework.Utilities;
using Rocket.Unturned;
using System.Reflection;
using Tavstal.TAdvancedHealth.Helpers;

namespace Tavstal.TAdvancedHealth.Handlers
{
    public static class HealthSystemEventHandler
    {
        private static DatabaseManager Database => TAdvancedHealthMain.Database;
        private static TAdvancedHealthConfig Config => TAdvancedHealthMain.Instance.Configuration.Instance;

        internal static void OnLoad()
        {
            EventManager.OnBaseHealthUpdated += Event_OnBaseHealthUpdated;
            EventManager.OnHeadHealthUpdated += Event_OnHeadHealthUpdated;
            EventManager.OnBodyHealthUpdated += Event_OnBodyHealthUpdated;
            EventManager.OnRightArmHealthUpdated += Event_OnRightArmUpdated;
            EventManager.OnRightLegHealthUpdated += Event_OnRightLegUpdated;
            EventManager.OnLeftArmHealthUpdated += Event_OnLeftArmUpdated;
            EventManager.OnLeftLegHealthUpdated += Event_OnLeftLegUpdated;
            EventManager.OnInjuredStateUpdated += Event_OnInjuredStateUpdated;
        }

        internal static void OnUnload()
        {
            EventManager.OnBaseHealthUpdated -= Event_OnBaseHealthUpdated;
            EventManager.OnHeadHealthUpdated -= Event_OnHeadHealthUpdated;
            EventManager.OnBodyHealthUpdated -= Event_OnBodyHealthUpdated;
            EventManager.OnRightArmHealthUpdated -= Event_OnRightArmUpdated;
            EventManager.OnRightLegHealthUpdated -= Event_OnRightLegUpdated;
            EventManager.OnLeftArmHealthUpdated -= Event_OnLeftArmUpdated;
            EventManager.OnLeftLegHealthUpdated -= Event_OnLeftLegUpdated;
            EventManager.OnInjuredStateUpdated -= Event_OnInjuredStateUpdated;
        }

        private static void Event_OnInjuredStateUpdated(string id, bool isInjured, DateTime bleedDate)
        {
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID((CSteamID)Convert.ToUInt64(id));
            
        }

        private static void Event_OnLeftLegUpdated(string id, float newHealth)
        {
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID((CSteamID)Convert.ToUInt64(id));
            TAdvancedHealthComponent comp = player.GetComponent<TAdvancedHealthComponent>();
            EffectHelper.sendUIEffectProgressBar((short)comp.EffectID, player.CSteamID, true, EProgressBarType.Health_LeftLeg, 
                (int)(newHealth / Config.CustomHealtSystemAndComponentSettings.LeftLegHealth * 100), (int)comp.progressBarData.LastHealthLeftLeg);
            EffectManager.sendUIEffectText((short)comp.EffectID, comp.transCon, true, "tb_LeftLeg", Math.Round(newHealth, 0).ToString());
        }

        private static void Event_OnLeftArmUpdated(string id, float newHealth)
        {
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID((CSteamID)Convert.ToUInt64(id));
            TAdvancedHealthComponent comp = player.GetComponent<TAdvancedHealthComponent>();
            EffectHelper.sendUIEffectProgressBar((short)comp.EffectID, player.CSteamID, true, EProgressBarType.Health_LeftArm,
                (int)(newHealth / Config.CustomHealtSystemAndComponentSettings.LeftArmHealth * 100), (int)comp.progressBarData.LastHealthLeftArm);
            EffectManager.sendUIEffectText((short)comp.EffectID, comp.transCon, true, "tb_LeftArm", Math.Round(newHealth, 0).ToString());
        }

        private static void Event_OnRightLegUpdated(string id, float newHealth)
        {
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID((CSteamID)Convert.ToUInt64(id));
            TAdvancedHealthComponent comp = player.GetComponent<TAdvancedHealthComponent>();
            EffectHelper.sendUIEffectProgressBar((short)comp.EffectID, player.CSteamID, true, EProgressBarType.Health_RightLeg,
                (int)(newHealth / Config.CustomHealtSystemAndComponentSettings.RightLegHealth * 100), (int)comp.progressBarData.LastHealthRightLeg);
            EffectManager.sendUIEffectText((short)comp.EffectID, comp.transCon, true, "tb_RightLeg", Math.Round(newHealth, 0).ToString());
        }

        private static void Event_OnRightArmUpdated(string id, float newHealth)
        {
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID((CSteamID)Convert.ToUInt64(id));
            TAdvancedHealthComponent comp = player.GetComponent<TAdvancedHealthComponent>();
            EffectHelper.sendUIEffectProgressBar((short)comp.EffectID, player.CSteamID, true, EProgressBarType.Health_RightArm,
                (int)(newHealth / Config.CustomHealtSystemAndComponentSettings.RightArmHealth * 100), (int)comp.progressBarData.LastHealthRightArm);
            EffectManager.sendUIEffectText((short)comp.EffectID, comp.transCon, true, "tb_RightArm", Math.Round(newHealth, 0).ToString());
        }

        private static void Event_OnBodyHealthUpdated(string id, float newHealth)
        {
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID((CSteamID)Convert.ToUInt64(id));
            TAdvancedHealthComponent comp = player.GetComponent<TAdvancedHealthComponent>();
            EffectHelper.sendUIEffectProgressBar((short)comp.EffectID, player.CSteamID, true, EProgressBarType.Health_Body,
                (int)(newHealth / Config.CustomHealtSystemAndComponentSettings.BodyHealth * 100), (int)comp.progressBarData.LastHealthBody);
            EffectManager.sendUIEffectText((short)comp.EffectID, comp.transCon, true, "tb_Body", Math.Round(newHealth, 0).ToString());
        }

        private static void Event_OnHeadHealthUpdated(string id, float newHealth)
        {
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID((CSteamID)Convert.ToUInt64(id));
            TAdvancedHealthComponent comp = player.GetComponent<TAdvancedHealthComponent>();
            EffectHelper.sendUIEffectProgressBar((short)comp.EffectID, player.CSteamID, true, EProgressBarType.Health_Head,
                (int)(newHealth / Config.CustomHealtSystemAndComponentSettings.HeadHealth * 100), (int)comp.progressBarData.LastHealthHead);
            EffectManager.sendUIEffectText((short)comp.EffectID, comp.transCon, true, "tb_Head", Math.Round(newHealth, 0).ToString());
        }

        private static void Event_OnBaseHealthUpdated(string id, float newHealth)
        {
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID((CSteamID)Convert.ToUInt64(id));
            TAdvancedHealthComponent comp = player.GetComponent<TAdvancedHealthComponent>();
            EffectHelper.sendUIEffectProgressBar((short)comp.EffectID, player.CSteamID, true, EProgressBarType.Health_Simple,
                (int)(newHealth / Config.CustomHealtSystemAndComponentSettings.BaseHealth * 100), (int)comp.progressBarData.LastSimpleHealth);
            EffectManager.sendUIEffectText((short)comp.EffectID, comp.transCon, true, "tb_Health", Math.Round(newHealth, 0).ToString());
        }
    }
}
