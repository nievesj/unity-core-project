using Core.Factory;
using Core.Services;

public abstract class CoreBehaviourPooled : CoreBehaviour, IInitializablePoolElement
{
    public abstract void PoolElementWakeUp();
    public abstract void PoolElementSleep();
}