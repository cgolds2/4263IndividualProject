using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour {
    GUIStyle myBox = new GUIStyle();

    public string seedtoEdit;
    // Use this for initialization
    void Start()
    {
    }

    void OnGUI()
    {
        GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        GUI.backgroundColor = Color.black;
        // Make a background box
        //GUI.Box(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 150, 400, 300+60),"");
        // Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
        if (GUI.Button(new Rect(Screen.width / 2 - 90, Screen.height / 2 - 80, 180, 60), "Play VS 2nd Player"))
        {
            GameMindScript.typeOfGame = GameMindScript.GameType.TwoPlayer;
            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");

        }

        // Make the second button.
        if (GUI.Button(new Rect(Screen.width / 2 - 90, Screen.height / 2 - 0, 180, 60), "Play VS Heuristic"))
        {
            GameMindScript.typeOfGame = GameMindScript.GameType.UseHeu;
            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");

        }

        // Make the second button.
        if (GUI.Button(new Rect(Screen.width / 2 - 90, Screen.height / 2 + 80, 180, 60), "Play VS AI"))
        {
            GameMindScript.typeOfGame = GameMindScript.GameType.UseAI;
            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");

        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
