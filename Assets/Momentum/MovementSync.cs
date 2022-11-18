using System;
using UnityEngine;

namespace Mirage.Momentum
{
    public class MovementSync : NetworkBehaviour, IComparable<MovementSync>
    {
        [Tooltip("Check if this object will be moved by a player,  uncheck if only the server moves this object")]
        public bool PlayerControlled;

        public int SnapshotPerSecond = 30;

        public int CompareTo(MovementSync other)
        {
            return NetId.CompareTo(other.NetId);
        }

        

        public void Update() {
            if (HasAuthority && PlayerControlled && IsClientOnly)
            {
                SendPlayerState();
            }
        }

        protected virtual void SendPlayerState()
        {
            throw new NotImplementedException();
        }
    }
}