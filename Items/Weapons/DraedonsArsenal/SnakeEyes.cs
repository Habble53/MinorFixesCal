using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
	public class SnakeEyes : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Snake Eyes");
			Tooltip.SetDefault("Summons a mechanical watcher that zaps and flies around enemies.");
		}

		public override void SetDefaults()
		{
			item.shootSpeed = 10f;
			item.damage = 211;
			item.mana = 12;
			item.width = 38;
			item.height = 24;
			item.useTime = item.useAnimation = 30;
			item.useStyle = ItemUseStyleID.HoldingUp;
			item.noMelee = true;
			item.knockBack = 3f;

			item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
			item.rare = ItemRarityID.Red;
			item.Calamity().customRarity = CalamityRarity.DraedonRust;

			item.UseSound = SoundID.Item15;
			item.autoReuse = true;
			item.shoot = ModContent.ProjectileType<SnakeEyesSummon>();
			item.shootSpeed = 10f;
			item.summon = true;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI);
			return false;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 15);
			recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 15);
			recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 8);
			recipe.AddIngredient(ItemID.LunarBar, 4);
			recipe.AddIngredient(ModContent.ItemType<GodspawnHelixStaff>());
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
