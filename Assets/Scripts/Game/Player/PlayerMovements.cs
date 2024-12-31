using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// Handles the player's movements
/// </summary>
public class PlayerMovements : MonoBehaviour
{
    [Header("Stand")]
    [SerializeField] private float speedNormal;
    [SerializeField] private float sizeNormal;
    [SerializeField] private float cameraNormal;

    [Header("Crouch")]
    [SerializeField] private float speedCrouch;
    [SerializeField] private float sizeCrouch;
    [SerializeField] private float crouchSmooth;
    [SerializeField] private float cameraCrouch;

    [Header("Looking")]
    [SerializeField] private int sensibility;
    [SerializeField] private Transform playerRoot;
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private float maxXRot = 80;
    private float xrot = 0;


    [Header("Jumping")]
    [SerializeField] private float jumpForce;
    [SerializeField] private Transform groundChecker;
    [SerializeField] private float jumpRaycastLength = 0.15f;
    [SerializeField] private float jumpRaycastSize = 0.1f;


    [Header("Misc")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CapsuleCollider playerCollider;
    [SerializeField] private Animator handsAnimator;


    [Header("Network")]
    [SerializeField] private NetworkedAnimator bodyAnimator;


    private bool crouched = false;
    private bool grounded = false;
    private bool running = false;

    void Start()
    {
        crouched = false;
        grounded = true;
        running = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(groundChecker.position, jumpRaycastSize);
        Gizmos.DrawLine(groundChecker.position, groundChecker.position - Vector3.up * jumpRaycastLength);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameGUI.instance.inMenu || !PlayerNetwork.localPlayer.health.Alive || !PlayerNetwork.localPlayer.CanMove) return;

        grounded = Physics.SphereCast(groundChecker.position, jumpRaycastSize, -Vector3.up, out _, jumpRaycastLength);
        running = Input.GetKey(KeyCode.LeftShift);

        bodyAnimator.SetBool("OnGround", grounded);

        // Movements
        if (grounded)
        {
            float currentSpeed = crouched ? speedCrouch : speedNormal * (running ? 1.5f : 1);
            Vector3 PlayerMovement = playerRoot.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"))) * currentSpeed * Time.timeScale;
            rb.linearVelocity = new Vector3(PlayerMovement.x, rb.linearVelocity.y, PlayerMovement.z);

            float animatorValue = (PlayerMovement.x == 0 || PlayerMovement.z == 0) ? 0 : running ? 2 : 1;
            bodyAnimator.SetFloat("Speed", animatorValue);
            handsAnimator.SetFloat("Speed", animatorValue);
        }
        else
        {
            handsAnimator.SetFloat("Speed", 0);
        }

        // Looking
        Vector2 camMovements = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        playerRoot.Rotate(0f, camMovements.x * sensibility * Time.timeScale, 0f);
        xrot = Mathf.Clamp(xrot - camMovements.y * sensibility * Time.timeScale, -maxXRot, maxXRot);
        cameraRoot.localRotation = Quaternion.Euler(xrot, 0f, 0f);

        // Crouch
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            crouched = !crouched;

            playerCollider.height = crouched ? sizeCrouch : sizeNormal;
            playerCollider.center = new Vector3(0,
                crouched ? sizeCrouch : sizeNormal / 2f
            , 0);
        }

        // Jump
        if (grounded && Input.GetKeyDown(KeyCode.Space))
        {
            bodyAnimator.SetTrigger("Jump");
            rb.AddForce(transform.up * jumpForce / (crouched ? 2f : 1f));
        }

        // Camera crouch lerp
        cameraRoot.localPosition = Vector3.Lerp(cameraRoot.localPosition,
        new Vector3(0, crouched ? cameraCrouch : cameraNormal, 0),
        Time.deltaTime * crouchSmooth);
    }
}
