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
	public class Aeries : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Aeries");
			Tooltip.SetDefault("Their lives are yours");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 36;
	        item.ranged = true;
	        item.width = 50;
	        item.height = 32;
	        item.useTime = 10;
	        item.useAnimation = 10;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 5.5f;
	        item.value = 350000;
	        item.rare = 7;
	        item.UseSound = SoundID.Item41;
	        item.autoReuse = false;
	        item.shootSpeed = 24f;
	        item.shoot = mod.ProjectileType("ShockblastRound");
	        item.useAmmo = 97;
	    }
	    
	    public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5, 0);
		}
	    
	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("ShockblastRound"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	    	return false;
		}
	
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.SpectreBar, 5);
	        recipe.AddIngredient(ItemID.PhoenixBlaster);
	        recipe.AddIngredient(ItemID.ShroomiteBar, 5);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}