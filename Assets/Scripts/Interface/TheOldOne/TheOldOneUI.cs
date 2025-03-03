using DG.Tweening;
using Farm.Gameplay.Definitions;
using Farm.Interface.Popups;
using Farm.Utils.Timer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Farm.Interface.TheOldOne
{
    public class TheOldOneUI : MonoBehaviour
    {
        [SerializeField] private TheOldOneLifeTimeUI _theOldOneLifeTimeUI;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private Image _satietyBar;
        [SerializeField] private TMP_Text _satietyAmount;
        [SerializeField] private float _timeToUpdateBar;
        [SerializeField] private float _timeToUpdateColor;
        [SerializeField] private Color _goodColor;
        [SerializeField] private Color _badColor;
        [SerializeField] private Button _questButton;
        [SerializeField] private FoodInfo _foodInfo;
        [SerializeField] private Image _cautionImage;
        [SerializeField] private float _blinkingCycleTime;
        [SerializeField] private Button _pauseButton;
        [Inject] private PopupManager _popupManager;
        private Sequence _blinkingTween;

        private int _maxPhases;

        public void Initialize(TheOldOneDefinition definition, TimerHandle lifeTimer)
        {
            _name.text = definition.HeaderInfo;
            _maxPhases = definition.SatietyPhasesData.Count;
            _theOldOneLifeTimeUI.Initialize(definition, lifeTimer);
            _foodInfo.SetFoodInfo(definition);
            UpdateSatietyBar(definition.StartSatiety, definition.MaxSatiety, true);
            _cautionImage.DOFade(0, 0);
        }

        public void PhaseChanged(int phaseNum)
        {
            Color newColor = Color.Lerp(_goodColor, _badColor, (float)phaseNum / _maxPhases);

            _satietyBar.DOColor(newColor, _timeToUpdateColor);
        }

        public void UpdateSatietyBar(float current, float max) =>
            UpdateSatietyBar(current, max, false);

        public void OnRampageStateChanged(bool inRampage)
        {
            if (inRampage)
            {
                _blinkingTween = DOTween.Sequence();

                _blinkingTween.Append(_cautionImage.DOFade(1, _blinkingCycleTime));
                _blinkingTween.Append(_cautionImage.DOFade(0, _blinkingCycleTime)).OnComplete(() => _blinkingTween.Restart());

                _blinkingTween.Restart();
            }
            else
            {
                _blinkingTween.Kill();
            }
        }

        public void TutorialComplete() => 
            _theOldOneLifeTimeUI.TutorialComplete();
        
        private void UpdateSatietyBar(float current, float max, bool instant)
        {
            float progress = current / max;
            _satietyAmount.text = $"{current} / {max}";
            _satietyBar.DOFillAmount(progress, instant ? 0 : _timeToUpdateBar);
        }

        private void Awake() => _pauseButton.onClick.AddListener(OnPauseClicked);

        private void OnPauseClicked() => _popupManager.OpenPause();

        private void OnDestroy()
        {
            _pauseButton.onClick.RemoveListener(OnPauseClicked);
            _cautionImage.DOKill();
            _blinkingTween.Kill();
        }
        
    }
}
