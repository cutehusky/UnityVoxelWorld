using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.Utilities;
using VoxelWorld.World.Chuck;
using VoxelWorld.World.WorldGenerate;

namespace VoxelWorld.JSONDatabases.Manager
{
    [Utilities.ExecutionOrder.ExecuteAfter(typeof(StaticDatabasesManager))]
    public class RuntimeDatabasesManager : MonoBehaviour
    {
        public WorldGenerateRules rules;
        public Dictionary<string, ChuckData> chuckDatas;

        private void InitialFolder()
        {
            FileTools.CreateFolder("Worlds");
        }

        private void Awake()
        {
            InitialFolder();
            chuckDatas = new();
        }

        public void LoadWorldGenerateRules(string name)
        {
            var raw = Resources.Load<TextAsset>("Databases/Rules/WorldGenerate/" + name);
            rules = JsonConvert.DeserializeObject<WorldGenerateRules>(raw.text);
        }

        public void SaveChuck(string name, string path)
        {
            JsonWriter.WriteData(path + "/Chucks/Chuck_" + name + ".json", chuckDatas[name], true, true);
        }

        public bool LoadChuck(Vector2 pos, string path)
        {
            ChuckData data = new();
            if (JsonReader.ReadDataFromPath(path + "/Chucks/Chuck_" + pos.x + "_" + pos.y + ".json", ref data, true))
            {
                chuckDatas.Add(pos.x + "_" + pos.y, data);
                return true;
            }
            return false;
        }
    }
}