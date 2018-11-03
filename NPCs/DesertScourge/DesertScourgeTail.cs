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
	public class DesertScourgeTail : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Desert Scourge");
		}
		
		public override void SetDefaults()
		{
			npc.damage = 9;
			npc.npcSlots = 5f;
			npc.width = 32; //324
			npc.height = 48; //216
			npc.defense = 18;
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
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 3; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScourgeTail"), 1f);
				for (int k = 0; k < 10; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
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
		
		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.7f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.8f);
		}
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			if (Main.expertMode)
			{
				player.AddBuff(BuffID.Bleeding, 100, true);
			}
		}
	}
}