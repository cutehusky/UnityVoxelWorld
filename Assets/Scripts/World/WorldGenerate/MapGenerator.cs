using UnityEngine;
using VoxelWorld.World.Chuck;
using VoxelWorld.World.MeshRender;

namespace VoxelWorld.World.WorldGenerate
{
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

        public static ChuckData GenerateNoiseChuck(Vector2 pos, WorldGenerateRules rules, int seed)
        {
            var chuck = new ChuckData(pos);
            for (int i = 0; i < VoxelData.ChuckWidth; i++)
                for (int j = 0; j < VoxelData.ChuckWidth; j++)
                {
                    var terrainHeight = Mathf.FloorToInt(Noise.Get2DNoise(new Vector2(i + pos.x * VoxelData.ChuckWidth,
                        j + pos.y * VoxelData.ChuckWidth), 500, 0.08f) * VoxelData.ChuckHeight);
                    chuck.blockData[i, 0, j] = rules.blockDatas[0];

                    for (int k = 1; k <= terrainHeight; k++)
                        chuck.blockData[i, k, j] = rules.blockDatas[k * (rules.blockDatas.Length - 1) / terrainHeight];
                }
                    
            return chuck;
        }
    }
}