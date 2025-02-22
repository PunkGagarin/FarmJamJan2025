using Farm.Gameplay.Configs.MiniGame;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.Gameplay.MiniGame
{
    public class Segment : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Color _positiveColor;
        [SerializeField] private Color _negativeColor;
        
        public float StartAngle { get; private set; }
        public float Angle { get; private set; }
        public MiniGameEffect MiniGameEffect {get; private set;}

        public void SetSegmentAngle(float angle)
        {
            Angle = angle;
            _image.fillAmount = angle / 360f;
        }
        
        public void SetEffect(MiniGameEffect miniGameEffect)
        {
            MiniGameEffect = miniGameEffect;
            _image.color = miniGameEffect.Value > 0 ? _positiveColor : _negativeColor;
        }
        public void SetStartAngle(float startAngle)
        {
            transform.localRotation = Quaternion.Euler(0, 0, -startAngle);
            StartAngle = startAngle;
        }
    }
}
