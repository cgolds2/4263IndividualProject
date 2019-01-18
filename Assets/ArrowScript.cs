using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour {

    Material mat;
    static bool red;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnMouseEnter()
    {
        if(GameMindScript.GetGameState() == GameMindScript.GameState.PickingRotation)
        {
            //If your mouse hovers over the GameObject with the script attached, output this message
            Debug.Log("Mouse entered " + this.name);
            var cursorObj = GameObject.Find("CursorObject");
            cursorObj.GetComponent<Renderer>().enabled = true;

            cursorObj.transform.position = this.transform.position;
            cursorObj.transform.AddPos(y: 1);
        }
      


    }
    void OnMouseOver()
    {
        //If your mouse hovers over the GameObject with the script attached, output this message
        //Debug.Log("Mouse is over " + this.name);
    }

    void OnMouseExit()
    {
        if (GameMindScript.GetGameState() == GameMindScript.GameState.PickingRotation)
        {
 //The mouse is no longer hovering over the GameObject so output this message each frame
            Debug.Log("Mouse is no longer on " + this.name);
        var cursorObj = GameObject.Find("CursorObject");
        cursorObj.GetComponent<Renderer>().enabled = false;
        }
           

    }

    void OnMouseDown()
    {
        if (GameMindScript.GetGameState() == GameMindScript.GameState.PickingRotation)
        {
            GameMindScript.SetGameState(GameMindScript.GameState.NotTurn);

            // load a new scene
            Debug.Log("Mouse clcked " + this.name);


        int index = int.Parse(this.name.Substring(15, 1));

        var loc = GameMindScript.ArrayLocationFromIndex(index);


            int rotIndex = index / 2;
            int left = index % 2;


            GameMindScript.currentMove.rotIndex = rotIndex;
            GameMindScript.currentMove.rotLeft = (left==0);
            GameMindScript.MovePlayer();
            GameMindScript.SetGameState(GameMindScript.GameState.Moving);
            GameMindScript.SetNextGameState(GameMindScript.GameState.SecondPlayerTurn);
        }
           
    }
}
