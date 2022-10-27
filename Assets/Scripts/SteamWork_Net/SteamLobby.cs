using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using Mirror;

public class SteamLobby : MonoSingleton<SteamLobby>
{
    //Callbacks
    protected Callback<LobbyCreated_t>LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t>JoinRequest;
    protected Callback<LobbyEnter_t>LobbyEnterd;


    protected Callback<LobbyMatchList_t>LobbyList;
    protected Callback<LobbyDataUpdate_t>LobbyDataUpdated;

    public List<CSteamID>lobbyIDs = new List<CSteamID>();

    //Variables
    public ulong CurrentLobbyID;
    
    private const string HostAddresskey = "DeadWay";
    private CustomNetworkManager manager;
    //GameObject
    [Header("디버그용")]
    [SerializeField]
    private bool fastGameStart;
    private bool JoinWithAddress = false;
    private string joinAddress;
    public void Input_SurverAddress(string address){
        LobbiesListManager.Instance.ReFreshLobbies();
        JoinWithAddress = true;
        joinAddress = address;
        
    }
    private void LetsGoAddress(){
        if(!JoinWithAddress)return;
        JoinWithAddress = false;
        Debug.Log(lobbyIDs.Count);
        foreach(var lobbyID in lobbyIDs){
            string name = SteamMatchmaking.GetLobbyData((CSteamID)lobbyID.m_SteamID, "name");
            string[] spstring = name.Split('$');
            Debug.Log(spstring[2]);
            if(spstring[2]==joinAddress){
                SteamLobby.Instance.JoinLobby(lobbyID);
            }
        }
    }
    public string GetAddress(){
        return StringConverter.ConvertToSimpleEncoding(SteamFriends.GetPersonaName().ToString());
    }

    private void Start(){
        if(!SteamManager.Initialized) return;
        manager = GetComponent<CustomNetworkManager>();
        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEnterd = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        LobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbyList);
        LobbyDataUpdated = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyData);
        if(fastGameStart){
            HostLobby();
        }
    }

    public void HostLobby(){
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, manager.maxConnections);//k_ELobbyTypePublic, k_ELobbyTypeFriendsOnly

    }
    public void OnLobbyCreated(LobbyCreated_t callback){
        if(callback.m_eResult != EResult.k_EResultOK)return;

        manager.StartHost();

        string inviteCode = StringConverter.ConvertToSimpleEncoding(SteamFriends.GetPersonaName().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddresskey, SteamUser.GetSteamID().ToString() );
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", "DeadWay$"+SteamFriends.GetPersonaName().ToString()+"$"+inviteCode);
        
        Debug.Log("로비가 성공적으로 만들어졌습니다.");
        Debug.Log("DeadWay$"+SteamFriends.GetPersonaName().ToString()+"$"+inviteCode);


    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback){
        Debug.Log("Request To Join Lobby");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }
    

    private void OnLobbyEntered(LobbyEnter_t callback){
        
        CurrentLobbyID = callback.m_ulSteamIDLobby;
        if(NetworkServer.active)return;
        
        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddresskey);
        Debug.Log(manager.networkAddress);
        manager.StartClient();
    }
    public void JoinLobby(CSteamID lobbyID){
        SteamMatchmaking.JoinLobby(lobbyID);
        LobbiesListManager.Instance.ReFreshLobbies();
    }
    public void GetLobbiesList(){
        if(lobbyIDs.Count > 0){
            lobbyIDs.Clear();
        }
        SteamMatchmaking.AddRequestLobbyListResultCountFilter(60);
        SteamMatchmaking.RequestLobbyList();
    }

    public void OnGetLobbyList(LobbyMatchList_t result){
        if(LobbiesListManager.Instance.listOfLobbys.Count > 0){
            LobbiesListManager.Instance.DestroyLobbies();
        }
        Debug.Log("현재 받아온 로비 수"+result.m_nLobbiesMatching);
        for(int i=0;i<result.m_nLobbiesMatching;i++){
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            //Debug.Log(SteamMatchmaking.GetLobbyData((CSteamID)lobbyID.m_SteamID, "name"));
            
            string name = SteamMatchmaking.GetLobbyData((CSteamID)lobbyID.m_SteamID, "name");
            string[] spstring = name.Split('$');
            if(spstring[0] != "DeadWay")continue;
            Debug.Log("데드웨이");
            if(SteamMatchmaking.GetLobbyData((CSteamID)lobbyID.m_SteamID, "name")!=""){
                lobbyIDs.Add(lobbyID);
                SteamMatchmaking.RequestLobbyData(lobbyID);
            }
        }
        Debug.Log("데드웨이의 서버들"+lobbyIDs.Count);
        LetsGoAddress();
    }
    void OnGetLobbyData(LobbyDataUpdate_t result){
        LobbiesListManager.Instance.DisaplayLobbies(lobbyIDs, result);
    }
}
