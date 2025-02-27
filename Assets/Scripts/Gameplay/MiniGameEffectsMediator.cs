using System;
using Farm.Gameplay.Configs.MiniGame;
using UnityEngine;

namespace Farm.Gameplay
{
    public class MiniGameEffectsMediator
    {
        public event Action<MiniGameEffect> OnEffectChanged;
        
        public void SetMiniGameEffect(MiniGameEffect miniGameEffect)
        {
            Debug.Log(miniGameEffect == null ? "mini game effect: null" : $"mini game effect: {miniGameEffect.BuffType}, {miniGameEffect.Value}");
            OnEffectChanged?.Invoke(miniGameEffect);
        }
    }
}
