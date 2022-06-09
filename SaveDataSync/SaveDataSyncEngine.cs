using SaveDataSync.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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

        // TODO: This stuff doesn't use await. No clue how to fix...
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

        public async Task AddSave(string name, string location)
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
                await SaveAllData();
            }
        }

        // Returns all files successfully exported
        // TODO: Refactor
        public async Task<string[]> ExportSaves(string[] saves, ProgressBarControl progress)
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
                try
                {
                    using var tmpFile = new FileUtils.TemporaryFile();
                    await LocalSaves.ArchiveSaveData(save, tmpFile.FilePath);
                    await Server.UploadSaveData(save, tmpFile.FilePath);
                    success.Add(save);
                }
                catch (Exception)
                {
                    if (Array.IndexOf(saves, save) == saves.Length - 1) // Change message dialog on last
                    {
                        MessageBox.Show("Could not export save data for " + save + ".",
                            "Warning",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return success.ToArray();
                    }
                    else
                    {
                        var response = MessageBox.Show("Could not export save data for " + save + ". Would you like to continue exporting other files?",
                            "Warning",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning);
                        if (response == DialogResult.No) return success.ToArray(); // Abort on pressing no
                        continue;
                    }
                }
            }

            return success.ToArray();
        }

        // TODO: Refactor
        public async Task<string[]> ImportSaves(string[] saves, ProgressBarControl progress)
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

                try
                {
                    using var tmpFile = new FileUtils.TemporaryFile();
                    await Server.GetSaveData(save, tmpFile.FilePath);
                    await LocalSaves.ExtractSaveData(save, tmpFile.FilePath);
                    success.Add(save);
                }
                catch (Exception)
                {
                    if (Array.IndexOf(saves, save) == saves.Length - 1) // Change message dialog on last
                    {
                        MessageBox.Show("Could not retrieve remote save data for " + save + ".",
                            "Warning",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return success.ToArray();
                    }
                    else
                    {
                        var response = MessageBox.Show("Could not retrieve remote save data for " + save + ". Would you like to continue importing other files?",
                            "Warning",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning);
                        if (response == DialogResult.No) return success.ToArray(); // Abort on pressing no
                        continue;
                    }
                }
            }
            return success.ToArray();
        }

        public async Task<string> GetLocalHash(string save)
        {
            using var tmpFile = new FileUtils.TemporaryFile();
            await LocalSaves.ArchiveSaveData(save, tmpFile.FilePath);
            var saveHash = Server.GetLocalSaveHash(tmpFile.FilePath);
            return await saveHash;
        }

        public async Task<string> GetRemoteHash(string save)
        {
            return await Server.GetRemoteSaveHash(save);
        }

        public async Task SaveAllData()
        {
            await DataManager.SaveAll(_localsaves, _server, _settings);
        }
    }
}