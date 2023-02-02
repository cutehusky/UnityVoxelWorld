using UnityEngine;

namespace VoxelWorld.World.MeshRender
{
    public static class VoxelData
    {
        public static readonly int ChuckWidth = 16;
        public static readonly int ChuckHeight = 255;

        public static readonly Vector3[] voxelVerts = new Vector3[8] {
            new Vector3(0,0,0),
            new Vector3(1f,0,0),
            new Vector3(1f,0,1f),
            new Vector3(0,0,1f),
            new Vector3(0,1f,0),
            new Vector3(1f,1f,0),
            new Vector3(1f,1,1f),
            new Vector3(0,1f,1f)
        };

        public static readonly Vector3[] facecheck = new Vector3[6]
        {
            new Vector3(0,1,0),
            new Vector3(0,-1,0),
            new Vector3(0,0,-1),
            new Vector3(0,0,1),
            new Vector3(1,0,0),
            new Vector3(-1,0,0)
        };

        public static readonly Vector2[] voxelUVs = new Vector2[4] {
            new Vector2(0,0),
            new Vector2(0,1f),
            new Vector2(1f,1f),
            new Vector2(1f,0)
        };

        public static readonly int[,] voxelTris = new int[6, 4] {
            {4,7,6,5 }, //top
            {3,0,1,2 }, //bottom
            {0,4,5,1 }, //back
            {2,6,7,3 }, //front
            {1,5,6,2 }, //right
            {3,7,4,0 }  //left
        };
    }
}