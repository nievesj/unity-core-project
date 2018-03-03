using ModestTree;
using UnityEngine;
using Zenject;

#pragma warning disable 649

namespace Zenject.Tests.Bindings.FromPrefab
{
    public class Gorp : MonoBehaviour
    {
        [Inject]
        string _arg;

        public string Arg
        {
            get { return _arg; }
        }

        [Inject]
        public void Initialize()
        {
            ModestTree.Log.Trace("Received arg '{0}' in Gorp", _arg);
        }
    }
}
