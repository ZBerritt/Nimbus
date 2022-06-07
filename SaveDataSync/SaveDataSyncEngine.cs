using ICSharpCode.SharpZipLib.Zip;
using SaveDataSync.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace SaveDataSync
{
    public class SaveDataSyncEngine
    {
        public static SaveDataSyncEngine Instance { get; } = new SaveDataSyncEngine();

        private DataManager DataManager { get; set; }

        private LocalSaves _localsaves;
        private IServer _server;
        private Settings _settings;

        public LocalSaves LocalSaves
        {
            get => _localsaves;
            set
            {
                _localsaves = value;
                DataManager.SaveLocalSaves(_localsaves);
            }
        }

        public IServer Server
        {
            get => _server;
            set
            {
                _server = value;
                DataManager.SaveServerData(_server);
            }
        }

        public Settings Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                DataManager.SaveSettings(_settings);
            }
        }

        public static SaveDataSyncEngine Start()
        {
            return Start(Locations.DataDirectory);
        }

        public static SaveDataSyncEngine Start(string dataLocation)
        {
            Instance.DataManager = new DataManager(dataLocation);

            Instance.LocalSaves = Instance.DataManager.GetLocalSaves();
            Instance.Server = Instance.DataManager.GetServerData();
            Instance.Settings = Instance.DataManager.GetSettings();

            return Instance;
        }

        public void AddSave(string name, string location)
        {
            try
            {
                LocalSaves.AddSave(name, location);
            }
            catch (InvalidSaveException)
            {
                throw;
            }
            finally
            {
                SaveAllData();
            }
        }

        // Returns all files successfully exported
        // TODO: Refactor
        public string[] ExportSaves(string[] saves, ProgressBarControl progress)
        {
            var success = new List<string>();
            foreach (string save in saves)
            {
                progress.Increment($"Exporting {save}");

                // Remote saves should NEVER be called in this but it'll check anyways
                if (!LocalSaves.Saves.ContainsKey(save))
                {
                    throw new Exception("Remote files cannot be exported");
                }

                var saveLocation = LocalSaves.GetSavePath(save);
                if (!Directory.Exists(saveLocation) && !File.Exists(saveLocation))
                {
                    if (Array.IndexOf(saves, save) == saves.Length - 1) // Change message dialog on last
                    {
                        MessageBox.Show("Save file/folder does not exist for " + save + ".",
                            "Warning",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return success.ToArray();
                    }
                    else
                    {
                        var response = MessageBox.Show("Save file/folder does not exist for " + save + ". Would you like to continue exporting other files?",
                                "Warning",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning);
                        if (response == DialogResult.No) return success.ToArray(); // Abort on pressing no
                        continue;
                    }
                }

                var zipData = LocalSaves.GetSaveZipData(save);
                if (zipData == null || zipData.Length == 0)
                {
                    if (Array.IndexOf(saves, save) == saves.Length - 1) // Change message dialog on last
                    {
                        MessageBox.Show("Save folder is empty for " + save + ".",
                            "Warning",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return success.ToArray();
                    }
                    else
                    {
                        var response = MessageBox.Show("Save folder is empty for " + save + ". Would you like to continue exporting other files?",
                            "Warning",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning);
                        if (response == DialogResult.No) return success.ToArray(); // Abort on pressing no
                        continue;
                    }
                }
                Server.UploadSaveData(save, zipData);
                success.Add(save);
            }

            return success.ToArray();
        }

        // TODO: Refactor
        public string[] ImportSaves(string[] saves, ProgressBarControl progress)
        {
            var success = new List<string>();
            foreach (string save in saves)
            {
                progress.Increment($"Importing {save}");

                // If save is remote, prompt for a location
                if (!LocalSaves.Saves.ContainsKey(save))
                {
                    var prompt = new SaveFileWindow(Instance)
                    {
                        ShowIcon = true
                    };
                    string location = SaveFileWindow.ImportWindow(prompt, save);
                    if (location == "") throw new Exception("Import aborted by user!");
                }
                var saveLocation = LocalSaves.GetSavePath(save);

                if (!Directory.Exists(saveLocation) && !File.Exists(saveLocation))
                {
                    if (Array.IndexOf(saves, save) == saves.Length - 1) // Change message dialog on last
                    {
                        MessageBox.Show("Save file/folder does not exist for " + save + ".",
                            "Warning",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return success.ToArray();
                    }
                    else
                    {
                        var response = MessageBox.Show("Save file/folder does not exist for " + save + ". Would you like to continue importing other files?",
                                "Warning",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning);
                        if (response == DialogResult.No) return success.ToArray(); // Abort on pressing no
                        continue;
                    }
                }

                var remoteZipData = Server.GetSaveData(save);
                if (remoteZipData == null || remoteZipData.Length == 0)
                {
                    if (Array.IndexOf(saves, save) == saves.Length - 1) // Change message dialog on last
                    {
                        MessageBox.Show("No remote save data found for " + save + ".",
                            "Warning",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return success.ToArray();
                    }
                    else
                    {
                        var response = MessageBox.Show("No remote save data found for " + save + ". Would you like to continue importing other files?",
                            "Warning",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning);
                        if (response == DialogResult.No) return success.ToArray(); // Abort on pressing no
                        continue;
                    }
                }

                LocalSaves.ExtractSaveData(save, remoteZipData);
                success.Add(save);
            }

            return success.ToArray();
        }

        public void SaveAllData()
        {
            DataManager.SaveAll(_localsaves, _server, _settings);
        }
    }
}