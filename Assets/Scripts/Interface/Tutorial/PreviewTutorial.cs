using DG.Tweening;
using Farm.Audio;
using Farm.Gameplay;
using Farm.Utils.Pause;
using Farm.Utils.Timer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Farm.Interface.Tutorial
{
    public class PreviewTutorial : MonoBehaviour
    {
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private MainTutorial _mainTutorial;
        [SerializeField] private TMP_Text _firstText;
        [SerializeField] private TMP_Text _secondText;
        [SerializeField] private TMP_Text _thirdText;

        [SerializeField] private Transform _curtainRight;
        [SerializeField] private Transform _curtainLeft;
        [SerializeField] private GameObject _mockImage;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Image _bg;
        [SerializeField] private Color _transparentBlack;
        [SerializeField] private Color _black;

        [Inject] private WriterService _writerService;
        [Inject] private TimerService _timerService;
        [Inject] private PauseService _pauseService;
        [Inject] private MasterSoundManager _masterSoundManager;
        
        private string _text1,_text2,_text3;
        private bool _continueAvailable;
        private TimerHandle _timer;
        private float _tempVolume;

        private void Awake()
        {
            _tempVolume = _masterSoundManager.Volume;
            _masterSoundManager.ChangeVolume(0, 0);
            _pauseService.SetPaused(false);
            _curtainLeft.gameObject.SetActive(false);
            _curtainRight.gameObject.SetActive(false);
            
            _text1 = _firstText.text;
            _text2 = _secondText.text;
            _text3 = _thirdText.text;
            
            _firstText.text = string.Empty;
            _secondText.text = string.Empty;
            _thirdText.text = string.Empty;
        }
        
        private void Start()
        {
            _continueButton.onClick.AddListener(_writerService.StopWriter);
            _timer = _timerService.AddTimer(2f, ShowSecondText);
            
            _writerService.WriteText(_firstText, _text1);
        }
        
        private void ShowSecondText()
        {
            _timer?.EarlyComplete();
            _timer = null;

            _timer = _timerService.AddTimer(2f, ShowCurtainAnimation);
            _writerService.WriteText(_secondText, _text2);
        }
        
        private void ShowCurtainAnimation()
        {
            _timer?.EarlyComplete();
            _timer = null;
            _continueButton.onClick.RemoveListener(_writerService.StopWriter);
            
            _firstText.gameObject.SetActive(false);
            _secondText.gameObject.SetActive(false);
            _curtainLeft.gameObject.SetActive(true);
            _curtainRight.gameObject.SetActive(true);
            _bg.color = _transparentBlack;
            _continueButton.onClick.AddListener(GoodLuck);

            _curtainLeft.DOLocalMoveX(-1000, 2f);
            _curtainRight.DOLocalMoveX(1000, 2f).OnComplete(() =>
            {
                _curtainLeft.DOLocalMoveX(0, 2f);
                _curtainRight.DOLocalMoveX(0, 2f).OnComplete(GoodLuck);
            });

            void GoodLuck()
            {
                _continueButton.onClick.RemoveListener(GoodLuck);
                
                _bg.color = _black;
                _curtainLeft.gameObject.SetActive(false);
                _curtainRight.gameObject.SetActive(false);

                _continueButton.onClick.AddListener(_writerService.StopWriter);
                _timer = _timerService.AddTimer(2f, ShowGoodLuck);
                _writerService.WriteText(_thirdText, _text3);
            }
        }
        
        private void ShowGoodLuck()
        {
            _timer?.EarlyComplete();
            _timer = null;
            _curtainLeft.DOKill();
            _curtainRight.DOKill();
            _continueButton.onClick.RemoveListener(_writerService.StopWriter);

            _continueButton.onClick.AddListener(StartNextPhase);

            _timer = _timerService.AddTimer(2f, StartNextPhase);
        }
        
        private void StartNextPhase()
        {
            _masterSoundManager.ChangeVolume(_tempVolume, _tempVolume);
            _timer?.EarlyComplete();
            _timer = null;
            _continueButton.onClick.RemoveListener(StartNextPhase);
            _curtainLeft.gameObject.SetActive(false);
            _curtainRight.gameObject.SetActive(false);
            _firstText.gameObject.SetActive(false);
            _secondText.gameObject.SetActive(false);
            _thirdText.gameObject.SetActive(false);
            _mockImage.gameObject.SetActive(false);
            
            _continueButton.onClick.AddListener(() => _bg.DOKill());
            
            _gameManager.SetupOldOne();
            
            _bg.DOColor(new Color(0,0,0,0), 1f).OnKill(() =>
            {
                _continueButton.onClick.RemoveAllListeners();
                gameObject.SetActive(false);
                _mainTutorial.StartTutorial();
            });
        }
        
        private void OnDestroy()
        {
            _continueButton.onClick.RemoveAllListeners();
            _writerService.OnTextWritten -= ShowSecondText;
            _writerService.OnTextWritten -= ShowCurtainAnimation;
            _writerService.OnTextWritten -= ShowGoodLuck;
        }
    }
}
