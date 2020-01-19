using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Projectiles.Rogue
{
    public class Valaricicle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Valaricicle");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.Calamity().rogue = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 180;
            projectile.aiStyle = 1;
			projectile.coldDamage = true;
        }

        public override void AI()
        {
            projectile.velocity.X *= 0.9995f;
            projectile.velocity.Y = projectile.velocity.Y + 0.01f;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 27);
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 67, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 120);
        }
    }
}
