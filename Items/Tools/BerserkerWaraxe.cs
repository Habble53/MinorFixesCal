﻿using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Tools
{
    public class BerserkerWaraxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Berserker Waraxe");
        }

        public override void SetDefaults()
        {
            Item.damage = 51;
            Item.knockBack = 8f;
            Item.useTime = 5;
            Item.useAnimation = 13;
            Item.axe = 180 / 5;

            Item.DamageType = DamageClass.Melee;
            Item.width = 66;
            Item.height = 56;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.buyPrice(0, 60, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DraedonBar>(9).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 61);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Venom, 300);
        }
    }
}
