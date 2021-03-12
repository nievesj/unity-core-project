using System.Linq;
using Core.Systems.Network.Extensions;
using Core.Tools.Misc;
using LiteNetLib.Utils;
using UniRx;
using UnityEngine;

namespace Core.Systems.Network.Components
{
    public class MovementHistory
    {
        public ushort PeerId;
        public uint Tick;
        public Vector3 Position;
    }

    [RequireComponent(typeof(NetworkIdentity))]
    public class NetworkTransform : CoreNetworkBehavior
    {
        private CircularBuffer<MovementHistory> _movementBuffer = new CircularBuffer<MovementHistory>(30);
        private CircularBuffer<MovementHistory> _localHistory = new CircularBuffer<MovementHistory>(30);

        private MovementHistory _currentMovement;
        private float moveSpeed = 3.5f;

        private void Start()
        {
            if (!IsHost)
            {
                CoreNetworkManager.Client.OnTick()
                    .Subscribe(OnClientTick)
                    .AddTo(this);
            }
            else
            {
                CoreNetworkManager.Server.OnTick()
                    .Subscribe(OnServerTick)
                    .AddTo(this);
            }
        }

        private void FixedUpdate()
        {
            if (!IsLocalEntity)
            {
                if (_currentMovement != null)
                    transform.position = Vector3.Lerp(transform.position, _currentMovement.Position, (moveSpeed * 8) * Time.fixedDeltaTime);
            }
            else
                ReconcileLocal();
        }

        private void ReconcileLocal()
        {
            while (!_movementBuffer.IsEmpty)
            {
                var serverItem = _movementBuffer.PopBack();
                var localItem = _localHistory.ToArray().FirstOrDefault(i => i.Tick >= serverItem.Tick);

                var distance = 0f;
                if (localItem != null)
                    distance = Vector3.Distance(serverItem.Position, localItem.Position);

                var itemsToReplay = _movementBuffer.Where(i => i.Tick >= serverItem.Tick);
                if (distance > 0.9f)
                {
                    if (itemsToReplay.Count() > 0)
                    {
                        foreach (var item in itemsToReplay)
                            transform.position = Vector3.Lerp(transform.position, item.Position, (moveSpeed * 8) * Time.fixedDeltaTime);
                    }
                    else
                    {
                        transform.position = Vector3.Lerp(transform.position, serverItem.Position, (moveSpeed * 8) * Time.fixedDeltaTime);
                    }
                }
            }
        }

        private void OnClientTick(uint deltaTick)
        {
            if (!IsHost)
            {
                if (!IsLocalEntity)
                {
                    if (!_movementBuffer.IsEmpty)
                    {
                        var history = _movementBuffer.PopBack();

                        //first time
                        if (_currentMovement == null)
                            _currentMovement = history;

                        if (history.Tick > _currentMovement.Tick)
                            _currentMovement = history;
                    }
                }
                else
                    _localHistory.PushFront(new MovementHistory {Tick = NetworkTick, Position = transform.position});
            }
        }

        private void OnServerTick(uint deltaTick)
        {
            NetworkTick = deltaTick;
            Serialize(false);
        }

        protected override void OnSerialize(NetDataWriter writer)
        {
            writer.Put(NetworkTick);
            writer.Put(transform.position);
        }

        protected override void OnDeserialize(NetDataReader reader)
        {
            if (IsHost) return;

            NetworkTick = reader.GetUInt();
            var pos = reader.GetVector3();

            var hist = new MovementHistory {PeerId = PeerId, Tick = NetworkTick, Position = pos};
            _movementBuffer.PushFront(hist);
        }
    }
}