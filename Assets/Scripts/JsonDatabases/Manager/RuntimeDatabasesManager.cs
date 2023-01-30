using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelWorld.JSONDatabases.Manager
{
    [Utilities.ExecutionOrder.ExecuteAfter(typeof(StaticDatabasesManager))]
    public class RuntimeDatabasesManager : MonoBehaviour
    {
        private void Awake()
        {

        }
    }
}