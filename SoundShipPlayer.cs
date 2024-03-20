using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunarLander
{
    class SoundShipPlayer
    {
        private SoundEffect m_thrustSound;
        private SoundEffectInstance m_thrustSoundInstance;
        private SoundEffect m_crash;
        private SoundEffect m_landed;

        private bool canPlayCrash = true;


        public SoundShipPlayer(SoundEffect thrust, SoundEffect crash, SoundEffect landed)
        { 
            this.m_thrustSound = thrust;
            this.m_thrustSoundInstance = this.m_thrustSound.CreateInstance();
            m_thrustSoundInstance.IsLooped = true;
            this.m_crash = crash;
            this.m_landed = landed;
        }

        public void updateThrustSound(bool isThrusting)
        {
            if (isThrusting && m_thrustSoundInstance.State != SoundState.Playing)
            {
                m_thrustSoundInstance.Play();
            }
            else if (!isThrusting && m_thrustSoundInstance.State == SoundState.Playing)
            {
                m_thrustSoundInstance.Stop();
            }
        }

        public void playCrash()
        { 
            if (canPlayCrash)
            {
                m_crash.Play();
                canPlayCrash = false;
            }
        }

        public void playLanded()
        { 
            m_landed.Play();
        }
    }
}
