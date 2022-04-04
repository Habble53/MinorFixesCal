﻿using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.Astral
{
    public class Atlas : ModNPC
    {
        //TODO -- draw 4 pixels lower than actual bottom position (arm overlaps tiles)

        private static Texture2D glowmask;

        //CONSTANTS
        private const int sheetHeight = 852;
        private const int startWalkFrameY = 710;
        private const float idle_walkMaxSpeed = 0.8f;
        private const float idle_walkAcceleration = 0.1f;
        private float target_walkMaxSpeed = 1.6f;
        private float target_walkAcceleration = 0.12f;
        private const float swing_minXDistance = 96f;
        private const float swing_minYDistance = 48f;
        private const int swing_minCounterHit = 16;
        private const int swing_maxCounterHit = 25; //the minimum and maximum values for the counter in which the swing's main hitbox is active.
        private const int swing_playSoundOnFrame = 15;

        private bool idling;
        private bool idle_impulseWalk;
        private bool idle_walkLeft;
        private bool idle_blocked;

        private bool swinging;
        private bool swingYeet;
        private byte swing_counter;
        private short swing_untilNext;

        private float idle_counter
        {
            get
            {
                return NPC.ai[0];
            }
            set
            {
                NPC.ai[0] = value;
            }
        }
        private float target_counter
        {
            get
            {
                return NPC.ai[1];
            }
            set
            {
                NPC.ai[1] = value;
            }
        }
        private float grounded_counter
        {
            get
            {
                return NPC.ai[2];
            }
            set
            {
                NPC.ai[2] = value;
            }
        }
        private Player Target
        {
            get
            {
                if (NPC.HasValidTarget)
                {
                    return Main.player[NPC.target];
                }
                return null;
            }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Atlas");
            Main.npcFrameCount[NPC.type] = 6;
            //not really important seeing as custom drawing, but for heights sake, 6
            //also it's visuals are messed up on npc spawners etc. because the sheet is 3 wide.
            //not much we can do. looks fine in-game so /shrug
            if (!Main.dedServ)
                glowmask = ModContent.Request<Texture2D>("CalamityMod/NPCs/Astral/AtlasGlow");
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.lavaImmune = true;
            NPC.width = 78;
            NPC.height = 88;
            NPC.damage = 70;
            NPC.defense = 40;
            NPC.DR_NERD(0.15f);
            NPC.lifeMax = 1200;
            NPC.knockBackResist = 0.08f;
            NPC.value = Item.buyPrice(0, 1, 0, 0);
            NPC.aiStyle = -1;
            NPC.DeathSound = Mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/AtlasDeath");
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<AtlasBanner>();
            if (CalamityWorld.downedAstrageldon)
            {
                NPC.damage = 100;
                NPC.defense = 50;
                NPC.knockBackResist = 0.04f;
                NPC.lifeMax = 1600;
            }
            if (NPC.downedAncientCultist)
            {
                NPC.damage = 150;
                NPC.defense = 75;
                NPC.knockBackResist = 0f;
                NPC.lifeMax = 2400;
            }
            if (CalamityWorld.death)
            {
                target_walkAcceleration = 0.18f;
                target_walkMaxSpeed = 3.2f;
            }
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            BitsByte bb = new BitsByte(idling, idle_impulseWalk, idle_walkLeft, idle_blocked, swinging, swingYeet);
            writer.Write(bb);
            writer.Write(swing_counter);
            writer.Write(swing_untilNext);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            BitsByte bb = reader.ReadByte();
            idling = bb[0];
            idle_impulseWalk = bb[1];
            idle_walkLeft = bb[2];
            idle_blocked = bb[3];
            swinging = bb[4];
            swingYeet = bb[5];

            swing_counter = reader.ReadByte();
            swing_untilNext = reader.ReadInt16();
        }

        public override void AI()
        {
            //PICK A TARGET
            if (!NPC.HasValidTarget || idling)
            {
                //target and reset
                NPC.TargetClosest(false);
                idling = false;

                if (!NPC.HasValidTarget) //if we don't have valid target
                {
                    idling = true;
                }
                else if (!Collision.CanHit(NPC.position + Vector2.UnitY * 8, 15, 15, Target.position, Target.width, Target.height)) //else if we do but can't see them
                {
                    idling = true;
                }
                else
                {
                    //PLAY SOUND (SAD) *but this time it has a target. more like an indicator. he sad because he has to kill you now :c
                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/AtlasSad0"), NPC.Center);
                }
            }

            if (NPC.velocity.Y == 0)
            {
                grounded_counter++;
            }
            else
            {
                grounded_counter = 0;
            }

            swing_untilNext--;

            if (idling)
            {
                DoIdleAI();
            }
            else if (swinging)
            {
                DoSwingUpdate();
            }
            else
            {
                DoTargetAI();
            }

            NPC.stepSpeed = 0.75f;

            if (NPC.velocity.Y >= 0f)
            {
                Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 1);
            }
        }

        private void DoSwingUpdate()
        {
            NPC.velocity.X *= 0.84f;
            swing_counter++;

            if (swing_counter == swing_playSoundOnFrame)
            {
                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/AtlasSwing"), NPC.Center);
            }

            if (swing_counter >= swing_minCounterHit && swing_counter <= swing_maxCounterHit)
            {
                int centerX = (int)NPC.Center.X;
                int centerY = (int)NPC.Center.Y;
                int width = (int)swing_minXDistance;
                int height = 142;
                Rectangle hitbox = new Rectangle(
                    centerX + (NPC.direction == -1 ? -width : 0),
                    centerY - height / 2,
                    width, height);

                for (int i = 0; i < Main.player.Length; i++)
                {
                    if (Main.player[i].active && !Main.player[i].dead && Main.player[i].getRect().Intersects(hitbox))
                    {
                        Vector2 before = Main.player[i].velocity;
                        Main.player[i].Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), NPC.damage, NPC.direction);
                        Vector2 after = Main.player[i].velocity;
                        Vector2 difference = after - before;

                        float horMult = 3.5f;
                        float verMult = 1.2f;

                        swingYeet = false;

                        if (Main.rand.NextBool(1000)) //Launch the player very fast very rarely
                        {
                            horMult = 12f;
                            verMult = 2.3f;
                            swingYeet = true;
                        }

                        if (Main.player[i].noKnockback)
                        {
                            horMult *= 0.5f;
                            verMult *= 0.5f;
                        }

                        difference *= new Vector2(horMult, verMult);

                        Main.player[i].velocity = before + difference;
                    }
                }
            }
        }

        private void DoIdleAI()
        {
            //idling, so I should mostly just walk around and do nothing useful with my life
            //oh hey, it's just me haha
            if (HoleBelow() || (NPC.collideX && NPC.oldPosition.X == NPC.position.X))
            {
                idle_impulseWalk = false;
                idle_blocked = true;
            }

            idle_counter++;
            if (idle_impulseWalk)
            {
                if (idle_counter == 4)
                {
                    NPC.frame.Y = startWalkFrameY; //reset frame to start walk animation.
                }

                NPC.velocity.X += idle_walkAcceleration * (idle_walkLeft ? -1 : 1);
                if (Math.Abs(NPC.velocity.X) > idle_walkMaxSpeed)
                {
                    NPC.velocity.X = idle_walkLeft ? -idle_walkMaxSpeed : idle_walkMaxSpeed;
                }
                idle_impulseWalk = idle_impulseWalk && idle_counter > 20 && Main.rand.Next(150) != 0;
                if (!idle_impulseWalk)
                {
                    idle_counter = 0;
                }

                NPC.direction = idle_walkLeft ? -1 : 1;
            }
            else
            {
                idle_impulseWalk = idle_counter > 100 && Main.rand.NextBool(400); //try to impulse
                if (idle_impulseWalk) //if we've just decided to stroll
                {
                    idle_counter = 0;
                    if (idle_blocked) //if we were blocked before, pick the other direction.
                    {
                        idle_blocked = false;
                        idle_walkLeft = !idle_walkLeft;
                    }
                    else
                    {
                        idle_walkLeft = Main.rand.NextBool(); //pick direction
                    }
                    NPC.frameCounter = 0; // reset framecounter to start walk.

                    //PLAY SOUND (IDLE)
                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/AtlasIdle" + Main.rand.Next(2)), NPC.Center);
                }
                else
                {
                    NPC.velocity.X *= 0.9f;
                    if (Math.Abs(NPC.velocity.X) < 0.1f)
                    {
                        NPC.velocity.X = 0f;
                    }
                }
            }
        }

        private void DoTargetAI()
        {
            //got a target, so I'm gonna chase them down and smack em.
            bool targetToLeft = Target.Center.X < NPC.Center.X;
            int mult = targetToLeft ? -1 : 1;

            NPC.velocity.X += target_walkAcceleration * mult;
            if (Math.Abs(NPC.velocity.X) > target_walkMaxSpeed)
            {
                NPC.velocity.X = target_walkMaxSpeed * mult;
            }

            //based on velocity, as he a big hunk and he can't walk backwards whilst facing target
            NPC.direction = NPC.velocity.X < 0f ? -1 : 1;

            //if have been on ground for at least 1.5 seonds, and are hitting wall or there is a hole
            if (grounded_counter > 90 && (HoleBelow() || (NPC.collideX && NPC.position.X == NPC.oldPosition.X)))
            {
                //jump
                NPC.velocity.Y = -6f;
                target_counter++;
            }

            bool canHitTarget = Collision.CanHit(NPC.position + Vector2.UnitY * 8, 15, 15, Target.position, Target.width, Target.height);

            if (target_counter > 0 && !canHitTarget)
            {
                target_counter++;
            }
            else
            {
                target_counter = 0;
            }

            if (target_counter > 180)
            {
                idling = true;
                target_counter = 0;

                //PLAY SOUND (SAD)        he sad cos he lost his target :C
                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/AtlasSad1"), NPC.Center);
            }
            else if (target_counter == 0 && NPC.velocity.Y == 0)
            {
                //check if we can swing at the target
                Vector2 distance = NPC.Center - Target.Center;
                if (swing_untilNext < 0 && canHitTarget && Math.Abs(distance.X) < swing_minXDistance && Math.Abs(distance.Y) < swing_minYDistance)
                {
                    StartSwing();
                }
            }
        }

        private bool HoleBelow()
        {
            //width of npc in tiles
            int tileWidth = 5;
            int tileX = (int)(NPC.Center.X / 16f) - tileWidth;
            if (NPC.velocity.X > 0) //if moving right
            {
                tileX += tileWidth;
            }
            int tileY = (int)((NPC.position.Y + NPC.height) / 16f);
            for (int y = tileY; y < tileY + 2; y++)
            {
                for (int x = tileX; x < tileX + tileWidth; x++)
                {
                    if (Main.tile[x, y].active())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void StartSwing()
        {
            swinging = true;
            swing_counter = 0;
            swing_untilNext = 300;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            //play sound
            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 15;
                switch (Main.rand.Next(3))
                {
                    case 0:
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/NPCHit/AtlasHurt0"), NPC.Center);
                        break;
                    case 1:
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/NPCHit/AtlasHurt1"), NPC.Center);
                        break;
                    case 2:
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/NPCHit/AtlasHurt2"), NPC.Center);
                        break;
                }
            }

            CalamityGlobalNPC.DoHitDust(NPC, hitDirection, (Main.rand.Next(0, Math.Max(0, NPC.life)) == 0) ? 5 : ModContent.DustType<AstralEnemy>(), 1f, 3, 30);

            //if dead do gores
            if (NPC.life <= 0)
            {
                //head
                Gore.NewGore(NPC.Top, NPC.velocity * 0.5f, Mod.GetGoreSlot("Gores/Atlas/AtlasGore4"));
                //hand
                Gore.NewGore(NPC.direction == 1 ? NPC.Right : NPC.Left, NPC.velocity * 0.5f, Mod.GetGoreSlot("Gores/Atlas/AtlasGore2"));
                //rest
                Gore.NewGore(NPC.Center, NPC.velocity * 0.5f, Mod.GetGoreSlot("Gores/Atlas/AtlasGore0"));
                Gore.NewGore(NPC.Center, NPC.velocity * 0.5f, Mod.GetGoreSlot("Gores/Atlas/AtlasGore1"));
                Gore.NewGore(NPC.Center, NPC.velocity * 0.5f, Mod.GetGoreSlot("Gores/Atlas/AtlasGore3"));
                Gore.NewGore(NPC.Center, NPC.velocity * 0.5f, Mod.GetGoreSlot("Gores/Atlas/AtlasGore5"));
                Gore.NewGore(NPC.Center, NPC.velocity * 0.5f, Mod.GetGoreSlot("Gores/Atlas/AtlasGore6"));
            }
        }

        public override void FindFrame(int frameHeight)
        {
            int width = 142;
            int height = 142;

            //ensure width and height are set.
            NPC.frame.Width = width;
            NPC.frame.Height = height;

            if (idling)
            {
                NPC.frameCounter++;
                if (!idle_impulseWalk)
                {
                    NPC.frame.X = 0;
                    if (NPC.frameCounter > 9)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += height;
                        if (NPC.frame.Y >= sheetHeight)
                        {
                            NPC.frame.Y = 0;
                        }
                    }
                }
                else
                {
                    NPC.frame.X = width;
                    if (NPC.frameCounter > 8)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += height;
                        if (NPC.frame.Y >= sheetHeight)
                        {
                            NPC.frame.Y = 0;
                        }
                    }
                }
            }
            else if (swinging)
            {
                NPC.frameCounter = 0;
                NPC.frame.X = width * 2;
                if (swing_counter < 8)
                {
                    NPC.frame.Y = 0;
                }
                else if (swing_counter < 16)
                {
                    NPC.frame.Y = height;
                }
                else if (swing_counter < 23)
                {
                    NPC.frame.Y = height * 2;
                }
                else if (swing_counter < 30)
                {
                    NPC.frame.Y = height * 3;
                }
                else if (swing_counter < 38)
                {
                    NPC.frame.Y = height * 4;
                }
                else if (swing_counter < 46)
                {
                    NPC.frame.Y = height * 5;
                }
                else
                {
                    swinging = false;
                    swing_counter = 0;
                    idling = false;
                }
            }
            else
            {
                //if walking
                NPC.frameCounter++;
                NPC.frame.X = width;
                if (NPC.frameCounter > 7)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += height;
                    if (NPC.frame.Y >= sheetHeight)
                    {
                        NPC.frame.Y = 0;
                    }
                }
            }

            //if we're in the air
            if (NPC.velocity.Y != 0)
            {
                NPC.frame.X = width;
                NPC.frame.Y = height * 4; //5th frame down
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Vector2 position = NPC.position - new Vector2(30, 48) - Main.screenPosition;
            SpriteEffects effect = NPC.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            //draw actual sprite
            spriteBatch.Draw(Main.npcTexture[NPC.type], position, NPC.frame,
                drawColor, 0f, default, 1f, //color, rotation, origin, scale
                effect, 0f); //effect, drawlayer

            //draw glowmask
            spriteBatch.Draw(
                glowmask, position, NPC.frame,
                Color.White * 0.65f, 0f, default, 1f, //color, rotation, origin, scale
                effect, 0f); //effect, drawlayer

            return false;
        }

        private Vector2 GetEyePos()
        {
            int x = 0;
            int y = 0;
            switch (NPC.frame.Y)
            {
                case 0:
                    x = 45;
                    y = 79;
                    break;
                case 142:
                    x = 41;
                    y = 79;
                    break;
                case 284:
                    x = 35;
                    y = 75;
                    break;
                case 426:
                    x = 25;
                    y = 73;
                    break;
                case 568:
                    x = 31;
                    y = 77;
                    break;
                case 710:
                    x = 43;
                    y = 81;
                    break;
            }
            if (NPC.direction == 1)
            {
                x = 142 - x;
            }
            return new Vector2(x, y);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.Player))
            {
                return 0f;
            }
            else if (spawnInfo.Player.InAstral(1) && NPC.downedAncientCultist && !CalamityWorld.downedStarGod)
            {
                return 0.27f;
            }
            else if (spawnInfo.Player.InAstral(1))
            {
                return 0.09f;
            }
            return 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 300, true);
        }

        public override void NPCLoot()
        {
            int minStardust = Main.expertMode ? 7 : 6;
            int maxStardust = Main.expertMode ? 9 : 8;
            DropHelper.DropItem(NPC, ModContent.ItemType<Stardust>(), minStardust, maxStardust);

            DropHelper.DropItemCondition(NPC, ModContent.ItemType<TitanArm>(), CalamityWorld.downedAstrageldon, 7, 1, 1);
            DropHelper.DropItem(NPC, ModContent.ItemType<TitanHeart>());
        }
    }
}
