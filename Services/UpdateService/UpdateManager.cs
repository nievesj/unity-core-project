using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Services.UpdateManager
{
    [Obsolete]
    public class UpdateManager : MonoBehaviour
    {
        private List<BehaviourDelegateType> update;
        private List<BehaviourDelegateType> fixedUpdate;
        private List<BehaviourDelegateType> lateUpdate;

        private void Awake()
        {
            update = new List<BehaviourDelegateType>();
            fixedUpdate = new List<BehaviourDelegateType>();
            lateUpdate = new List<BehaviourDelegateType>();
        }

        private void Update()
        {
            if (update == null || update.Count <= 0) return;

            for (var i = 0; i < update.Count; i++)
            {
                if (!update[i].behaviour)
                    update[i].updateMethod();
            }
        }

        private void FixedUpdate()
        {
            if (fixedUpdate == null || fixedUpdate.Count <= 0) return;

            for (var i = 0; i < fixedUpdate.Count; i++)
            {
                if (fixedUpdate[i].behaviour)
                    fixedUpdate[i].updateMethod();
            }
        }

        private void LateUpdate()
        {
            if (lateUpdate == null || lateUpdate.Count <= 0) return;

            for (var i = 0; i < lateUpdate.Count; i++)
            {
                if (lateUpdate[i].behaviour)
                    lateUpdate[i].updateMethod();
            }
        }

        public void Attach(BehaviourDelegateType behaviourDelegateType)
        {
            if (!Contains(behaviourDelegateType))
            {
                switch (behaviourDelegateType.type)
                {
                    case UpdateType.Update:
                        update.Add(behaviourDelegateType);
                        break;

                    case UpdateType.FixedUpdate:
                        fixedUpdate.Add(behaviourDelegateType);
                        break;

                    case UpdateType.LateUpdate:
                        lateUpdate.Add(behaviourDelegateType);
                        break;
                }
            }
        }

        public void Detach(BehaviourDelegateType behaviourDelegateType)
        {
            if (Contains(behaviourDelegateType))
            {
                switch (behaviourDelegateType.type)
                {
                    case UpdateType.Update:
                        update.Remove(behaviourDelegateType);
                        break;

                    case UpdateType.FixedUpdate:
                        fixedUpdate.Remove(behaviourDelegateType);
                        break;

                    case UpdateType.LateUpdate:
                        lateUpdate.Remove(behaviourDelegateType);
                        break;
                }
            }
        }

        private bool Contains(BehaviourDelegateType behaviour)
        {
            switch (behaviour.type)
            {
                case UpdateType.Update:
                    return update.Contains(behaviour);

                case UpdateType.FixedUpdate:
                    return fixedUpdate.Contains(behaviour);

                case UpdateType.LateUpdate:
                    return lateUpdate.Contains(behaviour);
            }

            return false;
        }
    }
}