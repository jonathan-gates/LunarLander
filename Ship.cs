using CS5410;
using Maze.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace LunarLander
{
    class Ship
    {
        public Vector2 position { get; private set; }
        public Vector2 velocity { get; private set; }
        public Vector2 direction { get; private set; }
        private SoundShipPlayer player;
        public  float collisionRadius { get; private set; }
        public float scale;
        private KeyboardInput m_inputKeyboard;
        public float targetRadius = 38.0f;
        private const float gravity = 6f;
        private const float thrustAmount = 20.0f;
        public float fuel = 20.0f;
        private float oldFuel = 20.0f;
        private bool thrustOn;
        private bool isDead;
        private bool hasLanded;
        private bool controlable;
        private ParticleSystem m_particleSystemThrust;
        private ParticleSystem m_particleSystemCrash;
        private ParticleSystemRenderer m_renderThrust;
        private ParticleSystemRenderer m_renderCrash;
        private PersistenceManager m_persistenceManager;
        private bool m_controlsLoaded;

        public Ship(Vector2 position, Vector2 velocity, Vector2 direction, float scale, SoundEffect thrustSound, SoundEffect crashSound, SoundEffect landedSound, ContentManager content) 
        { 
            this.position = position;
            this.velocity = velocity;
            this.direction = direction;
            this.scale = scale;
            this.collisionRadius = targetRadius * scale;

            m_persistenceManager = new PersistenceManager();
            m_persistenceManager.loadControls();

            m_inputKeyboard = new KeyboardInput();

            player = new SoundShipPlayer(thrustSound, crashSound, landedSound);

            m_particleSystemThrust = new ParticleSystem(
                10, 4,
                0.12f, 0.05f,
                1000, 250);
            m_renderThrust = new ParticleSystemRenderer("Images/thrust");
            m_renderThrust.LoadContent(content);

            m_particleSystemCrash = new ParticleSystem(
                10, 4,
                0.12f, 0.05f,
                2000, 500);
            m_renderCrash = new ParticleSystemRenderer("Images/crash");
            m_renderCrash.LoadContent(content);
        }

        public void Update(GameTime gameTime) 
        {
            if (!m_controlsLoaded && m_persistenceManager.m_controlsPersistence != null)
            { 
                m_inputKeyboard.registerCommand(m_persistenceManager.m_controlsPersistence.ThrustKey, false, new IInputDevice.CommandDelegate(thrust));
                m_inputKeyboard.registerCommand(m_persistenceManager.m_controlsPersistence.RotateLeftKey, false, new IInputDevice.CommandDelegate(rotateLeft));
                m_inputKeyboard.registerCommand(m_persistenceManager.m_controlsPersistence.RotateRightKey, false, new IInputDevice.CommandDelegate(rotateRight));
                m_controlsLoaded = true;
                controlable = true;
            }

            m_inputKeyboard.Update(gameTime);
            if (oldFuel == fuel)
            {
                thrustOn = false;
                player.updateThrustSound(thrustOn);
            }
            oldFuel = fuel;
            addGravity(gameTime);
            updatePosition(gameTime);
            m_particleSystemThrust.update(gameTime);
            m_particleSystemCrash.update(gameTime);
        }

        private void addGravity(GameTime gameTime)
        {
            if (!controlable || isDead) return;
            velocity += new Vector2(0, gravity * scale * (float)(gameTime.ElapsedGameTime.TotalMilliseconds) / 1000.0f);
        }

        private void thrust(GameTime gameTime, float scale) 
        {
            if (isDead || fuel <= 0 || !controlable) return;
            thrustOn = true;
            m_particleSystemThrust.shipThrust(
                position - Vector2.Normalize(direction) * collisionRadius,
                (float)Math.Atan2(-direction.Y, -direction.X));
            player.updateThrustSound(thrustOn);
            velocity += Vector2.Normalize(direction) * thrustAmount * scale * (float)(gameTime.ElapsedGameTime.TotalMilliseconds) / 1000.0f;
            fuel -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        private void rotateLeft(GameTime gameTime, float scale)
        {
            if (!controlable || isDead) return;

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
            if (!controlable || isDead) return;

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
            if (!controlable || isDead) return;

            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public bool checkLandedSafely()
        {
            if (getMeterPerSec() <= 2 && (GetRotationInDegrees() <= 5 || GetRotationInDegrees() >= 355))
            {
                return true;
            }
            return false;
        }

        public void hasWon()
        {
            if (!hasLanded)
            { 
                hasLanded = true;
                controlable= false;
                // TODO: other stuff, stop time. Maybe return winning values
                player.playLanded();
            }
        }

        public void hasDied()
        {
            if (!isDead)
            { 
                isDead = true;
                controlable= false;
                player.playCrash();
                m_particleSystemCrash.shipCrash(position);
            }
        }

        public float getMeterPerSec()
        {
            return velocity.Y / 11;
        }

        public float GetRotationInDegrees()
        {
            float angleRadians = (float)Math.Atan2(direction.X, -direction.Y);

            // Convert radians to degrees
            float angleDegrees = angleRadians * (180f / (float)Math.PI);

            // Normalize the angle to be within the range [0, 360)
            if (angleDegrees < 0)
            {
                angleDegrees += 360f;
            }

            return angleDegrees;
        }

        public bool LineIntersectsCircle(Vector2 pt1, Vector2 pt2)
        {
            Vector2 v1 = pt2 - pt1;
            Vector2 v2 = pt1 - this.position; // Use the ship's position as the circle's center
            float b = -2 * (v1.X * v2.X + v1.Y * v2.Y);
            float c = 2 * (v1.X * v1.X + v1.Y * v1.Y);
            float dSquared = b * b - 2 * c * (v2.X * v2.X + v2.Y * v2.Y - this.collisionRadius * this.collisionRadius);

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

        public void render(SpriteBatch m_spriteBatch, Texture2D m_texShip, Texture2D m_texCircle, float m_scale)
        {
            float rotationAngle = (float)Math.Atan2(this.direction.Y, this.direction.X);
            Vector2 origin = new Vector2(m_texShip.Width / 2f, m_texShip.Height / 2f);
            Vector2 circle_origin = new Vector2(m_texCircle.Width / 2, m_texCircle.Height / 2);

            if (!isDead)
            { 
                m_spriteBatch.Draw(
                    m_texShip,
                    new Vector2(this.position.X, this.position.Y),
                    null,
                    Color.White,
                    rotationAngle,
                    origin,
                    0.5f * this.scale,
                    SpriteEffects.None,
                    0f);
                m_spriteBatch.Draw(
                    m_texCircle,
                    this.position,
                    null,
                    Color.White,
                    0f,
                    circle_origin,
                    m_scale,
                    SpriteEffects.None,
                    0f);
            }

            m_renderThrust.draw(m_spriteBatch, m_particleSystemThrust);
            m_renderCrash.draw(m_spriteBatch, m_particleSystemCrash);
        }
    }
}
