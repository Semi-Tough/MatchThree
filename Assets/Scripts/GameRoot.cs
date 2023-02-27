using System;
using UnityEngine;
public class GameRoot : MonoBehaviour {
	public static GameRoot Instance;
	#region UIDefine
	public HandleLobbyWindow HandleLobby;
	public HandleTipsWindow HandleTips;
	public HandleFightWindow HandleFight;
	public HandleMenuWindow HandleMenu;
	#endregion
	#region DateArea
	private int mScore;
	private int mCoin;
	private string mTime;
	#endregion
	void Start() {
		Instance = this;
		OpenTipsWindow("成功启动游戏");
		InitGameDate();
		OpenLobbyWindow();
	}
	void InitGameDate() {
		if(PlayerPrefs.HasKey("FistLogin")) {
			mCoin = PlayerPrefs.GetInt("mCoin");
			mScore = PlayerPrefs.GetInt("mScore");
			mTime = PlayerPrefs.GetString("mTime");
		}
		else {
			PlayerPrefs.SetString("FistLogin", "Yes");
			mScore = 0;
			mCoin = 8888;
			mTime = "";
			PlayerPrefs.SetInt("mScore", mScore);
			PlayerPrefs.SetInt("mCoin", mCoin);
			PlayerPrefs.SetString("mTime", mTime);
			OpenTipsWindow("首次登录赠送8888金币");
		}
	}
	#region Button
	public void SetFightRun() {
		HandleFight.SetFightRun();
	}
	public void BackLobby() {
		HandleFight.ClearFightDate();
		HandleFight.gameObject.SetActive(false);
		PlayLobbyAudio();
	}
	public void ReStartGame() {
		HandleFight.ClearFightDate();
		HandleLobby.OpenFightWindow();
	}
	#endregion
	#region OpenWindows
	public void OpenLobbyWindow() {
		HandleLobby.gameObject.SetActive(true);
		HandleLobby.Init();
		PlayLobbyAudio();
	}
	public void OpenFightWindow() {
		HandleFight.gameObject.SetActive(true);
		HandleFight.Init();
		PlayFightAudio();
	}
	public void OpenMenuWindow(OpType op) {
		HandleMenu.gameObject.SetActive(true);
		HandleMenu.Init(op);
	}
	public void OpenTipsWindow(string tips) {
		HandleTips.AddTips(tips);
	}
	#endregion
	#region UpDateDate
	private int CurScore;
	private bool isNewRecord;
	public void UpdateCoinDate(int Coin) {
		mCoin = Coin;
		PlayerPrefs.SetInt("mCoin", mCoin);
	}
	public void UpdateScoreDate(int newScore) {
		isNewRecord = false;
		CurScore = newScore;
		if(newScore > mScore) {
			mScore = newScore;
			isNewRecord = true;
			PlayerPrefs.SetInt("mScore", newScore);
			var dt = DateTime.Now;
			string str = dt.Year + "-" + dt.Month + "-" + dt.Day + " " + dt.ToLongTimeString();
			mTime = str;
			PlayerPrefs.SetString("mTime", str);
			HandleLobby.RefreshScoreAndTimeDate();
		}
	}
	#endregion
	#region ToolFunctions
	public int GetRecordCoin() {
		return mCoin;
	}
	public int GetRecordScore() {
		return mScore;
	}
	public int GetCurScore() {
		return CurScore;
	}
	public bool GetIsNewRecord() {
		return isNewRecord;
	}
	public string GetRecordTime() {
		return mTime;
	}
	#endregion
	#region Audio
	public AudioSource BgAudio;
	public AudioSource ClickAudio;
	public AudioSource EffectAudio;

	public AudioClip BgFight;
	public AudioClip BgLobby;
	public AudioClip Bomb;
	public AudioClip Click;
	public AudioClip Lighting;
	public AudioClip Wave;
	public AudioClip[] ClearAudio;

	public void PlayLobbyAudio() {
		if(BgAudio.clip == null || BgAudio.clip == BgFight) {
			BgAudio.clip = BgLobby;
			BgAudio.Play();
		}
	}
	public void PlayFightAudio() {
		if(BgAudio.clip == null || BgAudio.clip == BgLobby) {
			BgAudio.clip = BgFight;
			BgAudio.Play();
		}
	}
	public void PlayClickAudio() {
		ClickAudio.Play();
	}
	public void PlayEffectAudio(string name) {
		for(int i = 0; i < ClearAudio.Length; i++) {
			AudioClip clip = ClearAudio[i];
			if(clip.name == name) {
				EffectAudio.clip = clip;
				EffectAudio.Play();
				return;
			}
		}

		if(Bomb.name == name) {
			EffectAudio.clip = Bomb;
		}
		if(Lighting.name == name) {
			EffectAudio.clip = Lighting;
		}
		if(Wave.name == name) {
			EffectAudio.clip = Wave;
		}
		EffectAudio.Play();
	}
	#endregion
}