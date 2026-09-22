
using UnityEngine;
using UnityEngine.InputSystem;



namespace PlayerControl
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        [SerializeField] private InputActionAsset playerControls;


        public InputAction RightClickMovement { get; private set; }
        public InputAction LeftClickSelection { get; private set; }

        public InputAction ZoomAction { get; private set; }
        public InputAction ShiftAction { get; private set; }




        private void Awake()
        {

            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            DontDestroyOnLoad(gameObject);

            // Initialize the InputAction

            RightClickMovement = playerControls.FindActionMap("Player").FindAction("RightClickMove");
            ZoomAction = playerControls.FindActionMap("Player").FindAction("ZoomPlayer");
            ShiftAction = playerControls.FindActionMap("Player").FindAction("Shift");
            LeftClickSelection = playerControls.FindActionMap("Player").FindAction("LeftClickSelect");

        }

        private void OnEnable()
        {
            RightClickMovement.Enable();
            ZoomAction.Enable();
            ShiftAction.Enable();
            LeftClickSelection.Enable();
        }

        private void OnDisable()
        {
            RightClickMovement.Disable();
            ZoomAction.Disable();
            ShiftAction.Disable();
            LeftClickSelection.Disable();
        }
    }

}