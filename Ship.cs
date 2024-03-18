using Maze.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunarLander
{
    class Ship
    {
        public Vector2 position { get; private set; }
        public Vector2 velocity { get; private set; }
        public Vector2 direction { get; private set; }
        private KeyboardInput m_inputKeyboard;
        private bool controlable;
        private const float gravity = 6f;
        private const float thrustAmount = 15.0f;

        public Ship(Vector2 position, Vector2 velocity, Vector2 direction, bool controlable) 
        { 
            this.position = position;
            this.velocity = velocity;
            this.direction = direction;
            this.controlable = controlable;

            m_inputKeyboard = new KeyboardInput();

            // TODO: get keys from memory
            m_inputKeyboard.registerCommand(Keys.W, false, new IInputDevice.CommandDelegate(thrust));
            m_inputKeyboard.registerCommand(Keys.A, false, new IInputDevice.CommandDelegate(rotateLeft));
            m_inputKeyboard.registerCommand(Keys.D, false, new IInputDevice.CommandDelegate(rotateRight));
        }

        public void Update(GameTime gameTime) 
        {
            m_inputKeyboard.Update(gameTime);

            addGravity(gameTime);
            updatePosition(gameTime);
        }

        private void addGravity(GameTime gameTime)
        {
            if (!controlable) return;

            velocity += new Vector2(0, gravity * (float)(gameTime.ElapsedGameTime.TotalMilliseconds) / 1000.0f);
        }

        private void thrust(GameTime gameTime, float scale) 
        {
            if (!controlable) return;

            velocity += Vector2.Normalize(direction) * thrustAmount * (float)(gameTime.ElapsedGameTime.TotalMilliseconds) / 1000.0f;
        }

        private void rotateLeft(GameTime gameTime, float scale)
        {
            if (!controlable) return;

            float rotationAngle = scale * (float)(gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0);
            float cosTheta = (float)Math.Cos(rotationAngle);
            float sinTheta = (float)Math.Sin(rotationAngle);

            Vector2 newDirection = new Vector2(
                direction.X * cosTheta + direction.Y * sinTheta,
                direction.X * -sinTheta + direction.Y * cosTheta
            );

            direction = Vector2.Normalize(newDirection);
        }

        private void rotateRight(GameTime gameTime, float scale)
        {
            if (!controlable) return;

            float rotationAngle = scale * (float)(gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0);
            float cosTheta = (float)Math.Cos(rotationAngle);
            float sinTheta = (float)Math.Sin(rotationAngle);

            Vector2 newDirection = new Vector2(
                direction.X * cosTheta - direction.Y * sinTheta,
                direction.X * sinTheta + direction.Y * cosTheta
            );

            direction = Vector2.Normalize(newDirection);
        }

        private void updatePosition(GameTime gameTime)
        {
            if (!controlable) return;

            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
