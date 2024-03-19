using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace CS5410
{
    public class ParticleSystem
    {
        private Dictionary<long, Particle> m_particles = new Dictionary<long, Particle>();
        public Dictionary<long, Particle>.ValueCollection particles { get { return m_particles.Values; } }
        private MyRandom m_random = new MyRandom();

        private int m_sizeMean; // pixels
        private int m_sizeStdDev;   // pixels
        private float m_speedMean;  // pixels per millisecond
        private float m_speedStDev; // pixles per millisecond
        private float m_lifetimeMean; // milliseconds
        private float m_lifetimeStdDev; // milliseconds

        public ParticleSystem(int sizeMean, int sizeStdDev, float speedMean, float speedStdDev, int lifetimeMean, int lifetimeStdDev)
        {
            m_sizeMean = sizeMean;
            m_sizeStdDev = sizeStdDev;
            m_speedMean = speedMean;
            m_speedStDev = speedStdDev;
            m_lifetimeMean = lifetimeMean;
            m_lifetimeStdDev = lifetimeStdDev;
        }

        //private Particle create(Vector2 center)
        //{
        //    float size = (float)m_random.nextGaussian(m_sizeMean, m_sizeStdDev);
        //    var p = new Particle(
        //            center,
        //            m_random.nextCircleVector(),
        //            (float)m_random.nextGaussian(m_speedMean, m_speedStDev),
        //            new Vector2(size, size),
        //            new System.TimeSpan(0, 0, 0, 0, (int)(m_random.nextGaussian(m_lifetimeMean, m_lifetimeStdDev)))); ;

        //    return p;
        //}

        public void update(GameTime gameTime)
        {
            // Update existing particles
            List<long> removeMe = new List<long>();
            foreach (Particle p in m_particles.Values)
            {
                if (!p.update(gameTime))
                {
                    removeMe.Add(p.name);
                }
            }

            // Remove dead particles
            foreach (long key in removeMe)
            {
                m_particles.Remove(key);
            }

        }

        private Particle createThrustParticle(Vector2 center, float baseAngle)
        {
            float size = (float)m_random.nextGaussian(m_sizeMean, m_sizeStdDev);
            var p = new Particle(
                    center,
                    generateParticleDirection(baseAngle),
                    (float)m_random.nextGaussian(m_speedMean, m_speedStDev),
                    new Vector2(size, size),
                    new System.TimeSpan(0, 0, 0, 0, (int)(m_random.nextGaussian(m_lifetimeMean, m_lifetimeStdDev)))); ;

            return p;
        }

        private Particle createCrashParticle(Vector2 center)
        {
            float size = (float)m_random.nextGaussian(m_sizeMean, m_sizeStdDev);
            var p = new Particle(
                    center,
                    m_random.nextCircleVector(),
                    (float)m_random.nextGaussian(m_speedMean, m_speedStDev),
                    new Vector2(size, size),
                    new System.TimeSpan(0, 0, 0, 0, (int)(m_random.nextGaussian(m_lifetimeMean, m_lifetimeStdDev)))); ;

            return p;
        }

        public void shipThrust(Vector2 center, float baseAngle)
        {
            // Generate some new particles
            for (int i = 0; i < 8; i++)
            {
                var particle = createThrustParticle(center, baseAngle);
                m_particles.Add(particle.name, particle);
            }
        }

        public void shipCrash(Vector2 center)
        {
            // Generate some new particles
            for (int i = 0; i < 360; i++)
            {
                var particle = createCrashParticle(center);
                m_particles.Add(particle.name, particle);
            }
        }

        private Vector2 generateParticleDirection(float baseAngle)
        {
            float spreadAngle = MathHelper.ToRadians(30);
            float randomAngle = baseAngle + (float)(m_random.NextDouble() - 0.5) * spreadAngle;
            Vector2 particleDirection = new Vector2((float)Math.Cos(randomAngle), (float)Math.Sin(randomAngle));
            return particleDirection;
        }
    }
}