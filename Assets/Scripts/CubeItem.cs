using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class CubeItem : MonoBehaviour {
	public RectTransform rectTrans;
	public Image IconImage;
	public int XIndex;
	public int YIndex;
	public int IconIndex;
	public bool IsSkillItem;
	public void SetIconImage(int index) {
		IconIndex = index;
		Sprite sp = Resources.Load<Sprite>("ResImages/Cubes/cube_" + index);
		IconImage.sprite = sp;
	}

	#region SkillTip
	public void SetSkillIconImage(int index) {
		IconIndex = index;
		Sprite sp = Resources.Load<Sprite>("ResImages/Cubes/cube_" + index);
		Animator ani = GetComponent<Animator>();
		ani.Play("SkillTip", 0, 0);
		RuntimeAnimatorController run = ani.runtimeAnimatorController;
		var clips = run.animationClips;

		float aniTime = clips[1].length;
		StartCoroutine(DelayGetSkillItem(aniTime, sp));
	}
	IEnumerator DelayGetSkillItem(float time, Sprite sp) {
		yield return new WaitForSeconds(time / 3);
		IconImage.sprite = sp;
	}
	#endregion
	public bool IsSameType(CubeItem item) {
		if(item.IconIndex == IconIndex) return true;
		else {
			return false;
		}
	}
	public bool Equals(CubeItem item) {
		if(item.XIndex == XIndex && item.YIndex == YIndex) {
			return true;
		}
		else {
			return false;
		}
	}
	#region Animator
	private bool isMove;
	private Vector2 targetPos = Vector2.zero;
	private float moveTime;
	private Vector3 moveVelocity = Vector3.zero;
	private float TimeCount;
	public void MoveInTime(float Time, Vector2 from, Vector2 to) {
		moveTime = Time;
		rectTrans.localPosition = from;
		targetPos = to;
		float speedX = (to.x - from.x) / Time;
		float speedY = (to.y - from.y) / Time;
		moveVelocity = new Vector3(speedX, speedY, 0);
		isMove = true;
	}
	private void Update() {
		if(isMove) {
			//运算符"+"对于"Vector3"和"Vector2"类型的操作数具有二义性
			rectTrans.localPosition += moveVelocity * Time.deltaTime;
			TimeCount += Time.deltaTime;
		}
		if(TimeCount > moveTime) {
			rectTrans.localPosition = targetPos;
			isMove = false;
			TimeCount = 0;
		}
	}
	#endregion
}