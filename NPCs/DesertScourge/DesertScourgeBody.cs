﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.DesertScourge
{
	public class DesertScourgeBody : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Desert Scourge");
		}
		
		public override void SetDefaults()
		{
			npc.damage = 12; //70
			npc.npcSlots = 5f;
			npc.width = 32; //324
			npc.height = 36; //216
			npc.defense = 8;
            npc.lifeMax = CalamityWorld.revenge ? 2500 : 2300;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 4900;
            }
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 8400000 : 7500000;
            }
            npc.aiStyle = 6; //new
            aiType = -1; //new
            animationType = 10; //new
			npc.knockBackResist = 0f;
			npc.alpha = 255;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
			npc.boss = true;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/DesertScourge");
            npc.behindTiles = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.netAlways = true;
			npc.dontCountMe = true;
			if (Main.expertMode)
			{
				npc.scale = 1.15f;
			}
		}
		
		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}
		
		public override void AI()
		{
            Player player = Main.player[npc.target];
            npc.dontTakeDamage = !player.ZoneDesert && !CalamityWorld.bossRushActive;
            if (!Main.npc[(int)npc.ai[1]].active)
            {
                npc.life = 0;
                npc.HitEffect(0, 10.0);
                npc.active = false;
            }
			if (Main.npc[(int)npc.ai[1]].alpha < 128)
			{
				npc.alpha -= 42;
				if (npc.alpha < 0)
				{
					npc.alpha = 0;
				}
			}
            if (Main.netMode != 1 && (CalamityWorld.revenge || CalamityWorld.bossRushActive))
            {
                npc.localAI[0] += (float)Main.rand.Next(4);
                if (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
                {
                    npc.localAI[0] += 4f;
                }
                if (npc.localAI[0] >= (float)Main.rand.Next(1400, 26000))
                {
                    npc.localAI[0] = 0f;
                    npc.TargetClosest(true);
                    if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                    {
                        Vector2 vector104 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)(npc.height / 2));
                        float num942 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector104.X + (float)Main.rand.Next(-20, 21);
                        float num943 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector104.Y + (float)Main.rand.Next(-20, 21);
                        float num944 = (float)Math.Sqrt((double)(num942 * num942 + num943 * num943));
                        int projectileType = mod.ProjectileType("SandBlast");
                        int damage = 15;
                        float num941 = (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged ? 15f : 6f);
                        num944 = num941 / num944;
                        num942 *= num944;
                        num943 *= num944;
                        num942 += (float)Main.rand.Next(-5, 6) * 0.05f;
                        num943 += (float)Main.rand.Next(-5, 6) * 0.05f;
                        vector104.X += num942 * 5f;
                        vector104.Y += num943 * 5f;
                        if (Main.rand.Next(2) == 0)
                        {
                            Projectile.NewProjectile(vector104.X, vector104.Y, num942, num943, projectileType, damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        npc.netUpdate = true;
                    }
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
		
		public override void HitEffect(int hitDirection, double damage)
		{
			int wormCount = 5;
			if (npc.life <= (npc.lifeMax * 0.75f) && NPC.CountNPCS(mod.NPCType("DriedSeekerHead")) < wormCount)
			{
				if (Main.rand.Next(10) == 0 && Main.netMode != 1)
				{
					Vector2 spawnAt = npc.Center + new Vector2(0f, (float)npc.height / 2f);
					int seeker = NPC.NewNPC((int)spawnAt.X, (int)spawnAt.Y, mod.NPCType("DriedSeekerHead"));
					if (Main.netMode == 2 && seeker < 200)
					{
						NetMessage.SendData(23, -1, -1, null, seeker, 0f, 0f, 0f, 0, 0, 0);
					}
					npc.netUpdate = true;
				}
			}
			for (int k = 0; k < 3; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				float randomSpread = (float)(Main.rand.Next(-100, 100) / 100);
				Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/ScourgeBody"), 1f);
				Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/ScourgeBody2"), 1f);
				Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/ScourgeBody3"), 1f);
				for (int k = 0; k < 10; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
		
		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.8f);
		}
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			if (Main.expertMode)
			{
				player.AddBuff(BuffID.Bleeding, 200, true);
			}
		}
	}
}