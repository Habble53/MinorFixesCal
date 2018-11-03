﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Accessories
{
    public class EtherealTalisman : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ethereal Talisman");
            Tooltip.SetDefault("15% increased magic damage, critical strike chance, and 20% decreased mana usage\n" +
                "+150 max mana\n" +
                "Increases life regen and reveals treasure locations\n" +
                "Reduces the cooldown of healing potions\n" +
                "You automatically use mana potions when needed\n" +
                "Magic attacks have a chance to instantly kill normal enemies");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.value = 10000000;
            item.accessory = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.eTalisman = true;
            player.findTreasure = true;
            player.pStone = true;
            player.manaFlower = true;
            player.lifeRegen += 2;
            player.statManaMax2 += 150;
            player.magicDamage += 0.15f;
            player.manaCost *= 0.8f;
            player.magicCrit += 15;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "SigilofCalamitas");
            recipe.AddIngredient(ItemID.ManaFlower);
            recipe.AddIngredient(null, "Phantoplasm", 20);
            recipe.AddIngredient(null, "NightmareFuel", 20);
            recipe.AddIngredient(null, "EndothermicEnergy", 20);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}