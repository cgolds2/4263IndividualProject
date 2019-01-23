using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxColliderScript : MonoBehaviour {
    Material mat;
    static bool red;

    // Use this for initialization
    void Start () {
       
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnMouseEnter()
    {
        if(GameMindScript.GetGameState() == GameMindScript.GameState.PickingCoord)
        {
            //If your mouse hovers over the GameObject with the script attached, output this message
            //Debug.Log("Mouse entered " + this.name);
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
        if (GameMindScript.GetGameState() == GameMindScript.GameState.PickingCoord)
        {
            //The mouse is no longer hovering over the GameObject so output this message each frame
            //Debug.Log("Mouse is no longer on " + this.name);
            var cursorObj = GameObject.Find("CursorObject");


            cursorObj.GetComponent<Renderer>().enabled = false;
        }
     

    }

  

    void OnMouseDown()
    {
        if (GameMindScript.GetGameState() == GameMindScript.GameState.PickingCoord)
        {
            int index = int.Parse(this.name.Substring(12, 2));
            var loc = GameMindScript.ArrayLocationFromIndex(index);


   

            if (GameMindScript.currentGameData.gameBoard[loc.Item1,loc.Item2] != GameMindScript.TileVals.Blank){
                //tile occupied

            }else{
                GameMindScript.currentMove = new GameMove
                {
                    xCord = loc.Item1,
                    yCord = loc.Item2
                };
                //place the marble
                var cursorObj = GameObject.Find("CursorObject");

                cursorObj.GetComponent<Renderer>().enabled = false;

                // load a new scene
                //Debug.Log("Mouse clcked " + this.name);

                var marble = GameMindScript.PlaceMarble(this.transform.position);


                GameMindScript.lastMarble = marble;


                GameMindScript.SetGameState(GameMindScript.GameState.PickingRotation);

            }



        }

    }

}


public static class Helper
{
    public static void SetPos(this Transform transform, float? x = null, float? y = null, float? z = null)
    {
        Vector3 position = transform.position;

        if (x.HasValue) position.x = x.Value;
        if (y.HasValue) position.y = y.Value;
        if (z.HasValue) position.z = z.Value;
        transform.position = position;
    }

    public static void AddPos(this Transform transform, float? x = null, float? y = null, float? z = null)
    {
        Vector3 position = transform.position;

        if (x.HasValue) position.x += x.Value;
        if (y.HasValue) position.y += y.Value;
        if (z.HasValue) position.z += z.Value;
        transform.position = position;
    }
}