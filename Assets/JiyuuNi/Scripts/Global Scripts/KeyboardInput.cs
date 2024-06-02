using UnityEngine;
using UnityEngine.EventSystems;
using TiltFive;
using System;

public class KeyboardInput : BaseInput
{
    public string TurnInputName = "Horizontal";
    public string AccelerateButtonName = "Accelerate";
    public string BrakeButtonName = "Brake";

    // "Dead Zone" to smooth out jittery or accidental inputs
    public float deadZone = 0f;

    private bool isReverse = false;
    private TiltFiveProperties.T5ControllerMode lastT5Orientation;

    private bool isStarted = false;
    //private string thisClass = nameof(KeyboardInput);

    public override InputData GenerateInput()
    {
        if (!isStarted)
        {
            ExtendedInputModule tempInput = FindFirstObjectByType<ExtendedInputModule>();
            lastT5Orientation = tempInput.lastT5Orientation;
            isStarted = true;
        }

        InputData result = new InputData
        {
            Accelerate = false,
            Brake = false,
            TurnInput = 0f
        };

        /*
         * Possible expected sources of input (based on how Unity's
         *    legacy input system understands them):
         * - TiltFive Wand (as either controller or pointer)
         * - Android touchscreen
         * - Gamepad or keyboard
         */
        InputData tiltFiveInput = checkTiltFiveInput();
        InputData androidInput = checkAndroidInput();
        InputData pcInput = checkPCInput();

        if (androidInput.Accelerate || pcInput.Accelerate || tiltFiveInput.Accelerate)
        {
            result.Accelerate = true;
        }

        if (androidInput.Brake || pcInput.Brake || tiltFiveInput.Brake)
        {
            result.Brake = true;
        }

        // Turn input is a bit tricky, since only one input should control turning at
        //    any given time. Since this build is focused on TiltFive and Android, the
        //    default priority order will be as follows:
        //    1) Tilt Five Wand (as either controller or pointer)
        //    2) Android touchscreen
        //    3) Gamepad or keyboard
        if (tiltFiveInput.TurnInput != 0)
        {
            result.TurnInput = tiltFiveInput.TurnInput;
        }
        else if (androidInput.TurnInput != 0)
        {
            result.TurnInput = androidInput.TurnInput;
        }
        else
        {
            result.TurnInput = pcInput.TurnInput;
        }

        return result;
    }

    private InputData checkAndroidInput()
    {
        InputData result = new InputData
        {
            Accelerate = false,
            Brake = false,
            TurnInput = 0f
        };

        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            // If touchCount isn't checked, touchscreen input will block
            //    any other input from working while on a mobile device
            if (UnityEngine.Input.touchCount > 0)
            {
                Touch oneFinger = UnityEngine.Input.GetTouch(0);

                if (oneFinger.phase != TouchPhase.Canceled &&
                    oneFinger.phase != TouchPhase.Ended)
                {
                    if (oneFinger.deltaPosition.y < -60f)
                    {
                        isReverse = true;
                    }

                    if (isReverse)
                    {
                        result.Accelerate = false;
                        result.Brake = true;
                    }
                    else
                    {
                        result.Accelerate = true;
                        result.Brake = false;
                    }

                    // Do nothing for steering if there's no change in position

                    if (oneFinger.deltaPosition.x > 2f)
                    {
                        result.TurnInput = 0.8f;
                    }

                    if (oneFinger.deltaPosition.x < -2f)
                    {
                        result.TurnInput = -0.8f;
                    }
                }
                else
                {
                    if (isReverse)
                        isReverse = false;
                }
            }
        }

