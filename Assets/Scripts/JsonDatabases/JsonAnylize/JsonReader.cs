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
            if (raw != null)
                return ReadDatasFromText<T>(raw, true);
            return null;
        }

        public static bool ReadDataFromPath<T>(string path, ref T output, bool ignoreError = false)
        {
            try
            {
                string raw = File.ReadAllText(Application.persistentDataPath + "/" + path);
                if (raw == null)
                    return false;
                var data = JsonConvert.DeserializeObject<T>(raw);
                if (data != null)
                {
                    output = data;
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                if (!ignoreError) 
                    Debug.Log(e);
                return false;
            }
        }

        public static List<T> ReadDatasFromText<T>(string[] raw, bool ignoreError = false)
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
                    if (!ignoreError)
                        Debug.Log(e);
                }
            }
            return result;
        }

        public static List<T> ReadDatasFromText<T>(TextAsset[] raw, bool unloadAsset = false, bool ignoreError = false)
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
                    if (!ignoreError)
                        Debug.Log(e);
                }
            }
            return result;
        }
    }
}