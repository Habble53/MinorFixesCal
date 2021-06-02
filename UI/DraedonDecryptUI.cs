using CalamityMod.Items.DraedonMisc;
using CalamityMod.TileEntities;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace CalamityMod.UI
{
    public static class DraedonDecryptUI
    {
        public static int ViewedTileEntityID = -1;
        public static bool AwaitingDecryptionTextClose = false;
        public static float VerificationButtonScale = 1f;
        public static float ContactButtonScale = 1f;
        public static readonly Vector2 BackgroundCenter = new Vector2(500f, Main.screenHeight * 0.5f);

        public static Rectangle MouseScreenArea => Utils.CenteredRectangle(Main.MouseScreen, Vector2.One * 2f);
        public static void Draw(SpriteBatch spriteBatch)
        {
            // If not viewing the specific tile entity's interface anymore, or if the ID is for some reason invalid
            // don't do anything except resetting/update necessary information.
            if (!TileEntity.ByID.ContainsKey(ViewedTileEntityID) || !(TileEntity.ByID[ViewedTileEntityID] is TECodebreaker codebreakerTileEntity))
            {
                VerificationButtonScale = 1f;
                ContactButtonScale = 1f;
                ViewedTileEntityID = -1;
                return;
            }

            // If too far away from the tile entity, stop drawing.
            if (!Main.LocalPlayer.WithinRange(codebreakerTileEntity.Center, 270f))
            {
                VerificationButtonScale = 1f;
                ContactButtonScale = 1f;
                ViewedTileEntityID = -1;
                return;
            }

            // Close the UI if the inventory is no longer open.
            if (!Main.playerInventory)
            {
                VerificationButtonScale = 1f;
                ContactButtonScale = 1f;
                ViewedTileEntityID = -1;
                return;
			}

            // Draw the background.
            Texture2D backgroundTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/DraedonDecrypterBackground");
            spriteBatch.Draw(backgroundTexture, BackgroundCenter, null, Color.White, 0f, backgroundTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);

            Rectangle backgroundArea = Utils.CenteredRectangle(BackgroundCenter, backgroundTexture.Size());
            if (MouseScreenArea.Intersects(backgroundArea))
                Main.blockMouse = Main.LocalPlayer.mouseInterface = true;

            Vector2 backgroundTopLeft = BackgroundCenter - backgroundTexture.Size() * 0.5f;

            // Draw the cell payment slot icon.
            Texture2D emptyCellIconTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/PowerCellSlot_Empty");
            Texture2D occupiedCellIconTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/PowerCellSlot_Filled");
            Texture2D cellTexture = codebreakerTileEntity.InputtedCellCount > 0 ? occupiedCellIconTexture : emptyCellIconTexture;
            Vector2 cellDrawCenter = backgroundTopLeft + Vector2.One * 60f;

            Vector2 schematicSlotDrawCenter = cellDrawCenter + Vector2.UnitY * 70f;

            // Display some error text if the codebreaker isn't strong enough to decrypt the text.
            if (codebreakerTileEntity.HeldSchematicID != 0 && !codebreakerTileEntity.CanDecryptHeldSchematic)
            {
                Vector2 errorDisplayLocation = schematicSlotDrawCenter + Vector2.UnitY * 10f;
                DisplayNotStrongEnoughErrorText(errorDisplayLocation);
            }

            // Handle decryption costs.
            else if (codebreakerTileEntity.HeldSchematicID != 0 && codebreakerTileEntity.DecryptionCountdown == 0)
            {
                int cost = codebreakerTileEntity.DecryptionCellCost;
                Vector2 costDisplayLocation = schematicSlotDrawCenter + Vector2.UnitY * 20f;
                Vector2 costVerificationLocation = costDisplayLocation + Vector2.UnitY * 60f;
                DisplayCostText(costDisplayLocation, cost);

                if (codebreakerTileEntity.InputtedCellCount >= cost)
                    DrawCostVerificationButton(codebreakerTileEntity, costVerificationLocation, cost);
            }

            Vector2 summonButtonCenter = backgroundTopLeft + new Vector2(140f, backgroundTexture.Height - 55f);
            if (codebreakerTileEntity.ReadyToSummonDreadon)
                HandleDraedonSummonButton(codebreakerTileEntity, summonButtonCenter);

            if (codebreakerTileEntity.DecryptionCountdown > 0 || AwaitingDecryptionTextClose)
                HandleDecryptionStuff(codebreakerTileEntity, backgroundTexture, backgroundTopLeft, schematicSlotDrawCenter + Vector2.UnitY * 80f);

            // Draw the schematic icon.
            Texture2D emptySchematicIconTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/EncryptedSchematicSlot_Empty");
            Texture2D occupiedSchematicIconTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/EncryptedSchematicSlot_Full");

            Texture2D schematicTexture = codebreakerTileEntity.HeldSchematicID != 0 ? occupiedSchematicIconTexture : emptySchematicIconTexture;
            spriteBatch.Draw(schematicTexture, schematicSlotDrawCenter, null, Color.White, 0f, schematicTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
            HandleSchematicSlotInteractions(codebreakerTileEntity, schematicSlotDrawCenter, cellTexture.Size());

            // Create a temporary item for drawing purposes.
            // If cells are present, make the item reflect that.
            Item temporaryPowerCell = new Item();
            temporaryPowerCell.SetDefaults(ModContent.ItemType<PowerCell>());
            temporaryPowerCell.stack = codebreakerTileEntity.InputtedCellCount;

            CalamityUtils.DrawPowercellSlot(spriteBatch, temporaryPowerCell, cellDrawCenter, 1f);
            HandleCellSlotInteractions(codebreakerTileEntity, temporaryPowerCell, cellDrawCenter, cellTexture.Size());
        }

        public static void HandleCellSlotInteractions(TECodebreaker codebreakerTileEntity, Item temporaryItem, Vector2 cellIconCenter, Vector2 area)
        {
            Rectangle clickArea = Utils.CenteredRectangle(cellIconCenter, area);

            // If the mouse is not in the specific area don't do anything else.
            if (!MouseScreenArea.Intersects(clickArea))
                return;

            ref Item playerHandItem = ref Main.mouseItem;

            if (!temporaryItem.IsAir)
                Main.HoverItem = temporaryItem;

            if (Main.mouseLeft && Main.mouseLeftRelease && codebreakerTileEntity.DecryptionCountdown <= 0)
            {
                int powerCellID = ModContent.ItemType<PowerCell>();
                short cellStackDiff = 0;
                bool shouldPlaySound = true;

                // If the player is holding shift and has space for the power cells, just spawn all of them on his or her face, up to the max-stack limit.
                if (Main.keyState.PressingShift() && Main.LocalPlayer.ItemSpace(temporaryItem))
                {
                    cellStackDiff = (short)-Math.Min(temporaryItem.stack, temporaryItem.maxStack);
                    DropHelper.DropItem(Main.LocalPlayer, powerCellID, -cellStackDiff);

                    // Do not play a sound in this situation. The player is going to pick up the dropped cells in a few frames, which will make sound.
                    shouldPlaySound = false;
                }

                // If the slot is normally clicked, behavior depends on whether the player is holding power cells.
                else
                {
                    bool holdingPowercell = playerHandItem.type == powerCellID;

                    // If the player's held power cells can be stacked on top of what's already in the codeberaker, then stack them.
                    if (holdingPowercell && temporaryItem.stack < TECodebreaker.MaxCellCapacity)
                    {
                        int spaceLeft = TECodebreaker.MaxCellCapacity - temporaryItem.stack;

                        // If the player has more cells than there is space left, insert as many as possible. Otherwise insert all the cells.
                        int cellsToInsert = Math.Min(playerHandItem.stack, spaceLeft);
                        cellStackDiff = (short)cellsToInsert;
                        playerHandItem.stack -= cellsToInsert;
                        if (playerHandItem.stack == 0)
                            playerHandItem.TurnToAir();
                        AwaitingDecryptionTextClose = false;
                    }
                    // If the player is holding nothing, then pick up all the power cells (if any exist), up to the max-stack limit.
                    else if (playerHandItem.IsAir && temporaryItem.stack > 0)
                    {
                        cellStackDiff = (short)-temporaryItem.stack;
                        if (cellStackDiff < -temporaryItem.maxStack)
                            cellStackDiff = (short)-temporaryItem.maxStack;

                        playerHandItem.SetDefaults(temporaryItem.type);
                        playerHandItem.stack = -cellStackDiff;
                        temporaryItem.TurnToAir();
                        AwaitingDecryptionTextClose = false;
                    }
                }

                if (cellStackDiff != 0)
                {
                    if (shouldPlaySound)
                        Main.PlaySound(SoundID.Grab);
                    AwaitingDecryptionTextClose = false;
                    codebreakerTileEntity.InputtedCellCount += cellStackDiff;
                    codebreakerTileEntity.SyncContainedStuff();
                }
            }

            // Force the hover item to be drawn.
            // Since HoverItem is active, we don't need to input anything into this method.
            if (temporaryItem.stack > 0)
                Main.instance.MouseTextHackZoom(string.Empty);
        }

        public static void HandleSchematicSlotInteractions(TECodebreaker codebreakerTileEntity, Vector2 schematicIconCenter, Vector2 area)
        {
            Rectangle clickArea = Utils.CenteredRectangle(schematicIconCenter, area);

            // If the mouse is not in the specific area don't do anything else.
            if (!MouseScreenArea.Intersects(clickArea))
                return;

            ref Item playerHandItem = ref Main.mouseItem;

            if (Main.mouseLeft && Main.mouseLeftRelease)
            {
                if (playerHandItem.IsAir && CalamityLists.EncryptedSchematicIDRelationship.ContainsKey(codebreakerTileEntity.HeldSchematicID) && codebreakerTileEntity.DecryptionCountdown <= 0)
                {
                    playerHandItem.SetDefaults(CalamityLists.EncryptedSchematicIDRelationship[codebreakerTileEntity.HeldSchematicID]);
                    codebreakerTileEntity.HeldSchematicID = 0;
                    codebreakerTileEntity.DecryptionCountdown = 0;
                    codebreakerTileEntity.SyncContainedStuff();
                    Main.PlaySound(SoundID.Grab);

                    AwaitingDecryptionTextClose = false;
                }
                else if (CalamityLists.EncryptedSchematicIDRelationship.ContainsValue(playerHandItem.type))
                {
                    codebreakerTileEntity.HeldSchematicID = CalamityLists.EncryptedSchematicIDRelationship.First(i => i.Value == Main.mouseItem.type).Key;
                    playerHandItem.TurnToAir();
                    codebreakerTileEntity.SyncContainedStuff();
                    Main.PlaySound(SoundID.Grab);

                    AwaitingDecryptionTextClose = false;
                }
            }
        }

        public static void DisplayCostText(Vector2 drawPosition, int totalCellsCost)
        {
            string text = "Cost: ";
            drawPosition.X -= 30f;
            Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, text, drawPosition.X, drawPosition.Y + 20f, Color.White * (Main.mouseTextColor / 255f), Color.Black, Vector2.Zero, 1f);

            Texture2D cellTexture = ModContent.GetTexture("CalamityMod/Items/DraedonMisc/PowerCell");
            Vector2 offsetDrawPosition = new Vector2(drawPosition.X + ChatManager.GetStringSize(Main.fontMouseText, text, Vector2.One, -1f).X + 15f, drawPosition.Y + 30f);
            Main.spriteBatch.Draw(cellTexture, offsetDrawPosition, null, Color.White, 0f, cellTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
            Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontItemStack, totalCellsCost.ToString(), offsetDrawPosition.X - 11f, offsetDrawPosition.Y, Color.White, Color.Black, new Vector2(0.3f), 0.75f);
        }

        public static void DrawCostVerificationButton(TECodebreaker codebreakerTileEntity, Vector2 drawPosition, int totalCellsCost)
        {
            Texture2D confirmationTexture = Main.cameraTexture[5];
            Rectangle clickArea = Utils.CenteredRectangle(drawPosition, confirmationTexture.Size() * VerificationButtonScale);
            if (MouseScreenArea.Intersects(clickArea))
            {
                VerificationButtonScale = MathHelper.Clamp(VerificationButtonScale + 0.035f, 1f, 1.35f);
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    Main.PlaySound(SoundID.Zombie, Main.LocalPlayer.Center, 67);
                    AwaitingDecryptionTextClose = true;
                    codebreakerTileEntity.InitialCellCountBeforeDecrypting = codebreakerTileEntity.InputtedCellCount;
                    codebreakerTileEntity.DecryptionCountdown = codebreakerTileEntity.DecryptionTotalTime;
                    codebreakerTileEntity.SyncContainedStuff();
                    codebreakerTileEntity.SyncDecryptCountdown();
                }
            }
            else
                VerificationButtonScale = MathHelper.Clamp(VerificationButtonScale - 0.05f, 1f, 1.35f);

            Main.spriteBatch.Draw(confirmationTexture, drawPosition, null, Color.White, 0f, confirmationTexture.Size() * 0.5f, VerificationButtonScale, SpriteEffects.None, 0f);
        }

        public static void HandleDecryptionStuff(TECodebreaker codebreakerTileEntity, Texture2D backgroundTexture, Vector2 backgroundTopLeft, Vector2 barCenter)
		{
            Texture2D textPanelTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/DraedonDecrypterScreen");
            Vector2 textPanelCenter = backgroundTopLeft + Vector2.UnitX * backgroundTexture.Width + textPanelTexture.Size() * new Vector2(-0.5f, 0.5f);
            Main.spriteBatch.Draw(textPanelTexture, textPanelCenter, null, Color.White, 0f, textPanelTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);

            // Generate gibberish and use slowly insert the real text.
            // When decryption is done the gibberish will go away and only the underlying text will remain.
            int textPadding = 6;
            string trueMessage = codebreakerTileEntity.UnderlyingSchematicText;
            StringBuilder text = new StringBuilder(codebreakerTileEntity.DecryptionCountdown == 0 ? trueMessage : CalamityUtils.GenerateRandomAlphanumericString(500));

            // Don't mess with whitespace characters. Doing so can cause the word-wrap to "jump" around.
            for (int i = 0; i < trueMessage.Length; i++)
            {
                if (char.IsWhiteSpace(trueMessage[i]))
                    text[i] = trueMessage[i];
            }

            // Insert the necessary amount of true text.
            for (int i = 0; i < (int)(trueMessage.Length * codebreakerTileEntity.DecryptionCompletion); i++)
                text[i] = trueMessage[i];

            Vector2 currentTextDrawPosition = backgroundTopLeft + Vector2.UnitX * backgroundTexture.Width - Vector2.UnitX * (textPanelTexture.Width - textPadding);
            currentTextDrawPosition.Y += 6f;
            foreach (string line in Utils.WordwrapString(text.ToString(), Main.fontMouseText, (int)(textPanelTexture.Width * 1.5 - textPadding * 2), 10, out _))
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, line, currentTextDrawPosition, Color.Cyan, 0f, Vector2.Zero, new Vector2(0.6f, 0.6f));
                currentTextDrawPosition.Y += 16;
            }

            // Draw a small bar at the bottom to indicate how much work is left.
            Texture2D edgeTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/ChargeMeterBorder");
            Texture2D barTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/ChargeMeter");
            Main.spriteBatch.Draw(edgeTexture, barCenter, null, Color.White, 0f, edgeTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);

            Rectangle barRectangle = new Rectangle(0, 0, (int)(barTexture.Width * codebreakerTileEntity.DecryptionCompletion), barTexture.Width);
            Main.spriteBatch.Draw(barTexture, barCenter, barRectangle, Color.White, 0f, edgeTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);

            // Display a completion percentage below the bar.

            string percentString = (codebreakerTileEntity.DecryptionCompletion * 100f).ToString("n2");
            string completionText = $"{percentString}%";
            Vector2 textDrawPosition = barCenter + new Vector2(-Main.fontMouseText.MeasureString(completionText).X * 0.5f, 10f);
            Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, completionText, textDrawPosition.X, textDrawPosition.Y, Color.Red * (Main.mouseTextColor / 255f), Color.Black, Vector2.Zero, 1f);
        }

        public static void HandleDraedonSummonButton(TECodebreaker codebreakerTileEntity, Vector2 drawPosition)
		{
            Texture2D contactButton = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/ContactButton");
            Rectangle clickArea = Utils.CenteredRectangle(drawPosition, contactButton.Size() * VerificationButtonScale);
            if (MouseScreenArea.Intersects(clickArea))
            {
                ContactButtonScale = MathHelper.Clamp(ContactButtonScale + 0.035f, 1f, 1.35f);
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    Main.PlaySound(SoundID.Zombie, Main.LocalPlayer.Center, 67);
                    CalamityWorld.DraedonSummonCountdown = CalamityWorld.DraedonSummonCountdownMax;
                    CalamityWorld.DraedonSummonPosition = codebreakerTileEntity.Center + new Vector2(-8f, -100f);
                    CalamityNetcode.SyncWorld();
                }
            }
            else
                ContactButtonScale = MathHelper.Clamp(ContactButtonScale - 0.05f, 1f, 1.35f);

            Main.spriteBatch.Draw(contactButton, drawPosition, null, Color.White, 0f, contactButton.Size() * 0.5f, ContactButtonScale, SpriteEffects.None, 0f);

            string contactText = "Contact";
            drawPosition.X -= Main.fontMouseText.MeasureString(contactText).X * 0.5f;
            Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, contactText, drawPosition.X, drawPosition.Y + 20f, Color.Red * (Main.mouseTextColor / 255f), Color.Black, Vector2.Zero, 1f);
        }

        public static void DisplayNotStrongEnoughErrorText(Vector2 drawPosition)
        {
            string text = "Encryption unsolveable: Upgrades required.";
            drawPosition.X -= 30f;
            Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, text, drawPosition.X, drawPosition.Y + 20f, Color.IndianRed * (Main.mouseTextColor / 255f), Color.Black, Vector2.Zero, 1f);
        }
    }
}
