using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelWorld.JSONDatabases
{
    public static class JsonReader
    {
        public static List<T> ReadStaticDataFromFolder<T>(string path)
        {
            TextAsset[] raw = Resources.LoadAll<TextAsset>(path);
            List<T> result = new();
            foreach (var file in raw)
            {
                try
                {
                    var data = JsonConvert.DeserializeObject<T>(file.text);
                    if (data != null)
                        result.Add(data);
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