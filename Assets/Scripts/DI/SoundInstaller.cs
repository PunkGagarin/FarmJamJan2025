using Farm.Audio;
using UnityEngine;
using Zenject;
namespace Farm.DI
{
    public class SoundInstaller : MonoInstaller
    {
        [SerializeField] private SoundManager _soundManager;
        [SerializeField] private MasterSoundManager _masterSoundManager;
        [SerializeField] private MusicManager _musicManager;
        [SerializeField] private AmbientSoundPlayer _ambientSoundPlayer;

        public override void InstallBindings()
        {
            Container.Bind<MasterSoundManager>().FromInstance(_masterSoundManager).AsSingle();
            Container.Bind<SoundManager>().FromInstance(_soundManager).AsSingle();
            Container.Bind<MusicManager>().FromInstance(_musicManager).AsSingle();
            Container.Bind<AmbientSoundPlayer>().FromInstance(_ambientSoundPlayer).AsSingle();
        }
    }
}