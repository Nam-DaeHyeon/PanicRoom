using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventMessageScript : MonoBehaviour
{
    public CanvasGroup eventMessageCanvas;
    public Text eventMessageText;
    public Image eventMessageImage;

    public GameObject c1Btn;
    public Text c1BtnText;

    public GameObject c2Btn;
    public Text c2BtnText;
    
    public Text foodText;

    public Sprite noteImage;
    [TextArea]
    public string noteScript;
    public GameObject noteOkBtn;

    // 각 글자 별 Delay
    public float delayPerChar = 0.1f;

    // script control variable
    private float conTime = 0;
    private bool doNextScript = false;

    bool isClickC1;

    // ================================================================= public function ==================================================================

    public void EventMessageStart()
    {
        // 위의 변수들을 바탕으로 스크립팅 시작
        StartCoroutine(EventMessageStarting());
    }

    public void StartNote()
    {
        gameObject.SetActive(true);
        StartCoroutine(NoteStarting());
    }

    public void OnClickBtn()
    {
        delayPerChar = 0;
        conTime = 20;
    }

    public void OnClickNoteOkBtn()
    {
        doNextScript = true;
    }

    public void OnClickC1Btn()
    {
        isClickC1 = true;

        doNextScript = true;
    }

    public void OnClickC2Btn()
    {
        isClickC1 = false;

        doNextScript = true;
    }

    IEnumerator EventMessageStarting()
    {
        c1Btn.SetActive(false);
        c2Btn.SetActive(false);
        noteOkBtn.SetActive(false);

        List<Dictionary<string, object>> eventMessageCSV = CSVReader.Read("CSV/EventMessageCSV");
        List<Dictionary<string, object>> afterEventMessageCSV = CSVReader.Read("CSV/AfterEventMessageCSV");

        // 정보를 초기화
        eventMessageText.text = "";
        delayPerChar = 0.1f;

        eventMessageCanvas.alpha = 1;
        eventMessageImage.sprite = null;
        doNextScript = false;

        // 무작위 이벤트 지정
        int eventIndex = Random.Range(0, eventMessageCSV.Count);

        // 이벤트 이미지 세팅
        eventMessageImage.sprite = Resources.Load<Sprite>("EventMessage/" + eventMessageCSV[eventIndex]["NAME"] as string);
        // 텍스트 지정
        string tempStr = eventMessageCSV[eventIndex]["TEXT"] as string;

        yield return new WaitForSeconds(1.2f);

        // 해당 라인의 글자들을 일정 간격(delayPerChar)으로 입력
        for (int conChar = 0; conChar < tempStr.Length; conChar++)
        {
            // 현재 string의 입력값이 코드일 경우 따로 처리. 그 외엔 UI에 표시
            switch (tempStr[conChar])
            {
                case 'd':
                    float T1 = 0.5f;
                    if (delayPerChar == 0)
                        T1 = 0;

                    for (float temp = 0; temp < T1;)
                    {
                        temp += Time.deltaTime;
                        yield return null;
                    }
                    continue;
                case 'e':
                    eventMessageText.text = eventMessageText.text + '\n';
                    break;
                case '<':
                    eventMessageText.text = eventMessageText.text + ',';
                    break;
                case '^':
                    eventMessageText.text = eventMessageText.text + ',';
                    break;
                default:
                    eventMessageText.text = eventMessageText.text + tempStr[conChar];
                    break;
            }

            // 다음 글자를 입력할 때까지 대기
            while (conTime < delayPerChar)
            {
                conTime += Time.deltaTime;
                yield return null;
            }
            conTime = 0;
        }

        int choiceCount = (int)eventMessageCSV[eventIndex]["COUNT"];
        switch(choiceCount)
        {
            case 1:
                c1BtnText.text = eventMessageCSV[eventIndex]["C1_TEXT"] as string;
                c1Btn.SetActive(true);

                c2Btn.SetActive(false);

                break;
            case 2:
                c1BtnText.text = eventMessageCSV[eventIndex]["C1_TEXT"] as string;
                c1Btn.SetActive(true);

                c2BtnText.text = eventMessageCSV[eventIndex]["C2_TEXT"] as string;
                c2Btn.SetActive(true);

                break;
        }

        // 모든 글자를 출력했으므로 화면을 터치할 때까지 대기
        while (!doNextScript)
            yield return null;

        c1Btn.SetActive(false);
        c2Btn.SetActive(false);

        bool isFinish;
        string effectName;
        if (isClickC1)
        {
            effectName = eventMessageCSV[eventIndex]["C1_EFFECT"] as string;
            isFinish = EventProcessing(eventMessageCSV[eventIndex]["C1_EFFECT"] as string);
        }
        else
        {
            effectName = eventMessageCSV[eventIndex]["C2_EFFECT"] as string;
            isFinish = EventProcessing(eventMessageCSV[eventIndex]["C2_EFFECT"] as string);
        }

        if (isFinish)
        {
            GetComponent<Animator>().SetTrigger("Finish");
            yield return new WaitForSeconds(0.5f);

            gameObject.SetActive(false);
        }
        else
        {
            // 두 번째 대화 진입..
            doNextScript = false;
            eventMessageText.text = "";
            delayPerChar = 0.1f;

            int secondIndex = 0;

            // 텍스트 지정
            for(int i = 0; i < afterEventMessageCSV.Count; i++)
            {
                if(effectName == afterEventMessageCSV[i]["NAME"] as string)
                {
                    secondIndex = i;
                    break;
                }
            }
            tempStr = afterEventMessageCSV[secondIndex]["TEXT"] as string;

            // 해당 라인의 글자들을 일정 간격(delayPerChar)으로 입력
            for (int conChar = 0; conChar < tempStr.Length; conChar++)
            {
                // 현재 string의 입력값이 코드일 경우 따로 처리. 그 외엔 UI에 표시
                switch (tempStr[conChar])
                {
                    case 'd':
                        float T1 = 0.5f;
                        if (delayPerChar == 0)
                            T1 = 0;

                        for (float temp = 0; temp < T1;)
                        {
                            temp += Time.deltaTime;
                            yield return null;
                        }
                        continue;
                    case 'e':
                        eventMessageText.text = eventMessageText.text + '\n';
                        break;
                    case '<':
                        eventMessageText.text = eventMessageText.text + ',';
                        break;
                    case '^':
                        eventMessageText.text = eventMessageText.text + ',';
                        break;
                    default:
                        eventMessageText.text = eventMessageText.text + tempStr[conChar];
                        break;
                }

                // 다음 글자를 입력할 때까지 대기
                while (conTime < delayPerChar)
                {
                    conTime += Time.deltaTime;
                    yield return null;
                }

                conTime = 0;
            }

            c1BtnText.text = afterEventMessageCSV[secondIndex]["C1_TEXT"] as string;
            c1Btn.SetActive(true);

            c2Btn.SetActive(false);

            // 모든 글자를 출력했으므로 화면을 터치할 때까지 대기
            while (!doNextScript)
                yield return null;

            c1Btn.SetActive(false);
            c2Btn.SetActive(false);

            EventProcessing(afterEventMessageCSV[secondIndex]["C1_EFFECT"] as string);

            GetComponent<Animator>().SetTrigger("Finish");
            yield return new WaitForSeconds(0.5f);
            gameObject.SetActive(false);
        }
    }

    IEnumerator NoteStarting()
    {
        c1Btn.SetActive(false);
        c2Btn.SetActive(false);
        noteOkBtn.SetActive(false);

        // 정보를 초기화
        eventMessageText.text = "";
        delayPerChar = 0.1f;

        eventMessageCanvas.alpha = 1;
        eventMessageImage.sprite = noteImage;
        doNextScript = false;

        yield return new WaitForSeconds(1.2f);

        bool isColor = false;
        // 해당 라인의 글자들을 일정 간격(delayPerChar)으로 입력
        for (int conChar = 0; conChar < noteScript.Length; conChar++)
        {
            // 현재 string의 입력값이 코드일 경우 따로 처리. 그 외엔 UI에 표시
            switch (noteScript[conChar])
            {
                case 't':
                    eventMessageText.text = eventMessageText.text += '\n';
                    continue;
                case 'd':
                    float T1 = 0.5f;
                    if (delayPerChar == 0)
                        T1 = 0;

                    for (float temp = 0; temp < T1;)
                    {
                        temp += Time.deltaTime;
                        yield return null;
                    }
                    continue;
                default:
                    // % : 파랑
                    // * : 빨강
                    // 의 색으로 다음 글자를 변경
                    if (noteScript[conChar] == '*')
                    {
                        isColor = true;
                        continue;
                    }

                    if (isColor)
                        eventMessageText.text = eventMessageText.text + "<color=#FF0000FF>" + noteScript[conChar] + "</color>";
                    else
                        eventMessageText.text = eventMessageText.text + noteScript[conChar];
                    isColor = false;
                    break;
            }

            // 다음 글자를 입력할 때까지 대기
            while (conTime < delayPerChar)
            {
                conTime += Time.deltaTime;
                yield return null;
            }

            conTime = 0;
        }

        noteOkBtn.SetActive(true);

        // 모든 글자를 출력했으므로 화면을 터치할 때까지 대기
        while (!doNextScript)
            yield return null;

        GetComponent<Animator>().SetTrigger("Finish");
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);

    }

    bool EventProcessing(string code)
    {

        bool returnBool = false;
        switch(code)
        {
            case "O":
                returnBool = true;
                break;
            case "F":
                returnBool = true;
                GameManager.foodCount += 10;
                foodText.text = GameManager.foodCount.ToString();
                break;
            case "M":
                returnBool = true;
                GameManager.foodCount -= 5;
                foodText.text = GameManager.foodCount.ToString();
                break;
            default:
                returnBool = false;

                break;
        }
        return returnBool;
    }

}
