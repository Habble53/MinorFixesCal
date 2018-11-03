using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items
{
    public class WulfrumPickaxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Pickaxe");
        }

        public override void SetDefaults()
        {
            item.damage = 8;
            item.melee = true;
            item.width = 48;
            item.height = 48;
            item.useTime = 7;
            item.useAnimation = 14;
            item.useTurn = true;
            item.pick = 35;
            item.useStyle = 1;
            item.knockBack = 2;
            item.value = 25000;
            item.rare = 1;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "WulfrumShard", 12);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}