using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Farm
{
    public class TestTimer : MonoBehaviour
    {

        private TextMeshProUGUI _textMeshPro;

        private float _currentTime;

        // Start is called before the first frame update
        void Start()
        {
            _textMeshPro = GetComponent<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update()
        {
            _currentTime += Time.deltaTime;
            _textMeshPro.text = _currentTime.ToString("F2");
        }
    }
}