using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

namespace TPSBR.UI
{
    public class UIHitDamageIndicator : MonoBehaviour
    {
        [SerializeField]
        private GameObject _normalHitTextPrefab;
        [SerializeField]
        private GameObject _criticalHitTextPrefab;
        [SerializeField]
        private GameObject _fatalHitTextPrefab;
        [SerializeField]
        private GameObject _comboTextPrefab;
        [SerializeField]
        private GameObject _fatalExperienceTextPrefab; // Prefab for fatal hit experience text

        private List<GameObject> _activeTexts = new List<GameObject>();
        private List<GameObject> _inactiveTexts = new List<GameObject>();
        private List<GameObject> _activeComboTexts = new List<GameObject>();
        private List<GameObject> _inactiveComboTexts = new List<GameObject>();
        private List<GameObject> _activeFatalExperienceTexts = new List<GameObject>();
        private List<GameObject> _inactiveFatalExperienceTexts = new List<GameObject>();

        private int _comboCount = 0;
        private float _comboTimer = 0f;
        private const float COMBO_INTERVAL = 2f;

        private RectTransform _canvasRectTransform;

        private void Start()
        {
            _canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        }

        public void HitPerformed(HitData hitData)
        {
            SpawnHitText(hitData);
            if (hitData.IsFatal)
            {
                SpawnFatalExperienceText(hitData);
            }
            UpdateComboCounter();
        }

        private void SpawnHitText(HitData hitData)
        {
            GameObject prefab = GetHitTextPrefab(hitData);
            GameObject hitText = _inactiveTexts.Count > 0 ? _inactiveTexts[_inactiveTexts.Count - 1] : null;

            if (hitText == null)
            {
                hitText = Instantiate(prefab, transform);
            }
            else
            {
                _inactiveTexts.RemoveAt(_inactiveTexts.Count - 1);
                if (hitText.name != prefab.name)
                {
                    Destroy(hitText);
                    hitText = Instantiate(prefab, transform);
                }
            }

            _activeTexts.Add(hitText);

            var textMeshPro = hitText.GetComponentInChildren<TextMeshProUGUI>();
            textMeshPro.text = hitData.Amount.ToString("F0");

            RectTransform hitTextRectTransform = hitText.GetComponent<RectTransform>();
            hitTextRectTransform.anchoredPosition = new Vector2(0, 0);

            hitText.transform.localScale = Vector3.one;
            hitText.SetActive(true);

            CanvasGroup canvasGroup = hitText.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = hitText.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 1f;

            AnimateHitText(hitText);
        }

        private GameObject GetHitTextPrefab(HitData hitData)
        {
            if (hitData.IsFatal)
            {
                return _fatalHitTextPrefab;
            }
            else if (hitData.IsCritical)
            {
                return _criticalHitTextPrefab;
            }
            else
            {
                return _normalHitTextPrefab;
            }
        }

        private void AnimateHitText(GameObject hitText)
        {
            CanvasGroup canvasGroup = hitText.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = hitText.AddComponent<CanvasGroup>();
            }

            Vector3 randomOffset = new Vector3(Random.Range(-150f, 150f), Random.Range(-50f, 50f), 0);

            hitText.transform.DOLocalMove(randomOffset, 0.5f).SetRelative();
            hitText.transform.DOScale(1.2f, 0.5f).OnComplete(() =>
            {
                hitText.transform.DOScale(0f, 0.5f);
                canvasGroup.DOFade(0f, 0.5f).OnComplete(() =>
                {
                    hitText.SetActive(false);
                    RecycleHitText(hitText);
                });
            });
        }

        private void RecycleHitText(GameObject hitText)
        {
            _activeTexts.Remove(hitText);
            _inactiveTexts.Add(hitText);
        }

        private void UpdateComboCounter()
        {
            _comboCount++;
            _comboTimer = 0f;

            if (_comboCount > 1)
            {
                SpawnComboText();
            }
        }

