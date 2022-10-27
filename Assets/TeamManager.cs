using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TeamManager : MonoSingleton<TeamManager>
{
    [SerializeField]
    private float timeDeley = 5f;
    [SerializeField]
    private TextMeshPro countdownTextMeshPro1;
    [SerializeField]
    private TextMeshPro countdownTextMeshPro2;
    [SerializeField]
    private bool isTimeGo = false;
    private List<PlayerObjectControler> team1 = new List<PlayerObjectControler>();
    private List<PlayerObjectControler> team2 = new List<PlayerObjectControler>();
    private bool gameStarted = false;
    [SerializeField]
    private Transform team1StartPoint;
    [SerializeField]
    private Transform team2StartPoint;
    //private LobbiesListManager
    private void Update() {
        if(!gameStarted){
            if(isTimeGo){
                timeDeley-=Time.deltaTime;
            }
            countdownTextMeshPro1.text = "" + (int)timeDeley;
            countdownTextMeshPro2.text = "" + (int)timeDeley;
            if(timeDeley<=0f){
                gameStarted = true;
                for(int i=0;i<team1.Count;i++){
                    team1[i].transform.position = team1StartPoint.position;
                }
                for(int i=0;i<team2.Count;i++){
                    team2[i].transform.position = team2StartPoint.position;
                }
            }
        }else{

        }

    }
    
    public void SetTeam(bool isTeam1, List<PlayerObjectControler> team){
        if(isTeam1){
            team1 = team;
        }else{
            team2 = team;
        }
        if((team1.Count+team2.Count != 0)&&(Mathf.Abs(team1.Count - team2.Count))==0){
            isTimeGo = true;
        }else{
            isTimeGo = false;
            timeDeley = 5f;
        }
    }

}
