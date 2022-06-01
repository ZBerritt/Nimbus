using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace SaveDataSync
{
    public class LocalSaveList
    {
        private Dictionary<string, string> saveGameLocations = new Dictionary<string, string>();

        public LocalSaveList()
        { }

        public Dictionary<string, string> GetSaves()
        {
            return saveGameLocations;
        }

        public void AddSave(string name, string location)
        {
            if (saveGameLocations.ContainsKey(name)) throw new Exception("Save game with name " + name + " already exists.");
            var locations = saveGameLocations.Values;
            foreach (var loc in locations)
            {
                if (Path.GetFullPath(loc).Equals(Path.GetFullPath(location)))
                    throw new Exception("Save game with location " + location + " already exists.");
            }
            saveGameLocations[name] = location;
        }

        public void RemoveSave(string name)
        {
            if (!saveGameLocations.ContainsKey(name)) throw new Exception("Game list does not contain the save " + name);
            saveGameLocations.Remove(name);
        }

        public string GetSavePath(string name)
        {
            if (!saveGameLocations.ContainsKey(name)) throw new Exception("Save file with the name " + name + " does not exist.");
            return saveGameLocations[name];
        }

        // TODO: Store original permissions/date and put them back when extracting
        public byte[] GetSaveZipData(string name)
        {
            string location = GetSavePath(name);
            FileAttributes attr = File.GetAttributes(location);
            bool isDirectory = attr.HasFlag(FileAttributes.Directory);

            // This entire mess basically just zips the file into what we need. We need to do it manually isntead of using fastzip
            using (var tmpFile = new FileUtils.TemporaryFile())
            {
                using (ZipOutputStream OutputStream = new ZipOutputStream(File.Open(tmpFile.FilePath, FileMode.Open)))
                {
                    byte[] buffer = new byte[4096];
                    int perm = Convert.ToInt32("444", 8);

                    if (isDirectory)
                    {
                        string[] files = FileUtils.GetFileList(location);
                        foreach (string file in files)
                        {
                            string entryName = Path.Combine(name, file.Substring(location.Length + 1, file.Length - location.Length - 1));
                            ZipEntry entry = new ZipEntry(entryName);
                            entry.DateTime = DateTimeOffset.FromUnixTimeMilliseconds(0).DateTime; // Reset date time
                            entry.ExternalFileAttributes = (1 | perm) << 16; // Do something with the perms idk it works
                            OutputStream.PutNextEntry(entry);
                            using (FileStream fs = File.OpenRead(file))
                            {
                                int sourceBytes;

                                do
                                {
                                    sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                    OutputStream.Write(buffer, 0, sourceBytes);
                                } while (sourceBytes > 0);
                            }
                        }
                    }
                    else
                    {
                        ZipEntry entry = new ZipEntry(Path.GetFileName(location));
                        entry.DateTime = DateTimeOffset.FromUnixTimeMilliseconds(0).DateTime;
                        entry.ExternalFileAttributes = (1 | perm) << 16;
                        OutputStream.PutNextEntry(entry);
                        using (FileStream fs = File.OpenRead(location))
                        {
                            int sourceBytes;

                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                OutputStream.Write(buffer, 0, sourceBytes);
                            } while (sourceBytes > 0);
                        }
                    }
                    OutputStream.Finish();
                    OutputStream.Close();

                    // Read from temporary file
                    Console.WriteLine(tmpFile.FilePath);
                    return File.ReadAllBytes(tmpFile.FilePath);
                }
            }
        }

        public void WriteData(string name, byte[] data)
        {
            string location = GetSavePath(name);
            using (var tmpFile = new FileUtils.TemporaryFile())
            {
                File.WriteAllBytes(tmpFile.FilePath, data); // Write the data to a file to use the zip util functions
                using (ZipFile zipFile = new ZipFile(tmpFile.FilePath))
                {
                    foreach (ZipEntry entry in zipFile)
                    {
                        if (entry.IsFile)
                        {
                            Stream stream = zipFile.GetInputStream(entry);
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                string fileName = entry.Name.Replace("/", "\\"); // OCD lol
                                string pathName = Path.Combine(location, fileName);
                                using (StreamWriter writer = File.CreateText(pathName))
                                {
                                    writer.Write(reader.ReadToEnd());
                                }
                            }
                        }
                    }
                }
            }
        }

        public string ToJson()
        {
            JObject json = new JObject();
            JArray saves = new JArray();
            foreach (KeyValuePair<string, string> pair in saveGameLocations)
            {
                JObject saveObject = new JObject();
                saveObject.Add("name", pair.Key);
                saveObject.Add("location", pair.Value);
                saves.Add(saveObject);
            }
            json.Add("saves", saves);
            return json.ToString();
        }

        public static LocalSaveList FromJson(string json)
        {
            JObject deserializedJson = JsonConvert.DeserializeObject<JObject>(json);
            LocalSaveList list = new LocalSaveList();
            JArray saves = (JArray)deserializedJson.GetValue("saves");
            foreach (JObject save in saves)
            {
                string name = save.GetValue("name").ToString();
                string location = save.GetValue("location").ToString();
                list.AddSave(name, location);
            }

            return list;
        }
    }
}