using UnityEngine;
using Zenject;

namespace Zenject.Tests.Factories.BindFactory
{
    public interface IFoo
    {
    }

    public class IFooFactory : Factory<IFoo>
    {
    }

    public class Foo : MonoBehaviour, IFoo
    {
        public class Factory : Factory<Foo>
        {
        }
    }
}
