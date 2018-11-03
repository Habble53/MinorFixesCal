﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using CalamityMod.NPCs;

namespace CalamityMod.Items
{
	public class Ectoheart : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ectoheart");
			Tooltip.SetDefault("Permanently makes Adrenaline Mode take 5 less seconds to charge\n" +
                "Revengeance drop");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 4));
        }
		
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.useAnimation = 30;
			item.useTime = 30;
			item.useStyle = 4;
			item.UseSound = SoundID.Item122;
			item.consumable = true;
		}

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(0, 255, 0);
                }
            }
        }

        public override bool CanUseItem(Player player)
		{
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			if (modPlayer.adrenalineBoostThree)
			{
				return false;
			}
			return true;
		}

		public override bool UseItem(Player player)
		{
			if (player.itemAnimation > 0 && player.itemTime == 0)
			{
				player.itemTime = item.useTime;
				CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
				modPlayer.adrenalineBoostThree = true;
			}
			return true;
		}
	}
}