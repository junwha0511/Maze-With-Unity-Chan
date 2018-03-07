//
// Mecanim 애니메이션 데이터가 원점에서 이동하지 않는 경우 Rigidbody있는 컨트롤러
// 샘플

// 2014/03/13 N.Kobyasahi
//
using UnityEngine;
using System.Collections;

// 필요한 구성 요소의 나열
[RequireComponent(typeof (Animator))]
[RequireComponent(typeof (CapsuleCollider))]
[RequireComponent(typeof (Rigidbody))]

public class UnityChanControlScriptWithRgidBody : MonoBehaviour
{

	public float animSpeed = 1.5f;				// 애니메이션 재생 속도 설정
	public float lookSmoother = 3.0f;			// 카메라 모션의 부드러운 정도
	public bool useCurves = true;				//Mecanim 커브 조정을 사용하거나 설정하기
												// 이 변수가 true가 아니면 커브는 사용되지 않는다
	public float useCurvesHeight = 0.5f;		// 커브 보정의 유효 높이 (지상을 빠져 쉬운 때는 확대)

	//아래는 캐릭터 컨트롤러 매개 변수
	// 전진 속도
	public float forwardSpeed = 7.0f;
	// 후퇴 속도
	public float backwardSpeed = 2.0f;
	// 회전 속도
	public float rotateSpeed = 2.0f;
	// 점프 파워
	public float jumpPower = 3.0f; 
	// 캡슐 콜라이더
	private CapsuleCollider col;
	private Rigidbody rb;
	// 캡슐콜라이더의 이동량??? 캐릭터컨트롤러（カプセルコライダ）の移動量
	private Vector3 velocity;
	// CapsuleCollider로 설정되어있는 콜라이더의 Heiht, Center의 초기 값을 담을 변수
	private float orgColHight;
	private Vector3 orgVectColCenter;
	
	private Animator anim;							// 캐릭터에 연결된 애니메이터에 대한 참조

	private AnimatorStateInfo currentBaseState;			// base layer에서 사용되는 애니메이터의 현재 상태의 참조

