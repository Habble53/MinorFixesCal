﻿using System;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using CalamityMod.Particles;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Items.Weapons.Ranged;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class BlightFlames : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/Ranged/BlightFlames";

        public ref float ScaleFactor => ref Projectile.ai[0];
        public ref float LightPower => ref Projectile.ai[1];

        public int Time = 0;
        public bool postTileHit = false;
        public bool postEnemyHit = false;

        public Color FogColor = new Color(30, 255, 30);
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 5;
            Projectile.timeLeft = 400;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 55;
        }
        public override void AI()
        {
            Time++;
            Projectile.rotation += Main.rand.NextFloat(0.2f, 0.9f);
            if (Time > 6 && Time < 540 && Main.rand.NextBool(4 + Time / 6))
            {
                DirectionalPulseRing pulse = new DirectionalPulseRing(Projectile.Center + Main.rand.NextVector2Circular(10 + Time * 0.5f, 10 + Time * 0.5f), Vector2.Zero, Main.rand.NextBool(3) ? Color.LimeGreen : Color.Green, new Vector2(1, 1), 0, Main.rand.NextFloat(0.03f, 0.09f) + Time * 0.00055f, 0f, 25);
                GeneralParticleHandler.SpawnParticle(pulse);
            }
            if (Time > 6 && Time < 135 && !postTileHit && !postEnemyHit && Main.rand.NextBool(3 + Time / 5))
            {
                SmallSmokeParticle smoke = new SmallSmokeParticle(Projectile.Center + Main.rand.NextVector2Circular(5 + Time * 0.2f, 5 + Time * 0.2f), -Projectile.velocity * 0.05f, Color.Chartreuse, Main.rand.NextBool(3) ? Color.LimeGreen : Color.Lime, Main.rand.NextFloat(0.5f, 1.8f) + Time * 0.00555f, 60);
                GeneralParticleHandler.SpawnParticle(smoke);
            }
            if (Time == 8)
            {
                for (int i = 0; i <= 8; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, 91, Projectile.velocity * 0.6f);
                    dust.scale = Main.rand.NextFloat(1.1f, 1.9f);
                    dust.velocity = Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.3f, 2.1f);
                    dust.noGravity = true;
                    dust.color = Main.rand.NextBool() ? Color.Chartreuse : Color.LimeGreen;
                    dust.noLight = true;
                    dust.alpha = 90;
                }
            }
            if (Time == 10)
            {
                for (int i = 0; i <= 3; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, 220, Projectile.velocity * 0.6f);
                    dust.scale = Main.rand.NextFloat(0.4f, 1.2f);
                    dust.velocity = Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.3f, 2.1f);
                    dust.noGravity = false;
                }
            }
            if (Main.rand.NextBool(4) && Time > 135 && !postTileHit && !postEnemyHit)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(145, 145), 220, Vector2.Zero);
                dust.scale = Main.rand.NextFloat(0.2f, 0.4f);
                dust.noGravity = true;
            }
            if (Main.rand.NextBool(20) && (postTileHit || postEnemyHit))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(145, 145), 220, Vector2.Zero);
                dust.scale = Main.rand.NextFloat(0.4f, 1.2f);
                dust.noGravity = true;
            }
            // Add some degree of variation to the fog with rotation/color

            if (Main.rand.NextBool(3))
                FogColor.G = (byte)Main.rand.Next(160, 230 + 1);
            ScaleFactor += 0.0061f;
            ScaleFactor = MathHelper.Clamp(ScaleFactor, 0f, Projectile.scale);
            Lighting.AddLight(Projectile.Center, new Vector3(1f, 1f, 0.25f) * ScaleFactor);

            Projectile.velocity *= 0.99f;
            Projectile.Opacity = Utils.GetLerpValue(30f, 50f, Projectile.timeLeft, true) * Utils.GetLerpValue(0f, 130f, Projectile.timeLeft, true);

            // Calculate light power. This checks below the position of the fog to check if this fog is underground.
            // Without this, it may render over the fullblack that the game renders for obscured tiles.
            float lightPowerBelow = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16 + 6).ToVector3().Length() / (float)Math.Sqrt(3D);
            LightPower = MathHelper.Lerp(LightPower, lightPowerBelow, 0.15f);

            if (postEnemyHit && !postTileHit)
            {
                CalamityUtils.HomeInOnNPC(Projectile, true, 300f, 25f, 35f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!postTileHit)
                Projectile.Center = Projectile.Center + Main.rand.NextVector2Circular(20, 20);
            target.AddBuff(ModContent.BuffType<Plague>(), 420);

            for (int i = 0; i <= 3; i++)
            {
                SmallSmokeParticle smoke = new SmallSmokeParticle(Projectile.Center + Main.rand.NextVector2Circular(5 + Time * 0.2f, 5 + Time * 0.2f), new Vector2(Main.rand.NextFloat(2, 6), Main.rand.NextFloat(2, 6)).RotatedByRandom(60), Color.Chartreuse, Main.rand.NextBool(3) ? Color.LimeGreen : Color.Lime, Main.rand.NextFloat(1.2f, 2.8f), 60);
                GeneralParticleHandler.SpawnParticle(smoke);
            }
            if (!postTileHit && !postEnemyHit)
            {
                SoundEngine.PlaySound(BlightSpewer.Nanomachines, Projectile.Center);
                Projectile.velocity = Vector2.Zero;
                Projectile.timeLeft = 300;
                postEnemyHit = true;
            }

        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Projectile.damage = (int)(Projectile.damage * 0.95f);
            if (Projectile.damage < 1)
                Projectile.damage = 1;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!postEnemyHit)
            {
                Projectile.velocity = oldVelocity * 0.95f;
                Projectile.position -= Projectile.velocity;
                if (!postTileHit)
                {
                    Projectile.timeLeft = 800;
                    postTileHit = true;
                }
            }
            return false;
        }

        // Expanding hitbox
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            int size = (int)Utils.Remap(Time, 0f, 90, 10f, 95f);
            hitbox.Inflate(size, size);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.SetBlendState(BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float opacity = Utils.GetLerpValue(0f, 0.08f, LightPower, true) * Projectile.Opacity * 0.65f;
            Color drawColor = FogColor * opacity;
            Main.EntitySpriteDraw(texture, drawPosition, null, drawColor, Projectile.rotation, texture.Size() * 0.5f, ScaleFactor, SpriteEffects.None);

            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}
