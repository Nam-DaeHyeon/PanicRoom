using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

public enum GameState
{
    READY,
    IDLE,
    BATTLE,
    GAMEOVER,
    NEXTSTAGE,
}

public partial class StageManager : MonoBehaviour
{
    //SingleTon
    public static StageManager instance;
    private void Awake()
    {
        instance = this;
    }
    
    public List<Transform> PositionSet_Monster;
    public List<Transform> PositionSet_Player;

    public List<Monster> MonsterPrefabs;
    public List<Monster> BlackSheepPrefabs;
    public int MonsterCount;

    public List<Player> PlayerPrefabs;

    public List<Monster> InfectedPrefabs;
    public Player SupporterPrefabs;

    [Header("Stage Manager")]
    public GameState gameState;
    [SerializeField] Transform _nextLine;
    [SerializeField] Transform _fadeBoard;
    [SerializeField] float _fadeSpeed = 12f;
    [SerializeField] Image Map_PosBar;
    Monster currInfectedPlayer;
    [SerializeField] float _detectDistance = 1.5f;
    public bool isIgnored = false;

    [Header("Memory Pool & Turn System")]
    public List<Player> PlayerPool;
    public List<Monster> MonsterPool;
    List<Unit> turnPool = new List<Unit>();
    Unit turnUnit;
    public List<Vector3> prevPos;
    public bool TurnEnd;

    public Item readyItem;

    //순서정렬을 위한 임시 변수
    IOrderedEnumerable<Unit> tempQry;
    List<Unit> tempList = new List<Unit>();
    bool onTemp = false;
    int tempStartIdx = 0;

    [Header("Player UI")]
    public GameObject PlayerGameOverUI;
    public Image Image_ReadyItem;

    [Header("MouseOver - Player Info Displayer")]
    public GameObject MOVER_FrameUI;
    public GameObject MOVER_EquipUI;
    public Image MOVER_EquipIcon;
    public Text MOVER_EquipNameText;
    public Text MOVER_EquipComment;
    public Text MOVER_NameText;
    public Text MOVER_HPText;
    public Text MOVER_RPText;
    public Text MOVER_ATKText;
    public Text MOVER_DEFText;
    public Text MOVER_CRIText;

    [Header("MouseOver - Inventory Item Info Displayer")]
    public GameObject MOVER_FrameUI_Item;
    public Text MOVER_Name_Item;
    public Text MOVER_OptionAlly_Item;
    public Text MOVER_OptionEnemy_Item;
    public Text MOVER_Comment_Item;

    [Header("Inventory UI")]
    [SerializeField] GameObject page_PartyInventory;

    [Header("Reward UI")]
    public GameObject RW_UI;
    public List<MS_RewardItem> RW_Items;
    public Text RW_Msg;
    [SerializeField] int _firstRewardRate = 70;
    [SerializeField] int _secondRewardRate = 25;
    [SerializeField] int _thirdRewardRate = 5;
    
    [Header("PopUp UI")]
    [SerializeField] GameObject _popupObj;
    [SerializeField] Text _popupText;

    [Header("Monster UI")]
    public List<Image> MonsterHUDs;

    [Header("Event UI")]
    public GameObject Button_BackHome;

    [Header("Battle Mode - Hit Log")]
    public List<Text> HitLogs;
    [SerializeField] Color _hitColorWhite;
    [SerializeField] Color _hitColorYellow;
    [SerializeField] Color _hitColorGreen;
    [SerializeField] Color _hitColorRed;
    int currHitLogIdx;

    [Header("BackGround Resources")]
    [SerializeField] SpriteRenderer targetBG;
    [SerializeField] Sprite rscBG_Street;
    [SerializeField] Sprite rscBG_Hospital;

    int playerIndex;
    
    bool _clickedBox = false;

    private void Start()
    {
        //복귀 지점 인덱스 확인 : 현재 지점이 해당 지점보다 멀 경우 초기화시킨다.
        if (GameManager.instance.DungeonCount > Get_BackHomeIndex()) Delete_BackHomeIndex();

        //맵 위치 표시
        //Map_PosBar.fillAmount = ((float)GameManager.instance.DungeonCount) / ((float)GameManager.instance.maps.Count);
        Map_PosBar.fillAmount = GameManager.instance.Get_Current_MapPosition();

        //배경 리소스 확인
        Set_BGResource();

        //몬스터 & 아이템 박스 생성 여부 확인.
        isIgnored = GameManager.instance.Ignore_CreateMonsterAndItemBox();
        //if(isIgnored) Debug.Log("All ignored.");

        //플레이어 및 몬스터 메모리 풀링.
        Reset_UnitPool();

        for (int i = 0; i < PlayerPool.Count; i++)
        {
            //던전 회차에 따른 능력치 초기화 및 유지.
            PlayerPool[i].Set_InitParameter(i);
        }

        //조력자 등장 여부 확인.
        Set_NPC_Supporter();

        //기타 NPC 등장 여부 확인.
        Set_NPC_Others();

        //사망자 비활성화
        for (int i = 0; i < PlayerPool.Count; i++)
        {
            if(PlayerPool[i].Get_currentHP() <= 0)
            {
                PlayerPool[i].HideHUD();
                PlayerPool[i].gameObject.SetActive(false);
            }
        }

        //오브젝트 독립화 : 렌더 조정을 위해
        Set_PoolParentNull();

        //게임시작.
        StartCoroutine(IE_FadeIn());

        Calculate_MonsterDistance();

        //감염된 플레이어 간에 거리 측정 - 근접시 강제 전투.
        Calculate_InfectedPlayerDistance();

        //상호작용 키 입력 확인
        StartCoroutine(IE_Interact_UI());

        //게임종료확인
        StartCoroutine(IE_Check_NextStage());
    }

