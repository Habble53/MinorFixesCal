using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.BaseProjectiles
{
    public abstract class BaseLaserbeamProjectile : ModProjectile
    {
        #region Auto-Properties
        public float RotationalSpeed
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        public float Time
        {
            get => projectile.localAI[0];
            set => projectile.localAI[0] = value;
        }
        public float LaserLength
        {
            get => projectile.localAI[1];
            set => projectile.localAI[1] = value;
        }
        #endregion

        #region Virtual Methods

        /// <summary>
        /// Handles all AI logic for the laser. Can be overridden, but you probably won't need to do that.
        /// </summary>
        public virtual void Behavior()
        {
            // Attach to some arbitrary thing/position optionally. (The ai[1] value is a reserved for this in vanilla's Phantasmal Deathray)
            AttachToSomething();

            // Ensure the the velocity is a unit vector and is not a <0,0> vector.
            projectile.velocity = projectile.velocity.SafeNormalize(-Vector2.UnitY);

            Time++;
            if (Time >= Lifetime)
            {
                projectile.Kill();
                return;
            }

            DetermineScale();

            UpdateLaserMotion();

            float idealLaserLength = DetermineLaserLength();
            LaserLength = MathHelper.Lerp(LaserLength, idealLaserLength, 0.9f); // Very quickly approach the ideal laser length.

            DelegateMethods.v3_1 = LightCastColor.ToVector3();
            Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * LaserLength, projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));
        }

        /// <summary>
        /// Handles movement logic for the laser. By default causes arcing/sweeping motiom.
        /// </summary>
        public virtual void UpdateLaserMotion()
        {
            // This part is rather complicated at a glance.
            // What it's doing is converting the velocity to an angle, doing something with that angle, and changing it back into the velocity.
            // In this case, "doing something with that angle" means incrementing it by a constant. This allows the laser to perform "arcing" motion.
            // You could attempt to make it intelligent by having it move towards the target like the Last Prism, but that's not done here.

            float updatedVelocityDirection = projectile.velocity.ToRotation() + RotationalSpeed;
            projectile.rotation = updatedVelocityDirection - MathHelper.PiOver2; // Pretty much all lasers have a vertical sheet.
            projectile.velocity = updatedVelocityDirection.ToRotationVector2();
        }

        /// <summary>
        /// Calculates the total scale of the laser. By default uses a clamped sine of time.
        /// </summary>
        public virtual void DetermineScale()
        {
            projectile.scale = (float)Math.Sin(Time / Lifetime * MathHelper.Pi) * 6f * MaxScale;
            if (projectile.scale > MaxScale)
            {
                projectile.scale = MaxScale;
            }
        }
        
        /// <summary>
        /// Handles direct attachment to things. The projectile.ai[1] array index is reserved for this. Does nothing by default.
        /// </summary>
        public virtual void AttachToSomething() { }

        /// <summary>
        /// Calculates the current laser's length. By default does not collide with tiles. <see cref="DetermineLaserLength_CollideWithTiles"/> is a generic laser collision method if you want to do that.
        /// </summary>
        /// <returns>The laser length as a float.</returns>
        public virtual float DetermineLaserLength() => MaxLaserLength;

        /// <summary>
        /// An extra, empty by default method that exists so that a developer can add custom code after all typical AI logic is done. Think of it like PostAI.
        /// </summary>
        public virtual void ExtraBehavior() { }
        #endregion

        #region Helper Methods

        /// <summary>
        /// Calculates the laser length while taking tiles into account.
        /// </summary>
        /// <param name="samplePointCount">The amount of samples the ray uses. The higher this is, the more precision, but also more calculations done.</param>
        public float DetermineLaserLength_CollideWithTiles(int samplePointCount)
        {
            float[] laserLengthSamplePoints = new float[samplePointCount];
            Collision.LaserScan(projectile.Center, projectile.velocity, projectile.scale, MaxLaserLength, laserLengthSamplePoints);
            return laserLengthSamplePoints.Average();
        }
        #endregion

        #region Hook Overrides
        public override void AI()
        {
            Behavior();
            ExtraBehavior();
        }
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackMelee;
            Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * LaserLength, projectile.Size.Length() * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CutTiles));
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            // This should never happen, but just in case-
            if (projectile.velocity == Vector2.Zero)
                return false;

            // Start texture drawing.
            spriteBatch.Draw(LaserBeginTexture,
                             projectile.Center - Main.screenPosition,
                             null,
                             LaserOverlayColor,
                             projectile.rotation,
                             LaserBeginTexture.Size() / 2f,
                             projectile.scale,
                             SpriteEffects.None,
                             0f);

            // Prepare things for body drawing.
            float laserBodyLength = LaserLength;
            laserBodyLength -= (LaserBeginTexture.Height / 2 + LaserEndTexture.Height) * projectile.scale;
            Vector2 centerOnLaser = projectile.Center;
            centerOnLaser += projectile.velocity * projectile.scale * LaserBeginTexture.Height / 2f;

            // Body drawing.
            if (laserBodyLength > 0f)
            {
                float laserOffset = LaserMiddleTexture.Height * projectile.scale;
                float incrementalBodyLength = 0f;
                while (incrementalBodyLength + 1f < laserBodyLength)
                {
                    spriteBatch.Draw(LaserMiddleTexture,
                                     centerOnLaser - Main.screenPosition,
                                     null,
                                     LaserOverlayColor,
                                     projectile.rotation,
                                     LaserMiddleTexture.Width * 0.5f * Vector2.UnitX,
                                     projectile.scale,
                                     SpriteEffects.None,
                                     0f);
                    incrementalBodyLength += laserOffset;
                    centerOnLaser += projectile.velocity * laserOffset;
                }
            }

            // End texture drawing.
            if (Math.Abs(LaserLength - DetermineLaserLength()) < 30f)
            {
                Vector2 laserEndCenter = centerOnLaser - Main.screenPosition;
                spriteBatch.Draw(LaserEndTexture,
                                 laserEndCenter,
                                 null,
                                 LaserOverlayColor,
                                 projectile.rotation,
                                 LaserEndTexture.Frame(1, 1, 0, 0).Top(),
                                 projectile.scale,
                                 SpriteEffects.None,
                                 0f);
            }
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
                return true;
            float unused = 69420f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity * LaserLength, projectile.Size.Length() * projectile.scale, ref unused);
        }
        #endregion

        #region Virtual Properties
        public virtual float Lifetime => 120f;
        public virtual float MaxScale => 1f;
        public virtual float MaxLaserLength => 2400f; // Be careful with this. Going too high will cause lag.
        public virtual Color LightCastColor => Color.White;
        public virtual Color LaserOverlayColor => Color.White * 0.9f;
        public virtual Texture2D LaserBeginTexture { get; }
        public virtual Texture2D LaserMiddleTexture { get; }
        public virtual Texture2D LaserEndTexture { get; }
        #endregion
    }
}
