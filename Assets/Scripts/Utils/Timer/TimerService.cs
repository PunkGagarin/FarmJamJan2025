using System;
using System.Collections.Generic;
using Farm.Utils.Pause;
using UnityEngine;
using Zenject;

namespace Farm.Utils.Timer
{
    public class TimerService : ITickable, IPauseHandler, IDisposable
    {
        private readonly List<TimerHandle> _timers = new();
        private readonly HashSet<TimerHandle> _timersToRemove = new();
        private readonly PauseService _pauseService;
        private bool _isPaused;

        [Inject]
        public TimerService(PauseService pauseService)
        {
            _pauseService = pauseService;
            pauseService.Register(this);
        }

        public TimerHandle AddTimer(float duration, Action callback)
        {
            var timer = new TimerHandle(duration, callback);
            _timers.Add(timer);
            return timer;
        }

        public void RemoveTimer(TimerHandle timerToRemove)
        {
            if (_timers.Contains(timerToRemove))
                _timersToRemove.Add(timerToRemove);
        }

        public void ClearAllTimers()
        {
            _timers.Clear();
            _timersToRemove.Clear();
        }

        public void Tick()
        {
            if (_isPaused) 
                return;

            float deltaTime = Time.deltaTime;
            for (int i = _timers.Count - 1; i >= 0; i--)
            {
                var timer = _timers[i];
               
                if (_timersToRemove.Contains(timer)) 
                    continue;

                timer.Tick(deltaTime);

                if (timer.RemainingTime <= 0)
                {
                    timer.Callback?.Invoke();
                    _timersToRemove.Add(timer);
                }
            }
            
            if (_timersToRemove.Count > 0) 
            {
                _timers.RemoveAll(t => _timersToRemove.Contains(t));
                _timersToRemove.Clear();
            }
        }

        public void SetPaused(bool isPaused)
        {
            _isPaused = isPaused;
        }

        public void Dispose()
        {
            ClearAllTimers();
            _pauseService.Unregister(this);
        }
    }
}
