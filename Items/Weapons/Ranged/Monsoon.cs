﻿using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Monsoon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Monsoon");
            Tooltip.SetDefault("Fires a spread of 5 arrows\n" +
                "Wooden arrows are converted into typhoon arrows and sharks");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 121;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 46;
            Item.height = 78;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.5f;
            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Arrow;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo spawnSource, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 source = player.RotatedRelativePoint(player.MountedCenter);
            float piOver10 = MathHelper.Pi * 0.1f;
            int totalProjectiles = 5;

            velocity.Normalize();
            velocity *= 40f;
            bool canHit = Collision.CanHit(source, 0, 0, source + velocity, 0, 0);
            for (int p = 0; p < totalProjectiles; p++)
            {
                float offsetAmt = p - (totalProjectiles - 1f) / 2f;
                Vector2 offset = velocity.RotatedBy(piOver10 * offsetAmt);
                if (!canHit)
                    offset -= velocity;

                if (type == ProjectileID.WoodenArrowFriendly)
                {
                    int newType = type;
                    switch (p)
                    {
                        case 0:
                        case 1:
                        case 3:
                        case 4:
                            newType = ModContent.ProjectileType<MiniSharkron>();
                            break;
                        case 2:
                            newType = ModContent.ProjectileType<TyphoonArrow>();
                            break;
                    }
                    int proj = Projectile.NewProjectile(spawnSource, source.X + offset.X, source.Y + offset.Y, velocity.X, velocity.Y, newType, (int)(damage * 1.1), knockback, player.whoAmI);
                    if (proj.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[proj].arrow = true;
                        Main.projectile[proj].extraUpdates += 1;
                    }
                }
                else
                {
                    int proj = Projectile.NewProjectile(spawnSource, source.X + offset.X, source.Y + offset.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
                    Main.projectile[proj].noDropItem = true;
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Tsunami).
                AddIngredient<FlarewingBow>().
                AddIngredient<ReaperTooth>(6).
                AddIngredient(ItemID.SharkFin, 2).
                AddIngredient<DepthCells>(10).
                AddIngredient<Lumenite>(10).
                AddIngredient<Tenebris>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
