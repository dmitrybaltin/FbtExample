using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Tools.FPSDisplay
{
    public class FPSDisplay : MonoBehaviour
    {
        [Tooltip("Text element to display FPS value.")]
        public TMP_Text fpsText;

        [Tooltip("Time constant for exponential smoothing (in seconds).")]
        public float timeConstant = 0.5f;

        private float smoothedDeltaTime;
        private readonly List<string> fpsStringCache = new();
        private const int InitialCacheSize = 500;

        private void Start()
        {
            if (fpsText == null)
                fpsText = GetComponentInChildren<TMP_Text>();

            FillFpsCache(InitialCacheSize);
        }

        private void Update()
        {
            if (fpsText == null)
                return;

            // Exponential smoothing independent of frame rate
            float alpha = 1f - Mathf.Exp(-Time.unscaledDeltaTime / timeConstant);
            smoothedDeltaTime += (Time.unscaledDeltaTime - smoothedDeltaTime) * alpha;

            if (smoothedDeltaTime > 0f)
            {
                int fps = Mathf.RoundToInt(1f / smoothedDeltaTime);
                if (fps >= fpsStringCache.Count)
                    FillFpsCache(fps + 100); // Extend cache if FPS exceeds current range

                fpsText.text = fpsStringCache[fps];
            }
        }

        /// <summary>
        /// Fills the FPS string cache up to the specified maximum FPS value.
        /// </summary>
        private void FillFpsCache(int maxFps)
        {
            int start = fpsStringCache.Count;
            for (int i = start; i <= maxFps; i++)
                fpsStringCache.Add(i.ToString("0"));
        }
    }
}