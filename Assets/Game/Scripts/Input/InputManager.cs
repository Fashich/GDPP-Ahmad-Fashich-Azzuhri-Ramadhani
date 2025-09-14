using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Action<Vector2> OnMoveInput;
    // Tambahkan properti untuk menyimpan state movement
    public Vector2 CurrentMovementInput { get; private set; } // 👈 BARIS BARU

    public Action OnJump;
    public Action<bool> OnSprint;
    public Action OnCrouch;
    public Action OnChangePOV;
    public Action OnCancel;
    public Action OnClimb;
    public Action OnGlide;
    public Action OnPunch;
    public Action OnMainMenu;

    private bool _wasSprinting = false;
    private bool _wasCrouching = false;

    private void Update(){
        CheckJumpInput();
        CheckSprintInput();
        CheckCrouchInput();
        CheckChangePOVInput();
        CheckClimbInput();
        CheckGlideInput();
        CheckCancelInput();
        CheckPunchInput();
        CheckMainMenuInput();
        CheckMovementInput();
    }

    private void CheckMovementInput(){
        float verticalAxis = Input.GetAxis("Vertical");
        float horizontalAxis = Input.GetAxis("Horizontal");
        Vector2 inputAxis = new Vector2(horizontalAxis, verticalAxis);

        // Simpan state movement terbaru
        CurrentMovementInput = inputAxis; // 👈 BARIS BARU

        if (OnMoveInput != null){
            OnMoveInput(inputAxis);
        }
    }
    private void CheckJumpInput(){
        if (Input.GetKeyDown(KeyCode.Space)) {
            OnJump?.Invoke(); // Trigger event
        }
    }

    private void CheckSprintInput(){
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (isSprinting != _wasSprinting) {
            OnSprint?.Invoke(isSprinting); // Trigger event
            _wasSprinting = isSprinting;
        }
    }

    private void CheckCrouchInput(){
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)) {
            OnCrouch?.Invoke(); // Trigger event
            _wasCrouching = !_wasCrouching;
        }
    }

    private void CheckChangePOVInput() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            OnChangePOV?.Invoke(); // Trigger event
        }
    }

    private void CheckClimbInput(){
        if (Input.GetKeyDown(KeyCode.E)) {
            OnClimb?.Invoke(); // Trigger event
        }
    }

    private void CheckGlideInput(){
        if (Input.GetKeyDown(KeyCode.G)) {
            OnGlide?.Invoke(); // Trigger event
        }
    }

    private void CheckCancelInput(){
        if (Input.GetKeyDown(KeyCode.C)) {
            OnCancel?.Invoke(); // Trigger event
        }
    }

    private void CheckPunchInput(){
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            OnPunch?.Invoke(); // Trigger event
        }
    }

    private void CheckMainMenuInput(){
        if (Input.GetKeyDown(KeyCode.Escape)) {
            OnMainMenu?.Invoke(); // Trigger event
        }
    }
}