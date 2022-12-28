﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.SunkenSea
{
    public class EutrophicSand : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithDesert(Type);

            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.CanBeDugByShovel[Type] = true;

            DustType = 108;
            ItemDrop = ModContent.ItemType<Items.Placeables.EutrophicSand>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Eutrophic Sand");
            AddMapEntry(new Color(100, 100, 150), name);
            MineResist = 2f;
        }

        // You can't set this to false on world gen otherwise you can't slope the tiles :)
        /*public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            return DownedBossSystem.downedDesertScourge;
        }

        public override bool CanExplode(int i, int j)
        {
            return DownedBossSystem.downedDesertScourge;
        }*/

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CustomMergeFrame(i, j, Type, TileID.Sandstone);
            return false;
        }
    }
}
