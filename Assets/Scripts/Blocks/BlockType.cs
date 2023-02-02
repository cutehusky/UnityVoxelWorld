using Newtonsoft.Json;
using VoxelWorld.JSONDatabases.Converter;
using VoxelWorld.Texture;

namespace VoxelWorld.Block
{
    [JsonConverter(typeof(BlockTypeConverter))]
    public class BlockType
    {
        public string name = "";
        public int ID = 0;
        public string stringID = "";
        public string texturePath = "";
        public string materialPath = "";
        public BlockUVInfo[] uvInfo;
    }
}