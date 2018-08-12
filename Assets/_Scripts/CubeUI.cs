using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeUI : MonoBehaviour
{
    /*
     *  7 Animations:
     *  StartMenu -> colour, rotate, prev, next
     *  NextModel -> back on the left side cubes
     *  PrevModel -> back on the right side rocket
     *  NextColour -> back on the left side cubes turned
     *  PrevColour -> back on the right side rocket turned
     *  RotateMenu -> arrows on both sides and back on top
     *  ColourMenu -> prev, next, back, brush
     */

    private Animator anim;
    private string action = null;
    private string actionPrevious = null;
    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();

}
    private void Update()
    {
        
        if (action != null && action != actionPrevious)
        {
            anim.SetBool(actionPrevious, false);
            anim.SetBool(action, true);
            //Debug.Log(actionPrevious + " -> " + action);
            actionPrevious = action;
        }
    }

    public void animationOnCube(string actionNew)
    {
        if(action != actionNew)
        {
            action = actionNew;
        }
        
    }
}