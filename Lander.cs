﻿using CS5410;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace LunarLander
{
    public class Lander : Game
    {
        private GraphicsDeviceManager m_graphics;
        private IGameState m_currentState;
        private Dictionary<GameStateEnum, IGameState> m_states;

        public Lander()
        {
            m_graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            m_graphics.PreferredBackBufferWidth = 1920;
            m_graphics.PreferredBackBufferHeight = 1080;

            m_graphics.ApplyChanges();

            // Create all the game states here
            m_states = new Dictionary<GameStateEnum, IGameState>
            {
                { GameStateEnum.MainMenu, new MainMenuView() },
                { GameStateEnum.GamePlay, new GamePlayView() },
                { GameStateEnum.HighScores, new HighScoresView() },
                { GameStateEnum.Controls, new ControlsView() },
                { GameStateEnum.Credits, new CreditsView() }
            };

            // Give all game states a chance to initialize, other than constructor
            foreach (var item in m_states)
            {
                item.Value.initialize(this.GraphicsDevice, m_graphics);
            }

            // We are starting with the main menu
            m_currentState = m_states[GameStateEnum.MainMenu];

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Give all game states a chance to load their content
            foreach (var item in m_states)
            {
                item.Value.loadContent(this.Content);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            GameStateEnum nextStateEnum = m_currentState.processInput(gameTime);

            // Special case for exiting the game
            if (nextStateEnum == GameStateEnum.Exit)
            {
                Exit();
            }
            else
            {
                m_currentState.update(gameTime);
                if (nextStateEnum == GameStateEnum.GamePlay && m_currentState is not GamePlayView)
                {

                    ((GamePlayView)m_states[nextStateEnum]).resetGameState();
                    ((GamePlayView)m_states[nextStateEnum]).loadHighScores();
                }
                else if (nextStateEnum == GameStateEnum.HighScores && m_currentState is not HighScoresView)
                {
                    ((HighScoresView)m_states[nextStateEnum]).loadHighScores();
                }
                m_currentState = m_states[nextStateEnum];
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            m_currentState.render(gameTime);

            base.Draw(gameTime);
        }
    }
}