using ICSharpCode.SharpZipLib.Zip;
using SaveDataSync.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace SaveDataSync
{
    internal class SaveDataSyncEngine
    {
        private static SaveDataSyncEngine Instance { get; } = new SaveDataSyncEngine();

        public LocalSaves LocalSaves { get; set; }
        public IServer Server { get; set; }
        public Settings Settings { get; set; }

        private DataManager DataManager { get; set; }

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

            Instance.Save(); // Prevents some possible errors

            return Instance;
        }

        public void AddSave(string name, string location)
        {
            LocalSaves.AddSave(name, location);
            Save();
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
                using var tmpFile = new FileUtils.TemporaryFile();
                using var tmpDir = new FileUtils.TemporaryFolder();
                var tempFile = tmpFile.FilePath;
                var tempDir = tmpDir.FolderPath;
                File.WriteAllBytes(tempFile, remoteZipData);
                FastZip fastZip = new FastZip();
                fastZip.ExtractZip(tempFile, tempDir, null);
                string[] content = Directory.GetFiles(tempDir, "*.*", SearchOption.TopDirectoryOnly);
                if (content.Length == 0) content = Directory.GetDirectories(tempDir, "*.*", SearchOption.TopDirectoryOnly);
                var saveContent = content[0]; // There should be only one output
                FileAttributes attr = File.GetAttributes(saveContent);
                if (attr.HasFlag(FileAttributes.Directory))
                {
                    // Handle as a dir
                    foreach (string dir in Directory.GetDirectories(saveContent, "*", SearchOption.AllDirectories))
                    {
                        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir.Replace(saveContent, saveLocation)); // Add all dirs
                    }

                    foreach (string file in Directory.GetFiles(saveContent, "*", SearchOption.AllDirectories))
                    {
                        File.Copy(file, file.Replace(saveContent, saveLocation), true); // Add all files
                    }
                }
                else
                {
                    // Handle as a file
                    File.Copy(saveContent, saveLocation, true);
                }

                success.Add(save);
            }

            return success.ToArray();
        }

        public void Save()
        {
            DataManager.SaveAll(LocalSaves, Server, Settings);
        }
    }
}