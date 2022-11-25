using System.Collections;
using System.Collections.Generic;
using Mirage.Momentum;
using UnityEngine;

namespace Mirage.Examples.Tanks
{
    public struct TankState : ObjectState
    {
        public ushort NetId { get; set; }
        public Vector3 position;
        public Quaternion rotation;

        public Vector2 moveInput;
        public bool fireInput;
    }
}
