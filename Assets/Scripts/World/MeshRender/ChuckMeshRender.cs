using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.Block;
using VoxelWorld.Texture;
using VoxelWorld.World.Chuck;

namespace VoxelWorld.World.MeshRender
{
    public class ChuckMeshRender
    {
        public GameObject chuck;
        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;
        private BlockTextureBuilder textureBuilder;

        private int vertexIndex;
        private List<Vector3> verts;
        private List<int>[] tris;
        private List<Vector2> uvs;

        public void UpdateChuck(ChuckData chuckData)
        {
            InitialMeshData();
            AddMeshData(chuckData.blockData);
            BuildMesh();
            ResetMeshData();
        }

        public void UpdateChuck(BlockData[,,] blockData)
        {
            InitialMeshData();
            AddMeshData(blockData);
            BuildMesh();
            ResetMeshData();
        }

        public Vector2 position { get => new Vector2(chuck.transform.position.x, chuck.transform.position.z); }

        public bool isActive { get => chuck.activeSelf; set => chuck.SetActive(value); }

        public ChuckMeshRender(ChuckData chuckData, BlockTextureBuilder _textureBuilder, Transform parent)
        {
            chuck = new GameObject();
            chuck.name = "Chuck_" + chuckData.position.x + "_" + chuckData.position.y;
            chuck.transform.parent = parent;
            chuck.transform.position = new Vector3(chuckData.position.x * VoxelData.ChuckWidth,
                0, chuckData.position.y * VoxelData.ChuckWidth);
            textureBuilder = _textureBuilder;
            meshFilter = chuck.AddComponent<MeshFilter>();
            meshRenderer = chuck.AddComponent<MeshRenderer>();
            meshRenderer.materials = textureBuilder.blockMaterial.ToArray();
            InitialMeshData();
            AddMeshData(chuckData.blockData);
            BuildMesh();
            ResetMeshData();
        }

        public void Destroy()
        {
            Object.Destroy(chuck);
        }

        private void AddMeshData(BlockData[,,] blockData)
        {
            for (int i = 0; i < VoxelData.ChuckWidth; i++)
                for (int j = 0; j < VoxelData.ChuckWidth; j++)
                    for (int k = 0; k < VoxelData.ChuckHeight; k++)
                        AddPos(new Vector3(i, k, j), blockData);
        }

        private void ResetMeshData()
        {
            vertexIndex = 0;
            verts = null;
            uvs = null;
            tris = null;
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

        private bool Checkface(Vector3 pos, BlockData[,,] blockData)
        {
            int x = Mathf.RoundToInt(pos.x),
                y = Mathf.RoundToInt(pos.y),
                z = Mathf.RoundToInt(pos.z);

            if (y < 0)
                return true;

            if (x < 0 || x > VoxelData.ChuckWidth - 1
                || z < 0 || z > VoxelData.ChuckWidth - 1
                || y > VoxelData.ChuckHeight - 1)
                return false;

            return blockData[x, y, z] != null;
        }

        private void AddPos(Vector3 pos, BlockData[,,] blockData)
        {
            if (blockData[Mathf.RoundToInt(pos.x),
                 Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z)] == null)
                return;
            int blockID = blockData[Mathf.RoundToInt(pos.x),
                 Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z)].ID;
            if (blockID == 0)
                return;

            for (int i = 0; i < 6; i++)
            {
                if (!Checkface(pos + VoxelData.facecheck[i], blockData))
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