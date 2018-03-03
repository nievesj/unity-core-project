using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zenject.Tests.SceneDecorators
{
    public class Bar : MonoBehaviour
    {
        [Inject]
        public void Construct(Foo foo)
        {
        }
    }
}
