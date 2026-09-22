using UnityEngine;
using UnityEngine.InputSystem;


public class CameraController : MonoBehaviour
{
    [Header("Déplacement")]
    [SerializeField] private float moveSpeed = 30f;
    [SerializeField] private float borderThreshold = 20f;
    [SerializeField] private bool useScreenEdge = true;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField][Range(5f, 50f)] private float minZoom = 10f;
    [SerializeField][Range(20f, 200f)] private float maxZoom = 100f;
    [SerializeField][Range(10f, 100f)] private float baseZoom = 25f;
    [SerializeField] private AnimationCurve speedByZoomCurve = AnimationCurve.Linear(0, 0.5f, 1, 2f);

    [Header("Limites")]
    [SerializeField] private bool useBoundaries = false;
    [SerializeField] private float minX = -500f;
    [SerializeField] private float maxX = 500f;
    [SerializeField] private float minZ = -500f;
    [SerializeField] private float maxZ = 500f;

    [Header("Curseur")]
    [SerializeField] private bool confineCursor = true;

    private Camera cam;
    private float targetZoom;
    private Vector3 targetPosition;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        targetZoom = baseZoom;
        Vector3 pos = transform.position;
        pos.y = targetZoom;
        transform.position = pos;
        targetPosition = transform.position;
    }

    private void Start()
    {
        UpdateTargetZoom(0f);
    }

    private void Update()
    {

#if UNITY_EDITOR
        if (!Application.isFocused) return;
#endif

        HandleMouseMovement();
        HandleZoom();
        HandleCursorConfinement();
    }

    private void HandleMouseMovement()
    {
        if (!useScreenEdge) return;

        Vector3 moveDirection = Vector3.zero;
        Vector2 mousePos = Mouse.current.position.ReadValue();

        // Bordure gauche
        if (mousePos.x < borderThreshold)
        {
            moveDirection += Vector3.right;
        }
        // Bordure droite
        else if (mousePos.x > Screen.width - borderThreshold)
        {
            moveDirection += Vector3.left;
        }

        // Bordure basse
        if (mousePos.y < borderThreshold)
        {
            moveDirection += Vector3.forward;
        }
        // Bordure haute
        else if (mousePos.y > Screen.height - borderThreshold)
        {
            moveDirection += Vector3.back;
        }

        if (moveDirection != Vector3.zero)
        {
            // Normaliser pour que les mouvements diagonaux ne soient pas plus rapides
            moveDirection.Normalize();

            // Calculer la vitesse basée sur le niveau de zoom
            float zoomFactor = GetZoomFactor();
            float adjustedSpeed = moveSpeed * zoomFactor * Time.deltaTime;
            // Appliquer le mouvement
            targetPosition += moveDirection * adjustedSpeed;

            // Appliquer les limites si activées
            if (useBoundaries)
            {
                targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
                targetPosition.z = Mathf.Clamp(targetPosition.z, minZ, maxZ);
            }
        }


        transform.position = targetPosition;
    }

    private void HandleZoom()
    {
        float scrollInput = Mouse.current.scroll.ReadValue().y / 120f; // Normaliser la valeur du scroll

        if (scrollInput != 0)
        {
            UpdateTargetZoom(scrollInput);
        }

        if (cam.orthographic)
        {
            cam.orthographicSize = targetZoom;
        }
        else
        {
            Vector3 currentPos = transform.position;
            currentPos.y = targetZoom;
            transform.position = currentPos;
        }
    }

    private void UpdateTargetZoom(float scrollInput)
    {
        targetZoom -= scrollInput * zoomSpeed;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
    }

    private float GetZoomFactor()
    {
        // Calculer un facteur basé sur le niveau actuel de zoom (entre 0 et 1)
        float zoomRange = maxZoom - minZoom;
        float currentZoomNormalized = cam.orthographic
            ? (cam.orthographicSize - minZoom) / zoomRange
            : (transform.position.y - minZoom) / zoomRange;

        // Utiliser la courbe d'animation pour déterminer le multiplicateur de vitesse
        return speedByZoomCurve.Evaluate(currentZoomNormalized);
    }

    public void SetZoomLimits(float min, float max)
    {
        minZoom = min;
        maxZoom = max;

        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public void SetZoomSpeed(float speed)
    {
        zoomSpeed = speed;
    }

    public void SetBoundaries(float minXValue, float maxXValue, float minZValue, float maxZValue)
    {
        minX = minXValue;
        maxX = maxXValue;
        minZ = minZValue;
        maxZ = maxZValue;
        useBoundaries = true;
    }

    private void HandleCursorConfinement()
    {
        if (Keyboard.current != null && Keyboard.current.f9Key.wasPressedThisFrame)
        {
            Debug.Log("Toggle Cursor Confinement");
            confineCursor = !confineCursor;
            ApplyCursorConfinement();
        }
    }

    private void ApplyCursorConfinement()
    {
        if (confineCursor)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void DisableBoundaries()
    {
        useBoundaries = false;
    }
}