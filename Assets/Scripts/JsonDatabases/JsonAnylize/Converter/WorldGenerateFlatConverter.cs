using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using VoxelWorld.Block;
using VoxelWorld.World;
using VoxelWorld.World.WorldGenerate;

namespace VoxelWorld.JSONDatabases.Converter
{
    public class WorldGenerateRulesConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(WorldGenerateRules) == objectType;
        }

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jobj = serializer.Deserialize<JObject>(reader);
            var rules = new WorldGenerateRules();
            var jarr = jobj.Value<JArray>("rules");
            var type = jobj.Value<int>("type");
            var format = jobj.Value<string>("format_version");

            if (jarr == null || format != "1.0.0")
                throw new FormatException("Invalid json format");

            rules.blockDatas = new BlockData[jarr.Count];
            rules.type = (WORLDTYPE)type;
            for (int i = 0; i < jarr.Count; i++)
            {
                var block = new BlockData();
                var blockData = jarr.Value<JObject>(i);
                block.ID = blockData.Value<int>("id");
                block.Data = blockData.Value<int>("data");

                var arr = blockData.Value<JArray>("list");
                if (arr != null)
                    for (int j = 0; j < arr.Count; j++)
                        block.List.Add(arr.Value<string>(j));

                var dic = blockData.Value<JObject>("tag");
                if (dic != null)
                    foreach (var item in dic)
                        block.Tag.Add(item.Key, dic[item.Key].Value<string>());
                rules.blockDatas[i] = block;
            }
            return rules;
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Can't writte json");
        }
    }
}