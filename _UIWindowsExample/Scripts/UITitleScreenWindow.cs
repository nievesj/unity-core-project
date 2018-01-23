using System.Collections;
using System.Collections.Generic;
using Core.LevelLoaderService;
using Core.Service;
using Core.UI;
using UnityEngine;

namespace MatchGame
{
	public class UITitleScreenWindow : UIWindow
	{
		LevelLoaderService levelLoader;

		public override void Initialize(IUIService svc)
		{
			base.Initialize(svc);

			levelLoader = ServiceLocator.GetService<ILevelLoaderService>() as LevelLoaderService;
		}

		public void OnStartClick()
		{
			levelLoader.LoadLevel(Levels.Level_1);
			Close();
		}
	}
}