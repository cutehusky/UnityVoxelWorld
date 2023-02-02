using System.Collections.Generic;

namespace VoxelWorld.Block
{
    public class BlockData
    {
        public int ID = 0;
        public int Data = 0;
        public List<string> List = new();
        public Dictionary<string, string> Tag = new();
    }
}