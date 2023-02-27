using UnityEngine;
using UnityEngine.UI;
public class HandleLobbyWindow : MonoBehaviour {
	private GameRoot root;
	public Text TxtCoin;
	public Text TxtScoreAndTime;
	public int FightCost;
	public void Init() {
		root = GameRoot.Instance;
		RefreshCoinDate();
		RefreshScoreAndTimeDate();
	}
	#region ClickEvents
	public void OnBtnResetClicked() {
		PlayerPrefs.DeleteAll();
		root.OpenTipsWindow("数据清除完成");
	}
	public void OpenFightWindow() {
		root.PlayClickAudio();
		if(root.GetRecordCoin() >= FightCost) {
			root.UpdateCoinDate(root.GetRecordCoin() - FightCost);
			RefreshCoinDate();
			root.OpenFightWindow();
			root.OpenTipsWindow("开始战斗");
		}
		else {
			root.OpenTipsWindow("金币不足");
		}
	}
	public void OnBtnMenuClicked() {
		root.PlayClickAudio();
		root.OpenMenuWindow(OpType.Help);
	}
	public void OnBtnMatchClicked() {
		root.PlayClickAudio();
		root.OpenTipsWindow("开发中。。。");
	}
	#endregion

	#region ToolFunctions
	public void RefreshCoinDate() {
		TxtCoin.text = root.GetRecordCoin().ToString();
	}
	public void RefreshScoreAndTimeDate() {
		string mTime = root.GetRecordTime();
		if(mTime != "") {
			int mScore = root.GetRecordScore();
			TxtScoreAndTime.text = "最高积分：" + mScore.ToString() + "\n\n记录时间：" + mTime;
			TxtScoreAndTime.alignment = TextAnchor.MiddleLeft;
		}
		else {
			TxtScoreAndTime.text = "无数据";
		}
	}
	#endregion
}