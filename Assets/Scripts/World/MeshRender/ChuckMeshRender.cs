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

        public void UpdateChuck(ChuckData chuckData,
            BlockData[,,] blockData_f = null, BlockData[,,] blockData_b = null,
            BlockData[,,] blockData_r = null, BlockData[,,] blockData_l = null)
        {
            InitialMeshData();
            AddMeshData(chuckData.blockData,
                blockData_f, blockData_b, blockData_r, blockData_l);
            BuildMesh();
            ResetMeshData();
        }

        public void UpdateChuck(BlockData[,,] blockData,
            BlockData[,,] blockData_f = null, BlockData[,,] blockData_b = null,
            BlockData[,,] blockData_r = null, BlockData[,,] blockData_l = null)
        {
            InitialMeshData();
            AddMeshData(blockData,
                blockData_f, blockData_b, blockData_r, blockData_l);
            BuildMesh();
            ResetMeshData();
        }

        public Vector2 position { get => new Vector2(chuck.transform.position.x, chuck.transform.position.z); }

        public bool isActive { get => chuck.activeSelf; set => chuck.SetActive(value); }

        public ChuckMeshRender(ChuckData chuckData, BlockTextureBuilder _textureBuilder, Transform parent,
            BlockData[,,] blockData_f = null, BlockData[,,] blockData_b = null,
            BlockData[,,] blockData_r = null, BlockData[,,] blockData_l = null)
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
            AddMeshData(chuckData.blockData, blockData_f, blockData_b, blockData_r, blockData_l);
            BuildMesh();
            ResetMeshData();
        }

        public void Destroy()
        {
            Object.Destroy(chuck);
        }

        private void AddMeshData(BlockData[,,] blockData,
            BlockData[,,] blockData_f, BlockData[,,] blockData_b, BlockData[,,] blockData_r,
            BlockData[,,] blockData_l)
        {
            for (int i = 0; i < VoxelData.ChuckWidth; i++)
                for (int j = 0; j < VoxelData.ChuckWidth; j++)
                    for (int k = 0; k < VoxelData.ChuckHeight; k++)
                        AddPos(new Vector3(i, k, j), blockData,
                            blockData_f, blockData_b,blockData_r, blockData_l);
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

        private bool Checkface(Vector3 pos, BlockData[,,] blockData, 
            BlockData[,,] blockData_f,BlockData[,,] blockData_b,BlockData[,,] blockData_r, 
            BlockData[,,] blockData_l)
        {
            int x = Mathf.RoundToInt(pos.x),
                y = Mathf.RoundToInt(pos.y),
                z = Mathf.RoundToInt(pos.z);

            if (y < 0)
                return true;

            if (y > VoxelData.ChuckHeight - 1)
                return false;

            if (x < 0)
            {
                if (blockData_l == null)
                    return false;
                return blockData_l[VoxelData.ChuckWidth - 1, y, z]?.ID != 0 &&
                    blockData_l[VoxelData.ChuckWidth - 1, y, z] != null;
            }
                

            if (x > VoxelData.ChuckWidth - 1)
            {
                if (blockData_r == null)
                    return false;
                return blockData_r[0, y, z] != null &&
                    blockData_r[0, y, z]?.ID != 0;
            }
                

            if (z < 0)
            {
                if (blockData_b == null)
                    return false;
                return blockData_b[x, y, VoxelData.ChuckWidth - 1] != null &&
                    blockData_b[x, y, VoxelData.ChuckWidth - 1]?.ID != 0;
            }
  

            if (z > VoxelData.ChuckWidth - 1)
            {
                if (blockData_f == null)
                    return false;
                return blockData_f[x, y, 0] != null &&
                    blockData_f[x, y, 0]?.ID != 0;
            }
                
            return blockData[x, y, z] != null && blockData[x, y, z]?.ID != 0;
        }

        private void AddPos(Vector3 pos, BlockData[,,] blockData,
            BlockData[,,] blockData_f, BlockData[,,] blockData_b, BlockData[,,] blockData_r,
            BlockData[,,] blockData_l)
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
                if (!Checkface(pos + VoxelData.facecheck[i], blockData,
                            blockData_f, blockData_b, blockData_r, blockData_l))
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