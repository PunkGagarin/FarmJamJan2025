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
        
        public int AllowTiers
        {
            get
            {
                if (_feelImage.fillAmount < _miniGameConfig.Tier1SpeedCap) return 1;
                if (_feelImage.fillAmount < _miniGameConfig.Tier2SpeedCap) return 2;
                return 3;
            }
        }

        public void Init(MiniGameConfig miniGameConfig)
        {
            _miniGameConfig = miniGameConfig;

            SetupPlanks();
            SetSpeed(0);
        }
        
        private void SetupPlanks()
        {
            var position = _tier1Plank.rectTransform.localPosition;
            position.y = _selfRectTransform.rect.height * _miniGameConfig.Tier1SpeedCap;
            _tier1Plank.rectTransform.anchoredPosition = position;
            position.y = _selfRectTransform.rect.height * _miniGameConfig.Tier2SpeedCap;
            _tier2Plank.rectTransform.anchoredPosition = position;
        }
        
        public void SetSpeed(float speed) =>
            _feelImage.fillAmount = (speed - _miniGameConfig.StartSpeed) / (_miniGameConfig.MaxSpeed - _miniGameConfig.StartSpeed);
    }
}
