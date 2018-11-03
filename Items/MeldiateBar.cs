﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items
{
    public class MeldiateBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Meld Bar");
        }

        public override void SetDefaults()
        {
            item.width = 15;
            item.height = 12;
            item.maxStack = 999;
            item.value = 75000;
            item.rare = 9;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Ectoplasm);
            recipe.AddIngredient(ItemID.HallowedBar);
            recipe.AddIngredient(null, "MeldBlob", 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this, 5);
            recipe.AddRecipe();
        }
    }
}