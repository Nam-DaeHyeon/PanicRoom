using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.IO;

public class MS_EventManager : MonoBehaviour
{
    public Text testCount;

    enum EventState
    {
        Idle_BeforeBattle,
        Idle_AfterBattle,
        Battle,
        Next,
        BackHome,
        Item,
        ClickObject
    }

    EventState currEventState;
    [SerializeField] float _typingTime = 0.08f;
    [SerializeField] float _floatingTime = 1.5f;

    [Header("Component & Object")]
    [SerializeField] GameObject _imageObj;
    [SerializeField] GameObject _textObj;
    [SerializeField] GameObject _tellerObj;
    [SerializeField] GameObject _questObj;
    [SerializeField] GameObject _buttonSkipObj;

    [SerializeField] Image _imageField;
    [SerializeField] Text _commentField;
    [SerializeField] Text _tellerField;
    [SerializeField] Text _questField;

    [System.Serializable]
    public class narrationSet
    {
        public string cutImageName;

        [TextArea(3, 10)]
        public string narration;
        public string narrator;
        public string narratorBoardPos = "LEFT";
    }

    [Header("Narrations")]
    [SerializeField] List<narrationSet> _idleBeforeNarr = new List<narrationSet>();
    [SerializeField] List<narrationSet> _idleAfterNarr = new List<narrationSet>();
    [SerializeField] List<narrationSet> _battleNarr = new List<narrationSet>();
    [SerializeField] List<narrationSet> _nextNarr = new List<narrationSet>();
    [SerializeField] List<narrationSet> _backhomeNarr = new List<narrationSet>();
    [SerializeField] List<narrationSet> _itemNarr = new List<narrationSet>();
    [SerializeField] List<narrationSet> _objectNarr = new List<narrationSet>();
    [SerializeField] List<string> _actionEvents = new List<string>();
    [SerializeField] List<string> _popUps1 = new List<string>();
    [SerializeField] List<string> _popUps2 = new List<string>();
    [SerializeField] string _questMessege;

    public bool isEnd_idleBefore = false;
    public bool isEnd_idleAfter = false;
    public bool isEnd_battle = false;
    public bool isEnd_next = false;
    public bool isEnd_backhome = false;
    public bool isEnd_item = false;
    public bool isEnd_clickObj = false;
    public bool isClosed_pop = false;

    public bool OnSupporter_Meet;   //합류 이전
    public bool OnSupporter_Accompany;  //합류
    public bool OnSupporter_Escape; //합류 이탈
    public bool OnEnemy_BlackSheep;

    public bool OnEssentialItem;    //아이템 상자 필수 클릭.
    public bool OnActionEvent;      //특수 연출
    public bool OnPreEmptive;       //선제공격 여부
    
    public static MS_EventManager instance;
    private void Awake()
    {
        instance = this;


        GameManager.instance.Set_StageList();

        //튜토리얼 중일떄
        if (PlayerPrefs.GetString("PlayTutorial", "F").Equals("F"))
        {
            Read_XML_GetStageMapKey("TutorialStoryBoard");
        }
        //기본
        else
        {
            Read_XML_GetStageMapKey("MainStoryBoard");
        }
    }

    private void Start()
    {
        //현재 미션 표시.
        if (GameManager.instance.mainStage_Mission != null)
        {
            if (!GameManager.instance.mainStage_Mission.Trim().Equals(""))
            {
                _questField.text = GameManager.instance.mainStage_Mission;
                _questObj.SetActive(true);
            }
        }
    }

    private void Update()
    {
        //testCount.text = GameManager.instance.stageList == null ? "NULL" : GameManager.instance.stageList.Count.ToString();
        testCount.text = _idleBeforeNarr.Count + " " + _idleAfterNarr.Count + " " + _objectNarr.Count + " " + _popUps1.Count + " " + _popUps2.Count
                            + " = " + IsPlayed_StoryLine() + " : " + _commentField.text;
    }

    public void Pop_EventMessage_IdleBeforeBattle()
    {
        isEnd_idleBefore = false;
        StopAllCoroutines();
        currEventState = EventState.Idle_BeforeBattle;
        if (IsPlayed_StoryLine())
        {
            Set_End_Message();
            return;
        }
        StartCoroutine(IE_Animate_Message());
    }

    public void Pop_EventMessage_IdleAfterBattle()
    {
        isEnd_idleAfter = false;
        StopAllCoroutines();
        currEventState = EventState.Idle_AfterBattle;
        if (IsPlayed_StoryLine())
        {
            Set_End_Message();
            return;
        }
        StartCoroutine(IE_Animate_Message());
    }

