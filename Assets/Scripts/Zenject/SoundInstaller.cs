using Audio;
using UnityEngine;
using Zenject;

public class SoundInstaller : MonoInstaller
{
    [SerializeField] private SoundManager _soundManager;
    [SerializeField] private MasterSoundManager _masterSoundManager;
    [SerializeField] private MusicManager _musicManager;

    public override void InstallBindings()
    {
        Container.Bind<MasterSoundManager>().FromInstance(_masterSoundManager).AsSingle();
        Container.Bind<SoundManager>().FromInstance(_soundManager).AsSingle();
        Container.Bind<MusicManager>().FromInstance(_musicManager).AsSingle();
    }
}