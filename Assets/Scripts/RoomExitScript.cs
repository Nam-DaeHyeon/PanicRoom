using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoomExitScript : MonoBehaviour
{
    public float fadeTime;

    public NoManDisplayer displayer;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Entering());
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // ============================================================== public functuin ====================================================================

    public void OnClickExit()
    {
        if (GameManager.instance.goPlayers.Count == 0)
        {
            displayer.OnVisible();
        }
        else
        {
            LeaveShelter();
        }
    }

    // InShelterBG로 되돌아간다.(로비 씬으로 이동)
    public void GoToInShelter()
    {
        StartCoroutine(GoingToInShelter());
    }

    // OutShelterBG로 되돌아간다.(멤버 선택 씬으로 이동)
    public void GoToOutShelter()
    {
        // 현재 여정을 떠나는 멤버의 수가 0
        if (GameManager.instance.goPlayers.Count == 0)
        {
            // 멤버를 만들도록 유도.
        }
        else
        {
            // 여정을 떠날 수 있도록 허용.
        }

        StartCoroutine(GoingToOutShelter());
    }

    // 여정을 떠난다.
    public void LeaveShelter()
    {
        StartCoroutine(LeavingShelter());
    }





    // ============================================================== private functuin ====================================================================



    // InShelterBG로 되돌아가는 코루틴
    // 여정을 떠나는 코루틴
    private IEnumerator GoingToInShelter()
    {
        // 연출을 위한 검은 패널
        Image blackPanel = GameObject.Find("BlackPanel").GetComponent<Image>();
        blackPanel.raycastTarget = true;

        // 연출을 하는 카메라
        Camera mainCamera = Camera.main;

        // Fade In
        float conTime = 0;
        while (conTime < fadeTime)
        {
            blackPanel.color = new Color(0, 0, 0, conTime / fadeTime);
            mainCamera.orthographicSize = 5 + conTime / fadeTime;
            conTime += Time.deltaTime;
            yield return null;
        }
        blackPanel.color = new Color(0, 0, 0, 1);

        // 카메라 이동
        mainCamera.transform.position = new Vector3(0, 0, -10);
        mainCamera.orthographicSize = 4;

        // Fade Out
        conTime = 0;
        while (conTime < fadeTime)
        {
            blackPanel.color = new Color(0, 0, 0, 1 - conTime / fadeTime);
            mainCamera.orthographicSize = 4 + conTime / fadeTime;
            conTime += Time.deltaTime;
            yield return null;
        }
        blackPanel.color = new Color(0, 0, 0, 0);
        mainCamera.orthographicSize = 5;



       
    }




    // OutShelterBG로 되돌아가는 코루틴
    private IEnumerator GoingToOutShelter()
    {
        // 연출을 위한 검은 패널
        Image blackPanel = GameObject.Find("BlackPanel").GetComponent<Image>();
        blackPanel.raycastTarget = true;

        // 연출을 하는 카메라
        Camera mainCamera = Camera.main;

        // Fade In
        float conTime = 0;
        while (conTime < fadeTime)
        {
            blackPanel.color = new Color(0, 0, 0, conTime / fadeTime);
            mainCamera.orthographicSize = 5 - conTime / fadeTime;
            conTime += Time.deltaTime;
            yield return null;
        }
        blackPanel.color = new Color(0, 0, 0, 1);

        // 카메라 이동
        mainCamera.transform.position = new Vector3(50, 0, -10);
        mainCamera.orthographicSize = 6;

        // Fade Out
        conTime = 0;
        while (conTime < fadeTime)
        {
            blackPanel.color = new Color(0, 0, 0, 1 - conTime / fadeTime);
            mainCamera.orthographicSize = 6 - conTime / fadeTime;
            conTime += Time.deltaTime;
            yield return null;
        }
        blackPanel.color = new Color(0, 0, 0, 0);
        mainCamera.orthographicSize = 5;
    }


    // 여정을 떠나는 코루틴
    private IEnumerator LeavingShelter()
    {
        Image blackPanel = GameObject.Find("BlackPanel").GetComponent<Image>();

        // Fade In 동안 조작 금지
        blackPanel.raycastTarget = true;

        // Fade In
        float conTime = 0;
        while(conTime < fadeTime)
        {
            blackPanel.color = new Color(0, 0, 0, conTime / fadeTime);

            conTime += Time.deltaTime;
            yield return null;
        }
        blackPanel.color = new Color(0, 0, 0, 1);

        // 씬 이동
        GameManager.instance.Go_NextDungeon();
    }

    IEnumerator Entering()
    {
        Image blackPanel = GameObject.Find("BlackPanel").GetComponent<Image>();

        blackPanel.raycastTarget = true;

        float conTime = fadeTime;
        while (conTime >= 0)
        {
            blackPanel.color = new Color(0, 0, 0, conTime / fadeTime);

            conTime -= Time.deltaTime;
            yield return null;
        }
        blackPanel.color = new Color(0, 0, 0, 0);
        
        blackPanel.raycastTarget = false;
    }
}
