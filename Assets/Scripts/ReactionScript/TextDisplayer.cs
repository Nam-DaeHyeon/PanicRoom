using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextDisplayer : MonoBehaviour {


    //대화를 다음 대화로 업데이트하는데 걸리는 시간
    [Range(0.0f, 1.0f)]
    public float timeBetUpdateLetters;


    public Text dialougeText;
    public GameObject dialougeFrameDisplayer;


    //public Text nameText;
    public GameObject nameFrameDisplayer;
	public Text nameText;

	public Button reactionButton;
	public ChoiceFrame choiceFrame;
	//public TripleChoiceFrame tripleChoiceFrame;

	public CanvasGroup mapPannel;
	public Text mapText;


    //바깥에서 현재 타이핑 중인지 컨펌가능
    public bool isTyping
    {
        get; private set;
    }

    private string onTypingDialogue;

    public void ShowDialogueHolder()
    {
        
        dialougeFrameDisplayer.SetActive(true);
        if(nameText.text == "")
            nameFrameDisplayer.SetActive(false);
        else
            nameFrameDisplayer.SetActive(true);
            
    }

    public void HideDialogueHolder()
    {
        dialougeFrameDisplayer.SetActive(false);
        nameFrameDisplayer.SetActive(false);
    }

    //현재 타이핑 중인데, 이것을 바로 스킵하고 전체 문자열을 띄우고 싶을떄.
    //얘를 들면 문자가 나타나는 도중 터치하는 경우.
    public void SkipTypingLetter()
    {

        StopCoroutine("TypeText");
        StopCoroutine("TypeAndAddText");

        isTyping = false;

        dialougeText.text = onTypingDialogue;
    }




    public void Say(string text_, string name_)
    {
        nameText.text = name_;
        onTypingDialogue = text_;
        //HideDialogueHolder();

        if (timeBetUpdateLetters <= 0f)
        {
            dialougeText.text = text_;
        }
        else
        {
            StartCoroutine("TypeText", onTypingDialogue);
            
        }


    }


    //텍스트가 업데이트 되는 간격을 지정하기 위해서 존재
    public IEnumerator TypeText(string texts)
    {
        // 필요하다면 후에 DisplayableDipalyer 그림을 그릴때 일시 정지 루프를 돌릴 수 있겠음

        ShowDialogueHolder();

        isTyping = true;


        dialougeText.text = string.Empty;
        foreach (char letter in texts.ToCharArray())
        {

            dialougeText.text += letter;
            yield return new WaitForSeconds(timeBetUpdateLetters);
        }

        isTyping = false;
    }

    public IEnumerator TypeAndAddText(string texts)
    {

        ShowDialogueHolder();



        isTyping = true;

        foreach (char letter in texts.ToCharArray())
        {

            dialougeText.text += letter;
            yield return new WaitForSeconds(timeBetUpdateLetters);
        }

        isTyping = false;
    }

	public void SetMapText(string text)
	{
		mapText.text = text;
	}

}
