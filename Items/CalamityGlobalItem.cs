using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Utilities;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Items
{
	public class CalamityGlobalItem : GlobalItem
	{
        public override bool OnPickup(Item item, Player player)
        {
            if (item.type == ItemID.Heart || item.type == ItemID.CandyApple || item.type == ItemID.CandyCane)
            {
                if (NPC.AnyNPCs(mod.NPCType("SupremeCalamitas")))
                {
                    player.statLife -= 10;
                    if (Main.myPlayer == player.whoAmI)
                    {
                        player.HealEffect(-10, true);
                    }
                }
            }
            return true;
        }

        public override bool CanUseItem(Item item, Player player)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            if ((item.type == ItemID.SuperAbsorbantSponge || item.type == ItemID.EmptyBucket) && modPlayer.ZoneAbyss)
            {
                return false;
            }
            if (item.type == ItemID.MagicMirror || item.type == ItemID.IceMirror || item.type == ItemID.CellPhone || item.type == ItemID.RecallPotion)
            {
                return !NPC.AnyNPCs(mod.NPCType("SupremeCalamitas"));
            }
            if (CalamityWorld.revenge)
            {
                if (item.type == ItemID.RodofDiscord && modPlayer.scarfCooldown)
                {
                    player.AddBuff(BuffID.ChaosState, 720);
                }
            }
            return true;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ItemID.SuperAbsorbantSponge)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Capable of soaking up an endless amount of water\n" +
                            "Cannot be used in the Abyss";
                    }
                }
            }
            if (item.type == ItemID.EmptyBucket)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Defense")
                    {
                        line2.text = "1 defense\n" +
                            "Cannot be used in the Abyss";
                    }
                }
            }
            if (item.type == ItemID.CrimsonHeart)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Summons a heart to provide light\n" +
                            "Provides a small amount of light in the abyss";
                    }
                }
            }
            if (item.type == ItemID.ShadowOrb)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Creates a magical shadow orb\n" +
                            "Provides a small amount of light in the abyss";
                    }
                }
            }
            if (item.type == ItemID.MagicLantern)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Summons a magic lantern that exposes nearby treasure\n" +
                            "Provides a small amount of light in the abyss";
                    }
                }
            }
            if (item.type == ItemID.ArcticDivingGear)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
                    {
                        line2.text = "Provides light under water and extra mobility on ice\n" +
                            "Provides a small amount of light in the abyss\n" +
                            "Moderately reduces breath loss in the abyss";
                    }
                }
            }
            if (item.type == ItemID.JellyfishNecklace)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Provides light under water\n" +
                            "Provides a small amount of light in the abyss";
                    }
                }
            }
            if (item.type == ItemID.JellyfishDivingGear)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
                    {
                        line2.text = "Provides light under water\n" +
                            "Provides a small amount of light in the abyss";
                    }
                }
            }
            if (item.type == ItemID.FairyBell)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Summons a magical fairy\n" +
                            "Provides a moderate amount of light in the abyss";
                    }
                }
            }
            if (item.type == ItemID.DD2PetGhost)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Summons a pet flickerwick to provide light\n" +
                            "Provides a moderate amount of light in the abyss";
                    }
                }
            }
            if (item.type == ItemID.ShinePotion)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "BuffTime")
                    {
                        line2.text = "5 minute duration\n" +
                            "Provides a moderate amount of light in the abyss";
                    }
                }
            }
            if (item.type == ItemID.WispinaBottle)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Summons a Wisp to provide light\n" +
                            "Provides a large amount of light in the abyss";
                    }
                }
            }
            if (item.type == ItemID.SuspiciousLookingTentacle)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
                    {
                        line2.text = "'I know what you're thinking...'\n" +
                            "Provides a large amount of light in the abyss";
                    }
                }
            }
            if (item.type == ItemID.GillsPotion)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "BuffTime")
                    {
                        line2.text = "2 minute duration\n" +
                            "Greatly reduces breath loss in the abyss";
                    }
                }
            }
            if (item.type == ItemID.DivingHelmet)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Greatly extends underwater breathing\n" +
                            "Moderately reduces breath loss in the abyss";
                    }
                }
            }
            if (item.type == ItemID.NeptunesShell)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Transforms the holder into merfolk when entering water\n" +
                            "Greatly reduces breath loss in the abyss";
                    }
                }
            }
            if (item.type == ItemID.MoonShell)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip0")
                    {
                        line2.text = "Turns the holder into a werewolf at night and a merfolk when entering water\n" +
                            "Greatly reduces breath loss in the abyss";
                    }
                }
            }
            if (item.type == ItemID.CelestialEmblem)
            {
                foreach (TooltipLine line2 in tooltips)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
                    {
                        line2.text = "Minor increases to all stats\n" +
                            "Greatly reduces breath loss in the abyss";
                    }
                }
            }
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            if (item.type == ItemID.JellyfishNecklace || item.type == ItemID.JellyfishDivingGear || item.type == ItemID.ArcticDivingGear)
            {
                modPlayer.jellyfishNecklace = true;
            }
        }
    }
}
