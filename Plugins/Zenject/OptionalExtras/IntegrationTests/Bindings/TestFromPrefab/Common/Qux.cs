using ModestTree;
using UnityEngine;
using Zenject;

#pragma warning disable 649

namespace Zenject.Tests.Bindings.FromPrefab
{
    public class Qux : MonoBehaviour
    {
        [Inject]
        int _arg;

        [Inject]
        public void Initialize()
        {
            ModestTree.Log.Trace("Received arg '{0}' in Qux", _arg);
        }
    }
}
