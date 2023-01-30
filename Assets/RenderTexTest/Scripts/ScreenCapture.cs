using Cysharp.Threading.Tasks;
using UnityEngine;

public class ScreenCapture : MonoBehaviour
{
    [SerializeField] private Transform drawArea;

    private Rect _captureRect;

    private void Start()
    {
        var pos = drawArea.position;
        var size = drawArea.GetComponent<Renderer>().bounds.size;
        var leftBottom = pos - new Vector3(size.x * 0.5f, size.y * 0.5f, 0f);
        var rightTop = pos + new Vector3(size.x * 0.5f, size.y * 0.5f, 0f);

        leftBottom = Camera.main.WorldToScreenPoint(leftBottom);
        rightTop = Camera.main.WorldToScreenPoint(rightTop);

        _captureRect = new Rect(leftBottom, rightTop - leftBottom);
    }

    public async UniTask<Texture2D> TakeCapture()
    {
        // MonoBehaviourを渡すとコルーチンと同じようにWaitForEndOfFrameが動くらしい。
        await UniTask.WaitForEndOfFrame(this);

        var capture = new Texture2D((int)_captureRect.size.x, (int)_captureRect.size.y);
        capture.ReadPixels(_captureRect, 0, 0);
        capture.Apply();

        return capture;
    }
}