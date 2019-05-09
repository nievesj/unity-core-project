using Core.Factory;
using Core.Services;

public abstract class CoreBehaviourPooled : CoreBehaviour, IPoolElement
{
    public abstract void PoolElementWakeUp();
    public abstract void PoolElementSleep();
}