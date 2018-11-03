using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons 
{
	public class GoldplumeSpear : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Goldplume Spear");
		}

		public override void SetDefaults()
		{
			item.width = 54;
			item.damage = 23;
			item.melee = true;
			item.noMelee = true;
			item.useTurn = true;
			item.noUseGraphic = true;
			item.useAnimation = 23;
			item.useStyle = 5;
			item.useTime = 23;
			item.knockBack = 5.75f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = false;
			item.height = 54;
			item.value = 85000;
			item.rare = 3;
			item.shoot = mod.ProjectileType("GoldplumeSpearProjectile");
			item.shootSpeed = 5f;
		}
	
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "AerialiteBar", 10);
			recipe.AddIngredient(ItemID.SunplateBlock, 4);
	        recipe.AddTile(TileID.SkyMill);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
