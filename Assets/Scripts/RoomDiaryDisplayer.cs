using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomDiaryDisplayer : MonoBehaviour, IRoomDisplayer
{
    private int index = 0;
    private List<string> days;
    private List<string> contents;
    private int contentCount;

    [Header("Note")]
    public RoomEventDisplayer eventDisplayer;
    public Sprite eventSprite;
    public string eventString;

    [Header("Content")]
    public Text writeDay;
    public Text writeContent;

    // ========================================================= public function =================================================================

    public void Init()
    {
        days = new List<string>();
        contents = new List<string>();

        contentCount = contents.Count;

        // 이전에 저장된 content들을 가져와서 저장

    }

    public void OnInvisible()
    {
        gameObject.SetActive(false);
    }

    public void OnVisible()
    {
        SoundManager.instance.Play_SE_OpenBook();
        ResetDisplayer();

        gameObject.SetActive(true);
    }

    public void ResetDisplayer()
    {
        // 가장 최근에 얻은 content를 화면에 노출
        index = contentCount - 1;

        
    }

    public void OnClickNote()
    {
        eventDisplayer.SetImageAndText(eventSprite, eventString);
        eventDisplayer.OnVisible();
    }

    public void OnClickEndGame()
    {
        Application.Quit();
    }

    public void AddContent(string day, string content)
    {
        days.Add(day);
        contents.Add(content);
        contentCount++;
    }

    public void OnClickBefore()
    {
        index--;
        if (index < 0)
            index = contentCount - 1;

        writeDay.text = days[index];
        writeContent.text = days[index];
    }
    public void OnClickNext()
    {
        index++;
        if (index >= contentCount)
            index = 0;

        writeDay.text = days[index];
        writeContent.text = days[index];
    }


    // ========================================================= private function =================================================================




}
