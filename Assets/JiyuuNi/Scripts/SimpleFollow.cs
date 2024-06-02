using UnityEngine;
using UnityEditor;

/**
 * Anything this script is attached to (and all its children)
 * will "follow" the position of the specified GameObject.
 */
public class SimpleFollow : MonoBehaviour
{
    // The object to be followed.
    [HideInInspector, SerializeField]
    private GameObject followObject;

    [HideInInspector, SerializeField]
    private bool isOffsetFromFollow;

    // Distance from follow object
    [HideInInspector, SerializeField]
    private float distanceFromFollow;

    // Adjustments to position
    [HideInInspector, SerializeField]
    private Vector3 positionAdjustment;

    // Adjustments to rotation
    [HideInInspector, SerializeField]
    private Vector3 rotationAdjustment;

    private string thisClass = nameof(SimpleFollow);

    private void OnEnable()
    {
        KartSelector.onKartChange += UpdateFollowObject;
    }

    private void Start()
    {
        Follow();
    }

    // Update is called once per frame
    private void Update()
    {
        Follow();
    }

    private void OnDisable()
    {
        KartSelector.onKartChange -= UpdateFollowObject;
    }

    private void Follow()
    {
        Vector3 followPos = followObject.transform.position;
        transform.position = followPos;

        if (isOffsetFromFollow)
        {
            Vector3 adjustedPosition = Vector3.forward * distanceFromFollow;
            adjustedPosition += positionAdjustment;
            adjustedPosition = Quaternion.Euler(0, followObject.transform.rotation.eulerAngles.y, 0) * adjustedPosition;
            transform.position += adjustedPosition;
        }

        // Cancel out X and Z elements of the follow object's rotation (to
        //    prevent motion sickness during collisions)
        transform.rotation = Quaternion.Euler(0, followObject.transform.rotation.eulerAngles.y, 0);
        transform.Rotate(rotationAdjustment);
    }

    public void UpdateFollowObject(GameObject newFollow)
    {
        print($"{thisClass}: Updating follow object to {newFollow.name}");
        followObject = newFollow;
    }

    /**
     * <credit>
     * Frankie Griffin - Stack Overflow - https://stackoverflow.com/a/72735168
     * </credit>
     */
#if UNITY_EDITOR
    [CustomEditor(typeof(SimpleFollow)), CanEditMultipleObjects]
    public class FollowObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Call normal GUI (displaying any variables you might have)
            base.OnInspectorGUI();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("followObject"), new GUIContent("Object To Follow"));

            SerializedProperty tempOffset = serializedObject.FindProperty("isOffsetFromFollow");

            EditorGUILayout.PropertyField(tempOffset, new GUIContent("Offset From Follow"));

            if (tempOffset.boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("distanceFromFollow"), new GUIContent("Distance From Follow"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("positionAdjustment"), new GUIContent("Position"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("rotationAdjustment"), new GUIContent("Rotation"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}