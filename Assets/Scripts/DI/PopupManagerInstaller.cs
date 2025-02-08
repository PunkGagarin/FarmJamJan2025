using Farm.Interface.Popups;
using UnityEngine;
using Zenject;

namespace Farm.DI
{
    public class PopupManagerInstaller : MonoInstaller
    {
        [SerializeField] private PopupManager _popupManagerPrefab;
        
        public override void InstallBindings()
        {
            Container
                .Bind<PopupManager>()
                .FromInstance(_popupManagerPrefab)
                .AsSingle()
                .NonLazy();
        }
    }
}
