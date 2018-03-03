using UnityEngine;
using Zenject;

namespace Zenject.Tests.Factories.BindFactory
{
    //[CreateAssetMenu(fileName = "Bar", menuName = "Installers/Bar")]
    public class Bar : ScriptableObject
    {
        public class Factory : Factory<Bar>
        {
        }
    }
}

