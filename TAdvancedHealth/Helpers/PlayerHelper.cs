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
    public static class PlayerHelper
    {
        public static List<Player> GetPlayersInRadius(Vector3 center, float sqrRadius, Player ignoredPlayer = null)
        {
            List<Player> result = new List<Player>();
            for (int i = 0; i < Provider.clients.Count; i++)
            {
                Player player = Provider.clients[i].player;
                if (!(player == null) && (player.transform.position - center).sqrMagnitude < sqrRadius)
                {
                    if (ignoredPlayer == null || ignoredPlayer != player)
                        result.Add(player);
                }
            }
            return result;
        }
        public static List<RocketPermissionsGroup> GetMutualGroups(UnturnedPlayer p1, UnturnedPlayer p2)
        {
            List<RocketPermissionsGroup> p1Groups = Rocket.Core.R.Permissions.GetGroups(p1, true);
            List<RocketPermissionsGroup> p2Groups = Rocket.Core.R.Permissions.GetGroups(p2, true);

            return p1Groups.Intersect(p2Groups).ToList();
        }
    }
}
