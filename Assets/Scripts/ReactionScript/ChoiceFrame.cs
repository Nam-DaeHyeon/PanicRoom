using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceFrame : MonoBehaviour {

	public Text firstText;
	public Text secondText;

	public Button firstButton;
	public Button secondButton;

	public void ShowChoiceFrame(string firstText, string secondText, ReactionCollection firstReaction, ReactionCollection secondReaction){
		Debug.Log ("??");
		gameObject.SetActive (true);
		this.firstText.text = firstText;
		this.secondText.text = secondText;

		firstButton.onClick.AddListener(delegate{
			transform.parent.GetComponent<TextDisplayer>().reactionButton.enabled = true;
			firstReaction.InitAndReact();
			HideChoiceFrame();
		});
		secondButton.onClick.AddListener (delegate {
			transform.parent.GetComponent<TextDisplayer>().reactionButton.enabled = true;
			secondReaction.InitAndReact ();
			HideChoiceFrame();
		});

	}

	public void HideChoiceFrame(){

		firstButton.onClick.RemoveAllListeners ();
		secondButton.onClick.RemoveAllListeners ();


		gameObject.SetActive (false);
	}
}
