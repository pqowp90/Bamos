using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System;
public interface AniFunction{

    public float GetRigidbodyY();
    public bool GetIsGround();
    public bool GetJumpTrigger();
    public bool GetFallTrigger();
    public bool GetHasAuthority();
    public void SetAniDrct(Vector3 _moveDirect);
    public void SetAniSpeed(float _moveSpeed);
    public void SetAniLoseHP(int _loseHP);
    public void SetAniRigidbodyY(float _rigidbodyY);
    public void SetAniIsGround(bool _isGround);
    public void SetAniIsRun(bool _isRun);
    public void SetAniJumpTrigger(bool _jumpTrigger);
    public void SetAniFallTrigger(bool _fallTrigger);
    public void SetAniGunShootTrigger();
    public void CmdAniGunShootVector(Vector3 gunShootVector);
}
public class PlayerInfoSync : NetworkBehaviour, AniFunction
{
    private float walkAnimationSmooth = 10f;
    private float speedAnimationSmooth = 10f;
    [SyncVar(hook = nameof(SetDirect))] public Vector3 moveDirect = Vector3.zero; private Vector3 myMoveDirect = Vector3.zero;
    private Vector3 realDirection;
    [SyncVar(hook = nameof(SetSpeed))] public float moveSpeed = 0f;private float myMoveSpeed = 0f;
    [SyncVar(hook = nameof(SetanimationEnum))] public int animationEnum = 0;
    public float rigidbodyY = 0f;
    [SyncVar(hook = nameof(SetLoseHP))] public int hp = 0;
    private float realSpeed;
    public bool isGround = false;
    public bool isRun = false;
    private bool isStop = false;
    public bool jumpTrigger = false;
    public bool fallTrigger = false;
    [SyncVar(hook = nameof(SetGunShootVector))]public Vector3 gunShootVector = Vector3.zero;
    [SerializeField]
    private Animator animator = null;
    [SerializeField]
    private Animator firstCameraAnimator = null;
    private Scene nowScene;
    private AnimationEnum nowAnimationEnum;
    private AnimationEnum oldAnimationEnum;
    [SerializeField]
    private RuntimeAnimatorController runtimeAnimatorController1;
    [SerializeField]
    private RuntimeAnimatorController runtimeAnimatorController2;

    private void Start(){
        
        //animator = GetComponentInChildren<Animator>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        if(hasAuthority)
            animator.runtimeAnimatorController = runtimeAnimatorController1;
        else{
            animator.runtimeAnimatorController = runtimeAnimatorController2;
        }
        hp = 100;
        EventManager.TriggerEvent("ResetHp", hp);
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        nowScene = scene;
        if((scene.name != "Lobby")){
            firstCameraAnimator = FindObjectOfType<CameraMove>()?.animator2;
            Debug.Log(FindObjectOfType<CameraMove>()?.animator2);
        }
    }
    private void Update(){
        AnimationUpdate(animator);
        if(hasAuthority&&SceneManager.GetActiveScene().name != "Lobby"&&firstCameraAnimator!=null){
            AnimationUpdate(firstCameraAnimator);
        }
        if(hasAuthority){
            Enum.TryParse<AnimationEnum>(animator.GetCurrentAnimatorStateInfo(0).ToString(), out nowAnimationEnum);
            if(nowAnimationEnum != oldAnimationEnum){
                oldAnimationEnum = nowAnimationEnum;
                SetAniEnum((int)nowAnimationEnum);
                
            }
        }
    }
    private void AnimationUpdate(Animator animator){
        Vector3 Direct = (hasAuthority)?myMoveDirect:moveDirect;
        realDirection = Vector3.Lerp(realDirection, Direct, walkAnimationSmooth * Time.deltaTime);
        animator.SetFloat("PlayerAngleX", realDirection.x);
        animator.SetFloat("PlayerAngleY", realDirection.z);
        animator.SetBool("IsStop",Direct.z == 0 && Direct.x == 0);
        realSpeed = Mathf.Lerp(realSpeed, moveSpeed, speedAnimationSmooth * Time.deltaTime);
        animator.SetFloat("WalkSpeed", (hasAuthority)?myMoveSpeed:moveSpeed);   
    }
    private void SetDirect(Vector3 Oldvalue, Vector3 NewValue){
        this.moveDirect = NewValue;
    }
    private void SetGunShootVector(Vector3 Oldvalue, Vector3 NewValue){
        this.gunShootVector = NewValue;
        if(!hasAuthority){
            // 여기서 총발사 좌표를 보낸다.
        }
    }






    




