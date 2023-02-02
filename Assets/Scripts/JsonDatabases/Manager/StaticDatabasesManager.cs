using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.Block;
using VoxelWorld.Utilities;
using VoxelWorld.World;
using VoxelWorld.World.Chuck;

namespace VoxelWorld.JSONDatabases.Manager
{
    [Utilities.ExecutionOrder.ExecuteAfter(typeof(WorldManager))]
    public class StaticDatabasesManager : MonoBehaviour
    {
        public List<BlockType> BlockList;
        public List<ChuckData> chuckDatas;

        private void Awake()
        {
            FileTools.CreateFolder(Application.persistentDataPath + "/ResourcePacks");

            BlockList = JsonReader.ReadStaticDatasFromFolder<BlockType>("Databases/Blocks");
        }
    }
}