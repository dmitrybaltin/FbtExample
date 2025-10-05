using TMPro;
using UnityEngine;

namespace Tools.DebugUI
{
    public class DebugParamRow : MonoBehaviour
    {
        [SerializeField] private TMP_Text keyText;
        [SerializeField] private TMP_Text valueText;

        public void Set(string key, string value)
        {
            keyText.text = key;
            valueText.text = value;
        }
    }
}