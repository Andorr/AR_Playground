using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class CubeManager : MonoBehaviour, ITrackableEventHandler
{
    protected TrackableBehaviour mTrackableBehaviour;
    public bool cubeSeen = false;

    #region EVENTS
    public delegate void CubeAction();

    public static event CubeAction OnTurn; // -> the cube has been turned, one of the 3 sides (sideUp, sideCam or sideRight) have been changed
    public static event CubeAction OnFound; // -> is called when the cube is seen, but it was not seen before
    public static event CubeAction OnLost; // -> the cube is not tracked
    public static event CubeAction DirectionFinderFail; // -> *DirectionFinder()* couldn't understand the turn of the cube, so none of the turning events have been called

    public static event CubeAction Left;
    public static event CubeAction Right;
    public static event CubeAction Up;
    public static event CubeAction Down;
    // 90 degrees tilt to the right on the side along the camera-facing axis
    // Please, message me, if you know a better name: masha@moviemask.io
    public static event CubeAction Flip;
    // 90 degrees tilt to the left on the side along the camera-facing axis
    public static event CubeAction Flop;
    #endregion


    #region SIDES OF THE CUBE

    public int sideCam; public int sideCamPrev;
    public int sideUp; public int sideUpPrev;
    public int sideRight; public int sideRightPrev;

    private readonly string[] sideName = new string[] { "MagicCube", "Person", "Cubes", "Rocket", "Globe", "Logo" };
    public const int MagicCube = 0;
    public const int Person = 1;
    public const int Cubes = 2;
    public const int Rocket = 3;
    public const int Globe = 4;
    public const int Logo = 5;
    #endregion


    private void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();

        //Register this class as an event handler so you can get the informaiton 
        // about the status change of the cube (if it is tracked or lost)
        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);

        // Find the direction of the turn every time the cube is turned or found
        OnTurn += DirectionFinder;
        OnFound += DirectionFinder;

    }
    private void OnDestroy()
    {
        //Unsubscribe to the events about the cube if it is destroyed
        if (mTrackableBehaviour)
            mTrackableBehaviour.UnregisterTrackableEventHandler(this);
    }

    // Checking all the time if the side facing the camera or up or right has been changed when the cube is seen
    void Update()
    {
        if (cubeSeen)
        {
            // temp vars
            int sideCamTemp = sideCam;
            int sideUpTemp = sideUp;
            int sideRightTemp = sideRight;

            // Find the sides facing 3 directions
            sideCamTemp = getSide(Vector3.back);
            sideUpTemp = getSide(Vector3.up);
            sideRightTemp = getSide(Vector3.right);

            // Check if any of the sides have been changed
            if (sideUpTemp != sideUp || sideCamTemp != sideCam || sideRightTemp != sideRight)
            {
                Debug.Log("Turning");
                // Set the current side to be the previous seen side
                sideUpPrev = sideUp;
                sideCamPrev = sideCam;
                sideRightPrev = sideRight;

                // Set the newly found sides to be the current sides
                sideUp = sideUpTemp;
                sideCam = sideCamTemp;
                sideRight = sideRightTemp;

                //Notify that the cube has been turned
                OnTurn();
            }

        }
        
    }

    private int getSide(Vector3 v)
    {
        float[] angles = new float[6];
        float lowestAngle = 200;
        int side = -1;

        angles[0] = Vector3.Angle(transform.up, v);              //       magic cube
        angles[1] = Vector3.Angle(-transform.up, v);             //       person
        angles[2] = Vector3.Angle(transform.right, v);           //       cubes
        angles[3] = Vector3.Angle(-transform.right, v);          //       rocket    
        angles[4] = Vector3.Angle(-transform.forward, v);        //       globe
        angles[5] = Vector3.Angle(transform.forward, v);         //       logo

        for (int i = 0; i <= 5; i++)
        {

            if (angles[i] < lowestAngle)
            {
                lowestAngle = angles[i];
                side = i;
            }
        }

        return side;
    }

    private void DirectionFinder()
    {
        //Debug.Log("Previously facing up: " + sideName[sideUpPrev] + ", previously facing camera: " + sideName[sideCamPrev] + ", previously facing right: " + sideName[sideRightPrev]);
        //Debug.Log("Facing up: " + sideName[sideUp] + ", facing camera: " + sideName[sideCam] + ", facing right: " + sideName[sideRight]);


        if (sideRightPrev == sideRight)
        {
            if (sideUpPrev == sideCam)
            {
                if (Down != null) { Down(); }
            }
            else if (sideCamPrev == sideUp)
            {
                if (Up != null) { Up(); }
            }
            else { if (DirectionFinderFail != null) { DirectionFinderFail(); } }

        }
        else if (sideUpPrev == sideUp)
        {
            if (sideCamPrev == sideRight)
            {
                if (Right != null) { Right(); }
            }
            else if (sideRightPrev == sideCam)
            {
                if (Left != null) { Left(); }
            }
            else { if (DirectionFinderFail != null) { DirectionFinderFail(); } }

        }
        else if (sideCamPrev == sideCam)
        {
            if (sideUpPrev == sideRight)
            {
                if (Flip != null) { Flip(); }
            }
            else if (sideRightPrev == sideUp)
            {
                if (Flop != null) { Flop(); }
            }
            else { if (DirectionFinderFail != null) { DirectionFinderFail(); } }
        }

    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {


        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            cubeSeen = true;
            if (previousStatus == TrackableBehaviour.Status.NO_POSE)
            {
                OnFound();
            }
        }
        else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                 newStatus == TrackableBehaviour.Status.NO_POSE)
        {
            cubeSeen = false;
            // If the cube is not seen, then notify that it has been lost
            if (OnLost != null) { OnLost(); }
        }
        else
        {
            cubeSeen = false;
            // If the cube is not seen, then notify that it has been lost
            if (OnLost != null) { OnLost(); }
        }
    }

    
}