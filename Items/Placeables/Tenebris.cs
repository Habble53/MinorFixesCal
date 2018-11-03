﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Placeables
{
    public class Tenebris : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tenebris");
        }

        public override void SetDefaults()
        {
            item.createTile = mod.TileType("Tenebris");
            item.useStyle = 1;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 13;
            item.height = 10;
            item.maxStack = 999;
            item.value = 2750;
            item.rare = 6;
        }
    }
}