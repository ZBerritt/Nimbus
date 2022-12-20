using SaveDataSync.UI;
using SaveDataSync.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SaveDataSync
{
    /// <summary>
    /// Manages operations between the saves and the server
    /// </summary>
    public class SaveManager
    {
        private readonly Server _server;
        private readonly LocalSaves _saves;
        public SaveManager(Server server, LocalSaves saves)
        {
            _server = server;
            _saves = saves;
        }

        /// <summary>
        /// Adds a save to the save list
        /// </summary>
        /// <param name="name">The name of the game save</param>
        /// <param name="location">The file/folder location</param>
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

        /// <summary>
        /// Exports selected save data to cloud server
        /// </summary>
        /// <param name="saves">The list of saves to export</param>
        /// <param name="progress">Control for the main window progress bar</param>
        /// <returns>An asynchronous task resulting in a list of the successfully exported saves</returns>
        public async Task<string[]> ExportSaves(string[] saves, ProgressBarControl progress)
        {
            var success = new List<string>();
            foreach (string save in saves)
            {
                progress.Increment($"Exporting {save}");

                // Remote saves should NEVER be called in this but it'll check anyways
                if (!_saves.Saves.ContainsKey(save))
                {
                    throw new Exception("Remote files cannot be exported"); // TODO: should NOT throw errors
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

        /// <summary>
        /// Imports selected save data from cloud server
        /// </summary>
        /// <param name="saves">The list of saves to impoty</param>
        /// <param name="progress">Control for the main window progress bar</param>
        /// <returns>An asynchronous task resulting in a list of the successfully import saves</returns>
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
                        PopupDialog.ErrorPopup("Could not retrieve remote save data for " + save + ".");
                        return success.ToArray();
                    }
                    else
                    {
                        var response = PopupDialog.ErrorPrompt("Could not retrieve remote save data for "
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
