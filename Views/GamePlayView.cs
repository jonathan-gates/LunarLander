using LunarLander;
using LunarLander.Terrain;
using Microsoft.Xna.Framework;
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
        private Terrain m_terrain;
        private string state = "";
        

        public override void loadContent(ContentManager contentManager)
        {
            m_font = contentManager.Load<SpriteFont>("Fonts/menu");

            m_terrain = new Terrain(m_graphics, m_random);

            m_terrain.GenerateTerrain();

            m_texShip = contentManager.Load<Texture2D>("Images/rocket");
            m_texCircle = contentManager.Load<Texture2D>("Images/circle");

            float shipScaleFactor = 0.1f; // Example: Ship size = 10% of screen height
            int screenHeight = m_graphics.GraphicsDevice.Viewport.Height; // Assuming you have access to GraphicsDevice or similar
            float shipHeight = m_texShip.Height; // Assuming shipTexture is your ship's texture
            float scale = (screenHeight * shipScaleFactor) / shipHeight;
            m_ship = new Ship(new Vector2(50, 50), new Vector2(0, 0), new Vector2(1, 0), scale);

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

            float rotationAngle = (float)Math.Atan2(m_ship.direction.Y, m_ship.direction.X);
            Vector2 origin = new Vector2(m_texShip.Width / 2f, m_texShip.Height / 2f);
            Vector2 circle_origin = new Vector2(m_texCircle.Width / 2, m_texCircle.Height / 2);
            float scale = m_ship.radius / (m_texCircle.Width / 2);

            m_spriteBatch.Draw(
                m_texShip, 
                new Vector2(m_ship.position.X, m_ship.position.Y),
                null, 
                Color.White, 
                rotationAngle, 
                origin, 
                0.5f * m_ship.scale, 
                SpriteEffects.None, 
                0f);
            m_spriteBatch.Draw(
                m_texCircle, 
                m_ship.position,
                null,
                Color.White,
                0f,
                circle_origin,
                scale,
                SpriteEffects.None,
                0f);

            string velocityText = $"Velocity: {m_ship.getMeterPerSec():F2} m/s";
            Vector2 position = new Vector2(10, 10); // Example position for the text
            m_spriteBatch.DrawString(m_font, velocityText, position, Color.White);
            Vector2 position_fuel = new Vector2(10, 30); // Example position for the text
            m_spriteBatch.DrawString(m_font, m_ship.fuel.ToString(), position_fuel, Color.White);
            m_spriteBatch.DrawString(m_font, m_ship.GetRotationInDegrees().ToString(), new Vector2(1800, 30), Color.White);
            m_spriteBatch.DrawString(m_font, state, new Vector2(1000, 30), Color.White);
            m_spriteBatch.DrawString(m_font, m_ship.scale.ToString(), new Vector2(1000, 100), Color.White);

            m_spriteBatch.End();
        }

        public override void update(GameTime gameTime)
        {
            m_ship.Update(gameTime);
            state = "";
            // TODO: remove test colide
            if (m_ship.LineIntersectsCircle(new Vector2(100, 100), new Vector2(200, 200)))
            {
                
            }
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
                            state = "Landing";
                        }
                        else 
                        {
                            m_ship.isDead = true;
                            state = "Dead";

                        }
                    }
                    else 
                    {
                        m_ship.isDead = true;
                        state = "Dead";
                    }
                }
            }
        }

    }
}
