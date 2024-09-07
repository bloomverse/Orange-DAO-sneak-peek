using System.Collections.Generic;

namespace TPSBR
{
    public class LobbyData
    {
        public string Name;
        public Dictionary<string, Dictionary<string, string>> data;
        public bool isPrivate;
        public int maxPlayers;
        public LobbyPlayer player;
    }
}
