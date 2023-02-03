using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.JSONDatabases.Manager;
using VoxelWorld.Texture;
using VoxelWorld.World.Chuck;
using VoxelWorld.World.MeshRender;
using VoxelWorld.World.WorldGenerate;

namespace VoxelWorld.World
{
    [Utilities.ExecutionOrder.ExecuteAfter(typeof(BlockTextureBuilder))]
    public class World : MonoBehaviour
    {
        public Vector2 playerPos;
        public int DrawDistance;
        public int LoadDistance;
        public int UnloadDistance;
        public WorldManager worldManager;
        private BlockTextureBuilder textureBuilder;
        public RuntimeDatabasesManager databases;

        private void Awake()
        {
            textureBuilder = GetComponent<BlockTextureBuilder>();
            databases.LoadWorldGenerateRules(worldManager.worldData.worldType.ToString());
        }

        private void RenderChuck(ChuckData chuckData)
        {
            if (chuckData.meshRender == null)
            {
                chuckData.meshRender = new ChuckMeshRender(chuckData, textureBuilder, transform,
                databases.chuckDatas?[chuckData.position.x + "_" +
                (chuckData.position.y + 1)]?.blockData,
                databases.chuckDatas?[chuckData.position.x + "_" +
                (chuckData.position.y - 1)]?.blockData,
                databases.chuckDatas?[(chuckData.position.x + 1) + "_" +
                chuckData.position.y]?.blockData,
                databases.chuckDatas?[(chuckData.position.x - 1) + "_" +
                chuckData.position.y]?.blockData);
                //chuckData.meshRender = new ChuckMeshRender(chuckData, textureBuilder, transform);
            }
            else
                chuckData.meshRender.isActive = true;
        }

        private void UpdateChuck(ChuckData chuckData)
        {
            /*chuckData.meshRender?.UpdateChuck(chuckData,
                databases.chuckDatas?[chuckData.position.x + "_" +
                ( chuckData.position.y+ 1)]?.blockData,
                databases.chuckDatas?[chuckData.position.x + "_" +
                (chuckData.position.y - 1)]?.blockData,
                databases.chuckDatas?[(chuckData.position.x + 1) + "_" +
                chuckData.position.y]?.blockData,
                databases.chuckDatas?[(chuckData.position.x - 1) + "_" +
                chuckData.position.y]?.blockData);*/
            chuckData.meshRender?.UpdateChuck(chuckData);
        }

        private void UnrenderChuck(ChuckData chuckData)
        {
            if (chuckData.meshRender != null)
                chuckData.meshRender.isActive = false;
        }

        private void FixedUpdate()
        {
            ChucksManager();
        }

        private Vector2 GetPlayerChuck(Vector2 pos)
        {
            return new Vector2((int)pos.x / VoxelData.ChuckWidth, (int)pos.y / VoxelData.ChuckWidth);
        }

        private void ChucksManager()
        {
            var playerChuck = GetPlayerChuck(playerPos);
            List<string> unloadList = new();

            //load chuck
            for (int i = -LoadDistance; i < LoadDistance; i++)
                for (int j = -LoadDistance; j < LoadDistance; j++)
                {
                    if (i * i + j * j > LoadDistance * LoadDistance)
                        continue;
                    var pos = playerChuck + new Vector2(i, j);
                    if (!databases.chuckDatas.ContainsKey(pos.x + "_" + pos.y))
                    {
                        if (!databases.LoadChuck(pos, "Worlds/" + worldManager.worldData.uid))
                        {
                            if (worldManager.worldData.worldType == WORLDTYPE.NORMAL)
                                databases.chuckDatas.Add(pos.x + "_" + pos.y,
                                MapGenerator.GenerateNoiseChuck(pos, databases.rules,
                                worldManager.worldData.seed));
                            else
                                databases.chuckDatas.Add(pos.x + "_" + pos.y,
                                MapGenerator.GenerateFlatChuck(pos, databases.rules));
                        }
                    }
                }

            //render chuck
            foreach (var chuck in databases.chuckDatas)
            {
                if ((chuck.Value.position - playerChuck).sqrMagnitude > UnloadDistance * UnloadDistance)
                    unloadList.Add(chuck.Key);
                else if ((chuck.Value.position - playerChuck).sqrMagnitude < DrawDistance * DrawDistance)
                    RenderChuck(databases.chuckDatas[chuck.Key]);
                else
                    UnrenderChuck(databases.chuckDatas[chuck.Key]);
            }

            //unload chuck
            for (int i = 0; i < unloadList.Count; i++)
            {
                databases.chuckDatas[unloadList[i]].meshRender?.Destroy();
                databases.SaveChuck(unloadList[i], "Worlds/" + worldManager.worldData.uid);
                databases.chuckDatas.Remove(unloadList[i]);
            }
        }
    }
}