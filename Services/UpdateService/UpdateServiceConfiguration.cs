using System.Collections;
using System.Collections.Generic;
using Core.Services;
using UnityEngine;

namespace Core.Services.UpdateManager
{
	public class UpdateServiceConfiguration : ServiceConfiguration
	{
		public Core.Services.UpdateManager.UpdateManager updateManager;

		override protected IService ServiceClass { get { return new UpdateService(); } }
	}
}