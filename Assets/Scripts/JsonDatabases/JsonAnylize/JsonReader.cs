using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace VoxelWorld.JSONDatabases
{
    public static class JsonReader
    {
        public static List<T> ReadStaticDatasFromFolder<T>(string path)
        {
            TextAsset[] raw = Resources.LoadAll<TextAsset>(path);
            return ReadDatasFromText<T>(raw, true);
        }

        public static bool ReadDataFromPath<T>(string path, ref T output)
        {
            string raw = File.ReadAllText(Application.persistentDataPath + "/" + path);
            if (raw != null)
                return ReadDataFromText<T>(raw, ref output);
            return false;
        }

        public static bool ReadDataFromText<T>(string raw, ref T output)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<T>(raw);
                if (data != null)
                    output = data;
                return true;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return false;
            }
        }

        public static List<T> ReadDatasFromText<T>(string[] raw)
        {
            List<T> result = new();
            foreach (var file in raw)
            {
                try
                {
                    var data = JsonConvert.DeserializeObject<T>(file);
                    if (data != null)
                        result.Add(data);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
            return result;
        }

        public static List<T> ReadDatasFromText<T>(TextAsset[] raw, bool unloadAsset = false)
        {
            List<T> result = new();
            foreach (var file in raw)
            {
                try
                {
                    var data = JsonConvert.DeserializeObject<T>(file.text);
                    if (data != null)
                        result.Add(data);
                    if (unloadAsset)
                        Resources.UnloadAsset(file);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
            return result;
        }
    }
}