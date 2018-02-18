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
	public abstract class MouseTouchControls : MonoBehaviour
	{
		[SerializeField]
		protected float minDragDistance = 1;

		protected Vector3 currentTouchMousePosition = Vector3.zero;

		protected MouseTouchState mouseTouchState = MouseTouchState.Nothing;
		protected ControlState controlState = ControlState.Enabled;

		protected Vector3 startDragOnMouseDownPosition = Vector3.zero;

		protected abstract void OnMouseDown(Vector3 pos);
		protected abstract void OnMouseUp(Vector3 pos);
		protected abstract void OnMouseDrag(Vector3 pos);

		/// <summary>
		/// Since we're dealing with inputs using Update is more appropiate than using a co-routine.
		/// </summary>
		protected virtual void Update()
		{
			if (controlState.Equals(ControlState.Enabled))
			{
#if (UNITY_IOS || UNITY_ANDROID)&& !UNITY_EDITOR
				TouchControl();
#elif UNITY_WEBGL || UNITY_EDITOR || UNITY_STANDALONE || UNITY_FACEBOOK
				MouseControl();
#endif
			}
		}

		/// <summary>
		/// Manages mouse controls for editor and webgl
		/// </summary>
		protected virtual void MouseControl()
		{
			currentTouchMousePosition = UnityEngine.Input.mousePosition;
			currentTouchMousePosition.z = Camera.main.nearClipPlane;

			if (UnityEngine.Input.GetMouseButton(0))
			{
				// MouseDown(UnityEngine.Input.mousePosition);
				MouseDown(currentTouchMousePosition);
			}
			if (UnityEngine.Input.GetMouseButtonUp(0))
			{
				// MouseUp(UnityEngine.Input.mousePosition);
				MouseUp(currentTouchMousePosition);
			}
		}

		/// <summary>
		/// Manages touch control for mobile devices
		/// </summary>
		protected virtual void TouchControl()
		{
			//limited to one finger, no need to track additional fingers for now...
			if (UnityEngine.Input.touchCount > 0)
			{
				currentTouchMousePosition = UnityEngine.Input.GetTouch(0).position;
				currentTouchMousePosition.z = Camera.main.nearClipPlane;

				switch (UnityEngine.Input.GetTouch(0).phase)
				{
					case TouchPhase.Began:
						MouseDown(currentTouchMousePosition);
						break;
					case TouchPhase.Moved:
						MouseDrag(currentTouchMousePosition);
						break;
					case TouchPhase.Stationary:
						//Debug.Log("Stationary");
						break;
					case TouchPhase.Canceled:
						MouseUp(currentTouchMousePosition);
						break;
					case TouchPhase.Ended:
						MouseUp(currentTouchMousePosition);
						break;
				}
			}
		}

		protected virtual void MouseDown(Vector3 mousePosition)
		{
			if (mouseTouchState.Equals(MouseTouchState.Nothing))
			{
				OnMouseDown(mousePosition);
				startDragOnMouseDownPosition = mousePosition;
				mouseTouchState = MouseTouchState.MouseDown;
			}

			//For Android, WebGL, PC, Mac, Linux the drag happens here
#if UNITY_WEBGL || UNITY_EDITOR
			MouseDrag(mousePosition);
#endif
		}

		protected virtual void MouseUp(Vector3 mousePosition)
		{
			mouseTouchState = MouseTouchState.MouseUp;

			OnMouseUp(mousePosition);
			mouseTouchState = MouseTouchState.Nothing;
		}

		protected virtual void MouseDrag(Vector3 mousePosition)
		{
			if (Vector3.Distance(startDragOnMouseDownPosition, mousePosition)> minDragDistance)
			{
				mouseTouchState = MouseTouchState.MouseDrag;
				OnMouseDrag(mousePosition);
			}
		}

		/// <summary>
		/// Returns the mouse or finger world position (tracking only one finger)
		/// </summary>
		/// <returns></returns>
		protected virtual Vector3 GetPointerLocationWorldPosition()
		{
			Vector3 position = Vector3.zero;

#if (UNITY_IOS || UNITY_ANDROID)&& !UNITY_EDITOR
			if (UnityEngine.Input.touchCount > 0)// && Input.GetTouch(0).phase == TouchPhase.Moved)
			{
				position = Camera.main.ScreenToWorldPoint(UnityEngine.Input.GetTouch(0).position);
			}
#elif UNITY_WEBGL || UNITY_EDITOR || UNITY_STANDALONE || UNITY_FACEBOOK

			position = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
#endif
			return position;
		}

		/// <summary>
		/// Returns de delta position change of the mouse/finger
		/// </summary>
		/// <returns></returns>
		protected virtual Vector3 GetPointerDeltaChange()
		{
			Vector3 position = Vector3.zero;

#if (UNITY_IOS || UNITY_ANDROID)&& !UNITY_EDITOR
			if (UnityEngine.Input.touchCount > 0 && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Moved)
			{
				position.x = .03f * UnityEngine.Input.GetTouch(0).deltaPosition.x;
				position.y = .03f * UnityEngine.Input.GetTouch(0).deltaPosition.y;
			}
#elif UNITY_WEBGL || UNITY_EDITOR || UNITY_STANDALONE || UNITY_FACEBOOK

			position.x = UnityEngine.Input.GetAxis(Constants.MouseAxisY);
			position.y = UnityEngine.Input.GetAxis(Constants.MouseAxisX);
#endif
			return position;
		}

		protected virtual Vector3 ScreenToViewportPoint()
		{
#if (UNITY_IOS || UNITY_ANDROID)&& !UNITY_EDITOR
			return Camera.main.ScreenToViewportPoint(UnityEngine.Input.GetTouch(0).position);
#elif UNITY_WEBGL || UNITY_EDITOR || UNITY_STANDALONE || UNITY_FACEBOOK
			return Camera.main.ScreenToViewportPoint(UnityEngine.Input.mousePosition);
#endif
		}

		/// <summary>
		/// Platform independent ScreenPointToRay
		/// Uses current mouse or touch position to calculate the ray
		/// </summary>
		/// <returns></returns>
		protected virtual Ray ScreenPointToRay()
		{
#if (UNITY_IOS || UNITY_ANDROID)&& !UNITY_EDITOR
			return Camera.main.ScreenPointToRay(UnityEngine.Input.GetTouch(0).position);
#elif UNITY_WEBGL || UNITY_EDITOR || UNITY_STANDALONE || UNITY_FACEBOOK
			return Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
#endif
		}
	}
}