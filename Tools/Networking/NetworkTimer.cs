using System;
using System.Diagnostics;

namespace Core.Tools.Networking
{
    public class NetworkTimer 
    {
        private double _accumulator;
        private long _lastTime;

        private readonly Stopwatch _stopwatch;
        private readonly Action _action;
        public const float FixedDelta = 1.0f / FramesPerSecond;
        public const float FramesPerSecond = 30.0f;

        public NetworkTimer(Action action)
        {
            _stopwatch = new Stopwatch();
            _action = action;
        }

        public void Start()
        {
            _lastTime = 0;
            _accumulator = 0.0;
            _stopwatch.Restart();
        }

        public void Stop()
        {
            _stopwatch.Stop();
        }

        public void Tick()
        {
            var elapsedTicks = _stopwatch.ElapsedTicks;
            _accumulator += (double) (elapsedTicks - _lastTime) / Stopwatch.Frequency;
            _lastTime = elapsedTicks;

            while (_accumulator >= FixedDelta)
            {
                _action.Invoke();
                _accumulator -= FixedDelta;
            }
        }
    }
}