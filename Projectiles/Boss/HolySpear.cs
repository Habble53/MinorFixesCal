﻿using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class HolySpear : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Holy Spear");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			projectile.width = 30;
			projectile.height = 30;
			projectile.hostile = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.penetrate = -1;
			projectile.alpha = 255;
			projectile.timeLeft = 900;
			cooldownSlot = 1;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(projectile.localAI[0]);
			writer.Write(projectile.localAI[1]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			projectile.localAI[0] = reader.ReadSingle();
			projectile.localAI[1] = reader.ReadSingle();
		}

		public override void AI()
		{
			if (projectile.timeLeft > 815)
			{
				if (projectile.ai[0] == 0f)
					projectile.velocity.X = 1f;
				else
					projectile.velocity.X = -1f;
			}
			else
			{
				if (projectile.ai[0] == 0f)
					projectile.velocity.X = 15f;
				else
					projectile.velocity.X = -15f;
			}
			projectile.rotation = projectile.velocity.ToRotation() + 1.57079637f;
			if (projectile.localAI[0] == 0f && projectile.timeLeft < 815)
			{
				projectile.localAI[0] = 1f;
				Main.PlayTrackedSound(SoundID.DD2_BetsyFireballShot, projectile.Center);
			}
		}

		public override bool CanDamage()
		{
			if (projectile.timeLeft > 815 || projectile.timeLeft < 85)
			{
				return false;
			}
			return true;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			if (projectile.timeLeft > 883)
			{
				projectile.localAI[1] += 5f;
				byte b2 = (byte)(((int)projectile.localAI[1]) * 3);
				byte a2 = (byte)(100f * ((float)b2 / 255f));
				return new Color((int)b2, (int)b2, (int)b2, (int)a2);
			}
			if (projectile.timeLeft < 85)
			{
				byte b2 = (byte)(projectile.timeLeft * 3);
				byte a2 = (byte)(100f * ((float)b2 / 255f));
				return new Color((int)b2, (int)b2, (int)b2, (int)a2);
			}
			return new Color(255, 255, 255, 100);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
			return false;
		}

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			target.AddBuff(mod.BuffType("HolyLight"), 120);
		}
	}
}
