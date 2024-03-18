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
        public  float radius { get; private set; }
        private KeyboardInput m_inputKeyboard;
        private const float gravity = 6f;
        private const float thrustAmount = 15.0f;
        public float fuel = 20000.0f;
        public bool isDead;

        public Ship(Vector2 position, Vector2 velocity, Vector2 direction, float radius) 
        { 
            this.position = position;
            this.velocity = velocity;
            this.direction = direction;
            this.radius = radius;

            m_inputKeyboard = new KeyboardInput();

            // TODO: get keys from memory
            m_inputKeyboard.registerCommand(Keys.W, false, new IInputDevice.CommandDelegate(thrust));
            m_inputKeyboard.registerCommand(Keys.A, false, new IInputDevice.CommandDelegate(rotateLeft));
            m_inputKeyboard.registerCommand(Keys.D, false, new IInputDevice.CommandDelegate(rotateRight));
        }

        public void Update(GameTime gameTime) 
        {
            m_inputKeyboard.Update(gameTime);

            //addGravity(gameTime);
            updatePosition(gameTime);
        }

        private void addGravity(GameTime gameTime)
        {
            velocity += new Vector2(0, gravity * (float)(gameTime.ElapsedGameTime.TotalMilliseconds) / 1000.0f);
        }

        private void thrust(GameTime gameTime, float scale) 
        {
            if (isDead || fuel <= 0) return;

            velocity += Vector2.Normalize(direction) * thrustAmount * (float)(gameTime.ElapsedGameTime.TotalMilliseconds) / 1000.0f;
            fuel -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        private void rotateLeft(GameTime gameTime, float scale)
        {
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
            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public bool LineIntersectsCircle(Vector2 pt1, Vector2 pt2)
        {
            Vector2 v1 = pt2 - pt1;
            Vector2 v2 = pt1 - this.position; // Use the ship's position as the circle's center
            float b = -2 * (v1.X * v2.X + v1.Y * v2.Y);
            float c = 2 * (v1.X * v1.X + v1.Y * v1.Y);
            float dSquared = b * b - 2 * c * (v2.X * v2.X + v2.Y * v2.Y - this.radius * this.radius);

            if (dSquared < 0) // No intersection
            {
                return false;
            }

            float d = (float)Math.Sqrt(dSquared);

            // These represent the unit distance of point one and two on the line
            float u1 = (b - d) / c;
            float u2 = (b + d) / c;

            return (u1 <= 1 && u1 >= 0) || (u2 <= 1 && u2 >= 0); // If point on the line segment
        }
    }
}
