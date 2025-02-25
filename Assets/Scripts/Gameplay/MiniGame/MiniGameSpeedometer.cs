using Farm.Gameplay.Configs.MiniGame;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.Gameplay.MiniGame
{
    public class MiniGameSpeedometer : MonoBehaviour
    {
        [SerializeField] private RectTransform _selfRectTransform;
        [SerializeField] private Image _feelImage;
        [SerializeField] private Image _tier1Plank, _tier2Plank;

        private MiniGameConfig _miniGameConfig;

        public void Init(MiniGameConfig miniGameConfig)
        {
            _miniGameConfig = miniGameConfig;

            SetupPlanks();
            SetSpeed(0);
        }
        
        private void SetupPlanks()
        {
            var position = _tier1Plank.rectTransform.localPosition;
            position.y = _selfRectTransform.rect.height * _miniGameConfig.LowRiskStats.Speed / _miniGameConfig.HighRiskStats.Speed;
            _tier1Plank.rectTransform.anchoredPosition = position;
            position.y = _selfRectTransform.rect.height * _miniGameConfig.MediumRiskStats.Speed / _miniGameConfig.HighRiskStats.Speed;
            _tier2Plank.rectTransform.anchoredPosition = position;
        }
        
        public void SetSpeed(float speed) =>
            _feelImage.fillAmount = speed / _miniGameConfig.HighRiskStats.Speed;
    }
}
