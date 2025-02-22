﻿using System;
using UnityEngine;

namespace Farm.Utils.Timer
{
    public class TimerHandle : IDisposable
    {
        private float _speedMultiplier = 1f;
        private bool _isLooping;

        public float RemainingTime { get; internal set; }
        public float Duration { get; }
        public bool IsLooping => _isLooping;
        public Action OnTimerExpire { get; private set; }

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
        
        public void EarlyComplete() => 
            RemainingTime = 0;

        public void AddTime(float additionalTime) => 
            RemainingTime = Mathf.Min(RemainingTime + additionalTime, Duration);

        public void RemoveTime(float additionalTime) => 
            RemainingTime = Mathf.Max(RemainingTime - additionalTime, 0);

        public void Tick(float deltaTime) => 
            RemainingTime -= deltaTime * _speedMultiplier;

        public void Reset(bool isHard = false)
        {
            if (isHard)
                OnTimerExpire = null;
            RemainingTime = Duration;
        }

        public void FinalizeTimer() => 
            _isLooping = false;

        public void Dispose()
        {
            OnTimerExpire = null;
        }
    }
}
