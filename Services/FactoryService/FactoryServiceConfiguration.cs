using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Services.Factory
{
	public class FactoryServiceConfiguration : ServiceConfiguration
	{
		public override Service ServiceClass { get { return new FactoryService(this, null); } }
	}
}
