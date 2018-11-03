using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Perforators
{
	public class VeinBurster : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Vein Burster");
		}

		public override void SetDefaults()
		{
			item.width = 52;
			item.damage = 22;
			item.melee = true;
			item.useAnimation = 25;
			item.useStyle = 1;
			item.useTime = 25;
			item.useTurn = true;
			item.knockBack = 4.25f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 50;
			item.value = 50000;
			item.rare = 4;
			item.shoot = mod.ProjectileType("BloodBall");
			item.shootSpeed = 5f;
		}
		
		public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.Vertebrae, 5);
	        recipe.AddIngredient(ItemID.CrimtaneBar, 5);
	        recipe.AddIngredient(null, "BloodSample", 15);
	        recipe.AddTile(TileID.DemonAltar);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}
