﻿using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items
{
    class SquishyBeanMount : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Suspicious Looking Jelly Bean");
            Tooltip.SetDefault("JELLY BEAN");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 1;
            item.value = 100000000;
            item.expert = true;
            item.UseSound = SoundID.Item3;
            item.noMelee = true;
            item.mountType = mod.MountType("SquishyBean");
        }
    }
}
