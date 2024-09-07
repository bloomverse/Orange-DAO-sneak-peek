using System.Runtime.InteropServices;
using UnityEngine;
using TPSBR;
using System;
using System.Collections;

public static class GlobalFunctions
{
    // Import the JavaScript function from the jslib file
    [DllImport("__Internal")]
    private static extern void ChangeBrowserURL(string url);

    // Public static method to change the browser URL
    public static void ChangeURL(string url)
    {
        // Check if we are running in WebGL build before calling the JavaScript method
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            ChangeBrowserURL(url);
        }
        else
        {
            Debug.LogWarning("ChangeURL is only supported in WebGL builds.");
        }
    }

    public static TournamentRounds GetLastRound(TournamentDetail tournamentDetail){
            TournamentRounds lastTrueRound = null;
            //currentRound = tournamentDetail.rounds[tournamentDetail.rounds.Length-1];
            for (int i = 0; i < tournamentDetail.rounds.Length; i++){

               var mongoDate = DateTime.Parse(tournamentDetail.rounds[i].date);
               DateTime unityDate = DateTime.SpecifyKind(mongoDate, DateTimeKind.Utc);
               var endTime = unityDate; 
               TimeSpan timeLeft = endTime - DateTime.Now;   
//               Debug.Log(timeLeft.TotalMinutes + " tiempo de round" + tournamentDetail.rounds[i].number);

                if (tournamentDetail.rounds[i].closed == "false"){
                    return tournamentDetail.rounds[i];
                }

                if (tournamentDetail.rounds[i].closed == "true" && timeLeft.TotalMinutes > -20  ){
                    // If the current round is closed, update lastTrueRound
                    return tournamentDetail.rounds[i];
                }
              
            }
            return null;
        }

        public static TournamentMatches GetCurrentMatch(TournamentDetail tournamentDetail,TournamentRounds currentRound,string userId){
            TournamentMatches currentMatch =null;
            for (int i = 0; i < tournamentDetail.matches.Length; i++){
                if (tournamentDetail.matches[i].round == currentRound.number){
                    var loopMatch = tournamentDetail.matches[i];
//                    Debug.Log(loopMatch.lobby + " - " + loopMatch.players);
                    for (int a = 0; a < loopMatch.players.Length; a++){
                        if( loopMatch.players[a]._id==userId){
                            currentMatch = loopMatch;
                        }
                    }                       
                }
            }
            return currentMatch;
        }
}



