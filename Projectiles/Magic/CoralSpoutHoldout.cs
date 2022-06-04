﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System;
using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Magic;


namespace CalamityMod.Projectiles.Magic
{
    //Holdout, but invisible. It may as well be named "CoralSpoutHandler"
    public class CoralSpoutHoldout : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public static float MaxCharge = 50;
        public static int ShotProjectiles = 5;

        public ref float Charge => ref Projectile.ai[0];
        public float ChargeProgress => MathHelper.Clamp(Charge, 0, MaxCharge) / MaxCharge;
        public float FullChargeProgress => MathHelper.Clamp(Charge, 0, MaxCharge * 1.5f) / (MaxCharge * 1.5f);
        public float Spread => MathHelper.PiOver2 * (1 - ChargeProgress * 0.95f);

        public Player Owner => Main.player[Projectile.owner];



        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Coral Spout");
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            if (Owner.channel)
            {
                Projectile.timeLeft = 2;
                Owner.itemTime = 25;
                Owner.itemAnimation = 25;
                Owner.heldProj = Projectile.whoAmI;
            }

            float pointingRotation = (Owner.Calamity().mouseWorld - Owner.Center).ToRotation();
            Projectile.Center = Owner.Center + pointingRotation.ToRotationVector2() * 40f;

            for (int i = -1; i <= 1; i += 2)
            {
                float angle = pointingRotation + (Spread / 2f) * i;
                for (int j = 0; j < 10; j++)
                {
                    Dust waterDust = Dust.NewDustPerfect(Owner.Center + angle.ToRotationVector2() * j * 40f, 33, Vector2.Zero, 100, default, 0.9f);
                    waterDust.noGravity = true;
                }
            }

            if (Projectile.soundDelay <= 0)
            {
                SoundEngine.PlaySound(CoralSpout.ChargeSound with { Pitch = 0.5f * ChargeProgress}, Owner.Center);
                Projectile.soundDelay = 10;
            }

            if (Charge == (int)(MaxCharge * 1.5f) && Owner.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.MaxMana, Owner.Center);
            }

            Charge++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }

        public override void Kill(int timeLeft)
        {
            float mainAngle = (Projectile.Center - Owner.Center).ToRotation();

            if (FullChargeProgress < 1)
            {
                SoundEngine.PlaySound(SoundID.Item167 with { Volume = SoundID.Item167.Volume * 0.4f }, Owner.Center);


                for (int i = 0; i < ShotProjectiles; i++)
                {
                    float angleOffset = MathHelper.Lerp(Spread * -0.5f, Spread * 0.5f, i / (float)ShotProjectiles);
                    Vector2 direction = (mainAngle + angleOffset).ToRotationVector2();
                    
                    if (Owner.whoAmI == Main.myPlayer)
                    {
                        float speed = 10 + 15 * ChargeProgress;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center + direction * 30f, direction * speed, ModContent.ProjectileType<CoralSpike>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI, ChargeProgress);
                    }

                    Color pulseColor = Main.rand.NextBool() ? Color.Coral : Color.DeepSkyBlue;
                    Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, pulseColor, new Vector2(0.5f, 1f), direction.ToRotation(), 0.02f, 0.1f, 30);
                    GeneralParticleHandler.SpawnParticle(pulse);
                }

            }

            else
            {
                SoundEngine.PlaySound(SoundID.Item42, Owner.Center);
                Vector2 direction = mainAngle.ToRotationVector2();

                if (Owner.whoAmI == Main.myPlayer)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center + direction * 30f, direction * 35, ModContent.ProjectileType<ManaChargedCoral>(), (int)Projectile.damage * (ShotProjectiles + 1), Projectile.knockBack, Owner.whoAmI);
                }

                Color pulseColor = Main.rand.NextBool() ? Color.Coral : Color.DeepSkyBlue;
                Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, pulseColor, new Vector2(0.5f, 1f), direction.ToRotation(), 0.05f, 0.34f + Main.rand.NextFloat(0.3f), 30);
                GeneralParticleHandler.SpawnParticle(pulse);
            }
        }
    }
}
