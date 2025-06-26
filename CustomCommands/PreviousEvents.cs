using InventorySystem;
using InventorySystem.Items;
using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace CustomCommands
{
    public class PreviousEvents
    {
        //[PluginEvent(ServerEventType.PlayerCoinFlip)]
        public void CoinFlip(Player player, bool isTails)
        {
            MEC.Timing.CallDelayed(2, () =>
            {
                if (!isTails)
                {
                    Item item = player.CurrentItem;
                    player.ReferenceHub.inventory.ServerRemoveItem(item.Base.ItemSerial, null);
                    ExplosionUtils.ServerExplode(player.ReferenceHub, ExplosionType.PinkCandy);
                }
            });
        }
    }
}
