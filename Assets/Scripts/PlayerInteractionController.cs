using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerInteractionController : MonoBehaviour
{
    [Header("Impostazioni Movimento")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Impostazioni Camera")]
    public Camera playerCamera;
    public float mouseSensitivity = 100f;
    float xRotation = 0f;

    [Header("Impostazioni Raccolta Oggetti")]
    public float pickupRange = 3f;
    public Transform holdPosition;
    public LayerMask pickupLayer;

    // Variabili private
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private GameObject heldObject;
    private Rigidbody heldObjRb;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMovement();
        HandleInteraction();
    }

    void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null)
            {
                TryPickupObject();
            }
            else
            {
                DropObject();
            }
        }
    }

    void TryPickupObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, pickupRange, pickupLayer))
        {
            if (hit.collider.GetComponent<Rigidbody>())
            {
                PickupObject(hit.collider.gameObject);
            }
        }
    }

    // --- PARTE MODIFICATA ---
    void PickupObject(GameObject obj)
    {
        heldObject = obj;
        heldObjRb = obj.GetComponent<Rigidbody>();

        // 1. Rendiamo l'oggetto Kinematic: la fisica non lo sposterà più
        heldObjRb.isKinematic = true;

        // 2. Lo spostiamo ESATTAMENTE nella posizione della mano
        heldObject.transform.position = holdPosition.position;
        
        // 3. (Opzionale) Resettiamo la rotazione per allinearlo alla mano
        // heldObject.transform.rotation = holdPosition.rotation; 

        // 4. Lo imparentiamo: ora si muove al 100% con il player
        heldObject.transform.parent = holdPosition;
    }

    void DropObject()
    {
        // 1. Svincoliamo l'oggetto dal player
        heldObject.transform.parent = null;

        // 2. Riattiviamo la fisica
        heldObjRb.isKinematic = false;

        // 3. (Opzionale) Diamo una piccola spinta in avanti per lanciarlo via leggermente
        heldObjRb.AddForce(playerCamera.transform.forward * 2f, ForceMode.Impulse);

        heldObject = null;
    }
}