using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Materials
{
    public class StormlionMandible : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stormlion Mandible");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 6));
        }

        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 24;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 1, copper: 40);
            item.rare = ItemRarityID.Blue;
        }
    }
}
