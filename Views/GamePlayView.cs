using LunarLander;
using LunarLander.Terrain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace CS5410
{
    public class GamePlayView : GameStateView
    {
        
        private SpriteFont m_font;
        private Random m_random = new Random();
        private Ship m_ship; 
        private Texture2D m_texShip;
        private Texture2D m_texCircle;
        private SoundEffect thrustSound;
        private SoundEffect crashSound;
        private SoundEffect landedSound;
        private ContentManager m_contentManager;
        private Terrain m_terrain;

        public override void loadContent(ContentManager contentManager)
        {
            m_font = contentManager.Load<SpriteFont>("Fonts/menu");

            m_terrain = new Terrain(m_graphics, m_random);

            m_texShip = contentManager.Load<Texture2D>("Images/rocket");
            m_texCircle = contentManager.Load<Texture2D>("Images/circle");

            thrustSound = contentManager.Load<SoundEffect>("Sounds/thrust");
            crashSound = contentManager.Load<SoundEffect>("Sounds/crash");
            landedSound = contentManager.Load<SoundEffect>("Sounds/landed");

            m_contentManager = contentManager;

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
            int screenWidth = m_graphics.PreferredBackBufferWidth;
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
            m_spriteBatch.DrawString(m_font, fuelStr, line1Position, fuelColor, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
            m_spriteBatch.DrawString(m_font, speedStr, line2Position, speedColor, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
            m_spriteBatch.DrawString(m_font, angleStr, line3Position, angleColor, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);

            m_spriteBatch.End();
        }

        public override void update(GameTime gameTime)
        {
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
                            // win
                            m_ship.hasWon();
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
            float shipScaleFactor = 0.1f; 
            int screenHeight = m_graphics.GraphicsDevice.Viewport.Height;
            float shipHeight = m_texShip.Height;
            float scale = (screenHeight * shipScaleFactor) / shipHeight;
            m_ship = new Ship(new Vector2(50, 50), new Vector2(0, 0), new Vector2(1, 0), scale, thrustSound, crashSound, landedSound, m_contentManager);
            m_terrain.GenerateTerrain(scale, true);
        }

    }
}
