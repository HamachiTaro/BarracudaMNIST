using System;
using UniRx;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace RenderTexTest.Scripts
{
    public class ClearController : MonoBehaviour
    {
        [SerializeField] private Button clearButton;

        public IObservable<Unit> OnClearAsObservable() => clearButton.OnClickAsObservable();
    }
}