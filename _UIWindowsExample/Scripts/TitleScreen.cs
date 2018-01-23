using System.Collections;
using System.Collections.Generic;
using Core.LevelLoaderService;
using Core.Service;
using Core.UI;
using UniRx;
using UnityEngine;

namespace MatchGame
{
	public class TitleScreen : Level
	{
		protected override void LevelLoaded()
		{
			base.LevelLoaded();

			uiService = ServiceLocator.GetService<IUIService>() as UIService;
			uiService.OnWindowClosed.Subscribe(OnWindowClosed);

			uiService.Open(UIWindows.UITitle);
		}

		protected override void OnWindowClosed(UIWindow window)
		{
			base.OnWindowClosed(window);

			if (window is UITitleScreenWindow)
			{
				// uiService.OnWindowClosed.Dispose();
				Unload();
			}
		}
	}
}