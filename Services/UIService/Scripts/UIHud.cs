using System.Collections;
using System.Collections.Generic;
using Core.LevelLoaderService;
using Core.Service;
using Core.UI;
using UniRx;
using UnityEngine;

namespace Core.UI
{
	public class UIHud : UIWindow
	{

		public void OnGearClick()
		{
			var levelLoader = ServiceLocator.GetService<ILevelLoaderService>()as LevelLoaderService.LevelLoaderService;
			// Close()
			// 	.Subscribe(hud =>
			// 	{
			// 		levelLoader.LoadLevel(Levels.MainLevel);
			// 	});

			// levelLoader.LoadLevel(Levels.MainLevel)
			// 	.Subscribe(level =>
			// 	{
			// 		Close();
			// 	});

			levelLoader.LoadLevel(MatchBattle.Constants.Levels.MAIN);
			Close();
		}
	}
}