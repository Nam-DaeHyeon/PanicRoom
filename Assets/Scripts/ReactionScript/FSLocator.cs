using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSLocator
{

    public static TextDisplayer textDisplayer
    {
        get
        {
            if (!m_textDisplayer)
            {
                m_textDisplayer = GameObject.FindObjectOfType<TextDisplayer>();
            }

            return m_textDisplayer;
        }
    }

    private static TextDisplayer m_textDisplayer;

    /*
    private static CharacterDisplayer m_characterDisplayer;


    public static CharacterDisplayer characterDisplayer
    {
        get
        {
            if (!m_characterDisplayer)
            {
                m_characterDisplayer = GameObject.FindObjectOfType<CharacterDisplayer>();
            }
            return m_characterDisplayer;
        }
    }


    private static BackgroundDisplayer m_backgroundDisplayer;


    public static BackgroundDisplayer backgroundDisplayer
    {
        get
        {
            if (!m_backgroundDisplayer)
            {
                m_backgroundDisplayer = GameObject.FindObjectOfType<BackgroundDisplayer>();
            }
            return m_backgroundDisplayer;
        }
    }


    private static FadeDisplayer m_fadeDisplayer;
    public static FadeDisplayer fadeDisplayer
    {
        get
        {
            if (!m_fadeDisplayer)
            {
                m_fadeDisplayer = GameObject.FindObjectOfType<FadeDisplayer>();
            }
            return m_fadeDisplayer;
        }
    }



    private static ChoiceDisplayer m_choiceDisplayer;
    public static ChoiceDisplayer choiceDisplayer
    {
        get
        {
            if(!m_choiceDisplayer)
            {
                m_choiceDisplayer = GameObject.FindObjectOfType<ChoiceDisplayer>();
            }
            return m_choiceDisplayer;
        }
    }

	private static UIContainer m_uiContainer;
	public static UIContainer uiContainer
	{
		get {
			if (!m_uiContainer) {
				m_uiContainer = GameObject.FindObjectOfType<UIContainer> ();
			}
			return m_uiContainer;
		}
	}



    private static Timer m_timer;
    public static Timer timer
    {
        get
        {
            if (!m_timer)
            {
                m_timer = GameObject.FindObjectOfType<Timer>();
            }
            return m_timer;
        }
    }

    private static CutSceneTextDisplayer m_cutSceneTextDisplayer;

    public static CutSceneTextDisplayer cutSceneTextDisplayer
    {
        get
        {
            if (!m_cutSceneTextDisplayer)
            {
                m_cutSceneTextDisplayer = GameObject.FindObjectOfType<CutSceneTextDisplayer>();
            }
            return m_cutSceneTextDisplayer;
        }
    }

	private static ChangeSceneManager m_changeSceneManager;

	public static ChangeSceneManager changeSceneManager
	{
		get {
			if (!m_changeSceneManager) {
				m_changeSceneManager = GameObject.FindObjectOfType<ChangeSceneManager> ();
			}
			return m_changeSceneManager;
		}
	}
    */
}
