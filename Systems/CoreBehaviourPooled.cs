using Core.Services;
using Core.Systems;

public abstract class CoreBehaviourPooled : CoreBehaviour, IInitializablePoolElement
{
    public abstract void PoolElementWakeUp();
    public abstract void PoolElementSleep();
}