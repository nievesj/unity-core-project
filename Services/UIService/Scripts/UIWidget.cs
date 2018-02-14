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

		protected override void Awake() {}

		public void ShowHideWidget()
		{
			if (!isTransitioning)
			{
				isTransitioning = true;

				if (isShowing)
					Hide().Subscribe();
				else
					Show().Subscribe();
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