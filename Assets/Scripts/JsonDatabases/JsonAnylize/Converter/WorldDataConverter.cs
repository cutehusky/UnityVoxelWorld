using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using VoxelWorld.World;

namespace VoxelWorld.JSONDatabases.Converter
{
    public class WorldDataConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(WorldData);
        }

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            WorldData worldData = new();
            var jobj = serializer.Deserialize<JObject>(reader);
            var format = jobj.Value<string>("format_version");
            worldData.uid = jobj.Value<string>("uid");
            worldData.name = jobj.Value<string>("name");
            worldData.worldType = (WORLDTYPE)jobj.Value<int>("world_type");
            if (worldData.uid == null || worldData.name == null || format != "1.0.0")
                throw new FormatException("Invalid json format");
            return worldData;
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, JsonSerializer serializer)
        {
            var jobj = new JObject();
            var worldData = value as WorldData;
            jobj.Add("format_version", "1.0.0");
            jobj.Add("name", worldData.name);
            jobj.Add("uid", worldData.uid);
            jobj.Add("world_type", (int)worldData.worldType);
            var jsonstr = jobj.ToString();
            writer.WriteRaw(jsonstr);
        }
    }
}