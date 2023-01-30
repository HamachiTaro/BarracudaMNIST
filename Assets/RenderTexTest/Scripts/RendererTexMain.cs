using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;
using Unity.Barracuda;

namespace RenderTexTest.Scripts
{
    public class RendererTexMain : MonoBehaviour
    {
        /// <summary>
        /// .onnxモデルアセット
        /// </summary>
        [SerializeField] private NNModel modelAsset;
        /// <summary>
        /// 線を引くコンポーネント
        /// </summary>
        [SerializeField] private DrawLine drawLine;
        /// <summary>
        /// キャプチャを撮るコンポーネント
        /// </summary>
        [SerializeField] private ScreenCapture screenCapture;
        /// <summary>
        /// 結果秒時
        /// </summary>
        [SerializeField] private ResultPresenter resultPresenter;
        /// <summary>
        /// クリア
        /// </summary>
        [SerializeField] private ClearController clearController;
        
        /// <summary>
        /// 推論
        /// </summary>
        private MnistTest _mnist;

        private Texture2D _capture;
        
        private void Start()
        {
            _mnist = new MnistTest(modelAsset);
            
            drawLine.OnDrawLineAsObservable()
                .Subscribe(_ => { TakeCapture().Forget(); })
                .AddTo(gameObject);

            clearController.OnClearAsObservable()
                .Subscribe(_ =>
                {
                    Debug.Log("クリア");
                    drawLine.Clear();
                    resultPresenter.Clear();
                    Destroy(_capture);
                })
                .AddTo(gameObject);
        }

        private async UniTaskVoid TakeCapture()
        {
            Debug.Log("推論開始");
            _capture = await screenCapture.TakeCapture();
            var result = _mnist.Execute(_capture);
            resultPresenter.Result(result);
        }
    }
}