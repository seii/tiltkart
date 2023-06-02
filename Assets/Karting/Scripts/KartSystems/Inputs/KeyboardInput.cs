using UnityEngine;
using TiltFive;

namespace KartGame.KartSystems {

    public class KeyboardInput : BaseInput
    {
        public string TurnInputName = "Horizontal";
        public string AccelerateButtonName = "Accelerate";
        public string BrakeButtonName = "Brake";

        public override InputData GenerateInput() {
            InputData result;

            if (TiltFive.Input.GetWandAvailability())
            {
                result = new InputData
                {
                    // Wand controller button "A"
                    Accelerate = TiltFive.Input.GetButton(TiltFive.Input.WandButton.A),
                    // Wand controller button "B"
                    Brake = TiltFive.Input.GetButton(TiltFive.Input.WandButton.B),
                    // Wand controller stick, assuming that the user will turn the wand on its side to use it like a gamepad
                    TurnInput = TiltFive.Input.GetStickTilt().y * -1
                };
            }else if (SystemInfo.deviceType == DeviceType.Handheld)
            {
                Touch oneFinger = UnityEngine.Input.GetTouch(0);
                bool fingerMove = false;
                float xAxis = 0f;


                if (oneFinger.phase != TouchPhase.Canceled && oneFinger.phase != TouchPhase.Ended)
                {
                    fingerMove = true;
                }
                else
                {
                    fingerMove = false;
                }

                if(Mathf.Abs(oneFinger.deltaPosition.x) > 0.1f)
                {
                    xAxis = oneFinger.deltaPosition.x;
                }

                if(oneFinger.phase == TouchPhase.Ended)
                {
                    xAxis = 0f;
                }

                result = new InputData
                {
                    Accelerate = fingerMove,
                    Brake = !fingerMove,
                    TurnInput = xAxis
                };
            }
            else
            {
                result = new InputData
                {
                    Accelerate = UnityEngine.Input.GetButton(AccelerateButtonName),
                    Brake = UnityEngine.Input.GetButton(BrakeButtonName),
                    TurnInput = UnityEngine.Input.GetAxis("Horizontal")
                };
            }

            return result;
        }
    }
}
