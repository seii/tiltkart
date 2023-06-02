using UnityEngine;

[DefaultExecutionOrder(100)]
public class FollowKart : MonoBehaviour
{
    // The Game Board Transform.
    public Transform gameBoardTransform;

    // The object to be followed.
    public Transform followObject;

    // Update is called once per frame
    void Update()
    {
        gameBoardTransform.position = followObject.position;
        gameBoardTransform.rotation = followObject.rotation;
    }
}