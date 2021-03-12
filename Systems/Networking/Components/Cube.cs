using LiteNetLib.Utils;
using UniRx;
using UnityEngine;

namespace Core.Systems.Network.Components
{
    public class Cube : CoreNetworkBehavior
    {
        public float moveSpeed = 5f;
        public Vector2 minMax = new Vector2(10, -10);

        private void Start()
        {
            CoreNetworkManager.Client.OnTick()
                .Subscribe(OnClientTick)
                .AddTo(this);

            if (IsHost)
            {
                CoreNetworkManager.Server.OnTick()
                    .Subscribe(OnServerTick)
                    .AddTo(this);
            }

            originalPosition = transform.position;
            CoreNetworkManager.RegisterNetworkObject(this);
        }

        private bool isMovingLeft;
        public Vector3 originalPosition;

        private void FixedUpdate()
        {
            if (!isMovingLeft)
            {
                transform.position += Vector3.right * moveSpeed * Time.fixedDeltaTime;
                if (transform.position.x >= (originalPosition.x + minMax.x))
                    isMovingLeft = true;
            }
            else
            {
                transform.position += Vector3.left * moveSpeed * Time.fixedDeltaTime;
                if (transform.position.x <= (originalPosition.x + minMax.y))
                    isMovingLeft = false;
            }
        }

        private void OnClientTick(uint deltaTick) { }

        private void OnServerTick(uint deltaTick) { }

        protected override void OnSerialize(NetDataWriter writer) { }

        protected override void OnDeserialize(NetDataReader reader) { }
    }
}