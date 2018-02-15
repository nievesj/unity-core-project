using System;
using System.Collections;
using System.Collections.Generic;
using Core.Services.UI;
using UniRx;
using UnityEngine;

namespace Core.Services.UI
{
	public class UIScreenFader : UIElement
	{
		private bool isBlockingScreen = false;
		public bool IsBlockingScreen { get { return isBlockingScreen; } }

		public IObservable<UIElement> BlockScreen(bool isblock)
		{
			return Observable.Create<UIElement>(
				(IObserver<UIElement> observer)=>
				{
					Action<UIElement> OnFadeDone = uiElement =>
					{
						observer.OnNext(this);
						observer.OnCompleted();
					};

					if (isblock)
						return Show().Subscribe(OnFadeDone);
					else
						return Hide().Subscribe(OnFadeDone);
				});
		}

		protected override void Awake() {}

		protected override void Start() {}

		protected override void OnElementHide() {}

		protected override void OnElementShow() {}
	}
}