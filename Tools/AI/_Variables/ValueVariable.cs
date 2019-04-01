using Core.Animation;
using UnityEngine;

namespace Core.AI.Variables
{
    public abstract class ValueVariable<T>
    {
        protected T _value;

        public virtual T GetValue()
        {
            return _value;
        }

        public virtual void SetValue(T val)
        {
            _value = val;
        }
    }

    [System.Serializable]
    public class BoolValue : ValueVariable<bool> { }

    [System.Serializable]
    public class IntValue : ValueVariable<int> { }

    [System.Serializable]
    public class FloatValue : ValueVariable<float> { }

    [System.Serializable]
    public class DoubleValue : ValueVariable<double> { }

    [System.Serializable]
    public class StringValue : ValueVariable<string> { }

    [System.Serializable]
    public class SystemObjectValue : ValueVariable<System.Object> { }

    [System.Serializable]
    public class UnityObjectValue : ValueVariable<Object> { }

    [System.Serializable]
    public class ComponentValue : ValueVariable<Component> { }

    [System.Serializable]
    public class TransformValue : ValueVariable<Transform> { }

    [System.Serializable]
    public class GameObjectValue : ValueVariable<GameObject> { }

    [System.Serializable]
    public class AnimationControllerValue : ValueVariable<AnimationController> { }
}