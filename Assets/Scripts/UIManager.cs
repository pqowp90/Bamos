using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField]
    private Image hpbar;
    private PlayerStat playerStat;
    [SerializeField]
    private float hp, realHp, hpDemp;
    
    private PlayerInfoSync playerInfoSync;
    private void Start() {
        EventManager.StartListening("ReduceHp", ReduceHp, 1);
        EventManager.StartListening("ResetHp", ResetHp, 1);
    }
    private void Update() {
        realHp = Mathf.Lerp(realHp, hp, hpDemp * Time.deltaTime);
        hpbar.fillAmount = realHp;
    }
    public void ReduceHp(int _hp){
        hp -= _hp;
    }
    public void ResetHp(int _hp){
        hp = _hp;
    }
}
