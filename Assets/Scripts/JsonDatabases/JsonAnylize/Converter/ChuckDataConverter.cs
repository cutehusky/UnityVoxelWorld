using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;
using VoxelWorld.Block;
using VoxelWorld.World.Chuck;
using VoxelWorld.World.MeshRender;

namespace VoxelWorld.JSONDatabases.Converter
{
    public class ChuckDataConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ChuckData);
        }

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType,
            object existingValue, JsonSerializer serializer)
        {
            ChuckData chuckData = new();
            var jobj = serializer.Deserialize<JObject>(reader);
            var pos = jobj.Value<JArray>("pos");
            var format = jobj.Value<string>("format_version");

            if (pos == null || format != "1.0.0")
                throw new FormatException("Invalid json format");
            chuckData.position = new Vector2(pos.Value<int>(0), pos.Value<int>(1));
            var blockData = jobj.Value<JArray>("block_data");
            if (blockData == null)
                throw new FormatException("Invalid json format");

            for (int i = 0; i < blockData.Count; i++)
            {
                pos = blockData[i].Value<JArray>("pos");
                if (pos == null)
                    continue;
                var block = new BlockData();
                block.ID = blockData[i].Value<int>("id");
                block.Data = blockData[i].Value<int>("data");

                var arr = blockData[i].Value<JArray>("list");
                if (arr != null)
                    for (int j = 0; j < arr.Count; j++)
                        block.List.Add(arr.Value<string>(j));

                var dic = blockData[i].Value<JObject>("tag");
                if (dic != null)
                    foreach (var item in dic)
                        block.Tag.Add(item.Key, dic[item.Key].Value<string>());

                chuckData.blockData[pos.Value<int>(0), pos.Value<int>(1), pos.Value<int>(2)] = block;
            }
            return chuckData;
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, JsonSerializer serializer)
        {
            var chuckData = value as ChuckData;

            var blockData = new JArray();
            for (int _i = 0; _i < VoxelData.ChuckWidth; _i++)
            {
                for (int _j = 0; _j < VoxelData.ChuckHeight; _j++)
                {
                    for (int _k = 0; _k < VoxelData.ChuckWidth; _k++)
                    {
                        if (chuckData.blockData[_i, _j, _k] == null)
                            continue;

                        var _data = new JObject
                        {
                            { "id", chuckData.blockData[_i, _j, _k].ID },
                            { "pos", new JArray { _i, _j,_k } },
                        };
                        if (chuckData.blockData[_i, _j, _k].Data != 0)
                            _data.Add("data", chuckData.blockData[_i, _j, _k].Data);

                        if (chuckData.blockData[_i, _j, _k].List.Count > 0)
                        {
                            var arr = new JArray();
                            for (int __i = 0; __i < chuckData.blockData[_i, _j, _k].List.Count; __i++)
                                arr.Add(chuckData.blockData[_i, _j, _k].List[__i]);
                            _data.Add("list", arr);
                        }

                        if (chuckData.blockData[_i, _j, _k].Tag.Count > 0)
                        {
                            var dic = new JObject();
                            foreach (var data in chuckData.blockData[_i, _j, _k].Tag)
                                dic.Add(data.Key, data.Value);
                            _data.Add("tag", dic);
                        }
                        blockData.Add(_data);
                    }
                }
            }

            var jobj = new JObject
                {
                    { "format_version", "1.0.0" },
                    { "pos", new JArray
                        {
                            Mathf.Round(chuckData.position.x),
                            Mathf.Round(chuckData.position.y)
                        }
                    },
                    { "block_data",  blockData}
                };
            var jsonstr = jobj.ToString();
            writer.WriteRaw(jsonstr);
        }
    }
}