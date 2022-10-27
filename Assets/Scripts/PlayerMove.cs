 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    [Header ("속성")]
    [Tooltip("캐릭터 이동속도 설정")]
    //캐릭터 이동속도 설정
    [SerializeField]
    private float realMoveSpd, targetMoveSpd, moveY;
    private CharacterController characterController;
    [SerializeField]
    private Vector3 targetDirect, realDirect, aniDirect;
    [SerializeField]
    private float moveSmooth = 5, gravity = 10;
    [SerializeField]
    private Transform bottomChkTransform;
    [SerializeField]
    private float bottomChkSize=1f;
    [SerializeField]
    private Transform topChkTransform;
    [SerializeField]
    private float topChkSize=1f;
    [SerializeField]
    private bool isGround = false;
    [SerializeField]
    private bool useGravity = true;
    public GameObject playerModel = null;
    private bool isRun = false;
    private bool isMoving = false;
    private bool isFalling;
    private bool isSit = false;
    private float capsuleHight;
    private float capsuleCenter;
    [SerializeField]
    private float sitDemp;
    private float CameraYPos = 0f;
    private Transform cameraTarget = null;
    [Header("캐릭터 스탯 관련")]
    [SerializeField]
    private PlayerStat playerStat;
    public PlayerStat GetPlayerStat => playerStat;

    [Header("네트워크 관련")]
    public AniFunction aniFunction = null;    





    //-------------------------------
    private Vector3 hitPointNormal;
    private Vector3 hitPoint;
    [SerializeField]
    private float slopeSpeed = 8f;
    private bool IsSliding{
        get{
            Debug.DrawRay(transform.position, Vector3.down*2f, Color.blue);
            if(characterController.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 2f)){
                hitPointNormal = slopeHit.normal;
                hitPoint = slopeHit.point;
                return Vector3.Angle(hitPointNormal, Vector3.up)>=characterController.slopeLimit;
            }else{
                return false;
            }
        }
    }
    void Start()
    {
        cameraTarget = transform.GetChild(1);
        playerStat = gameObject.AddComponent<PlayerStat>();
        characterController = GetComponent<CharacterController>();
        playerModel.SetActive(false);
        useGravity = false;
        aniFunction = GetComponent<AniFunction>();
        aniFunction?.SetAniSpeed(realMoveSpd);
        aniFunction?.SetAniIsGround(isGround);
        //밑으로는 테스트용
        isGround = true;
    }
    void OnEnable()
    {
        
        // 델리게이트 체인 추가
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(aniFunction == null)aniFunction = GetComponent<AniFunction>();
        if((scene.name != "Lobby")){
            if(aniFunction.GetHasAuthority()){
                SetCameraTransform();
                SetPosition();
                GameManager.Instance.mainCamera.GetComponent<CameraMove>().player = transform;
                useGravity = true;
            }
        }
    }
    void OnDisable()
    {
        // 델리게이트 체인 제거
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void Update()
    {
        if(aniFunction == null){
            playerModel.SetActive(true);
            Move();
            return;
        }
        if((SceneManager.GetActiveScene().name != "Lobby")){
            if(playerModel.activeSelf == false&&!aniFunction.GetHasAuthority()){
                playerModel.SetActive(true);
            }
            else if(aniFunction.GetHasAuthority()){
                Move();
            }
        }
        
    }

    private void SetCameraTransform(){
        //GameManager.Instance.mainCamera.SetParent(this.transform);
        GameManager.Instance.mainCamera.GetComponent<CameraMove>().player = this.transform;
        GameManager.Instance.mainCamera.localPosition = new Vector3(0f, 3f, -3f);
        GameManager.Instance.mainCamera.GetComponent<CameraMove>().SetTarget();
    }
    ///<summary>
    ///캐릭터 이동 함수
    ///</summary>
    void Move(){
        
        if(!isGround && characterController.isGrounded){
            //착지
        }
        if(isGround && !characterController.isGrounded){
            if(moveY<0f)
                moveY = 0f;
            
        }
        isGround = characterController.isGrounded;
        
        bool emptyBool = IsSliding;
        if(isGround){
            moveY = -5f;
            isFalling = false;
        }
        else if(!isFalling && Vector3.Distance(transform.position, hitPoint)>0.5f && !BottomChk()){
            aniFunction.SetAniFallTrigger(!aniFunction.GetFallTrigger());

            isFalling = true;
        }
        //당에 닿지 않았을때만 중력을 준다
        useGravity = !isGround;
        //서버에 isGround보냄
        aniFunction?.SetAniIsGround(isGround);
        targetDirect = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        targetDirect.Normalize();
        //애니메이션 방향을 서버에 보냄
        aniDirect = targetDirect;
        aniFunction?.SetAniDrct(aniDirect);

        isSit = Input.GetKey(KeyCode.LeftControl);
        Sit();
        
        isRun = Input.GetKey(KeyCode.LeftShift)&&!isSit;
        if(targetDirect.z<=0f)isRun = false;
        Run();


        targetDirect = transform.TransformDirection(targetDirect); 

        //몸의 방향으로 백터변환
        realDirect = Vector3.Lerp(realDirect, targetDirect, moveSmooth*Time.deltaTime);
        
        //if(btCHK || !fdCHK)
        

        //미끄러지지 않고 바로 멈추는 코드
        if(targetDirect == Vector3.zero){
            //realDirect = realDirect * 0.9f;
            //멈출시 스피드제거
            if(isMoving){
                realMoveSpd = realMoveSpd * 0.6f;
                isMoving = false;
            }
            targetMoveSpd = 0f;
        }else{
            isMoving = true;
        }
        realMoveSpd =  Mathf.Lerp(realMoveSpd, targetMoveSpd, playerStat.GetPlayerStat.speedDemp * Time.deltaTime);

        
        
        //땅이면 중력초기화 밑 점프체크
        
        


        if(isGround && !IsSliding){
            if(Input.GetButtonDown("Jump")){
                Jump();
            }
        }
        //중력
        if(useGravity){
            moveY -= gravity * Time.deltaTime;
        }
        Vector3 slidingDirect = Vector3.zero;
        if(IsSliding){
            slidingDirect = new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slopeSpeed;
        }
        //최종무브
        characterController.Move((realDirect * realMoveSpd + Vector3.up * moveY + slidingDirect)*Time.deltaTime);

        //무브스피드와 리지드바디를 서버에보냄
        aniFunction?.SetAniSpeed(realMoveSpd);
        aniFunction?.SetAniRigidbodyY(moveY);
    }
    private void Sit(){
        if(TopChk())isSit = true;
        if(isSit){
            capsuleHight = 1.08f;
            capsuleCenter = 0.54f;
            CameraYPos = -0.72f;
        }else{
            capsuleHight = 1.8f;
            capsuleCenter = 0.9f;
            CameraYPos = 0f;
        }
        
        characterController.height = Mathf.Lerp(characterController.height, capsuleHight, sitDemp * Time.deltaTime);
        characterController.center = Vector3.Lerp(characterController.center, new Vector3(0f, capsuleCenter, 0f), sitDemp * Time.deltaTime);
        cameraTarget.localPosition = Vector3.Lerp(cameraTarget.localPosition, new Vector3(0f, CameraYPos, 0f), sitDemp * Time.deltaTime);
    }
    private void Run(){
        aniFunction.SetAniIsRun((isRun&&isMoving));
        if(isRun){
            targetMoveSpd = playerStat.GetPlayerStat.moveSpd*playerStat.GetPlayerStat.runPower;
        }else{
            targetMoveSpd = playerStat.GetPlayerStat.moveSpd;
        }
        
    }
    private void Jump(){
        aniFunction?.SetAniJumpTrigger(!aniFunction.GetJumpTrigger());
        moveY = Mathf.Clamp(moveY, 0, Mathf.Infinity);
        moveY += playerStat.GetPlayerStat.jumpForce;
    }
    private bool BottomChk(){
        if(bottomChkTransform == null){
            Debug.LogError("캐릭터 컨트롤러에 바텀체크 넣으세요");
        }
        int layerMask = 1 << LayerMask.NameToLayer("Bottom");
        bool _isGround = Physics.CheckSphere(bottomChkTransform.position, bottomChkSize, layerMask);
        aniFunction?.SetAniIsGround(_isGround);
        return _isGround;
    }
    private bool TopChk(){
        int layerMask = 1 << LayerMask.NameToLayer("Bottom");
        bool _isGround = Physics.CheckSphere(topChkTransform.position, topChkSize, layerMask);
        return _isGround;
    }
    private void OnDrawGizmos() {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawSphere(bottomChkTransform.position, bottomChkSize);
        Gizmos.DrawSphere(topChkTransform.position, topChkSize);
        //Vector3 hightVector = Vector3.up*(capsuleCollider.height/2f-(capsuleCollider.radius+0.1f));
        //Vector3 capsuleCenter = transform.position + capsuleCollider.center;
        //Debug.Log(capsuleCenter);
        
    }

    public void SetPosition(){
        transform.position = Vector3.zero;
        //transform.position = new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // 충돌된 물체의 릿지드 바디를 가져옴
        Rigidbody body = hit.collider.attachedRigidbody;

        // 만약에 충돌된 물체에 콜라이더가 없거나, isKinematic이 켜저있으면 리턴
        if (body == null || body.isKinematic) return;

        if (hit.moveDirection.y < -0.3f)
        {
            return;
        }

        // pushDir이라는 벡터값에 새로운 백터값 저장. 부딪힌 물체의 x의 방향과 y의 방향을 저장
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        // 부딪힌 물체의 릿지드바디의 velocity에 위에 저장한 백터 값과 힘을 곱해줌
        body.velocity = pushDir * 4f;
    }
    
    
}
