using System.Collections;
using System.Collections.Generic;
using Core.Service;
using UnityEngine;

namespace Core.ControlSystem
{
	public class ControlServiceConfiguration : ServiceConfiguration
	{
		override protected IService ServiceClass { get { return new ControlService(); } }

		public GameObject MouseTouchControls;
	}
}