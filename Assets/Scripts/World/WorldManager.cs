using System.Collections.Generic;
using UnityEngine;
using VoxelWorld.Utilities;
using VoxelWorld.World.Chuck;

namespace VoxelWorld.World
{
    public class WorldManager : MonoBehaviour
    {
        public WorldData worldData;

        public static WorldManager Instance { get; private set; }

        public void InitialWorld(string name = "New World")
        {
            worldData = new();
            worldData.name = name;
            worldData.worldType = WORLDTYPE.FLAT;
            FileTools.CreateWorld(Application.persistentDataPath + "/Worlds", ref worldData);
        }

        public void GetWorld(string uid)
        {
            FileTools.FindWorld(Application.persistentDataPath + "/Worlds", uid, ref worldData);
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            InitialWorld();
        }
    }
}