    IEnumerator IE_Check_NextStage()
    {
        while(true)
        {
            for(int i = 0; i < PlayerPool.Count; i++)
            {
                if(PlayerPool[i].transform.position.x > _nextLine.position.x)
                {
                    GameSet_NEXTSTAGE();
                    yield break;
                }
            }

            yield return null;
        }
    }

    private void Reset_UnitPool()
    {
        //플레이어 생성
        for (int i = 0; i < PlayerPrefabs.Count; i++)
        {
            if (i < GameManager.instance.goPlayers.Count)
            {
                SpriteRenderer characterImage = PlayerPrefabs[i].transform.GetChild(1).GetComponent<SpriteRenderer>();
                #region 캐릭터 이미지 생성.
                if (GameManager.instance.goPlayers[i].privateItem != null)
                {
                    switch (GameManager.instance.goPlayers[i].privateItem.Get_ItemName())
                    {
                        case "조잡한 몽둥이":
                            characterImage.sprite = Resources.Load<Sprite>("CharacterImage/Human_Bat");
                            break;
                        case "조잡한 슬링":
                        case "날붙이 슬링":
                            characterImage.sprite = Resources.Load<Sprite>("CharacterImage/Human_Sling");
                            break;
                        case "네일 배트":
                        case "철근 몽둥이":
                            characterImage.sprite = Resources.Load<Sprite>("CharacterImage/Human_IronBat");
                            break;
                        case "호신용 가스총":
                            characterImage.sprite = Resources.Load<Sprite>("CharacterImage/Human_Gun");
                            break;
                        case "조잡한 나이프 창":
                        case "철근 나이프 창":
                            characterImage.sprite = Resources.Load<Sprite>("CharacterImage/Human_Spear");
                            characterImage.transform.localPosition = new Vector2(0.4f, 0);  //위치 보정.
                            break;
                        default:
                            characterImage.sprite = Resources.Load<Sprite>("CharacterImage/Human_None");
                            break;
                    }
                }
                #endregion

                PlayerPrefabs[i].playerIndex = i;
                PlayerPool.Add(PlayerPrefabs[i]);
            }
            else
            {
                PlayerPrefabs[i].HideHUD();
                PlayerPrefabs[i].gameObject.SetActive(false);
            }
        }
        
        //몬스터 생성 무시
        if (isIgnored) return;

        //특수 이벤트 : 메인 빌런(검은양)
        if (MS_EventManager.instance.OnEnemy_BlackSheep)
        {
            for (int i = 0; i < 3; i++)
            {
                int randIndex = Random.Range(0, BlackSheepPrefabs.Count);
                if (randIndex == BlackSheepPrefabs.Count) randIndex--;

                GameObject monsterObj = Instantiate(BlackSheepPrefabs[randIndex].gameObject);
                monsterObj.transform.position = PositionSet_Monster[i].position;

                Monster monster = monsterObj.GetComponent<Monster>();
                monster.Set_OriginPos();
                monster.Set_DetailStatus();

                monster.UI_HPBar = MonsterHUDs[i];
                //monster.UI_HPBar.transform.position = monster.HUDPos.position;

                MonsterPool.Add(monster);
            }

            if (MonsterCount > 0) MonsterPool[1].gameObject.SetActive(true);

            return;
        }

        //몬스터 생성
        if (GameManager.instance.Get_CurrentStage_Rate("MONSTER") == 0) return;
        if (GameManager.instance.Get_CurrentStage_Rate("MONSTER") >= Random.Range(0, 100))
        {
            List<InfectedPlayer> tempInfectedPlayers = GameManager.instance.Get_InfectedPlayerData();

            // 감염된 사람이 있을 경우,
            if (tempInfectedPlayers.Count > 0)
            {
                int randIndex = Random.Range(0, InfectedPrefabs.Count);
                if (randIndex == InfectedPrefabs.Count) randIndex--;

                GameObject monsterObj = Instantiate(InfectedPrefabs[randIndex].gameObject);
                monsterObj.transform.position = PositionSet_Monster[0].position;

                currInfectedPlayer = monsterObj.GetComponent<Monster>();
                currInfectedPlayer.Set_InfectedData(tempInfectedPlayers[0]);
                currInfectedPlayer.Set_OriginPos();
                currInfectedPlayer.Set_DetailStatus();

                currInfectedPlayer.UI_HPBar = MonsterHUDs[0];
                //monster.UI_HPBar.transform.position = monster.HUDPos.position;

                MonsterPool.Add(currInfectedPlayer);
                GameManager.instance.Remove_InfectedPlayerData(tempInfectedPlayers[0].characterName);
            }
            // 감염된 사람이 없을 경우
            else
            {
                for (int i = 0; i < MonsterCount; i++)
                {
                    int randIndex = Random.Range(0, MonsterPrefabs.Count);
                    if (randIndex == MonsterPrefabs.Count) randIndex--;

                    GameObject monsterObj = Instantiate(MonsterPrefabs[randIndex].gameObject);
                    monsterObj.transform.position = PositionSet_Monster[i].position;

                    Monster monster = monsterObj.GetComponent<Monster>();
                    monster.Set_OriginPos();
                    monster.Set_DetailStatus();

                    monster.UI_HPBar = MonsterHUDs[i];
                    //monster.UI_HPBar.transform.position = monster.HUDPos.position;

                    MonsterPool.Add(monster);
                }
            }

            if (MonsterCount > 0) MonsterPool[0].gameObject.SetActive(true);
        }
    }

