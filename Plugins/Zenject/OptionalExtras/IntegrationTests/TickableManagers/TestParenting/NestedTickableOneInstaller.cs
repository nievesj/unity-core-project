namespace Zenject.Tests.TickableManagers.Parenting
{
    public class NestedTickableOneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<NestedTickableOne>().AsSingle().NonLazy();
        }
    }
}