    public void Pop_EventMessage_Battle()
    {
        isEnd_battle = false;
        StopAllCoroutines();
        currEventState = EventState.Battle;
        if (IsPlayed_StoryLine())
        {
            Set_End_Message();
            return;
        }
        StartCoroutine(IE_Animate_Message());
    }

    public void Pop_EventMessage_NextMap()
    {
        isEnd_next = false;
        StopAllCoroutines();
        currEventState = EventState.Next;
        if (IsPlayed_StoryLine())
        {
            Set_End_Message();
            return;
        }
        StartCoroutine(IE_Animate_Message());
    }

    public void Pop_EventMessage_BackHome()
    {
        isEnd_backhome = false;
        StopAllCoroutines();
        currEventState = EventState.BackHome;
        if (IsPlayed_StoryLine())
        {
            Set_End_Message();
            return;
        }
        StartCoroutine(IE_Animate_Message());
        
        //복귀 인덱스 저장.
        StageManager.instance.Set_BackHomeIndex_Int(GameManager.instance.DungeonCount);
    }

    public void Pop_EventMessage_MainItem()
    {
        isEnd_item = false;
        StopAllCoroutines();
        currEventState = EventState.Item;
        if (IsPlayed_StoryLine())
        {
            Set_End_Message();
            return;
        }
        StartCoroutine(IE_Animate_Message());
    }

    public void Pop_EventMessage_ClickObject()
    {
        isEnd_clickObj = false;
        StopAllCoroutines();
        currEventState = EventState.ClickObject;
        if (IsPlayed_StoryLine())
        {
            Set_End_Message();
            return;
        }
        StartCoroutine(IE_Animate_Message());
    }

    public bool IsPlayed_StoryLine()
    {
        if(PlayerPrefs.HasKey("BackHomeIndex"))
        {
            if (GameManager.instance.DungeonCount <= StageManager.instance.Get_BackHomeIndex())
            {
                return true;
            }
        }

        return false;
    }

    IEnumerator IE_Animate_Message()
    {
        //플레이어 이동제한
        for (int i = 0; i < StageManager.instance.PlayerPool.Count; i++)
        {
            StageManager.instance.PlayerPool[i]._inMessage = true;
        }

        //이벤트 메세지 로드
        List<narrationSet> _narrations = null;
        switch (currEventState)
        {
            //case EventState.Idle_BeforeBattle:
            default:
                _narrations = _idleBeforeNarr;
                break;
            case EventState.Idle_AfterBattle:
                _narrations = _idleAfterNarr;
                break;
            case EventState.Battle:
                _narrations = _battleNarr;
                break;
            case EventState.Next:
                _narrations = _nextNarr;
                break;
            case EventState.BackHome:
                _narrations = _backhomeNarr;
                break;
            case EventState.Item:
                _narrations = _itemNarr;
                break;
            case EventState.ClickObject:
                _narrations = _objectNarr;
                break;
        }

        //생략(스킵) 버튼 비활성화
        if (_buttonSkipObj.activeSelf) _buttonSkipObj.SetActive(false);

        //이벤트 메세지 출력
        for (int i = 0; i < _narrations.Count; i++)
        {
            if (_narrations[i] == null) continue;
            
            //컷이미지
            if (_narrations[i].cutImageName != null)
            {
                _imageField.sprite = Resources.Load<Sprite>("Event/" + _narrations[i].cutImageName);
                _imageObj.SetActive(true);
            }
            else
            {
                if (_imageObj.activeSelf)
                {
                    _imageObj.SetActive(false);
                }
            }

            //내레이터
            if (_narrations[i].narrator != null)
            {
                //위치보정.
                float _x = _tellerObj.transform.localPosition.x;
                if (_narrations[i].narratorBoardPos.Equals("LEFT"))
                {
                    if (_x > 0) _tellerObj.transform.localPosition = new Vector2(-1 * _x, _tellerObj.transform.localPosition.y);
                }
                else if(_narrations[i].narratorBoardPos.Equals("RIGHT"))
                {
                    if(_x < 0) _tellerObj.transform.localPosition = new Vector2(-1 * _x, _tellerObj.transform.localPosition.y);
                }

                //이벤트 여부 확인
                if (_narrations[i].narrator.Equals("<event>"))
                {
                    StageManager.instance.Action_NPC(_narrations[i].narration.Trim());
                    continue;
                }
                else
                {
                    _tellerField.text = _narrations[i].narrator;
                    if (!_tellerObj.activeSelf) _tellerObj.SetActive(true);
                }
            }
            else
            {
                if (_tellerObj.activeSelf)
                {
                    _tellerField.text = null;
                    _tellerObj.SetActive(false);
                }
            }

            //내레이션
            if (_narrations[i].narration != null)
            {
                //그냥 띄우는 연출
                //_commentField.text = _narrations[i].narration;
                //if (!_textObj.activeSelf) _textObj.SetActive(true);

                //타자치는 연출
                _commentField.text = string.Empty;
                if (!_textObj.activeSelf) _textObj.SetActive(true);
                char[] charArray = _narrations[i].narration.ToCharArray();
                for (int c = 0; c < charArray.Length; c++)
                {
                    _commentField.text += charArray[c];
                    if (c != charArray.Length - 1) yield return new WaitForSeconds(_typingTime);
                }

                if (!_buttonSkipObj.activeSelf)
                {
                    if (i != _narrations.Count - 1) _buttonSkipObj.SetActive(true);
                }
            }
            else
            {
                if (_textObj.activeSelf)
                {
                    _commentField.text = null;
                    _textObj.SetActive(false);
                }
            }

            //플로팅 타임
            if (_narrations[i].narration != null || _narrations[i].cutImageName != null)
                yield return new WaitForSeconds(_floatingTime);
        }

        //미션 갱신
        if (_questMessege != null && !_questMessege.Trim().Equals(""))
        {
            _questField.text = _questMessege;
            _questObj.SetActive(true);

            GameManager.instance.mainStage_Mission = _questMessege;
        }

        //이벤트 메세지 출력 종료.
        Set_End_Message();

        yield break;
    }

