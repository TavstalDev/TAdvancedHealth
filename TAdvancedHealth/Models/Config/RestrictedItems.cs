using SDG.Unturned;
using System.Collections.Generic;

namespace Tavstal.TAdvancedHealth.Models.Config
{
    public class RestrictedItems
    {
        public List<EItemType> ItemTypes { get; set; }
        public List<ushort> ItemID { get; set; }

        public RestrictedItems() { }

        public RestrictedItems(List<EItemType> itemTypes, List<ushort> itemID)
        {
            ItemTypes = itemTypes;
            ItemID = itemID;
        }
    }
}
