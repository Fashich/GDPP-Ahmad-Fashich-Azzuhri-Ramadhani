using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
void Update(){
    CheckJumpInput();
    CheckSprintInput();
    CheckCrouchInput();
    CheckChangePOVInput();
    CheckClimbInput();
    CheckGlideInput();
    CheckCancelInput();
    CheckPunchInput();
    CheckMainMenuInput();
}
private void CheckJumpInput(){
    bool isPressJumpInput = Input.GetKeyDown(KeyCode.Space);
    if (isPressJumpInput) {
        Debug.Log("Jump");
    }
}
private void CheckSprintInput(){
    bool isHoldSprintInput = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    if (isHoldSprintInput){
        Debug.Log("Sprinting");
    }
    else {
        Debug.Log("Not Sprinting");
    }
}
private void CheckCrouchInput(){
    bool isPressCrouchInput = Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl);
    if (isPressCrouchInput) {
        Debug.Log("Crouch");
    }
    else {
        Debug.Log("Standing");
    }
}
private void CheckChangePOVInput() {
    bool isPressChangePOVInput = Input.GetKeyDown(KeyCode.Q);
    if (isPressChangePOVInput){
        Debug.Log("Change POV: First Person Camera");
    }
    else {
        Debug.Log("Change POV: Third Person Camera");
    }}
    private void CheckClimbInput(){
        bool isPressClimbInput = Input.GetKeyDown(KeyCode.E);
        if (isPressClimbInput){
            Debug.Log("Climb");
        }
    }
    private void CheckGlideInput(){
        bool isPressGlideInput = Input.GetKeyDown(KeyCode.G);
        if (isPressGlideInput){
            Debug.Log("Glide");
        }
    }
    private void CheckCancelInput(){
        bool isPressCancelInput = Input.GetKeyDown(KeyCode.C);
        if (isPressCancelInput){
            Debug.Log("Cancel Climb or Glide");
        }
        }
        private void CheckPunchInput(){
            bool isPressPunchInput = Input.GetKeyDown(KeyCode.Moused);
            if (isPressPunchInput){
                Debug.Log("Punch");
            }
        }
        private void CheckMainMenuInput(){
            bool isPressMainMenuInput = Input.GetKeyDown(KeyCode.Escape);
            if (isPressMainMenuInput){
                Debug.Log("Back To Main Menu");
            }
        }
}
