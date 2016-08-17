using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WizardWarzRotW
{
    /// <summary>
    /// Interaction logic for AudioManager.xaml. Runs all music within the game. Needs to be initialised through an instance, within each class (which needs sound/music).
    /// </summary>
    public partial class AudioManager : UserControl
    {
        MediaPlayer jukeBox = new MediaPlayer();
        public string trackLocation;
        public double newVolume;
        public bool titleOrMain = false; // false for title, true for main
        bool isLooping = false;
        public bool audioOn = true;

        // All sound plays have a 'Stop Track', this is to prevent stacking sounds. 


        public AudioManager()
        {
            InitializeComponent();
            audioOn = MainWindow.GlobalAudio1;
        }


        /// <summary>
        /// Runs the main music track within the Audio Manager. Audio Manager is required to be instanced within each class where a track is to be played. Ex. AudioManager newAudioMan = new AudioManager(); ... newAudioMan.playMainMusic();
        /// </summary> 
        public void playMainMusic()
        {
            titleOrMain = true;
            isLooping = true;
            trackLocation = "Wizard_Warz_02.mp3";
            newVolume = 0.15;
            PlayTrack();
        }

        public void playWizardOne()
        {
            titleOrMain = false;
            isLooping = true;
            trackLocation = "Wizard_Warz_00.wav";
            newVolume = 0.15;
            PlayTrack();
        }

        //public void playGameTheme()
        //{

        //    trackLocation = "gameTheme.mp3";
        //    newVolume = 0.15;
        //    PlayTrack();
        //}

        /// <summary>
        /// Calculate audio volume is called from the PlayTrack section, and essentially checks the global audio bool, as to whether it is True or False. Depending on the answer, it will set the audio accordingly.
        /// </summary> 
        private void CalculateAudioVolume()
        {
            if (!audioOn)
            {
                jukeBox.Volume = 0;
            }
            else if (audioOn)
            {
                jukeBox.Volume = newVolume;

            }
        }

        /// <summary>
        /// Stops the currently playing track. This is used to prevent tracks stacking, and multiple sounds being played at once, from the same source. 
        /// Ex. Bomb sound is played, and then played again soon after - rather than the track playing twice, it will stop the currently playing sound
        /// </summary> 
        public void StopTrack()
        {
            jukeBox.Stop();

        }

        /// <summary>
        /// The PlayTrack function is only required within the Audio Manager, and automatically run from each track function. If adding additional sounds, Stop and Play track functions need to appear there. 
        /// </summary> 
        private void PlayTrack()
        {
            // Check whether song should loop - if it is meant to, the mediaended event should fire, and set the jukebox.position back to 0, and starting the song again. NOT SURE WHETHER THIS IS ACTUALLY WORKING!!! :(
            if (isLooping)
            {

                Uri uriStreaming = new Uri(@"./Resources/" + trackLocation, UriKind.Relative);

                CalculateAudioVolume();                
                jukeBox.Open(uriStreaming);
                jukeBox.MediaEnded += new EventHandler(mediaElement_MediaEnded);
                jukeBox.Play();

            }
            else
            {
                Uri uriStreaming = new Uri(@"./Resources/" + trackLocation, UriKind.Relative);
                
                CalculateAudioVolume();
                jukeBox.Open(uriStreaming);
                jukeBox.Play();
            }
        }

        // This event fires when the main audio track stops - thus looping the music
        void mediaElement_MediaEnded(object sender, EventArgs e)
        {
            // Loops a particular track
            jukeBox.Position = TimeSpan.Zero;
            if(titleOrMain)
            {
                playMainMusic();
            }
            else
            {
                playWizardOne();
            }
            
        }

        /// <summary>
        /// Plays the Bomb Explode Sound Effect within the Audio Manager. Audio Manager is required to be instanced within each class where a track is to be played. Ex. AudioManager newAudioMan = new AudioManager(); ... newAudioMan.playMainMusic();
        /// </summary> 
        public void playBombExplode()
        {
            StopTrack();
            trackLocation = "bomb_explode.wav";
            newVolume = 0.4;
            PlayTrack();
        }

        /// <summary>
        /// Plays the Bomb Tick Sound Effect within the Audio Manager. Audio Manager is required to be instanced within each class where a track is to be played. Ex. AudioManager newAudioMan = new AudioManager(); ... newAudioMan.playMainMusic();
        /// </summary> 
        public void playBombTick()
        {
            StopTrack();
            trackLocation = "timer_fuse.wav";
            newVolume = 1.2;
            PlayTrack();
        }

        /// <summary>
        /// Plays the Life Pickup Sound Effect within the Audio Manager. Audio Manager is required to be instanced within each class where a track is to be played. Ex. AudioManager newAudioMan = new AudioManager(); ... newAudioMan.playMainMusic();
        /// </summary> 
        public void playPickupLife()
        {
            StopTrack();
            trackLocation = "pickup.wav";
            newVolume = 0.8;
            PlayTrack();
        }

        /// <summary>
        /// Plays the Bomb Pickup Sound Effect within the Audio Manager. Audio Manager is required to be instanced within each class where a track is to be played. Ex. AudioManager newAudioMan = new AudioManager(); ... newAudioMan.playMainMusic();
        /// </summary> 
        public void playPickupBomb()
        {
            StopTrack();
            trackLocation = "pickup.wav";
            newVolume = 0.8;
            PlayTrack();
        }

        /// <summary>
        /// Plays the Enemy Attack Sound Effect within the Audio Manager. Audio Manager is required to be instanced within each class where a track is to be played. Ex. AudioManager newAudioMan = new AudioManager(); ... newAudioMan.playMainMusic();
        /// </summary> 
        public void playEnemyAttack()
        {
            StopTrack();
            trackLocation = "break_wall.wav";
            newVolume = 2;
            PlayTrack();
        }

        /// <summary>
        /// Plays the Player Death Sound Effect within the Audio Manager. Audio Manager is required to be instanced within each class where a track is to be played. Ex. AudioManager newAudioMan = new AudioManager(); ... newAudioMan.playMainMusic();
        /// </summary> 
        public void playPlayerDeath()
        {
            StopTrack();
            trackLocation = "defeat.wav";
            newVolume = 0.5;
            PlayTrack();
        }

        /// <summary>
        /// Plays the Title Screen sound effect within the Audio Manager. Audio Manager is required to be instanced within each class where a track is to be played. Ex. AudioManager newAudioMan = new AudioManager(); ... newAudioMan.playMainMusic();
        /// </summary> 
        public void playTitleSound()
        {
            StopTrack();
            trackLocation = "titleTest2.wav";
            newVolume = 0.5;
            PlayTrack();

        }
    }
}