    private void SetSpeed(float Oldvalue, float NewValue){
        this.moveSpeed = NewValue;
    }
    private void SetanimationEnum(int Oldvalue, int NewValue){
        animator.SetTrigger("Go");
        this.animationEnum = NewValue;
    }
    private void SetRigidbodyY(float Oldvalue, float NewValue){
        this.rigidbodyY = NewValue;
        if(!hasAuthority)
            animator.SetFloat("VelocityY", this.rigidbodyY);
    }








    
    private void SetGunShootTrigger(){
        if(!hasAuthority){
            animator.SetTrigger("GunShoot");
        }
    }
    public void SetLoseHP(int Oldvalue, int NewValue)
    {
        Debug.Log("쳐맞음");
        this.hp = NewValue;
        EventManager.TriggerEvent("ReduceHp", this.hp);
    }



    public void SetAniDrct(Vector3 _moveDirect){
        if(hasAuthority){
            myMoveDirect = _moveDirect;
        }
        if(isServer){
            this.moveDirect = _moveDirect;
        }else{
            CmdAniDrct(_moveDirect);
        }
    }
    public void SetAniGunShootVector(Vector3 _gunShootVector){ // 여기에 맞은곳좌표를 보낸다.

        if(isServer){
            this.gunShootVector = _gunShootVector;
        }else{
            CmdAniGunShootVector(_gunShootVector);
        }
    }






    







    public void SetAniSpeed(float _moveSpeed){
        if(hasAuthority)
            myMoveSpeed = _moveSpeed;
        if(isServer){
            this.moveSpeed = _moveSpeed;
        }else{
            CmdAniMoveSpeed(_moveSpeed);
        }
    }

    public void SetAniEnum(int aniEnum){
        if(hasAuthority)
            this.animationEnum = aniEnum;
        if(isServer){
            this.animationEnum = aniEnum;
        }else{
            CmdAnianimationEnum(aniEnum);
            
        }
    }
    public void SetAniRigidbodyY(float _rigidbodyY){
        if(hasAuthority){
            animator.SetFloat("VelocityY", _rigidbodyY);
            firstCameraAnimator?.SetFloat("VelocityY", _rigidbodyY);
        }
            
        if(isServer){
            this.rigidbodyY = _rigidbodyY;
        }
    }

    public void SetAniLoseHP(int _loseHP){
        if(isServer){
            this.hp -= _loseHP;
        }else{
            CmdAniLoseHP(_loseHP);
        }
    }



    public void SetAniIsGround(bool _isGround){
        if(hasAuthority){
            animator.SetBool("IsGround", _isGround);
            firstCameraAnimator?.SetBool("IsGround", _isGround);
        }
            
        if(isServer){
            this.isGround = _isGround;
        }
    }
    
    public void SetAniIsRun(bool _isRun)
    {
        if(hasAuthority){
            animator.SetBool("IsRun", _isRun);
            firstCameraAnimator?.SetBool("IsRun", _isRun);
        }
            
        if(isServer){
            this.isRun = _isRun;
        }
    }

    public void SetAniJumpTrigger(bool _jumpTrigger){
        if(hasAuthority){
            animator.SetTrigger("JumpTrigger");
            firstCameraAnimator?.SetTrigger("JumpTrigger"); 
        }
        if(isServer){
            this.jumpTrigger = _jumpTrigger;
        }
    }

    public void SetAniFallTrigger(bool _fallTrigger)
    {
        if(hasAuthority){
            animator.SetTrigger("FallTrigger");
            firstCameraAnimator?.SetTrigger("FallTrigger"); 
        }
        if(isServer){
            this.fallTrigger = _fallTrigger;
        }
    }
    public void SetAniGunShootTrigger(){
        if(hasAuthority){
            animator.SetTrigger("GunShoot");
            firstCameraAnimator?.SetTrigger("GunShoot"); 
            Debug.Log("펀치");
        }
        if(!isServer){
            CmdGunShootTrigger();
        }
    }


    [Command]
    public void CmdGunShootTrigger(){
        SetGunShootTrigger();
    }
    [Command]
    public void CmdAniDrct(Vector3 _moveDirect){
        SetDirect(this.moveDirect, _moveDirect);
    }
    [Command]
    public void CmdAniGunShootVector(Vector3 _gunShootVector){
        SetGunShootVector(this.gunShootVector, _gunShootVector);
    }

    [Command]
    public void CmdAniMoveSpeed(float _moveSpeed){
        SetSpeed(this.moveSpeed,  _moveSpeed);
    }
    [Command]
    public void CmdAnianimationEnum(int aniEnum){
        SetanimationEnum(this.animationEnum,  aniEnum);
    }


    [Command]
    public void CmdAniLoseHP(int _loseHP){
        SetLoseHP(this.hp,  _loseHP);
    }







    public float GetRigidbodyY()
    {
        return rigidbodyY;
    }

    public bool GetIsGround()
    {
        return isGround;
    }

    public bool GetJumpTrigger()
    {
        return jumpTrigger;
    }

    public bool GetHasAuthority()
    {
        return hasAuthority;
    }

    public bool GetFallTrigger()
    {
        return fallTrigger;
    }

    
}

