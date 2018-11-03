using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.HiveMind
{
	public class HiveMindBag : ModItem
	{
		public override void SetStaticDefaults()
 		{
 			DisplayName.SetDefault("Treasure Bag");
 			Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
 		}
		
		public override void SetDefaults()
		{
			item.maxStack = 999;
			item.consumable = true;
			item.width = 24;
			item.height = 24;
			item.rare = 9;
			item.expert = true;
			bossBagNPC = mod.NPCType("HiveMindP2");
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override void OpenBossBag(Player player)
		{
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("ShaderainStaff"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("LeechingDagger"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("ShadowdropStaff"));
			}
			if (Main.rand.Next(7) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("HiveMindMask"));
			}
			if(Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("PerfectDark"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("Shadethrower"));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("RotBall"), Main.rand.Next(50, 76));
			}
			if (Main.rand.Next(3) == 0)
			{
				player.QuickSpawnItem(mod.ItemType("DankStaff"));
			}
			player.QuickSpawnItem(mod.ItemType("RottenBrain"));
			player.QuickSpawnItem(ItemID.RottenChunk, Main.rand.Next(10, 21));
			player.QuickSpawnItem(ItemID.DemoniteBar, Main.rand.Next(9, 15));
			player.QuickSpawnItem(mod.ItemType("TrueShadowScale"), Main.rand.Next(30, 41));
            if (Main.hardMode)
            {
                player.QuickSpawnItem(ItemID.CursedFlame, Main.rand.Next(15, 31));
            }
        }
	}
}