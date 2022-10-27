using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InputAddress : MonoBehaviour
{
    
    private string address;
    public void SetAddress(string _address){
        address = _address;
    }
    public void Input_Address(){
        SteamLobby.Instance.Input_SurverAddress(address);
    }
    public void CopyAddress(){
        CopyToClipboard(SteamLobby.Instance.GetAddress());
    }
    public static void CopyToClipboard(string str)
    {
        GUIUtility.systemCopyBuffer = str;
    }
}
