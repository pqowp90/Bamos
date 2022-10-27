using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class LobbiesListManager : MonoSingleton<LobbiesListManager>
{

    public GameObject lobbiesMenu;
    public GameObject lobbyDateItemPrefab;
    public GameObject lobbyListContent;
    public GameObject lobbiesButton, hostButton;

    public List<GameObject> listOfLobbys = new List<GameObject>();
    public void GetListOfLobbys(){
        lobbiesButton.SetActive(false);
        hostButton.SetActive(false);

        lobbiesMenu.SetActive(true);

        SteamLobby.Instance.GetLobbiesList();
    }
    public void DisaplayLobbies(List<CSteamID> lobbyIDs, LobbyDataUpdate_t result){
        //DestroyLobbies();
        for(int i=0;i<lobbyIDs.Count;i++){
            if(lobbyIDs[i].m_SteamID == result.m_ulSteamIDLobby){
                Debug.Log("GOGOGOGOGGOO");
                GameObject createdItem = Instantiate(lobbyDateItemPrefab);
                LobbyDataEntry lobbyDataEntry = createdItem.GetComponent<LobbyDataEntry>();
                lobbyDataEntry.lobbyID = (CSteamID)lobbyIDs[i].m_SteamID;
                lobbyDataEntry.lobbyName 
                    = SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDs[i].m_SteamID, "name");

                lobbyDataEntry.SetLobbyData();

                createdItem.transform.SetParent(lobbyListContent.transform);
                createdItem.transform.localScale = Vector3.one;

                listOfLobbys.Add(createdItem);
            }
        }
    }
    public void DestroyLobbies(){
        foreach(GameObject lobbyItem in listOfLobbys){
            Destroy(lobbyItem);
        }
        listOfLobbys.Clear();
    }
    public void ReFreshLobbies(){
        DestroyLobbies();
        GetListOfLobbys();
    }
}
