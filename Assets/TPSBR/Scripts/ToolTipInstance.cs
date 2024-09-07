using System.Collections;
using System.Collections.Generic;
using TPSBR.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TPSBR
{
    public class ToolTipInstance : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        
        private UIButton uIButton;
        [SerializeField] private string displayText;

        [SerializeField] private float XOffset = 0;

        void Start()
        {
            uIButton = GetComponent<UIButton>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Tooltip.ShowToolTip_Static(displayText,XOffset);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Tooltip.HideToolTip_Static();
        }

      
        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
