using Newtonsoft.Json;
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