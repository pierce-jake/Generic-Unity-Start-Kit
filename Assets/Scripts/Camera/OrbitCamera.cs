using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour
{

    [SerializeField] private Transform target = default;

    [Header("Inputs")]
    [SerializeField] private bool invertX;
    [SerializeField] private bool invertY;
    [SerializeField] private bool invertScroll = true;
    [SerializeField] private float scrollSpeed = 1f;

    [Range(1, 360)]
    [SerializeField] private float rotationSpeed = 90f;
    [Range(0.1f, 2)]
    [SerializeField] private float sensitivity = 0.5f;

    [Header("Camera Settings")]
    [SerializeField] private Vector2 clampDistance = new(2f, 6f);

    [Range(1, 20)]
    [SerializeField] private float distance = 5f;

    [Min(0)]
    [SerializeField] private float focusRadius = 1f;

    [Range(0, 1)]
    [SerializeField] private float focusCentering = 0.5f;
    [Space]

    [Range(-90, 90)]
    [SerializeField] private float minVerticalAngle = -30f, maxVerticalAngle = 60f;
    [Space]
    [SerializeField] private bool autoAlign = true;
    [Min(0f)]
    [SerializeField] private float alignDelay = 5f;
    [Range(0, 90)]
    [SerializeField] private float alignCameraSmoothing = 45f;

    [SerializeField] private LayerMask obstructionMask = -1;

    private Vector2 orbitAngles = new(45, 0);
    private Vector3 focusPoint, lastFocusPoint;
    private const float e = 0.0001f;    // Epsillion

    private Vector3 CameraHalfExtends
    {
        get
        {
            Vector3 halfExtends;
            halfExtends.y = cam.nearClipPlane * Mathf.Tan(0.5f * Mathf.Deg2Rad * cam.fieldOfView);
            halfExtends.x = halfExtends.y * cam.aspect;
            halfExtends.z = 0;
            return halfExtends;
        }
    }

    private float lastRotationTime;

    private Camera cam;

    private static float GetAngle(Vector2 direction)
    {
        float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
        return direction.x < 0f ? 360f - angle : angle;
    }

    private void OnValidate()
    {
        if (maxVerticalAngle < minVerticalAngle)
            maxVerticalAngle = minVerticalAngle;
    }

    private void Awake()
    {
        cam = GetComponent<Camera>();

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        //This will allow for the camera to not always be on top of our object
        focusPoint = target.position;
        transform.localRotation = Quaternion.Euler(orbitAngles);    //Syncs the cameras rotation with the passed in angles
    }


    private void LateUpdate()
    {
        if (GameManager.Instance.state == GameState.Paused)
            return;

        UpdateFocusPoint();
        Quaternion lookRotation;

        if (ManualCameraRotation() || AutomaticCameraRotation())
        {
            ConstrainCameraAngles();
            lookRotation = Quaternion.Euler(orbitAngles);
        }
        else
        {
            lookRotation = transform.localRotation;
        }

        distance += Input.GetAxis("Mouse ScrollWheel") * scrollSpeed * (invertScroll ? -1 : 1);
        distance = Mathf.Clamp(distance, clampDistance.x, clampDistance.y);

        //Spawn camera on object then move back in the opposite facing direction to look at object
        Vector3 lookDirection = lookRotation * Vector3.forward;
        Vector3 lookPosition = focusPoint - lookDirection * distance;

        Vector3 rectOffset = lookDirection * cam.nearClipPlane;
        Vector3 rectPosition = lookPosition + rectOffset;
        Vector3 castFrom = target.position;
        Vector3 castLine = rectPosition - castFrom;
        float castDistance = castLine.magnitude;
        Vector3 castDirection = castLine / distance;

        if (Physics.BoxCast(castFrom, CameraHalfExtends, castDirection, out RaycastHit hit, lookRotation, castDistance, obstructionMask))
        {
            rectPosition = castFrom + castDirection * hit.distance;
            lookPosition = rectPosition - rectOffset;
        }

        transform.SetPositionAndRotation(lookPosition, lookRotation);
    }

    private void UpdateFocusPoint()
    {
        lastFocusPoint = focusPoint;

        if (focusRadius <= 0)
        {
            focusPoint = target.position;
            return;
        }

        float distance = Vector3.Distance(target.position, focusPoint);
        float t = 1;

        if (distance > e && focusCentering > 0)
            t = Mathf.Pow(1f - focusCentering, Time.unscaledDeltaTime);

        if (distance > focusRadius)
            t = Mathf.Min(t, focusRadius / distance);

        focusPoint = Vector3.Lerp(target.position, focusPoint, t);
    }

    private void ConstrainCameraAngles()
    {
        orbitAngles.x = Mathf.Clamp(orbitAngles.x, minVerticalAngle, maxVerticalAngle);

        //Ensures no overflow errors occur
        if (orbitAngles.y < 0 || orbitAngles.y >= 360)
            orbitAngles.y %= 360;
    }

    private bool AutomaticCameraRotation()
    {
        if (Time.unscaledTime - lastRotationTime < alignDelay || !autoAlign)
            return false;

        Vector2 movement = new(focusPoint.x - lastFocusPoint.x, focusPoint.z - lastFocusPoint.z);
        float movementDeltaSqr = movement.sqrMagnitude;
        if (movementDeltaSqr < e)       //Barely Moved
            return false;

        float headingAngle = GetAngle(movement / Mathf.Sqrt(movementDeltaSqr));
        float rotationRate = rotationSpeed * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSqr);
        float deltaAbs = Mathf.Abs(Mathf.DeltaAngle(orbitAngles.y, headingAngle));

        if (deltaAbs < alignCameraSmoothing)
        {
            rotationRate *= deltaAbs / alignCameraSmoothing;
        }
        else if (180f - deltaAbs < alignCameraSmoothing)
        {
            rotationRate *= (180f - deltaAbs) / alignCameraSmoothing;
        }

        orbitAngles.y = Mathf.MoveTowardsAngle(orbitAngles.y, headingAngle, rotationRate);  //Smooths out rotation when instant turning

        return true;
    }

    private bool ManualCameraRotation()
    {
        Vector2 input = new(Input.GetAxis("Mouse Y") * (invertY ? -1 : 1), Input.GetAxis("Mouse X") * (invertX ? -1 : 1));
        if (Mathf.Abs(input.x) > e || Mathf.Abs(input.y) > e)
        {
            orbitAngles += rotationSpeed * sensitivity * Time.unscaledDeltaTime * input;
            lastRotationTime = Time.unscaledDeltaTime;
            return true;
        }

        return false;
    }

}
