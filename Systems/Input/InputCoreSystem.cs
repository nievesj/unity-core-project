using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Systems
{
    /// <summary>
    /// These map directly to the CoreFrameworkInputMappings asset.
    /// </summary>
    public enum InputActionType
    {
        None,
        Move,
        Look,
        ButtonNorth,
        ButtonSouth,
        ButtonWest,
        ButtonEast,
        RightShoulder,
        RightTrigger,
        LeftTrigger,
        LeftShoulder,
        Start,
        Select,
        RightStickPress,
        LeftStickPress,
        DPadUp,
        DPadDown,
        DPadLeft,
        DPadRight,
    }

    public enum CoreInputSource
    {
        GamePad,
        Mouse,
        KeyBoard,
        Touch
    }

    public enum CoreActionEventType
    {
        Started,
        Performed,
        Canceled
    }

    public struct RegistrationValue
    {
        public InputActionType InputActionType;
        public CoreInputSource CoreInputSource;
        public InputAction InputAction;
        public CoreActionEventType CoreActionEventType;
    }

    public class ObservableInput
    {
        public RegistrationValue RegistrationValue = new RegistrationValue();

        public Subject<RegistrationValue> OnActionStarted = new Subject<RegistrationValue>();
        public Subject<RegistrationValue> OnActionPerformed = new Subject<RegistrationValue>();
        public Subject<RegistrationValue> OnActionCancelled = new Subject<RegistrationValue>();
    }

    public class InputCoreSystem : CoreSystem
    {
        [SerializeField]
        private PlayerInput playerInput;

        private Dictionary<InputActionType, ObservableInput> _subscriptions = new Dictionary<InputActionType, ObservableInput>();

        protected override void Awake()
        {
            base.Awake();

            Observable.FromEvent<PlayerInput>(
                    handler => playerInput.onControlsChanged += handler,
                    handler => playerInput.onControlsChanged -= handler)
                .Subscribe(OnControlsChanged)
                .AddTo(this);

            Observable.FromEvent<PlayerInput>(
                    handler => playerInput.onDeviceLost += handler,
                    handler => playerInput.onDeviceLost -= handler)
                .Subscribe(OnDeviceLost)
                .AddTo(this);

            Observable.FromEvent<PlayerInput>(
                    handler => playerInput.onDeviceRegained += handler,
                    handler => playerInput.onDeviceRegained -= handler)
                .Subscribe(OnDeviceRegained)
                .AddTo(this);

            //cant use subscription patterns like the ones above because it returns two values.
            InputSystem.onActionChange += OnActionChange;

            foreach (InputActionType inputActionType in Enum.GetValues(typeof(InputActionType)))
                _subscriptions.Add(inputActionType, new ObservableInput());
        }

        private void OnActionChange(object obj, InputActionChange inputActionChange)
        {
            if (!(obj is InputAction inputAction)) return;
            if (!Enum.TryParse(inputAction.name, out InputActionType coreInputAction)) return;

            // Debug.Log($"{inputAction.name} {inputActionChange} {inputAction.activeControl.device}");

            switch (inputActionChange)
            {
                case InputActionChange.ActionStarted:
                    ActionEvent(inputAction, coreInputAction, CoreActionEventType.Started);
                    break;
                case InputActionChange.ActionPerformed:
                    ActionEvent(inputAction, coreInputAction, CoreActionEventType.Performed);
                    break;
                case InputActionChange.ActionCanceled:
                    ActionEvent(inputAction, coreInputAction, CoreActionEventType.Canceled);
                    break;
            }
        }

        private void ActionEvent(InputAction inputAction, InputActionType inputActionType, CoreActionEventType coreActionEventType)
        {
            if (_subscriptions.ContainsKey(inputActionType))
            {
                var value = _subscriptions[inputActionType].RegistrationValue;
                value.InputAction = inputAction;
                value.InputActionType = inputActionType;
                value.CoreActionEventType = coreActionEventType;
                
                switch (coreActionEventType)
                {
                    case CoreActionEventType.Started:
                        _subscriptions[inputActionType].OnActionStarted.OnNext(value);
                        break;
                    case CoreActionEventType.Performed:
                        _subscriptions[inputActionType].OnActionPerformed.OnNext(value);
                        break;
                    case CoreActionEventType.Canceled:
                        _subscriptions[inputActionType].OnActionCancelled.OnNext(value);
                        break;
                }
            }
        }

        public IObservable<RegistrationValue> RegisterInputAction(InputActionType inputActionType, CoreActionEventType coreActionEventType)
        {
            switch (coreActionEventType)
            {
                case CoreActionEventType.Started:
                    return _subscriptions[inputActionType].OnActionStarted;
                case CoreActionEventType.Performed:
                    return _subscriptions[inputActionType].OnActionPerformed;
                case CoreActionEventType.Canceled:
                    return _subscriptions[inputActionType].OnActionCancelled;

                default:
                    throw new ArgumentOutOfRangeException(nameof(coreActionEventType), coreActionEventType, "RegisterInputAction critical error. Verify that subscriptions are setup correctly.");
            }
        }

        private void OnControlsChanged(PlayerInput input) { }

        private void OnDeviceLost(PlayerInput input) { }

        private void OnDeviceRegained(PlayerInput input) { }

        private void OnDestroy()
        {
            InputSystem.onActionChange -= OnActionChange;
        }
    }
}