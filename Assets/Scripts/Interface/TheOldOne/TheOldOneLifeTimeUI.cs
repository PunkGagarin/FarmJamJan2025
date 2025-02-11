using System.Collections.Generic;
using Farm.Gameplay;
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
        private const float FULL_CIRCLE_EDGE = 360;
        private TimerHandle _lifeTime;
        private float _fixedAngle;

        public void Initialize(TheOldOneDefinition definition, TimerHandle lifeTime)
        {
            InstantiateMarks(definition);
            _lifeTime = lifeTime;
        }
        
        private void InstantiateMarks(TheOldOneDefinition definition)
        {
            _marks.ForEach(mark => mark.gameObject.SetActive(false));
            
            if (_marks.Count < definition.SatietyPhasesData.Count)
            {
                int marksToSpawn = definition.SatietyPhasesData.Count - _marks.Count;

                for (int i = 0; i < marksToSpawn; i++)
                    _marks.Add(Instantiate(_markPrefab, _timerTransform));
            }


            _fixedAngle = -(definition.SatietyPhasesData[1].PhaseStartTime / definition.LifeTime) * FULL_CIRCLE_EDGE / 2;
            
            for (int i = 0; i < definition.SatietyPhasesData.Count; i++)
            {
                _marks[i].gameObject.SetActive(true);
                _marks[i].gameObject.name = $"mark = {i}";

                float normalizedTime = definition.SatietyPhasesData[i].PhaseStartTime / definition.LifeTime;
                float rotateAmount = -normalizedTime * FULL_CIRCLE_EDGE - _fixedAngle;

                _marks[i].transform.localRotation = Quaternion.Euler(0, 0, rotateAmount);
            }
        }

        private void Update()
        {
            if (_lifeTime != null)
            {
                Vector3 angle = new Vector3(0, 0, FULL_CIRCLE_EDGE * (1 - _lifeTime.Progress) - _fixedAngle);
                _timerTransform.localRotation = Quaternion.Euler(angle);
            }
        }
    }
}