    private void Test_Read_XML()
    {
        TextAsset textXml = Resources.Load("MainStoryBoard", typeof(TextAsset)) as TextAsset;

        XmlNodeList xmlNodeList = null;
        XmlDocument xmlDoc = new XmlDocument();

        // XML 로드하고.
        xmlDoc.LoadXml(textXml.ToString());

        // 최 상위 노드 선택.
        xmlNodeList = xmlDoc.SelectNodes("MainStoryBoard");

        foreach (XmlNode node in xmlNodeList)
        {
            // 자식이 있을 때에 돌아요.
            if (node.Name.Equals("MainStoryBoard") && node.HasChildNodes)
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    Debug.Log("StageIndex : " + child.Attributes.GetNamedItem("StageIndex").Value);
                    Debug.Log("MapIndex : " + child.Attributes.GetNamedItem("MapIndex").Value);
                    Debug.Log("Teller : " + child.Attributes.GetNamedItem("Teller").Value);
                    Debug.Log("Narration : " + child.Attributes.GetNamedItem("Narration").Value);
                }
            }
        }
    }

    public void Read_XML_GetStageMapKey(string xmlFileName)
    {
        //메세지 초기화
        if (_idleBeforeNarr.Count != 0) _idleBeforeNarr.Clear();
        if (_idleAfterNarr.Count != 0) _idleAfterNarr.Clear();
        if (_battleNarr.Count != 0) _battleNarr.Clear();
        if (_nextNarr.Count != 0) _nextNarr.Clear();
        if (_backhomeNarr.Count != 0) _backhomeNarr.Clear();
        if (_itemNarr.Count != 0) _itemNarr.Clear();

        //XML 등록
        TextAsset textXml = Resources.Load(xmlFileName, typeof(TextAsset)) as TextAsset;

        XmlNodeList xmlNodeList = null;
        XmlDocument xmlDoc = new XmlDocument();
        string _mapIndex, _tellingOrder, _teller, _narration, _cutImageName, _quest;
        bool onRight = false;

        // XML 로드하고.
        xmlDoc.LoadXml(textXml.ToString());

        // 최 상위 노드 선택.
        xmlNodeList = xmlDoc.SelectNodes(xmlFileName);

        foreach (XmlNode node in xmlNodeList)
        {
            // 자식이 있을 때에 돌아요.
            if (node.Name.Equals(xmlFileName) && node.HasChildNodes)
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    _mapIndex = child.Attributes.GetNamedItem("MapIndex").Value;
                    if (GameManager.instance.DungeonCount != int.Parse(_mapIndex)) continue;

                    _tellingOrder = child.Attributes.GetNamedItem("TellingOrder").Value;
                    _teller = (child.Attributes.GetNamedItem("Teller") == null) ? null : child.Attributes.GetNamedItem("Teller").Value;
                    _narration = (child.Attributes.GetNamedItem("Narration") == null) ? null : child.Attributes.GetNamedItem("Narration").Value;
                    _cutImageName = (child.Attributes.GetNamedItem("CutImageName") == null) ? null : child.Attributes.GetNamedItem("CutImageName").Value;
                    _quest = (child.Attributes.GetNamedItem("Quest") == null) ? null : child.Attributes.GetNamedItem("Quest").Value;

                    if (child.Attributes.GetNamedItem("Unit_ID") != null)
                    {
                        switch (child.Attributes.GetNamedItem("Unit_ID").Value.Trim())
                        {
                            case "조력자_합류이전":
                                OnSupporter_Meet = true;
                                onRight = true;
                                break;
                            case "조력자_합류":
                                OnSupporter_Accompany = true;
                                onRight = false;
                                break;
                            case "조력자_이탈":
                                OnSupporter_Escape = true;
                                onRight = false;
                                break;
                            case "검은양":
                                OnEnemy_BlackSheep = true;
                                onRight = true;
                                break;
                        }
                    }
                    else onRight = false;

                    narrationSet newSet = new narrationSet();
                    newSet.cutImageName = _cutImageName;
                    newSet.narrator = _teller;
                    newSet.narratorBoardPos = (onRight) ? "RIGHT" : "LEFT";
                    newSet.narration = _narration;
                    _questMessege = _quest;

                    switch (_tellingOrder)
                    {
                        //case "before":
                        default:
                            _idleBeforeNarr.Add(newSet);
                            break;
                        case "after":
                            _idleAfterNarr.Add(newSet);
                            break;
                        case "battle":
                            _battleNarr.Add(newSet);
                            break;
                        case "next":
                            _nextNarr.Add(newSet);
                            break;
                        case "backhome":
                            _backhomeNarr.Add(newSet);
                            break;
                        case "item":
                            _itemNarr.Add(newSet);
                            break;
                        case "pop2":
                            _popUps2.Add(newSet.narration.Trim());
                            break;
                        case "object_pop1":
                            _popUps1.Add(newSet.narration.Trim());
                            OnEssentialItem = true;
                            break;
                        case "pre-emptive_pop1":
                            _popUps1.Add(newSet.narration.Trim());
                            OnPreEmptive = true;
                            break;
                        case "object":
                            _objectNarr.Add(newSet);
                            OnEssentialItem = true;
                            break;
                        case "event":
                            _actionEvents.Add(newSet.narration.Trim());
                            break;
                    }

                }
            }
        }
    }

    public int Get_AfterBattleCount() { return _idleAfterNarr.Count; }
    public int Get_BattleCount() { return _battleNarr.Count; }
    
    //FIFO
    public string Get_PopMessage_Index0(bool prevBeforeNarr)
    {
        List<string> popUps;
        if (prevBeforeNarr) popUps = _popUps1;
        else popUps = _popUps2;

        if (popUps.Count == 0) return null;

        string tempStr = popUps[0].Trim();

        if (tempStr == "") return null;
        return tempStr;
    }

    public void Remove_PopMessage_Index0(bool prevBeforeNarr)
    {
        List<string> popUps;
        if (prevBeforeNarr) popUps = _popUps1;
        else popUps = _popUps2;

        popUps.RemoveAt(0);
    }

    public List<string> Get_List_ActionEvents() { return _actionEvents; }
    public List<string> Get_List_ActionEvents_inNarrs()
    {
        List<string> temp = new List<string>();
        for(int i = 0; i < _idleBeforeNarr.Count; i++)
        {
            if (_idleBeforeNarr[i].narrator == null) continue;
            if (_idleBeforeNarr[i].narration == null) continue;

            if (_idleBeforeNarr[i].narrator.Trim().Equals("<event>")) temp.Add(_idleBeforeNarr[i].narration.Trim());
        }

        return temp;
    }
    public void UI_Skip_Message()
    {
        StopAllCoroutines();
        
        //미션 갱신
        if (_questMessege != null && !_questMessege.Trim().Equals(""))
        {
            _questField.text = _questMessege;
            _questObj.SetActive(true);

            GameManager.instance.mainStage_Mission = _questMessege;
        }

        StageManager.instance.Skiped_Action_NPC();

        Set_End_Message();
    }

    private void Set_End_Message()
    {
        if (_imageObj.activeSelf) _imageObj.SetActive(false);
        if (_textObj.activeSelf) _textObj.SetActive(false);
        switch (currEventState)
        {
            //case EventState.Idle_BeforeBattle:
            default:
                isEnd_idleBefore = true;
                break;
            case EventState.Idle_AfterBattle:
                isEnd_idleAfter = true;
                break;
            case EventState.Battle:
                isEnd_battle = true;
                break;
            case EventState.Next:
                isEnd_next = true;
                break;
            case EventState.BackHome:
                isEnd_backhome = true;
                break;
            case EventState.Item:
                isEnd_item = true;
                break;
            case EventState.ClickObject:
                isEnd_clickObj = true;
                break;
        }

        //이동제한 해제
        for(int i = 0; i < StageManager.instance.PlayerPool.Count; i++)
        {
            StageManager.instance.PlayerPool[i]._inMessage = false;
        }
    }
}
