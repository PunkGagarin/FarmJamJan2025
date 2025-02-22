﻿using DG.Tweening;
using Farm.Gameplay;
using Farm.Gameplay.Definitions;
using Farm.Utils.Timer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.Interface.TheOldOne
{
    public class TheOldOneUI : MonoBehaviour
    {
        [SerializeField] private TheOldOneLifeTimeUI _theOldOneLifeTimeUI;
        [SerializeField] private Image _satietyBar;
        [SerializeField] private TMP_Text _satietyAmount;
        [SerializeField] private float _timeToUpdateBar;
        [SerializeField] private float _timeToUpdateColor;
        [SerializeField] private Color _goodColor;
        [SerializeField] private Color _badColor;
        
        private int _maxPhases;

        public void Initialize(TheOldOneDefinition definition, TimerHandle lifeTime)
        {
            _maxPhases = definition.SatietyPhasesData.Count;
            _theOldOneLifeTimeUI.Initialize(definition, lifeTime);
            UpdateSatietyBar(definition.StartSatiety, definition.MaxSatiety, true);
        }
        
        public void PhaseChanged(int phaseNum)
        {
            Color newColor = Color.Lerp(_goodColor, _badColor, (float)phaseNum / _maxPhases);

            _satietyBar.DOColor(newColor, _timeToUpdateColor);
        }

        public void UpdateSatietyBar(float current, float max) =>
            UpdateSatietyBar(current, max, false);

        private void UpdateSatietyBar(float current, float max, bool instant)
        {
            float progress = current / max;
            _satietyAmount.text = $"{current} / {max}";
            _satietyBar.DOFillAmount(progress, instant ? 0 : _timeToUpdateBar);
        }
    }
}
