using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TPSBR
{
    public class LeaderList : MonoBehaviour
    {
        [SerializeField] VerticalLayoutGroup gridContainer;
        [SerializeField] GridLeaderItem gridItem;


         public void StartLeader()
        {
            DataManager.instance.topScoreStart("hopscotch");
            DataManager.OnTopScores += topRes;
        }

        // Update is called once per frame
        void topRes(topScores topScores){
            setList(topScores);
        }
        


       public void setList(topScores topScores)
{
        // Clear existing items
        Debug.Log("Creatying list------------------ scores " + topScores.scores.Length);
        /*foreach (Transform child in gridContainer.transform)
        {
            Destroy(child.gameObject);
        }*/

        // Populate the list with new items
        for (int i = 0; i < topScores.scores.Length; i++)
        {
            GridLeaderItem gridLeaderItem = Instantiate(gridItem, gridContainer.transform);
            gridLeaderItem.setValues((i + 1).ToString(), topScores.scores[i].username,topScores.scores[i].score.ToString());
            gridLeaderItem.gameObject.SetActive(true); // Ensure the instantiated item is active
        }
}

       
    }
}
