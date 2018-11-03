﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.Leviathan
{
	public class LeviathanStart : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("???");
			Main.npcFrameCount[npc.type] = 4;
		}
		
		public override void SetDefaults()
		{
			npc.aiStyle = -1;
			aiType = -1;
			npc.damage = 0;
			npc.width = 120; //324
			npc.height = 120; //216
			npc.defense = 0;
			npc.lifeMax = 3000;
			npc.knockBackResist = 0f;
			npc.value = 0f;
			npc.noGravity = true;
			npc.chaseable = false;
			npc.HitSound = SoundID.NPCHit1;
			npc.rarity = 2;
			music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/SirenLure");
		}
		
		public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.1f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }
		
		public override void AI()
		{
			npc.TargetClosest(true);
			Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0f, 0.5f, 0.3f);
			for (int num569 = 0; num569 < 200; num569++)
			{
				if (Main.npc[num569].active && Main.npc[num569].boss)
				{
					npc.active = false;
				}
			}
		}
		
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.playerSafe || 
                NPC.AnyNPCs(NPCID.DukeFishron) || 
                NPC.AnyNPCs(mod.NPCType("LeviathanStart")) || 
                NPC.AnyNPCs(mod.NPCType("Siren")) || 
                NPC.AnyNPCs(mod.NPCType("Leviathan")) || 
                spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneSulphur)
			{
				return 0f;
			}
            if (!Main.hardMode)
            {
                return SpawnCondition.OceanMonster.Chance * 0.025f;
            }
            if (!NPC.downedPlantBoss && !CalamityWorld.downedCalamitas)
			{
				return SpawnCondition.OceanMonster.Chance * 0.1f;
			}
			return SpawnCondition.OceanMonster.Chance * 0.4f;
		}
		
		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = 3000;
			npc.damage = 0;
		}

        public override void NPCLoot()
        {
            if (CalamityWorld.revenge && Main.rand.Next(4) == 0)
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, (Main.rand.Next(2) == 0 ? mod.ItemType("SirensHeart") : mod.ItemType("SirensHeartAlt")));
            }
        }

        public override void HitEffect(int hitDirection, double damage)
		{
			if (npc.life > 0)
			{
				for (int k = 0; k < 5; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
			else
			{
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 67, hitDirection, -1f, 0, default(Color), 1f);
				}
				if (Main.netMode != 1)
				{
					NPC.NewNPC((int)npc.Center.X, (int)npc.position.Y + npc.height, mod.NPCType("Siren"), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
				}
			}
		}
	}
}