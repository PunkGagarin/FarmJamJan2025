using System;
using Farm.Enums;
using Farm.Gameplay.Capsules;
using Farm.Gameplay.Configs;
using Farm.Gameplay.Configs.MiniGame;
using Farm.Gameplay.MiniGame;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

namespace Farm.Interface.Popups
{
    public class CapsulePopup : Popup
    {
        [SerializeField] private Image _embryoView;
        [SerializeField] private Button _closeButton;
        [SerializeField] private MiniGameVisual _miniGame;
        [SerializeField] private Button _createEmbryoButton;
        [SerializeField] private TMP_Text _cost;
        [SerializeField] private TMP_Text _createEmbryoButtonText;
        [SerializeField] private CapsuleInfo _capsuleInfo;
        [Header("Tier buttons")]
        [SerializeField] private Button _minusTier;
        [SerializeField] private Button _plusTier;

        [Inject] private CapsuleConfig _capsuleConfig;
        [Inject] private EmbryoConfig _embryoConfig;
        [Inject] private InventoryUI _inventory;
        
        private int _selectedTier;
        private Capsule _capsule;
        private Embryo _selectedEmbryo;

        private const float PERCENT_VALUE = 100f;
        private const string CREATE_EMBRYO_TEXT = "Create embryo Tier ";

        private int Cost => _embryoConfig.EmbryoTiers[_selectedTier].BaseCost;
        private bool CanBuy => _inventory.CanBuy(Cost);
        
        public void Initialize(Capsule capsule)
        {
            _miniGame.gameObject.SetActive(false);
            _capsule = capsule;
            _selectedEmbryo = capsule.Embryo;
            _createEmbryoButton.interactable = _capsule.Embryo == null;
            _createEmbryoButton.onClick.AddListener(OnBuy);
            _closeButton.onClick.AddListener(Close);
            _capsule.OnEmbryoStateChanged += UpdateEmbryoView;
            _selectedTier = 0;

            UpdatePopupInfo();
        
            UpdateButtonsInfo();
            
            UpdateEmbryoView();
        }
        
        private void UpdatePopupInfo()
        {
            _capsuleInfo.SetCapsuleInfo(_capsule.Tier, _capsuleConfig.BaseHumanChance, _capsuleConfig.BaseAnimalChance, _capsuleConfig.BaseFishChance);

            _capsuleInfo.SetEmbryoInfo(_selectedEmbryo);
        }

        public override void Close()
        {
            _createEmbryoButton.onClick.RemoveListener(ShowMiniGame);
            _closeButton.onClick.RemoveListener(Close);
            _capsule.OnEmbryoStateChanged -= UpdateEmbryoView;
            _createEmbryoButton.interactable = false;

            base.Close();
        }

        private void PlusTier()
        {
            _selectedTier = Mathf.Clamp(_selectedTier + 1, 0, _capsule.Tier);
            UpdateButtonsInfo();
            UpdatePopupInfo();
        }
        
        private void MinusTier()
        {
            _selectedTier = Mathf.Clamp(_selectedTier - 1, 0, _capsule.Tier);
            UpdateButtonsInfo();
            UpdatePopupInfo();
        }
        
        private void ShowMiniGame()
        {
            _closeButton.gameObject.SetActive(false);
            _miniGame.gameObject.SetActive(true);
            _miniGame.Initialize(_selectedTier, _embryoConfig.EmbryoTiers.Count);
            _miniGame.OnMiniGameEnds += StartEmbryoProcess;
        }

