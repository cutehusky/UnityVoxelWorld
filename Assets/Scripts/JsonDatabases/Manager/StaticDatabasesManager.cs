using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.Block;

namespace VoxelWorld.JSONDatabases.Manager
{
    public class StaticDatabasesManager : MonoBehaviour
    {
        public List<BlockType> BlockList;

        private void Awake()
        {
            BlockList = JsonReader.ReadStaticDataFromFolder<BlockType>("Databases/Blocks");

        }
    }
}