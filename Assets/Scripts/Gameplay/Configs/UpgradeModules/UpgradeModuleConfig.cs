using System.Collections.Generic;
using UnityEngine;

namespace Farm.Gameplay.Configs.UpgradeModules
{
    [CreateAssetMenu(fileName = "Upgrade Module Config", menuName = "Game Resources/Configs/Upgrade Module")]
    public class UpgradeModuleConfig : ScriptableObject
    {
        [SerializeField] private UpgradeModulePool _pool;
        [SerializeField] private float _oneStatPercent;
        [SerializeField] private float _twoStatPercent;
        [SerializeField] private float _threeStatPercent;
        [SerializeField] private float _negativePercentForTwo;
        [SerializeField] private float _negativePercentForThree;
        [SerializeField] private float _canBeThreeStatCount;
        [SerializeField] private List<Sprite> _icons;

        public List<UpgradeModuleStat> Stats => _pool.Stats;

        public float CanBeThreeStatCount => _canBeThreeStatCount;

        public UpgradeModule GetRandomModule(bool canBeThree)
        {
            var hasNegativeStat = false;
            var maxStatsCount = Random.Range(1, (canBeThree ? 3 : 2) + 1);
            var finalStats = new List<UpgradeModuleStat>();
            var shuffledStatsPool = new List<UpgradeModuleStat>(Stats);
            
            Shuffle(shuffledStatsPool);
            var moduleRandomizedStats = shuffledStatsPool.GetRange(0, maxStatsCount);
            for (var i = 0; i < moduleRandomizedStats.Count; i++)
            {
                var stat = moduleRandomizedStats[i];
                stat.SetValue(Random.Range(stat.MinValue, stat.MaxValue));

                if (!hasNegativeStat)
                {
                    switch (i)
                    {
                        case 1 when Random.Range(0, 100) <= _negativePercentForTwo:
                        case 2 when Random.Range(0, 100) <= _negativePercentForThree:
                            stat.SetValue(stat.Value * -1);
                            hasNegativeStat = true;
                            break;
                    }   
                }

                finalStats.Add(stat);
            }

            return new UpgradeModule(finalStats, _icons[Random.Range(0, _icons.Count)]); // todo ask GD about icon;
        }

        private static void Shuffle<T>(IList<T> ts) {
            var count = ts.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i) {
                var r = Random.Range(i, count);
                (ts[i], ts[r]) = (ts[r], ts[i]);
            }
        }
    }
    
}