using System.Collections;
using System.Collections.Generic;
using Core.Services;
using UnityEngine;

namespace Core.Services.UpdateManager
{
	public class UpdateServiceConfiguration : ServiceConfiguration
	{
		public Core.Services.UpdateManager.UpdateManager updateManager;

		protected override IService ServiceClass { get { return new UpdateService(this); } }
	}
}