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
        private readonly LocalSaveList _saves;
        public SaveManager(Server server, LocalSaveList saves)
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
            foreach (string saveName in saves)
            {
                progress.Increment($"Exporting {saveName}");

                // Remote saves should NEVER be called in this but it'll check anyways
                if (!_saves.HasSave(saveName))
                {
                    throw new Exception("Remote files cannot be exported"); // TODO: should NOT throw errors
                }

                if (!_saves.HasSave(saveName)) continue;
                var saveLocation = _saves.GetSave(saveName).Location;
                if (!Directory.Exists(saveLocation) && !File.Exists(saveLocation))
                {
                    if (Array.IndexOf(saves, saveName) == saves.Length - 1) // Change message dialog on last
                    {
                        PopupDialog.WarningPopup("Save file/folder does not exist for " + saveName + ".");
                        return success.ToArray();
                    }
                    else
                    {
                        var response = PopupDialog.WarningPrompt("Save file/folder does not exist for "
                            + saveName + ". Would you like to continue exporting other files?");
                        if (response == DialogResult.No) return success.ToArray(); // Abort on pressing no
                        continue;
                    }
                }
                try
                {
                    using var tmpFile = new FileUtils.TemporaryFile();
                    await _saves.ArchiveSaveData(saveName, tmpFile.FilePath);
                    await _server.UploadSaveData(saveName, tmpFile.FilePath);
                    success.Add(saveName);
                }
                catch (Exception)
                {
                    if (Array.IndexOf(saves, saveName) == saves.Length - 1) // Change message dialog on last
                    {
                        PopupDialog.WarningPopup("Could not export save data for " + saveName + ".");
                        return success.ToArray();
                    }
                    else
                    {
                        var response = PopupDialog.WarningPrompt("Could not export save data for "
                            + saveName + ". Would you like to continue exporting other files?");
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
            foreach (string saveName in saves)
            {
                progress.Increment($"Importing {saveName}");

                // If save is remote, prompt for a location
                if (!_saves.HasSave(saveName))
                {
                    var prompt = new SaveFileWindow(SaveDataSyncEngine.Instance)
                    {
                        ShowIcon = true
                    };
                    string location = SaveFileWindow.ImportWindow(prompt, saveName);
                    if (location == "") throw new Exception("Import aborted by user!");
                }
                if (!_saves.HasSave(saveName)) continue;
                var saveLocation = _saves.GetSave(saveName).Location;

                if (!Directory.Exists(saveLocation) && !File.Exists(saveLocation))
                {
                    if (Array.IndexOf(saves, saveName) == saves.Length - 1) // Change message dialog on last
                    {
                        PopupDialog.WarningPopup("Save file/folder does not exist for " + saveName + ".");
                        return success.ToArray();
                    }
                    else
                    {
                        var response = PopupDialog.WarningPrompt("Save file/folder does not exist for "
                            + saveName + ". Would you like to continue importing other files?");
                        if (response == DialogResult.No) return success.ToArray(); // Abort on pressing no
                        continue;
                    }
                }

                try
                {
                    using var tmpFile = new FileUtils.TemporaryFile();
                    await _server.GetSaveData(saveName, tmpFile.FilePath);
                    await _saves.ExtractSaveData(saveName, tmpFile.FilePath);
                    success.Add(saveName);
                }
                catch (Exception)
                {
                    if (Array.IndexOf(saves, saveName) == saves.Length - 1) // Change message dialog on last
                    {
                        PopupDialog.ErrorPopup("Could not retrieve remote save data for " + saveName + ".");
                        return success.ToArray();
                    }
                    else
                    {
                        var response = PopupDialog.ErrorPrompt("Could not retrieve remote save data for "
                            + saveName + ". Would you like to continue importing other files?");
                        if (response == DialogResult.No) return success.ToArray(); // Abort on pressing no
                        continue;
                    }
                }
            }
            return success.ToArray();
        }
    }
}
