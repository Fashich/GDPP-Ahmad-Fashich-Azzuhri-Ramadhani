// // using System;
// // using System.Collections;
// // using System.Collections.Generic;
// // using UnityEngine;

// // public class PlayerMovement : MonoBehaviour
// // {
// // [SerializeField]
// // private float _walkSpeed;
// // [SerializeField]
// // private InputManager _input;
// // private void Start() {
// //     _input.OnMoveInput += Move;
// // }
// // private void OnDestroy(){
// // _input.OnMoveInput -= Move;
// // }
// // private void Move(Vector2 axisDirection){
// // Vector3 movementDirection = new Vector3(axisDirection.x, 0, axisDirection.y);
// // Debug.Log(movementDirection);
// // }
// // }
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// // public class Player : MonoBehaviour
// // {
// //     [SerializeField]
// //     private float _speed;
// //     private Rigidbody _rigidbody;
// //     private void Awake() {
// //         _rigidbody = GetComponent<Rigidbody>();
// //     }

// //     void FixedUpdate()
// //     {
// //         float horizontal = Input.GetAxis("Horizontal");
// //         float vertical = Input.GetAxis("Vertical");
// //         Vector3 movementDirection = new Vector3(horizontal, 0, vertical);
// //         //_rigidbody.MovePosition(transform.position + movementDirection * _speed * Time.deltaTime);
// //         _rigidbody.linearVelocity = movementDirection * _speed * Time.fixedDeltaTime;
// //         Debug.Log("Horizontal: " + horizontal);
// //         Debug.Log("Vertical: " + vertical);
// //         if (!Physics.Raycast(transform.position, movementDirection, 0.5f))
// //         {
// //             _rigidbody.MovePosition(
// //                 transform.position +
// //                 movementDirection * _speed * Time.fixedDeltaTime
// //             );
// //         }
// //     }
// // }
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _speed = 5f;
    [SerializeField] private InputManager _input;
    [SerializeField] private float _sprintMultiplier = 1.5f;
    [SerializeField] private float _crouchHeight = 0.8f;
    [SerializeField] private float _standHeight = 1.8f;
    [SerializeField] private float _jumpForce = 5f;

    // 🔼 Tambahkan variabel baru untuk climb (tanpa sistem jump attempt)
    [SerializeField] private float _climbSpeed = 3f;
    [SerializeField] private float _climbCheckDistance = 1.2f;
    [SerializeField] private LayerMask _climbableLayer; // Untuk deteksi dinding climbable

    [Header("Camera Settings")]
    [SerializeField] private float _mouseSensitivity = 10f;
    [SerializeField] private float _minVerticalAngle = -90f;
    [SerializeField] private float _maxVerticalAngle = 90f;

    // Tambahkan variabel untuk Power-Up
    // public bool canEatEnemies = false;
    // public float powerUpTimer = 0f; // Ubah menjadi public
    // private float powerUpDuration = 10f;
    // public AudioClip powerUpSound;
    // public AudioClip eatEnemySound;

    // Tambahkan variabel untuk posisi awal
    public Vector3 initialPosition;

    private Rigidbody _rigidbody;
    private Transform _cameraTransform;
    private float _verticalRotation = 0f;
    private BoxCollider _boxCollider;

    // Tambahkan variabel untuk state
    private bool _isSprinting;
    private bool _isCrouching;
    private bool _isGrounded;
    private bool _isClimbing;
    private bool _isGliding;
    private bool _isFirstPerson;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _boxCollider = GetComponent<BoxCollider>();

        // 🔧 PERBAIKAN 1: Force-initialize BoxCollider.size.y = _standHeight
        Vector3 initialSize = _boxCollider.size;
        initialSize.y = _standHeight;
        _boxCollider.size = initialSize;

        initialPosition = transform.position;

        // Setup camera
        GameObject mainCamera = GameObject.FindWithTag("MainCamera");
        if (mainCamera != null)
        {
            _cameraTransform = mainCamera.transform;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Debug.LogError("MainCamera not found! Please tag your camera with 'MainCamera'");
        }
    }

    private void Start()
    {
        if (_input != null)
        {
            _input.OnJump += Jump;
            _input.OnSprint += HandleSprint;
            _input.OnCrouch += HandleCrouch;
            _input.OnChangePOV += TogglePOV;
            _input.OnCancel += CancelAction;
        }
        else
        {
            Debug.LogError("InputManager reference not set in PlayerMovement!");
        }
    }

    private void OnDestroy()
    {
        if (_input != null)
        {
            _input.OnJump -= Jump;
            _input.OnSprint -= HandleSprint;
            _input.OnCrouch -= HandleCrouch;
            _input.OnChangePOV -= TogglePOV;
            _input.OnCancel -= CancelAction;
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleCameraRotation();
        CheckGrounded(); // Pastikan ini dipanggil di FixedUpdate

        if (_isClimbing)
        {
            HandleClimbing();
        }
    }

    // 🔧 PERBAIKAN UTAMA: Deteksi grounded dari kaki karakter dengan SphereCast
    private void CheckGrounded()
{
    // Hitung posisi dasar collider (kaki karakter)
    Vector3 raycastOrigin = transform.position + Vector3.down * (_boxCollider.size.y / 2);

    // 🔧 PERBAIKAN: Gunakan Raycast alih-alih SphereCast
    _isGrounded = Physics.Raycast(
        raycastOrigin,
        Vector3.down,
        0.2f // Jarak raycast lebih panjang
    );

    // Debug visual (wajib aktif untuk verifikasi)
    Debug.DrawRay(raycastOrigin, Vector3.down * 0.2f, _isGrounded ? Color.green : Color.red);
}

    private void HandleMovement()
    {
        float horizontal = 0f;
        float vertical = 0f;

        if (_input != null)
        {
            horizontal = _input.CurrentMovementInput.x;
            vertical = _input.CurrentMovementInput.y;
        }
        else
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
        }

        Vector3 movementDirection =
            transform.right * horizontal +
            transform.forward * vertical;

        movementDirection.y = 0;
        movementDirection = movementDirection.normalized;

        if (movementDirection != Vector3.zero)
        {
            float currentSpeed = _speed;
            if (_isSprinting) currentSpeed *= _sprintMultiplier;
            if (_isCrouching) currentSpeed *= 0.5f;

            if (!Physics.Raycast(transform.position, movementDirection, 0.5f))
            {
                _rigidbody.MovePosition(
                    transform.position +
                    movementDirection * currentSpeed * Time.fixedDeltaTime
                );
            }
        }
    }

    // 🔼 Perbaikan: Jump hanya saat grounded (tanpa sistem attempt/remaining)
    private void Jump()
    {
        // Cek apakah bisa memanjat (prioritas 1)
        if (!_isGrounded && !_isClimbing && IsNearClimbableSurface(out RaycastHit climbHit))
        {
            StartClimbing(climbHit.point);
            return;
        }

        // Jika tidak bisa memanjat, lakukan jump normal HANYA SAAT GROUNDED
        if (_isGrounded)
        {
            _rigidbody.linearVelocity = new Vector3(
                _rigidbody.linearVelocity.x,
                0,
                _rigidbody.linearVelocity.z
            );
            _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.VelocityChange);
        }
    }

    private void HandleSprint(bool isSprinting)
    {
        _isSprinting = isSprinting;
    }

    private void HandleCrouch()
    {
        _isCrouching = !_isCrouching;

        // ✅ Perbaikan: Gunakan .size.y untuk BoxCollider
        Vector3 newSize = _boxCollider.size;
        newSize.y = _isCrouching ? _crouchHeight : _standHeight;
        _boxCollider.size = newSize;

        // Sesuaikan posisi agar karakter tidak "melayang" saat crouch
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y + (_isCrouching ? -0.5f : 0.5f),
            transform.position.z
        );
    }

    private void TogglePOV()
    {
        _isFirstPerson = !_isFirstPerson;
        _cameraTransform.localPosition = _isFirstPerson
            ? new Vector3(0, 1.2f, 0)
            : new Vector3(0, 1.8f, -3f);
    }

    private void CancelAction()
    {
        if (_isClimbing || _isGliding)
        {
            _isClimbing = false;
            _isGliding = false;
            Debug.Log("Action canceled: Climbing/Gliding stopped");
        }
    }

    private void HandleCameraRotation()
    {
        if (_cameraTransform == null) return;

        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;

        _verticalRotation -= mouseY;
        _verticalRotation = Mathf.Clamp(_verticalRotation, _minVerticalAngle, _maxVerticalAngle);

        _cameraTransform.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    // 🔧 PERBAIKAN KRITIS: Fix error CS1620 dengan QueryTriggerInteraction
    private bool IsNearClimbableSurface(out RaycastHit hit)
    {
        return Physics.Raycast(
            transform.position + Vector3.up * 0.5f,
            transform.forward,
            out hit,
            _climbCheckDistance,
            _climbableLayer,
            QueryTriggerInteraction.UseGlobal // 🔥 PENAMBAHAN KRITIS
        );
    }

    private void StartClimbing(Vector3 contactPoint)
    {
        _isClimbing = true;
        _rigidbody.isKinematic = true; // Nonaktifkan fisika saat memanjat

        // Align karakter ke dinding
        Quaternion targetRotation = Quaternion.LookRotation(-transform.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.5f);

        Debug.Log("Started climbing!");
    }

    private void HandleClimbing()
    {
        float verticalInput = Input.GetAxis("Vertical");

        // Gerakan vertikal saat memanjat
        if (Mathf.Abs(verticalInput) > 0.1f)
        {
            Vector3 climbDirection = Vector3.up * verticalInput;
            transform.position += climbDirection * _climbSpeed * Time.fixedDeltaTime;
        }

        // Otomatis berhenti jika mencapai puncak (deteksi dengan raycast ke atas)
        if (Physics.Raycast(transform.position, Vector3.up, 0.5f))
        {
            StopClimbing();
        }
    }

    private void StopClimbing()
    {
        _isClimbing = false;
        _rigidbody.isKinematic = false;
        Debug.Log("Stopped climbing!");
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * 0.5f);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, Vector3.down * 0.1f);

            // 🔼 Tambahkan: Visualisasi raycast climb (tanpa menghapus kode lama)
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(
                transform.position + Vector3.up * 0.5f,
                transform.forward * _climbCheckDistance
            );
        }
    }
}