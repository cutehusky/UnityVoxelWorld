using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.Block;
using VoxelWorld.JSONDatabases.Converter;

namespace VoxelWorld.World.WorldGenerate
{
    [JsonConverter(typeof(WorldGenerateRulesConverter))]
    public class WorldGenerateRules
    {
        public WORLDTYPE type;
        public BlockData[] blockDatas;
    }
}