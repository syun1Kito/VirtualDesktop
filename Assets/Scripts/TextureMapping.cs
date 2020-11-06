using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[DefaultExecutionOrder(10000)] 
public class TextureMapping : MonoBehaviour
{
    [SerializeField, Range(0.0001f, 179)]
    float _fieldOfView = 60;
    //[SerializeField, Range(0.2f, 5.0f)]
    float _aspect;
    [SerializeField, Range(0.0001f, 1000.0f)]
    float _nearClipPlane = 0.01f;
    [SerializeField, Range(0.0001f, 1000.0f)]
    float _farClipPlane = 100.0f;
    //[SerializeField]
    //bool _orthographic = false;
    //[SerializeField]
    //float _orthographicSize = 1.0f;
    [SerializeField]
    Texture2D _texture;

    void Start()
    {
        _aspect = (float)_texture.width / _texture.height;
    }

    private void LateUpdate()
    {
        if (_texture == null)
        {
            return;
        }
        // V行列
        var viewMatrix = Matrix4x4.Scale(new Vector3(1, 1, -1)) * transform.worldToLocalMatrix;
        Matrix4x4 projectionMatrix;
        //if (_orthographic)
        //{
        //    var orthographicWidth = _orthographicSize * _aspect;
        //    projectionMatrix = Matrix4x4.Ortho(-orthographicWidth, orthographicWidth, -_orthographicSize, _orthographicSize, _nearClipPlane, _farClipPlane);
        //}
        //else
        //{
            var camera = GetComponent<Camera>();
            // 透視投影行列(P行列)
            projectionMatrix = Matrix4x4.Perspective(_fieldOfView, _aspect, _nearClipPlane, _farClipPlane);
        //}
        // プラットフォーム間の補正
        projectionMatrix = GL.GetGPUProjectionMatrix(projectionMatrix, true);
        // VP行列をシェーダーに渡す
        Shader.SetGlobalMatrix("_ObserverMatrixVP", projectionMatrix * viewMatrix);
        // Textureをシェーダーに渡す
        Shader.SetGlobalTexture("_ObserverTexture", _texture);
        // プロジェクターの位置を渡す
        var observerPos = Vector4.zero;
        //projectorPos = _orthographic ? transform.forward : transform.position;
        //projectorPos.w = _orthographic ? 0 : 1;
        observerPos = transform.position;
        observerPos.w = 1;
        //視点
        Shader.SetGlobalVector("_ObserverPos", observerPos);
    }

    //シーン画面上に錐体を表示
    private void OnDrawGizmos()
    {
        var gizmosMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

        //if (_orthographic)
        //{
        //    var orthographicWidth = _orthographicSize * _aspect;
        //    var length = _farClipPlane - _nearClipPlane;
        //    var start = _nearClipPlane + length / 2;
        //    Gizmos.DrawWireCube(Vector3.forward * start, new Vector3(orthographicWidth * 2, _orthographicSize * 2, length));
        //}
        //else
        //{
            Gizmos.DrawFrustum(Vector3.zero, _fieldOfView, _farClipPlane, _nearClipPlane, _aspect);
        //}

        Gizmos.matrix = gizmosMatrix;
    }
}