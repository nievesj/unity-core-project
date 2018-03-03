namespace Zenject.Tests.TickableManagers.Parenting
{
    public class DoublyNestedTickableInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<DoublyNestedTickable>().AsSingle().NonLazy();
        }
    }
}