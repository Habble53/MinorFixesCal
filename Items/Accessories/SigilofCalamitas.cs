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
    public class SigilofCalamitas : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sigil of Calamitas");
            Tooltip.SetDefault("10% increased magic damage, critical strike chance, and 15% decreased mana usage\n" +
                "+100 max mana, increases life regen, and reveals treasure locations\n" +
                "Reduces the cooldown of healing potions");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 8));
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.value = 5000000;
            item.rare = 9;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.findTreasure = true;
            player.pStone = true;
            player.lifeRegen += 1;
            player.statManaMax2 += 100;
            player.magicDamage += 0.1f;
            player.manaCost *= 0.85f;
            player.magicCrit += 10;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CharmofMyths);
            recipe.AddIngredient(ItemID.SorcererEmblem);
            recipe.AddIngredient(ItemID.CrystalShard, 20);
            recipe.AddIngredient(null, "CalamityDust", 5);
            recipe.AddIngredient(null, "CoreofChaos", 5);
            recipe.AddIngredient(ItemID.SpellTome);
            recipe.AddIngredient(null, "ChaosAmulet");
            recipe.AddIngredient(ItemID.UnholyWater, 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CharmofMyths);
            recipe.AddIngredient(ItemID.SorcererEmblem);
            recipe.AddIngredient(ItemID.CrystalShard, 20);
            recipe.AddIngredient(null, "CalamityDust", 5);
            recipe.AddIngredient(null, "CoreofChaos", 5);
            recipe.AddIngredient(ItemID.SpellTome);
            recipe.AddIngredient(null, "ChaosAmulet");
            recipe.AddIngredient(ItemID.BloodWater, 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}