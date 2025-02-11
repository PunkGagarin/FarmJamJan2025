using UnityEngine;

namespace Farm.Gameplay.Definitions
{
    [CreateAssetMenu(fileName = "Capsule Definition", menuName = "Game Resources/Definition/Capsule")]
    public class CapsuleDefinition : Definition
    {
        [SerializeField, Tooltip("1 - значит капсула развивает эмбрион со скорость 100%")] private float _growthSpeed;
        [SerializeField, Tooltip("Шанс на создание определяется по сумме всех шансов, т.е. шансы не ограничены 100%.")] private EmbryoChances _baseEmbryoChances;
        [SerializeField, Range(0,2)] private int _upgradeSlotsAmount;
        [SerializeField, Tooltip("Стоимость равная 0 значит что капсула сразу открыта")] private int _costToUnlock;

        public float GrowthSpeed => _growthSpeed;
        public EmbryoChances BaseEmbryoChances => _baseEmbryoChances;
        public int UpgradeSlotsAmount => _upgradeSlotsAmount;
        public int CostToUnlock => _costToUnlock;
    }
}
