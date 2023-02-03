using UnityEngine;
using VoxelWorld.World.Chuck;
using VoxelWorld.World.MeshRender;

namespace VoxelWorld.World.WorldGenerate
{
    [Utilities.ExecutionOrder.ExecuteAfter(typeof(World))]
    public static class MapGenerator
    {
        public static ChuckData GenerateFlatChuck(Vector2 pos, WorldGenerateRules rules)
        {
            var chuck = new ChuckData(pos);
            for (int k = 0; k < rules.blockDatas.Length; k++)
                for (int i = 0; i < VoxelData.ChuckWidth; i++)
                    for (int j = 0; j < VoxelData.ChuckWidth; j++)
                        chuck.blockData[i, k, j] = rules.blockDatas[k];
            return chuck;
        }
    }
}