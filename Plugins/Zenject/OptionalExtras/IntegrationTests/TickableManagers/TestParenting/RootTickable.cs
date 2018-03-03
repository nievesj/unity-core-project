namespace Zenject.Tests.TickableManagers.Parenting
{
    public class RootTickable: ITickable
    {
        public static int TickCount = 0;

        public void Tick()
        {
            TickCount++;
        }
    }
}