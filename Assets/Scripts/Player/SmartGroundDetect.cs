using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartGroundDetect : MonoBehaviour
{
    [SerializeField]
    private LayerMask groundLayers;
    [SerializeField]
    private float checkDistance;
    [SerializeField]
    private Vector3 offset;
    [SerializeField]
    private Vector2 boxSize;

    public bool IsGrounded { get; protected set; }
    public LayerMask Layers => groundLayers;

    private Vector3[] _raycastPoints;
    private bool[] _hitResult;
    private Vector3[] _hitPoints;

    void Awake()
    {
        GenerateRaycastPoints();
        _hitResult = new bool[5];
        _hitPoints = new Vector3[5];
    }

    void GenerateRaycastPoints()
    {
        _raycastPoints = new Vector3[5];
        _raycastPoints[0] = offset;
        _raycastPoints[1] = offset + new Vector3(boxSize.x / 2, 0, boxSize.y / 2);
        _raycastPoints[2] = offset + new Vector3(boxSize.x / 2, 0, -boxSize.y / 2);
        _raycastPoints[3] = offset + new Vector3(-boxSize.x / 2, 0,  -boxSize.y / 2);
        _raycastPoints[4] = offset + new Vector3(-boxSize.x / 2, 0,  boxSize.y / 2);
    }

    void Update()
    {
        Vector3 down = Vector3.down * checkDistance;
        IsGrounded = false;

        for (int i = 0; i < _raycastPoints.Length; i++)
        {
            Vector3 point = transform.TransformPoint(_raycastPoints[i]);
            if (Physics.Raycast(point, Vector3.down, out RaycastHit hit, checkDistance, groundLayers))
            {
                _hitResult[i] = true;
                _hitPoints[i] = hit.point;

                IsGrounded = true;
            }
            else
                _hitResult[i] = false;
        }
    }

    void OnValidate()
    {
        GenerateRaycastPoints();
    }

    void OnDrawGizmosSelected()
    {
        if (_raycastPoints == null)
            GenerateRaycastPoints();

        // Gizmos.DrawWireCube(transform.position + offset, new Vector3(boxSize.x, 0.01f, boxSize.y));

        for (int i = 0; i < _raycastPoints.Length; i++)
        {
            if (_hitResult != null && i < _hitResult.Length)
            {
                Gizmos.color = _hitResult[i] ? Color.red : Color.white;
            }
            else
                Gizmos.color = Color.white;

            Gizmos.DrawLine(
                transform.TransformPoint(_raycastPoints[i]),
                transform.TransformPoint(_raycastPoints[i] + Vector3.down * checkDistance));


            bool isLast = i == _raycastPoints.Length - 1;
            Gizmos.DrawLine(
                transform.TransformPoint(_raycastPoints[i]),
                transform.TransformPoint(_raycastPoints[isLast ? 0 : i + 1]));
        }
    }
}
