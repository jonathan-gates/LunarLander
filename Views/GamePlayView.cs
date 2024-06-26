﻿using LunarLander;
using LunarLander.Terrain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace CS5410
{
    public class GamePlayView : GameStateView
    {
        
        private SpriteFont m_font;
        private Random m_random = new Random();
        private Ship m_ship; 
        private Texture2D m_texShip;
        private Texture2D m_texCircle;
        private Texture2D m_texBackground;
        private SoundEffect thrustSound;
        private SoundEffect crashSound;
        private SoundEffect landedSound;
        private ContentManager m_contentManager;
        private Terrain m_terrain;
        public uint m_level = 1;
        public double m_countDown = 3;
        private bool m_inTransition;
        private float score = 0;
        private bool levelTwoWon;
        private PersistenceManager m_persistenceManager;

        public override void loadContent(ContentManager contentManager)
        {
            m_font = contentManager.Load<SpriteFont>("Fonts/menu");

            m_terrain = new Terrain(m_graphics, m_random);

            m_texShip = contentManager.Load<Texture2D>("Images/rocket");
            m_texCircle = contentManager.Load<Texture2D>("Images/circle");
            m_texBackground = contentManager.Load<Texture2D>("Images/background");

            thrustSound = contentManager.Load<SoundEffect>("Sounds/thrust");
            crashSound = contentManager.Load<SoundEffect>("Sounds/crash");
            landedSound = contentManager.Load<SoundEffect>("Sounds/landed");

            m_contentManager = contentManager;
            m_persistenceManager = new PersistenceManager();
            loadHighScores();
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                return GameStateEnum.MainMenu;
            }

            return GameStateEnum.GamePlay;
        }

        public override void render(GameTime gameTime)
        {
            int screenWidth = m_graphics.GraphicsDevice.Viewport.Width;
            int screenHeight = m_graphics.GraphicsDevice.Viewport.Height;

            m_spriteBatch.Begin();
            m_spriteBatch.Draw(m_texBackground, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
            m_spriteBatch.End();
            
            m_terrain.Draw();

            m_spriteBatch.Begin();

            float scale = m_ship.collisionRadius / (m_texCircle.Width / 2);

            m_ship.render(m_spriteBatch, m_texShip, m_texCircle, scale);

            float fuelInt;

            if (m_ship.fuel <= 0)
            {
                fuelInt = 0;
            }
            else
            { 
                fuelInt= m_ship.fuel;
            }

            


            string fuelStr  = "  FUEL:  " + String.Format("{0,5:F2}", fuelInt) + " s";
            string speedStr = "      SPEED: " + String.Format("{0,5:F2}", m_ship.getMeterPerSec()) + " m/s";
            string angleStr = "       ANGLE: " + String.Format("{0,5:F2}", m_ship.GetRotationInDegrees()) + " deg";
            // Assume m_font is your loaded SpriteFont
            Vector2 fuelStrSize = m_font.MeasureString(fuelStr);
            float lineHeight = 22 * m_ship.scale;
            int yOffset = (int)(m_graphics.GraphicsDevice.Viewport.Height * 0.075);
            int xOffset = (int)(m_graphics.GraphicsDevice.Viewport.Width * 0.05);
            // Calculate positions
            Vector2 line1Position = new Vector2(screenWidth - fuelStrSize.X / 2 - xOffset, 14 * m_ship.scale + yOffset); // 10 pixels from the top and right
            Vector2 line2Position = new Vector2(screenWidth - fuelStrSize.X / 2 - xOffset, 14 + lineHeight + yOffset); // Below line 1
            Vector2 line3Position = new Vector2(screenWidth - fuelStrSize.X / 2 - xOffset, 14 + lineHeight * 2 + yOffset); // Below line 2
            //draw
            Color fuelColor = fuelInt > 0 ? Color.Green : Color.White;
            Color speedColor = m_ship.getMeterPerSec() <= 2.0f ? Color.Green : Color.White;
            Color angleColor = (m_ship.GetRotationInDegrees() <= 5 || m_ship.GetRotationInDegrees() >= 355) ? Color.Green : Color.White;


            // Backdrop
            //float maxWidth = m_graphics.GraphicsDevice.Viewport.Width * 0.25f;
            float maxWidth = Math.Max(fuelStrSize.X * 1.20f, m_graphics.GraphicsDevice.Viewport.Width * 0.25f);
            float totalHeight = m_graphics.GraphicsDevice.Viewport.Height * 0.075f; 
            Vector2 backdropPosition = new Vector2(line1Position.X - fuelStrSize.X / 2 + xOffset, yOffset); // Size of the backdrop
            Vector2 backdropSize = new Vector2(maxWidth - (m_graphics.GraphicsDevice.Viewport.Width / 11 * m_ship.scale), totalHeight);
            Texture2D pixel = new Texture2D(m_graphics.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White }); // A white pixel
            Color backdropColor = new Color(0, 0, 0, 200);
            m_spriteBatch.Draw(pixel, new Rectangle((int)backdropPosition.X - (int)(m_graphics.GraphicsDevice.Viewport.Width /38.4), (int)backdropPosition.Y, (int)backdropSize.X, (int)backdropSize.Y), backdropColor);


            drawOutlineText(m_spriteBatch, m_font, fuelStr, Color.Black, fuelColor, line1Position, m_ship.scale * 0.45f);
            drawOutlineText(m_spriteBatch, m_font, speedStr, Color.Black, speedColor, line2Position, m_ship.scale * 0.45f);
            drawOutlineText(m_spriteBatch, m_font, angleStr, Color.Black, angleColor, line3Position, m_ship.scale * 0.45f);


            Vector2 screenSize = new Vector2(m_graphics.GraphicsDevice.Viewport.Width, m_graphics.GraphicsDevice.Viewport.Height);
            if (m_inTransition)
            {
                string transitionText = "Next Level in: " + m_countDown.ToString("F2");
                Vector2 position = screenSize / 2;

                // scale to fit width
                float baseScreenWidth = 1920; // Assume 1920 is the base width you designed for
                float scalingFactor = m_graphics.GraphicsDevice.Viewport.Width / baseScreenWidth;
                float textScale = Math.Min(scalingFactor, m_ship.scale);
                Vector2 textSize = m_font.MeasureString(transitionText) * textScale;
                if (textSize.X > m_graphics.GraphicsDevice.Viewport.Width)
                {
                    // The text is too wide to fit on the screen, reduce the scale further
                    textScale *= m_graphics.GraphicsDevice.Viewport.Width / textSize.X;
                }

                drawOutlineText(m_spriteBatch, m_font, transitionText, Color.Black, Color.White, position, textScale);
            }
            if (levelTwoWon)
            {
                string transitionText = "Your score of " + score.ToString("F3") + " has been saved! Press ESC to return to Main Menu.";
                Vector2 position = screenSize / 2;

                // scale to fit width
                float baseScreenWidth = 1920; // Assume 1920 is the base width you designed for
                float scalingFactor = m_graphics.GraphicsDevice.Viewport.Width / baseScreenWidth;
                float textScale = Math.Min(scalingFactor, m_ship.scale);

                Vector2 textSize = m_font.MeasureString(transitionText) * textScale;
                if (textSize.X > m_graphics.GraphicsDevice.Viewport.Width)
                {
                    // The text is too wide to fit on the screen, reduce the scale further
                    textScale *= m_graphics.GraphicsDevice.Viewport.Width / textSize.X;
                }

                drawOutlineText(m_spriteBatch, m_font, transitionText, Color.Black, Color.White, position, textScale);
            }
            if (m_ship.isDead)
            {
                string transitionText = "Better luck next time! Press ESC to return to Main Menu.";
                Vector2 position = screenSize / 2;

                // scale to fit width
                float baseScreenWidth = 1920; // Assume 1920 is the base width you designed for
                float scalingFactor = m_graphics.GraphicsDevice.Viewport.Width / baseScreenWidth;
                float textScale = Math.Min(scalingFactor, m_ship.scale);
                Vector2 textSize = m_font.MeasureString(transitionText) * textScale;
                if (textSize.X > m_graphics.GraphicsDevice.Viewport.Width)
                {
                    // The text is too wide to fit on the screen, reduce the scale further
                    textScale *= m_graphics.GraphicsDevice.Viewport.Width / textSize.X;
                }

                drawOutlineText(m_spriteBatch, m_font, transitionText, Color.Black, Color.White, position, textScale);
            }

            m_spriteBatch.End();
        }

        public override void update(GameTime gameTime)
        {
            if (m_inTransition && m_countDown > 0)
            {
                m_countDown -= gameTime.ElapsedGameTime.TotalSeconds;
            }
            m_ship.Update(gameTime);
            // check colision
            foreach (var line in m_terrain.getLines())
            {
                if (m_ship.LineIntersectsCircle(line.startingPosition, line.endingPosition))
                {
                    if (line.is_safe_zone)
                    {
                        if (m_ship.checkLandedSafely())
                        {
                            m_ship.hasWon();
                            if (m_level == 1)
                            {
                                m_inTransition = true;
                                if (m_countDown <= 0)
                                {
                                    m_inTransition = false;
                                    score += m_ship.fuel;
                                    resetLevelTwo();
                                }

                            }
                            else 
                            {
                                if (!levelTwoWon)
                                { 
                                    levelTwoWon = true;
                                    score += m_ship.fuel;
                                    if (m_persistenceManager.m_scoresPersistence == null)
                                    {
                                        // create first high score
                                        m_persistenceManager.saveScore(new List<float>(), score);
                                    }
                                    else 
                                    {
                                        // add score
                                        m_persistenceManager.saveScore(m_persistenceManager.m_scoresPersistence.Scores, score);
                                    }
                                }
                            }
                        }
                        else 
                        {
                            m_ship.hasDied();

                        }
                    }
                    else 
                    {
                        m_ship.hasDied();
                    }
                }
            }
        }

        public void resetGameState()
        {
            float scale = getScaleAndCreateShip();
            m_level = 1;
            m_countDown = 3;
            m_terrain.GenerateTerrain(scale, true);
            levelTwoWon = false;
            score = 0;
        }

        private void resetLevelTwo()
        {
            float scale = getScaleAndCreateShip();
            m_terrain.GenerateTerrain(scale, false);
            m_level = 2;
        }

        private float getScaleAndCreateShip()
        {
            float shipScaleFactor = 0.1f;
            int screenHeight = m_graphics.GraphicsDevice.Viewport.Height;
            float shipHeight = m_texShip.Height;
            float scale = (screenHeight * shipScaleFactor) / shipHeight;
            m_ship = new Ship(new Vector2((int)(50 * scale + m_graphics.GraphicsDevice.Viewport.Width * 0.075), (int)(50 * scale)), new Vector2(0, 0), new Vector2(1, 0), scale, thrustSound, crashSound, landedSound, m_contentManager);
            return scale;
        }

        public void loadHighScores()
        {
            if (m_persistenceManager != null)
            {
                m_persistenceManager.loadHighScores();
            }
        }

        protected static void drawOutlineText(SpriteBatch spriteBatch, SpriteFont font, string text, Color outlineColor, Color frontColor, Vector2 position, float scale)
        {
            const float PIXEL_OFFSET = 1.0f;
            Vector2 origin = font.MeasureString(text) / 2;
            //
            // Offset to the upper left and lower right - faster, but not as good
            //spriteBatch.DrawString(font, text, position - new Vector2(PIXEL_OFFSET * scale, PIXEL_OFFSET * scale), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);
            //spriteBatch.DrawString(font, text, position + new Vector2(PIXEL_OFFSET * scale, PIXEL_OFFSET * scale), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);

            //
            // Offset in each of left,right, up, down directions - slower, but good
            spriteBatch.DrawString(font, text, position - new Vector2(PIXEL_OFFSET * scale / 2, 0), outlineColor, 0, origin, scale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, position + new Vector2(PIXEL_OFFSET * scale / 2, 0), outlineColor, 0, origin, scale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, position - new Vector2(0, PIXEL_OFFSET * scale / 2), outlineColor, 0, origin, scale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, position + new Vector2(0, PIXEL_OFFSET * scale / 2), outlineColor, 0, origin, scale, SpriteEffects.None, 1f);

            //
            // This sits inside the text rendering done just above
            spriteBatch.DrawString(font, text, position, frontColor, 0, origin, scale, SpriteEffects.None, 0f);
        }

    }
}
