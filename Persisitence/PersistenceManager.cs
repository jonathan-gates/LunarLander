using CS5410;
using LunarLander.Persistence;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using LunarLander.Persisitence;

namespace LunarLander
{
    class PersistenceManager
    {
        private bool saving = false;
        private bool loading = false;
        public ControlsPersistence m_controlsPersistence { get; private set; }
        public ScoresPersistence m_scoresPersistence { get; private set; }

        public void saveControls(Keys thrust, Keys rotateLeft, Keys rotateRight)
        {
            lock (this)
            {
                if (!this.saving)
                {
                    this.saving = true;
                    ControlsPersistence myState = new ControlsPersistence(thrust, rotateLeft, rotateRight);

                    finalizeSaveControlsAsync(myState);
                }
            }
        }

        private async Task finalizeSaveControlsAsync(ControlsPersistence state)
        {
            await Task.Run(() =>
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        using (IsolatedStorageFileStream fs = storage.OpenFile("Controls.json", FileMode.Create))
                        {
                            if (fs != null)
                            {
                                DataContractJsonSerializer mySerializer = new DataContractJsonSerializer(typeof(ControlsPersistence));
                                mySerializer.WriteObject(fs, state);
                            }
                        }
                    }
                    catch (IsolatedStorageException)
                    {
                        
                    }
                }

                this.saving = false;
            });
        }

        public void loadControls() 
        {
            lock (this)
            {
                if (!this.loading)
                {
                    this.loading = true;
                    // Yes, I know the result is not being saved, I dont' need it
                    var result = finalizeLoadControlsAsync().ContinueWith(task =>
                    {
                        if (m_controlsPersistence == null) // If loading didn't set up controls, set defaults
                        {
                            // Specify your default keys here
                            saveControls(Keys.Up, Keys.Left, Keys.Right); // Save the default controls
                        }
                    }); 
                    result.Wait();

                }
            }
        }

        private async Task finalizeLoadControlsAsync()
        {
            await Task.Run(() =>
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        if (storage.FileExists("Controls.json"))
                        {
                            using (IsolatedStorageFileStream fs = storage.OpenFile("Controls.json", FileMode.Open))
                            {
                                if (fs != null)
                                {
                                    DataContractJsonSerializer mySerializer = new DataContractJsonSerializer(typeof(ControlsPersistence));
                                    m_controlsPersistence = (ControlsPersistence)mySerializer.ReadObject(fs);
                                }
                            }
                        }
                    }
                    catch (IsolatedStorageException)
                    {
                        
                    }
                }

                this.loading = false;
            });
        }

        public void saveScore(List<float> scores, float newScore)
        {
            lock (this)
            {
                if (!this.saving)
                {
                    this.saving = true;
                    ScoresPersistence myState = new ScoresPersistence(scores, newScore);

                    finalizeSaveScoresAsync(myState);
                }
            }
        }

        private async Task finalizeSaveScoresAsync(ScoresPersistence state)
        {
            await Task.Run(() =>
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        using (IsolatedStorageFileStream fs = storage.OpenFile("HighScores.json", FileMode.Create))
                        {
                            if (fs != null)
                            {
                                DataContractJsonSerializer mySerializer = new DataContractJsonSerializer(typeof(ScoresPersistence));
                                mySerializer.WriteObject(fs, state);
                            }
                        }
                    }
                    catch (IsolatedStorageException)
                    {

                    }
                }

                this.saving = false;
            });
        }

        public void loadHighScores() 
        {
            lock (this)
            {
                if (!this.loading)
                {
                    this.loading = true;
                    // Yes, I know the result is not being saved, I dont' need it
                    var result = finalizeLoadScoresAsync();
                    result.Wait();

                }
            }
        }

        private async Task finalizeLoadScoresAsync()
        {
            await Task.Run(() =>
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        if (storage.FileExists("HighScores.json"))
                        {
                            using (IsolatedStorageFileStream fs = storage.OpenFile("HighScores.json", FileMode.Open))
                            {
                                if (fs != null)
                                {
                                    DataContractJsonSerializer mySerializer = new DataContractJsonSerializer(typeof(ScoresPersistence));
                                    m_scoresPersistence = (ScoresPersistence)mySerializer.ReadObject(fs);
                                }
                            }
                        }
                    }
                    catch (IsolatedStorageException)
                    {
                    }
                }

                this.loading = false;
            });
        }
    }
}
