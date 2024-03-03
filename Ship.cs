using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunarLander
{
    internal class Ship
    {
        public Vector2 position { get; private set; }
        public Vector2 velocity { get; private set; }
        public Vector2 direction { get; private set; }
        private bool controlable;
        private const float gravity = 1.62f;
        private const float thrustAmount = 5f;
        public bool thrustOn;

        public Ship(Vector2 position, Vector2 velocity, Vector2 direction, bool controlable) 
        { 
            this.position = position;
            this.velocity = velocity;
            this.direction = direction;
            this.controlable = controlable;
        }

        public void Update(GameTime gameTime) 
        {
            addGravity(gameTime);
            thrust(gameTime);
            updatePosition(gameTime);
        }

        private void addGravity(GameTime gameTime)
        {
            if (!controlable) return;

            velocity += new Vector2(0, gravity * (float)(gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0));
        }

        private void thrust(GameTime gameTime) 
        {
            if (!controlable || !thrustOn) return;

            velocity += Vector2.Normalize(direction) * thrustAmount * (float)(gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0);
        }

        private void updatePosition(GameTime gameTime)
        {
            if (!controlable) return;

            position += new Vector2((int)(velocity.X * (float)(gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0)), (int)(velocity.Y * (float)(gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0)));
        }

        public void toggleThrust(GameTime gameTime)
        {
            thrustOn = !thrustOn;
        }
    }
}
