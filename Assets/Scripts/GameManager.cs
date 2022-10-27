using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public Transform mainCamera;
    
    private void Start(){
        if(mainCamera == null)
            mainCamera = Camera.main.transform;
    }
}
