using UnityEngine;

public class TextButtonScript : MonoBehaviour
{

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
        GetComponent<Renderer>().enabled = true;



    }

    void OnMouseExit()
    {
        GetComponent<Renderer>().enabled = false;



    }

    void OnMouseDown()
    {
        

        switch (name)
        {
            case "2Player":
                GameMindScript.SetGameType(GameMindScript.GameType.TwoPlayer);
                UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
                break;
            case "Heu":
                GameMindScript.SetGameType(GameMindScript.GameType.UseHeu);
                UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");

                break;
            case "NN":
                GameMindScript.SetGameType(GameMindScript.GameType.UseAI);
                UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
                break;
            case "Options":
                break;
            case "Credits":
                break;
            default:
                break;
        }

    }


}
