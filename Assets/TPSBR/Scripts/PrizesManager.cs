using UnityEngine;
using System;
using TPSBR;

public static class InventoryManager
{
    private const string InventoryKey = "PlayerInventory2";

    public static  int maxChests = 3;
    public static  DateTime lastClaimedTime;
    public static string typeActivity = "lobbychest";

    // Adds a prize to the inventory and saves it
    public static void AddPrizeAndSave(Prize newPrize)
    {
        //PrizeInventory inventory = GetInventory();
        //inventory.prizes.Add(newPrize);
        //SaveInventory(inventory);
        SaveLastClaimDate(newPrize.internalId);
    }

    public static void clearCache(){
        PlayerPrefs.DeleteAll();
    }
   /*private static void SaveInventory(PrizeInventory inventory)
    {
        string json = JsonUtility.ToJson(inventory);
        PlayerPrefs.SetString(InventoryKey, json);
        PlayerPrefs.Save();
    }*/

    public static void GetInventory()
    {

        Debug.Log("getting chest inventory");
        DataManager.instance.GeLastActivitiesForUser(typeActivity,1);
        DataManager.OnActivitiesFetched += getStateClaimable;

        /*if (PlayerPrefs.HasKey(InventoryKey))
        {
            string json = PlayerPrefs.GetString(InventoryKey);
            return JsonUtility.FromJson<PrizeInventory>(json);
        }
        return new PrizeInventory(); // Return new inventory if not found*/
    }
    private static void  getStateClaimable(Activities activities,string type){

            if(type==typeActivity){
                 if(activities.activityData.Length>0 ){
                   ActivityData activityData = activities.activityData[0];
                lastClaimedTime = activityData.GetDateTime();
   
                getRemaining(lastClaimedTime);

                DateTime now = DateTime.Now;
                double hoursSinceLastClaim = (now - lastClaimedTime).TotalHours;
                bool valid = hoursSinceLastClaim >= 3; 

                Debug.Log("Time CHEST QUERY");
                Debug.Log(hoursSinceLastClaim + " Hour since" + lastClaimedTime + " -- " + activityData.ts +  " -- " + activityData.name);

                if(!valid){
                   // interaction.setState(false,GetFormattedRemainingTime(lastClaimedTime));
                   // leverObj.DORotate(new Vector3(45,0,0),1,RotateMode.LocalAxisAdd).SetEase(Ease.InOutBack);//.SetLoops(1,LoopType.Yoyo);
                    
                }else{
                    //interaction.setState(true,GetFormattedRemainingTime(lastClaimedTime));
                    //interaction.SetActive(true);
                }
            }else{
                //interaction.setState(true,DateTime.Now.ToString());
                //interaction.SetActive(true);
            }
            }
           

         
        }
    
   public static void RetrieveInventory()
    {
        GetInventory(); 
    }

    public static void SaveLastClaimDate(string internalId)
    {

        Debug.Log("saving activity");

         ActivityData ad = new ActivityData();
            ad.type = "lobbychest";
            ad.name = "Lobby Chests";
            ad.description = internalId;
            DataManager.instance.AddActivityStart(ad);
            DataManager.OnActivityAdded += addActivityReturn;

            
        //DateTime now = DateTime.Now;
        //PlayerPrefs.SetString("LastClaimDate", now.ToString("o")); // "o" is the round-trip date/time pattern.
        //PlayerPrefs.Save();
    }

    public static void addActivityReturn(ActivityData activityData){
        GetInventory(); 
        DataManager.OnActivityAdded -= addActivityReturn;
    }

    public static double getRemaining(DateTime lastClaimedTime){
    /*DateTime lastClaimedTime;
    if (!DateTime.TryParse(PlayerPrefs.GetString("LastClaimDate", ""), out lastClaimedTime))
    {
        return 0; // If there's no stored time, a prize can be claimed immediately.
    }*/

    DateTime now = DateTime.Now;
    double hoursSinceLastClaim = (now - lastClaimedTime).TotalHours;
    if (hoursSinceLastClaim >= 3) return 0; // A prize can be claimed now.

    return 3 - hoursSinceLastClaim; // Hours remaining until the next prize can be claimed.
}

   public static bool TodayLimit(){

       /* if (!DateTime.TryParse(PlayerPrefs.GetString("LastClaimDate", ""), out lastClaimedTime))
        {
            return true;
        }*/
//        Debug.Log(lastClaimedTime);
        if(lastClaimedTime == DateTime.MinValue){
            return true;
        }
        
        DateTime now = DateTime.Now;
        double hoursSinceLastClaim = (now - lastClaimedTime).TotalHours;
//        Debug.Log(hoursSinceLastClaim >= 3);
        return hoursSinceLastClaim >= 3; 
    }

    public static string GetFormattedRemainingTime(DateTime lastClaimedDate){
    double remainingHours = getRemaining(lastClaimedDate);
//    Debug.Log(remainingHours + " timeh") ;
    if (remainingHours <= 0)
    {
        return "Find a chest now!";
    }
    else
    {
        TimeSpan timeSpan = TimeSpan.FromHours(remainingHours);
        return string.Format("Available chest in: {0} H : {1} M", 
                             (int)timeSpan.TotalHours, 
                             timeSpan.Minutes);
    }
}

   
}
