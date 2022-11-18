using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mirage.Momentum
{

 
    public class Snapshot
    {
        public struct ObjectState
        {
            public uint NetId;
            public Vector3 Position;
            public Quaternion Rotation;
        }
        
        internal ushort Id;
        // server time when this snapshot got generated
        public double Time;

        public List<ObjectState> ObjectsState = new List<ObjectState>();

        
        [NonSerialized]
        public HashSet<INetworkPlayer> Players = new();
    }
}