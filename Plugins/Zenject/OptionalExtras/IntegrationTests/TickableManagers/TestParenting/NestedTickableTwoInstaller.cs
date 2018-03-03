using UnityEngine;

namespace Zenject.Tests.TickableManagers.Parenting
{
    public class NestedTickableTwoInstaller : MonoInstaller
    {
        public GameObject DoublyNestedPrefab;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<NestedTickableTwo>().AsSingle().NonLazy();

            Container.Bind<NestedTickableManagerHolder>().AsSingle();

            Container.Bind<DoublyNestedTickable>()
                .FromSubContainerResolve()
                .ByNewPrefab(DoublyNestedPrefab)
                .AsSingle()
                .NonLazy();
        }
    }
}