using UnityEngine;

namespace Core.Animation
{
    [CreateAssetMenu(menuName = "Core/Animation/CoreAnimationEvent")]
    public class CoreAnimationEvent : ScriptableObject
    {
        public string eventName;
    }
}