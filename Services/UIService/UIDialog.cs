using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Services.UI
{
	/// <summary>
	/// Dialog UI is a window or screen that expects some kind of user input.
	/// IUIService sends OnGamePause signal when a window of this type is opened.
	/// </summary>
	public class UIDialog : UIElement
	{
		[SerializeField]
		private Button closeButton;

		protected override void Awake()
		{
			if (closeButton)
				closeButton.onClick.AddListener(OnCloseButtonClicked);
		}

		public void OnCloseButtonClicked()
		{
			Close().Subscribe();
		}

		protected override void OnElementShow() {}

		protected override void OnElementHide() {}
	}
}