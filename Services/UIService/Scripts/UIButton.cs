using System.Collections;
using System.Collections.Generic;
using Core.Audio;
using Core.Service;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.UI
{
	[RequireComponent(typeof(Button))]
	public class UIButton : MonoBehaviour
	{
		public AudioPlayer onClickSound;
		protected IAudioService audioService;
		protected Button button;

		protected void Awake()
		{
			audioService = Services.GetService<IAudioService>();
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