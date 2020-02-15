using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    [Header("Map Light")]
    public Transform maplLight;
    public float maplLightBlink;
    public float minSize;
    public float maxSize;

    [Header("Scene Manager")]
    public float fadeTime;

    [Header("Lobby")]
    public GameObject lobbyUI;
    public Text DDayText;
    public Text foodText;

    [Header("Stage Choice")]
    public GameObject stageChoiceUI;

    [Header("Prologue")]
    public Image blackPanel;
    public PrologueScript prologue;

    [Header("Tutorial")]
    public TutorialScript tutorial;

    [Header("CutScene")]
    public CutSceneScript cutScene;

    [Header("EventMessage")]
    public EventMessageScript eventMessage;

    [Header("ETC")]
    [SerializeField] GameObject _memberDisplayerObj;
    [SerializeField] GameObject _inventoryDisplayerObj;
    [SerializeField] GameObject _craftingDisplayerObj;
    [SerializeField] GameObject _DiaryDisplayerObj;

    // UI들 Fade In Out
    public CanvasGroup canvasGroup;
    // UI가 위아래로 이동하는 연출
    public RectTransform downToUpIcons;
    // UI가 좌우로 이동하는 연출
    public RectTransform leftToRightIcons;


    // 현재 여정 멤버로 등록된 인물들의 수를 표시
    public Text memberCountText;

    // 현재 여정을 떠나는 멤버들의 정보
    [System.Serializable]
    public class MemberInfo
    {
        public Text chracter_name;
        public GameObject chracter_NoMan;
        public GameObject chracter_Image;
    }
    public MemberInfo[] memberInfos;

    // 여정을 떠나는 버튼
    public Text goText;
    public Image goBG;


    // Start is called before the first frame update
    void Start()
    {
        SoundManager.instance.Play_BGM_Lobby();

        // Map Light 깜빡이는 코루틴 실행
        StartCoroutine(Blinking());

        // 게임 해상도에 맞도록 조절
        SetResolution();

        // 이전에 튜토리얼을 진행한 적이 있는가?
        if (PlayerPrefs.GetString("PlayTutorial","F") == "F")
        {
            // 튜토리얼을 진행
            TutorialStart();
        }
        else
        {
            // 죽은 사람이 없음
            if(GameManager.instance.diePlayerCount == 0)
            {
                // 평화로운 컷씬 시작
                NormalStart(3);
            }
            else
            {
                // 모두 죽음
                if(GameManager.instance.diePlayerCount == GameManager.instance.goPlayerCount)
                {
                    NormalStart(5);
                }
                // 일부만 죽음
                else
                {
                    NormalStart(4);
                }
            }
        }

    }

    // ============================================================== public functuin ====================================================================


    public void TutorialStart()
    {
        StartCoroutine(StartTutorial());

    }
    public void NormalStart(int index)
    {
        StartCoroutine(StartNormal(index));
    }

    public void Set_DayCount()
    {
        DDayText.text = "D + " + GameManager.instance.DayCount.ToString();
    }

    // LobbyBG로 되돌아간다.(로비 씬으로 이동)
    public void GoToLobby()
    {
        StartCoroutine(GoingToLobby());
    }

    // StageChoiceBG로 이동한다.(멤버 선택 씬으로 이동)
    public void GoToStageChoice()
    {
        // 현재 여정을 떠나는 멤버의 수가 0
        if (GameManager.instance.goPlayers.Count == 0)
        {
            // 좌측 캐릭터 표시 칸을 모두 빈칸 처리
            memberCountText.text = "현재 파티 구성원 ( 0 / 3 )";
            for(int i = 0; i < memberInfos.Length; i++)
            {
                memberInfos[i].chracter_name.text = "";
                memberInfos[i].chracter_NoMan.SetActive(true);
                memberInfos[i].chracter_Image.SetActive(false);
            }

            // 멤버를 만들도록 유도.
            goText.text = "생존자 명단에서 파티를 구성하세요!";
            goBG.color = new Color32(135, 85, 85, 255);
        }
        // 현재 여정을 떠나는 멤버의 수가 > 0
        else
        {
            int conCount = 0;

            // 플레이어가 선택한 캐릭터의 정보를 좌측에 노출
            memberCountText.text = "현재 파티 구성원 ( " + GameManager.instance.goPlayers.Count.ToString() + " / 3 )";
            for (int i = 0; i < memberInfos.Length; i++)
            {
                // 좌측 칸부더 하나씩 기록하되, 만약 캐릭터가 최대 칸수보다 못미치면 나머지 칸은 빈칸처리
                if (conCount < GameManager.instance.goPlayers.Count)
                {
                    memberInfos[i].chracter_name.text = GameManager.instance.goPlayers[conCount].characterName;
                    memberInfos[i].chracter_NoMan.SetActive(false);
                    memberInfos[i].chracter_Image.SetActive(true);

                    conCount++;
                }
                else
                {
                    memberInfos[i].chracter_name.text = "";
                    memberInfos[i].chracter_NoMan.SetActive(true);
                    memberInfos[i].chracter_Image.SetActive(false);
                }

            }

            // 여정을 떠날 수 있도록 허용.
            goText.text = "이 인원으로  출발 하시겠습니까?";
            goBG.color = new Color32(86, 136, 96, 255);
        }

        StartCoroutine(GoingToStageChoice());
    }

    // 여정을 떠난다.
    public void LeaveShelter()
    {
        // 현재 여정을 떠나는 멤버의 수가 0
        if (GameManager.instance.goPlayers.Count != 0)
        {
            StartCoroutine(LeavingShelter());
        }

    }



    // ============================================================== private functuin ====================================================================

    private IEnumerator Blinking()
    {
        while(true)
        {
            yield return new WaitForSeconds(maplLightBlink);
    
            maplLight.localScale = new Vector2(maxSize, maxSize);
            yield return new WaitForSeconds(0.2f);
            maplLight.localScale = new Vector2(minSize, minSize);

            yield return new WaitForSeconds(0.8f);

            maplLight.localScale = new Vector2(maxSize, maxSize);
            yield return new WaitForSeconds(0.2f);
            maplLight.localScale = new Vector2(minSize, minSize);
        }
    }

    private void SetResolution()
    {
        if ((1920 * Screen.height / Screen.width) > 1080)
        {
            CanvasScaler[] canvases = GameObject.FindObjectsOfType<CanvasScaler>();
            for (int i = 0; i < canvases.Length; i++)
                canvases[i].referenceResolution = new Vector2(1920, 1920 * Screen.height / Screen.width);
        }
    }


    // StageChoiceBG로 이동하는 코루틴(멤버 선택 씬으로 이동)
    private IEnumerator GoingToStageChoice()
    {
        // 연출을 위한 검은 패널을 킨다.
        blackPanel.gameObject.SetActive(true);

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

        // UI 처리
        lobbyUI.SetActive(false);
        stageChoiceUI.SetActive(true);

        _memberDisplayerObj.SetActive(false);

        // Fade Out
        conTime = 0;
        while (conTime < fadeTime)
        {
            blackPanel.color = new Color(0, 0, 0, 1 - conTime / fadeTime);
            mainCamera.orthographicSize = 6 - conTime / fadeTime;
            downToUpIcons.anchoredPosition = new Vector2(0, -225 + 225 * conTime / fadeTime);
            leftToRightIcons.anchoredPosition = new Vector2(-150 + 150 * conTime / fadeTime, 0);
            canvasGroup.alpha = conTime / fadeTime;
            conTime += Time.deltaTime;
            yield return null;
        }
        blackPanel.color = new Color(0, 0, 0, 0);
        mainCamera.orthographicSize = 5;
        downToUpIcons.anchoredPosition = new Vector2(0, 0);
        leftToRightIcons.anchoredPosition = new Vector2(0, 0);
        canvasGroup.alpha = 1;

        // 연출을 위한 검은 패널을 끈다.
        blackPanel.gameObject.SetActive(false);
    }




    // LobbyBG로 되돌아가는 코루틴(로비 씬으로 이동)
    private IEnumerator GoingToLobby()
    {
        // 연출을 위한 검은 패널을 킨다.
        blackPanel.gameObject.SetActive(true);

        // 연출을 하는 카메라
        Camera mainCamera = Camera.main;

        // Fade In
        float conTime = 0;
        while (conTime < fadeTime)
        {
            blackPanel.color = new Color(0, 0, 0, conTime / fadeTime);
            mainCamera.orthographicSize = 5 + conTime / fadeTime;
            downToUpIcons.anchoredPosition = new Vector2(0, -225 * conTime / fadeTime);
            leftToRightIcons.anchoredPosition = new Vector2(-150 * conTime / fadeTime, 0);
            canvasGroup.alpha = 1 - conTime / fadeTime;
            conTime += Time.deltaTime;
            yield return null;
        }
        blackPanel.color = new Color(0, 0, 0, 1);
        downToUpIcons.anchoredPosition = new Vector2(0, -225);
        leftToRightIcons.anchoredPosition = new Vector2(-150, 0);
        canvasGroup.alpha = 0;

        // 카메라 이동
        mainCamera.transform.position = new Vector3(0, 0, -10);
        mainCamera.orthographicSize = 4;

        // UI 처리
        lobbyUI.SetActive(true);
        stageChoiceUI.SetActive(false);

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

        // 연출을 위한 검은 패널을 끈다.
        blackPanel.gameObject.SetActive(false);
    }







    // 여정을 떠나는 코루틴
    private IEnumerator LeavingShelter()
    {
        // 연출을 위한 검은 패널을 킨다.
        blackPanel.gameObject.SetActive(true);

        // Fade In
        float conTime = 0;
        while (conTime < fadeTime)
        {
            blackPanel.color = new Color(0, 0, 0, conTime / fadeTime);

            conTime += Time.deltaTime;
            yield return null;
        }
        blackPanel.color = new Color(0, 0, 0, 1);

        cutScene.gameObject.SetActive(true);

        cutScene.CutSceneStart(2);

        while (cutScene.gameObject.activeSelf == true)
            yield return null;

        // 씬 이동
        GameManager.instance.Go_NextDungeon();
    }

    // 하루 넘기기 연출
    private IEnumerator IE_PassTheDay()
    {
        // 연출을 위한 검은 패널을 킨다.
        blackPanel.gameObject.SetActive(true);

        // Fade In
        float conTime = 0;
        while (conTime < fadeTime)
        {
            blackPanel.color = new Color(0, 0, 0, conTime / fadeTime);

            conTime += Time.deltaTime;
            yield return null;
        }
        blackPanel.color = new Color(0, 0, 0, 1);

        cutScene.gameObject.SetActive(true);

        cutScene.CutSceneStart(1);

        while (cutScene.gameObject.activeSelf == true)
            yield return null;

        //모든 디스플레이어 off
        if (_memberDisplayerObj.activeSelf) _memberDisplayerObj.SetActive(false);
        if (_inventoryDisplayerObj.activeSelf) _inventoryDisplayerObj.SetActive(false);
        if (_craftingDisplayerObj.activeSelf) _craftingDisplayerObj.SetActive(false);
        if (_DiaryDisplayerObj.activeSelf) _DiaryDisplayerObj.SetActive(false);


        //메인 프로세스 실행
        GameManager.instance.Pass_theDay();

        foodText.text = GameManager.foodCount.ToString();

        //페이드 인
        StartCoroutine(Entering());

        // 인게임 진입이 완료될때까지 대기
        while (blackPanel.gameObject.activeSelf == true)
            yield return null;

        yield return new WaitForSeconds(0.5f);

        // 복귀 인 상황에서는 이벤트메시지 출력
        eventMessage.gameObject.SetActive(true);
        eventMessage.EventMessageStart();

        // 이벤트메시지가 완료될때까지 대기
        while (eventMessage.gameObject.activeSelf == true)
            yield return null;
    }

    IEnumerator Entering()
    {
        Set_DayCount();

        // 연출을 위한 검은 패널을 킨다.
        blackPanel.gameObject.SetActive(true);

        float conTime = fadeTime;
        while (conTime >= 0)
        {
            blackPanel.color = new Color(0, 0, 0, conTime / fadeTime);

            conTime -= Time.deltaTime;
            yield return null;
        }
        blackPanel.color = new Color(0, 0, 0, 0);

        // 연출을 위한 검은 패널을 끈다.
        blackPanel.gameObject.SetActive(false);
    }

    public void UI_Button_PassDay()
    {
        if (GameManager.instance == null) return;

        StartCoroutine(IE_PassTheDay());
    }

    IEnumerator StartNormal(int index)
    {
        // 연출을 위한 검은 패널을 킨다.
        blackPanel.gameObject.SetActive(true);

        cutScene.gameObject.SetActive(true);

        cutScene.CutSceneStart(index);

        while (cutScene.gameObject.activeSelf == true)
            yield return null;

        //모든 디스플레이어 off
        if (_memberDisplayerObj.activeSelf) _memberDisplayerObj.SetActive(false);
        if (_inventoryDisplayerObj.activeSelf) _inventoryDisplayerObj.SetActive(false);
        if (_craftingDisplayerObj.activeSelf) _craftingDisplayerObj.SetActive(false);
        if (_DiaryDisplayerObj.activeSelf) _DiaryDisplayerObj.SetActive(false);

        //페이드 인
        StartCoroutine(Entering());

        // 인게임 진입이 완료될때까지 대기
        while (blackPanel.gameObject.activeSelf == true)
            yield return null;

        yield return new WaitForSeconds(0.5f);

        // 복귀 인 상황에서는 이벤트메시지 출력
        if(index == 3)
        {
            eventMessage.gameObject.SetActive(true);
            eventMessage.EventMessageStart();

            // 이벤트메시지가 완료될때까지 대기
            while (eventMessage.gameObject.activeSelf == true)
                yield return null;
        }
    }

    IEnumerator StartTutorial()
    {
        // 연출을 위한 검은 패널을 킨다.
        blackPanel.gameObject.SetActive(true);
        blackPanel.color = new Color(0, 0, 0, 1);
        
        cutScene.gameObject.SetActive(true);

        cutScene.CutSceneStart(0);

        while (cutScene.gameObject.activeSelf == true)
            yield return null;

        // 인게임 진입
        StartCoroutine(Entering());

        // 인게임 진입이 완료될때까지 대기
        while (blackPanel.gameObject.activeSelf == true)
            yield return null;

        // 튜토리얼 시작
        tutorial.gameObject.SetActive(true);
        tutorial.TutorialStart();

        // 튜토리얼이 완료될때까지 대기
        while (tutorial.gameObject.activeSelf == true)
            yield return null;

        PlayerPrefs.SetString("PlayTutorial", "T");
    }
}
