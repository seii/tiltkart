using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GhostTransform
{
    public Vector3 position;
    public Quaternion rotation;

    public GhostTransform(Transform ghostKart)
    {
        position = ghostKart.position;
        rotation = ghostKart.rotation;
    }
}

/**
 * Manager for non-colliding kart objects used to show the player their previous
 * lap path ("ghost karts")
 */
public class GhostKartManager : MonoBehaviour
{
    private Transform kart;
    private Transform ghostKart;
    private bool recording = false;
    private bool playing = false;

    private Queue<GhostTransform> previousLapGhostTransforms = new Queue<GhostTransform>();
    private Queue<GhostTransform> currentLapGhostTransforms = new Queue<GhostTransform>();
    private GhostTransform lastRecordedGhostTransform;

    private string thisClass = nameof(GhostKartManager);

    // Start is called before the first frame update
    void Start()
    {
        ObjectiveCompleteLaps.OnStartLap += LapCheck;
        ObjectiveCompleteLaps.OnEndLastLap += LastLap;
        ResetToStartingLine.OnOutOfBounds += ResetGhostKartLap;

        DetectGhosts();
    }

    private void DetectGhosts()
    {
        GameObject[] allGhosts = GameObject.FindGameObjectsWithTag("Ghost");
        if (allGhosts != null && allGhosts.Length > 0)
        {
            foreach (GameObject ghost in allGhosts)
            {
                //print($"{thisClass}: Checking ghost: " + ghost.name);
                if (ghost.name.StartsWith(PlayerPrefs.GetString("Kart")))
                {
                    print($"{thisClass}: Ghost " + ghost.name + " found!");
                    ghostKart = ghost.transform;
                }

                // All ghosts should start the race deactivated
                ghost.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    protected void Update()
    {
        if (recording)
        {
            //print($"{thisClass}: Recording!");
            if (kart.position != lastRecordedGhostTransform.position)
            {
                //print($"{thisClass}: Recording position " + kart.position + " and rotation " + kart.rotation);
                var newGhostTransform = new GhostTransform(kart);
                currentLapGhostTransforms.Enqueue(newGhostTransform);

                lastRecordedGhostTransform = newGhostTransform;
            }
        }

        if (playing)
        {
            Play();
        }
    }

    private void OnDisable()
    {
        ObjectiveCompleteLaps.OnStartLap -= LapCheck;
        ObjectiveCompleteLaps.OnEndLastLap -= LastLap;
        ResetToStartingLine.OnOutOfBounds -= ResetGhostKartLap;
    }

    private void Play()
    {
        if(previousLapGhostTransforms != null && previousLapGhostTransforms.Count > 0)
        {
            if(!ghostKart.gameObject.activeInHierarchy)
            {
                print($"{thisClass}: Setting ghost kart to active");
                ghostKart.gameObject.SetActive(true);
                lastRecordedGhostTransform = new GhostTransform(kart);
            }
            
            StartCoroutine(StartGhost());
        }
    }

    private IEnumerator StartGhost()
    {
        GhostTransform currentKartFrame;
        bool isGhostframePresent = previousLapGhostTransforms.TryDequeue(out currentKartFrame);

        if (isGhostframePresent)
        {
            Vector3 tempPosition = currentKartFrame.position;
            ghostKart.position = new Vector3(tempPosition.x += Time.deltaTime, tempPosition.y += Time.deltaTime, tempPosition.z += Time.deltaTime);
            ghostKart.rotation = currentKartFrame.rotation;
            //Debug.Log($"Ghost position is {currentKartFrame.position} and rotation is {currentKartFrame.rotation}");
        }

        yield return new WaitForFixedUpdate();
    }

    private void LapCheck()
    {
        // Race start
        // Theoretically both "recording" and "playing" would also be false at the end of a race,
        //    but we should be leaving the scene when that happens
        if (!recording && !playing)
        {
            print($"{thisClass}: Starting recording");
            recording = true;

            GameObject[] tempKarts = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject oneKart in tempKarts)
            {
                if(oneKart.name.EndsWith("_Player"))
                {
                    print($"{thisClass}: Found player kart: " + oneKart.name);
                    kart = oneKart.transform;
                    print($"{thisClass}: Transform name: " + kart.name);
                }
                
            }
        }
        // Mid-race lap
        else
        {
            ResetGhostKartLap("mid-lap");

            //print($"{thisClass}: Size of previous is " + previousLapGhostTransforms.Count +
            //    " and current is " + currentLapGhostTransforms.Count);
            playing = true;
        }
    }

    private void ResetGhostKartLap(string collider)
    {
        print($"{thisClass}: Storing previous lap's data");
        GhostTransform[] tempArray = new GhostTransform[currentLapGhostTransforms.Count];
        currentLapGhostTransforms.CopyTo(tempArray, 0);
        previousLapGhostTransforms = new Queue<GhostTransform>(tempArray);
        currentLapGhostTransforms.Clear();
    }

    private void LastLap()
    {
        print($"{thisClass}: Stopping recording");
        recording = false;
        playing = false;
    }
}
