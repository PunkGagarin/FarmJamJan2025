using System.Collections.Generic;
using Farm.Gameplay.Definitions;
using UnityEngine;

namespace Farm.Gameplay.Repositories
{
    public abstract class Repository<T> : ScriptableObject where T : Definition
    {
        [SerializeField] private List<T> _definitions;
        
        public List<T> Definitions => _definitions;
    }
}
