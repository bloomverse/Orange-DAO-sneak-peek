using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TPSBR
{
    public class HopscotchProductPuzzle : MonoBehaviour
    {
        [SerializeField] private HopscotchProductPuzzlePlatform m_LeftPlatform, m_RightPlatform;
        [SerializeField] private AudioSource m_AudioSource;
        [SerializeField] private AudioClip m_EnableSFX;
        [SerializeField] private TMP_Text m_LeftProductHintText, m_RightProductHintText, m_ProductText;
        [SerializeField] private MeshRenderer m_LeftProductHintImage, m_RightProductHintImage, m_ProductImage;

        private int m_ProductIndex;
        private bool m_ImageHint;

        public HopscotchProductPuzzle Init(Material[] a_ProductMaterials, bool a_ImageHint)
        {
            List<Material> _ProductMaterials = new List<Material>(a_ProductMaterials);
            
            m_ProductIndex = Random.Range(0, _ProductMaterials.Count);
            m_ImageHint = a_ImageHint;
            bool _LeftCorrect = Random.Range(0, 2) == 0;

            if (a_ImageHint)
            {
                if (_LeftCorrect)
                {
                    m_LeftProductHintText.text = _ProductMaterials[m_ProductIndex].GetTexture("_MainAlbedo").name;
                    m_ProductImage.material = _ProductMaterials[m_ProductIndex];
                    _ProductMaterials.RemoveAt(m_ProductIndex);
                    m_RightProductHintText.text = _ProductMaterials[Random.Range(0, _ProductMaterials.Count)].GetTexture("_MainAlbedo").name;

                    m_LeftPlatform.Init(_LeftCorrect);
                    m_RightPlatform.Init(!_LeftCorrect);
                }
                else
                {
                    m_RightProductHintText.text = _ProductMaterials[m_ProductIndex].GetTexture("_MainAlbedo").name;
                    m_ProductImage.material = _ProductMaterials[m_ProductIndex];
                    _ProductMaterials.RemoveAt(m_ProductIndex);
                    m_LeftProductHintText.text = _ProductMaterials[Random.Range(0, _ProductMaterials.Count)].GetTexture("_MainAlbedo").name;

                    m_LeftPlatform.Init(_LeftCorrect);
                    m_RightPlatform.Init(!_LeftCorrect);
                }
            }
            else
            {
                if (_LeftCorrect)
                {
                    m_LeftProductHintImage.material = _ProductMaterials[m_ProductIndex];
                    m_ProductText.text = m_LeftProductHintImage.material.GetTexture("_MainAlbedo").name;
                    _ProductMaterials.RemoveAt(m_ProductIndex);
                    m_RightProductHintImage.material = _ProductMaterials[Random.Range(0, _ProductMaterials.Count)];

                    m_LeftPlatform.Init(_LeftCorrect);
                    m_RightPlatform.Init(!_LeftCorrect);
                }
                else
                {
                    m_RightProductHintImage.material = _ProductMaterials[m_ProductIndex];
                    m_ProductText.text = m_RightProductHintImage.material.GetTexture("_MainAlbedo").name;
                    _ProductMaterials.RemoveAt(m_ProductIndex);
                    m_LeftProductHintImage.material = _ProductMaterials[Random.Range(0, _ProductMaterials.Count)];

                    m_LeftPlatform.Init(_LeftCorrect);
                    m_RightPlatform.Init(!_LeftCorrect);
                }
            }

            return this;
        }

        public void TogglePuzzle(bool a_State)
        {
            if (a_State)
            {
                m_AudioSource.PlayOneShot(m_EnableSFX);
            }

            if (m_ImageHint)
            {
                m_ProductImage.gameObject.SetActive(a_State);
                m_LeftProductHintText.gameObject.SetActive(a_State);
                m_RightProductHintText.gameObject.SetActive(a_State);
            }
            else
            {
                m_ProductText.gameObject.SetActive(a_State);
                m_LeftProductHintImage.gameObject.SetActive(a_State);
                m_RightProductHintImage.gameObject.SetActive(a_State);
            }
        }
    }
}