    IEnumerator IE_LocateProcess(Unit unit, Vector3 pos)
    {
        while(!unit.IsArrived)
        {
            unit.Check_Located_Unit(pos);
            yield return null;
        }
    }

    IEnumerator IE_StageIdleProcess()
    {
        //복귀했었는지 확인. 
        if (!Has_BackHomeIndex())
        {
            //시작 팝업 메세지 확인
            if (MS_EventManager.instance.Get_PopMessage_Index0(true) != null)
            {
                StartCoroutine(IE_PopUpMessage(true));
                yield return new WaitUntil(() => !_popupObj.activeSelf);
            }

            //전투 전 이벤트메세지
            if (!MS_EventManager.instance.isEnd_idleBefore)
            {
                MS_EventManager.instance.Pop_EventMessage_IdleBeforeBattle();
                yield return new WaitUntil(() => MS_EventManager.instance.isEnd_idleBefore);
                
                if (MS_EventManager.instance.Get_PopMessage_Index0(false) != null)
                {
                    StartCoroutine(IE_PopUpMessage(false));
                    yield return new WaitUntil(() => !_popupObj.activeSelf);
                }

                //강제전투처리 : 전투 후 이벤트메세지가 있는지 확인
                if (MS_EventManager.instance.Get_AfterBattleCount() > 0
                    || MS_EventManager.instance.Get_BattleCount() > 0)
                {
                    if (!MS_EventManager.instance.OnPreEmptive)
                    {
                        GameSet_BATTLE();
                        yield break;
                    }
                }
            }
            //전투 후 이벤트메세지
            else
            {
                //전투보상 닫은 이후 진행
                yield return new WaitUntil(() => !RW_UI.activeSelf);

                MS_EventManager.instance.Pop_EventMessage_IdleAfterBattle();
                yield return new WaitUntil(() => MS_EventManager.instance.isEnd_idleAfter);
            }

            if (MS_EventManager.instance.OnEssentialItem)
            {
                Coroutine coroutine = StartCoroutine(IE_Check_ClickItemBox());
                //yield return new WaitUntil(() => MS_EventManager.instance.isClosed_pop);
                yield return new WaitUntil(() => _clickedBox);
                StopCoroutine(coroutine);

                //아이템상자(오브젝트) 클릭 후 팝업_이벤트메세지
                MS_EventManager.instance.Pop_EventMessage_ClickObject();
                yield return new WaitUntil(() => MS_EventManager.instance.isEnd_clickObj);
            }

            //몬스터가 없을 경우(전투 이후)
            if (!Check_AliveMonster(true))
            {
                //메인스토리아이템 이벤트메세지
                MS_EventManager.instance.Pop_EventMessage_MainItem();
                yield return new WaitUntil(() => MS_EventManager.instance.isEnd_item);
            }
        }

        StartCoroutine(IE_StartIdle_ClickInteract());
    }

