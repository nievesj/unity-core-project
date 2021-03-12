using System;
using Core.Systems.Network.Extensions;
using Core.Tools.Misc;
using LiteNetLib.Utils;
using UniRx;
using UnityEngine;
using Zenject;
using Logger = UnityLogger.Logger;

namespace Core.Systems.Network.Components
{
    public class PositionHistory
    {
        public uint Tick;
        public Vector2 Position;
    }

    [RequireComponent(typeof(NetworkIdentity))]
    [RequireComponent(typeof(NetworkTransform))]
    [RequireComponent(typeof(ZenAutoInjecter))]
    public class PlayerNetworkController : CoreNetworkBehavior
    {
        [SerializeField]
        private float moveSpeed = 3.5f;

        [SerializeField]
        private float turnSpeed = 120f;

        [Inject]
        private InputCoreSystem _inputCoreSystem;

        [Inject]
        private SpawnPointCollection _spawnPointCollection;

        private ushort _packetSequenceId;

        private Vector3 _moveVector;
        private uint _tick;

        private PositionHistory _currentCommand;
        private CircularBuffer<PositionHistory> _serverCachedPackets = new CircularBuffer<PositionHistory>(30);
        private CircularBuffer<PositionHistory> _vectorsToSend = new CircularBuffer<PositionHistory>(30);

        protected void Start()
        {
            var trans = _spawnPointCollection.GetRandomSpawnTransform();
            transform.SetParent(trans);
            transform.localPosition = Vector3.zero;

            CoreNetworkManager.Client.OnTick()
                .Subscribe(OnClientTick)
                .AddTo(this);

            if (IsLocalEntity)
            {
                _inputCoreSystem.RegisterInputAction(InputActionType.Move, CoreActionEventType.Started).Subscribe(ProcessInputCommand).AddTo(this);
                _inputCoreSystem.RegisterInputAction(InputActionType.Move, CoreActionEventType.Performed).Subscribe(ProcessInputCommand).AddTo(this);
                _inputCoreSystem.RegisterInputAction(InputActionType.Move, CoreActionEventType.Canceled).Subscribe(ProcessInputCommand).AddTo(this);
            }

            if (IsHost)
            {
                CoreNetworkManager.Server.OnTick()
                    .Subscribe(OnServerTick)
                    .AddTo(this);
            }
        }

        private void ProcessInputCommand(RegistrationValue value)
        {
            var vector = value.InputAction.ReadValue<Vector2>();
            _vectorsToSend.PushBack(new PositionHistory {Position = vector, Tick = _tick});

            if (IsLocalEntity)
            {
                var vector3 = new Vector3(vector.x, 0, vector.y);
                _moveVector = vector3;
            }
        }

        private void FixedUpdate()
        {
            transform.position += _moveVector * (moveSpeed * Time.fixedDeltaTime);
        }

        private void OnServerTick(uint deltaTick)
        {
            try
            {
                if (!_serverCachedPackets.IsEmpty)
                {
                    var command = _serverCachedPackets.PopFront();

                    if (_currentCommand == null)
                        _currentCommand = command;

                    if (_currentCommand.Tick > command.Tick)
                        _serverCachedPackets.PushBack(command);

                    _currentCommand = command;

                    var vector = new Vector3(_currentCommand.Position.x, 0, _currentCommand.Position.y);
                    _moveVector = vector;
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }

        private void OnClientTick(uint deltaTick)
        {
            _tick = deltaTick;
            //send all commands
            if (!_vectorsToSend.IsEmpty)
                Serialize();
        }

        protected override void OnSerialize(NetDataWriter writer)
        {
            writer.Put(_vectorsToSend.Size);
            foreach (var vector in _vectorsToSend)
            {
                writer.Put(vector.Tick);
                writer.Put(vector.Position);
            }

            //clear buffer - send and forget
            _vectorsToSend.FastClear();
        }

        protected override void OnDeserialize(NetDataReader reader)
        {
            var amount = reader.GetInt();
            for (var i = 0; i < amount; i++)
            {
                var tick = reader.GetUInt();
                var vector = reader.GetVector2();

                _serverCachedPackets.PushBack(new PositionHistory {Tick = tick, Position = vector});
            }
        }
    }
}