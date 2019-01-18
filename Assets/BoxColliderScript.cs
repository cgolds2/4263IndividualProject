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
        //If your mouse hovers over the GameObject with the script attached, output this message
        Debug.Log("Mouse entered " + this.name);
        var cursorObj = GameObject.Find("CursorObject");
        cursorObj.transform.position = this.transform.position;
        cursorObj.transform.AddPos(y: 1);

    }
    void OnMouseOver()
    {
        //If your mouse hovers over the GameObject with the script attached, output this message
        //Debug.Log("Mouse is over " + this.name);
    }

    void OnMouseExit()
    {
        //The mouse is no longer hovering over the GameObject so output this message each frame
        Debug.Log("Mouse is no longer on " + this.name);
    }

    void OnMouseDown()
    {
        // load a new scene
        Debug.Log("Mouse clcked " + this.name);

        GameMindScript.PlaceMarble(this.transform.position);

        int index = int.Parse(this.name.Substring(12, 2));
        var loc = GameMindScript.ArrayLocationFromIndex(index);
        GameMove g = new GameMove(loc.Item1,loc.Item2,0,false);
        GameMindScript.MoveOther();

        var o = GameObject.Find("BR_TL");
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