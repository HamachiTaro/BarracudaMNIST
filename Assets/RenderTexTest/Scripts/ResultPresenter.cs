using TMPro;
using UnityEngine;

namespace RenderTexTest.Scripts
{
    public class ResultPresenter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI resultText;

        private void Start()
        {
            Clear();
        }

        public void Result(int result)
        {
            resultText.text = result.ToString();
        }

        public void Clear()
        {
            resultText.text = "";
        }
    }
}