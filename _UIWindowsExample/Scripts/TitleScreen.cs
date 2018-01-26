using System.Collections;
using System.Collections.Generic;
using Core.LevelLoaderService;
using Core.Service;
using Core.UI;
using UniRx;
using UnityEngine;

namespace MatchGame
{
	[System.Obsolete("Tagged to be removed. Do not use anymore.", true)]
	public class TitleScreen : Level
	{
		protected override void Awake()
		{
			base.Awake();

			uiService.OnWindowClosed.Subscribe(OnWindowClosed);
			uiService.OpenWindow(UIWindows.UITitle.ToString());
		}

		protected void OnWindowClosed(UIWindow window)
		{
			if (window is UITitleScreenWindow)
			{
				// uiService.OnWindowClosed.Dispose();
				Unload();
			}
		}
	}
}