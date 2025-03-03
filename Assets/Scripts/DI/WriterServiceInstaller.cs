using Farm.Interface;
using UnityEngine;
using Zenject;

namespace Farm.DI
{
    public class WriterServiceInstaller : MonoInstaller
    {
        [SerializeField, Range(0.01f, 0.05f)] private float _writerCooldown = 0.025f;

        public override void InstallBindings()
        {
            Container.Bind<WriterService>().FromNewComponentOnNewGameObject().AsSingle().WithArguments(_writerCooldown);
        }
    }
}
