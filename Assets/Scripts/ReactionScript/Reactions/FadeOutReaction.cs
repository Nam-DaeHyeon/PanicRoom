using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutReaction : DelayedReaction
{
    // fadeOut 하는 객체
    public GameObject fadeOutObject;

    public float fadeOutSpeed;

    // 코루틴 사용을 도와줄 객체
    private GameObject coroutineHelper;

    protected override void SpecificInit()
    {

    }

    protected override void ImmediateReaction()
    {
        // 페이드아웃 코루틴 시작
        coroutineHelper = CoroutineHelper.StartNewCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        // 페이드아웃이 끝날때까지 다음 리액션이 실행되는것을 방지
        FSLocator.textDisplayer.reactionButton.enabled = false;

        fadeOutObject.SetActive(true);

        // UI - InGame 어느 타입의 객체를 페이드아웃 하는지 식별
        bool isCanvas = false;
        if (fadeOutObject.GetComponentInChildren<Image>() != null)
            isCanvas = true;
        


        // Image의 알파값을 조절
        if(isCanvas)
        {
            Image tempImage = fadeOutObject.GetComponentInChildren<Image>();
            float conAlpha = 1.0f;
            while (conAlpha > 0)
            {
                tempImage.color = new Color(tempImage.color.r, tempImage.color.g, tempImage.color.b, conAlpha);
                conAlpha -= Time.deltaTime * fadeOutSpeed;
                yield return null;
            }

            conAlpha = 0;
            tempImage.color = new Color(tempImage.color.r, tempImage.color.g, tempImage.color.b, conAlpha);
        }
        // 스프라이트의 알파값을 조절
        else
        {
            SpriteRenderer tempSprite = fadeOutObject.GetComponentInChildren<SpriteRenderer>();
            float conAlpha = 1.0f;
            while (conAlpha > 0)
            {
                tempSprite.color = new Color(tempSprite.color.r, tempSprite.color.g, tempSprite.color.b, conAlpha);
                conAlpha -= Time.deltaTime * fadeOutSpeed;
                yield return null;
            }

            conAlpha = 0;
            tempSprite.color = new Color(tempSprite.color.r, tempSprite.color.g, tempSprite.color.b, conAlpha);
        }

        fadeOutObject.SetActive(false);

        // 다음 이벤트 내용이 담긴 코루틴을 실행할 수 있도록 한다.
        FSLocator.textDisplayer.reactionButton.enabled = true;
        FSLocator.textDisplayer.reactionButton.onClick.Invoke();

        // 코루틴 사용 종료
        Destroy(coroutineHelper);

        yield return null;
    }
}
