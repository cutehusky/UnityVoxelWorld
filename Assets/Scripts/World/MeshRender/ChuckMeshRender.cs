using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.Block;
using VoxelWorld.Texture;

namespace VoxelWorld.World.MeshRender
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [Utilities.ExecutionOrder.ExecuteAfter(typeof(BlockTextureBuilder))]
    public class ChuckMeshRender : MonoBehaviour
    {
        [HideInInspector] public MeshRenderer meshRenderer;
        [HideInInspector] public MeshFilter meshFilter;
        public BlockTextureBuilder textureBuilder;

        private int vertexIndex;

        private List<Vector3> verts;
        private List<int>[] tris;
        private List<Vector2> uvs;

        public BlockData[,,] ChuckData = new BlockData[VoxelData.ChuckWidth, VoxelData.ChuckHeight, VoxelData.ChuckWidth];

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.materials = textureBuilder.blockMaterial.ToArray();
            InitialMeshData();
        }

        private void InitialMeshData()
        {
            meshFilter.mesh.Clear();
            vertexIndex = 0;
            verts = new();
            uvs = new();
            tris = new List<int>[textureBuilder.blockMaterial.Count];
            for (int i = 0; i < textureBuilder.blockMaterial.Count; i++)
            {
                tris[i] = new();
            }
        }

        private void Start()
        {
            var block = new BlockData();
            block.ID = 1;
            var block2 = new BlockData();
            block2.ID = 0;
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    for (int k = 0; k < 16; k++)
                    {
                        ChuckData[i, k, j] = block2;
                    }
                }
            }
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    for (int k = 0; k < 5; k++)
                    {
                        ChuckData[i, k, j] = block;
                    }
                }
            }

            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    for (int k = 0; k < 16; k++)
                    {
                        AddPos(new Vector3(i, k, j));
                    }
                }
            }
            BuildMesh();
        }

        private bool Checkface(Vector3 pos)
        {
            int x = Mathf.RoundToInt(pos.x),
                y = Mathf.RoundToInt(pos.y),
                z = Mathf.RoundToInt(pos.z);

            if (x < 0 || x > VoxelData.ChuckWidth - 1
                || z < 0 || z > VoxelData.ChuckWidth - 1
                || y < 0 || y > VoxelData.ChuckHeight - 1)
                return false;

            return ChuckData[x, y, z].ID != 0;
        }

        private void AddPos(Vector3 pos)
        {
            int blockID = ChuckData[Mathf.RoundToInt(pos.x),
                 Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z)].ID;
            if (blockID == 0)
                return;

            for (int i = 0; i < 6; i++)
            {
                if (!Checkface(pos + VoxelData.facecheck[i]))
                {
                    var uvInfo = textureBuilder.GetBlockUVInfo(blockID, i);
                    verts.Add(VoxelData.voxelVerts[VoxelData.voxelTris[i, 0]] + pos);
                    verts.Add(VoxelData.voxelVerts[VoxelData.voxelTris[i, 1]] + pos);
                    verts.Add(VoxelData.voxelVerts[VoxelData.voxelTris[i, 2]] + pos);
                    verts.Add(VoxelData.voxelVerts[VoxelData.voxelTris[i, 3]] + pos);
                    uvs.Add(VoxelData.voxelUVs[0] * textureBuilder.uvScale + uvInfo.UVOffset);
                    uvs.Add(VoxelData.voxelUVs[1] * textureBuilder.uvScale + uvInfo.UVOffset);
                    uvs.Add(VoxelData.voxelUVs[2] * textureBuilder.uvScale + uvInfo.UVOffset);
                    uvs.Add(VoxelData.voxelUVs[3] * textureBuilder.uvScale + uvInfo.UVOffset);
                    tris[uvInfo.MaterialIndex].Add(vertexIndex);
                    tris[uvInfo.MaterialIndex].Add(vertexIndex + 1);
                    tris[uvInfo.MaterialIndex].Add(vertexIndex + 2);
                    tris[uvInfo.MaterialIndex].Add(vertexIndex + 2);
                    tris[uvInfo.MaterialIndex].Add(vertexIndex + 3);
                    tris[uvInfo.MaterialIndex].Add(vertexIndex);
                    vertexIndex += 4;
                }
            }
        }

        private void BuildMesh()
        {
            Mesh mesh = new()
            {
                subMeshCount = textureBuilder.blockMaterial.Count,
                vertices = verts.ToArray(),
                uv = uvs.ToArray()
            };

            for (int i = 0; i < textureBuilder.blockMaterial.Count; i++)
            {
                mesh.SetTriangles(tris[i].ToArray(), i);
                mesh.RecalculateNormals();
            }

            meshFilter.mesh = mesh;
        }
    }
}