using UnityEngine;
using System.Collections;

//
// ↑ ↓ 키로 루프 애니메이션을 전환 스크립트 (임의 변경 포함) Ver.3
// 2014/04/03 N.Kobayashi
//

// Require these components when using this script
[RequireComponent (typeof (Animator))]



public class IdleChanger : MonoBehaviour
{

private Animator anim; // Animator에 대한 참조
private AnimatorStateInfo currentState; // 현재 상태 상태를 저장하는 참조
private AnimatorStateInfo previousState; // 하나 전의 상태 상태를 저장하는 참조
public bool _random = false; // 랜덤 판정 스타트 스위치
public float _threshold = 0.5f; // 랜덤 판정의 한계
public float _interval = 2f; // 랜덤 판정의 간격
// private float _seed = 0.0f; // 랜덤 판정 용 씨앗



// Use this for initialization
void Start ()
{
// 각 참조 초기화
anim = GetComponent <Animator> ();
currentState = anim.GetCurrentAnimatorStateInfo (0);
previousState = currentState;
// 랜덤 판정 용 함수를 시작하는
StartCoroutine ( "RandomChange");
}

// Update is called once per frame
void Update ()
{
// ↑ 키 / 공간이 밀리면, 상태를 다음 쓰기 처리
if (Input.GetKeyDown ( "up") || Input.GetButton ( "Jump")) {
// 부울 Next를 true로
anim.SetBool ( "Next", true);
}

// ↓ 키를 누르면되면 상태를 전에 취소 처리
if (Input.GetKeyDown ( "down")) {
// 부울 Back을 true로
anim.SetBool ( "Back", true);
}

// "Next"플래그가 true 때의 처리
if (anim.GetBool ( "Next")) {
// 현재 상태를 확인하고 상태 이름이 다르다면 부울을 false로 다시
currentState = anim.GetCurrentAnimatorStateInfo (0);
if (previousState.nameHash != currentState.nameHash) {
anim.SetBool ( "Next", false);
previousState = currentState;
}
}

// "Back"플래그가 true 때의 처리
if (anim.GetBool ( "Back")) {
// 현재 상태를 확인하고 상태 이름이 다르다면 부울을 false로 다시
currentState = anim.GetCurrentAnimatorStateInfo (0);
if (previousState.nameHash != currentState.nameHash) {
anim.SetBool ( "Back", false);
previousState = currentState;
}
}
}


void OnGUI ()
{
GUI.Box (new Rect (Screen.width - 110, 10, 100, 90), "Change Motion");
if (GUI.Button (new Rect (Screen.width - 100, 40, 80, 20), "Next"))
anim.SetBool ( "Next", true);
if (GUI.Button (new Rect (Screen.width - 100, 70, 80, 20), "Back"))
anim.SetBool ( "Back", true);
}


// 랜덤 판정 용 함수
IEnumerator RandomChange ()
{
// 무한 루프 시작
while (true) {
// 랜덤 판정 스위치 온의 경우
if (_random) {
// 임의의 씨앗을 꺼내 그 크기에 따라 플래그 설정을
float _seed = Random.Range (-1f, 1f);
if (_seed <= -_threshold) {
anim.SetBool ( "Back", true);
} else if (_seed >= _threshold) {
anim.SetBool ( "Next", true);
}
}
// 다음의 판정까지 간격을 둔다
yield return new WaitForSeconds (_interval);
}

}

}