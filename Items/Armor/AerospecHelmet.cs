﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Armor;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class AerospecHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aerospec Helmet");
            Tooltip.SetDefault("8% increased summon damage");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 50000;
            item.rare = 3;
            item.defense = 2; //13
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("AerospecBreastplate") && legs.type == mod.ItemType("AerospecLeggings");
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "5% increased movement speed\n" +
                "Summons a valkyrie to protect you\n" +
                "Taking over 25 damage in one hit will cause a spread of homing feathers to fall\n" +
                "Allows you to fall more quickly and disables fall damage";
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.valkyrie = true;
            modPlayer.aeroSet = true;
            player.noFallDmg = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(mod.BuffType("Valkyrie")) == -1)
                {
                    player.AddBuff(mod.BuffType("Valkyrie"), 3600, true);
                }
                if (player.ownedProjectileCounts[mod.ProjectileType("Valkyrie")] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, mod.ProjectileType("Valkyrie"), 25, 0f, Main.myPlayer, 0f, 0f);
                }
            }
            player.moveSpeed += 0.05f;
        }

        public override void UpdateEquip(Player player)
        {
            player.minionDamage += 0.08f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "AerialiteBar", 5);
            recipe.AddIngredient(ItemID.Cloud, 3);
            recipe.AddIngredient(ItemID.RainCloud);
            recipe.AddIngredient(ItemID.Feather);
            recipe.AddTile(TileID.SkyMill);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}