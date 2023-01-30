using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;
using VoxelWorld.Block;

namespace VoxelWorld.JSONDatabases.Converter
{
    public class BlockTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(BlockType);
        }

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jobj = serializer.Deserialize<JObject>(reader);
            var format = jobj.Value<string>("format_version");
            var stringID = jobj.Value<string>("string_ID");
            var name = jobj.Value<string>("name");
            var id = jobj.Value<int>("id");
            var texturePath = jobj.Value<string>("texture");
            var materialPath = jobj.Value<string>("material");

            if (format != "1.0.0" || id == 0 || stringID == null || texturePath == null ||
                name == null || materialPath == null)
                throw new FormatException("Invalid json format");

            return new BlockType
            {
                name = name,
                ID = id,
                stringID = stringID,
                texturePath = texturePath,
                materialPath = materialPath
            };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new FormatException("Can't writte json");
        }
    }
}