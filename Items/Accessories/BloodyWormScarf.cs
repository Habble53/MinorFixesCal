﻿using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Neck)]
    public class BloodyWormScarf : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Bloody Worm Scarf");
            Tooltip.SetDefault("10% increased damage reduction\n" +
                "7% increased melee damage and speed");
        }

        public override void SetDefaults()
        {
            Item.defense = 7;
            Item.width = 26;
            Item.height = 42;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.bloodyWormTooth = true;
            player.endurance += 0.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BloodyWormTooth>().
                AddIngredient(ItemID.WormScarf).
                AddIngredient(ItemID.SoulofNight, 3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
