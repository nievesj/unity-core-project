﻿using UnityEngine;

namespace Core.Common.Extensions.BitStrap
{
    /// <summary>
    /// Bunch of utility extension methods to the GameObject class.
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Like the Component.GetComponentInParent however it allows to find in inactive GameObjects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
        public static T GetComponentInParent<T>(this GameObject self, bool includeInactive) where T : Component
        {
            if (includeInactive)
            {
                T component = null;

                for (var t = self.transform; t != null; t = t.parent)
                {
                    component = t.GetComponent<T>();
                    if (component != null)
                        break;
                }

                return component;
            }
            else
            {
                return self.GetComponentInParent<T>();
            }
        }
    }
}