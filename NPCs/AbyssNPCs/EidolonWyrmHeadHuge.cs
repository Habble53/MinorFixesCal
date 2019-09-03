﻿using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.NPCs.AbyssNPCs
{
    public class EidolonWyrmHeadHuge : ModNPC
	{
		public bool detectsPlayer = false;
		public const int minLength = 40;
		public const int maxLength = 41;
		public float speed = 7.5f; //10
		public float turnSpeed = 0.15f; //0.15
		bool TailSpawned = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Eidolon Wyrm");
		}

		public override void SetDefaults()
		{
			npc.npcSlots = 24f;
			npc.damage = 1500;
			npc.width = 254; //36
			npc.height = 138; //20
			npc.defense = 3000;
			npc.lifeMax = 1000000;
			npc.aiStyle = -1;
			aiType = -1;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
			npc.knockBackResist = 0f;
			npc.value = Item.buyPrice(10, 0, 0, 0);
			npc.behindTiles = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath6;
			npc.netAlways = true;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(detectsPlayer);
			writer.Write(npc.chaseable);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			detectsPlayer = reader.ReadBoolean();
			npc.chaseable = reader.ReadBoolean();
		}

		public override void AI()
		{
			if (npc.justHit || (double)npc.life <= (double)npc.lifeMax * 0.98 || Main.player[npc.target].chaosState)
			{
				detectsPlayer = true;
				npc.damage = 1500;
			}
			else
			{
				npc.damage = 0;
			}
			npc.chaseable = detectsPlayer;
			if (detectsPlayer)
			{
				if (npc.soundDelay <= 0)
				{
					npc.soundDelay = 420;
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/EidolonWyrmRoarClose").WithVolume(2.5f), (int)npc.position.X, (int)npc.position.Y);
				}
			}
			else
			{
				if (Main.rand.Next(900) == 0)
				{
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/EidolonWyrmRoarClose").WithVolume(2.5f), (int)npc.position.X, (int)npc.position.Y);
				}
			}
			if (npc.ai[3] > 0f)
			{
				npc.realLife = (int)npc.ai[3];
			}
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
			{
				npc.TargetClosest(true);
			}
			npc.velocity.Length();
			if (Main.netMode != 1)
			{
				if (!TailSpawned && npc.ai[0] == 0f)
				{
					int Previous = npc.whoAmI;
					for (int num36 = 0; num36 < maxLength; num36++)
					{
						int lol = 0;
						if (num36 >= 0 && num36 < minLength)
						{
							if (num36 % 2 == 0)
							{
								lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), mod.NPCType("EidolonWyrmBodyHuge"), npc.whoAmI);
							}
							else
							{
								lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), mod.NPCType("EidolonWyrmBodyAltHuge"), npc.whoAmI);
							}
						}
						else
						{
							lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), mod.NPCType("EidolonWyrmTailHuge"), npc.whoAmI);
						}
						Main.npc[lol].realLife = npc.whoAmI;
						Main.npc[lol].ai[2] = (float)npc.whoAmI;
						Main.npc[lol].ai[1] = (float)Previous;
						Main.npc[Previous].ai[0] = (float)lol;
						NetMessage.SendData(23, -1, -1, null, lol, 0f, 0f, 0f, 0);
						Previous = lol;
					}
					TailSpawned = true;
				}
				if (detectsPlayer)
				{
					npc.localAI[0] += 1f;
					if (npc.localAI[0] >= 300f)
					{
						npc.localAI[0] = 0f;
						npc.TargetClosest(true);
						npc.netUpdate = true;
						int damage = Main.expertMode ? 300 : 400;
						float xPos = (Main.rand.Next(2) == 0 ? npc.position.X + 200f : npc.position.X - 200f);
						Vector2 vector2 = new Vector2(xPos, npc.position.Y + Main.rand.Next(-200, 201));
						int random = Main.rand.Next(3);
						if (random == 0)
						{
							Projectile.NewProjectile(vector2.X, vector2.Y, 0f, 0f, 465, damage, 0f, Main.myPlayer, 0f, 0f);
							Projectile.NewProjectile(-vector2.X, -vector2.Y, 0f, 0f, 465, damage, 0f, Main.myPlayer, 0f, 0f);
						}
						else if (random == 1)
						{
							Vector2 vec = Vector2.Normalize(Main.player[npc.target].Center - npc.Center);
							vec = Vector2.Normalize(Main.player[npc.target].Center - npc.Center + Main.player[npc.target].velocity * 20f);
							if (vec.HasNaNs())
							{
								vec = new Vector2((float)npc.direction, 0f);
							}
							for (int n = 0; n < 1; n++)
							{
								Vector2 vector4 = vec * 4f;
								Projectile.NewProjectile(vector2.X, vector2.Y, vector4.X, vector4.Y, 464, damage, 0f, Main.myPlayer, 0f, 1f);
								Projectile.NewProjectile(-vector2.X, -vector2.Y, -vector4.X, -vector4.Y, 464, damage, 0f, Main.myPlayer, 0f, 1f);
							}
						}
						else
						{
							if (Math.Abs(Main.player[npc.target].velocity.X) > 0.1f || Math.Abs(Main.player[npc.target].velocity.Y) > 0.1f)
							{
								Main.PlaySound(SoundID.Item117, Main.player[npc.target].position);
								for (int num621 = 0; num621 < 20; num621++)
								{
									int num622 = Dust.NewDust(new Vector2(Main.player[npc.target].position.X, Main.player[npc.target].position.Y),
										Main.player[npc.target].width, Main.player[npc.target].height, 185, 0f, 0f, 100, default(Color), 2f);
									Main.dust[num622].velocity *= 0.6f;
									if (Main.rand.Next(2) == 0)
									{
										Main.dust[num622].scale = 0.5f;
										Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
									}
								}
								for (int num623 = 0; num623 < 30; num623++)
								{
									int num624 = Dust.NewDust(new Vector2(Main.player[npc.target].position.X, Main.player[npc.target].position.Y),
										Main.player[npc.target].width, Main.player[npc.target].height, 185, 0f, 0f, 100, default(Color), 3f);
									Main.dust[num624].noGravity = true;
									num624 = Dust.NewDust(new Vector2(Main.player[npc.target].position.X, Main.player[npc.target].position.Y),
										Main.player[npc.target].width, Main.player[npc.target].height, 185, 0f, 0f, 100, default(Color), 2f);
									Main.dust[num624].velocity *= 0.2f;
								}
								if (Math.Abs(Main.player[npc.target].velocity.X) > 0.1f)
								{
									Main.player[npc.target].velocity.X = -Main.player[npc.target].velocity.X * 2f;
								}
								if (Math.Abs(Main.player[npc.target].velocity.Y) > 0.1f)
								{
									Main.player[npc.target].velocity.Y = -Main.player[npc.target].velocity.Y * 2f;
								}
							}
						}
					}
				}
			}
			if (npc.velocity.X < 0f)
			{
				npc.spriteDirection = -1;
			}
			else if (npc.velocity.X > 0f)
			{
				npc.spriteDirection = 1;
			}
			if (Main.player[npc.target].dead)
			{
				npc.TargetClosest(false);
			}
			npc.alpha -= 42;
			if (npc.alpha < 0)
			{
				npc.alpha = 0;
			}
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 6400f || !NPC.AnyNPCs(mod.NPCType("EidolonWyrmTailHuge")))
			{
				npc.active = false;
			}
			float num188 = speed;
			float num189 = turnSpeed;
			Vector2 vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
			float num191 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2);
			float num192 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2);
			int num42 = -1;
			int num43 = (int)(Main.player[npc.target].Center.X / 16f);
			int num44 = (int)(Main.player[npc.target].Center.Y / 16f);
			for (int num45 = num43 - 2; num45 <= num43 + 2; num45++)
			{
				for (int num46 = num44; num46 <= num44 + 15; num46++)
				{
					if (WorldGen.SolidTile2(num45, num46))
					{
						num42 = num46;
						break;
					}
				}
				if (num42 > 0)
				{
					break;
				}
			}
			if (num42 > 0)
			{
				num42 *= 16;
				float num47 = (float)(num42 - 600); //800
				if (!detectsPlayer)
				{
					num192 = num47;
					if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) < 400f) //500
					{
						if (npc.velocity.X > 0f)
						{
							num191 = Main.player[npc.target].Center.X + 500f; //600
						}
						else
						{
							num191 = Main.player[npc.target].Center.X - 500f; //600
						}
					}
				}
			}
			if (detectsPlayer)
			{
				num188 = 10f;
				num189 = 0.175f;
				if (!Main.player[npc.target].wet)
				{
					num188 = 20f;
					num189 = 0.25f;
				}
			}
			float num48 = num188 * 1.3f;
			float num49 = num188 * 0.7f;
			float num50 = npc.velocity.Length();
			if (num50 > 0f)
			{
				if (num50 > num48)
				{
					npc.velocity.Normalize();
					npc.velocity *= num48;
				}
				else if (num50 < num49)
				{
					npc.velocity.Normalize();
					npc.velocity *= num49;
				}
			}
			if (!detectsPlayer)
			{
				for (int num51 = 0; num51 < 200; num51++)
				{
					if (Main.npc[num51].active && Main.npc[num51].type == npc.type && num51 != npc.whoAmI)
					{
						Vector2 vector3 = Main.npc[num51].Center - npc.Center;
						if (vector3.Length() < 400f)
						{
							vector3.Normalize();
							vector3 *= 1000f;
							num191 -= vector3.X;
							num192 -= vector3.Y;
						}
					}
				}
			}
			else
			{
				for (int num52 = 0; num52 < 200; num52++)
				{
					if (Main.npc[num52].active && Main.npc[num52].type == npc.type && num52 != npc.whoAmI)
					{
						Vector2 vector4 = Main.npc[num52].Center - npc.Center;
						if (vector4.Length() < 60f)
						{
							vector4.Normalize();
							vector4 *= 200f;
							num191 -= vector4.X;
							num192 -= vector4.Y;
						}
					}
				}
			}
			num191 = (float)((int)(num191 / 16f) * 16);
			num192 = (float)((int)(num192 / 16f) * 16);
			vector18.X = (float)((int)(vector18.X / 16f) * 16);
			vector18.Y = (float)((int)(vector18.Y / 16f) * 16);
			num191 -= vector18.X;
			num192 -= vector18.Y;
			float num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
			float num196 = System.Math.Abs(num191);
			float num197 = System.Math.Abs(num192);
			float num198 = num188 / num193;
			num191 *= num198;
			num192 *= num198;
			if ((npc.velocity.X > 0f && num191 > 0f) || (npc.velocity.X < 0f && num191 < 0f) || (npc.velocity.Y > 0f && num192 > 0f) || (npc.velocity.Y < 0f && num192 < 0f))
			{
				if (npc.velocity.X < num191)
				{
					npc.velocity.X = npc.velocity.X + num189;
				}
				else
				{
					if (npc.velocity.X > num191)
					{
						npc.velocity.X = npc.velocity.X - num189;
					}
				}
				if (npc.velocity.Y < num192)
				{
					npc.velocity.Y = npc.velocity.Y + num189;
				}
				else
				{
					if (npc.velocity.Y > num192)
					{
						npc.velocity.Y = npc.velocity.Y - num189;
					}
				}
				if ((double)System.Math.Abs(num192) < (double)num188 * 0.2 && ((npc.velocity.X > 0f && num191 < 0f) || (npc.velocity.X < 0f && num191 > 0f)))
				{
					if (npc.velocity.Y > 0f)
					{
						npc.velocity.Y = npc.velocity.Y + num189 * 2f;
					}
					else
					{
						npc.velocity.Y = npc.velocity.Y - num189 * 2f;
					}
				}
				if ((double)System.Math.Abs(num191) < (double)num188 * 0.2 && ((npc.velocity.Y > 0f && num192 < 0f) || (npc.velocity.Y < 0f && num192 > 0f)))
				{
					if (npc.velocity.X > 0f)
					{
						npc.velocity.X = npc.velocity.X + num189 * 2f; //changed from 2
					}
					else
					{
						npc.velocity.X = npc.velocity.X - num189 * 2f; //changed from 2
					}
				}
			}
			else
			{
				if (num196 > num197)
				{
					if (npc.velocity.X < num191)
					{
						npc.velocity.X = npc.velocity.X + num189 * 1.1f; //changed from 1.1
					}
					else if (npc.velocity.X > num191)
					{
						npc.velocity.X = npc.velocity.X - num189 * 1.1f; //changed from 1.1
					}
					if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)num188 * 0.5)
					{
						if (npc.velocity.Y > 0f)
						{
							npc.velocity.Y = npc.velocity.Y + num189;
						}
						else
						{
							npc.velocity.Y = npc.velocity.Y - num189;
						}
					}
				}
				else
				{
					if (npc.velocity.Y < num192)
					{
						npc.velocity.Y = npc.velocity.Y + num189 * 1.1f;
					}
					else if (npc.velocity.Y > num192)
					{
						npc.velocity.Y = npc.velocity.Y - num189 * 1.1f;
					}
					if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)num188 * 0.5)
					{
						if (npc.velocity.X > 0f)
						{
							npc.velocity.X = npc.velocity.X + num189;
						}
						else
						{
							npc.velocity.X = npc.velocity.X - num189;
						}
					}
				}
			}
			npc.rotation = (float)System.Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) + 1.57f;
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			cooldownSlot = 1;
			return true;
		}

		public override bool? CanBeHitByProjectile(Projectile projectile)
		{
			if (projectile.minion)
			{
				return detectsPlayer;
			}
			return null;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
			{
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
			Vector2 center = new Vector2(npc.Center.X, npc.Center.Y);
			Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));
			Vector2 vector = center - Main.screenPosition;
			vector -= new Vector2((float)mod.GetTexture("NPCs/AbyssNPCs/EidolonWyrmHeadGlowHuge").Width, (float)(mod.GetTexture("NPCs/AbyssNPCs/EidolonWyrmHeadGlowHuge").Height / Main.npcFrameCount[npc.type])) * 1f / 2f;
			vector += vector11 * 1f + new Vector2(0f, 0f + 4f + npc.gfxOffY);
			Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.LightYellow);
			Main.spriteBatch.Draw(mod.GetTexture("NPCs/AbyssNPCs/EidolonWyrmHeadGlowHuge"), vector,
				new Microsoft.Xna.Framework.Rectangle?(npc.frame), color, npc.rotation, vector11, 1f, spriteEffects, 0f);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneAbyssLayer4 && spawnInfo.water && !NPC.AnyNPCs(mod.NPCType("EidolonWyrmHeadHuge")) && CalamityWorld.downedPolterghast)
			{
				return SpawnCondition.CaveJellyfish.Chance * 0.012f;
			}
			return 0f;
		}

		public override void NPCLoot()
		{
			Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Voidstone"), Main.rand.Next(80, 101));
			Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EidolicWail"));
			Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("SoulEdge"));
			Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Ectoplasm, Main.rand.Next(21, 33));
			Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Lumenite"), Main.rand.Next(50, 109));
			if (CalamityWorld.revenge)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("HalibutCannon"));
			}
			if (Main.expertMode && Main.rand.Next(2) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Lumenite"), Main.rand.Next(15, 28));
			}
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 4, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 15; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 4, hitDirection, -1f, 0, default(Color), 1f);
				}
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/WyrmAdult"), 1f);
			}
		}

		public override bool CheckActive()
		{
			if (detectsPlayer && !Main.player[npc.target].dead)
			{
				return false;
			}
			if (npc.timeLeft <= 0 && Main.netMode != 1)
			{
				for (int k = (int)npc.ai[0]; k > 0; k = (int)Main.npc[k].ai[0])
				{
					if (Main.npc[k].active)
					{
						Main.npc[k].active = false;
						if (Main.netMode == 2)
						{
							Main.npc[k].life = 0;
							Main.npc[k].netSkip = -1;
							NetMessage.SendData(23, -1, -1, null, k, 0f, 0f, 0f, 0, 0, 0);
						}
					}
				}
			}
			return true;
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(mod.BuffType("CrushDepth"), 1200, true);
			if (CalamityWorld.revenge)
			{
				player.AddBuff(mod.BuffType("Horror"), 1200, true);
				player.AddBuff(mod.BuffType("MarkedforDeath"), 1200);
			}
		}
	}
}
