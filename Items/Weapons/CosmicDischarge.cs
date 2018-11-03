using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons 
{
	public class CosmicDischarge : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cosmic Discharge");
			Tooltip.SetDefault("Legendary Drop\n" +
				"Striking an enemy with the whip causes glacial explosions and grants the player the cosmic freeze buff\n" +
				"This buff gives the player increased life regen while standing still and freezes enemies near the player\n" +
                "Revengeance drop");
		}

		public override void SetDefaults()
		{
			item.width = 16;
			item.height = 16;
			item.damage = 1050;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.channel = true;
			item.autoReuse = true;
			item.melee = true;
			item.useAnimation = 15;
			item.useTime = 15;
			item.useStyle = 5;
			item.knockBack = 0.5f;
			item.UseSound = SoundID.Item122;
			item.value = 5000000;
			item.shootSpeed = 24f;
			item.shoot = mod.ProjectileType("CosmicDischarge");
		}
		
		public override void ModifyTooltips(List<TooltipLine> list)
	    {
	        foreach (TooltipLine line2 in list)
	        {
	            if (line2.mod == "Terraria" && line2.Name == "ItemName")
	            {
	                line2.overrideColor = new Color(150, Main.DiscoG, 255);
	            }
	        }
	    }
		
		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	float ai3 = (Main.rand.NextFloat() - 0.75f) * 0.7853982f; //0.5
	       	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0.0f, ai3);
	    	return false;
		}
	}
}
