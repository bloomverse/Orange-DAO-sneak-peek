using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace TPSBR {



public class ObjectSwing : MonoBehaviour, IActivable
{
    // Start is called before the first frame update



    void Start()
    {
        transform.DOLocalRotate(new Vector3(0,20,0),4).SetEase(Ease.OutBack).SetLoops(-1,LoopType.Yoyo);
    }

    public void Activate(){
       
       InfoManager.instance.Activate("Press E for pizza ordering");
    }

    public void Deactivate(){
        InfoManager.instance.Deactivate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


}