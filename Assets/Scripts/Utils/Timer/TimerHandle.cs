using System;
using UnityEngine;

namespace Farm.Utils.Timer
{
    public class TimerHandle
    {
        private float _speedMultiplier = 1f;
        private bool _isLooping;

        public float RemainingTime { get; internal set; }
        public float Duration { get; }
        public bool IsLooping => _isLooping;
        public Action OnTimerExpire { get; }

        public float Progress => Mathf.Clamp01(RemainingTime / Duration);    
        
        public float SpeedMultiplier
        {
            get => _speedMultiplier;
            set => _speedMultiplier = Mathf.Max(0.01f, value);
        }

        public TimerHandle(float duration, Action onTimerExpire, bool isLooping)
        {
            Duration = duration;
            RemainingTime = duration;
            OnTimerExpire = onTimerExpire;
            _isLooping = isLooping;
        }

        public void AddTime(float additionalTime) => 
            RemainingTime = Mathf.Min(RemainingTime + additionalTime, Duration);

        public void RemoveTime(float additionalTime) => 
            RemainingTime = Mathf.Max(RemainingTime - additionalTime, 0);

        public void Tick(float deltaTime) => 
            RemainingTime -= deltaTime * _speedMultiplier;

        public void Reset() => 
            RemainingTime = Duration;

        public void FinalizeTimer() => 
            _isLooping = false;
    }
}
