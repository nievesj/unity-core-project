using Core.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Services.Input
{
	public class ControlServiceConfiguration : ServiceConfiguration
	{
		override protected IService ServiceClass { get { return new ControlService(); } }

		public GameObject MouseTouchControls;
	}
}