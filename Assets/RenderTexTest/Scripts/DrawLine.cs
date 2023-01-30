using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class DrawLine : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRendererPrefab;

    private LineRenderer _lineRenderer;

    private int _index;
    
    private Camera _mainCamera;

    private readonly Subject<Unit> _drawLineSubject = new();
    public IObservable<Unit> OnDrawLineAsObservable() => _drawLineSubject;

    private readonly List<LineRenderer> _lineRenderers = new();

    private void Start()
    {
        _mainCamera = Camera.main;
        // perspectiveではなくorthographicにする
        _mainCamera.orthographic = true;
    }

    private void OnDestroy()
    {
        _drawLineSubject.Dispose();
        _lineRenderers.Clear();
    }

    private void Update()
    {
        // UI上だったら何もしない
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            _lineRenderer = GameObject.Instantiate(lineRendererPrefab);
            _lineRenderers.Add(_lineRenderer);
            _index = 0;
            return;
        }

        if (Input.GetMouseButton(0))
        {
            // スクリーン座標が返ってくるので、
            var screenPos = Input.mousePosition;
            // ワールド座標に変換する。
            var worldPos = _mainCamera.ScreenToWorldPoint(screenPos);
            // カメラに映るようにz座標を調整
            worldPos.z = 0;
            
            _index++;
            // 手動でpositionCountを更新しなければOut of indexになる。
            _lineRenderer.positionCount = _index;
            _lineRenderer.SetPosition(_index - 1, worldPos);
        }

        if (Input.GetMouseButtonUp(0))
        {
            _drawLineSubject.OnNext(Unit.Default);
            _index = 0;
        }
    }

    public void Clear()
    {
        foreach (var x in _lineRenderers)
        {
            Destroy(x.gameObject);
        }
        _lineRenderers.Clear();
    }
}
