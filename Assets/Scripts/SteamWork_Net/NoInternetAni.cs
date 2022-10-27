using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AnimationEnum{
    Fall,
    Jump,
    Landing,
    Walk,
    Run,
}

public class NoInternetAni : MonoBehaviour, AniFunction
{
    private float walkAnimationSmooth = 10f;
    private float speedAnimationSmooth = 10f;
    private Vector3 myMoveDirect = Vector3.zero;
    private Vector3 realDirection;
    private float myMoveSpeed = 0f;
    private float realSpeed;
    public float rigidbodyY = 0f;
    public bool isGround = false;
    public bool jumpTrigger = false;
    public bool fallTrigger = false;
    private bool isStop = false;
    public bool isRun = false;
    [SerializeField]
    private Animator animator;
    private Animator firstCameraAnimator;
    private void Start(){
        //animator = GetComponentInChildren<Animator>();
        firstCameraAnimator = FindObjectOfType<CameraMove>().animator;
    }
    private void Update(){
        AnimationUpdate(animator);
        AnimationUpdate(firstCameraAnimator);

    }
    private void AnimationUpdate(Animator animator){
        Vector3 Direct = myMoveDirect;
        realDirection = Vector3.Lerp(realDirection, Direct, walkAnimationSmooth * Time.deltaTime);
        animator.SetFloat("PlayerAngleX", realDirection.x);
        animator.SetFloat("PlayerAngleY", realDirection.z);
        animator.SetBool("IsStop",Direct.z == 0 && Direct.x == 0);
        realSpeed = Mathf.Lerp(realSpeed, myMoveSpeed, speedAnimationSmooth * Time.deltaTime);
        animator.SetFloat("WalkSpeed", myMoveSpeed);
    }

    public void SetAniDrct(Vector3 _moveDirect)
    {
        myMoveDirect = _moveDirect;
    }

    public void SetAniSpeed(float _moveSpeed)
    {
        myMoveSpeed = _moveSpeed;
    }

    public void SetAniRigidbodyY(float _rigidbodyY)
    {
        rigidbodyY = _rigidbodyY;
        animator.SetFloat("VelocityY", _rigidbodyY);
        firstCameraAnimator.SetFloat("VelocityY", _rigidbodyY);
    }

    public void SetAniIsGround(bool _isGround)
    {
        isGround = _isGround;
        animator.SetBool("IsGround", _isGround);
        firstCameraAnimator.SetBool("IsGround", _isGround);
    }
    public void SetAniIsRun(bool _isRun)
    {
        isRun = _isRun;
        animator.SetBool("IsRun", _isRun);
        firstCameraAnimator.SetBool("IsRun", _isRun);
    }

    public void SetAniJumpTrigger(bool _jumpTrigger)
    {
        jumpTrigger = _jumpTrigger;
        animator.SetTrigger("JumpTrigger");
        firstCameraAnimator.SetTrigger("JumpTrigger");
    }
    public void SetAniFallTrigger(bool _fallTrigger)
    {
        fallTrigger = _fallTrigger;
        animator.SetTrigger("FallTrigger");
        firstCameraAnimator.SetTrigger("FallTrigger");
    }
    public void SetAniGunShootTrigger()
    {
        firstCameraAnimator.SetFloat("PunchIndex", firstCameraAnimator.GetFloat("PunchIndex") * -1);
        Debug.Log(firstCameraAnimator.GetFloat("PunchIndex") * -1);
        animator.SetTrigger("GunShoot");
        firstCameraAnimator.SetTrigger("GunShoot");
    }
    public void SetAniLoseHP(int _loseHP)
    {
        throw new System.NotImplementedException();
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
        return true;
    }
    public bool GetFallTrigger()
    {
        return fallTrigger;
    }

    public void CmdAniGunShootVector(Vector3 gunShootVector)
    {
        return;
    }

    
}
