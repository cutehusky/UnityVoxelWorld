using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using UnityEngine;

namespace VoxelWorld.JSONDatabases
{
    public static class JsonWriter
    {
        public static bool WriteData<T>(string path, T data, bool _override = true, 
            bool ignoreError = false)
        {
            path = Application.persistentDataPath + "/" + path;
            if (Directory.Exists(path))
                return false;

            if (Path.GetExtension(path) != ".json")
                path += ".json";

            if (File.Exists(path) && !_override)
            {
                try
                {
                    string raw = File.ReadAllText(path);
                    JObject o1 = JObject.Parse(raw);
                    JObject o2 = JObject.Parse(JsonConvert.SerializeObject(data));
                    o1.Merge(o2, new JsonMergeSettings
                    {
                        MergeArrayHandling = MergeArrayHandling.Union
                    });
                    File.WriteAllText(path, o1.ToString());
                    return true;
                }
                catch (Exception e)
                {
                    if (!ignoreError)
                        Debug.Log(e);
                    return false;
                }
            }

            try
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(data));
                return true;
            }
            catch (Exception e)
            {
                if (!ignoreError)
                    Debug.Log(e);
                return false;
            }
        }
    }
}