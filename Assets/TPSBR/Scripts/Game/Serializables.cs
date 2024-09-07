
namespace TPSBR
{

using System;
using System.Collections.Generic;
    using NanoSockets;
    using Unity.Services.Lobbies.Models;
    using UnityEngine;

[System.Serializable]
public class UserData
{
    public string username;
    public string _id = "";
    public string solanaWallet = "";
    public string ParticleConnection;
    public string gameShiftWallet;
    public string phantomWallet;
    public string availableWallet;
    public string googleId = "";
    public string email = "";
    public string nickname = "";
    public float cbloomies = 0;
    public float vbloomies = 0;
    public int lastBloomieBonus;
    public long lastPlayed;
    public List<characters> characters = new List<characters> {};
    public int userLevel = 0;
    public int userExp = 0;
    public int gamesPlayed = 0;
    public int status;
    
    
}


[System.Serializable]
public class Prize
{
    public string internalId;
    public string claimedDate;
}

[System.Serializable]
public class PrizeInventory
{
    public List<Prize> prizes = new List<Prize>();
}

[System.Serializable]
public class characters
{
    public string id;
    public int level;
    public float exp;
}

[System.Serializable]
public class Mission
{
    public int missionId;
    public string missionTitle;
    public string missionDescription;
    public bool assigned;
    public List<Objective> objectives;
}
[System.Serializable]
public class Objective
{
    public int objectiveId;
    public string objectiveDescription;
    public int currentValue;
    public int targetValue;
    public int rank;
}

[System.Serializable]
public class Currencies
{
    public Currency[] data;
}

[System.Serializable]
public class Currency
{
    public float amount;
    public string id;
    public string mintAddress;
    public string name;
    public string symbol;
}

[System.Serializable]
public class gptResponse 
{
    public string result;
}


[System.Serializable]
public class Lobby2 
{
    public string id;
}




[System.Serializable]
public class UserInfo
{
    public string accountPublicKey;
    public string email;
    public string username;
}

[System.Serializable]
public class JSONWebToken
{
    public string token;
}

[System.Serializable]
public class apiResponse
{
    public int statusCode;
    public string desc;
}

[System.Serializable]
public class LoginValidationReturn
{
    public string status = "";
    public string _id = "";
    public string email = "";
    public string token;
    public string solanaWallet  = "";
    public string gameShiftWallet = "";
    public string phantomWallet = "";
    public string gameShiftCreated = "";
}


[System.Serializable]
public class AuthUrl
{
    public string url;
    public string code;
}

[System.Serializable]
public class AuthResult
{
    public string clientId;
    public string code;

    public AuthResult(string clientId, string code)
    {
        this.clientId = clientId;
        this.code = code;
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }
}

[System.Serializable]
public class UserToken
{
    public string bearerToken;
    public string userId;
}




[System.Serializable]
public class Coin
{
    public string amount;
    public float decimals;
    public string uiAmount;
    public string symbol;
    public string name;
    public string address;
    public string logoUri;
}

[System.Serializable]
public class Coins
{
    public List<Coin> coins = new List<Coin>();
}

[System.Serializable]
public class Files
{
    public string type;
    public string uri;
}

[System.Serializable]
public class Item
{
    public string id;
    public string name;
    public bool isForSale;
    public List<Files> files = new List<Files>();
}

[System.Serializable]
public class Items
{
    public List<Item> items = new List<Item>();
}

[System.Serializable]
public class NFT
{
    public string name;
    public string collectionName;
    public string tokenAddress;
    public string collectionAddress;
    public string imageUrl;
    public string chain;
    public string network;
    public string description;
    public string tokenCreator;
    public Trait[] traits;
    
}

[Serializable]
public class NFTS
{
    public NFT[] nfts;
}

[Serializable]
public class Trait
{
    public string trait_type;
    public string value;
    public string display_type;
}


[System.Serializable]
public class UserToBase
{
    public string UserID;
    public string DBID;
    public int matchEXP;
    public string SelectedChar;
    public int bloomies;
    public int vbloomies;
    public int lastBloomieBonus = 0;
    public long lastPlayed;
    

}



[System.Serializable]
public class PrizeData
{
    public string UserID;
    public string DBID;
    
}


[System.Serializable]
public class CustomPrizeData
{
    public string UserID;
    public string nickname;
    public string DBID;
    public string internalId;
    
}




[System.Serializable]
public class GameEndAnounce
{
    public int amount;
    public string description;
    public AudioClip sound;
    public string SelectedChar;
    

}

[System.Serializable]
public class Tournaments
{   
    public TournamentDetail[] tournaments;
}


[System.Serializable]
public class Tournament
{
    public string name;
    public string _id;
    public string region;
    public bool status;
    public string start_date;
    public string end_date;
    public int num_rounds;
    public int num_players;
    public string[] prizes;
    

}


[System.Serializable]
public class TournamentDetail
{
    public string name;
    public string _id;
    public string description;
    public bool active;
    public string status;
    public string start_date;
    public string end_date;
    public int max_players;
    public string region;
    public int num_rounds;
    public int num_players;
    public string image;
    public string background;
    public string logo;
    public TournamentPlayer[] players ; 
    public int playerCount;
    public TournamentPrizes[] prizes;
    public TournamentConditions[] conditions;
    public TournamentRounds[] rounds;
    public TournamentMatches[] matches;
}

[System.Serializable]
public class TournamentPrizes
{
    public int position;
    public string prize;
}

[System.Serializable]
public class TournamentRounds
{
    public string date;
    public int wait_time;
    public string map;
    public int number;
    public int players;
    public string gameplay_type;
    public string closed;
}

[System.Serializable] // NOT FINISHED NOT WORKING
public class TournamentMatches
{
    public string lobby;
    public Lobby lobbyData;
    public int round;
    public string region;
    public string State;
    public TournamentPlayer[] players;
}

[System.Serializable]
public class TournamentConditions
{
    public string condition; // token / level
    public string value;  // tokenid
    public float amount;
    public int lives;
    public string description = "";

    
}


[System.Serializable]
public class TournamentPlayers
{
    public TournamentPlayer[] players; 
}


[System.Serializable]
public class TournamentPlayer
{
   // public string unity;
    public string _id;
    public string nickname;
    public int score;
    public int rounds;
    public int kills;
    public int assists;
    public int deaths;
    public float damage;
    

}


[System.Serializable]
public class TournamentMatchInfo
{
   // public string unity;
    public string _id;
    public string status;
    public string name;
    public Rounds[] rounds;
    public Matches[] matches;
}

[System.Serializable]
public class Rounds
{
    public int number=0;
    public int wait_time;
    public int duration;
    public string map;
    public string gameplay_type;
    public int players;
    public string closed;
    public string date;


}

[System.Serializable]
public class Matches
{
    public string lobby;
    public int round;
    public string id;
}




[System.Serializable]
public class finalAnimation
{
    public string description;
    public int  amount = 0;
    public GameObject attractor;
    public int particleIndex;
    public string particleName ;
}




}