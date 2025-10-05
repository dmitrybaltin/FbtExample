using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tools.DebugUI
{
    public class DebugOverlay : MonoBehaviour
    {
        [SerializeField] private DebugParamRow rowPrefab; // можно задать, можно нет
        private Transform _container;

        private readonly Dictionary<string, TMP_Text> _rows = new();
        private readonly Dictionary<string, string> _cachedValues = new();
        private bool _visible = true;

        public static DebugOverlay Instance { get; private set; }

        void Start()
        {
            if (Instance is not null)
            {
                Destroy(gameObject);
                return;
            }
            
            _container = transform;

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
                ToggleVisibility();
        }

        /// <summary>
        /// Устанавливает значение параметра.
        /// Обновление происходит только если значение реально изменилось.
        /// </summary>
        public void SetParam(string key, object? value)
        {
            var str = value?.ToString() ?? "null";

            if (_cachedValues.TryGetValue(key, out var old) && old == str)
                return;

            _cachedValues[key] = str;

            if (!_rows.TryGetValue(key, out var valueText))
                valueText = CreateRow(key);
            
            valueText.text = str;
        }
        
        private TMP_Text CreateRow(string key)
        {
            if (rowPrefab != null)
            {
                var row = Instantiate(rowPrefab, _container);
                row.Set(key, "");
                
                var newRow = row.GetComponentInChildren<TMP_Text>();
                
                _rows[key] = newRow;
            
                return newRow;
            }

            // Динамически создаём строку
            var rowObj = new GameObject(key, typeof(RectTransform));
            rowObj.transform.SetParent(_container, false);

            var layout = rowObj.AddComponent<HorizontalLayoutGroup>();
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.spacing = 5;

            // Ключ
            var keyObj = new GameObject("Key", typeof(RectTransform));
            keyObj.transform.SetParent(rowObj.transform, false);
            var keyText = keyObj.AddComponent<TextMeshProUGUI>();
            keyText.text = key + ": ";

            // Значение
            var valObj = new GameObject("Value", typeof(RectTransform));
            valObj.transform.SetParent(rowObj.transform, false);
            var valueText = valObj.AddComponent<TextMeshProUGUI>();

            _rows[key] = valueText;
            
            return valueText;
        }

        private void ToggleVisibility()
        {
            _visible = !_visible;
            _container.gameObject.SetActive(_visible);
        }
    }
}
