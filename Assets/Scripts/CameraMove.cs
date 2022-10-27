using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraMove : MonoSingleton<CameraMove>
{
    private Transform realCamera;
    private Transform fakeCamera;
    [SerializeField]
    float followSpeed;
    float rotaionOnX;
    float rotaionOnY;
    [SerializeField]
    [Range(0, 25)]
    float mouseSensitivity = 12f;
    public bool NoInput;
    public Transform player;
    public Transform target;
    public Animator animator;
    public Animator animator2;
    
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        fakeCamera = transform.GetChild(0);
        realCamera = fakeCamera.GetChild(0);
        //player = FindObjectOfType<PlayerMove>().transform;
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.K)){
            //ShakeCameraRotation(0.3f, new Vector3(90f, 0f, 0f), 70, 90f, true);
            //ShakeCamera(0.3f, new Vector3(1f, 0f, 0f), 5, 90f, false, true);
        }
    }
    public void SetTarget(){
        target = player.GetChild(1);
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if(player == null)return;
        fakeCamera.localPosition = Vector3.Lerp(fakeCamera.localPosition, -realCamera.localPosition, 0.03f);
        fakeCamera.localRotation = Quaternion.Lerp(fakeCamera.localRotation, Quaternion.Inverse(realCamera.localRotation), 0.03f);
        CameraAngleMove();
        CameraMoveFunc();
    }
    public void ShakeCamera(float duration, float strength = 1, int vibrato = 10, float randomness = 90, bool snapping = false, bool fadeOut = true){
        realCamera.DOShakePosition(duration, strength, vibrato, randomness, snapping, fadeOut);
        
    }
    public void ShakeCamera(float duration, Vector3 strength, int vibrato = 10, float randomness = 90, bool snapping = false, bool fadeOut = true){
        realCamera.DOShakePosition(duration, strength, vibrato, randomness, snapping, fadeOut);
        
    }
    public void ShakeCameraRotation(float duration, float strength = 90, int vibrato = 10, float randomness = 90, bool fadeOut = true){
        realCamera.DOShakeRotation(duration, strength, vibrato, randomness, fadeOut);

    }
    public void ShakeCameraRotation(float duration, Vector3 strength, int vibrato = 10, float randomness = 90, bool fadeOut = true){
        realCamera.DOShakeRotation(duration, strength, vibrato, randomness, fadeOut);

    }
    private void CameraMoveFunc(){
        if(!player)return;
        transform.position = Vector3.Lerp(transform.position, target.position+new Vector3(0f, 1.5f, 0f), followSpeed * Time.deltaTime);
    }
    private void CameraAngleMove(){
        if(NoInput)return;
        float mouseY = -Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseX = Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotaionOnX -= mouseX;
        rotaionOnY -= mouseY;
        mouseX = (mouseX > 180.0f) ? mouseX - 360.0f : mouseX;
        mouseY = (mouseY > 180.0f) ? mouseY - 360.0f : mouseY;
        rotaionOnX = Mathf.Clamp(rotaionOnX, -90f, 90f);
        transform.localEulerAngles = new Vector3(rotaionOnX, rotaionOnY, 0f);

        PlayerAngleMove();
        
    }

    private void PlayerAngleMove(){
        player.rotation = Quaternion.Euler(new Vector3(0, rotaionOnY, 0f));
    }
    public void CameraShake(){
        transform.DOShakeRotation(0.3f, 30, 10, 90, true);
    }
}
