﻿using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class DeathsAscension : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Death's Ascension");
            Tooltip.SetDefault("Right click to launch a barrage of homing scythes");
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 400;
            Item.knockBack = 9f;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.channel = true;
            Item.width = 70;
            Item.height = 70;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<DeathsAscensionSwing>();
            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
            Item.Calamity().donorItem = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.shoot = ModContent.ProjectileType<DeathsAscensionProjectile>();
            }
            else
            {
                Item.shoot = ModContent.ProjectileType<DeathsAscensionSwing>();
            }
            return base.UseItem(player);
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2f)
            {
                Item.shootSpeed = 18f;
                Item.useTime = Item.useAnimation = 16;
                Item.useStyle = ItemUseStyleID.Swing;
                Item.UseSound = SoundID.Item71;
                Item.useTurn = true;
                Item.autoReuse = true;
                Item.noMelee = false;
                Item.noUseGraphic = false;
                Item.channel = false;
            }
            else
            {
                Item.shootSpeed = 24f;
                Item.useTime = Item.useAnimation = 30;
                Item.useStyle = ItemUseStyleID.Shoot;
                Item.UseSound = null;
                Item.useTurn = false;
                Item.autoReuse = false;
                Item.noMelee = true;
                Item.noUseGraphic = true;
                Item.channel = true;
            }
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int spreadfactor = 9;
            if (player.altFunctionUse == 2f)
            {
                for (int index = 0; index < 5; ++index)
                {
                    float SpeedX = velocity.X + Main.rand.NextFloat(-spreadfactor, spreadfactor + 1);
                    float SpeedY = velocity.Y + Main.rand.NextFloat(-spreadfactor, spreadfactor + 1);
                    Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, damage, knockback, player.whoAmI, 0f, 0f);
                }
            }
            else
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<DeathsAscensionSwing>(), damage * 3, knockback, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.DeathSickle).
                AddIngredient<RuinousSoul>(4).
                AddIngredient<TwistingNether>(1).
                AddIngredient(ItemID.SoulofNight, 15).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
