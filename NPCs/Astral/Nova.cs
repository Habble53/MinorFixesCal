﻿using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.Astral
{
    public class Nova : ModNPC
    {
        private static Texture2D glowmask;

        private float travelAcceleration = 0.2f;
        private float targetTime = 120f;
        private const float waitBeforeTravel = 20f;
        private const float maxTravelTime = 300f;
        private const float slowdown = 0.84f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nova");
            Main.npcFrameCount[NPC.type] = 8;

            if (!Main.dedServ)
                glowmask = ModContent.Request<Texture2D>("CalamityMod/NPCs/Astral/NovaGlow").Value;
        }

        public override void SetDefaults()
        {
            NPC.width = 78;
            NPC.height = 50;
            NPC.damage = 45;
            NPC.defense = 15;
            NPC.DR_NERD(0.15f);
            NPC.lifeMax = 230;
            NPC.DeathSound = Mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/AstralEnemyDeath");
            NPC.noGravity = true;
            NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 20, 0);
            NPC.aiStyle = -1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<NovaBanner>();
            if (DownedBossSystem.downedAstrageldon)
            {
                NPC.damage = 75;
                NPC.defense = 25;
                NPC.knockBackResist = 0.4f;
                NPC.lifeMax = 350;
            }
            if (CalamityWorld.death)
            {
                travelAcceleration = 0.3f;
                targetTime = 60f;
            }
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.ai[3] >= 0f)
            {
                if (NPC.frameCounter >= 8)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y >= frameHeight * 4)
                    {
                        NPC.frame.Y = 0;
                    }
                }
            }
            else
            {
                if (NPC.frameCounter >= 7)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y >= frameHeight * 8)
                    {
                        NPC.frame.Y = frameHeight * 4;
                    }
                }
            }

            //DO DUST
            Dust d = CalamityGlobalNPC.SpawnDustOnNPC(NPC, 114, frameHeight, ModContent.DustType<AstralOrange>(), new Rectangle(78, 34, 36, 18), Vector2.Zero, 0.45f, true);
            if (d != null)
            {
                d.customData = 0.04f;
            }
        }

        public override void AI()
        {
            Player target = Main.player[NPC.target];
            if (NPC.ai[3] >= 0)
            {
                CalamityGlobalNPC.DoFlyingAI(NPC, (CalamityWorld.death ? 8.25f : 5.5f), (CalamityWorld.death ? 0.0525f : 0.035f), 400f, 150, false);

                if (Collision.CanHit(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height))
                {
                    NPC.ai[3]++;
                }
                else
                {
                    NPC.ai[3] = 0f;
                }

                Vector2 between = target.Center - NPC.Center;

                //after locking target for x amount of time and being far enough away
                int random = CalamityWorld.death ? 90 : 180;
                if (between.Length() > 150 && NPC.ai[3] >= targetTime && Main.rand.NextBool(random))
                {
                    //set ai mode to target and travel
                    NPC.ai[3] = -1f;
                }
                return;
            }
            else
            {
                NPC.ai[3]--;
                Vector2 between = target.Center - NPC.Center;

                if (NPC.ai[3] < -waitBeforeTravel)
                {
                    if (NPC.collideX || NPC.collideY || NPC.ai[3] < -maxTravelTime)
                    {
                        Explode();
                    }

                    NPC.velocity += new Vector2(NPC.ai[1], NPC.ai[2]) * travelAcceleration; //acceleration per frame

                    //rotation
                    NPC.rotation = NPC.velocity.ToRotation();
                }
                else if (NPC.ai[3] == -waitBeforeTravel)
                {
                    between.Normalize();
                    NPC.ai[1] = between.X;
                    NPC.ai[2] = between.Y;

                    //rotation
                    NPC.rotation = between.ToRotation();
                    NPC.velocity = Vector2.Zero;
                }
                else
                {
                    //slowdown
                    NPC.velocity *= slowdown;

                    //rotation
                    NPC.rotation = between.ToRotation();
                }
                NPC.rotation += MathHelper.Pi;
            }
        }

        private void Explode()
        {
            //kill NPC
            SoundEngine.PlaySound(SoundID.Item14, NPC.Center);

            //change stuffs
            Vector2 center = NPC.Center;
            NPC.width = 200;
            NPC.height = 200;
            NPC.Center = center;

            Rectangle myRect = NPC.getRect();

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 200; i++)
                {
                    if (Main.player[i].getRect().Intersects(myRect))
                    {
                        int direction = NPC.Center.X - Main.player[i].Center.X < 0 ? -1 : 1;
                        Main.player[i].Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), NPC.damage, direction);
                    }
                }
            }

            //other things
            NPC.ai[3] = -20000f;
            NPC.value = 0f;
            NPC.extraValue = 0f;
            NPC.StrikeNPCNoInteraction(9999, 1f, 1);

            int size = 30;
            Vector2 off = new Vector2(size / -2f);

            for (int i = 0; i < 45; i++)
            {
                int dust = Dust.NewDust(NPC.Center - off, size, size, ModContent.DustType<AstralEnemy>(), Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f), 0, default, Main.rand.NextFloat(1f, 2f));
                Main.dust[dust].velocity *= 1.4f;
            }
            for (int i = 0; i < 15; i++)
            {
                int dust = Dust.NewDust(NPC.Center - off, size, size, 31, 0f, 0f, 100, default, 1.7f);
                Main.dust[dust].velocity *= 1.4f;
            }
            for (int i = 0; i < 27; i++)
            {
                int dust = Dust.NewDust(NPC.Center - off, size, size, 6, 0f, 0f, 100, default, 2.4f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 5f;
                dust = Dust.NewDust(NPC.Center - off, size, size, 6, 0f, 0f, 100, default, 1.6f);
                Main.dust[dust].velocity *= 3f;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 15;
                switch (Main.rand.Next(3))
                {
                    case 0:
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/NPCHit/AstralEnemyHit"), NPC.Center);
                        break;
                    case 1:
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/NPCHit/AstralEnemyHit2"), NPC.Center);
                        break;
                    case 2:
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/NPCHit/AstralEnemyHit3"), NPC.Center);
                        break;
                }
            }

            CalamityGlobalNPC.DoHitDust(NPC, hitDirection, (Main.rand.Next(0, Math.Max(0, NPC.life)) == 0) ? 5 : ModContent.DustType<AstralEnemy>(), 1f, 3, 40);

            //if dead do gores
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        Gore.NewGore(NPC.Center, NPC.velocity * 0.3f, Mod.Find<ModGore>("Gores/Nova/NovaGore" + i).Type);
                    }
                }
            }
        }

        public override bool PreNPCLoot()
        {
            return NPC.ai[3] > -10000;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.Draw(glowmask, NPC.Center - screenPos - new Vector2(0, 4f), NPC.frame, Color.White * 0.75f, NPC.rotation, new Vector2(57f, 37f), NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.Player))
            {
                return 0f;
            }
            else if (spawnInfo.Player.InAstral(1))
            {
                return 0.19f;
            }
            return 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120, true);
        }

        public override void NPCLoot()
        {
            DropHelper.DropItem(NPC, ModContent.ItemType<Stardust>(), 2, 3);
            DropHelper.DropItemCondition(NPC, ModContent.ItemType<Stardust>(), Main.expertMode);
            DropHelper.DropItemCondition(NPC, ModContent.ItemType<StellarCannon>(), DownedBossSystem.downedAstrageldon, 7, 1, 1);
            DropHelper.DropItemChance(NPC, ModContent.ItemType<GloriousEnd>(), 7, 1, 1);
        }
    }
}
