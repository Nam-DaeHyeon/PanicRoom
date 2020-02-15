using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DEBUFF
{
    NONE,
    STUN,
    MUTE,
    HORROR,
    BLOOD,
    BURN,
    CONFUSE
}

public class Unit : MonoBehaviour
{
    public class DebuffSet
    {
        public DEBUFF _debuffState;     //현재 행동 상태
        public int _debuffPower;        //디버프 세기
        public int _debuffLeftTurn;     //디버프 지속 시간

        public DebuffSet(DEBUFF debuff, int power, int turn)
        {
            _debuffState = debuff;
            _debuffPower = power;
            _debuffLeftTurn = turn;
        }
    }

    //필수 파라미터
    [SerializeField] protected string _nickName;
    [SerializeField] protected float _currHp;   //현재체력
    [SerializeField] protected float _maxHp;    //최대체력
    [SerializeField] protected List<DebuffSet> _debuffSets = new List<DebuffSet>();
    protected float _defaultHp = 10;            //기본체력
    [SerializeField] protected float _speed;    //이동속도
    
    public Item privateItem;

    //1차 스테이터스
    [SerializeField] protected int ab_str;  //힘
    [SerializeField] protected int ab_dex;  //민첩
    [SerializeField] protected int ab_con;  //인내
    [SerializeField] protected int ab_res;  //저항

    //2차 스테이터스
    protected float _attack;           //공격력
    protected float _criticalRate;     //치명타율
    protected float _criticalDamage;   //치명타 데미지 계수
    protected float _EndDamage;        //최종 피해량
    protected float _defence;          //방어력
    protected float _defenceRate;      //방어율
    protected float _avoidRate;        //회피율
    protected float _debuffRes;    //상태이상저항
    protected int _orderFactor;     //공격순서팩터

    public bool IsArrived;  //모드 전환 시 위치도달여부

    public bool IsInfectedPlayer;   //감염된 플레이어 여부.

    protected bool _canLongDistnace = false;     //원거리 공격 가능 여부

    public Outline _outline;

    protected Vector3 originPos;

    public virtual void Set_DetailStatus()
    {
        _maxHp = ab_str + ab_con * 2 + _defaultHp;
        _attack = ab_str;
        _criticalRate = ab_dex;
        _criticalDamage = 1.5f;
        _defence = ab_con;
        _defenceRate = 20 / (_defence + 20);
        _avoidRate = ab_dex;
        _debuffRes = ab_con + ab_res;

        _orderFactor = 100 + ab_dex;
    }

    public void Set_OriginPos()
    {
        originPos = transform.position;
    }

    public void Set_OriginPos(Vector3 pos)
    {
        originPos = pos;
    }

    public void Check_Located_Unit(Vector3 location)
    {
        if (IsArrived) return;

        Vector2 dir = location - transform.position;
        dir = dir.normalized;
        transform.Translate(dir * 10f * Time.deltaTime);

        float _locationDistance = Vector2.Distance(location, transform.position);
        if (_locationDistance < 0.1f)
        {
            IsArrived = true;
        }
        else IsArrived = false;

        return;
    }

    public virtual void Order()
    {
        //사망할 경우 명령 무시.
        if (_currHp <= 0) return;
        
        //선행 상태이상 처리
        for(int i = 0; i < _debuffSets.Count; i++)
        {
            switch (_debuffSets[i]._debuffState)
            {
                case DEBUFF.HORROR:
                    StageManager.instance.Get_HitLog(_debuffSets[i]._debuffState.ToString(), Get_HUDPosition(), "RED");
                    StageManager.instance.TurnEnd = true;
                    if (_outline != null) _outline.ShowHide_Outline(false);
                    Set_Down_DebuffTurn();
                    break;
                case DEBUFF.CONFUSE:
                    if (_debuffSets[i]._debuffPower >= Random.Range(0, 100))
                    {
                        StageManager.instance.Get_HitLog(_debuffSets[i]._debuffState.ToString(), Get_HUDPosition(), "RED");
                        StageManager.instance.TurnEnd = true;
                        if (_outline != null) _outline.ShowHide_Outline(false);
                        Set_Down_DebuffTurn();
                    }
                    break;
            }
        }
    }

    public void Reset_HP()
    {
        _currHp = _maxHp;
    }

    public void Take_HP_Heal(int amount)
    {
        StageManager.instance.Get_HitLog(amount.ToString(), Get_HUDPosition(), "GREEN");
        if (_currHp <= 0) return;
        _currHp += amount;
        if (_currHp > _maxHp) _currHp = _maxHp; 
    }

    public virtual void TakeDamage(float damage)
    {
        _EndDamage = damage * _defenceRate;
        _EndDamage = Mathf.Round(_EndDamage * 10) * 0.1f;

        _currHp -= _EndDamage;
    }

    public virtual void TakeDebuffDamage(float damage)
    {
        _EndDamage = damage;
        _EndDamage = Mathf.Round(_EndDamage * 10) * 0.1f;

        _currHp -= _EndDamage;
    }

    public void TakeDebuff(DEBUFF debuff, int debuffPower, int debuffTurn)
    {
        //상태이상(디버프) 적용
        if (_debuffRes <= Random.Range(0, 100))
        {
            switch (debuff)
            {
                case DEBUFF.HORROR:
                case DEBUFF.BLOOD:
                case DEBUFF.BURN:
                case DEBUFF.CONFUSE:
                    Set_UP_DebuffTurn(new DebuffSet(debuff, debuffPower, debuffTurn));
                    break;
            }
        }
    }

