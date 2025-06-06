using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ElasticJoystick
{
    [CreateAssetMenu(menuName = "InputReder")]
    public class InputReader : ScriptableObject, CustomInput.IControlsActions
    {
        #region Private attributes
        /// <summary>
        /// Instance of the CustomInput
        /// </summary>
        private CustomInput _customInput;
        #endregion

        #region Public attributes
        public event Action ScreenTouchStartedEvent;
        public event Action ScreenTouchCanceledEvent;
        #endregion

        #region Private methods
        private void OnEnable()
        {
            if (_customInput == null)
            {
                _customInput = new CustomInput();
                _customInput.Controls.SetCallbacks(this);
            }
            _customInput.Controls.Enable();
        }

        private void OnDisable()
        {
            _customInput.Controls.Disable();
        }
        #endregion


        #region Public methods

        public void inputEnabler()
        {
            _customInput.Controls.Enable();
        }

        public void inputDisabler()
        {
            _customInput.Controls.Disable();
        }

        public void OnScreenTouch(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                ScreenTouchStartedEvent?.Invoke();
            }
            if (context.phase == InputActionPhase.Canceled)
            {
                ScreenTouchCanceledEvent?.Invoke();
            }
        }

        #endregion
    }
}
