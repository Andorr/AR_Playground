using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class CubeActions : MonoBehaviour
{
    // This class is an example of how you can utilise the CubeManager class

    public TrackableBehaviour trackablePad;
    private CubeManager manager;
    private CubeUI cubeUI;

    // Put all the products to swap through in ModelsToSwap GameObject
    private GameObject modelsToSwap;
    public static readonly string MULTI_MODEL_TAG = "ModelsToSwap";
    private const int extraColours = 2; // models have 2 extra colours

    #region Swapping variables
    // The direction of the swapping, if true next, false previous
    public bool forwardDir = true;
    public bool mSwapModel = false;
    private bool mSwapColour = false;
    // in case you other objects on ImageTarget you want always to be on
    private GameObject activeModel;
    #endregion

    #region Rotation variables
    public bool rotateMode = false;
    //Saves the start rotation of the model to rotate
    private float[] modelXYZ;
    // Startpoint for the rotation of the cube
    private float[] cubeXYZ;
    #endregion

    #region Constant sides of the cube
    public const int MagicCube = 0;
    public const int Person = 1;
    public const int Cubes = 2;
    public const int Rocket = 3;
    public const int Globe = 4;
    public const int Logo = 5;
    #endregion


    void Start()
    {
        modelsToSwap = GameObject.FindGameObjectWithTag(MULTI_MODEL_TAG);
        activeModel = modelsToSwap.transform.GetChild(0).gameObject;

        if (trackablePad == null)
        {
            Debug.Log("Warning: Trackable pad is not set!");
        }
        if (modelsToSwap == null)
        {
            Debug.Log("Warning: No objects with " + MULTI_MODEL_TAG + " is found!");
        }
        else if (modelsToSwap.transform.childCount == 0)
        {
            Debug.Log("Warning: You have no models to swap through!");
        }

        manager = gameObject.GetComponent<CubeManager>();
        cubeUI = GetComponentInChildren<CubeUI>();


        CubeManager.DirectionFinderFail += actionFail;

        onMenu();

    }

    void Update()
    {
        if (trackablePad != null && modelsToSwap.transform.childCount != 0)
        {
          
                if (rotateMode)
                {
                    rotateModel();
                    //if the main side is visible, then exit the rotate mode
                    if (manager.sideCam == MagicCube)
                    {
                        rotateMode = false;
                        //reset the variables
                        cubeXYZ = null;
                        modelXYZ = null;
                        cubeUI.animationOnCube("StartMenu");
                        onMenu();
                    }
                }
                else if (mSwapModel || mSwapColour)
                {
                    Swap();
                    mSwapModel = false;
                    mSwapColour = false;
                } else if(manager.sideCam == MagicCube)
                {
                    cubeUI.animationOnCube("StartMenu");
                } else if (manager.sideCam == Logo)
                {
                    cubeUI.animationOnCube("ColourMenu");
                } else if (manager.sideCam == Globe)
                {
                    cubeUI.animationOnCube("RotateMenu");
                    rotateModeOn();
                }

           

        }
        else
        {
            Debug.Log("Something is off! Pad: " + trackablePad + " Model to show: " + modelsToSwap.transform.childCount);
        }
    }

    private void actionFail()
    {
        print("Could not understand how you turned the cube. Please, try again");
    }

    private void onMenu()
    {
        //When the menu is up register Left and Right turns
        CubeManager.Left += onLeft;
        CubeManager.Right += onRight;

    }

    private void onRight()
    {
        forwardDir = false;

        if (manager.sideUp == Logo && manager.sideCam == Rocket)
        {
            print("Previous model");
            cubeUI.animationOnCube("PreviousModel");
            mSwapModel = true;
        }
        else if (manager.sideUp == Person && manager.sideCam == Rocket)
        {
            print("Previous colour");
            cubeUI.animationOnCube("PreviousColour");
            mSwapColour = true;
        }
    }

    private void onLeft()
    {
        forwardDir = true;

        if (manager.sideUp == Logo && manager.sideCam == Cubes)
        {
            print("Next model");
            cubeUI.animationOnCube("NextModel");
            mSwapModel = true;
        }
        else if (manager.sideUp == Person && manager.sideCam == Cubes)
        {
            print("Next colour");
            cubeUI.animationOnCube("NextColour");
            mSwapColour = true;
        }
    }


    #region SWAPPING

    private void Swap()
    {
        // you can iterate through diferent things, depending on the case
        if (mSwapModel)
        {
            // Deactivate the old object
            activeModel.SetActive(false);

            // find the new object
            int newIndex = nextSwap(0, modelsToSwap.transform.childCount - 1, activeModel.transform.GetSiblingIndex());
            activeModel = modelsToSwap.transform.GetChild(newIndex).gameObject;

            // Activate the new object
            activeModel.SetActive(true);
            

        }
        else if (mSwapColour)
        {
            Animator anim = activeModel.GetComponent<Animator>();
            
            int nextAnim = nextSwap(0, extraColours, anim.GetInteger("colour"));
            anim.SetInteger("colour", nextAnim);
        }
    }

    private int nextSwap(int start, int end, int now)
    {
        int temp = now;

        if (forwardDir)
        {
            temp = (now + 1 <= end) ? now + 1 : start;
        }
        else
        {
            temp = (now - 1 >= start) ? now - 1 : end;
        }
        return temp;
    }



    #endregion

    #region ROTATION
    private void rotateModeOn()
    {
        //When the rotate mode is activated, it saves the start data
        if (cubeXYZ == null || modelXYZ == null)
        {
            cubeXYZ = new float[] { transform.rotation.x, transform.rotation.y, transform.rotation.z };
            modelXYZ = new float[] { activeModel.transform.rotation.x, activeModel.transform.rotation.y, activeModel.transform.rotation.z };
        }

        //turns on rotateMode, så that the update function starts calling rotateModel()
        rotateMode = true;

        //Unsubscribe to any other function (it should only react to the main picture)
        CubeManager.Left -= onLeft;
        CubeManager.Right -= onRight;

    }

    private void rotateModel()
    {
        Quaternion angle = Quaternion.AngleAxis(transform.eulerAngles.y - modelXYZ[1] - cubeXYZ[1], Vector3.up);
        activeModel.transform.localRotation = angle;
    }



    #endregion

}
