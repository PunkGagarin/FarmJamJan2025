using System.Linq;
using Farm.Enums;
using Farm.Gameplay.Definitions;
using UnityEngine;

namespace Farm.Gameplay.Repositories
{
    [CreateAssetMenu(fileName = "Embryo Repository", menuName = "Game Resources/Repository/Embryo")]
    public class EmbryoRepository : Repository<EmbryoDefinition>
    {
        public EmbryoDefinition GetEmbryoByType(EmbryoType embryoType) => 
            Definitions.FirstOrDefault(def => def.EmbryoType == embryoType);
    }
}
