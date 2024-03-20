using LunarLander;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CS5410
{
    public class CreditsView : GameStateView
    {
        private SpriteFont m_font;
        private const string MESSAGE = "Credits View";

        public override void loadContent(ContentManager contentManager)
        {
            m_font = contentManager.Load<SpriteFont>("Fonts/menu");
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                return GameStateEnum.MainMenu;
            }

            return GameStateEnum.Credits;
        }

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();

            string creditsText = "Credits:\n";

            creditsText += "Created by Jonathan Gates";
            creditsText += "Ship Texture: https://opengameart.org/content/rocket";
            creditsText += "Thrust and Crash Particles: https://opengameart.org/content/smoke-particle-assets";
            creditsText += "Crash Sound: https://opengameart.org/content/big-explosion";
            creditsText += "Thrust Sound: https://opengameart.org/content/fire-loop";
            creditsText += "Landing Sound: https://opengameart.org/content/win-sound-effect";

            Vector2 screenSize = new Vector2(m_graphics.GraphicsDevice.Viewport.Width, m_graphics.GraphicsDevice.Viewport.Height);
            Vector2 textSize = m_font.MeasureString(creditsText);
            Vector2 position = (screenSize - textSize) / 2;
            m_spriteBatch.DrawString(m_font, creditsText, position, Color.White);

            m_spriteBatch.End();
        }

        public override void update(GameTime gameTime)
        {
        }
    }
}
