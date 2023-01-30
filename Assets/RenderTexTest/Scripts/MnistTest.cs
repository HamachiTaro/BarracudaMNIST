using System;
using System.Linq;
using Unity.Barracuda;
using UnityEngine;

namespace RenderTexTest.Scripts
{
    public class MnistTest
    {
        private readonly Model _runtimeModel;

        public MnistTest(NNModel modelAsset)
        {
            _runtimeModel = ModelLoader.Load(modelAsset);
        }

        /// <summary>
        /// 推論実行。
        /// </summary>
        public int Execute(Texture2D inputTex)
        {
            var inputWidth = inputTex.width;
            var inputHeight = inputTex.height;

            ////////////////////
            // 入力用テンソルを作成
            ////////////////////
            using var input = new Tensor(1, 28, 28, 1);
            for (int y = 0; y < 28; y++)
            {
                for (int x = 0; x < 28; x++)
                {
                    // 機械学習では左上を原点とする一方、unityは左下を原点とする。
                    // モデルに揃えるためにy座標を反転している。
                    input[0, 27 - y, x, 0] = inputTex.GetPixel(x * inputWidth / 28, y * inputHeight / 28).grayscale;
                }
            }

            ////////////////////
            // モデルを実行する
            ////////////////////
            // モデルを動かすためのワーカーを作成
            using IWorker worker = WorkerFactory.CreateWorker(_runtimeModel);
            // 推論を実行
            worker.Execute(input);

            ////////////////////
            // 結果を取得する
            ////////////////////
            Tensor output = worker.PeekOutput();

            // 結果を加工
            var results = Enumerable.Range(0, 10).Select(x => output[0, 0, 0, x]).SoftMax().ToArray();
            return Array.IndexOf(results, results.Max());
        }
    }
}