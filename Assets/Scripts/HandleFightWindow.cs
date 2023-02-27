using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class HandleFightWindow : MonoBehaviour {
	public GameRoot root;
	public GameObject Go;
	public Transform CubeRooTransform;
	public Vector2 XYSpace;
	public float WaveTime;
	public float MoveTime;
	public Transform EffectRoot;
	public GameObject Clean;
	#region CountDownTime
	public Text CountdownTxt;
	public Image BottomBarFillImg;
	public int CountDownTime;
	private void Update() {
		if(isRun) {
			deltaCount += Time.deltaTime;
			if(deltaCount > 1) {
				deltaCount -= 1;
				mCount -= 1;
			}
			if(mCount <= 0) {
				mCount = 0;
				isRun = false;
				GameOver();
			}
			SetBottomBarValue(mCount);
		}
	}
	private void SetBottomBarValue(int time) {
		float val = time * 1.0f / CountDownTime;
		BottomBarFillImg.fillAmount = val;
		CountdownTxt.text = mCount + "S";
	}
	#endregion
	#region RefreshScore
	public int CubeScore;
	public Transform NumRootTrans;
	private Animator numAni;
	//private void SetScoreNum(int count)
	//{
	//    //ScoreTxt.text = mScore.ToString();
	//}
	private void SetScoreNum(int count, bool isJump = true) {
		mScore += count * CubeScore;
		if(isJump) {
			numAni.Play("ScoreNumAni", 0, 0);
			RuntimeAnimatorController runtime = numAni.runtimeAnimatorController;
			var clips = runtime.animationClips;
			float time = clips[0].length;
			StartCoroutine(ChangeNumImg(time / 3));
		}
		else {
			SetPicNum();
		}
	}
	private void SetPicNum() {
		Image[] image = new Image[5];
		for(int i = 0; i < NumRootTrans.childCount; i++) {
			Transform trans = NumRootTrans.GetChild(i);
			image[i] = trans.GetComponent<Image>();
		}

		string NumStr = mScore.ToString();
		int len = NumStr.Length;
		string[] numArr = new string[len];
		for(int i = 0; i < NumStr.Length; i++) {
			numArr[i] = NumStr.Substring(i, 1);
		}

		for(int i = 0; i < image.Length; i++) {
			if(i < NumStr.Length) {
				image[i].gameObject.SetActive(true);
				image[i].sprite = Resources.Load<Sprite>("ResImages/Fight/num_" + numArr[i]);
			}
			else {
				image[i].gameObject.SetActive(false);
			}
		}
	}
	IEnumerator ChangeNumImg(float time) {
		yield return new WaitForSeconds(time);
		SetPicNum();
	}
	#endregion
	#region Skill
	public int SkillPoit;
	public Image SkillBarImg;
	public Image SkillPointImg;
	public GameObject BombEffect;
	public GameObject LightingHEffect;
	public GameObject LightingVEffect;
	private void SetSkillPoint(int point) {
		float val = point * 1.0f / SkillPoit;
		SkillBarImg.fillAmount = val;
		float posX = 65 + (440 - 65) * val;
		SkillPointImg.rectTransform.localPosition = new Vector2(posX, SkillPointImg.rectTransform.localPosition.y);
	}
	#endregion
	#region DateArea
	private List<CubeItem> destroyList = new List<CubeItem>();
	private CubeItem[,] itemArr = new CubeItem[6, 6];
	private int SkillItemCount;
	private int mSkill;
	private int mScore;
	private int mCount;
	private float deltaCount;
	private bool canClick = true;
	private bool isRun;
	#endregion
	public void Init() {
		numAni = NumRootTrans.GetComponent<Animator>();
		root = GameRoot.Instance;
		InitCubeDate();
		isRun = true;
		mCount = CountDownTime;
		mScore = 0;
		SetScoreNum(mScore, false);

		mSkill = 0;
		SetSkillPoint(mSkill);
	}
	private void InitCubeDate() {
		int[,] indexArr;
		while(true) {
			indexArr = GetRandomArr(6, 6);
			if(IsValidValue(indexArr)) {
				break;
			}
		}
		for(int i = 0; i < 6; i++) {
			for(int j = 0; j < 6; j++) {
				GameObject cube = Instantiate(Go);
				RectTransform rect = cube.GetComponent<RectTransform>();
				CubeItem item = cube.GetComponent<CubeItem>();
				rect.SetParent(CubeRooTransform);
				item.name = "image_" + i + "_" + j;
				item.XIndex = i;
				item.YIndex = j;

				rect.localPosition = new Vector2(i * XYSpace.x, j * XYSpace.y);

				item.SetIconImage(indexArr[i, j]);
				itemArr[i, j] = item;

				Button btn = item.GetComponent<Button>();
				//btn.onClick.AddListener(item.OnItemClicked);
				btn.onClick.AddListener(() => OnItemClicked(item));
			}
		}
	}
	private int[,] GetRandomArr(int width, int height) {
		int[,] indexArr = new int[width, height];
		for(int i = 0; i < width; i++) {
			for(int j = 0; j < height; j++) {
				indexArr[i, j] = Random.Range(0, 5);
			}
		}
		return indexArr;
	}
	private bool IsValidValue(int[,] IndexArr) {
		for(int i = 0; i < 6; i++) {
			for(int j = 0; j < 6; j++) {
				int count = 0;
				int val = IndexArr[i, j];
				if(i <= 4 && val == IndexArr[i + 1, j]) count++;
				if(i >= 1 && val == IndexArr[i - 1, j]) count++;
				if(j >= 1 && val == IndexArr[i, j - 1]) count++;
				if(j <= 4 && val == IndexArr[i, j + 1]) count++;

				if(count >= 2) return true;
			}
		}
		return false;
	}
	public void OnItemClicked(CubeItem clickedItem) {
		if(!canClick || !isRun) {
			return;
		}
		root.PlayClickAudio();
		bool canDestroy = FindDestroyItems(clickedItem);
		if(canDestroy) {
			canClick = false;
			int[] createArr = new int[6];
			SetScoreNum(destroyList.Count);
			for(int i = 0; i < destroyList.Count; i++) {
				CubeItem ci = destroyList[i];
				itemArr[ci.XIndex, ci.YIndex] = null;
				createArr[ci.XIndex] += 1;
				Destroy(ci.gameObject);
			}
			MoveResetCubeItem();
			CreateNewCubeItem(createArr);

			if(mSkill < SkillPoit) {
				mSkill++;
			}
			if(mSkill >= SkillPoit) {
				mSkill = 0;
				SetRandomSkill();
			}
			SetSkillPoint(mSkill);

			SkillItemCount = 0;
			GetSkillItemCount();
			int[,] IconIndexArr = new int[6, 6];
			foreach(var item in itemArr) {
				IconIndexArr[item.XIndex, item.YIndex] = item.IconIndex;
			}
			if(!IsValidValue(IconIndexArr) && SkillItemCount <= 0) {
				isRun = false;
				while(true) {
					IconIndexArr = GetRandomArr(6, 6);
					if(IsValidValue(IconIndexArr)) {
						StartCoroutine(PlayWaveAni(IconIndexArr));
						break;
					}
				}
			}
		}

		if(!canDestroy) {
			mSkill = 0;
			SetSkillPoint(mSkill);
		}
	}
	private void GetSkillItemCount() {
		foreach(var item in itemArr) {
			if(item.IsSkillItem) {
				SkillItemCount++;
			}
		}
	}
	private void SetRandomSkill() {
		int indexX = Random.Range(0, 6);
		int indexY = Random.Range(0, 6);
		int iconIndex = Random.Range(5, 7);
		itemArr[indexX, indexY].SetSkillIconImage(iconIndex);
		itemArr[indexX, indexY].IsSkillItem = true;
	}
	IEnumerator PlayWaveAni(int[,] IconIndexArr) {
		yield return new WaitForSeconds(MoveTime + 0.2f);
		root.PlayEffectAudio("wave");
		for(int x = 0; x < 6; x++) {
			yield return new WaitForSeconds(WaveTime);
			for(int y = 0; y < 6; y++) {
				CubeItem item = itemArr[x, y];
				Animator ani = item.GetComponent<Animator>();
				ani.Play("RefreshCubeAni");
				RuntimeAnimatorController aniC = ani.runtimeAnimatorController;
				var clips = aniC.animationClips;
				float aniTime = clips[0].length;
				StartCoroutine(DelayIconSet(item, IconIndexArr, aniTime / 2));
			}
		}
	}
	IEnumerator DelayIconSet(CubeItem item, int[,] IconIndexArr, float delay) {
		yield return new WaitForSeconds(delay);
		item.SetIconImage(IconIndexArr[item.XIndex, item.YIndex]);
		yield return new WaitForSeconds(delay);
		isRun = true;
	}
	private void CreateNewCubeItem(int[] createArr) {
		for(int i = 0; i < createArr.Length; i++) {
			int count = createArr[i];
			for(int j = 1; j <= count; j++) {
				GameObject cube = Instantiate(Go);
				RectTransform rectTrans = cube.GetComponent<RectTransform>();
				rectTrans.SetParent(CubeRooTransform);
				CubeItem item = cube.GetComponent<CubeItem>();
				item.XIndex = i;
				int y = 5 - count + j;
				item.YIndex = y;
				item.name = "image_" + i + "_" + y;
				//rectTrans.localPosition = new Vector2(i * XYSpace.x,  y * XYSpace.y);

				Vector2 from = new Vector2(i * XYSpace.x, (5 + j) * XYSpace.y);
				Vector2 to = new Vector2(i * XYSpace.x, y * XYSpace.y);
				item.MoveInTime(MoveTime, from, to);
				int IconIndex = Random.Range(0, 5);
				item.SetIconImage(IconIndex);
				itemArr[i, y] = item;

				Button btn = cube.GetComponent<Button>();
				btn.onClick.AddListener((() => OnItemClicked(item)));
			}
		}
		StartCoroutine(CanClick());
	}
	private IEnumerator CanClick() {
		yield return new WaitForSeconds(MoveTime);
		canClick = true;
	}
	private void MoveResetCubeItem() {
		int[,] OffsetArr = new int[6, 6];
		for(int i = 0; i < destroyList.Count; i++) {
			CubeItem item = destroyList[i];
			for(int y = 0; y < 6; y++) {
				if(y > item.YIndex) {
					OffsetArr[item.XIndex, y] += 1;
				}
			}
		}

		for(int x = 0; x < 6; x++) {
			for(int y = 0; y < 6; y++) {
				CubeItem item = itemArr[x, y];
				if(item != null && OffsetArr[x, y] > 0) {
					RectTransform rectTrans = item.GetComponent<RectTransform>();
					Vector2 pos = rectTrans.localPosition;
					float offsetY = pos.y - OffsetArr[x, y] * XYSpace.y;
					//rectTrans.localPosition=new Vector2(pos.x ,offsetY );

					item.MoveInTime(MoveTime, pos, new Vector2(pos.x, offsetY));

					item.YIndex -= OffsetArr[x, y];
					itemArr[x, y] = null;
					itemArr[item.XIndex, item.YIndex] = item;
				}
			}
		}
	}
	private bool FindDestroyItems(CubeItem rootItem) {
		destroyList.Clear();
		destroyList.Add(rootItem);
		if(rootItem.IconIndex == 5 || rootItem.IconIndex == 6) {
			DestroyBySkill(rootItem);
			return true;
		}

		List<CubeItem> rootList = new List<CubeItem>{
			rootItem
		};
		while(rootList.Count > 0) {
			List<CubeItem> findList = new List<CubeItem>();
			for(int i = 0; i < rootList.Count; i++) {
				CubeItem item = rootList[i];
				if(item.XIndex <= 4) {
					CubeItem findItem = itemArr[item.XIndex + 1, item.YIndex];
					if(item.IsSameType(findItem) && !IsSelected(findItem)) {
						destroyList.Add(findItem);
						findList.Add(findItem);
					}
				}

				if(item.XIndex >= 1) {
					CubeItem findItem = itemArr[item.XIndex - 1, item.YIndex];
					if(item.IsSameType(findItem) && !IsSelected(findItem)) {
						destroyList.Add(findItem);
						findList.Add(findItem);
					}
				}

				if(item.YIndex <= 4) {
					CubeItem findItem = itemArr[item.XIndex, item.YIndex + 1];
					if(item.IsSameType(findItem) && !IsSelected(findItem)) {
						destroyList.Add(findItem);
						findList.Add(findItem);
					}
				}

				if(item.YIndex >= 1) {
					CubeItem findItem = itemArr[item.XIndex, item.YIndex - 1];
					if(item.IsSameType(findItem) && !IsSelected(findItem)) {
						destroyList.Add(findItem);
						findList.Add(findItem);
					}
				}
			}
			rootList = findList;
		}

		if(destroyList.Count >= 3) {
			int ran = Random.Range(1, 9);
			root.PlayEffectAudio("s_" + ran);

			for(int i = 0; i < destroyList.Count; i++) {
				CubeItem ci = destroyList[i];
				Vector2 pos = ci.GetComponent<RectTransform>().localPosition;
				GameObject clean = Instantiate(Clean, EffectRoot, true);
				clean.transform.localPosition = pos;
			}

			return true;
		}
		else {
			return false;
		}
	}
	private void DestroyBySkill(CubeItem item) {
		int x = item.XIndex;
		int y = item.YIndex;
		switch(item.IconIndex) {
			case 5:
				if(IsLegal(x, y + 1)) {
					destroyList.Add(itemArr[x, y + 1]);
				}
				if(IsLegal(x, y - 1)) {
					destroyList.Add(itemArr[x, y - 1]);
				}

				if(IsLegal(x - 1, y)) {
					destroyList.Add(itemArr[x - 1, y]);
				}
				if(IsLegal(x - 1, y - 1)) {
					destroyList.Add(itemArr[x - 1, y - 1]);
				}
				if(IsLegal(x - 1, y + 1)) {
					destroyList.Add(itemArr[x - 1, y + 1]);
				}

				if(IsLegal(x + 1, y)) {
					destroyList.Add(itemArr[x + 1, y]);
				}
				if(IsLegal(x + 1, y + 1)) {
					destroyList.Add(itemArr[x + 1, y + 1]);
				}
				if(IsLegal(x + 1, y - 1)) {
					destroyList.Add(itemArr[x + 1, y - 1]);
				}

				for(int i = 0; i < destroyList.Count; i++) {
					CubeItem ci = destroyList[i];
					Vector2 pos = ci.GetComponent<RectTransform>().localPosition;
					GameObject bomb = Instantiate(BombEffect, EffectRoot, true);
					bomb.GetComponent<RectTransform>().localPosition = pos;
				}
				root.PlayEffectAudio("bomb");
				break;
			case 6:
				for(int i = 0; i < 6; i++) {
					if(i != x) {
						destroyList.Add(itemArr[i, y]);
					}
					if(i != y) {
						destroyList.Add(itemArr[x, i]);
					}
				}
				GameObject lightingV = Instantiate(LightingVEffect, EffectRoot, true);
				lightingV.GetComponent<RectTransform>().localPosition = new Vector2(x * XYSpace.x, 315);


				GameObject lightingH = Instantiate(LightingHEffect, EffectRoot, true);
				lightingH.GetComponent<RectTransform>().localPosition = new Vector2(315, y * XYSpace.y);
				root.PlayEffectAudio("lighting");
				break;
		}
	}
	private bool IsLegal(int x, int y) {
		if(x < 0 || x > 5 || y < 0 || y > 5) {
			return false;
		}
		return true;
	}
	public bool IsSelected(CubeItem item) {
		for(int i = 0; i < destroyList.Count; i++) {
			if(item.Equals(destroyList[i])) {
				return true;
			}
		}
		return false;
	}
	private void GameOver() {
		root.UpdateScoreDate(mScore);
		root.OpenMenuWindow(OpType.End);
	}
	public void OnBtnMenuClicked() {
		root.PlayClickAudio();
		isRun = false;
		root.OpenMenuWindow(OpType.Pause);
	}
	public void SetFightRun() {
		isRun = true;
	}
	public void ClearFightDate() {
		SkillItemCount = 0;
		mSkill = 0;
		mScore = 0;
		mCount = 0;
		deltaCount = 0;
		isRun = false;
		destroyList.Clear();
		for(int i = 0; i < CubeRooTransform.childCount; i++) {
			Destroy(CubeRooTransform.GetChild(i).gameObject);
		}
	}
}