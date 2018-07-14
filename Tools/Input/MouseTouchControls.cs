using UnityEngine;

namespace Core.Services.Input
{
    internal static class Constants
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
        protected float MinDragDistance = 1;

        [SerializeField]
        protected float TimeToTriggerTap = 0.25f;

        [SerializeField]
        protected float TimeToTriggerStationary = 1f;

        protected Camera MainCamera;
        protected Vector3 CurrentTouchMousePosition = Vector3.zero;
        protected MouseTouchState MouseTouchState = MouseTouchState.Nothing;
        protected readonly ControlState controlState = ControlState.Enabled;
        protected Vector3 StartDragOnMouseDownPosition = Vector3.zero;

        private float _tapTimer = 0.0f;
        private float _stationaryTimer = 0.0f;
        private int _tapCount = 0;
        private bool _onStationaryTriggered = false;

        protected abstract void OnMouseFingerDown(Vector3 pos);

        protected abstract void OnMouseFingerUp(Vector3 pos);

        protected abstract void OnMouseFingerDrag(Vector3 pos);

        protected abstract void OnSingleTap(Vector3 pos);

        protected abstract void OnDoubleTap(Vector3 pos);

        protected abstract void OnStationary(Vector3 pos);

        protected abstract void OnMobilePinch(Touch touch0, Touch touch1);

        protected override void Awake()
        {
            base.Awake();

            MainCamera = Camera.main;
        }

        protected virtual void Update()
        {
            if (controlState == ControlState.Enabled)
            {
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
				TouchControl();
#elif UNITY_WEBGL || UNITY_EDITOR || UNITY_STANDALONE || UNITY_FACEBOOK
                MouseControl();
#endif

                //tap timers
                if (_tapCount > 0 && _tapTimer > TimeToTriggerTap)
                {
                    _tapTimer = 0;
                    _tapCount = 0;
                }
                else if (_tapCount > 0)
                {
                    _tapTimer += Time.deltaTime;
                }
            }
        }

        /// <summary>
        /// Manages mouse controls for editor and webgl
        /// </summary>
        private void MouseControl()
        {
            CurrentTouchMousePosition = UnityEngine.Input.mousePosition;

            if (UnityEngine.Input.GetMouseButton(0))
            {
                MouseDown(CurrentTouchMousePosition);
            }

            if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                MouseUp(CurrentTouchMousePosition);
            }
        }

        /// <summary>
        /// Manages touch control for mobile devices
        /// </summary>
        private void TouchControl()
        {
            if (UnityEngine.Input.touchCount == 1)
            {
                CurrentTouchMousePosition = UnityEngine.Input.GetTouch(0).position;
                CurrentTouchMousePosition.z = MainCamera.nearClipPlane;

                switch (UnityEngine.Input.GetTouch(0).phase)
                {
                    case TouchPhase.Began:
                        MouseDown(CurrentTouchMousePosition);
                        break;

                    case TouchPhase.Moved:
                        MouseDrag(CurrentTouchMousePosition);
                        break;

                    case TouchPhase.Stationary:
                        _stationaryTimer += Time.deltaTime;

                        if (!_onStationaryTriggered && MouseTouchState == MouseTouchState.MouseDown && _stationaryTimer >= TimeToTriggerStationary &&
                            Vector3.Distance(StartDragOnMouseDownPosition, CurrentTouchMousePosition) < MinDragDistance)
                        {
                            _onStationaryTriggered = true;
                            OnStationary(CurrentTouchMousePosition);
                        }

                        break;

                    case TouchPhase.Canceled:
                        // MouseUp(currentTouchMousePosition);
                        break;

                    case TouchPhase.Ended:
                        MouseUp(CurrentTouchMousePosition);
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
            if (MouseTouchState == MouseTouchState.Nothing)
            {
                OnMouseFingerDown(mousePosition);
                StartDragOnMouseDownPosition = mousePosition;
                MouseTouchState = MouseTouchState.MouseDown;
            }

            //For Android, WebGL, PC, Mac, Linux the drag happens here
#if UNITY_WEBGL || UNITY_EDITOR || UNITY_STANDALONE || UNITY_FACEBOOK
            if (!_onStationaryTriggered && MouseTouchState == MouseTouchState.MouseDown && _stationaryTimer >= TimeToTriggerStationary &&
                Vector3.Distance(StartDragOnMouseDownPosition, mousePosition) < MinDragDistance)
            {
                _onStationaryTriggered = true;
                OnStationary(mousePosition);
            }

            _stationaryTimer += Time.deltaTime;

            MouseDrag(mousePosition);
#endif
        }

        private void MouseUp(Vector3 mousePosition)
        {
            _stationaryTimer = 0;
            _onStationaryTriggered = false;
            MouseTouchState = MouseTouchState.MouseUp;

            OnMouseFingerUp(mousePosition);
            MouseTouchState = MouseTouchState.Nothing;

            if (_tapCount == 0)
            {
                _tapCount++;
                _tapTimer = 0;

                OnSingleTap(mousePosition);
            }
            else if (_tapCount == 1 && _tapTimer <= TimeToTriggerTap)
            {
                OnDoubleTap(mousePosition);
                _tapCount = 0;
            }
        }

        private void MouseDrag(Vector3 mousePosition)
        {
            if (Vector3.Distance(StartDragOnMouseDownPosition, mousePosition) > MinDragDistance)
            {
                MouseTouchState = MouseTouchState.MouseDrag;
                OnMouseFingerDrag(mousePosition);
            }
        }

        /// <summary>
        /// Returns the mouse or finger world position (tracking only one finger)
        /// </summary>
        /// <returns></returns>
        protected Vector3 GetPointerLocationWorldPosition()
        {
            var position = Vector3.zero;

#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
			if (UnityEngine.Input.touchCount > 0)// && Input.GetTouch(0).phase == TouchPhase.Moved)
			{
				position = MainCamera.ScreenToWorldPoint(UnityEngine.Input.GetTouch(0).position);
			}
#elif UNITY_WEBGL || UNITY_EDITOR || UNITY_STANDALONE || UNITY_FACEBOOK

            position = MainCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
#endif
            return position;
        }

        /// <summary>
        /// Returns the mouse or finger world position (tracking only one finger)
        /// </summary>
        /// <returns></returns>
        protected Vector3 GetPointerLocationWorldPosition(Vector3 pos)
        {
            return MainCamera.ScreenToWorldPoint(pos);
            ;
        }

        /// <summary>
        /// Returns the delta position change of the mouse/finger
        /// </summary>
        /// <returns></returns>
        protected Vector3 GetPointerDeltaChange()
        {
            var position = Vector3.zero;

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
			return MainCamera.ScreenToViewportPoint(UnityEngine.Input.GetTouch(0).position);
#elif UNITY_WEBGL || UNITY_EDITOR || UNITY_STANDALONE || UNITY_FACEBOOK
            return MainCamera.ScreenToViewportPoint(UnityEngine.Input.mousePosition);
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
			return MainCamera.ScreenPointToRay(UnityEngine.Input.GetTouch(0).position);
#elif UNITY_WEBGL || UNITY_EDITOR || UNITY_STANDALONE || UNITY_FACEBOOK
            return MainCamera.ScreenPointToRay(UnityEngine.Input.mousePosition);
#endif
        }
    }
}