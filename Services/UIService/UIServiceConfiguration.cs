using Core.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Services.UI
{
	public class UIServiceConfiguration : ServiceConfiguration
	{
		public UIContainer mainCanvas;
		// public bool cacheWindows = false;

		override protected IService ServiceClass { get { return new UIService(); } }
	}
}