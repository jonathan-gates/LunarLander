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
            Vector2 screenSize = new Vector2(m_graphics.GraphicsDevice.Viewport.Width, m_graphics.GraphicsDevice.Viewport.Height);
            Vector2 textSize = m_font.MeasureString(scoresText);
            Vector2 position = (screenSize - textSize) / 2;
            m_spriteBatch.DrawString(m_font, scoresText, position, Color.White);

            m_spriteBatch.End();
        }

        public override void update(GameTime gameTime)
        {
        }

        public void loadHighScores()
        {
            m_persistenceManager.loadHighScores();
        }
    }
}
