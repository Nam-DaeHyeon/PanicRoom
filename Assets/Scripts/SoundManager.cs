using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] AudioSource _bgmSource;
    [SerializeField] AudioSource _seSource;

    [Header("BGM")]
    [SerializeField] AudioClip _titleBGM;
    [SerializeField] AudioClip _lobbyBGM;
    [SerializeField] AudioClip _mainIdleBGM;
    [SerializeField] AudioClip _mainBattleBGM;

    [Header("Sound Effect")]
    [SerializeField] AudioClip _punchSE;
    [SerializeField] AudioClip _slashSE;
    [SerializeField] AudioClip _voimitSE;
    [SerializeField] AudioClip _bottelCrashSE;
    [SerializeField] AudioClip _batHitSE;
    [SerializeField] AudioClip _gunFireSE;
    [SerializeField] AudioClip _explosionSE;
    [SerializeField] AudioClip _missSE;

    [Header("Sound Effect - Event & Menu")]
    [SerializeField] AudioClip _bookSE;

    public static SoundManager s_instance;
    public static SoundManager instance
    {
        get
        {
            if (!s_instance)
            {
                s_instance = GameObject.FindObjectOfType(typeof(SoundManager)) as SoundManager;
                if (!s_instance)
                {
                    Debug.LogError("GameManager s_instance null");
                    return null;
                }
            }

            return s_instance;
        }
    }

    public void Play_BGM_Title()
    {
        if(_titleBGM == null)
        {
            _bgmSource.Stop();
            return;
        }
        _bgmSource.clip = _titleBGM;
        _bgmSource.Play();
    }

    public void Play_BGM_Lobby()
    {
        if(_lobbyBGM == null)
        {
            _bgmSource.Stop();
            return;
        }
        _bgmSource.clip = _lobbyBGM;
        _bgmSource.Play();
    }

    public void Play_BGM_MainIdle()
    {
        if(_mainIdleBGM == null)
        {
            _bgmSource.Stop();
            return;
        }
        _bgmSource.clip = _mainIdleBGM;
        _bgmSource.Play();
    }

    public void Play_BGM_MainBattle()
    {
        if(_mainBattleBGM == null)
        {
            _bgmSource.Stop();
            return;
        }
        _bgmSource.clip = _mainBattleBGM;
        _bgmSource.Play();
    }

    #region 사운드 이펙트 - 전투관련
    public void Play_WeaponSE(Item privateItem)
    {
        if(privateItem == null)
        {
            Play_SE_Punch();
            return;
        }

        switch(privateItem.Get_ItemName())
        {
            case "조잡한 몽둥이":
            case "네일 배트":
            case "철근 몽둥이":
                Play_SE_BatHit();
                break;
            case "커터칼 나이프":
            case "조잡한 나이프 창":
            case "철근 나이프 창":
                Play_SE_Slash();
                break;
            case "호신용 가스총":
                Play_SE_GunFire();
                break;
            case "날붙이 유리병":
            case "급조한 화염병":
                Play_SE_BottleCrash();
                break;
            case "사제 폭탄":
                Play_SE_Explosion();
                break;

        }
    }

    public void Play_SkillSE(string skillName)
    {
        switch (skillName)
        {
            case "산성비":
                Play_SE_Vomit();
                break;
        }
    }

    private void Play_SE_Punch()
    {
        if (_punchSE == null)
        {
            _seSource.Stop();
            return;
        }
        _seSource.clip = _punchSE;
        _seSource.Play();
    }

    private void Play_SE_Slash()
    {
        if (_slashSE == null)
        {
            _seSource.Stop();
            return;
        }
        _seSource.clip = _slashSE;
        _seSource.Play();
    }

    private void Play_SE_Vomit()
    {
        if (_voimitSE == null)
        {
            _seSource.Stop();
            return;
        }
        _seSource.clip = _voimitSE;
        _seSource.Play();
    }

    private void Play_SE_BottleCrash()
    {
        if (_bottelCrashSE == null)
        {
            _seSource.Stop();
            return;
        }
        _seSource.clip = _bottelCrashSE;
        _seSource.Play();
    }

    private void Play_SE_BatHit()
    {
        if (_batHitSE == null)
        {
            _seSource.Stop();
            return;
        }
        _seSource.clip = _batHitSE;
        _seSource.Play();
    }

    private void Play_SE_GunFire()
    {
        if (_gunFireSE == null)
        {
            _seSource.Stop();
            return;
        }
        _seSource.clip = _gunFireSE;
        _seSource.Play();
    }

    private void Play_SE_Explosion()
    {
        if (_explosionSE == null)
        {
            _seSource.Stop();
            return;
        }
        _seSource.clip = _explosionSE;
        _seSource.Play();
    }
    
    public void Play_SE_Miss()
    {
        if (_missSE == null)
        {
            _seSource.Stop();
            return;
        }
        _seSource.clip = _missSE;
        _seSource.Play();
    }
    #endregion

    #region 사운드이펙트 - 메뉴 및 이벤트관련
    public void Play_SE_OpenBook()
    {
        if (_bookSE == null)
        {
            _seSource.Stop();
            return;
        }
        _seSource.clip = _bookSE;
        _seSource.Play();
    }
    #endregion
}
