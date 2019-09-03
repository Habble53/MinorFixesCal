﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class UltimusCleaverDust : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ultimus Flame");
		}

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.penetrate = 5;
            projectile.timeLeft = 120;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 4;
			projectile.tileCollide = false;
            projectile.melee = true;
        }

        public override void AI()
        {
			if (projectile.velocity.X != projectile.velocity.X)
			{
				projectile.velocity.X = projectile.velocity.X * -0.1f;
			}
			if (projectile.velocity.X != projectile.velocity.X)
			{
				projectile.velocity.X = projectile.velocity.X * -0.5f;
			}
			if (projectile.velocity.Y != projectile.velocity.Y && projectile.velocity.Y > 1f)
			{
				projectile.velocity.Y = projectile.velocity.Y * -0.5f;
			}
			projectile.ai[0] += 1f;
			if (projectile.ai[0] > 5f)
			{
				projectile.ai[0] = 5f;
				if (projectile.velocity.Y == 0f && projectile.velocity.X != 0f)
				{
					projectile.velocity.X = projectile.velocity.X * 0.97f;
					if ((double)projectile.velocity.X > -0.01 && (double)projectile.velocity.X < 0.01)
					{
						projectile.velocity.X = 0f;
						projectile.netUpdate = true;
					}
				}
				projectile.velocity.Y = projectile.velocity.Y + 0.2f;
			}
			projectile.rotation += projectile.velocity.X * 0.1f;
			if (projectile.ai[1] == 0f)
			{
				projectile.ai[1] = 1f;
				Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 13);
			}
			int num199 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 246, 0f, 0f, 100, new Color(255, Main.DiscoG, 53), 1f);
			Dust expr_8976_cp_0 = Main.dust[num199];
			expr_8976_cp_0.position.X = expr_8976_cp_0.position.X - 2f;
			Dust expr_8994_cp_0 = Main.dust[num199];
			expr_8994_cp_0.position.Y = expr_8994_cp_0.position.Y + 2f;
			Main.dust[num199].scale += (float)Main.rand.Next(50) * 0.01f;
			Main.dust[num199].noGravity = true;
			Dust expr_89E7_cp_0 = Main.dust[num199];
			expr_89E7_cp_0.velocity.Y = expr_89E7_cp_0.velocity.Y - 2f;
			if (Main.rand.Next(2) == 0)
			{
				int num200 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 246, 0f, 0f, 100, new Color(255, Main.DiscoG, 53), 1f);
				Dust expr_8A4E_cp_0 = Main.dust[num200];
				expr_8A4E_cp_0.position.X = expr_8A4E_cp_0.position.X - 2f;
				Dust expr_8A6C_cp_0 = Main.dust[num200];
				expr_8A6C_cp_0.position.Y = expr_8A6C_cp_0.position.Y + 2f;
				Main.dust[num200].scale += 0.3f + (float)Main.rand.Next(50) * 0.01f;
				Main.dust[num200].noGravity = true;
				Main.dust[num200].velocity *= 0.1f;
			}
			if ((double)projectile.velocity.Y < 0.25 && (double)projectile.velocity.Y > 0.15)
			{
				projectile.velocity.X = projectile.velocity.X * 0.8f;
			}
			projectile.rotation = -projectile.velocity.X * 0.05f;
			if (projectile.velocity.Y > 16f)
			{
				projectile.velocity.Y = 16f;
			}
        }
    }
}
