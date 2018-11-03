using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.BrimstoneWaifu
{
	public class SeethingDischarge : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Seething Discharge");
			Tooltip.SetDefault("Fires a barrage of brimstone blasts");
		}

	    public override void SetDefaults()
	    {
			item.damage = 62;
			item.magic = true;
			item.mana = 15;
			item.width = 28;
			item.height = 32;
			item.useTime = 24;
			item.useAnimation = 24;
			item.useStyle = 5;
			item.noMelee = true; //so the item's animation doesn't do damage
			item.knockBack = 6.75f;
			item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/FlareSound");
			item.value = 500000;
			item.rare = 5;
			item.autoReuse = true;
			item.shoot = mod.ProjectileType("BrimstoneBarrage"); //idk why but all the guns in the vanilla source have this
			item.shootSpeed = 6f;
		}
	    
	    public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	        float SpeedX = speedX + 10f * 0.05f;
	        float SpeedY = speedY + 10f * 0.05f;
	        float SpeedX2 = speedX - 10f * 0.05f;
	        float SpeedY2 = speedY - 10f * 0.05f;
	        float SpeedX3 = speedX + 0f * 0.05f;
	        float SpeedY3 = speedY + 0f * 0.05f;
	        int projectile1 = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	        int projectile2 = Projectile.NewProjectile(position.X, position.Y, SpeedX2, SpeedY2, mod.ProjectileType("BrimstoneHellblast"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	        int projectile3 = Projectile.NewProjectile(position.X, position.Y, SpeedX3, SpeedY3, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	        Main.projectile[projectile1].hostile = false;
	        Main.projectile[projectile1].friendly = true;
	        Main.projectile[projectile1].magic = true;
	        Main.projectile[projectile2].hostile = false;
	        Main.projectile[projectile2].friendly = true;
	        Main.projectile[projectile2].tileCollide = false;
	        Main.projectile[projectile2].magic = true;
	        Main.projectile[projectile3].hostile = false;
	        Main.projectile[projectile3].friendly = true;
	        Main.projectile[projectile3].magic = true;
	    	return false;
		}
	}
}