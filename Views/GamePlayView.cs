using LunarLander;
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

            string fuelStr  = "  FUEL:  " + String.Format("{0,10:F2}", fuelInt) + " s";
            string speedStr = "SPEED: " + String.Format("{0,10:F2}", m_ship.getMeterPerSec()) + " m/s";
            string angleStr = "ANGLE: " + String.Format("{0,10:F2}", m_ship.GetRotationInDegrees()) + " degrees";
            // Assume m_font is your loaded SpriteFont
            Vector2 fuelStrSize = m_font.MeasureString(fuelStr);
            int lineHeight = 20;
            // Calculate positions
            Vector2 line1Position = new Vector2(screenWidth - fuelStrSize.X, 10); // 10 pixels from the top and right
            Vector2 line2Position = new Vector2(screenWidth - fuelStrSize.X, 10 + lineHeight); // Below line 1
            Vector2 line3Position = new Vector2(screenWidth - fuelStrSize.X, 10 + lineHeight * 2); // Below line 2
            //draw
            Vector2 textScale = new Vector2(0.5f, 0.5f);
            Color fuelColor = fuelInt > 0 ? Color.Green : Color.White;
            Color speedColor = m_ship.getMeterPerSec() <= 2.0f ? Color.Green : Color.White;
            Color angleColor = (m_ship.GetRotationInDegrees() <= 5 || m_ship.GetRotationInDegrees() >= 355) ? Color.Green : Color.White;

            drawOutlineText(m_spriteBatch, m_font, fuelStr, Color.Black, fuelColor, line1Position, 0.45f);
            drawOutlineText(m_spriteBatch, m_font, speedStr, Color.Black, speedColor, line2Position, 0.45f);
            drawOutlineText(m_spriteBatch, m_font, angleStr, Color.Black, angleColor, line3Position, 0.45f);


            Vector2 screenSize = new Vector2(m_graphics.GraphicsDevice.Viewport.Width, m_graphics.GraphicsDevice.Viewport.Height);
            if (m_inTransition)
            {
                string transitionText = "Next Level in: " + m_countDown.ToString("F2");
                Vector2 textSize = m_font.MeasureString(transitionText);
                Vector2 position = (screenSize - textSize) / 2;
                drawOutlineText(m_spriteBatch, m_font, transitionText, Color.Black, Color.White, position, 1.0f);
            }
            if (levelTwoWon)
            {
                string transitionText = "Your score of " + score.ToString("F3") + " has been saved! Press ESC to return to Main Menu.";
                Vector2 textSize = m_font.MeasureString(transitionText);
                Vector2 position = (screenSize - textSize) / 2;
                drawOutlineText(m_spriteBatch, m_font, transitionText, Color.Black, Color.White, position, 1.0f);
            }
            if (m_ship.isDead)
            {
                string transitionText = "Better luck next time! Press ESC to return to Main Menu.";
                Vector2 textSize = m_font.MeasureString(transitionText);
                Vector2 position = (screenSize - textSize) / 2;
                drawOutlineText(m_spriteBatch, m_font, transitionText, Color.Black, Color.White, position, 1.0f);
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
            m_ship = new Ship(new Vector2(50, 50), new Vector2(0, 0), new Vector2(1, 0), scale, thrustSound, crashSound, landedSound, m_contentManager);
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
            //
            // Offset to the upper left and lower right - faster, but not as good
            //spriteBatch.DrawString(font, text, position - new Vector2(PIXEL_OFFSET * scale, PIXEL_OFFSET * scale), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);
            //spriteBatch.DrawString(font, text, position + new Vector2(PIXEL_OFFSET * scale, PIXEL_OFFSET * scale), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);

            //
            // Offset in each of left,right, up, down directions - slower, but good
            spriteBatch.DrawString(font, text, position - new Vector2(PIXEL_OFFSET * scale, 0), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, position + new Vector2(PIXEL_OFFSET * scale, 0), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, position - new Vector2(0, PIXEL_OFFSET * scale), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, position + new Vector2(0, PIXEL_OFFSET * scale), outlineColor, 0, Vector2.Zero, scale, SpriteEffects.None, 1f);

            //
            // This sits inside the text rendering done just above
            spriteBatch.DrawString(font, text, position, frontColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

    }
}
