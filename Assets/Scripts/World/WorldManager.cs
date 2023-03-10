using UnityEngine;
using VoxelWorld.Utilities;

namespace VoxelWorld.World
{
    public class WorldManager : MonoBehaviour
    {
        public WorldData worldData;

        public static WorldManager Instance { get; private set; }

        public void InitialWorld(string name = "New World",int seed = 0,
            WORLDTYPE worldType = WORLDTYPE.NORMAL)
        {
            if (seed == 0)
                seed = Random.Range(0, int.MaxValue);
            worldData = new();
            worldData.name = name;
            worldData.worldType = worldType;
            FileTools.CreateWorld("Worlds/", ref worldData);
        }

        public void GetWorld(string uid)
        {
            FileTools.FindWorld("Worlds/", uid, ref worldData);
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