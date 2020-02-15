using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : Unit
{
    public float _RadioactivityRes;    //방사능 저항
    public float _RadioactivityResRate;    //방사능 저항률
    public float _AggroFactor;      //도발수치
    
    [Header("Parameter")]
    [SerializeField] float _currInfectedGauge;
    [SerializeField] float _maxInfectedGauge = 100;
    public bool IsInfected = false;

    [Header("UI")]
    [SerializeField] Transform HUDPos;
    [SerializeField] GameObject UI_HUD;
    [SerializeField] Image UI_HPBar;
    [SerializeField] Image UI_InfectedBar;
    [SerializeField] Text UI_InfectedText;

    public int playerIndex;     //턴전투시스템에서 사용되는 플레이어 개별 인덱스 값

    Unit _target;
    float _distance;
    [HideInInspector] public bool _inMessage;

    float _idleDistance;    //리더와의 거리값
    Player _reader;
    Vector3 _battleOriginPos;

    private void Start()
    {
        Restart_UIManager();
    }

    public void Restart_UIManager()
    {
        StartCoroutine(IE_UIManager());
    }

    IEnumerator IE_UIManager()
    {
        if (UI_HUD == null) yield break;
        UI_HUD.SetActive(true);

        while(true)
        {
            UI_HUD.transform.position = HUDPos.position;

            UI_HPBar.fillAmount = _currHp / _maxHp;
            UI_InfectedBar.fillAmount = _currInfectedGauge / _maxInfectedGauge;
            UI_InfectedText.text = ((int)_currInfectedGauge).ToString() + "%";

            yield return null;
        }
    }

    public override void Set_DetailStatus()
    {
        base.Set_DetailStatus();

        _RadioactivityRes = ab_res;
        _RadioactivityResRate = 20 / (_RadioactivityRes + 20);

        _AggroFactor = 100 + _attack;

        //아이템 부가 능력치 적용
        if (privateItem == null) return;
        //_attack += privateItem.add_attackPower;
        //_criticalRate += privateItem.add_criticalRate;
        //_avoidRate += privateItem.add_avoidRate;
        _attack += privateItem.Get_AttackPower();
        _canLongDistnace = privateItem.Get_CanLongDistance();
    }

    public void Set_InitParameter(int plyIdx)
    {
        if (plyIdx >= GameManager.instance.goPlayers.Count) return;

        //그외 캐릭터별 능력치 초기화
        _nickName = GameManager.instance.goPlayers[plyIdx].characterName;
        _currInfectedGauge = GameManager.instance.goPlayers[plyIdx].exposure;
        ab_str = GameManager.instance.goPlayers[plyIdx].str;
        ab_dex = GameManager.instance.goPlayers[plyIdx].dex;
        ab_con = GameManager.instance.goPlayers[plyIdx].con;
        ab_res = GameManager.instance.goPlayers[plyIdx].res;
        privateItem = GameManager.instance.goPlayers[plyIdx].privateItem;

        if (_currInfectedGauge >= _maxInfectedGauge) IsInfected = true;

        //세부 스테이터스 값 정의.
        Set_DetailStatus();

        if (GameManager.instance.isFirst) _currHp = _maxHp;
        else _currHp = GameManager.instance.goPlayers[plyIdx].conHP;

    }

    public void Set_Reader(Player player)
    {
        _reader = player;

        if(_idleDistance == 0) _idleDistance = Vector2.Distance(_reader.transform.position, transform.position);
    }

    public void Set_IdleMode()
    {
        if (_currHp <= 0) return;

        StartCoroutine(IE_PlayerIdleProcess());
    }

    IEnumerator IE_PlayerIdleProcess()
    {
        yield return new WaitUntil(() => !StageManager.instance.RW_UI.activeSelf);

        Vector2 dir;
        float h;
        
        _distance = Vector2.Distance(_reader.transform.position, transform.position);
        while (StageManager.instance.gameState.Equals(GameState.IDLE))
        {
            if (_reader == this)
            {
                h = Input.GetAxis("Horizontal");
                //이벤트메세지 출력중 이동제한
                if(!_inMessage) transform.Translate(Vector2.right * h * _speed * Time.deltaTime);
                
                //좌측 이동 좌표 제한.
                if (transform.position.x < -9)  
                {
                    transform.position = new Vector2(-9, transform.position.y);
                }
            }
            else
            {
                _distance = Vector2.Distance(_reader.transform.position, transform.position);

                if (_distance > _idleDistance)
                {
                    dir = _reader.transform.position - transform.position;
                    dir = dir.normalized;
                    transform.Translate(dir * _speed * Time.deltaTime);
                }
            }

            yield return null;
        }
    }

    public void Take_RP_Heal(int amount)
    {
        _currInfectedGauge -= amount;
        if (_currInfectedGauge <= 0) _currInfectedGauge = 0;
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        StageManager.instance.Get_HitLog(_EndDamage.ToString(), HUDPos.position, "WHITE");

        if (_currHp <= 0)
        {
            HideHUD();
            gameObject.SetActive(false);

            //다른 플레이어 상태 확인.
            if(!StageManager.instance.Check_AlivePlayer())
            {
                StageManager.instance.GameSet_GAMEOVER();
            }
        }
    }

    public override void TakeDebuffDamage(float damage)
    {
        base.TakeDebuffDamage(damage);

        //StageManager.instance.Get_HitLog(_EndDamage.ToString(), HUDPos.position, "RED");

        if (_currHp <= 0)
        {
            HideHUD();
            gameObject.SetActive(false);

            //다른 플레이어 상태 확인.
            if (!StageManager.instance.Check_AlivePlayer())
            {
                StageManager.instance.GameSet_GAMEOVER();
            }
        }
    }

    public void ToxicityUp(float amount)
    {
        if (_currHp <= 0) return;
        if (IsInfected) return;

        _currInfectedGauge += amount;
        if (_currInfectedGauge > _maxInfectedGauge)
        {
            IsInfected = true;
            _currInfectedGauge = _maxInfectedGauge;
        }
    }

    public override void Order()
    {
        base.Order();

        if (StageManager.instance.TurnEnd) return;

        _outline.ShowHide_Outline(true);

        //입력 대기
        StartCoroutine(IE_ReadyClick_Target());
    }
    
    IEnumerator IE_ReadyClick_Target()
    {
        _target = null;
        Item readyItem = null;

        //턴넘기기 입력 딜레이
        yield return new WaitForSeconds(0.1f);

        while (_target == null)
        {
            readyItem = StageManager.instance.readyItem;
            
            //입력 대기
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                Vector2 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                Ray2D ray2d = new Ray2D(wp, Vector2.zero);
                RaycastHit2D hit2d = Physics2D.Raycast(ray2d.origin, ray2d.direction);
                if (hit2d)
                {
                    //아이템 선택에 따른 타겟 대상 설정.
                    if (readyItem != null)
                    {
                        for (int i = 0; i < readyItem.Get_ItemOptions().Length; i++)
                        {
                            switch (readyItem.Get_ItemOptions()[i])
                            {
                                case ItemOption.방사능피폭완전제거_단일아군:
                                case ItemOption.방사능피폭제거_단일아군:
                                case ItemOption.체력회복_단일아군:
                                case ItemOption.출혈효과제거:
                                    //StageManager.instance.Enable_UnitCollider(true, false);
                                    _target = hit2d.transform.GetComponent<Player>();
                                    break;
                                case ItemOption.데미지_단일적군:
                                case ItemOption.데미지_전체적군:
                                    //StageManager.instance.Enable_UnitCollider(false, true);
                                    _target = hit2d.transform.GetComponent<Monster>();
                                    break;
                            }
                        }
                    }
                    else
                        _target = hit2d.transform.GetComponent<Monster>();
                }
            }
            else if(Input.GetMouseButtonDown(1))
            {
                PassMyTurn();

                readyItem = null;
                StageManager.instance.readyItem = readyItem;
                StageManager.instance.Image_ReadyItem.gameObject.SetActive(false);
                yield break;
            }

            yield return null;
        }

        //아이템을 사용한 경우
        if (readyItem != null)
        {
            UseItem(readyItem);

            readyItem = null;
            StageManager.instance.readyItem = readyItem;
            StageManager.instance.Image_ReadyItem.gameObject.SetActive(false);
        }
        //아이템 미사용일 경우 : 일반 공격.
        else Attack(_target as Monster);
    }

    //공격 명령.
    public void Attack(Monster target)
    {
        //타겟 결정
        _target = target;

        _battleOriginPos = transform.position;
        if (!_canLongDistnace) StartCoroutine(IE_PlayerBattleProcess());
        else
        {
            ToxicityUp(3 * _RadioactivityResRate);
            StartCoroutine(IE_PlayerBattleProcess_LongDistance());
        }
    }

    //공격 로직 : 근접 공격
    void AttackMove(Vector3 targetPos)
    {
        Vector2 dir = targetPos - transform.position;
        dir = dir.normalized;
        transform.Translate(dir * 10f * Time.deltaTime);
    }

    IEnumerator IE_PlayerBattleProcess()
    {
        StageManager.instance.Set_RenderSet_MyTurnStart(this);

        //공격 모션 : 타겟앞으로 이동.
        do
        {
            _distance = Vector2.Distance(_target.transform.position, transform.position);
            AttackMove(_target.transform.position);
            yield return null;
        } while (_distance > 0.3f);

        //공격 모션 : 타겟 공격.
        //1. 회피 여부
        if(_avoidRate <= Random.Range(0, 100))
        {
            SoundManager.instance.Play_WeaponSE(privateItem);

            //2. 치명타 여부
            _EndDamage = (_criticalRate <= Random.Range(0, 100))? _attack * _criticalDamage : _attack;
            _target.TakeDamage(_EndDamage);
            if (privateItem != null) privateItem.Use_Item(_target);
        }
        else
        {
            //Miss
            SoundManager.instance.Play_SE_Miss();
            StageManager.instance.Get_HitLog("MISS", _target.Get_HUDPosition(), "WHITE");
        }

        StageManager.instance.Set_RenderSet_MyTurnEnd(this);

        //공격 모션 : 원래 자리로 이동.
        do
        {
            _distance = Vector2.Distance(_battleOriginPos, transform.position);
            AttackMove(_battleOriginPos);
            yield return null;
        } while (_distance > 0.1f);

        //턴 종료
        transform.position = _battleOriginPos;
        _outline.ShowHide_Outline(false);
        StageManager.instance.TurnEnd = true;
        Set_Down_DebuffTurn();

        yield break;
    }

    IEnumerator IE_PlayerBattleProcess_LongDistance()
    {
        //공격 모션 : 타겟 공격.

        //1. 회피 여부
        if (_avoidRate <= Random.Range(0, 100))
        {
            SoundManager.instance.Play_WeaponSE(privateItem);

            //2. 치명타 여부
            _EndDamage = (_criticalRate <= Random.Range(0, 100)) ? _attack * _criticalDamage : _attack;
            _target.TakeDamage(_EndDamage);
            if (privateItem != null) privateItem.Use_Item(_target);
        }
        else
        {
            //Miss
            SoundManager.instance.Play_SE_Miss();
            StageManager.instance.Get_HitLog("MISS", _target.Get_HUDPosition(), "WHITE");
        }

        //턴 종료
        transform.position = _battleOriginPos;
        _outline.ShowHide_Outline(false);
        StageManager.instance.TurnEnd = true;
        Set_Down_DebuffTurn();

        yield break;
    }

    public void UseItem(Item usingItem)
    {
        //전체 대상
        if (usingItem.Check_AllTarget())
        {
            if (_target is Player)
            {
                for(int i = 0; i < StageManager.instance.PlayerPool.Count; i++)
                {
                    usingItem.Use_Item(StageManager.instance.PlayerPool[i]);
                }
            }
            if(_target is Monster)
            {
                for (int i = 0; i < StageManager.instance.MonsterPool.Count; i++)
                {
                    usingItem.Use_Item(StageManager.instance.MonsterPool[i]);
                }
            }
        }
        //단일 대상
        else usingItem.Use_Item(_target);

        GameManager.instance.Remove_Item_PartyInventory(usingItem.Get_ItemId());
        StageManager.instance.Update_PartyInventoryDisplayer();

        //턴 종료
        _outline.ShowHide_Outline(false);
        StageManager.instance.TurnEnd = true;
        Set_Down_DebuffTurn();

    }

    public void PassMyTurn()
    {
        ToxicityUp(1);
        
        //턴 종료
        _outline.ShowHide_Outline(false);
        StageManager.instance.TurnEnd = true;
        Set_Down_DebuffTurn();
    }

    public void HideHUD()
    {
        UI_HUD.SetActive(false);
    }

    public void Set_HUDElements(Player srcPlayer)
    {
        playerIndex = srcPlayer.playerIndex;

        UI_HUD = srcPlayer.UI_HUD;
        UI_HPBar = srcPlayer.UI_HPBar;
        UI_InfectedBar = srcPlayer.UI_InfectedBar;
        UI_InfectedText = srcPlayer.UI_InfectedText;
    }

    public float GetCurrInfectedGauge()
    {
        return _currInfectedGauge;
    }

    /// <summary>
    /// Get Image FillAmount : Float
    /// </summary>
    /// <param name="keyCode">HP | Exposure</param>
    /// <returns></returns>
    public float Get_FillAmount(string keyCode)
    {
        if (keyCode.Equals("HP"))
        {
            return UI_HPBar.fillAmount;
        }
        if (keyCode.Equals("Exposure"))
        {
            return UI_InfectedBar.fillAmount;
        }

        return 0;
    }

    public override Vector3 Get_HUDPosition() { return HUDPos.position; }

    private void OnMouseOver()
    {
        StageManager.instance.Update_MouseOverPlayerData(this);
    }

    private void OnMouseExit()
    {
        StageManager.instance.MOVER_FrameUI.SetActive(false);
    }
}
