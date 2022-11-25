using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirage;
namespace Mirage.Momentum
{

    /// <summary>
    /// The snapshot system generates snapshots of all objects in the server
    /// sends them to the clients
    /// and performs interpolation on the clients
    /// </summary>
    public abstract class MovementSystem<T> : MonoBehaviour where T : ObjectState
    {        
        public int SnapshotPerSecond = 30;

        private ushort _snapshotId = 0;

        public NetworkClient Client;
        public NetworkServer Server;

        public void Awake()
        {
            InitServer();
            InitClient();
        }

        private void InitServer()
        {
            Server.Started.AddListener(OnStartServer);
            Server.Stopped.AddListener(OnStopServer);
            Server.Connected.AddListener(OnServerConnected);
        }

        private void OnServerConnected(INetworkPlayer player)
        {
            player.NotifyDelivered += OnNotifyDelivered;
            player.RegisterHandler<SnapshotMessage>(OnServerReceiveSnapshot);
        }

        private void OnServerReceiveSnapshot(INetworkPlayer arg1, SnapshotMessage snapshotMsg)
        { 
            // apply the snapshot
            var snapshot = new Snapshot<T>()
            {
                Id = snapshotMsg.SnapshotId,
                Time = snapshotMsg.Time,
                ObjectsState = ReadSnapshotData(new BitBuffer(snapshotMsg.Data))
            };

            ApplySnapshot(snapshot, Server.World);
        }

        private void OnServerSnapshot(INetworkPlayer arg1, SnapshotMessage arg2)
        {
            throw new NotImplementedException();
        }

        private void OnNotifyDelivered(INetworkPlayer player, object token)
        {
            // if (token is Snapshot snapshot)
            // {
            //     snapshot.Players.Add(player);
            // }
        }

        #region Server generating and sending snapshots

        Coroutine serverSnapshotCoroutine;

        private void OnStartServer()
        {
            serverSnapshotCoroutine = StartCoroutine(SendSnapshots());
        }

        private void OnStopServer()
        {
            StopCoroutine(serverSnapshotCoroutine);
        }

        private IEnumerator SendSnapshots()
        {
            while (true)
            {
                SendSnapshot(Server.World);
                yield return new WaitForSeconds(1f / SnapshotPerSecond);
            }
        }
        private void SendSnapshot(NetworkWorld world)
        {
            // generate a snapshot of all objects and send it to the clients

            Snapshot<T> snapshot = TakeSnapshot(world);

            // delta compress this snapshot against the baseline
            BitBuffer buffer = new BitBuffer(1500);

            WriteSnapshotData(buffer, snapshot);

            var SnapshotMessage = new SnapshotMessage
            {
                SnapshotId = snapshot.Id,
                Time = snapshot.Time,
                Data = buffer.ToMemory()
            };
            foreach (var connection in Server.Players)
            {
                if (connection.IsReady && connection != Server.LocalPlayer)
                    connection.SendNotify(SnapshotMessage, null);
            }
        }

        private void WriteSnapshotData(BitBuffer buffer, Snapshot<T> snapshot)
        {
            // write the number of objects in this snapshot
            buffer.Write(snapshot.ObjectsState.Count, 16);

            // write the objects state
            foreach (var objectState in snapshot.ObjectsState)
            {                
                Serialize(buffer, objectState);
            }
        }

        protected abstract void Serialize(BitBuffer buffer, T objectState);

        private Snapshot<T> TakeSnapshot(NetworkWorld world)
        {
            var snapshot = new Snapshot<T>()
            {
                Time = Time.unscaledTime,
                Id = _snapshotId++,
                ObjectsState = new List<T>(world.SpawnedIdentities.Count)
            };

            foreach (var obj in world.SpawnedIdentities)
            {
                var movementSync = obj.GetComponent<MovementSync>();
                if (movementSync != null)
                {
                    snapshot.ObjectsState.Add(GetState(movementSync));
                }
            }       
            return snapshot;
        }

        protected abstract T GetState(MovementSync obj);

        #endregion;

        #region Client


        private void InitClient()
        {
            Client.Authenticated.AddListener(OnClientConnected);
            Client.Disconnected.AddListener(OnClientDisconnected);
        }

        Coroutine clientSnapshotCoroutine;

        private void OnClientConnected(INetworkPlayer connection)
        {
            if (!Client.IsLocalClient)
            {
                connection.RegisterHandler<SnapshotMessage>(OnClientReceiveSnapshot);
                clientSnapshotCoroutine = StartCoroutine(SendClientSnapshots());
            }
            else
            {
                connection.RegisterHandler<SnapshotMessage>(msg => { });
            }
        }

        private IEnumerator SendClientSnapshots()
        {
            while (true)
            {
                SendClientSnapshot(Client.World);
                yield return new WaitForSeconds(1f / SnapshotPerSecond);
            }
            
        }

        private void SendClientSnapshot(NetworkWorld world)
        {
            Snapshot<T> snapshot = TakeClientSnapshot(world);

            // delta compress this snapshot against the baseline
            BitBuffer buffer = new BitBuffer(1500);

            WriteSnapshotData(buffer, snapshot);

            var SnapshotMessage = new SnapshotMessage
            {
                SnapshotId = snapshot.Id,
                Time = snapshot.Time,
                Data = buffer.ToMemory()
            };
            Client.Player.SendNotify(SnapshotMessage, null);
        }

        private Snapshot<T> TakeClientSnapshot(NetworkWorld world)
        {
            var snapshot = new Snapshot<T>()
            {
                Time = Time.unscaledTime,
                Id = _snapshotId++,
                ObjectsState = new List<T>(1)
            };

            foreach (var obj in world.SpawnedIdentities)
            {
                if (obj.IsLocalPlayer)
                {
                    var movementSync = obj.GetComponent<MovementSync>();
                    if (movementSync != null && movementSync.PlayerControlled)
                    {
                        snapshot.ObjectsState.Add(GetState(movementSync));
                    }
                }
            }       
            return snapshot;
        }


        private void OnClientDisconnected()
        {
            if (clientSnapshotCoroutine is not null)
            {
                StopCoroutine(clientSnapshotCoroutine);
            }
        }

        // we will interpolate from this snapshot to the next snapshot

        private void OnClientReceiveSnapshot(INetworkPlayer arg1, SnapshotMessage snapshotMsg)
        { 
            // apply the snapshot
            var snapshot = new Snapshot<T>()
            {
                Id = snapshotMsg.SnapshotId,
                Time = snapshotMsg.Time,
                ObjectsState = ReadSnapshotData(new BitBuffer(snapshotMsg.Data))
            };

            ApplySnapshot(snapshot, Client.World);
        }

        private List<T> ReadSnapshotData(BitBuffer buffer)
        {
            uint count = buffer.ReadUInt32(16);
            List<T> result = new List<T>((int)count);

            for (int i = 0; i < count; i++) {
                var objectState = Deserialize(buffer);

                result.Add(objectState);
            }

            return result;
        }
        protected abstract T Deserialize(BitBuffer buffer);

        private void ApplySnapshot(Snapshot<T> snapshot, NetworkWorld world)
        {
            // apply the snapshot
            foreach (var objectState in snapshot.ObjectsState)
            {
                if (world.TryGetIdentity(objectState.NetId, out var identity))
                {
                    if (identity.TryGetComponent(out MovementSync movementSync))
                    {
                        SetState(movementSync, objectState);
                    }
                }
            }
        }

        protected abstract void SetState(MovementSync movementSync, T objectState);

        #endregion
    }

}