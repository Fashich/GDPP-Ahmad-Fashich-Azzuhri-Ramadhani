using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
void Update(){
    CheckJumpInput();
    CheckSprintInput();
    CheckCrouchInput();
    // CheckChangePOVInput();
    // CheckClimbInput();
    // CheckGlideInput();
    // CheckCancelInput();
    // CheckPunchInput();
    // CheckMainMenuInput();
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
        Debug.Log("Standing")
    }
}
}
