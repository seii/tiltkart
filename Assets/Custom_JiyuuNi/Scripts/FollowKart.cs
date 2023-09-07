using UnityEngine;

[DefaultExecutionOrder(100)]
public class FollowKart : MonoBehaviour
{
    // The Game Board Transform.
    public Transform gameBoardTransform;

    // The object to be followed.
    public Transform followObject;

    // Adjustments to position
    public Vector3 positionAdjustment;

    // Adjustments to rotation
    public Vector3 rotationAdjustment;

    // Update is called once per frame
    void Update()
    {
        gameBoardTransform.position = followObject.position;
        gameBoardTransform.Translate(positionAdjustment);

        // Cancel out X and Z elements of the follow object's rotation (to
        //    prevent motion sickness during collisions)
        Quaternion tempRotate = Quaternion.Euler(0, followObject.rotation.eulerAngles.y, 0);
        gameBoardTransform.rotation = tempRotate;
        gameBoardTransform.Rotate(rotationAdjustment);
    }
}