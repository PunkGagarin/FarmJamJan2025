using System.Collections.Generic;
using Farm.Gameplay.Definitions;
using Farm.Utils.Timer;
using UnityEngine;

namespace Farm.Interface.TheOldOne
{
    public class TheOldOneLifeTimeUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _markPrefab;
        [SerializeField] private Transform _timerTransform;

        private List<RectTransform> _marks = new List<RectTransform>();
        private TimerHandle _lifeTime;
        private float _circleSpinTime;
        
        private const float FULL_CIRCLE_EDGE = 360f;
        private const float POINTER_SHIFT = 90f;

        public void Initialize(TheOldOneDefinition definition, TimerHandle lifeTimer)
        {
            InstantiateMarks(definition);
            _lifeTime = lifeTimer;
            _circleSpinTime = FULL_CIRCLE_EDGE / definition.LifeTime;
        }
        
        private void InstantiateMarks(TheOldOneDefinition definition)
        {
            _marks.ForEach(mark => mark.gameObject.SetActive(false));
            
            if (_marks.Count < definition.SatietyPhasesData.Count - 1)
            {
                int marksToSpawn = definition.SatietyPhasesData.Count - _marks.Count - 1;

                for (int i = 0; i < marksToSpawn; i++)
                    _marks.Add(Instantiate(_markPrefab, _timerTransform));
            }
            
            for (int i = 0; i < definition.SatietyPhasesData.Count - 1; i++)
            {
                _marks[i].gameObject.SetActive(true);
                _marks[i].gameObject.name = $"mark = {i}";

                float normalizedTime = definition.SatietyPhasesData[i + 1].PhaseStartTime / definition.LifeTime;
                float rotateAmount = normalizedTime * FULL_CIRCLE_EDGE + POINTER_SHIFT;

                _marks[i].transform.localRotation = Quaternion.Euler(0, 0, rotateAmount);
            }
        }

        private void Update()
        {
            if (_lifeTime.IsActive)
                _timerTransform.Rotate(Vector3.forward, -_circleSpinTime * Time.deltaTime);
        }
    }
}