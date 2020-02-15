using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public partial class GameManager : MonoBehaviour
{
    public void UI_Title_NewGame()
    {
        PlayerPrefs.SetString("PlayTutorial", "F");
        if (PlayerPrefs.HasKey("BackHomeIndex")) PlayerPrefs.DeleteKey("BackHomeIndex");

        StartCoroutine(StartNewGame());
    }

    public void UI_Title_LoadGame()
    {

    }

    public void UI_Title_Option()
    {

    }

    public void UI_Title_QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
    }


    private IEnumerator StartNewGame()
    {
        Image blackPanel = GameObject.Find("BlackPanel").GetComponent<Image>();

        blackPanel.raycastTarget = true;

        float conTime = 0;
        float fadeTime = 1.5f;
        while (conTime <= fadeTime)
        {
            blackPanel.color = new Color(0, 0, 0, conTime / fadeTime);

            conTime += Time.deltaTime;
            yield return null;
        }
        blackPanel.color = new Color(0, 0, 0, 1);
        //SceneManager.LoadScene("LobbyStage");
        
        Add_goPlayer("H");
        Add_goPlayer("J");
        Add_goPlayer("철물점 아저씨");
        Go_NextDungeon();
    }
}
