using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace TPSBR
{
    /// <summary>
    /// QueryToLocalList the lobby resulting from a request into a LocalLobby for use in the game logic.
    /// </summary>
    public static class Converters
    {
        public static List<Lobby> QueryToLobbies(QueryResponse response)
        {
            List<Lobby> retLst = new List<Lobby>();
            foreach (var lobby in response.Results)
                retLst.Add(lobby);
            return retLst;
        }
    }
}
