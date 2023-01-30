using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Barracuda;
using UnityEngine;

public class MNISTTestMain : MonoBehaviour
{
    [SerializeField] private NNModel modelAsset;
    
    // 入力画像。
    [SerializeField] private Texture2D inputTex;

    void Start()
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
        // モデルをランタイム用に変換
        Model runtimeModel = ModelLoader.Load(modelAsset);
        // モデルを動かすためのワーカーを作成
        using IWorker worker = WorkerFactory.CreateWorker(runtimeModel);
        // 推論を実行
        worker.Execute(input);

        ////////////////////
        // 結果を取得する
        ////////////////////
        Tensor output = worker.PeekOutput();
        var results = Enumerable.Range(0, 10).Select(x => output[0, 0, 0, x]).SoftMax().ToArray();
        for (int i = 0; i < results.Length; i++)
        {
            Debug.Log($"{i} : {results[i]}");
        }
    }
}

public static class SoftmaxExtension
{
    public static IEnumerable<float> SoftMax(this IEnumerable<float> source)
    {
        var exp = source.Select(x => Mathf.Exp(x)).ToArray();
        var sum = exp.Sum();
        return exp.Select(x => x / sum);
    }
}

