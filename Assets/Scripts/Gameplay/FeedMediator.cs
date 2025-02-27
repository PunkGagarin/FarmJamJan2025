using Farm.Enums;
using UnityEngine;

namespace Farm.Gameplay
{
    public class FeedMediator
    {
        private TheOldOne _theOldOne;

        public void SetupTheOldOne(TheOldOne theOldOne)
        {
            _theOldOne = theOldOne;
        }

        public void FeedTheOldOne(int amount, EmbryoType embryoType)
        {
            if (_theOldOne == null)
            {
                Debug.LogError($"Trying to feed the old one that not exist!");
                return;
            }
            
            _theOldOne.Feed(amount, embryoType);
        }
    }
}
