using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using DG.Tweening.Plugins;
using Unity.Services.Lobbies.Models;
using TPSBR.UI;
using System;


namespace TPSBR
{

   
public class LobbyPlayers3D : MonoBehaviour
{
    [SerializeField]
    private GameObject[] slots; //Array containing all the slots
    private Unity.Services.Lobbies.Models.Player[] playersOn; //Array to check if the slot is occupied
    private GameObject[] prefab; //Array to check if the slot is occupied


    [SerializeField]
    private AudioClip occupiedSound; //Sound to play when the slot is occupied
     [SerializeField]
    private AudioClip readySound; //Sound to play when the slot is occupied
    private AudioSource audioSource;

    [SerializeField]
    private GameObject prefabLilly;

    [SerializeField] Color green;
    [SerializeField] Color red;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        playersOn = new Unity.Services.Lobbies.Models.Player[slots.Length];
        prefab = new GameObject[slots.Length];
    }

    public void updateLobbyPlayers(Lobby lobby){

        var players = lobby.Players;

        // Check if there is a missing player in new info
        clearSpots(players);
        // Iterate PLayers
        IteratePlayers(players);

    }

    private void clearSpots(List<Unity.Services.Lobbies.Models.Player> players){
        for (int i = 0; i < playersOn.Length; i++){
                if(playersOn[i]==null){
                    continue;
                }
                var pOn = playersOn[i].Id;
                bool found = false;
                for(int p=0; p< players.Count ; p++){
                    if(players[p].Id==pOn){
                        found = true;
                    }

                }
                if(found==false){
                    clearSpot(i);
                }
        }
    }

    private void clearSpot(int index){
        playersOn[index] = null;

        //slots[index].GetComponent<Renderer>().material.color = Color.white;
        var obj2 = slots[index].transform.Find("readyParticle");
        obj2.GetComponent<ParticleSystem>().Stop();
        Destroy(prefab[index]);
        prefab[index] = null;
    }


    private int assingSpot(){
        int index = -1;
        for (int i = 0; i < playersOn.Length; i++)
        {
            if(playersOn[i]==null){
                return i;
            }
        }
        return index;
    }
    private int searchPlayer(string id){
        int index = -1;
        for (int i = 0; i < playersOn.Length; i++)
        {   
            if(playersOn[i]==null)  continue;

            if(playersOn[i].Id == id){
                return i;
            }
        }

        return index;
    }

    //  if its already there , update info just if changed



    public void IteratePlayers(List<Unity.Services.Lobbies.Models.Player> players){

          for (int i = 0; i < players.Count; i++){

                var AgentID = players[i].Data["Agent"].Value;
                var Status = players[i].Data["Status"].Value;
                var Nickname =players[i].Data["Nickname"].Value;
                var Skin = players[i].Data["Skin"].Value;
                var Id = players[i].Id;

                 //Debug.Log(players.Count + " Players reales " + players[i].Id);
//               Debug.Log(AgentID +"-" + Status + " - " + "-" + Nickname + "-" + Skin +"-"+Id);
                 int indexPlayer = searchPlayer(Id);
                 if(indexPlayer==-1){
                    int spot =assingSpot(); 
//                    Debug.Log(spot + " new assigned spot");
                    var prefabPos = slots[spot].transform.position;
                    var agentSetup = Global.Settings.Agent.GetAgentSetup(AgentID);
                    GameObject instantiated = Instantiate(agentSetup.MenuAgentPrefab,prefabPos,slots[spot].transform.rotation);

                    instantiated.transform.parent = slots[spot].transform;
                    instantiated.transform.localPosition = new Vector3(0.43f,0.07f,.3f);
                    playersOn[spot] = players[i];
                    prefab[spot] = instantiated;
                    var obj = slots[spot].transform.Find("joinParticle");
                    obj.GetComponent<ParticleSystem>().Play();
                    audioSource.PlayOneShot(occupiedSound);

                    /*if(prefab[i].GetComponentInChildren<SkinAdaptorTextures>()!=null){
                            var pref = prefab[i].GetComponentInChildren<SkinAdaptorTextures>();
                            pref.setRandom(Skin);
                    }*/

                    if(prefab[i].GetComponentInChildren<Animator>()!=null){
                        
                        var pref = prefab[i].GetComponentInChildren<Animator>();
                         System.Random random = new System.Random();
                         int randomNumber = random.Next(0, 4);
             //            Debug.Log(randomNumber + " Random number animation");
                        pref.SetFloat("Blend",randomNumber);
                    }

                        var text = prefab[i].GetComponentInChildren<TextMeshProUGUI>();
                        text.text = Nickname;
//                        Debug.Log("Nickname: " + Nickname );

                    }else{

                    // Update player Data;
                   // if(playersOn[indexPlayer].Data["Nickname"].Value!=Nickname){
                        var text = prefab[indexPlayer].GetComponentInChildren<TextMeshProUGUI>();
                        text.text = Nickname;
                  //  }
                    

       
                    if(playersOn[indexPlayer].Data["Status"].Value!=Status){

                        if(Status=="Waiting"){
                              playersOn[indexPlayer].Data["Status"].Value = Status;
                     //       slots[indexPlayer].GetComponent<Renderer>().material.color = Color.red;
                            var obj2 = slots[indexPlayer].transform.Find("readyParticle");
                            obj2.GetComponent<ParticleSystem>().Stop();
                        }

                        if(Status=="Ready"){
                            playersOn[indexPlayer].Data["Status"].Value = Status;
                        var obj3 = slots[indexPlayer].transform.Find("readyParticle");
                        obj3.GetComponent<ParticleSystem>().Play();
                        audioSource.PlayOneShot(readySound);
//                        slots[indexPlayer].GetComponent<Renderer>().material.color = Color.green;
                        }
                    }

//                    Debug.Log(playersOn[indexPlayer].Data["Skin"].Value + " - " + Skin);
                    
                        if(prefab[indexPlayer].GetComponentInChildren<SkinAdaptorTextures>()!=null && prefab[indexPlayer].GetComponentInChildren<SkinAdaptorTextures>().IDon!=Skin){
                            var pref = prefab[indexPlayer].GetComponentInChildren<SkinAdaptorTextures>();
                            pref.setRandom(Skin);
                        }
                        

                  
                 }

            
            }
       

    }


}
}
