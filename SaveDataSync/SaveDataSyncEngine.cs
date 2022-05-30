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
        private static SaveDataSyncEngine instance;
        private LocalSaveList localSaveList;
        private Server server;
        private Settings settings;

        public static SaveDataSyncEngine CreateInstance()
        {
            if (instance != null) throw new Exception("Engine instance already exists!");
            instance = new SaveDataSyncEngine();
            return instance;
        }

        private SaveDataSyncEngine()
        {
            localSaveList = DataManagement.GetLocalSaveList();
            server = DataManagement.GetServerData();
            settings = DataManagement.GetSettings();
        }

        public LocalSaveList GetLocalSaveList()
        {
            return localSaveList;
        }

        public Settings GetSettings()
        {
            return settings;
        }

        public void SetSettings(Settings settings)
        {
            this.settings = settings;
            Save();
        }

        public Server GetServer()
        {
            return server;
        }

        public void SetServer(Server server)
        {
            this.server = server;
            Save();
        }

        // Button actions
        public void CreateSaveFile(string name, string location)
        {
            localSaveList.AddSave(name, location);
            Save();
        }

        // Returns all files successfully exported
        public string[] ExportSaves(string[] saves, ProgressBarControl progress)
        {
            var success = new List<string>();
            foreach (string save in saves)
            {
                progress.Increment("Exporting '" + save + "'");
                // Remote saves should NEVER be called in this but it'll check anyways
                if (!new List<string>(localSaveList.GetSaves().Keys).Contains(save))
                {
                    throw new Exception("Remote files cannot be exported");
                }

                var saveLocation = localSaveList.GetSavePath(save);
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
                        var response = MessageBox.Show("Save file/folder does not exist for " + save + ". Would you like to continue anyways?",
                                "Warning",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning);
                        if (response == DialogResult.No) return success.ToArray(); // Abort on pressing no
                        continue;
                    }
                }

                var zipData = localSaveList.GetSaveZipData(save);
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
                        var response = MessageBox.Show("Save folder is empty for " + save + ". Would you like to continue anyways?",
                            "Warning",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning);
                        if (response == DialogResult.No) return success.ToArray(); // Abort on pressing no
                        continue;
                    }
                }
                server.UploadSaveData(save, zipData);
                success.Add(save);
            }

            return success.ToArray();
        }

        public string[] ImportSaves(string[] saves, ProgressBarControl progress)
        {
            var success = new List<string>();
            foreach (string save in saves)
            {
                progress.Increment("Importing '" + save + "'");
                // If save is remote, prompt for a location
                if (!new List<string>(localSaveList.GetSaves().Keys).Contains(save))
                {
                    SaveFileWindow prompt = new SaveFileWindow(instance)
                    {
                        ShowIcon = true
                    };
                    string location = SaveFileWindow.ImportWindow(prompt, save);
                    if (location == "") throw new Exception("Import aborted by user!");
                }

                var saveLocation = localSaveList.GetSavePath(save);
                var remoteZipData = server.GetSaveData(save);
                using (var tmpFile = new FileUtils.TemporaryFile())
                using (var tmpDir = new FileUtils.TemporaryFolder())
                {
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
            }

            return success.ToArray();
        }

        public void Save()
        {
            DataManagement.SaveLocalSaveList(localSaveList);
            DataManagement.SaveServerData(server);
            DataManagement.SaveSettings(settings);
        }
    }
}