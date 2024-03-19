using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CS5410
{
    public class ControlsView : GameStateView
    {
        private SpriteFont m_fontMenu;
        private SpriteFont m_fontMenuSelect;
        private bool m_pickingNewButton;

        private enum ControlsState
        {
            Thrust,
            RotateLeft,
            RotateRight
        }

        private ControlsState m_currentSelection = ControlsState.Thrust;
        private bool m_waitForKeyRelease = false;

        public override void loadContent(ContentManager contentManager)
        {
            m_fontMenu = contentManager.Load<SpriteFont>("Fonts/menu");
            m_fontMenuSelect = contentManager.Load<SpriteFont>("Fonts/menu-select");
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                if (m_pickingNewButton)
                {
                    m_pickingNewButton = false;
                }
                else
                { 
                    return GameStateEnum.MainMenu;
                }
            }

            if (m_pickingNewButton)
            {
                // TODO: get key and change button
                Keys newKey = Keyboard.GetState().GetPressedKeys()[0];
            }

            // This is the technique I'm using to ensure one keypress makes one menu navigation move
            if (!m_waitForKeyRelease)
            {
                // Arrow keys to navigate the menu
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    // Wrap around
                    if (m_currentSelection < ControlsState.RotateRight)
                    {
                        m_currentSelection++;
                    }
                    else
                    {
                        m_currentSelection = ControlsState.Thrust;
                    }
                    m_waitForKeyRelease = true;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    // Wrap around
                    if (m_currentSelection > ControlsState.Thrust)
                    {
                        m_currentSelection--;
                    }
                    else
                    {
                        m_currentSelection = ControlsState.RotateRight;
                    }
                    m_waitForKeyRelease = true;
                }
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.Down) && Keyboard.GetState().IsKeyUp(Keys.Up))
            {
                m_waitForKeyRelease = false;
            }

            return GameStateEnum.Controls;
        }

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();

            // I split the first one's parameters on separate lines to help you see them better
            float bottom = drawMenuItem(
                m_currentSelection == ControlsState.Thrust ? m_fontMenuSelect : m_fontMenu,
                "Thrust",
                200,
                m_currentSelection == ControlsState.Thrust ? Color.Yellow : Color.Blue);
            bottom = drawMenuItem(m_currentSelection == ControlsState.RotateLeft ? m_fontMenuSelect : m_fontMenu, "Rotate Left", bottom, m_currentSelection == ControlsState.RotateLeft ? Color.Yellow : Color.Blue);
            drawMenuItem(m_currentSelection == ControlsState.RotateRight ? m_fontMenuSelect : m_fontMenu, "Rotate Right", bottom, m_currentSelection == ControlsState.RotateRight ? Color.Yellow : Color.Blue);

            m_spriteBatch.End();
        }

        private float drawMenuItem(SpriteFont font, string text, float y, Color color)
        {
            Vector2 stringSize = font.MeasureString(text);
            m_spriteBatch.DrawString(
                font,
                text,
                new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringSize.X / 2, y),
                color);

            return y + stringSize.Y;
        }

        public override void update(GameTime gameTime)
        {
        }
    }
}
