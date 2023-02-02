using UnityEngine;
using VoxelWorld.Texture;

namespace VoxelWorld.World.WorldGenerate
{
    [Utilities.ExecutionOrder.ExecuteAfter(typeof(World))]
    public class MapGenerator : MonoBehaviour
    {
        private World world;

        private void Awake()
        {
            world = GetComponent<World>();
        }

        public void GenerateChuck(Vector2 pos)
        {

        }
    }
}