using SDG.Unturned;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Tavstal.TAdvancedHealth.Models.Config
{
    public class RestrictedItems
    {
        [YamlMember(Order = 0, Description = "Item types affected by the restriction")]
        public List<EItemType> ItemTypes { get; set; }
        [YamlMember(Order = 1, Description = "Specific item IDs affected by the restriction")]
        public List<ushort> Items { get; set; }

        public RestrictedItems()
        {
            ItemTypes = new List<EItemType>();
            Items = new List<ushort>();
        }

        public RestrictedItems(List<EItemType> itemTypes, List<ushort> items)
        {
            ItemTypes = itemTypes;
            Items = items;
        }
    }
}
