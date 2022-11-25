using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirage.Momentum;
using UnityEngine.AI;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Mirage.Examples.Tanks
{
    public class TankMovementSystem : MovementSystem<TankState>
    {
        protected override TankState Deserialize(BitBuffer buffer)
        {
            TankState state = new TankState()
            {
                NetId = buffer.ReadUShort(),
                position = buffer.ReadVector3(),
                rotation = buffer.ReadCompressedQuaternion(),
                moveInput = buffer.ReadVector2(),
                fireInput = buffer.ReadBool()
            };
            return state;
        }

        protected override void Serialize(BitBuffer buffer, TankState objectState)
        {
            buffer.Write(objectState.NetId, 16);
            buffer.WriteVector3(objectState.position);
            buffer.WriteCompressedQuaternion(objectState.rotation);
            buffer.WriteVector2(objectState.moveInput);
            buffer.WriteBoolean(objectState.fireInput);
        }
        protected override TankState GetState(MovementSync obj)
        {
            var tankInput = obj.GetComponent<Tank>();

            TankState state = new TankState()
            {
                NetId = obj.NetId,
                position = obj.transform.position,
                rotation = obj.transform.rotation,
                moveInput = tankInput.MoveInput,
                fireInput = tankInput.FireInput
            };

            return state;
        }


        protected override void SetState(MovementSync movementSync, TankState objectState)
        {
            if (movementSync.NetIdentity.IsLocalPlayer)
                return;
            movementSync.transform.rotation = objectState.rotation;
            var tankInput = movementSync.GetComponent<Tank>();

            tankInput.MoveInput = objectState.moveInput;
            tankInput.FireInput = objectState.fireInput;

            var agent = movementSync.GetComponent<NavMeshAgent>();
            agent.Warp(objectState.position);
        }
    }
}