using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Look Settings")]
    public float mouseSensitivity = 2f;

    [Header("Head Bob Settings")]
    public float bobFrequency = 1.5f;   
    public float bobAmplitude = 0.05f;  
    public float smooth = 10f;

    [Header("Gravity Settings")]
    public float gravity = -9.81f;
    public float groundCheckDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Head Sway Settings")]
    public float swayFrequency = 1.5f;   
    public float swayAmplitude = 0.03f;  
    public float swaySmooth = 10f;       

    CharacterController controller;
    Camera playerCamera;

    float xRotation = 0f; 
    Vector3 originalCameraPosition;
    float bobTimer = 0f;
    Vector3 velocity;
    bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        originalCameraPosition = playerCamera.transform.localPosition;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

  
    void Update()
    {
        
        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float currentSpeed = MovePlayer();
        LookAround();
        HeadBob(currentSpeed);
        CameraSway(currentSpeed);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    float MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * moveSpeed * Time.deltaTime);

        
        return new Vector2(moveX, moveZ).magnitude;
    }

    void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        
        transform.Rotate(Vector3.up * mouseX);
    }
    void HeadBob(float speed)
    {
        if (speed > 0.1f) 
        {
            bobTimer += Time.deltaTime * bobFrequency;
            float bobOffsetY = Mathf.Sin(bobTimer) * bobAmplitude;
            Vector3 targetPosition = originalCameraPosition + new Vector3(0, bobOffsetY, 0);
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, targetPosition, Time.deltaTime * smooth);
        }
        else
        {
            
            bobTimer = 0f;
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, originalCameraPosition, Time.deltaTime * smooth);
        }
    }
    void CameraSway(float speed)
    {
        if (speed > 0.1f)
        {
           
            float swayTimer = Time.time * swayFrequency;
            float swayAngle = Mathf.Sin(swayTimer) * swayAmplitude * 50f; 

            Quaternion targetRotation = Quaternion.Euler(0f, 0f, -swayAngle);
            playerCamera.transform.localRotation = Quaternion.Lerp(playerCamera.transform.localRotation, targetRotation, Time.deltaTime * swaySmooth);
        }
        else
        {
            
            playerCamera.transform.localRotation = Quaternion.Lerp(playerCamera.transform.localRotation, Quaternion.identity, Time.deltaTime * swaySmooth);
        }
    }
}
