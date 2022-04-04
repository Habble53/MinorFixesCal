﻿using CalamityMod.Items.Placeables.Banners;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class ArmoredDiggerBody : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Armored Digger");
        }

        public override void SetDefaults()
        {
            NPC.damage = 70;
            NPC.width = 38;
            NPC.height = 38;
            NPC.defense = 20;
            NPC.DR_NERD(0.2f);
            NPC.lifeMax = 20000;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.netAlways = true;
            NPC.dontCountMe = true;
            Banner = ModContent.NPCType<ArmoredDiggerHead>();
            BannerItem = ModContent.ItemType<ArmoredDiggerBanner>();
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            if (NPC.ai[3] > 0f)
            {
                NPC.realLife = (int)NPC.ai[3];
            }
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest(true);
            }
            bool flag = false;
            if (NPC.ai[1] <= 0f)
            {
                flag = true;
            }
            else if (Main.npc[(int)NPC.ai[1]].life <= 0)
            {
                flag = true;
            }
            if (flag)
            {
                NPC.life = 0;
                NPC.HitEffect(0, 10.0);
                NPC.checkDead();
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.localAI[0] += (float)Main.rand.Next(4);
                if (NPC.localAI[0] >= (float)Main.rand.Next(1800, 26000))
                {
                    NPC.localAI[0] = 0f;
                    NPC.TargetClosest(true);
                    if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position,
                        Main.player[NPC.target].width, Main.player[NPC.target].height))
                    {
                        float speed = 7f;
                        Vector2 vector = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)(NPC.height / 2));
                        float num6 = Main.player[NPC.target].position.X + (float)Main.player[NPC.target].width * 0.5f - vector.X + (float)Main.rand.Next(-20, 21);
                        float num7 = Main.player[NPC.target].position.Y + (float)Main.player[NPC.target].height * 0.5f - vector.Y + (float)Main.rand.Next(-20, 21);
                        float num8 = (float)Math.Sqrt((double)(num6 * num6 + num7 * num7));
                        num8 = speed / num8;
                        num6 *= num8;
                        num7 *= num8;
                        int num9 = 30;
                        int num10 = ProjectileID.SaucerScrap;
                        vector.X += num6 * 5f;
                        vector.Y += num7 * 5f;
                        Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), vector.X, vector.Y, num6, num7, num10, num9, 0f, Main.myPlayer, 0f, 0f);
                        NPC.netUpdate = true;
                    }
                }
            }
            Vector2 vector3 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
            float num20 = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2);
            float num21 = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2);
            num20 = (float)((int)(num20 / 16f) * 16);
            num21 = (float)((int)(num21 / 16f) * 16);
            vector3.X = (float)((int)(vector3.X / 16f) * 16);
            vector3.Y = (float)((int)(vector3.Y / 16f) * 16);
            num20 -= vector3.X;
            num21 -= vector3.Y;
            float num22 = (float)Math.Sqrt((double)(num20 * num20 + num21 * num21));
            if (NPC.ai[1] > 0f && NPC.ai[1] < (float)Main.npc.Length)
            {
                try
                {
                    vector3 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                    num20 = Main.npc[(int)NPC.ai[1]].position.X + (float)(Main.npc[(int)NPC.ai[1]].width / 2) - vector3.X;
                    num21 = Main.npc[(int)NPC.ai[1]].position.Y + (float)(Main.npc[(int)NPC.ai[1]].height / 2) - vector3.Y;
                } catch
                {
                }
                NPC.rotation = (float)Math.Atan2((double)num21, (double)num20) + 1.57f;
                num22 = (float)Math.Sqrt((double)(num20 * num20 + num21 * num21));
                int num23 = (int)(44f * NPC.scale);
                num22 = (num22 - (float)num23) / num22;
                num20 *= num22;
                num21 *= num22;
                NPC.velocity = Vector2.Zero;
                NPC.position.X = NPC.position.X + num20;
                NPC.position.Y = NPC.position.Y + num21;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Fire, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Fire, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreNPCLoot()
        {
            return false;
        }
    }
}
