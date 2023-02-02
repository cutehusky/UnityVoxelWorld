using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.Texture;
using VoxelWorld.World.Chuck;
using VoxelWorld.World.MeshRender;
using VoxelWorld.World.WorldGenerate;
using Newtonsoft.Json;
using VoxelWorld.JSONDatabases.Manager;

namespace VoxelWorld.World
{
    [Utilities.ExecutionOrder.ExecuteAfter(typeof(BlockTextureBuilder))]
    public class World : MonoBehaviour
    {
        private Dictionary<string, ChuckMeshRender> chuckMeshRenders;
        public Vector2 playerPos;
        public int DrawDistance;
        public int RemoveDistance;
        public int LoadDistance;
        public WorldManager worldManager;
        private BlockTextureBuilder textureBuilder;
        private RuntimeDatabasesManager databases;
        private MapGenerator mapGenerator;

        private void Awake()
        {
            textureBuilder = GetComponent<BlockTextureBuilder>();
            mapGenerator = GetComponent<MapGenerator>();
            databases.LoadWorldGenerateRules(worldManager.worldData.worldType.ToString());
        }

        private void RenderChuck(ChuckData chuckData)
        {
            if (chuckMeshRenders.ContainsKey(chuckData.position.x + "_" + chuckData.position.y))
                chuckMeshRenders[chuckData.position.x + "_" + chuckData.position.y].isActive = true;
            else
                chuckMeshRenders.Add(chuckData.position.x + "_" + chuckData.position.y,
                    new ChuckMeshRender(chuckData, textureBuilder, transform));
        }

        private void UpdateChuck(ChuckData chuckData)
        {
            if (chuckMeshRenders.ContainsKey(chuckData.position.x + "_" + chuckData.position.y))
                chuckMeshRenders[chuckData.position.x + "_" + chuckData.position.y].UpdateChuck(chuckData);
            else
                chuckMeshRenders.Add(chuckData.position.x + "_" + chuckData.position.y,
                    new ChuckMeshRender(chuckData, textureBuilder, transform));
        }

        private void UnloadChuck(Vector2 pos)
        {
            if (chuckMeshRenders.ContainsKey(pos.x + "_" + pos.y))
                chuckMeshRenders[pos.x + "_" + pos.y].isActive = false;
        }

        private void ClearChuck(Vector2 pos)
        {
            if (chuckMeshRenders.ContainsKey(pos.x + "_" + pos.y))
            {
                chuckMeshRenders.Remove(pos.x + "_" + pos.y);
            }
        }

        private void ChucksManager()
        {
            for (int i = -LoadDistance; i < LoadDistance;i++)
            {
                for (int j = -LoadDistance; j < LoadDistance; j++)
                {
                    var pos = new Vector2(i, j);
                    if (databases.chuckDatas.FindIndex(chuck => chuck.position ==
                    (GetPlayerChuck(playerPos) + pos)) == -1)
                    {
                        if (!databases.LoadChuck(pos))
                            mapGenerator.GenerateChuck(pos);
                    }
                }
            }
        }

        private Vector2 GetPlayerChuck(Vector2 pos)
        {
            return new Vector2((int)pos.x / VoxelData.ChuckWidth, (int)pos.y / VoxelData.ChuckWidth);
        }

        private void RenderChucks()
        {
            var list = databases.chuckDatas.FindAll(chuck => (chuck.position -
                GetPlayerChuck(playerPos)).sqrMagnitude <= DrawDistance * DrawDistance);
            for (int i = 0; i < list.Count; i++)
                RenderChuck(list[i]);

            list = databases.chuckDatas.FindAll(chuck => (chuck.position -
                GetPlayerChuck(playerPos)).sqrMagnitude > RemoveDistance * RemoveDistance);
            for (int i = 0; i < list.Count; i++)
                ClearChuck(list[i].position);

            list = databases.chuckDatas.FindAll(chuck => (chuck.position -
                GetPlayerChuck(playerPos)).sqrMagnitude > DrawDistance * DrawDistance);
            for (int i = 0; i < list.Count; i++)
                UnloadChuck(list[i].position);
        }
    }
}