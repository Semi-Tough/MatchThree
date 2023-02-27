using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HandleTipsWindow : MonoBehaviour {
	public Image TipsBG;
	public Text TxtTips;
	public Animator TipsAni;
	private Queue<string> tipsQueue = new Queue<string>();
	private bool isTipsShow;
	public void AddTips(string tips) {
		TipsAni.gameObject.SetActive(true);
		tipsQueue.Enqueue(tips);
	}
	private void SetTips(string tips) {
		int len = tips.Length;
		TipsBG.GetComponent<RectTransform>().sizeDelta = new Vector2(40 * len + 50, 70);
		TxtTips.text = tips;
		TipsAni.enabled = true;

		TipsAni.Play("TipsAni", 0, 0);
		RuntimeAnimatorController rac = TipsAni.runtimeAnimatorController;
		AnimationClip[] clips = rac.animationClips;
		StartCoroutine(AniPlayDone(clips[0].length));
	}
	void Update() {
		if(tipsQueue.Count > 0 && isTipsShow == false) {
			string tips = tipsQueue.Dequeue();
			SetTips(tips);
			isTipsShow = true;
		}
	}
	IEnumerator AniPlayDone(float value) {
		yield return new WaitForSeconds(value);
		isTipsShow = false;
		TipsAni.enabled = false;
	}
}