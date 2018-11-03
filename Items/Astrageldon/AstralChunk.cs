﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Astrageldon
{
	public class AstralChunk : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Astral Chunk");
		}
		
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.maxStack = 20;
			item.rare = 7;
			item.useAnimation = 45;
			item.useTime = 45;
			item.useStyle = 4;
			item.consumable = true;
		}
		
		public override bool CanUseItem(Player player)
		{
			return !Main.dayTime && !NPC.AnyNPCs(mod.NPCType("Astrageldon"));
		}
		
		public override bool UseItem(Player player)
		{
            if (Main.netMode != 1)
            {
                int num = NPC.NewNPC((int)(player.position.X + (float)(Main.rand.Next(-50, 50))), (int)(player.position.Y - 150f), mod.NPCType("Astrageldon"), 0, 0f, 0f, 0f, 0f, 255);
                Main.PlaySound(SoundID.Roar, player.position, 0);
            }
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "Stardust", 15);
			recipe.AddIngredient(ItemID.FallenStar, 10);
            recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}