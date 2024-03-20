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

            string creditsText = "                                                                Credits:\n";

            creditsText += "                                             Created by Jonathan Gates\n";
            creditsText += "Ship Texture:" +
                "\n        https://opengameart.org/content/rocket\n";
            creditsText += "Thrust and Crash Particles: " +
                "\n       https://opengameart.org/content/smoke-particle-assets\n";
            creditsText += "Crash Sound: " +
                "\n       https://opengameart.org/content/big-explosion\n";
            creditsText += "Thrust Sound: " +
                "\n       https://opengameart.org/content/fire-loop\n";
            creditsText += "Landing Sound: " +
                "\n       https://opengameart.org/content/win-sound-effect\n";
            creditsText += "Collision Circle: " +
                "\n       https://commons.wikimedia.org/wiki/File:Circle_%28transparent%29.png";

            Vector2 screenSize = new Vector2(m_graphics.GraphicsDevice.Viewport.Width, m_graphics.GraphicsDevice.Viewport.Height);
            Vector2 position = screenSize / 2;
            int screenHeight = m_graphics.GraphicsDevice.Viewport.Height;
            float scale = (float)((screenHeight * 0.1) / 110);
            drawOutlineText(m_spriteBatch, m_font, creditsText, Color.Black, Color.White, position, scale);
            m_spriteBatch.End();
        }

        public override void update(GameTime gameTime)
        {
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