	private GameObject cameraObject;	// 메인 카메라에 대한 참조
		
// 애니메이터 각 상태에 대한 참조****
	static int idleState = Animator.StringToHash("Base Layer.Idle");
	static int locoState = Animator.StringToHash("Base Layer.Locomotion");
	static int jumpState = Animator.StringToHash("Base Layer.Jump");
	static int restState = Animator.StringToHash("Base Layer.Rest");

//초기화
	void Start ()
	{
		// Animator 컴포넌트를 가져옴
		anim = GetComponent<Animator>();
		// CapsuleCollider 컴포넌트를 가져옴 (캡슐형 충돌)
		col = GetComponent<CapsuleCollider>();
		rb = GetComponent<Rigidbody>();
		//메인 카메라를 가져옴
		cameraObject = GameObject.FindWithTag("MainCamera");
		// CapsuleCollider의 Height, Center의 초기 값을 저장한다
		orgColHight = col.height;
		orgVectColCenter = col.center;
}
	
	
// 다음으로는. Rigidbody와 관련된 것을 FixedUpdate에서 처리한다.
	void FixedUpdate ()
	{
		float h = Input.GetAxis("Horizontal");				// 입력 장치의 수평 축을 h로 정의
		float v = Input.GetAxis("Vertical");				// 입력 장치의 수직 축을 v 정의
		anim.SetFloat("Speed", v);							// Animator 측에서 설정하고있는 "Speed"매개 변수에 v를 전달
		anim.SetFloat("Direction", h); 						// Animator 측에서 설정 한 "Direction"매개 변수에 h를 전달
		anim.speed = animSpeed;								// Animator의 모션 재생 속도에 animSpeed을 설정
		currentBaseState = anim.GetCurrentAnimatorStateInfo(0);	//참조용 상태 변수에 Base Layer (0)의 현재 상태를 설정하는 참조 용 상태 변수에 Base Layer (0)의 현재 상태를 설정
		rb.useGravity = true;//점프 중에 중력을 없애지만 중력의 영향을 받도록한다
		
		
		
		// 이하, 캐릭터의 이동 처리
		velocity = new Vector3(0, 0, v);		// 상하 키 입력에서 Z 축 방향의 이동량을 취득
		// 캐릭터의 로컬 공간에서의 방향으로 변환
		velocity = transform.TransformDirection(velocity);
		//다음 v 임계 값은 Mecanim 측의 전환과 함께 조정
		if (v > 0.1) {
			velocity *= forwardSpeed;		// 뒤로
		} else if (v < -0.1) {
			velocity *= backwardSpeed;	// 앞으로
		}
		
		if (Input.GetButtonDown("Jump")) {	// 스페이스 키가 입력됨

			//애니메이션의 상태가 Locomotion 상태에서만 이동할 수 있다.
			if (currentBaseState.nameHash == locoState){
				//상태 전환 중에 있지 않으면 이동할 수 있다.
				if(!anim.IsInTransition(0))
				{
						rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
						anim.SetBool("Jump", true);		// Animator 점프로 전환 플래그 쓰기
				}
			}
		}
		

		// 상하 키 입력으로 캐릭터를 이동시킨다
		transform.localPosition += velocity * Time.fixedDeltaTime;

		// 좌우의 키 입력으로 문자를 Y 축으로 회전시킨다
		transform.Rotate(0, h * rotateSpeed, 0);	
	

		// 이하, Animator의 각 상태에서의 처리
		// Locomotion中
		// 현재베이스 레이어가 locoState 일때
		if (currentBaseState.nameHash == locoState){
			//곡선 콜라이더를 조정 하고 있을 때는 안전을 위해 재설정
			if(useCurves){
				resetCollider();
			}
		}
		// JUMP 중 처리
		// 현재베이스 레이어가 jumpState 일때
		else if(currentBaseState.nameHash == jumpState)
		{
			cameraObject.SendMessage("setCameraPositionJumpView");	// 점프 중에 카메라를 변경
			// 상태가 전환되고 있지 않은 경우
			if(!anim.IsInTransition(0))
			{
				
				// 이하, 커브 조정을하는 경우의 처리
				if(useCurves){
					// 다음 JUMP00 애니메이션에 연결 되어있는 곡선 JumpHeight과 GravityControl
					// JumpHeight : JUMP00에서 점프의 높이 (0-1)
					// GravityControl : 1⇒ 점프 중 (중력 함), 0⇒ 중력 활성화
					float jumpHeight = anim.GetFloat("JumpHeight");
					float gravityControl = anim.GetFloat("GravityControl"); 
					if(gravityControl > 0)
						rb.useGravity = false;	//점프 중 중력의 영향을 제거한다
										
					// 레이 캐스팅 캐릭터의 센터에서 떨어 뜨린다
					Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
					RaycastHit hitInfo = new RaycastHit();
					// 높이가 useCurvesHeight 이상인 경우에만 콜 라이더의 높이와 중심을 JUMP00 애니메이션에 붙어있는 곡선 조정
					if (Physics.Raycast(ray, out hitInfo))
					{
						if (hitInfo.distance > useCurvesHeight)
						{
							col.height = orgColHight - jumpHeight;			// 조정된 콜라이더의 높이
							float adjCenterY = orgVectColCenter.y + jumpHeight;
							col.center = new Vector3(0, adjCenterY, 0);	// 조정 된 콜라이더 중심
						}
						else{
							//임계 값보다 낮은 경우에는 초기 값으로 복원 (만약을 위해)					
							resetCollider();
						}
					}
				}
				// Jump bool 값을 재설정 (루프하지 않도록한다)				
				anim.SetBool("Jump", false);
			}
		}
		//IDLE일 경우의 처리
		// 현재베이스 레이어가 idleState 때
		else if (currentBaseState.nameHash == idleState)
		{
			//곡선 콜라이더 조정을 하고 있을 때는 안전을 위해 재설정
			if(useCurves){
				resetCollider();
			}
			// 스페이스 키를 입력하면 Rest 상태로
			if (Input.GetButtonDown("Jump")) {
				anim.SetBool("Rest", true);
			}
		}
		// REST중인 처리
		// 현재베이스 레이어가 restState일 때
		else if (currentBaseState.nameHash == restState)
		{
			//cameraObject.SendMessage("setCameraPositionFrontView");		// 카메라를 정면으로 전환
			// 상태가 전환되고 있지 않은 경우, Rest bool 값을 재설정 (루프하지 않도록한다)
			if(!anim.IsInTransition(0))
			{
				anim.SetBool("Rest", false);
			}
		}
	}


	// 캐릭터의 콜라이더 크기 재설정 함수
	void resetCollider()
	{
	// 구성 요소의 Height, Center의 초기 값을 리턴
		col.height = orgColHight;
		col.center = orgVectColCenter;
	}
}
