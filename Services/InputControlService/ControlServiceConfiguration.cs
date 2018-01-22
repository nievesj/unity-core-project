using System.Collections;
using System.Collections.Generic;
using Core.Service;
using UnityEngine;

namespace Core.ControlSystem
{
	public class ControlServiceConfiguration : ServiceConfiguration
	{
		public GameObject MouseTouchControls;

		protected override IService GetServiceClass()
		{
			return new ControlService();
		}
	}
}