namespace Zenject.Tests.TickableManagers.Parenting
{
    public class NestedTickableTwo : ITickable
    {
        public static int TickCount = 0;

        public void Tick()
        {
            TickCount++;
        }
    }
}