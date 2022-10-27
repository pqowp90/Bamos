using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TeamSelect : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro textMeshPro;
    [SerializeField]
    private List<PlayerObjectControler> PlayerList;
    [SerializeField]
    private bool isRed;
    private void OnTriggerEnter(Collider other) {
        Debug.Log(isRed);
        if(other.GetComponent<PlayerObjectControler>()!=null)
            PlayerList.Add(other.GetComponent<PlayerObjectControler>());
        UpdateBillboard();
    }
    private void OnTriggerExit(Collider other) {
        PlayerList.Remove(other.GetComponent<PlayerObjectControler>());
        UpdateBillboard();
    }
    private void UpdateBillboard(){
        string lists = "";
        if(PlayerList.Count>0){
            lists = PlayerList[0].PlayerName;
            for(int i=1;i<PlayerList.Count;i++){
                lists += ", " + PlayerList[i].PlayerName;
            }
            TeamManager.Instance.SetTeam(isRed, PlayerList);
        }
        textMeshPro.text = lists;
        
    }
}
