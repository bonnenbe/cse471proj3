using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;

namespace StepDX
{
    public class GameSounds
    {
        private Device SoundDevice = null;
        private SecondaryBuffer[] clank = new SecondaryBuffer[10];
        int clankToUse = 0;

        private SecondaryBuffer soundCollision = null;
        private SecondaryBuffer soundDeath = null;
        private SecondaryBuffer soundSong = null;
        private SecondaryBuffer soundThanks = null;

        public GameSounds(Form form)
        {
            SoundDevice = new Device();
            SoundDevice.SetCooperativeLevel(form, CooperativeLevel.Priority);

            Load(ref soundDeath, "../../wario_dies.wav");
            Load(ref soundCollision, "../../thud.wav");
            Load(ref soundSong, "../../megaman_title.wav");
            Load(ref soundThanks, "../../mario_thank_you.wav");

            SoundSong();
        }

        private void Load(ref SecondaryBuffer buffer, string filename)
        {
            try
            {
                buffer = new SecondaryBuffer(filename, SoundDevice);
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to load " + filename,
                                "Danger, Will Robinson", MessageBoxButtons.OK);
                buffer = null;
            }
        }

        public void SoundCollision()
        {
            if (soundCollision == null)
                return;

            if (!soundCollision.Status.Playing)
                soundCollision.Play(0, BufferPlayFlags.Default);
        }

        public void SoundThanks()
        {
            if (soundThanks == null)
                return;

            if (!soundThanks.Status.Playing)
                soundThanks.Play(0, BufferPlayFlags.Default);
        }

        public void SoundSong()
        {
            if (soundSong == null)
                return;

            if (!soundSong.Status.Playing)
                soundSong.Play(0, BufferPlayFlags.Default);
        }

        public void SoundDeath()
        {
            if (soundDeath == null)
                return;

            if (!soundDeath.Status.Playing)
            {
                soundDeath.SetCurrentPosition(0);
                soundDeath.Play(0, BufferPlayFlags.Default);
            }
        }

        public void SoundDeathEnd()
        {
            if (soundDeath == null)
                return;

            if (soundDeath.Status.Playing)
                soundDeath.Stop();
        }

    }
}
