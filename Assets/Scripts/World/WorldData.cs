using Newtonsoft.Json;
using VoxelWorld.JSONDatabases.Converter;

namespace VoxelWorld.World
{
    public enum WORLDTYPE
    {
        NORMAL,
        FLAT
    }

    [JsonConverter(typeof(WorldDataConverter))]
    public class WorldData
    {
        public string name;
        public string uid;
        public WORLDTYPE worldType;
    }
}