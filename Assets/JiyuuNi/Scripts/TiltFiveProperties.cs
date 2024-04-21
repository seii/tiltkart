using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/**
 * Each scene should have a way to request changes to the TiltFive board,
 * as well as a way to know which Canvas objects are set to World Space
 * for use with TiltFive instead of using another mode (such as "overlay")
 * normally intended for desktop
 */
[DefaultExecutionOrder(-1)]
public class TiltFiveProperties : MonoBehaviour
{
    #region Editor Fields
    [HideInInspector, SerializeField]
    private bool alterInitialValues = false;

    [HideInInspector, SerializeField]
    private Vector3 boardPosition;

    [HideInInspector, SerializeField]
    private Vector3 boardRotation;

    [HideInInspector, SerializeField]
    private Vector3 boardScale = Vector3.one;

    [HideInInspector, SerializeField]
    private bool isOffsetFromFollow = false;

    [HideInInspector, SerializeField]
    private GameObject followObject;

    // Distance from follow object
    [HideInInspector, SerializeField]
    private float distanceFromFollow;

    // Adjustments to position
    [HideInInspector, SerializeField]
    private Vector3 positionAdjustment;

    // Adjustments to rotation
    [HideInInspector, SerializeField]
    private Vector3 rotationAdjustment;

    [HideInInspector, SerializeField]
    [Tooltip("List any Canvas objects which use World Space (they need to be linked to the TiltFive camera)")]
    private List<Canvas> worldSpaceCanvases;
    #endregion

    #region Internal Fields
    private Transform board = null;
    private string thisClass = nameof(TiltFiveProperties);
    #endregion

    // Start is called before the first frame update
    private void Start()
    {
        print($"{thisClass}: Started");

        board = PreferenceManager.Instance.GetT5Board().transform;

        ChangePosition();

        // If canvases using "World Space" are set,
        // make sure they know to use the Tilt Five Camera object.
        if (worldSpaceCanvases != null && worldSpaceCanvases.Count > 0)
        {
            Camera t5Camera = PreferenceManager.Instance.GetT5Camera();

            foreach (Canvas tempCanvas in worldSpaceCanvases)
            {
                if (tempCanvas.renderMode == RenderMode.WorldSpace)
                {
                    print($"{thisClass}: Set {tempCanvas.name} world camera to {t5Camera.name}");
                    tempCanvas.worldCamera = t5Camera;
                }
            }
        }

        Follow();
    }

    // Update is called once per frame
    private void Update()
    {
        ChangePosition();
        Follow();
    }

    public void UpdateFollowObject(GameObject newFollowObject)
    {
        print($"Updated follow object to {newFollowObject.name}");
        followObject = newFollowObject;
        board = followObject.transform;
    }

    private void ChangePosition()
    {
        if(alterInitialValues)
        {
            if (board != null)
            {
                board.position = boardPosition;
                board.eulerAngles = boardRotation;
                board.localScale = boardScale;
            }
        }
    }

    private void Follow()
    {
        if(followObject != null)
        {
            //print($"Follow object name is {followObject.name}");
            Transform board = PreferenceManager.Instance.GetT5Board().transform;

            Vector3 followPos = followObject.transform.position;
            board.position = followPos;

            if (isOffsetFromFollow)
            {
                Vector3 adjustedPosition = Vector3.forward * distanceFromFollow;
                adjustedPosition += positionAdjustment;
                adjustedPosition = Quaternion.Euler(0, followObject.transform.rotation.eulerAngles.y, 0) * adjustedPosition;
                board.position += adjustedPosition;
            }

            // Cancel out X and Z elements of the follow object's rotation (to
            //    prevent motion sickness during collisions)
            board.rotation = Quaternion.Euler(0, followObject.transform.rotation.eulerAngles.y, 0);
            board.Rotate(rotationAdjustment);
        }
    }

    /**
     * <credit>
     * Frankie Griffin - Stack Overflow - https://stackoverflow.com/a/72735168
     * </credit>
     */
#if UNITY_EDITOR
    [CustomEditor(typeof(TiltFiveProperties)), CanEditMultipleObjects]
    public class FollowObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Call normal GUI (displaying any variables you might have)
            base.OnInspectorGUI();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("worldSpaceCanvases"), new GUIContent("World Space Canvases"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("followObject"), new GUIContent("Follow Object"));

            SerializedProperty tempValues = serializedObject.FindProperty("alterInitialValues");

            EditorGUILayout.PropertyField(tempValues, new GUIContent("Alter Board Defaults"));

            if(tempValues.boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("boardPosition"), new GUIContent("Position"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("boardRotation"), new GUIContent("Rotation"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("boardScale"), new GUIContent("Scale"));
            }

            SerializedProperty tempOffset = serializedObject.FindProperty("isOffsetFromFollow");
            
            EditorGUILayout.PropertyField(tempOffset, new GUIContent("Offset From Follow"));

            if(tempOffset.boolValue)
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
