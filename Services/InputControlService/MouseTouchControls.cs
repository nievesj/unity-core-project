using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Core.ControlSystem
{
	public class Constants
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

	public class MouseTouchControls : MonoBehaviour
	{
		public float cameraMovementSpeedMobile = 0.2f;
		public ControlState EnableDisableControlState
		{
			get { return controlState; }
			set { controlState = value; }
		}

		protected Subject<Vector2> onMouseDown = new Subject<Vector2>();
		public IObservable<Vector2> OnMouseDown { get { return onMouseDown; } }

		protected Subject<Vector2> onMouseUp = new Subject<Vector2>();
		public IObservable<Vector2> OnMouseUp { get { return onMouseUp; } }

		protected Subject<Vector2> onMouseDrag = new Subject<Vector2>();
		public IObservable<Vector2> OnMouseDrag { get { return onMouseDrag; } }

		protected MouseTouchState mouseTouchState = MouseTouchState.Nothing;
		protected ControlState controlState = ControlState.Enabled;
		protected Vector2 InputDeltaMovementChange = Vector2.zero;

		protected void Update()
		{

			if (controlState.Equals(ControlState.Enabled))
			{
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
				TouchControl();
#elif UNITY_WEBGL || UNITY_EDITOR
				MouseControl();
#endif
			}
		}

		/// <summary>
		/// Manages mouse controls for editor and webgl
		/// </summary>
		protected void MouseControl()
		{
			if (Input.GetMouseButton(0))
			{
				MouseDown(Input.mousePosition);
			}
			if (Input.GetMouseButtonUp(0))
			{
				MouseUp(Input.mousePosition);
			}
		}

		/// <summary>
		/// Manages touch control for mobile devices
		/// </summary>
		protected void TouchControl()
		{
			//limited to one finger, no need to track additional fingers for now...
			if (Input.touchCount > 0)
			{
				switch (Input.GetTouch(0).phase)
				{
					case TouchPhase.Began:
						MouseDown(Input.GetTouch(0).position);
						break;
					case TouchPhase.Moved:
						MouseDrag(Input.GetTouch(0).position);
						break;
					case TouchPhase.Stationary:
						//Debug.Log("Stationary");
						break;
					case TouchPhase.Canceled:
						MouseUp(Input.GetTouch(0).position);
						break;
					case TouchPhase.Ended:
						MouseUp(Input.GetTouch(0).position);
						break;
				}
			}
		}

		protected void MouseDown(Vector2 mousePosition)
		{
			if (mouseTouchState.Equals(MouseTouchState.Nothing))
				onMouseDown.OnNext((Vector2) Camera.main.ScreenToWorldPoint(mousePosition));

			mouseTouchState = MouseTouchState.MouseDown;

			//For Android, WebGL, PC, Mac, Linux the drag happens here
#if UNITY_WEBGL || UNITY_EDITOR
			MouseDrag(mousePosition);
#endif
		}

		protected void MouseUp(Vector2 mousePosition)
		{
			mouseTouchState = MouseTouchState.MouseUp;

			onMouseUp.OnNext((Vector2) Camera.main.ScreenToWorldPoint(mousePosition));
			mouseTouchState = MouseTouchState.Nothing;
		}

		protected void MouseDrag(Vector2 mousePosition)
		{
			mouseTouchState = MouseTouchState.MouseDrag;

			onMouseDrag.OnNext((Vector2) Camera.main.ScreenToWorldPoint(mousePosition));
		}

		/// <summary>
		/// Returns the mouse or finger world position (tracking only one finger)
		/// </summary>
		/// <returns></returns>
		public Vector2 GetPointerLocationWorldPosition()
		{
			Vector2 position = Vector2.zero;

#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
			if (Input.touchCount > 0) // && Input.GetTouch(0).phase == TouchPhase.Moved)
			{
				position = Camera.main.ScreenToWorldPoint(new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y));
			}
#elif UNITY_WEBGL || UNITY_EDITOR

			position = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
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

#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
			if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
			{
				position.x = .03f * Input.GetTouch(0).deltaPosition.x;
				position.y = .03f * Input.GetTouch(0).deltaPosition.y;
			}
#elif UNITY_WEBGL || UNITY_EDITOR

			position.x = Input.GetAxis(Constants.MouseAxisY);
			position.y = Input.GetAxis(Constants.MouseAxisX);
#endif
			return position;

		}
	}
}