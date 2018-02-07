using Core.Services;
using Core.Services.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.Services.UI
{
	[RequireComponent(typeof(Button))]
	public class UIButton : MonoBehaviour
	{
		public AudioPlayer onClickSound;
		protected IAudioService audioService;
		protected Button button;

		protected void Awake()
		{
			audioService = ServiceLocator.GetService<IAudioService>();
			button = GetComponent<Button>();

			button.onClick.AddListener(OnClick);
		}

		protected void OnClick()
		{
			if (audioService != null && onClickSound != null && onClickSound.Clip != null)
				audioService.PlayClip(onClickSound);
		}

		void OnDestroy()
		{
			button.onClick.RemoveListener(OnClick);
		}
	}
}