using UnityEngine;
using UnityEngine.UI;
public enum OpType {
	None,
	Help,
	Pause,
	End
}
public class HandleMenuWindow : MonoBehaviour {
	public Transform HelpRoot;
	public Transform PauseRoot;
	public Transform EndRoot;

	public Image NewRecordImg;
	public Text CurScore;
	public Text HisScore;

	private OpType opType = OpType.None;
	private GameRoot root;
	public void Init(OpType op) {
		root = GameRoot.Instance;
		opType = op;
		switch(opType) {
			case OpType.None:
				break;
			case OpType.Help:
				HelpRoot.gameObject.SetActive(true);
				PauseRoot.gameObject.SetActive(false);
				EndRoot.gameObject.SetActive(false);
				break;
			case OpType.Pause:
				HelpRoot.gameObject.SetActive(false);
				PauseRoot.gameObject.SetActive(true);
				EndRoot.gameObject.SetActive(false);
				break;
			case OpType.End:
				HelpRoot.gameObject.SetActive(false);
				PauseRoot.gameObject.SetActive(false);
				EndRoot.gameObject.SetActive(true);
				SetScoreDate();
				break;
		}
	}
	public void OnBtnContinueClicked() {
		root.PlayClickAudio();
		root.SetFightRun();
		gameObject.SetActive(false);
	}
	public void OnBtnBackLobbyClicked() {
		root.PlayClickAudio();
		root.BackLobby();
		gameObject.SetActive(false);
	}
	public void OnBtnShareClicked() {
		root.PlayClickAudio();
		root.OpenTipsWindow("开发中。。。");
	}
	public void OnBtnAgainClicked() {
		root.PlayClickAudio();
		root.ReStartGame();
		gameObject.SetActive(false);
	}
	private void SetScoreDate() {
		CurScore.text = root.GetCurScore().ToString();
		HisScore.text = root.GetRecordScore().ToString();
		if(root.GetIsNewRecord()) {
			NewRecordImg.gameObject.SetActive(true);
		}
		else {
			NewRecordImg.gameObject.SetActive(false);
		}
	}
}