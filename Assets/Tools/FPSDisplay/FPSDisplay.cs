using TMPro;
using UnityEngine;

namespace Tools.FPSDisplay
{
	public class FPSDisplay : MonoBehaviour
	{
		public TMP_Text fpsText;
		public float smoothingСoefficient = 0.1f;

		private float deltaTime;

		private void Start()
		{
			if (fpsText is null)
				fpsText = GetComponentInChildren<TMP_Text>();
		}
    
		private void Update()
		{
			if (fpsText == null) 
				return;

			deltaTime += (Time.unscaledDeltaTime - deltaTime) * smoothingСoefficient;
			if (deltaTime != 0)
				fpsText.text = (1.0f / deltaTime).ToString("0");
		}
	}
}