        return result;
    }

    private InputData checkPCInput()
    {
        InputData result = new InputData
        {
            Accelerate = UnityEngine.Input.GetButton(AccelerateButtonName),
            Brake = UnityEngine.Input.GetButton(BrakeButtonName),
            TurnInput = UnityEngine.Input.GetAxis("Horizontal")
        };

        return result;
    }

    private InputData checkTiltFiveInput()
    {
        InputData result = GetCoordinatesFromWand();

        return result;
    }

    private TiltFiveProperties.T5ControllerMode GetT5ControllerOrientation(Vector3 currentRotation)
    {
        TiltFiveProperties.T5ControllerMode result = lastT5Orientation;

        if (TiltFive.Input.GetWandAvailability() && TiltFive.Wand.IsTracked())
        {
            if ((currentRotation.x >= 315f || currentRotation.x <= 45f) &&
               (currentRotation.y >= 245f && currentRotation.y <= 295f))
            {
                result = TiltFiveProperties.T5ControllerMode.GamepadLeft;
            }
            else if ((currentRotation.x >= 315f || currentRotation.x <= 45f) &&
           (currentRotation.y >= 65f && currentRotation.y <= 115f))
            {
                result = TiltFiveProperties.T5ControllerMode.GamepadRight;
            }
            else
            {
                result = TiltFiveProperties.T5ControllerMode.Wand;
            }

            lastT5Orientation = result;
            //print($"KeyboardInput: {lastT5Orientation}");
        }

        return result;
    }

    private InputData GetCoordinatesFromWand()
    {
        InputData result = new InputData
        {
            Accelerate = false,
            Brake = false,
            TurnInput = 0f
        };

        if (TiltFive.Input.GetWandAvailability())
        {
            Vector2 stickCoordinates = TiltFive.Input.GetStickTilt();
            Quaternion wandRotation = Wand.GetRotation(ControllerIndex.Right);
            TiltFiveProperties.T5ControllerMode currentMode = lastT5Orientation;

            // If wand is tracked and board has been moved, tracking is tied to
            //    the board so wand coordinates must be compensated accordingly
            if (wandRotation != Quaternion.identity)
            {
                float boardRotationY =
                    PreferenceManager.Instance.GetT5Board().transform.rotation.eulerAngles.y;

                wandRotation = Quaternion.Euler(
                    wandRotation.eulerAngles.x,
                    wandRotation.eulerAngles.y - boardRotationY,
                    wandRotation.eulerAngles.z);

                currentMode = GetT5ControllerOrientation(wandRotation.eulerAngles);
            }

            if (currentMode.Equals(TiltFiveProperties.T5ControllerMode.GamepadLeft))
            {
                //print($"{thisClass}: Gamepad, left");

                // Gamepad, pointing left
                if (stickCoordinates.y != 0)
                {
                    if (-1 * stickCoordinates.y > 0)
                    {
                        result.TurnInput = 1f;
                    }
                    else
                    {
                        result.TurnInput = -1f;
                    }
                }

                if (TiltFive.Input.GetButton(TiltFive.Input.WandButton.A))
                {
                    //print($"{thisClass}: Wand button A");
                    result.Accelerate = true;
                }

                if (TiltFive.Input.GetButton(TiltFive.Input.WandButton.B))
                {
                    //print($"{thisClass}: Want button B");
                    result.Brake = true;
                }
            }
            else if(currentMode.Equals(TiltFiveProperties.T5ControllerMode.GamepadRight))
            {
                // Gamepad, pointing right
                if (stickCoordinates.y != 0)
                {
                    if (stickCoordinates.y > 0)
                    {
                        result.TurnInput = 1f;
                    }
                    else
                    {
                        result.TurnInput = -1f;
                    }
                }

                if (TiltFive.Input.GetButton(TiltFive.Input.WandButton.Y))
                {
                    //print($"{thisClass}: Wand button Y");
                    result.Accelerate = true;
                }

                if (TiltFive.Input.GetButton(TiltFive.Input.WandButton.B))
                {
                    //print($"{thisClass}: Wand button B");
                    result.Brake = true;
                }
            }
            else
            {
                //print($"{thisClass}: Wand");

                // Wand
                if (stickCoordinates.x != 0)
                {
                    if (stickCoordinates.x > 0)
                    {
                        result.TurnInput = 1f;
                    }
                    else
                    {
                        result.TurnInput = -1f;
                    }
                }

                if (TiltFive.Input.GetTrigger() > 0.3f)
                {
                    //print($"{thisClass}: Wand trigger");
                    result.Accelerate = true;
                }

                if (TiltFive.Input.GetButton(TiltFive.Input.WandButton.One))
                {
                    //print($"{thisClass}: Wand button one");
                    result.Brake = true;
                }
            }
        }

        return result;
    }
}
