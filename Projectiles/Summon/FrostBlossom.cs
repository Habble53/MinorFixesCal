﻿using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class FrostBlossom : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Blossom");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 36;
            projectile.height = 40;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
        }

        public override void AI()
        {
            bool isCorrectMinion = projectile.type == ModContent.ProjectileType<FrostBlossom>();
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            player.AddBuff(ModContent.BuffType<FrostBlossomBuff>(), 3600);
            if (isCorrectMinion)
            {
                if (player.dead)
                {
                    modPlayer.frostBlossom = false;
                }
                if (modPlayer.frostBlossom)
                {
                    projectile.timeLeft = 2;
                }
            }
            projectile.Center = Main.player[projectile.owner].Center + Vector2.UnitY * (Main.player[projectile.owner].gfxOffY - 60f);
            if (Main.player[projectile.owner].gravDir == -1f)
            {
                projectile.position.Y += 140f;
                projectile.rotation = MathHelper.Pi;
            }
            else
            {
                projectile.rotation = 0f;
            }
            projectile.position.X = (int)projectile.position.X;
            projectile.position.Y = (int)projectile.position.Y;
            projectile.scale = 1f + (float)Math.Sin(projectile.ai[0]++ / 40f) * 0.085f;
            projectile.Opacity = 1f - (float)Math.Sin(projectile.ai[1] / 45f) * 0.075f - 0.075f; // Range of 1f to 0.85f
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = (player.allDamage + player.minionDamage - 1f);
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                for (int i = 0; i < 36; i++)
                {
                    Dust dust = Dust.NewDustPerfect(projectile.Center, 113);
                    dust.noGravity = true;
                    dust.velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(2f, 7f);
                }
                projectile.localAI[0] += 1f;
            }
            if ((player.allDamage + player.minionDamage - 1f) != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int trueDamage = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    (player.allDamage + player.minionDamage - 1f));
                projectile.damage = trueDamage;
            }
            if (projectile.owner == Main.myPlayer)
            {
                NPC potentialTarget = projectile.Center.MinionHoming(1200f, player);
                if (potentialTarget != null)
                {
                    if (projectile.ai[1]++ % 35f == 34f &&
                        Collision.CanHit(projectile.position, projectile.width, projectile.height, potentialTarget.position, potentialTarget.width, potentialTarget.height))
                    {
                        Projectile.NewProjectile(projectile.Center, projectile.DirectionTo(potentialTarget.Center) * 20f,
                            ModContent.ProjectileType<FrostBeam>(), projectile.damage, projectile.knockBack, projectile.owner);
                    }
                }
            }
        }

        public override bool CanDamage() => false;
    }
}
