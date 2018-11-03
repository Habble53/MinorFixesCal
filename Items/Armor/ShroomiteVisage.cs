﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Armor {
[AutoloadEquip(EquipType.Head)]
public class ShroomiteVisage : ModItem
{
    public override void SetStaticDefaults()
    {
        DisplayName.SetDefault("Shroomite Visage");
        Tooltip.SetDefault("25% increased ranged damage for flamethrowers");
    }

    public override void SetDefaults()
    {
        item.width = 18;
        item.height = 18;
        item.value = 375000;
        item.rare = 8;
        item.defense = 11; //62
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ItemID.ShroomiteBreastplate && legs.type == ItemID.ShroomiteLeggings;
    }

    public override void UpdateArmorSet(Player player)
    {
    	CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
    	player.shroomiteStealth = true;
        player.setBonus = "Ranged stealth while standing still";
    }
    
    public override void UpdateEquip(Player player)
    {
    	CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
    	modPlayer.flamethrowerBoost = true;
    }

    public override void AddRecipes()
    {
        ModRecipe recipe = new ModRecipe(mod);
        recipe.AddIngredient(ItemID.ShroomiteBar, 12);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.SetResult(this);
        recipe.AddRecipe();
    }
}}