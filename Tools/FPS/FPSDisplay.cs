using Core.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Tools.FPS
{
    public class FPSDisplay : CoreBehaviour
    {
        [SerializeField]
        private Text text;

        private float _deltaTime = 0.0f;
        private float _msec;
        private float _fps;

        private void Update()
        {
            if (text)
            {
                _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
                _msec = _deltaTime * 1000.0f;
                _fps = 1.0f / _deltaTime;
                text.text = $"{_msec:0.0} ms ({_fps:0.} fps)";
            }
        }
    }
}