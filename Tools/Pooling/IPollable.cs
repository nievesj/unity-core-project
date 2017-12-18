
using UnityEngine;

namespace Core.Polling
{
	public interface IPollable
	{
		bool IsActive { get; set; }

		GameObject GetGameObject();
	}
}
