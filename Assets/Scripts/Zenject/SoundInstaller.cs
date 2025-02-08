using Audio;
using UnityEngine;
using Zenject;

public class SoundInstaller : MonoInstaller
{
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private MasterSoundManager masterSoundManager;
    [SerializeField] private MusicManager musicManager;

    public override void InstallBindings()
    {
        Container.Bind<MasterSoundManager>().FromInstance(masterSoundManager).AsSingle();
        Container.Bind<SoundManager>().FromInstance(soundManager).AsSingle();
        Container.Bind<MusicManager>().FromInstance(musicManager).AsSingle();
    }
}