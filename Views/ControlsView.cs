using LunarLander;
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
        private PersistenceManager m_persistenceManager;
        public Keys thrustKey;
        public Keys rotateLeftKey;
        public Keys rotateRightKey;
        private double m_showChangeAvailable;

        private enum ControlsState
        {
            Thrust,
            RotateLeft,
            RotateRight
        }

        private ControlsState m_currentSelection = ControlsState.Thrust;
        private bool m_waitForKeyRelease;
        private bool m_controlsLoaded;

        public override void loadContent(ContentManager contentManager)
        {
            m_fontMenu = contentManager.Load<SpriteFont>("Fonts/menu");
            m_fontMenuSelect = contentManager.Load<SpriteFont>("Fonts/menu-select");
            m_persistenceManager = new PersistenceManager();
            m_persistenceManager.loadControls();
            m_waitForKeyRelease = true;
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                if (m_pickingNewButton)
                {
                    m_pickingNewButton = false;
                    m_waitForKeyRelease = true;
                    m_showChangeAvailable = 0;
                }
                else
                {
                    m_pickingNewButton = false;
                    m_waitForKeyRelease = true;
                    m_showChangeAvailable = 0;
                    return GameStateEnum.MainMenu;
                }
            }
            // This is the technique I'm using to ensure one keypress makes one menu navigation move
            if (!m_waitForKeyRelease)
            {

                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    m_pickingNewButton = true;
                    m_waitForKeyRelease = true;
                    m_showChangeAvailable += 2;
                }

                else if (m_pickingNewButton && Keyboard.GetState().GetPressedKeys().Length > 0)
                {
                    // TODO: get key and change button
                    Keys[] pressedKeys = Keyboard.GetState().GetPressedKeys();
                    if (pressedKeys.Length > 0)
                    { 
                        Keys newKey = pressedKeys[0];
                        if (newKey != Keys.Escape)
                        { 
                            setKeyBasedOnSelection(newKey);
                            m_persistenceManager.saveControls(thrustKey, rotateLeftKey, rotateRightKey);
                        }
                    }
                    m_pickingNewButton = false;
                    m_showChangeAvailable = 0;
                    m_waitForKeyRelease = true;
                }

                // Arrow keys to navigate the menu
                else if (!m_pickingNewButton && Keyboard.GetState().IsKeyDown(Keys.Down))
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
                else if (!m_pickingNewButton && Keyboard.GetState().IsKeyDown(Keys.Up))
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
            else if (Keyboard.GetState().IsKeyUp(Keys.Down) && Keyboard.GetState().IsKeyUp(Keys.Up) && Keyboard.GetState().IsKeyUp(Keys.Enter))
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
                "Thrust: " + thrustKey.ToString(),
                200,
                getMenuItemColor(m_currentSelection == ControlsState.Thrust));
            bottom = drawMenuItem(m_currentSelection == ControlsState.RotateLeft ? m_fontMenuSelect : m_fontMenu, "Rotate Left: " + rotateLeftKey.ToString(), bottom, getMenuItemColor(m_currentSelection == ControlsState.RotateLeft));
            drawMenuItem(m_currentSelection == ControlsState.RotateRight ? m_fontMenuSelect : m_fontMenu, "Rotate Right: " + rotateRightKey.ToString(), bottom, getMenuItemColor(m_currentSelection == ControlsState.RotateRight));

            m_spriteBatch.End();
        }

        private void setKeyBasedOnSelection(Keys newKey)
        {
            if (m_currentSelection == ControlsState.Thrust)
            {
                thrustKey = newKey;
            }
            else if (m_currentSelection == ControlsState.RotateLeft)
            {
                rotateLeftKey = newKey;
            }
            else
            {
                rotateRightKey = newKey;
            }
        }

        private Color getMenuItemColor(bool isCurrentSelection)
        {
            if (isCurrentSelection && m_pickingNewButton && (int)m_showChangeAvailable % 2 == 0)
            {
                return Color.Green;
            }
            else if (isCurrentSelection)
            { 
                return Color.Yellow;
            }
            else
            {
                return Color.Blue;
            }
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
            if (!m_controlsLoaded && m_persistenceManager.m_controlsPersistence != null)
            {
                thrustKey = m_persistenceManager.m_controlsPersistence.ThrustKey;
                rotateLeftKey = m_persistenceManager.m_controlsPersistence.RotateLeftKey;
                rotateRightKey = m_persistenceManager.m_controlsPersistence.RotateRightKey;
                m_controlsLoaded = true;
            }

            if (m_pickingNewButton)
            {
                m_showChangeAvailable += gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
    }
}
