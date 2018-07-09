using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Core.Services.UI
{
	/// <summary>
	/// Widget UI is used to display bits of information on screen.
	/// Can be used to display player health, currency, tooltips, etc.
	/// </summary>
	public class UIWidget : UIElement
	{
		protected bool isShowing = false;
		protected bool isTransitioning = false;

		protected override void Awake()
		{
			base.Awake();
			_UiType = UIType.Widget;
		}

		public void ShowHideWidget()
		{
			if (!isTransitioning)
			{
				isTransitioning = true;

				if (isShowing)
					Hide().Run();
				else
					Show().Run();
			}
		}

		protected override void OnElementShow()
		{
			isShowing = true;
			isTransitioning = false;
		}

		protected override void OnElementHide()
		{
			isShowing = false;
			isTransitioning = false;
		}
	}
}