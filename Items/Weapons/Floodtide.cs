using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.Items.Weapons
{
    public class Floodtide : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Floodtide");
			Tooltip.SetDefault("Launches sharks, because sharks are awesome!");
		}

        public override void SetDefaults()
        {
            item.damage = 89;
            item.melee = true;
            item.width = 60;
            item.height = 64;
            item.useTime = 23;
            item.useAnimation = 23;
			item.useTurn = true;
            item.useStyle = 1;
            item.knockBack = 6f;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.shoot = 408;
            item.shootSpeed = 6f;
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			for (int i = 0; i < 2; i++)
			{
				float SpeedX = speedX + (float)Main.rand.Next(-20, 21) * 0.05f;
				float SpeedY = speedY + (float)Main.rand.Next(-20, 21) * 0.05f;
				int proj = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
				Main.projectile[proj].GetGlobalProjectile<CalamityGlobalProjectile>(mod).forceMelee = true;
			}
			return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.Next(5) == 0)
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 217);
            }
        }

        public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "VictideBar", 5);
	        recipe.AddIngredient(ItemID.SharkFin, 2);
	        recipe.AddIngredient(ItemID.AdamantiteBar, 5);
            recipe.AddIngredient(null, "DepthCells", 10);
            recipe.AddIngredient(null, "Lumenite", 10);
            recipe.AddIngredient(null, "Tenebris", 5);
            recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	        recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "VictideBar", 5);
	        recipe.AddIngredient(ItemID.SharkFin, 2);
	        recipe.AddIngredient(ItemID.TitaniumBar, 5);
            recipe.AddIngredient(null, "DepthCells", 10);
            recipe.AddIngredient(null, "Lumenite", 10);
            recipe.AddIngredient(null, "Tenebris", 5);
            recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
    }
}
