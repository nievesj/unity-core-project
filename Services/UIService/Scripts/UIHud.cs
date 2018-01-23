using System.Collections;
using System.Collections.Generic;
using Core.LevelLoaderService;
using Core.Service;
using Core.UI;
using UnityEngine;

namespace Core.UI
{
	public class UIHud : UIWindow
	{

		public void OnGearClick()
		{
			var levelLoader = ServiceLocator.GetService<ILevelLoaderService>() as LevelLoaderService.LevelLoaderService;
			levelLoader.LoadLevel(Levels.MainLevel);

			Close();
		}

	}
}