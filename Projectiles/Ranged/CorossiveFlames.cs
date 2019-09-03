﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class CorossiveFlames : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Flames");
		}

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 3;
            projectile.timeLeft = 90;
        }

        public override void AI()
        {
        	Lighting.AddLight(projectile.Center, 0f, 0.25f, 0f);
			if (projectile.timeLeft > 90)
			{
				projectile.timeLeft = 90;
			}
			if (projectile.ai[0] > 7f)
			{
				float num296 = 1f;
				if (projectile.ai[0] == 8f)
				{
					num296 = 0.25f;
				}
				else if (projectile.ai[0] == 9f)
				{
					num296 = 0.5f;
				}
				else if (projectile.ai[0] == 10f)
				{
					num296 = 0.75f;
				}
				projectile.ai[0] += 1f;
				int num297 = 89;
				if (Main.rand.Next(2) == 0)
				{
					for (int num298 = 0; num298 < 1; num298++)
					{
						int num299 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, num297, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default(Color), 1f);
						if (Main.rand.Next(3) == 0)
						{
							Main.dust[num299].noGravity = true;
							Main.dust[num299].scale *= 2.5f;
							Dust expr_DBEF_cp_0 = Main.dust[num299];
							expr_DBEF_cp_0.velocity.X = expr_DBEF_cp_0.velocity.X * 2f;
							Dust expr_DC0F_cp_0 = Main.dust[num299];
							expr_DC0F_cp_0.velocity.Y = expr_DC0F_cp_0.velocity.Y * 2f;
						}
						if (Main.rand.Next(6) == 0)
						{
							Main.dust[num299].noGravity = true;
							Main.dust[num299].scale *= 3f;
							Dust expr_DBEF_cp_0 = Main.dust[num299];
							expr_DBEF_cp_0.velocity.X = expr_DBEF_cp_0.velocity.X * 2f;
							Dust expr_DC0F_cp_0 = Main.dust[num299];
							expr_DC0F_cp_0.velocity.Y = expr_DC0F_cp_0.velocity.Y * 2f;
						}
						else
						{
							Main.dust[num299].scale *= 2f;
						}
						Dust expr_DC74_cp_0 = Main.dust[num299];
						expr_DC74_cp_0.velocity.X = expr_DC74_cp_0.velocity.X * 1.2f;
						Dust expr_DC94_cp_0 = Main.dust[num299];
						expr_DC94_cp_0.velocity.Y = expr_DC94_cp_0.velocity.Y * 1.2f;
						Main.dust[num299].scale *= num296;
					}
				}
			}
			else
			{
				projectile.ai[0] += 1f;
			}
			projectile.rotation += 0.3f * (float)projectile.direction;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.immune[projectile.owner] = 4;
        	target.AddBuff(mod.BuffType("Plague"), 300);
        }
    }
}
