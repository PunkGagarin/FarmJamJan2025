using UnityEngine;
using UnityEngine.UI;

namespace Farm.Gameplay.MiniGame
{
    public class Segment : MonoBehaviour
    {
        [SerializeField] private Image _image;

        public void SetSegmentFill(float fill) => 
            _image.fillAmount = fill;
    }
}
