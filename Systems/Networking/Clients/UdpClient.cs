using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Core.Tools.Networking;
using Cysharp.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Utils;
using UniRx;
using UnityLogger;

namespace Core.Networking.Clients
{
    public enum PacketType : byte
    {
        GameState,
        Serialized,
        Command,
        SerializedComponent
    }

    internal interface IObservablePacket
    {
        Type Type { get; }
        void Dispose();
    }

    public struct ComponentData
    {
        public NetPeer Peer;
        public NetPacketReader Reader;
        public DeliveryMethod DeliveryMethod;
    }

    internal class ObservablePacket<T> : IObservablePacket, IDisposable
    {
        public Type Type { get; }
        public readonly Action<T> Action;
        public readonly Subject<T> OnPacketReceived;

        public ObservablePacket()
        {
            Type = typeof(T);
            Action = OnActionInvoked;
            OnPacketReceived = new Subject<T>();
        }

        private void OnActionInvoked(T value)
        {
            OnPacketReceived.OnNext(value);
        }

        public void Dispose()
        {
            OnPacketReceived.Dispose();
        }
    }

    public struct CompositeData<T, TUserData>
    {
        public T Value;
        public TUserData UserData;
    }

    internal class ObservablePacket<T, TUserData> : IObservablePacket, IDisposable
    {
        public Type Type { get; }
        public Action<T, TUserData> Action;
        public Subject<CompositeData<T, TUserData>> OnPacketReceived;
        private CompositeData<T, TUserData> _cachedValue;

        public ObservablePacket()
        {
            Type = typeof(T);
            Action = OnActionInvoked;
            OnPacketReceived = new Subject<CompositeData<T, TUserData>>();
            _cachedValue = new CompositeData<T, TUserData>();
        }

        private void OnActionInvoked(T value, TUserData userData)
        {
            _cachedValue.Value = value;
            _cachedValue.UserData = userData;

            OnPacketReceived.OnNext(_cachedValue);
        }

        public void Dispose()
        {
            OnPacketReceived.OnCompleted();
            OnPacketReceived.Dispose();
        }
    }

    public abstract class UdpClient : INetEventListener, IDisposable //IDeliveryEventListener TODO: implement this
    {
        protected NetManager NetManager;
        protected NetPacketProcessor NetPacketProcessor;
        protected NetDataWriter NetDataWriter;

        protected readonly int Port;
        protected readonly string Key;
        protected NetworkTimer NetworkTimer;
        public uint Tick { get; protected set; }

        private readonly Dictionary<Type, IObservablePacket> _callbacks = new Dictionary<Type, IObservablePacket>();
        public bool IsRunning { get; protected set; }
        protected int EventPoolingFrequencyMilliseconds;
        protected Subject<uint> _onTick;
        private CancellationTokenSource _cancellationToken;
        private Subject<ComponentData> _onReceivedSerializedComponent = new Subject<ComponentData>();

        protected UdpClient(int port, string key, int eventPoolingFrequencyMilliseconds = 20)
        {
            Port = port;
            Key = key;
            NetPacketProcessor = new NetPacketProcessor();
            NetDataWriter = new NetDataWriter();
            NetManager = new NetManager(this);
            EventPoolingFrequencyMilliseconds = eventPoolingFrequencyMilliseconds;
            _onTick = new Subject<uint>();

            _cancellationToken = new CancellationTokenSource();
        }

        public virtual void Start()
        {
            IsRunning = true;
            // RunOnThreadPool(or UniTask.Void(async void), UniTask.Create(async UniTask))
            NetworkTimer = new NetworkTimer(OnTimerUpdate);
            NetworkTimer.Start();
            Run();
        }

        public virtual void Stop()
        {
            IsRunning = false;
            _cancellationToken.Cancel();
            NetworkTimer.Stop();
            NetManager.Stop();
        }

        public IObservable<uint> OnTick()
        {
            return _onTick;
        }

        public IObservable<ComponentData> OnReceivedSerializedComponent()
        {
            return _onReceivedSerializedComponent;
        }

        protected virtual async void Run()
        {
            try
            {
                while (IsRunning)
                {
                    PoolEvents();
                    NetworkTimer.Tick();

                    await UniTask.Delay(EventPoolingFrequencyMilliseconds, cancellationToken: _cancellationToken.Token);
                }
            }
            catch (OperationCanceledException e)
            {
                Logger.LogException(e); 
            }
        }

        protected virtual void PoolEvents()
        {
            NetManager?.PollEvents();
        }

        protected virtual void OnTimerUpdate()
        {
            Tick = ((Tick + 1) % uint.MaxValue);
            _onTick.OnNext(Tick);
        }

        public virtual void RegisterNestedType<T>() where T : struct, INetSerializable
        {
            NetPacketProcessor.RegisterNestedType<T>();
        }

        public virtual IObservable<T> RegisterToPacket<T>() where T : class, new()
        {
            if (_callbacks.ContainsKey(typeof(T)))
                _callbacks.Remove(typeof(T));

            var observableCallback = new ObservablePacket<T>();
            _callbacks.Add(typeof(T), observableCallback);

            NetPacketProcessor.SubscribeReusable<T>(observableCallback.Action);

            return observableCallback.OnPacketReceived;
        }

        public virtual IObservable<CompositeData<T, TUserData>> RegisterToPacket<T, TUserData>() where T : class, new()
        {
            if (_callbacks.ContainsKey(typeof(T)))
                _callbacks.Remove(typeof(T));

            var observableCallback = new ObservablePacket<T, TUserData>();
            _callbacks.Add(typeof(T), observableCallback);

            NetPacketProcessor.SubscribeReusable<T, TUserData>(observableCallback.Action);

            return observableCallback.OnPacketReceived;
        }

        public virtual void UnSubscribeToPacket<T>() where T : class, new()
        {
            var callback = _callbacks[typeof(T)];
            _callbacks.Remove(typeof(T));
            callback.Dispose();

            NetPacketProcessor.RemoveSubscription<T>();
        }

        public abstract void OnPeerConnected(NetPeer peer);

        public abstract void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo);

        public abstract void OnNetworkError(IPEndPoint endPoint, SocketError socketError);

        public abstract void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType);

        public abstract void OnNetworkLatencyUpdate(NetPeer peer, int latency);

        public abstract void OnConnectionRequest(ConnectionRequest request);

        public virtual void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            try
            {
                var packetType = reader.GetByte();
                var pt = (PacketType) packetType;

                switch (pt)
                {
                    case PacketType.GameState:
                        break;
                    case PacketType.Serialized:
                        NetPacketProcessor.ReadAllPackets(reader, peer);
                        break;
                    case PacketType.Command:
                        break;
                    case PacketType.SerializedComponent:
                        _onReceivedSerializedComponent.OnNext(new ComponentData {Peer = peer, Reader = reader, DeliveryMethod = deliveryMethod});
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }
        }

        public virtual void Dispose()
        {
            Stop();

            _onTick.Dispose();
            NetManager.DisconnectAll();
            NetManager.Stop();
            NetPacketProcessor = null;
            NetDataWriter = null;
            NetManager = null;
        }
    }
}