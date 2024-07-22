using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private Transform inputSpace;
    [SerializeField] private float movementSpeed = 5f;
    [Range(0f, 5f)]
    [SerializeField] private float speedMultiplier = 1f;
    [SerializeField] private float rotationSpeed = 5f;


    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector2 input = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input.magnitude == 0)
            return;

        Transform transformSpace = inputSpace ? inputSpace : transform;
        Vector3 forward = transformSpace.forward * input.y;
        Vector3 right = transformSpace.right * input.x;
        forward.y = 0;
        right.y = 0;
        float totalSpeed = movementSpeed * speedMultiplier;
        Vector3 direction = (forward + right).normalized;
        characterController.Move(totalSpeed * Time.deltaTime * direction + Physics.gravity * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
    }

}
