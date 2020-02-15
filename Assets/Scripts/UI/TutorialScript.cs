using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScript : MonoBehaviour
{
    [System.Serializable]
    public struct ScriptInfo
    {
        public Sprite scriptImg;

        [TextArea]
        public string scriptText;
    }
    public ScriptInfo[] scriptInfos;

    public GameObject onlyOk;
    public GameObject onlyNext;
    public GameObject nextAndBefore;
    public GameObject beforeAndOK;

    // scriptText show Text
    public Text tutoText;
    // scriptImg show Image
    public Image tutoImage;

    // 각 글자 별 Delay
    public float delayPerChar;

    private int conScript;
    private int maxScript;

    // master speed
    public float speed = 1;

    // script control variable
    private float conTime = 0;
    private bool doNextScript = false;


    // ================================================================= public function ==================================================================

    public void TutorialStart()
    {
        // 위의 변수들을 바탕으로 스크립팅 시작
        StartCoroutine(TutorialStarting());
    }

    public void OnClickBtn()
    {
        delayPerChar = 0;
        conTime = 20;
    }

    public void OnClickOkBtn()
    {
        doNextScript = true;

        onlyOk.SetActive(false);
    }

    public void OnClickNextBtn()
    {
        conScript++;
        doNextScript = true;

        onlyNext.SetActive(false);
        nextAndBefore.SetActive(false);
        beforeAndOK.SetActive(false);
    }

    public void OnClickBeforeBtn()
    {
        conScript--;
        doNextScript = true;

        onlyNext.SetActive(false);
        nextAndBefore.SetActive(false);
        beforeAndOK.SetActive(false);
    }

    // ================================================================= private function ==================================================================

    // skip을 누르거나, 스크립팅이 끝날 때 까지 대사를 출력
    IEnumerator TutorialStarting()
    {
        yield return new WaitForSeconds(1.5f);

        tutoText.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);

        string tempStr = "안녕하세요!d 방공호 튜토리얼을 시작하겠습니다.";

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
                default:
                    tutoText.text = tutoText.text + tempStr[conChar];
                    break;
            }

            // 다음 글자를 입력할 때까지 대기
            while (conTime < delayPerChar)
            {
                conTime += Time.deltaTime * speed;
                yield return null;
            }
            conTime = 0;
        }

        onlyOk.SetActive(true);

        // 모든 글자를 출력했으므로 화면을 터치할 때까지 대기
        while (!doNextScript)
            yield return null;

        doNextScript = false;

        tutoImage.gameObject.SetActive(true);
        tutoText.text = "";
        tutoText.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -215, 0);
        tutoText.fontSize = 20;

        // Line Control variable
        conScript = 0;
        maxScript = scriptInfos.Length;

        // 출력
        while (conScript < maxScript)
        {
            // 이전 script의 정보를 초기화
            tutoText.text = "";
            delayPerChar = 0.1f;
            conTime = 0;

            // 새로운 이미지를 표시
            tutoImage.sprite = scriptInfos[conScript].scriptImg;

            bool isColor = false;

            // 해당 라인의 글자들을 일정 간격(delayPerChar)으로 입력
            for (int conChar = 0; conChar < scriptInfos[conScript].scriptText.Length; conChar++)
            {
                // 현재 string의 입력값이 코드일 경우 따로 처리. 그 외엔 UI에 표시
                switch (scriptInfos[conScript].scriptText[conChar])
                {
                    case 't':
                        tutoText.text = tutoText.text += '\n';
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
                        if (scriptInfos[conScript].scriptText[conChar] == '%')
                        {
                            isColor = true;
                            continue;
                        }

                        if (isColor)
                            tutoText.text = tutoText.text + "<color=#568AFFFF>" + scriptInfos[conScript].scriptText[conChar] + "</color>";
                        else
                            tutoText.text = tutoText.text + scriptInfos[conScript].scriptText[conChar];
                        isColor = false;
                        break;
                }

                // 다음 글자를 입력할 때까지 대기
                while (conTime < delayPerChar)
                {
                    conTime += Time.deltaTime * speed;
                    yield return null;
                }
                conTime = 0;
            }

            if (conScript == 0)
                onlyNext.SetActive(true);
            else if (conScript == maxScript - 1)
                beforeAndOK.SetActive(true);
            else
                nextAndBefore.SetActive(true);

            // 모든 글자를 출력했으므로 화면을 터치할 때까지 대기
            while (!doNextScript)
                yield return null;

            // 이제부터 화면을 터치 시 Skip or 다음 script 출력
            doNextScript = false;
        }

        gameObject.SetActive(false);
    }
}
