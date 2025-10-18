using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Examples.UnitaskFbtExample.example2a
{
    /// <summary>
    /// Displays current debug parameters on screen.
    /// Requires a LayoutGroup (e.g. VerticalLayoutGroup) to automatically arrange text entries.
    /// </summary>
    [RequireComponent(typeof(LayoutGroup))]
    public class DebugPanel : MonoBehaviour
    {
        private struct SliderEntry
        {
            public Slider SliderView;
            public TextMeshProUGUI ValueView;

            public SliderEntry(Slider sliderView, TextMeshProUGUI uiMeshProUGUI)
            {
                SliderView = sliderView;
                ValueView = uiMeshProUGUI;
            }
        }
        
        [Header("Filter")] [Tooltip("Regex to filter which categories to display (e.g., 'Npc.*|AI')")]
        public string CategoryFilterRegex = ".*";

        [Header("Text Settings")] [Tooltip("Font size for text elements. Leave 0 to use TextMeshPro default.")]
        public float FontSize = 32f;

        [Tooltip("Line spacing inside each text element.")]
        public float LineSpacing = 0f;

        [Tooltip("Optional alignment override. If null, uses LayoutGroup's childAlignment.")]
        public TextAlignmentOptions? TextAlignmentOverride = null;

        [Header("Slider Settings")] public Vector2 SliderSize = new(200, 20);

        private Regex _filter;
        private readonly Dictionary<string, TextMeshProUGUI> _labels = new();
        private readonly Dictionary<string, SliderEntry> _sliders = new();

        private LayoutGroup _layoutGroup;

        private DebugParams _params = DebugParams.Instance;

        void Start()
        {
            _filter = new Regex(CategoryFilterRegex, RegexOptions.IgnoreCase);
            _layoutGroup = GetComponent<LayoutGroup>();
        }

        void UpdateOld()
        {
            foreach (var param in _params.GetAllDisplays())
            {
                if (!_filter.IsMatch(param.Category))
                    continue;

                var key = $"{param.Category}/{param.Name}";
                if (!_labels.TryGetValue(key, out var label))
                {
                    var go = new GameObject(key);
                    go.transform.SetParent(transform, false);

                    label = go.AddComponent<TextMeshProUGUI>();
                    label.enableWordWrapping = false;

                    // Add ContentSizeFitter to make height match actual text height
                    var fitter = go.AddComponent<ContentSizeFitter>();
                    fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

                    _labels[key] = label;
                }

                label.fontSize = FontSize > 0 ? FontSize : 32f;
                label.lineSpacing = LineSpacing;
                //label.alignment = TextAlignmentOverride ?? ConvertAlignment(_layoutGroup.childAlignment);

                label.text = $"{param.Category}/{param.Name}: {param.Value}";
            }
        }

        void Update()
        {
            UpdateDisplays();
            UpdateSliders();
        }

        private void UpdateDisplays()
        {
            foreach (var param in _params.GetAllDisplays())
            {
                if (!_filter.IsMatch(param.Category))
                    continue;

                GetOrCreateLabel($"{param.Category}/{param.Name}").text = 
                    $"{param.Category}/{param.Name}: {param.Value}";
            }
        }

        private TextMeshProUGUI GetOrCreateLabel(string key)
        {
            if (_labels.TryGetValue(key, out var label))
                return label;

            var go = new GameObject(key);
            go.transform.SetParent(transform, false);

            label = go.AddComponent<TextMeshProUGUI>();
            label.enableWordWrapping = false;
            label.fontSize = FontSize > 0 ? FontSize : 32f;
            label.lineSpacing = LineSpacing;

            var fitter = go.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

            _labels[key] = label;
            return label;
        }

        private void UpdateSliders()
        {
            foreach (var control in _params.GetAllControls())
            {
                if (!_filter.IsMatch(control.Category))
                    continue;

                UpdateSlider($"{control.Category}/{control.Name}", control);
            }
        }

        private void UpdateSlider(string key, DebugControl<float> control)
        {
            var entry = GetOrCreateSliderUI(key, control.Min, control.Max, val => control.Value = val);

            // Update value every frame
            entry.SliderView.value = control.Value;
            entry.ValueView.text = $"{control.Value:F2}";
        }
        
        private SliderEntry GetOrCreateSliderUI(
            string key,
            float min,
            float max,
            UnityAction<float> callback)
        {
            if (_sliders.TryGetValue(key, out var entry))
                return entry;

            // Container
            var row = new GameObject(key);
            row.transform.SetParent(transform, false);
            
            var rowLayout = row.AddComponent<HorizontalLayoutGroup>();
            rowLayout.childAlignment = TextAnchor.MiddleLeft;
            rowLayout.childForceExpandWidth = false;
            rowLayout.childForceExpandHeight = false;
            rowLayout.spacing = 0f;
            rowLayout.padding = new RectOffset(0, 0, 0, 0); 

            var fitter = row.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            
            // Label UI
            var label = new GameObject("ValueText");
            label.transform.SetParent(row.transform, false);
            var labelText = label.AddComponent<TextMeshProUGUI>();
            labelText.fontSize = FontSize > 0 ? FontSize : 32f;
            labelText.enableWordWrapping = false;
            labelText.text = key;

            // Slider UI
            var slider = CreateSliderUI(row.transform, min, max);
            slider.onValueChanged.AddListener(callback);

            // Value UI
            var value = new GameObject("ValueText");
            value.transform.SetParent(row.transform, false);
            var valueText = value.AddComponent<TextMeshProUGUI>();
            valueText.fontSize = FontSize > 0 ? FontSize : 32f;
            valueText.enableWordWrapping = false;

            entry = new SliderEntry(slider, valueText);
            
            _sliders[key] = entry;
            
            return entry;
        }

        /// <summary>
        /// Create a Slider UI
        /// </summary>
        private Slider CreateSliderUI(Transform parent, float min, float max)
        {
            // Container
            var container = new GameObject("Slider", typeof(RectTransform));
            container.transform.SetParent(parent, false);

            var containerRect = container.GetComponent<RectTransform>();
            containerRect.sizeDelta = new Vector2(400f, 20f);
            containerRect.anchorMin = new Vector2(0f, 0.5f);
            containerRect.anchorMax = new Vector2(0f, 0.5f);
            containerRect.pivot = new Vector2(0f, 0.5f);

            // ensure LayoutGroup gives this width
            var layoutElem = container.AddComponent<LayoutElement>();
            layoutElem.preferredWidth = containerRect.sizeDelta.x;
            layoutElem.preferredHeight = containerRect.sizeDelta.y;

            // Rail UI
            var background = new GameObject("Background", typeof(RectTransform), typeof(Image));
            background.transform.SetParent(container.transform, false);
            var bgRect = background.GetComponent<RectTransform>();
            background.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            // Handle UI
            var handle = new GameObject("Handle", typeof(RectTransform), typeof(Image));
            handle.transform.SetParent(container.transform, false);
            var handleImg = handle.GetComponent<Image>();
            handleImg.color = Color.white;
            var handleRect = handle.GetComponent<RectTransform>();

            StartCoroutine(ApplyHandleSizeNextFrame(handleRect, bgRect, 0.5f)); //Set handle size on the next frame
           
            // Slider Component
            var slider = container.AddComponent<Slider>();
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = min;
            slider.wholeNumbers = false;
            slider.direction = Slider.Direction.LeftToRight;

            slider.targetGraphic = handleImg;
            slider.handleRect = handleRect;
            
            return slider;
        }

        private System.Collections.IEnumerator ApplyHandleSizeNextFrame(
            RectTransform handle, RectTransform parent, float ratio)
        {
            yield return null;
            if (handle && parent)
                handle.sizeDelta = Vector2.one * (parent.rect.height * ratio);
        }
        
    }
}
