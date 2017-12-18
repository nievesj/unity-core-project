using System.Collections;
using System.Collections.Generic;
using Core.Service;
using UnityEngine;

namespace Core.UI
{
	public class UIServiceConfiguration : ServiceConfiguration
	{
		public Canvas mainCanvas;
		public bool cacheWindows = false;
		public UIWindows HUD = UIWindows.UIHUD;

		public override void ShowEditorUI() {}

		protected override IService GetServiceClass()
		{
			return new UIService();
		}
	}
}