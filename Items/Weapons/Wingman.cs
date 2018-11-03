using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons
{
    public class Wingman : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wingman");
        }

        public override void SetDefaults()
        {
            item.damage = 54;
            item.magic = true;
            item.mana = 12;
            item.width = 42;
            item.height = 22;
            item.useTime = 12;
            item.useAnimation = 12;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 3f;
            item.value = 500000;
            item.rare = 8;
            item.UseSound = SoundID.Item33;
            item.autoReuse = true;
            item.shootSpeed = 25f;
            item.shoot = 440;
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int num6 = 3;
            for (int index = 0; index < num6; ++index)
            {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, 440, (int)((double)damage), knockBack, player.whoAmI, 0.0f, 0.0f);
            }
            return false;
        }
    }
}