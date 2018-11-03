﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.NPCs;

namespace CalamityMod.Items
{
	public class Moonlight : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Moonlight");
			Tooltip.SetDefault("Summons the moon");
		}
		
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.rare = 5;
			item.useAnimation = 20;
			item.useTime = 20;
			item.useStyle = 4;
			item.UseSound = SoundID.Item60;
			item.consumable = false;
		}
		
		public override bool CanUseItem(Player player)
		{
			return Main.dayTime && !CalamityGlobalNPC.AnyBossNPCS();
		}
		
		public override bool UseItem(Player player)
		{
			Main.dayTime = false;
			if (Main.netMode == 2)
			{
				NetMessage.SendData(7, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
			}
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.SoulofNight, 7);
			recipe.AddIngredient(null, "CryoBar", 5);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}