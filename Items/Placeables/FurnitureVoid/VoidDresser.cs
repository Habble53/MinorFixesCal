using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureVoid
{
    public class VoidDresser : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 22;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.createTile = ModContent.TileType<Tiles.FurnitureVoid.VoidDresser>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SmoothVoidstone>(), 16);
            recipe.SetResult(this, 1);
            recipe.AddTile(ModContent.TileType<VoidCondenser>());
            recipe.AddRecipe();
        }
    }
}
