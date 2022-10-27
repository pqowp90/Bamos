using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunOnOff : MonoBehaviour
{
    public GameObject obj1;
    public GameObject obj2;
    public GameObject obj3;
    private bool onoff;
    private void Update() {
        if(Input.GetKeyDown(KeyCode.G))
            GunGunOnOff();
    }
    public void GunGunOnOff(){
        onoff = !onoff;
        obj1.SetActive(onoff);
        obj2.SetActive(onoff);
        obj3.SetActive(onoff);
    }
}
