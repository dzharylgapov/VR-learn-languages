using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class PlayerInputHandler : MonoBehaviour
    {

        [Tooltip("Sensitivity multiplier for moving the camera around")]
        public float LookSensitivity = 1f;

        [Tooltip("Additional sensitivity multiplier for WebGL")]
        public float WebglLookSensitivityMultiplier = 0.25f;

        [Tooltip("Limit to consider an input when using a trigger on a controller")]
        public float TriggerAxisThreshold = 0.4f;

        [Tooltip("Used to flip the vertical input axis")]
        public bool InvertYAxis = false;

        [Tooltip("Used to flip the horizontal input axis")]
        public bool InvertXAxis = false;

        PlayerCharacterController m_PlayerCharacterController;
        bool m_FireInputWasHeld;

        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            GetMouseInput();
        }

        public void GetMouseInput()
        {
            Camera mycam = GetComponentInChildren<Camera>();
            float rotateHorizontal = Input.GetAxis("Mouse X");
            float rotateVertical = Input.GetAxis("Mouse Y");
            transform.RotateAround(mycam.transform.position, Vector3.up, rotateHorizontal * 2); 
            //transform.Rotate(-transform.up * rotateHorizontal * 5); //instead if you dont want the camera to rotate around the player
            transform.RotateAround(mycam.transform.position, -transform.right, rotateVertical * 2); 
            //transform.Rotate(transform.right * rotateVertical * 5); //if you don't want the camera to rotate around the player


        }

        public int GetKeyboardInput()
        {
            if (Input.GetKeyDown(KeyCode.W))
                return 1;
            else if (Input.GetKeyDown(KeyCode.A))
                return 2;
            else if (Input.GetKeyDown(KeyCode.S))
                return 3;
            else if (Input.GetKeyDown(KeyCode.D))
                return 4;
            return 0;
        }

        public Vector3 GetMoveInput()
        {
            //if (CanProcessInput())
            {
                Vector3 move = new Vector3(Input.GetAxisRaw(GameConstants.k_AxisNameHorizontal), 0f,
                    Input.GetAxisRaw(GameConstants.k_AxisNameVertical));

                // constrain move input to a maximum magnitude of 1, otherwise diagonal movement might exceed the max move speed defined
                move = Vector3.ClampMagnitude(move, 1);

                return move;
            }

            //return Vector3.zero;
        }

        public float GetLookInputsHorizontal()
        {
            return GetMouseOrStickLookAxis(GameConstants.k_MouseAxisNameHorizontal,
                GameConstants.k_AxisNameJoystickLookHorizontal);
        }

        public float GetLookInputsVertical()
        {
            return GetMouseOrStickLookAxis(GameConstants.k_MouseAxisNameVertical,
                GameConstants.k_AxisNameJoystickLookVertical);
        }

        float GetMouseOrStickLookAxis(string mouseInputName, string stickInputName)
            {
                //if (CanProcessInput())
                {
                    // Check if this look input is coming from the mouse
                    bool isGamepad = Input.GetAxis(stickInputName) != 0f;
                    float i = isGamepad ? Input.GetAxis(stickInputName) : Input.GetAxisRaw(mouseInputName);

                    // handle inverting vertical input
                    if (InvertYAxis)
                        i *= -1f;

                    // apply sensitivity multiplier
                    i *= LookSensitivity;

                    if (isGamepad)
                    {
                        // since mouse input is already deltaTime-dependant, only scale input with frame time if it's coming from sticks
                        i *= Time.deltaTime;
                    }
                    else
                    {
                        // reduce mouse input amount to be equivalent to stick movement
                        i *= 0.01f;
    #if UNITY_WEBGL
                        // Mouse tends to be even more sensitive in WebGL due to mouse acceleration, so reduce it even more
                        i *= WebglLookSensitivityMultiplier;
    #endif
                    }

                    return i;
                }

                //return 0f;
            }
    }
}