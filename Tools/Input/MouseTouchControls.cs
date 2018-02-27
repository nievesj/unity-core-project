using System.Collections;
using System.Collections.Generic;
using Core.Services.UI;
using Core.Services.UpdateManager;
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
	/// Enables basic mouse / touch controls that can be used to interact with the game world It only
	/// tracks one finger. Emits the following events: OnMouseDown OnMouseUp OnMouseDrag
	/// </summary>
	public abstract class MouseTouchControls : CoreBehaviour
	{
		[SerializeField]
		protected float minDragDistance = 1;

		[SerializeField]
		protected float timeToTriggerTap = 0.25f;

		[SerializeField]
		protected float timeToTriggerStationary = 1f;

		protected Vector3 currentTouchMousePosition = Vector3.zero;

		protected MouseTouchState mouseTouchState = MouseTouchState.Nothing;
		protected ControlState controlState = ControlState.Enabled;

		protected Vector3 startDragOnMouseDownPosition = Vector3.zero;

		private float tapTimer = 0.0f;
		private float stationaryTimer = 0.0f;
		private int tapCount = 0;
		private bool onStationaryTriggered = false;
		private BehaviourDelegateType coreUpdateDelegate;

		protected abstract void OnMouseDown(Vector3 pos);

		protected abstract void OnMouseUp(Vector3 pos);

		protected abstract void OnMouseDrag(Vector3 pos);

		protected abstract void OnSingleTap(Vector3 pos);

		protected abstract void OnDoubleTap(Vector3 pos);

		protected abstract void OnStationary(Vector3 pos);

		protected abstract void OnMobilePinch(Touch touch0, Touch touch1);

		protected override void Start()
		{
			base.Start();
			coreUpdateDelegate = new BehaviourDelegateType(this, CoreUpdate, UpdateType.Update);
			updateService.Attach(coreUpdateDelegate);
		}

		protected override void CoreUpdate()
		{
			if (controlState == ControlState.Enabled)
			{
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
				TouchControl();
#elif UNITY_WEBGL || UNITY_EDITOR || UNITY_STANDALONE || UNITY_FACEBOOK
				MouseControl();
#endif

				//tap timers
				if ((tapCount > 0) && (tapTimer > timeToTriggerTap))
				{
					tapTimer = 0;
					tapCount = 0;
				}
				else if (tapCount > 0)
				{
					tapTimer += Time.deltaTime;
				}
			}
		}

		/// <summary>
		/// Manages mouse controls for editor and webgl
		/// </summary>
		private void MouseControl()
		{
			currentTouchMousePosition = UnityEngine.Input.mousePosition;
			currentTouchMousePosition.z = Camera.main.nearClipPlane;

			if (UnityEngine.Input.GetMouseButton(0))
			{
				MouseDown(currentTouchMousePosition);
			}
			if (UnityEngine.Input.GetMouseButtonUp(0))
			{
				MouseUp(currentTouchMousePosition);
			}
		}

		/// <summary>
		/// Manages touch control for mobile devices
		/// </summary>
		private void TouchControl()
		{
			if (UnityEngine.Input.touchCount == 1)
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
						stationaryTimer += Time.deltaTime;

						if (!onStationaryTriggered && mouseTouchState == MouseTouchState.MouseDown && (stationaryTimer >= timeToTriggerStationary) &&
							(Vector3.Distance(startDragOnMouseDownPosition, currentTouchMousePosition) < minDragDistance))
						{
							onStationaryTriggered = true;
							OnStationary(currentTouchMousePosition);
						}
						break;

					case TouchPhase.Canceled:
						// MouseUp(currentTouchMousePosition);
						break;

					case TouchPhase.Ended:
						MouseUp(currentTouchMousePosition);
						break;
				}
			}
			else if (UnityEngine.Input.touchCount == 2)
			{
				OnMobilePinch(UnityEngine.Input.GetTouch(0), UnityEngine.Input.GetTouch(1));
			}
		}

		private void MouseDown(Vector3 mousePosition)
		{
			if (mouseTouchState == MouseTouchState.Nothing)
			{
				OnMouseDown(mousePosition);
				startDragOnMouseDownPosition = mousePosition;
				mouseTouchState = MouseTouchState.MouseDown;
			}

			//For Android, WebGL, PC, Mac, Linux the drag happens here
#if UNITY_WEBGL || UNITY_EDITOR || UNITY_STANDALONE || UNITY_FACEBOOK
			if (!onStationaryTriggered && mouseTouchState == MouseTouchState.MouseDown && (stationaryTimer >= timeToTriggerStationary) &&
				(Vector3.Distance(startDragOnMouseDownPosition, mousePosition) < minDragDistance))
			{
				onStationaryTriggered = true;
				OnStationary(mousePosition);
			}

			stationaryTimer += Time.deltaTime;

			MouseDrag(mousePosition);
#endif
		}

		private void MouseUp(Vector3 mousePosition)
		{
			stationaryTimer = 0;
			onStationaryTriggered = false;
			mouseTouchState = MouseTouchState.MouseUp;

			OnMouseUp(mousePosition);
			mouseTouchState = MouseTouchState.Nothing;

			if (tapCount == 0)
			{
				tapCount++;
				tapTimer = 0;

				OnSingleTap(mousePosition);
			}
			else if (tapCount == 1 && tapTimer <= timeToTriggerTap)
			{
				OnDoubleTap(mousePosition);
				tapCount = 0;
			}
		}

		private void MouseDrag(Vector3 mousePosition)
		{
			if (Vector3.Distance(startDragOnMouseDownPosition, mousePosition) > minDragDistance)
			{
				mouseTouchState = MouseTouchState.MouseDrag;
				OnMouseDrag(mousePosition);
			}
		}

		/// <summary>
		/// Returns the mouse or finger world position (tracking only one finger)
		/// </summary>
		/// <returns></returns>
		protected Vector3 GetPointerLocationWorldPosition()
		{
			Vector3 position = Vector3.zero;

#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
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
		protected Vector3 GetPointerDeltaChange()
		{
			Vector3 position = Vector3.zero;

#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
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

		protected Vector3 ScreenToViewportPoint()
		{
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
			return Camera.main.ScreenToViewportPoint(UnityEngine.Input.GetTouch(0).position);
#elif UNITY_WEBGL || UNITY_EDITOR || UNITY_STANDALONE || UNITY_FACEBOOK
			return Camera.main.ScreenToViewportPoint(UnityEngine.Input.mousePosition);
#endif
		}

		/// <summary>
		/// Platform independent ScreenPointToRay Uses current mouse or touch position to calculate
		/// the ray
		/// </summary>
		/// <returns></returns>
		protected Ray ScreenPointToRay()
		{
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
			return Camera.main.ScreenPointToRay(UnityEngine.Input.GetTouch(0).position);
#elif UNITY_WEBGL || UNITY_EDITOR || UNITY_STANDALONE || UNITY_FACEBOOK
			return Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
#endif
		}

		private void OnDestroy()
		{
			updateService.Detach(coreUpdateDelegate);
		}
	}
}