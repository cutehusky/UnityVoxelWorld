using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.World.MeshRender;

namespace VoxelWorld.World.WorldGenerate
{
    public static class Noise
    {
        public static float Get2DNoise(Vector2 pos,float offset, float scale)
        {
            return Mathf.PerlinNoise((pos.x + 0.1f) / VoxelData.ChuckWidth * scale + offset,
                (pos.y + 0.1f) / VoxelData.ChuckWidth * scale + offset);
        }
    }
}