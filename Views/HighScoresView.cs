using LunarLander;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace CS5410
{
    public class HighScoresView : GameStateView
    {
        private SpriteFont m_font;
        private PersistenceManager m_persistenceManager;

        public override void loadContent(ContentManager contentManager)
        {
            m_font = contentManager.Load<SpriteFont>("Fonts/menu");
            m_persistenceManager = new PersistenceManager();
            loadHighScores();
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                return GameStateEnum.MainMenu;
            }

            return GameStateEnum.HighScores;
        }

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();

            string scoresText = "Top 5 HighScores:\n";

            if (m_persistenceManager.m_scoresPersistence != null)
            { 
                foreach (float score in m_persistenceManager.m_scoresPersistence.Scores)
                {
                    scoresText +=  "          " + score.ToString("F3") + "\n";
                }
            }
            int screenHeight = m_graphics.GraphicsDevice.Viewport.Height;
            float scale = (float)((screenHeight * 0.1) / 110);
            Vector2 screenSize = new Vector2(m_graphics.GraphicsDevice.Viewport.Width, m_graphics.GraphicsDevice.Viewport.Height);
            Vector2 position = screenSize / 2;

            // scale to fit width
            float baseScreenWidth = 1920; // Assume 1920 is the base width you designed for
            float scalingFactor = m_graphics.GraphicsDevice.Viewport.Width / baseScreenWidth;
            float textScale = scalingFactor;
            Vector2 textSize = m_font.MeasureString(scoresText) * textScale;
            if (textSize.X > m_graphics.GraphicsDevice.Viewport.Width)
            {
                // The text is too wide to fit on the screen, reduce the scale further
                textScale *= m_graphics.GraphicsDevice.Viewport.Width / textSize.X;
            }

            drawOutlineText(m_spriteBatch, m_font, scoresText, Color.Black, Color.White, position, textScale);

            m_spriteBatch.End();
        }

        public override void update(GameTime gameTime)
        {
        }

        public void loadHighScores()
        {
            m_persistenceManager.loadHighScores();
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
