using SaveDataSync.UI;
using SaveDataSync.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SaveDataSync
{
    public class SaveManager
    {
        private Server _server;
        private LocalSaves _saves;
        public SaveManager(Server server, LocalSaves saves)
        {
            _server = server;
            _saves = saves;
        }

        public void AddSave(string name, string location)
        {
            try
            {
                _saves.AddSave(name, location);
            }
            catch (InvalidSaveException)
            {
                throw;
            }
        }

        public async Task<string[]> ExportSaves(string[] saves, ProgressBarControl progress)
        {
            var success = new List<string>();
            foreach (string save in saves)
            {
                progress.Increment($"Exporting {save}");

                // Remote saves should NEVER be called in this but it'll check anyways
                if (!_saves.Saves.ContainsKey(save))
                {
                    throw new Exception("Remote files cannot be exported");
                }

                var saveLocation = _saves.GetSavePath(save);
                if (!Directory.Exists(saveLocation) && !File.Exists(saveLocation))
                {
                    if (Array.IndexOf(saves, save) == saves.Length - 1) // Change message dialog on last
                    {
                        PopupDialog.WarningPopup("Save file/folder does not exist for " + save + ".");
                        return success.ToArray();
                    }
                    else
                    {
                        var response = PopupDialog.WarningPrompt("Save file/folder does not exist for "
                            + save + ". Would you like to continue exporting other files?");
                        if (response == DialogResult.No) return success.ToArray(); // Abort on pressing no
                        continue;
                    }
                }
                try
                {
                    using var tmpFile = new FileUtils.TemporaryFile();
                    await _saves.ArchiveSaveData(save, tmpFile.FilePath);
                    await _server.UploadSaveData(save, tmpFile.FilePath);
                    success.Add(save);
                }
                catch (Exception)
                {
                    if (Array.IndexOf(saves, save) == saves.Length - 1) // Change message dialog on last
                    {
                        PopupDialog.WarningPopup("Could not export save data for " + save + ".");
                        return success.ToArray();
                    }
                    else
                    {
                        var response = PopupDialog.WarningPrompt("Could not export save data for "
                            + save + ". Would you like to continue exporting other files?");
                        if (response == DialogResult.No) return success.ToArray(); // Abort on pressing no
                        continue;
                    }
                }
            }

            return success.ToArray();
        }
        public async Task<string[]> ImportSaves(string[] saves, ProgressBarControl progress)
        {

            var success = new List<string>();
            foreach (string save in saves)
            {
                progress.Increment($"Importing {save}");

                // If save is remote, prompt for a location
                if (!_saves.Saves.ContainsKey(save))
                {
                    var prompt = new SaveFileWindow(SaveDataSyncEngine.Instance)
                    {
                        ShowIcon = true
                    };
                    string location = SaveFileWindow.ImportWindow(prompt, save);
                    if (location == "") throw new Exception("Import aborted by user!");
                }
                var saveLocation = _saves.GetSavePath(save);

                if (!Directory.Exists(saveLocation) && !File.Exists(saveLocation))
                {
                    if (Array.IndexOf(saves, save) == saves.Length - 1) // Change message dialog on last
                    {
                        PopupDialog.WarningPopup("Save file/folder does not exist for " + save + ".");
                        return success.ToArray();
                    }
                    else
                    {
                        var response = PopupDialog.WarningPrompt("Save file/folder does not exist for "
                            + save + ". Would you like to continue importing other files?");
                        if (response == DialogResult.No) return success.ToArray(); // Abort on pressing no
                        continue;
                    }
                }

                try
                {
                    using var tmpFile = new FileUtils.TemporaryFile();
                    await _server.GetSaveData(save, tmpFile.FilePath);
                    await _saves.ExtractSaveData(save, tmpFile.FilePath);
                    success.Add(save);
                }
                catch (Exception)
                {
                    if (Array.IndexOf(saves, save) == saves.Length - 1) // Change message dialog on last
                    {
                        PopupDialog.WarningPopup("Could not retrieve remote save data for " + save + ".");
                        return success.ToArray();
                    }
                    else
                    {
                        var response = PopupDialog.WarningPrompt("Could not retrieve remote save data for "
                            + save + ". Would you like to continue importing other files?");
                        if (response == DialogResult.No) return success.ToArray(); // Abort on pressing no
                        continue;
                    }
                }
            }
            return success.ToArray();
        }
    }
}
