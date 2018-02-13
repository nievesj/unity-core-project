using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Core.Services.Input
{
	internal class Constants
	{
		public const string MouseAxisX = "Mouse X";
		public const string MouseAxisY = "Mouse Y";
	}

	public enum ControlState
	{
		Enabled,
		Disabled
	}

	public enum MouseTouchState
	{
		MouseDown,
		MouseDrag,
		MouseUp,
		Nothing,
	}

	/// <summary>
	/// Enables basic mouse / touch controls that can be used to interact with the game world
	/// It only tracks one finger.
	/// Emits the following events:
	/// OnMouseDown
	/// OnMouseUp
	/// OnMouseDrag
	/// </summary>
	public class MouseTouchControls : MonoBehaviour
	{
		public float cameraMovementSpeedMobile = 0.2f;
		public ControlState EnableDisableControlState
		{
			get { return controlState; }
			set { controlState = value; }
		}

		private MouseTouchState mouseTouchState = MouseTouchState.Nothing;
		private ControlState controlState = ControlState.Enabled;

		private Subject<Vector2> onMouseDown = new Subject<Vector2>();
		public IObservable<Vector2> OnMouseDown { get { return onMouseDown; } }

		private Subject<Vector2> onMouseUp = new Subject<Vector2>();
		public IObservable<Vector2> OnMouseUp { get { return onMouseUp; } }

		private Subject<Vector2> onMouseDrag = new Subject<Vector2>();
		public IObservable<Vector2> OnMouseDrag { get { return onMouseDrag; } }

		public void Init()
		{
			onMouseDown.Dispose();
			onMouseUp.Dispose();
			onMouseDrag.Dispose();

			onMouseDown = new Subject<Vector2>();
			onMouseUp = new Subject<Vector2>();
			onMouseDrag = new Subject<Vector2>();
		}

		/// <summary>
		/// Since we're dealing with inputs using Update is more appropiate than using a co-routine.
		/// </summary>
		private void Update()
		{
			if (controlState.Equals(ControlState.Enabled))
			{
#if (UNITY_IOS || UNITY_ANDROID)&& !UNITY_EDITOR
				TouchControl();
#elif UNITY_WEBGL || UNITY_EDITOR
				MouseControl();
#endif
			}
		}

		/// <summary>
		/// Manages mouse controls for editor and webgl
		/// </summary>
		private void MouseControl()
		{
			if (UnityEngine.Input.GetMouseButton(0))
			{
				MouseDown(UnityEngine.Input.mousePosition);
			}
			if (UnityEngine.Input.GetMouseButtonUp(0))
			{
				MouseUp(UnityEngine.Input.mousePosition);
			}
		}

		/// <summary>
		/// Manages touch control for mobile devices
		/// </summary>
		private void TouchControl()
		{
			//limited to one finger, no need to track additional fingers for now...
			if (UnityEngine.Input.touchCount > 0)
			{
				switch (UnityEngine.Input.GetTouch(0).phase)
				{
					case TouchPhase.Began:
						MouseDown(UnityEngine.Input.GetTouch(0).position);
						break;
					case TouchPhase.Moved:
						MouseDrag(UnityEngine.Input.GetTouch(0).position);
						break;
					case TouchPhase.Stationary:
						//Debug.Log("Stationary");
						break;
					case TouchPhase.Canceled:
						MouseUp(UnityEngine.Input.GetTouch(0).position);
						break;
					case TouchPhase.Ended:
						MouseUp(UnityEngine.Input.GetTouch(0).position);
						break;
				}
			}
		}

		private void MouseDown(Vector2 mousePosition)
		{
			if (mouseTouchState.Equals(MouseTouchState.Nothing))
				onMouseDown.OnNext((Vector2)Camera.main.ScreenToWorldPoint(mousePosition));

			mouseTouchState = MouseTouchState.MouseDown;

			//For Android, WebGL, PC, Mac, Linux the drag happens here
#if UNITY_WEBGL || UNITY_EDITOR
			MouseDrag(mousePosition);
#endif
		}

		private void MouseUp(Vector2 mousePosition)
		{
			mouseTouchState = MouseTouchState.MouseUp;

			onMouseUp.OnNext((Vector2)Camera.main.ScreenToWorldPoint(mousePosition));
			mouseTouchState = MouseTouchState.Nothing;
		}

		private void MouseDrag(Vector2 mousePosition)
		{
			mouseTouchState = MouseTouchState.MouseDrag;

			onMouseDrag.OnNext((Vector2)Camera.main.ScreenToWorldPoint(mousePosition));
		}

		/// <summary>
		/// Returns the mouse or finger world position (tracking only one finger)
		/// </summary>
		/// <returns></returns>
		public Vector2 GetPointerLocationWorldPosition()
		{
			Vector2 position = Vector2.zero;

#if (UNITY_IOS || UNITY_ANDROID)&& !UNITY_EDITOR
			if (UnityEngine.Input.touchCount > 0)// && Input.GetTouch(0).phase == TouchPhase.Moved)
			{
				position = Camera.main.ScreenToWorldPoint(new Vector2(UnityEngine.Input.GetTouch(0).position.x, UnityEngine.Input.GetTouch(0).position.y));
			}
#elif UNITY_WEBGL || UNITY_EDITOR

			position = Camera.main.ScreenToWorldPoint(new Vector2(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y));
#endif
			return position;
		}

		/// <summary>
		/// Returns de delta position change of the mouse/finger
		/// </summary>
		/// <returns></returns>
		public Vector2 GetPointerDeltaChange()
		{
			Vector2 position = Vector2.zero;

#if (UNITY_IOS || UNITY_ANDROID)&& !UNITY_EDITOR
			if (UnityEngine.Input.touchCount > 0 && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Moved)
			{
				position.x = .03f * UnityEngine.Input.GetTouch(0).deltaPosition.x;
				position.y = .03f * UnityEngine.Input.GetTouch(0).deltaPosition.y;
			}
#elif UNITY_WEBGL || UNITY_EDITOR

			position.x = UnityEngine.Input.GetAxis(Constants.MouseAxisY);
			position.y = UnityEngine.Input.GetAxis(Constants.MouseAxisX);
#endif
			return position;
		}

		private void OnDestroy()
		{
			onMouseDown.Dispose();
			onMouseUp.Dispose();
			onMouseDrag.Dispose();
		}
	}
}