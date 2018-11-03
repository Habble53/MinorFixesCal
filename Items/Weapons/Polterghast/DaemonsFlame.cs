using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Polterghast
{
	public class DaemonsFlame : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Daemon's Flame");
			Tooltip.SetDefault("Shoots daemon flame arrows as well as regular arrows");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 130;
	        item.width = 20;
	        item.height = 12;
	        item.useTime = 12;
	        item.useAnimation = 12;
	        item.useStyle = 5;
	        item.knockBack = 4f;
	        item.value = 1000000;
	        item.UseSound = SoundID.Item5;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.ranged = true;
			item.channel = true;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("DaemonsFlame");
	        item.shootSpeed = 20f;
	        item.useAmmo = 40;
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
	    
	    public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("DaemonsFlame"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	    	return false;
	    }
	}
}