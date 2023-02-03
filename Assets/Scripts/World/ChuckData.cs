using Newtonsoft.Json;
using UnityEngine;
using VoxelWorld.Block;
using VoxelWorld.JSONDatabases.Converter;
using VoxelWorld.World.MeshRender;

namespace VoxelWorld.World.Chuck
{
    [JsonConverter(typeof(ChuckDataConverter))]
    public class ChuckData
    {
        public Vector2 position;

        public BlockData[,,] blockData = new BlockData[VoxelData.ChuckWidth,
            VoxelData.ChuckHeight, VoxelData.ChuckWidth];

        public ChuckMeshRender meshRender;

        public ChuckData()
        { }

        public ChuckData(Vector2 pos)
        {
            position = pos;
        }
    }
}