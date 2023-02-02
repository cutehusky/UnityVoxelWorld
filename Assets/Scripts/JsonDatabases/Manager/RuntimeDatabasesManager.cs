using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.Utilities;
using VoxelWorld.World;
using VoxelWorld.World.Chuck;
using VoxelWorld.World.WorldGenerate;

namespace VoxelWorld.JSONDatabases.Manager
{
    [Utilities.ExecutionOrder.ExecuteAfter(typeof(StaticDatabasesManager))]
    public class RuntimeDatabasesManager : MonoBehaviour
    {
        public WorldGenerateRules rules;
        public List<ChuckData> chuckDatas;


        private void InitialFolder()
        {
            FileTools.CreateFolder(Application.persistentDataPath + "/Worlds");
        }

        private void Awake()
        {
            InitialFolder();
        }

        public void LoadWorldGenerateRules(string name)
        {
            var raw = Resources.Load<TextAsset>("Databases/Rules/WorldGenerate/" + name);
            rules = JsonConvert.DeserializeObject<WorldGenerateRules>(raw.text);
        }

        public void SaveChuck(Vector2 pos)
        {

        }

        public bool LoadChuck(Vector2 pos)
        {
            return true;
        }

        private void Update()
        {
            
        }
    }
}