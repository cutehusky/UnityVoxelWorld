using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.Block;
using VoxelWorld.JSONDatabases.Manager;

namespace VoxelWorld.Texture
{
    public struct BlockUVInfo
    {
        public int MaterialIndex;
        public Vector2 UVOffset;
    }

    [Utilities.ExecutionOrder.ExecuteAfter(typeof(StaticDatabasesManager))]
    public class BlockTextureBuilder : MonoBehaviour
    {
        public StaticDatabasesManager databases;

        public int textureSize; // mod power of 2 == 0

        public int blockTextureSize; // power of 2

        [HideInInspector] public float uvScale;
        [HideInInspector] public List<Material> blockMaterial;

        private Dictionary<string, Material> materialTemplates;

        private Dictionary<string, List<Texture2D>> textures;

        private Dictionary<string, int> textureIndex;

        private Dictionary<string, int> i;

        private Dictionary<string, int> j;

        public BlockUVInfo GetBlockUVInfo(int BlockID, int face)
        {
            return databases.BlockList.Find(a => a.ID == BlockID).uvInfo[face];
        }

        public static Texture2D Resize(Texture2D texture2D, int targetX, int targetY)
        {
            RenderTexture rt = new RenderTexture(targetX, targetY, 24);
            RenderTexture.active = rt;
            Graphics.Blit(texture2D, rt);
            Texture2D result = new Texture2D(targetX, targetY);
            result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
            result.Apply();
            return result;
        }

        private void CreateMaterial()
        {
            foreach (var texture in textures)
            {
                for (int k = 0; k < textures[texture.Key].Count; k++)
                {
                    textures[texture.Key][k].Apply();
                    textures[texture.Key][k].filterMode = FilterMode.Point;
                    Material ma;
                    if (!materialTemplates.ContainsKey(texture.Key))
                    {
                        ma = Resources.Load<Material>("Materials/Blocks/" + texture.Key);
                        if (ma == null)
                            ma = materialTemplates["Default"];
                        else
                            materialTemplates.Add(texture.Key, ma);
                    }
                    else
                        ma = materialTemplates[texture.Key];
                    ma.mainTexture = textures[texture.Key][k];
                    blockMaterial.Add(new Material(ma));
                }
            }
        }

        private void BuildTexture(Texture2D texture2D, string material,
            ref BlockUVInfo blockUVInfo)
        {
            blockUVInfo = new();
            texture2D = Resize(texture2D, blockTextureSize, blockTextureSize);
            if (!i.ContainsKey(material))
            {
                i.Add(material, 0);
                j.Add(material, 0);
            }

            if (i[material] == 0 && j[material] == 0)
            {
                if (!textures.ContainsKey(material))
                {
                    textures.Add(material, new());
                    textureIndex.Add(material, -1);
                }
                textures[material].Add(new(textureSize, textureSize));
                textureIndex[material]++;
            }

            for (int _i = 0; _i < blockTextureSize; _i++)
            {
                for (int _j = 0; _j < blockTextureSize; _j++)
                {
                    textures[material][textureIndex[material]].SetPixel(i[material] * blockTextureSize + _i,
                        j[material] * blockTextureSize + _j, texture2D.GetPixel(_i, _j));
                }
            }

            blockUVInfo.MaterialIndex = textureIndex[material];
            blockUVInfo.UVOffset = new Vector2(i[material] * uvScale, j[material] * uvScale);
            i[material]++;
            if (i[material] == textureSize / blockTextureSize)
            {
                if (j[material] == textureSize / blockTextureSize)
                    j[material] = 0;
                else
                    j[material]++;
                i[material] = 0;
            }
        }

        private void LoadTextures(ref List<BlockType> blockList)
        {
            for (int k = 0; k < blockList.Count; k++)
            {
                var tmp = Resources.Load<Texture2D>("Textures/Blocks/" + blockList[k].texturePath + "_" + 0);
                if (tmp == null)
                {
                    Debug.Log("Missing texture: " + blockList[k].name);
                    continue;
                }
                blockList[k].uvInfo = new BlockUVInfo[6];
                BuildTexture(tmp, blockList[k].materialPath, ref blockList[k].uvInfo[0]);

                for (int p = 1; p < 6; p++)
                {
                    tmp = Resources.Load<Texture2D>("Textures/Blocks/" + blockList[k].texturePath + "_" + p);
                    if (tmp == null)
                    {
                        blockList[k].uvInfo[p] = blockList[k].uvInfo[p - 1];
                        continue;
                    }
                    BuildTexture(tmp, blockList[k].materialPath, ref blockList[k].uvInfo[p]);
                }
            }
        }

        private void ResetValue()
        {
            textureIndex = null;
            i = null;
            j = null;
            foreach (var material in materialTemplates)
            {
                Resources.UnloadAsset(materialTemplates[material.Key]);
            }
            materialTemplates = null;
        }

        private void InitialValue()
        {
            uvScale = (blockTextureSize * 1.0f) / textureSize;
            blockMaterial = new();
            textures = new();
            textureIndex = new();
            i = new();
            j = new();
            materialTemplates = new()
            {
                { "Default", Resources.Load<Material>("Materials/Blocks/Default") }
            };
        }

        private void Awake()
        {
            InitialValue();
            LoadTextures(ref databases.BlockList);
            CreateMaterial();
            ResetValue();
        }
    }
}