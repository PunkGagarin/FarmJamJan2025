using Farm.Interface;
using UnityEngine;
using Zenject;

namespace Farm.DI
{
    public class InventoryInstaller : MonoInstaller
    {
        [SerializeField] private InventoryUI _inventory;

        public override void InstallBindings()
        {
            Container
                .Bind<InventoryUI>()
                .FromInstance(_inventory)
                .AsSingle()
                .NonLazy();
        }
    }
}
