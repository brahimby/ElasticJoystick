using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ElasticJoystick
{
    public class ElasticJoystick : MonoBehaviour
    {
        #region Public Attributes
        [Header("Input")]
        public InputReader inputReader; // Input system

        public GameObject joystickBG;

        public float joystickRadius;

        public float speed = 0.17f;

        public float maxScale = 0.5f;

        public float scaleMultiplier = 0.5f;

        public float rotationThreshold = 20f;

        [Header("Direction Events")]
        public UnityEvent OnJoyLeft;
        public UnityEvent OnJoyRight;
        public UnityEvent OnJoyUp;
        public UnityEvent OnJoyDown;
        #endregion

        #region Private Attributes
        private GameObject joystick;

        private bool clickPerformed = false;

        private Vector2 mouseStartPos;

        private float currentScale = 0f;

        private float currentAngle = 0f;
        #endregion

        #region MonoBehaviour Methods
        // Start is called before the first frame update
        void Start()
        {
            joystick = joystickBG.transform.GetChild(0).gameObject;
            joystickRadius = joystickBG.GetComponent<RectTransform>().sizeDelta.y * .9f;
        }
        #endregion

        #region Private Methods
        private void OnEnable()
        {
            inputReader.inputEnabler();
            inputReader.ScreenTouchStartedEvent += Click_Started;
            inputReader.ScreenTouchCanceledEvent += Click_Canceled;
        }
        private void OnDisable()
        {
            inputReader.inputDisabler();
            inputReader.ScreenTouchStartedEvent -= Click_Started;
            inputReader.ScreenTouchCanceledEvent -= Click_Canceled;
        }
        private void Click_Started()
        {
            joystickBG.SetActive(true);
            clickPerformed = true;
            PointerDown();
            StartCoroutine(ScreenTouched());
            Debug.Log("Started");
        }
        private void Click_Canceled()
        {
            Debug.Log("Canceled");

            clickPerformed = false;
            joystickBG.SetActive(false);
        }

        private void PointerDown()
        {
            joystickBG.transform.position = Input.mousePosition;
            mouseStartPos = Input.mousePosition;

        }
        IEnumerator ScreenTouched()
        {
            while (clickPerformed)
            {
                Vector2 joystickTouchPos = Input.mousePosition;
                var joystickVec = (joystickTouchPos - mouseStartPos);
                float joystickDist = Vector2.Distance(mouseStartPos, joystickTouchPos);

                float mouseVelocity = Mathf.Min(Mathf.Sqrt(joystickVec.x * joystickVec.x + joystickVec.y * joystickVec.y) * 4f, 150f);
                float scaleValue = (mouseVelocity / 150f) * scaleMultiplier;
                currentScale += (scaleValue - currentScale) * speed;
                currentScale = Mathf.Clamp(currentScale, -maxScale, maxScale);
                joystick.transform.localScale = new Vector3(1 + currentScale, 1 - currentScale, 1f);
                // ROTATE
                float angle = Mathf.Atan2(joystickVec.y, joystickVec.x) * Mathf.Rad2Deg;
                if (mouseVelocity > 20f)
                {
                    currentAngle = angle;
                }
                joystick.transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
                if (joystickDist < joystickRadius)
                {
                    joystick.transform.position = mouseStartPos + joystickVec.normalized * joystickDist;
                }
                else
                {
                    joystick.transform.position = mouseStartPos + joystickVec.normalized * joystickRadius;

                }
                CheckDirection(joystickVec);
                yield return null;
            }

        }

        private void CheckDirection(Vector2 joystickVec)
        {

            // Normalized direction
            Vector2 dir = joystickVec.normalized;

            if (dir.magnitude > 0.5f) // add threshold so tiny movements don't trigger
            {

                //Remove the comment below to disable diagonal movement detection
                // if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                {
                    if (dir.x > 0.5f)
                        OnJoyRight?.Invoke();
                        
                    else if (dir.x < -0.5f)
                        OnJoyLeft?.Invoke();
                }
              //  else
                {
                    if (dir.y > 0.5f)
                        OnJoyUp?.Invoke();
                    else if (dir.y < -0.5f)
                        OnJoyDown?.Invoke();
                }
            }
        }

        #endregion
    }
}