        private void SpawnComboText()
        {
            GameObject comboText = _inactiveComboTexts.Count > 0 ? _inactiveComboTexts[_inactiveComboTexts.Count - 1] : null;

            if (comboText == null)
            {
                comboText = Instantiate(_comboTextPrefab, transform);
            }
            else
            {
                _inactiveComboTexts.RemoveAt(_inactiveComboTexts.Count - 1);
            }

            _activeComboTexts.Add(comboText);

            var textMeshPro = comboText.GetComponentInChildren<TextMeshProUGUI>();
            textMeshPro.text = $"+{_comboCount} hits combo";

            RectTransform comboTextRectTransform = comboText.GetComponent<RectTransform>();
            comboTextRectTransform.anchoredPosition = new Vector2(_canvasRectTransform.rect.width / 2 - 200, -100);

            comboText.transform.localScale = Vector3.one;
            comboText.SetActive(true);

            CanvasGroup canvasGroup = comboText.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = comboText.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 1f;

            AnimateComboText(comboText);
        }

        private void AnimateComboText(GameObject comboText)
        {
            CanvasGroup canvasGroup = comboText.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = comboText.AddComponent<CanvasGroup>();
            }

            float moveUpDistance = 60f;

            comboText.transform.DOLocalMoveY(comboText.transform.localPosition.y + moveUpDistance, 0.5f).SetEase(Ease.OutBack);
            comboText.transform.DOPunchScale(Vector3.one * 0.3f, 0.5f, 1, 1);
            canvasGroup.DOFade(0f, 0.3f).SetDelay(.5f).OnComplete(() =>
            {
                comboText.SetActive(false);
                RecycleComboText(comboText);
            });
        }

        private void RecycleComboText(GameObject comboText)
        {
            _activeComboTexts.Remove(comboText);
            _inactiveComboTexts.Add(comboText);
        }

        private void SpawnFatalExperienceText(HitData hitData)
        {
            GameObject fatalExperienceText = _inactiveFatalExperienceTexts.Count > 0 ? _inactiveFatalExperienceTexts[_inactiveFatalExperienceTexts.Count - 1] : null;

            if (fatalExperienceText == null)
            {
                fatalExperienceText = Instantiate(_fatalExperienceTextPrefab, transform);
            }
            else
            {
                _inactiveFatalExperienceTexts.RemoveAt(_inactiveFatalExperienceTexts.Count - 1);
            }

            _activeFatalExperienceTexts.Add(fatalExperienceText);

            var textMeshPro = fatalExperienceText.GetComponentInChildren<TextMeshProUGUI>();
            textMeshPro.text = $"+{hitData.Points} extra points";

            RectTransform fatalExperienceTextRectTransform = fatalExperienceText.GetComponent<RectTransform>();
            fatalExperienceTextRectTransform.anchoredPosition = new Vector2(0, 0);

            fatalExperienceText.transform.localScale = Vector3.one;
            fatalExperienceText.SetActive(true);

            CanvasGroup canvasGroup = fatalExperienceText.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = fatalExperienceText.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 1f;

            AnimateFatalExperienceText(fatalExperienceText);
        }

        private void AnimateFatalExperienceText(GameObject fatalExperienceText)
        {
            CanvasGroup canvasGroup = fatalExperienceText.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = fatalExperienceText.AddComponent<CanvasGroup>();
            }

            Vector3 randomOffset = new Vector3(Random.Range(0f, 250f), Random.Range(50f, 150f), 0);

            fatalExperienceText.transform.DOLocalMove(randomOffset, 0.5f).SetRelative();
            fatalExperienceText.transform.DOScale(1.2f, 0.5f).OnComplete(() =>
            {
                fatalExperienceText.transform.DOScale(1f, 0.5f);
                canvasGroup.DOFade(0f, 2.5f).OnComplete(() =>
                {
                    fatalExperienceText.SetActive(false);
                    RecycleFatalExperienceText(fatalExperienceText);
                });
            });
        }

        private void RecycleFatalExperienceText(GameObject fatalExperienceText)
        {
            _activeFatalExperienceTexts.Remove(fatalExperienceText);
            _inactiveFatalExperienceTexts.Add(fatalExperienceText);
        }

        private void Update()
        {
            if (_comboCount > 0)
            {
                _comboTimer += Time.deltaTime;

                if (_comboTimer > COMBO_INTERVAL)
                {
                    _comboCount = 0;
                }
            }
        }
    }
}