    IEnumerator IE_StartIdle_ClickInteract()
    {
        while (gameState.Equals(GameState.IDLE))
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                Vector2 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Ray2D ray2d = new Ray2D(wp, Vector2.zero);
                RaycastHit2D hit2d = Physics2D.Raycast(ray2d.origin, ray2d.direction);
                if (hit2d.transform != null)
                {
                    //몬스터일 경우
                    if (hit2d.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Monster")))
                    {
                        GameSet_BATTLE();
                    }
                    //파밍요소(아이템 박스)일 경우
                    if (hit2d.transform.tag.Equals("ItemBox"))
                    {
                        MS_ItemBox itemBox = hit2d.transform.GetComponent<MS_ItemBox>();
                        itemBox.Open_Box();
                        Item[] boxItems = itemBox.Get_Items();
                        Give_Reward_ItemBox(boxItems);
                    }
                }
            }

            yield return null;
        }
    }

    IEnumerator IE_Check_ClickItemBox()
    {
        while (!_clickedBox)
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                Vector2 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Ray2D ray2d = new Ray2D(wp, Vector2.zero);
                RaycastHit2D hit2d = Physics2D.Raycast(ray2d.origin, ray2d.direction);
                if (hit2d.transform != null)
                {
                    //파밍요소(아이템 박스)일 경우
                    if (hit2d.transform.tag.Equals("ItemBox"))
                    {
                        MS_ItemBox itemBox = hit2d.transform.GetComponent<MS_ItemBox>();
                        itemBox.Open_Box();
                        Item[] boxItems = itemBox.Get_Items();
                        Give_Reward_ItemBox(boxItems);

                        _clickedBox = true;

                        yield break;
                    }
                }
            }

            yield return null;
        }
    }

    //고정 순서
    #region 고정순서 : 플레이어 다음으로 몬스터 
    IEnumerator IE_StageBattleProcess_Fixed()
    {
        //플레이어 및 몬스터 위치 배치.
        for(int i = 0; i < PlayerPool.Count; i++)
        {
            PlayerPool[i].IsArrived = false;
            StartCoroutine(IE_LocateProcess(PlayerPool[i], PositionSet_Player[i].position));
        }
        for (int i = 0; i < MonsterPool.Count; i++)
        {
            MonsterPool[i].transform.position = MonsterPool[0].transform.position;
            MonsterPool[i].gameObject.SetActive(true);

            MonsterPool[i].IsArrived = false;
            StartCoroutine(IE_LocateProcess(MonsterPool[i], PositionSet_Monster[i].position));
        }

        //배치 완료 여부 확인
        bool allArrived = false;
        while(!allArrived)
        {
            for(int i = 0; i < PlayerPool.Count; i++)
            {
                if (PlayerPool[i].IsArrived) allArrived = true;
                else allArrived = false;
            }
            for (int i = 0; i < MonsterPool.Count; i++)
            {
                if (MonsterPool[i].IsArrived) allArrived = true;
                else allArrived = false;
            }

            yield return null;
        }

        //반복 로직.
        while (gameState.Equals(GameState.BATTLE))
        {
            //플레이어 로직.
            for (int i = 0; i < PlayerPool.Count; i++)
            {
                turnUnit = PlayerPool[i];
                playerIndex = i;

                if (turnUnit.gameObject.activeSelf)
                {
                    turnUnit.Order();
                    yield return new WaitUntil(() => TurnEnd);
                    TurnEnd = false;
                    playerIndex++;
                }

                if (!Check_AliveMonster())
                {
                    for (int k = 0; k < PlayerPool.Count; k++)
                    {
                        PlayerPool[k].IsArrived = false;
                        StartCoroutine(IE_LocateProcess(PlayerPool[k], prevPos[k]));
                    }
                    allArrived = false;
                    while (!allArrived)
                    {
                        for (int k = 0; k < PlayerPool.Count; k++)
                        {
                            if (PlayerPool[k].IsArrived) allArrived = true;
                            else allArrived = false;
                        }
                        yield return null;
                    }

                    GameSet_IDLE();
                    yield break;
                }
                
            }
            //PlayerBattleUI.SetActive(false);

            //몬스터 로직.
            for(int i = 0; i < MonsterPool.Count; i++)
            {
                turnUnit = MonsterPool[i];
                if (turnUnit.gameObject.activeSelf)
                {    
                    turnUnit.Order();
                    yield return new WaitUntil(() => TurnEnd);
                    TurnEnd = false;
                }

                if (!Check_AlivePlayer()) yield break;
            }

            yield return null;
        }
    }
    #endregion

    #region 고정순서 : 매 턴마다 순서 재설정.
    IEnumerator IE_StageBattleProcess_CheckOrderFactor()
    {
        //플레이어 및 몬스터 위치 배치.
        for (int i = 0; i < PlayerPool.Count; i++)
        {
            PlayerPool[i].IsArrived = false;
            StartCoroutine(IE_LocateProcess(PlayerPool[i], PositionSet_Player[i].position));
        }
        for (int i = 0; i < MonsterPool.Count; i++)
        {
            MonsterPool[i].transform.position = MonsterPool[0].transform.position;
            MonsterPool[i].gameObject.SetActive(true);

            MonsterPool[i].IsArrived = false;
            StartCoroutine(IE_LocateProcess(MonsterPool[i], PositionSet_Monster[i].position));
        }

        //배치 완료 여부 확인
        bool allArrived = false;
        while (!allArrived)
        {
            for (int i = 0; i < PlayerPool.Count; i++)
            {
                if (PlayerPool[i].IsArrived) allArrived = true;
                else allArrived = false;
            }
            for (int i = 0; i < MonsterPool.Count; i++)
            {
                if (MonsterPool[i].IsArrived) allArrived = true;
                else allArrived = false;
            }

            yield return null;
        }

        //전투 중 메세지
        if (!Has_BackHomeIndex())
        {
            if (!MS_EventManager.instance.isEnd_battle)
            {
                MS_EventManager.instance.Pop_EventMessage_Battle();
                yield return new WaitUntil(() => MS_EventManager.instance.isEnd_battle);
            }
        }

        //순서 결정 : 공격순서팩터 확인.
        if (turnPool != null && turnPool.Count != 0) turnPool.Clear();
        //1. 턴 목록 저장.
        for(int i = 0; i < PlayerPool.Count; i++)
        {
            if (PlayerPool[i].Get_currentHP() <= 0) continue;
            turnPool.Add(PlayerPool[i]);
        }
        for(int i = 0; i < MonsterPool.Count; i++)
        {
            if (MonsterPool[i].Get_currentHP() <= 0) continue;
            turnPool.Add(MonsterPool[i]);
        }

        //몬스터 HUD 활성화
        for(int i = 0; i < MonsterPool.Count; i++)
        {
            if (MonsterPool[i].Get_currentHP() <= 0) continue;
            
            MonsterPool[i].Active_HPBar();
        }

        //반복 로직.
        while (gameState.Equals(GameState.BATTLE))
        {
            //턴 재정렬.
            Sort_TurnPool();

            for (int i = 0; i < turnPool.Count; i++)
            {
                turnUnit = turnPool[i];
                if (turnUnit.Get_currentHP() <= 0) continue;

                if (turnUnit.GetComponent<Player>())
                {
                    page_PartyInventory.GetComponent<MS_PartyInventory>().Control_InteratableButtons(true);

                    if (turnUnit.gameObject.activeSelf)
                    {
                        Player tempPlayer = turnPool[i].GetComponent<Player>();
                        playerIndex = tempPlayer.playerIndex;
                        turnUnit.Order();
                        yield return new WaitUntil(() => TurnEnd);
                        TurnEnd = false;
                    }

                    if (!Check_AliveMonster())
                    {
                        for (int k = 0; k < PlayerPool.Count; k++)
                        {
                            PlayerPool[k].IsArrived = false;
                            StartCoroutine(IE_LocateProcess(PlayerPool[k], prevPos[k]));
                        }
                        allArrived = false;
                        while (!allArrived)
                        {
                            for (int k = 0; k < PlayerPool.Count; k++)
                            {
                                if (PlayerPool[k].IsArrived) allArrived = true;
                                else allArrived = false;
                            }
                            yield return null;
                        }

                        GameSet_IDLE();
                        yield break;
                    }
                }
                else if(turnUnit.GetComponent<Monster>())
                {
                    page_PartyInventory.GetComponent<MS_PartyInventory>().Control_InteratableButtons(false);

                    if (turnUnit.gameObject.activeSelf)
                    {
                        Monster tempMonster = turnUnit.GetComponent<Monster>();
                        turnUnit.Order();
                        yield return new WaitUntil(() => TurnEnd);
                        TurnEnd = false;
                    }

                    if (!Check_AlivePlayer()) yield break;
                }
            }
            
            yield return null;
        }
    }
    #endregion

    public bool Check_AlivePlayer()
    {
        bool result = false;
        for(int i = 0; i < PlayerPool.Count; i++)
        {
            if (PlayerPool[i].Get_currentHP() <= 0) result = false;
            else
            {
                result = true;
                break;
            }
        }

        return result;
    }

    public bool Check_AliveMonster(bool notReward = false)
    {
        bool result = false;
        for (int i = 0; i < MonsterPool.Count; i++)
        {
            if (MonsterPool[i].Get_currentHP() <= 0) result = false;
            else
            {
                result = true;
                break;
            }
        }

        if (notReward) return result;

        if(!result) Give_Reward_AfterBattle();

        return result;
    }

    public void GameSet_IDLE()
    {
        //이전에 전투를 했었다면..
        if (gameState.Equals(GameState.BATTLE)) SoundManager.instance.Play_BGM_MainIdle();

        gameState = GameState.IDLE;

        //소비아이템 설정 초기화
        readyItem = null;
        Image_ReadyItem.gameObject.SetActive(false);

        //중간지점
        if (GameManager.instance.Is_MiddleLine())
        {
            Button_BackHome.SetActive(true);
        }

        //플레이어 리더 결정.
        Player reader = null;
        for(int i = 0; i < PlayerPool.Count; i++)
        {
            if (PlayerPool[i].Get_currentHP() > 0)
            {
                reader = PlayerPool[i];
                break;
            }
        }

        //일반모드 프로세스 전환.
        for(int i = 0; i < PlayerPool.Count; i++)
        {
            PlayerPool[i].Set_Reader(reader);
            PlayerPool[i].Set_IdleMode();
        }
        
        StartCoroutine(IE_StageIdleProcess());
    }

    public void GameSet_BATTLE()
    {
        SoundManager.instance.Play_BGM_MainBattle();

        gameState = GameState.BATTLE;

        if (GameManager.instance.Is_MiddleLine())
        {
            Button_BackHome.SetActive(false);
        }

        if (prevPos.Count > 0) prevPos.Clear();

        //플레이어 이전(Idle) 위치값 저장.
        for(int i = 0; i < PlayerPool.Count; i++)
        {
            prevPos.Add(PlayerPool[i].transform.position);
        }

        //전투모드 프로세스 전환.
        //StartCoroutine(IE_StageBattleProcess_Fixed());
        StartCoroutine(IE_StageBattleProcess_CheckOrderFactor());
    }

    public void GameSet_GAMEOVER()
    {
        gameState = GameState.GAMEOVER;

        StartCoroutine(IE_FadeOut_GameOver());
    }

    public void GameSet_NEXTSTAGE()
    {
        gameState = GameState.NEXTSTAGE;

        StartCoroutine(IE_FadeOut());
    }

    IEnumerator IE_FadeOut(bool clickHome = false)
    {
        _fadeBoard.position = new Vector2(20, 0);

        //이벤트 메세지 처리
        if (clickHome)
        {
            MS_EventManager.instance.Pop_EventMessage_BackHome();
            yield return new WaitUntil(() => MS_EventManager.instance.isEnd_backhome);
        }
        else
        {
            MS_EventManager.instance.Pop_EventMessage_NextMap();
            yield return new WaitUntil(() => MS_EventManager.instance.isEnd_next);
        }

        //페이드아웃 처리
        while (_fadeBoard.position.x > 0)
        {
            _fadeBoard.Translate(Vector2.left * _fadeSpeed * Time.deltaTime);

            yield return null;
        }

        //방공호로 이동.
        if (clickHome)
        {
            GameManager.instance.Go_PanicRoom();
            yield break;
        }

        //다음 스테이지로 이동.
        if (!GameManager.instance.Is_EndLine())
        {
            GameManager.instance.Go_NextDungeon();
        }
        //최종지점일 경우 방공호로 이동.
        else
        {
            GameManager.instance.Go_PanicRoom();
            GameManager.instance.Arrived_EndLine();
        }
        
    }

    IEnumerator IE_FadeOut_GameOver()
    {
        //복귀 인덱스 저장.
        Set_BackHomeIndex_Int(GameManager.instance.DungeonCount);

        PlayerGameOverUI.SetActive(true);

        yield return new WaitForSeconds(2.5f);

        SpriteRenderer fadeRender = _fadeBoard.GetComponent<SpriteRenderer>();
        Color tempColor = new Color(255, 255, 255, 0);

        fadeRender.color = tempColor;
        _fadeBoard.position = new Vector2(0, 0);

        while(fadeRender.color.a < 1)
        {
            tempColor.a += (1.5f * Time.deltaTime);
            fadeRender.color = tempColor;
            yield return null;
        }

        yield return new WaitForSeconds(1);

        GameManager.instance.Go_PanicRoom();
    }

    IEnumerator IE_FadeIn()
    {
        _fadeBoard.position = new Vector2(0, 0);

        while(_fadeBoard.position.x > -20)
        {
            _fadeBoard.Translate(Vector2.left * _fadeSpeed * Time.deltaTime);

            yield return null;
        }
        
        //후공 몬스터가 있을 경우
        GameSet_IDLE();
    }

    void Sort_TurnPool()
    {
        if (turnPool.Count <= 1) return;

        #region 공격순서팩터 정렬
        tempQry = from node in turnPool
                      orderby node.Get_OrderFactor() descending
                      select node;
        turnPool = tempQry.ToList();
        #endregion

        #region 아군/적군 정렬
        for (int i = 0; i < turnPool.Count - 1; i++)
        {
            //공격순서값이 같은 경우
            if (turnPool[i].Get_OrderFactor() == turnPool[i + 1].Get_OrderFactor())
            {
                //다음 노드가 플레이어(아군)이라면 교체한다. 선 플레이어 후 몬스터
                if(turnPool[i].GetComponent<Monster>() && turnPool[i + 1].GetComponent<Player>())
                {
                    Unit temp = turnPool[i];
                    turnPool[i] = turnPool[i + 1];
                    turnPool[i + 1] = temp;
                }
            }
        }
        #endregion

        #region 종합능력치 정렬
        onTemp = false;
        tempList.Clear();
        tempStartIdx = 0;
        for (int i = 0; i < turnPool.Count - 1; i++)
        {
            //공격순서값이 같은 경우 && 아군이 선 정렬된 경우
            if (turnPool[i].Get_OrderFactor() == turnPool[i + 1].Get_OrderFactor()
                && ((turnPool[i].GetComponent<Player>() && turnPool[i + 1].GetComponent<Player>()) || (turnPool[i].GetComponent<Monster>() && turnPool[i + 1].GetComponent<Monster>())))
            {
                //같은 노드 임시 저장
                if (!onTemp)
                {
                    tempList.Clear();
                    tempStartIdx = i;
                }
                onTemp = true;

                if (!tempList.Contains(turnPool[i])) tempList.Add(turnPool[i]);
                if (!tempList.Contains(turnPool[i + 1])) tempList.Add(turnPool[i + 1]);
            }
            else
            {

                TempListSort("종합능력치");

                tempList.Clear();
            }
        }

        TempListSort("종합능력치");
        #endregion

        #region 공격력 정렬
        onTemp = false;
        tempList.Clear();
        tempStartIdx = 0;
        for (int i = 0; i < turnPool.Count - 1; i++)
        {
            //공격순서값이 같은 경우 && 아군이 선 정렬된 경우 && 종합 능력치값이 같은 경우
            if (turnPool[i].Get_OrderFactor() == turnPool[i + 1].Get_OrderFactor()
                && ((turnPool[i].GetComponent<Player>() && turnPool[i + 1].GetComponent<Player>()) || (turnPool[i].GetComponent<Monster>() && turnPool[i + 1].GetComponent<Monster>()))
                && turnPool[i].Get_TotalPoint() == turnPool[i + 1].Get_TotalPoint())
            {
                //같은 노드 임시 저장
                if (!onTemp)
                {
                    tempList.Clear();
                    tempStartIdx = i;
                }
                onTemp = true;

                if (!tempList.Contains(turnPool[i])) tempList.Add(turnPool[i]);
                if (!tempList.Contains(turnPool[i + 1])) tempList.Add(turnPool[i + 1]);
            }
            else
            {
                TempListSort("공격력");

                tempList.Clear();
            }
        }
        TempListSort("공격력");
        #endregion

        #region 방어도 정렬
        onTemp = false;
        tempList.Clear();
        tempStartIdx = 0;
        for (int i = 0; i < turnPool.Count - 1; i++)
        {
            //공격순서값이 같은 경우 && 아군이 선 정렬된 경우 && 종합 능력치값이 같은 경우 && 공격력이 같은 경우
            if (turnPool[i].Get_OrderFactor() == turnPool[i + 1].Get_OrderFactor()
                && ((turnPool[i].GetComponent<Player>() && turnPool[i + 1].GetComponent<Player>()) || (turnPool[i].GetComponent<Monster>() && turnPool[i + 1].GetComponent<Monster>()))
                && turnPool[i].Get_TotalPoint() == turnPool[i + 1].Get_TotalPoint()
                && turnPool[i].Get_AttackPower() == turnPool[i + 1].Get_AttackPower())
            {
                //같은 노드 임시 저장
                if (!onTemp)
                {
                    tempList.Clear();
                    tempStartIdx = i;
                }
                onTemp = true;
                if (!tempList.Contains(turnPool[i])) tempList.Add(turnPool[i]);
                if (!tempList.Contains(turnPool[i + 1])) tempList.Add(turnPool[i + 1]);
            }
            else
            {
                TempListSort("방어도");

                tempList.Clear();
            }
        }
        TempListSort("방어도");
        #endregion
    }

    void TempListSort(string KeyCode)
    {
        if (tempList.Count != 0)
        {
            if (onTemp)
            {
                onTemp = false;

                //임시 저장한 리스트 정렬
                switch (KeyCode)
                {
                    case "종합능력치":
                        tempQry = from node in tempList
                                  orderby node.Get_TotalPoint() descending
                                  select node;

                        break;
                    case "공격력":
                        tempQry = from node in tempList
                                  orderby node.Get_AttackPower() descending
                                  select node;

                        break;
                    case "방어도":
                        tempQry = from node in tempList
                                  orderby node.Get_DefencePower() descending
                                  select node;

                        break;
                }
                tempList = tempQry.ToList();

                //정렬된 리스트로 원본 변경.
                for (int k = 0; k < tempList.Count; k++)
                {
                    turnPool[tempStartIdx + k] = tempList[k];
                }
            }
        }
    }
    
    public void Give_Reward_AfterBattle()
    {
        //드랍 아이템 갯수 결정
        int maxCount = 0;
        int randomKey = Random.Range(0, 100);
        if (_firstRewardRate >= randomKey) maxCount++;
        if (_secondRewardRate >= randomKey) maxCount++;
        if (_thirdRewardRate >= randomKey) maxCount++;
        
        //드랍 아이템 생성.
        for(int i = 0; i < maxCount; i++)
        {
            RW_Items[i].Set_RewardItem(GameManager.instance.Get_Random_Item_RewardSpecialRate());
        }

        //아이템이 없다.
        if (maxCount == 0)
        {
            RW_Msg.text = "쓸만한 물건이 없습니다.";
            return;
        }
        else
        {
            RW_Msg.text = "아이템을 얻었습니다.";
            RW_UI.SetActive(true);
        }

        //감염된 플레이어와 전투했을 경우 해당 아이템을 필수 드랍한다.
        if (currInfectedPlayer != null) RW_Items[0].Set_RewardItem(currInfectedPlayer.privateItem);

        //UI 활성화/비활성화
        for(int i = 0; i < RW_Items.Count; i++)
        {
            if (RW_Items[i].Get_RewardItemId() == null) RW_Items[i].gameObject.SetActive(false);
            else RW_Items[i].gameObject.SetActive(true);
        }
    }

    private void Give_Reward_ItemBox(Item[] boxItems)
    {
        for (int i = 0; i < boxItems.Length; i++)
        {
            if (boxItems[i] == null) break;
            RW_Items[i].Set_RewardItem(boxItems[i]);
        }

        //아이템이 없다.
        if (boxItems[0] == null)
        {
            RW_Msg.text = "쓸만한 물건이 없습니다.";
            return;
        }
        else
        {
            RW_Msg.text = "아이템을 얻었습니다.";
            RW_UI.SetActive(true);
        }

        //UI 활성화/비활성화
        for (int i = 0; i < RW_Items.Count; i++)
        {
            if (RW_Items[i].Get_RewardItemId() == null) RW_Items[i].gameObject.SetActive(false);
            else RW_Items[i].gameObject.SetActive(true);
        }
    }

    public void Update_PartyInventoryDisplayer()
    {
        page_PartyInventory.GetComponent<MS_PartyInventory>().Init_Slots();
    }

    private void Calculate_InfectedPlayerDistance()
    {
        if (currInfectedPlayer == null)
        {
            return;
        }
        if (!currInfectedPlayer.gameObject.activeSelf)
        {
            return;
        }

        StartCoroutine(IE_Calculate_InfectedPlayerDistance());
    }

    IEnumerator IE_Calculate_InfectedPlayerDistance()
    {
        //리더 플레이어 지정.
        Player leader = null;
        for(int i = 0; i < PlayerPool.Count; i++)
        {
            if (PlayerPool[i].Get_currentHP() > 0)
            {
                leader = PlayerPool[i];
                break;
            }
        }

        //거리 측정.
        while(currInfectedPlayer.gameObject.activeSelf)
        {
            if (gameState.Equals(GameState.BATTLE)) yield break;

            float distance = Mathf.Abs(leader.transform.position.x - MonsterPool[0].transform.position.x);
            if (distance < _detectDistance)
            {
                GameSet_BATTLE();
            }

            yield return null;
        }
    }

    private void Calculate_MonsterDistance()
    {
        if (MonsterPool.Count == 0) return;

        if (MS_EventManager.instance.OnPreEmptive) StartCoroutine(IE_Calculate_MonsterDistance());
    }

    IEnumerator IE_Calculate_MonsterDistance()
    {
        //리더 플레이어 지정.
        Player leader = null;
        for (int i = 0; i < PlayerPool.Count; i++)
        {
            if (PlayerPool[i].Get_currentHP() > 0)
            {
                leader = PlayerPool[i];
                break;
            }
        }

        //거리 측정.
        while (MonsterPool[0].gameObject.activeSelf)
        {
            if (gameState.Equals(GameState.BATTLE)) yield break;

            float distance = Mathf.Abs(leader.transform.position.x - MonsterPool[0].transform.position.x);
            if (distance < _detectDistance)
            {
                GameSet_BATTLE();
            }

            yield return null;
        }
    }

    private void Set_BGResource()
    {
        if(GameManager.instance.mainStage_Mission.Contains("병원"))
        {
            if (GameManager.instance.DungeonCount < 6) return;

            if (targetBG.Equals(rscBG_Hospital)) return;
            targetBG.sprite = rscBG_Hospital;
        }
        else
        {
            if (targetBG.Equals(rscBG_Street)) return;
            targetBG.sprite = rscBG_Street;
        }
    }

}
