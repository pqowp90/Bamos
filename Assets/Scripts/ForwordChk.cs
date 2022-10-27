using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwordChk : MonoBehaviour
{
    public bool isWall = false;
    private void OnTriggerStay(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Bottom"))
            isWall = true;
    }
    private void OnTriggerExit(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Bottom"))
            isWall = false;
    }
}
