using CalamityMod.Items.Placeables.Banners;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AquaticScourge
{
    public class AquaticSeekerTail : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aquatic Seeker");
        }

        public override void SetDefaults()
        {
            NPC.damage = 5;
            NPC.width = 16;
            NPC.height = 22;
            NPC.defense = 20;
            NPC.lifeMax = 60;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.alpha = 255;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.netAlways = true;
            NPC.dontCountMe = true;
            Banner = ModContent.NPCType<AquaticSeekerHead>();
            BannerItem = ModContent.ItemType<AquaticSeekerBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead)
                NPC.TargetClosest(true);

            // Check if other segments are still alive, if not, die
            bool shouldDespawn = true;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<AquaticSeekerHead>())
                {
                    shouldDespawn = false;
                    break;
                }
            }
            if (!shouldDespawn)
            {
                if (NPC.ai[1] <= 0f)
                    shouldDespawn = true;
                else if (Main.npc[(int)NPC.ai[1]].life <= 0)
                    shouldDespawn = true;
            }
            if (shouldDespawn)
            {
                NPC.life = 0;
                NPC.HitEffect(0, 10.0);
                NPC.checkDead();
                NPC.active = false;
            }

            if (Main.npc[(int)NPC.ai[1]].alpha < 128)
            {
                NPC.alpha -= 42;
                if (NPC.alpha < 0)
                    NPC.alpha = 0;
            }

            if (Main.player[NPC.target].dead)
                NPC.TargetClosest(false);

            Vector2 vector18 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
            float num191 = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2);
            float num192 = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2);
            num191 = (float)((int)(num191 / 16f) * 16);
            num192 = (float)((int)(num192 / 16f) * 16);
            vector18.X = (float)((int)(vector18.X / 16f) * 16);
            vector18.Y = (float)((int)(vector18.Y / 16f) * 16);
            num191 -= vector18.X;
            num192 -= vector18.Y;
            float num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
            if (NPC.ai[1] > 0f && NPC.ai[1] < (float)Main.npc.Length)
            {
                try
                {
                    vector18 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                    num191 = Main.npc[(int)NPC.ai[1]].position.X + (float)(Main.npc[(int)NPC.ai[1]].width / 2) - vector18.X;
                    num192 = Main.npc[(int)NPC.ai[1]].position.Y + (float)(Main.npc[(int)NPC.ai[1]].height / 2) - vector18.Y;
                } catch
                {
                }
                NPC.rotation = (float)System.Math.Atan2((double)num192, (double)num191) + 1.57f;
                num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
                int num194 = NPC.width;
                num193 = (num193 - (float)num194) / num193;
                num191 *= num193;
                num192 *= num193;
                NPC.velocity = Vector2.Zero;
                NPC.position.X = NPC.position.X + num191;
                NPC.position.Y = NPC.position.Y + num192;

                if (num191 < 0f)
                    NPC.spriteDirection = 1;
                else if (num191 > 0f)
                    NPC.spriteDirection = -1;
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
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/AquaticScourgeGores/AquaticSeekerTail"), 1f);
            }
        }
    }
}