    public void Remove_Debuff(DEBUFF targetDebuff)
    {
        for(int i = 0; i < _debuffSets.Count; i++)
        {
            if(_debuffSets[i]._debuffState.Equals(targetDebuff))
            {
                _debuffSets[i]._debuffLeftTurn = 0;
                _debuffSets.RemoveAt(i);
                return;
            }
        }
    }

    protected void Set_UP_DebuffTurn(DebuffSet debuffSet)
    {
        //Debug.Log("디버프가 걸렸습니다. : " + debuffSet._debuffState.ToString());
        StageManager.instance.Get_HitLog("상태이상 - " + debuffSet._debuffState.ToString(), Get_HUDPosition(), "RED");

        //기존 디버프 탐색.
        for(int i = 0; i < _debuffSets.Count; i++)
        {
            //기존 디버프에 있을 경우, 지속 시간 초기화.
            if(_debuffSets[i]._debuffState.Equals(debuffSet._debuffState))
            {
                _debuffSets[i]._debuffLeftTurn = debuffSet._debuffLeftTurn;
                return;
            }
        }

        //기존 디버프에 없을 경우.
        _debuffSets.Add(debuffSet);
    }

    protected void Set_Down_DebuffTurn()
    {
        List<DebuffSet> removeSet = new List<DebuffSet>();
        
        //후행 상태이상(디버프) 처리
        for (int i = 0; i < _debuffSets.Count; i++)
        {
            //데미지 관련 상태이상(디버프)
            switch (_debuffSets[i]._debuffState)
            {
                case DEBUFF.BLOOD:
                    StageManager.instance.Get_HitLog(_debuffSets[i]._debuffState.ToString() + " " + _debuffSets[i]._debuffPower, Get_HUDPosition(), "RED");
                    break;
                case DEBUFF.BURN:
                    StageManager.instance.Get_HitLog(_debuffSets[i]._debuffState.ToString() + " " + _debuffSets[i]._debuffPower, Get_HUDPosition(), "RED");
                    break;
            }

            //TakeDamage(_debuffSets[i]._debuffPower);
            TakeDebuffDamage(_debuffSets[i]._debuffPower);

            //턴 감소 처리.
            _debuffSets[i]._debuffLeftTurn--;
            //삭제될 디버프 주소 저장.
            if (_debuffSets[i]._debuffLeftTurn <= 0)
            {
                removeSet.Add(_debuffSets[i]);
            }

            if (this as Player)
            {
                if (!StageManager.instance.Check_AlivePlayer())
                {
                    StageManager.instance.GameSet_GAMEOVER();
                }
            }
            else if(this as Monster)
            {
                if(!StageManager.instance.Check_AliveMonster())
                {
                    StageManager.instance.GameSet_IDLE();
                }
            }
        }

        //디버프 삭제
        for (int i = 0; i < removeSet.Count; i++)
        {
            if (_debuffSets.Contains(removeSet[i]))
            {
                //Debug.Log("디버프를 제거합니다. : " + removeSet[i]._debuffState.ToString());
                _debuffSets.Remove(removeSet[i]);
            }
        }
    }

    public void Set_InfectedData(InfectedPlayer infectedPlayer)
    {
        _nickName = infectedPlayer.characterName;
        ab_str = infectedPlayer.str;
        ab_dex = infectedPlayer.dex;
        ab_con = infectedPlayer.con;
        ab_res = infectedPlayer.res;

        //privateItem = GameManager.instance.Load_Item_DB(infectedPlayer.itemId);
        if(infectedPlayer.item == null)
        {
            infectedPlayer.item = GameManager.instance.Load_Item_DB(infectedPlayer.tempItemId);
        }
        privateItem = infectedPlayer.item;

        Set_DetailStatus();

        //스킬 지정.

        IsInfectedPlayer = true;
    }

    public void Set_Random_InitParameters(int _defaultStat, int _availableStat)
    {
        int availableStat = _availableStat;
        int bonus = 0;

        //str
        bonus = Random.Range(0, availableStat);
        availableStat -= bonus;
        ab_str = _defaultStat + bonus;

        //dex
        bonus = Random.Range(0, availableStat);
        availableStat -= bonus;
        ab_dex = _defaultStat + bonus;

        //con
        bonus = Random.Range(0, availableStat);
        availableStat -= bonus;
        ab_con = _defaultStat + bonus;

        //res
        ab_res = _defaultStat + availableStat;
    }

    public string Get_NickName() { return _nickName; }
    public float Get_currentHP() { return _currHp; }
    public float Get_maxHP() { return _maxHp; }
    public float Get_AttackPower() { return _attack; }
    public float Get_DefencePower() { return _defence; }
    public float Get_CriticalRate() { return _criticalRate; }
    public int Get_OrderFactor() { return _orderFactor; }
    public int Get_TotalPoint() { return ab_str + ab_con + ab_dex + ab_res; }

    public float Get_MoveSpeed() { return _speed; }

    public virtual Vector3 Get_HUDPosition() { return Vector3.zero; }
}
