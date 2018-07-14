using UnityEngine;

namespace Core.Services
{
	public abstract class ServiceConfiguration : ScriptableObject
	{
		public abstract Service ServiceClass { get; }
	}
}