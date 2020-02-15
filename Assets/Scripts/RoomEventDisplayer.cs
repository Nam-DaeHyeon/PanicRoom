using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomEventDisplayer : MonoBehaviour, IRoomDisplayer
{
    public Image eventImage;
    public Text  eventText;



    // =========================================================== public function ==========================================================

    public void Init()
    {
        //throw new System.NotImplementedException();
    }

    public void OnInvisible()
    {
        gameObject.SetActive(false);
    }

    public void OnVisible()
    {
        ResetDisplayer();

        gameObject.SetActive(true);
    }

    public void ResetDisplayer()
    {

    }

    public void SetImageAndText(Sprite sprite, string script)
    {
        eventImage.sprite = sprite;
        eventText.text = script;
    }


    // =========================================================== private function ==========================================================






}