        private void StartEmbryoProcess(MiniGameEffect miniGameResult)
        {
            Debug.Log(miniGameResult == null ? "Mini game result: none" : $"Mini game result: {miniGameResult.BuffType} : {miniGameResult.Value}");

            _closeButton.gameObject.SetActive(true);
            _miniGame.gameObject.SetActive(false);
            _miniGame.OnMiniGameEnds -= StartEmbryoProcess;
            if (miniGameResult != null)
                switch (miniGameResult.BuffType)
                {
                    case BuffType.UpdateTier:
                        int newTier = _selectedTier + miniGameResult.Value;
                        _selectedEmbryo.EnergyValue = _embryoConfig.EmbryoTiers[newTier].BaseEnergyAmount;
                        _selectedEmbryo.TimeToGrowth = _embryoConfig.EmbryoTiers[newTier].BaseGrowthSpeed;
                        _selectedEmbryo.StarvationValue = _embryoConfig.EmbryoTiers[newTier].BaseFoodAmount;
                        break;
                    case BuffType.GrowthSpeed:
                        _selectedEmbryo.TimeToGrowth += _selectedEmbryo.TimeToGrowth * (miniGameResult.Value / PERCENT_VALUE);
                        break;
                    case BuffType.EnergyAmount:
                        _selectedEmbryo.EnergyValue += Mathf.RoundToInt(_selectedEmbryo.EnergyValue * (miniGameResult.Value / PERCENT_VALUE));
                        break;
                    case BuffType.SatietyAmount:
                        _selectedEmbryo.StarvationValue += Mathf.RoundToInt(_selectedEmbryo.StarvationValue * (miniGameResult.Value / PERCENT_VALUE));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            
            UpdatePopupInfo();
            _capsule.StartEmbryoProcess(_selectedEmbryo);
            _embryoView.sprite = _capsule.Embryo.Image;
        }
        
        private void CreateEmbryo()
        {
            _createEmbryoButton.onClick.RemoveListener(OnBuy);

            _createEmbryoButton.interactable = false;
            EmbryoType type = GetEmbryoType();
            var embryoTier = _embryoConfig.EmbryoTiers[_selectedTier];
            _selectedEmbryo = new Embryo(type, embryoTier.BaseFoodAmount, embryoTier.BaseGrowthSpeed, embryoTier.BaseEnergyAmount, _embryoConfig.GetSprite(type));
            UpdatePopupInfo();
            UpdateEmbryoView();
        }

        private EmbryoType GetEmbryoType()
        {
            //TODO: изменить шансы выпадения в зависимости от модулей капсулы 
            float totalChance = 0;
            totalChance += _capsuleConfig.BaseHumanChance;
            float humanProcChance = totalChance;
            totalChance += _capsuleConfig.BaseAnimalChance;
            float animalProcChance = totalChance;
            totalChance += _capsuleConfig.BaseFishChance;

            var chance = Random.Range(0, totalChance);
            
            if (chance < humanProcChance)
                return EmbryoType.Human;

            if (chance < animalProcChance)
                return EmbryoType.Animal;

            return EmbryoType.Fish;
        }

        private void UpdateEmbryoView()
        { 
            _embryoView.gameObject.SetActive(_selectedEmbryo != null);
            _embryoView.sprite = _selectedEmbryo?.Image;
        }
        
        [UsedImplicitly]
        public void MouseEnter()
        {
            if (Cost == 0)
                return;
            if (CanBuy)
                _inventory.ShowCanBuy();
            else
                _inventory.ShowCanNotBuy();
        }

        [UsedImplicitly]
        public void MouseExit()
        {
            _inventory.ResetColor();
        }

        private void OnBuy()
        {
            if (_selectedTier == 0)
            {
                CreateEmbryo();
                ShowMiniGame();
            }
            else
            {
                if (CanBuy)
                {
                    _inventory.CurrentEnergy -= Cost;
                    _inventory.ResetColor();
                    CreateEmbryo();
                    ShowMiniGame();
                }
                else
                {
                    _inventory.ShakeCanNotBuy();
                }
            }
        }

        private void UpdateButtonsInfo()
        {
            _createEmbryoButtonText.text = $"{CREATE_EMBRYO_TEXT}{_selectedTier}";
            _cost.gameObject.SetActive(Cost > 0);
            _cost.text = Cost.ToString();
        }

        private void Awake()
        {
            _plusTier.onClick.AddListener(PlusTier);
            _minusTier.onClick.AddListener(MinusTier);
        }

        private void OnDestroy()
        {
            _plusTier.onClick.RemoveListener(PlusTier);
            _minusTier.onClick.RemoveListener(MinusTier);
        }
    }
}
