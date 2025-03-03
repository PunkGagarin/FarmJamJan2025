using System.Collections.Generic;

namespace Farm.Utils.Pause
{
    public class PauseService : IPauseHandler
    {
        private readonly List<IPauseHandler> _handlers = new();

        public bool IsPaused { get; private set; }

        public void Register(IPauseHandler handler)
        {
            _handlers.Add(handler);
        }

        public void Unregister(IPauseHandler handler)
        {
            _handlers.Remove(handler);
        }

        public void SetPaused(bool isPaused)
        {
            if (IsPaused == isPaused)
                return;
            
            IsPaused = isPaused;
            
            foreach (var handler in _handlers)
            {
                handler.SetPaused(isPaused);
            }
        }
    }
}