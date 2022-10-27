using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class PlayerStat : MonoBehaviour
{
    [Serializable]
    public class PlayerSuChi
    {
        [Header("플레이어 스텟")]
        [SerializeField] public float moveSpd;
        [SerializeField] public float runPower;
        [SerializeField] public float jumpForce;
        [SerializeField] public float speedDemp;
        [SerializeField] public int maxHp;
        public PlayerSuChi(){
            speedDemp = 15f;
            moveSpd = 3f;
            runPower = 1.7f;
            jumpForce = 3.5f;
            maxHp = 100;
        }
    }
    [SerializeField]
    private PlayerSuChi playerSuChi = new PlayerSuChi();
    public PlayerSuChi GetPlayerStat=>playerSuChi;
}
