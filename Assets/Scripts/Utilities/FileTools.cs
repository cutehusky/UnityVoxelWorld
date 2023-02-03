using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;
using VoxelWorld.World;

namespace VoxelWorld.Utilities
{
    public static class FileTools
    {
        public static void CreateFolder(string path)
        {
            path = Application.persistentDataPath + "/" + path;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static void CreateWorld(string path, ref WorldData worldData)
        {
            path = Application.persistentDataPath + "/" + path;
            worldData.uid = Guid.NewGuid().ToString();
            while (true)
            {
                if (!Directory.Exists(path + "/" + worldData.uid))
                {
                    Directory.CreateDirectory(path + "/" + worldData.uid);
                    File.WriteAllText(path + "/" + worldData.uid + "/WorldInfo.json",
                        JsonConvert.SerializeObject(worldData), System.Text.Encoding.UTF8);
                    Directory.CreateDirectory(path + "/" + worldData.uid + "/Chucks");
                    break;
                }
                worldData.uid = Guid.NewGuid().ToString();
            }
        }

        public static bool FindWorld(string path, string uid, ref WorldData worldData,
            bool ignoreError = false)
        {
            path = Application.persistentDataPath + "/" + path;
            if (!Directory.Exists(path))
                return false;

            var paths = Directory.GetDirectories(path);
            for (int i = 0; i < path.Length; i++)
            {
                if (File.Exists(paths + "/WorldInfo.json"))
                {
                    try
                    {
                        string raw = File.ReadAllText(paths + "/WorldInfo.json");
                        var data = JsonConvert.DeserializeObject<WorldData>(raw);
                        if (data != null)
                        {
                            if (data.uid == uid)
                            {
                                worldData = data;
                                return true;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        if (!ignoreError)
                            Debug.Log(e);
                    }
                }
            }
            return false;
        }
    }
}