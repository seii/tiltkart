using UnityEngine;
using TiltFive;
using System;

namespace KartGame.KartSystems {

    public class KeyboardInput : BaseInput
    {
        public string TurnInputName = "Horizontal";
        public string AccelerateButtonName = "Accelerate";
        public string BrakeButtonName = "Brake";

        // "Dead Zone" to smooth out jittery or accidental inputs
        public float deadZone = 0f;

        // Track current frame for input delays, if any
        private int accelDelay = 0;
        private int brakeDelay = 0;
        private int turnDelay = 0;

        public override InputData GenerateInput() {
            InputData result = new InputData
            {
                Accelerate = false,
                Brake = false,
                TurnInput = 0f
            };

            /*
             * Possible expected sources of input (based on how Unity's
             *    legacy input system understands them):
             * - Android touchscreen
             * - Gamepad or keyboard
             * - TiltFive Wand (as either controller or pointer)
             */
            InputData androidInput = checkAndroidInput();
            InputData pcInput = checkPCInput();
            InputData tiltFiveInput = checkTiltFiveInput();

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
            //    1) TiltFive Wand (as either controller or pointer)
            //    2) Android touchscreen
            //    3) Gamepad or keyboard
            if(tiltFiveInput.TurnInput != 0)
            {
                result.TurnInput = tiltFiveInput.TurnInput;
            }else if(androidInput.TurnInput != 0)
            {
                result.TurnInput = androidInput.TurnInput;
            }else
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
                Touch oneFinger = UnityEngine.Input.GetTouch(0);

                if (oneFinger.phase != TouchPhase.Canceled &&
                        oneFinger.phase != TouchPhase.Ended)
                {
                    result.Accelerate = true;
                }

                if (Mathf.Abs(oneFinger.deltaPosition.x) > 0.1f)
                {
                    result.TurnInput = oneFinger.deltaPosition.x;
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
            InputData result = new InputData
            {
                Accelerate = false,
                Brake = false,
                TurnInput = 0f
            };

            if (TiltFive.Input.GetWandAvailability())
            {
                // When the wand is in "controller" mode, the reverse of the stick's y-axis is,
                //    effectively, the x-axis for the player
                float xAxis = TiltFive.Input.GetStickTilt().y * -1;

                // TODO: Get TiltFive wand controls working

                // When the wand is used as a wand, use the x-axis of its tip to determine steering
                float xWand = TiltFive.Wand.GetPosition(
                    ControllerIndex.Right, ControllerPosition.Aim).x;

                // Since the user would have no need to use the stick if they use the wand,
                //    and there's no reliable way to know that the wand has been turned on its side,
                //    just prioritize the stick input so that the wand is only used if the stick
                //    is not.
                if (TiltFive.Input.GetButton(TiltFive.Input.WandButton.A))
                {
                    result.Accelerate = true;
                }else if(TiltFive.Input.GetTrigger() > 0.1f)
                {
                    result.Accelerate = true;
                }

                if (TiltFive.Input.GetButton(TiltFive.Input.WandButton.B))
                {
                    result.Brake = true;
                }/*else if(Math.Abs(xWand) != 0 &&
                    !TiltFive.Input.GetButton(TiltFive.Input.WandButton.Three))
                {
                    result.Brake = true;
                }*/

                if (Math.Abs(TiltFive.Input.GetStickTilt().y) != 0)
                {
                    result.TurnInput = xAxis;
                }/*else if(Math.Abs(xWand) != 0)
                {
                    result.TurnInput = xWand;
                }*/
            }

            return result;
        }
    }
}
