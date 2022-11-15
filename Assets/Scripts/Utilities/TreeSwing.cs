using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class TreeSwing : MonoBehaviour
{
    [SerializeField]
    private float noiseScale;
    [SerializeField]
    private Vector3 rotateAmount;
    [SerializeField]
    private float noiseStrength;
    [SerializeField]
    private Vector2 moveSpeed;
    private Vector2 offset;
    private Vector3 originEuler;

    void Awake()
    {
        originEuler = transform.rotation.eulerAngles;
    }

    void Update()
    {
        offset += moveSpeed * Time.deltaTime;

        Vector3 position = transform.position;
        float value = Mathf.PerlinNoise((position.x + offset.x) * noiseScale, (position.z + offset.y) * noiseScale);

        transform.rotation = Quaternion.Euler(rotateAmount * value + originEuler);
        //  = noiseMoveSpeed * value;
        // Vector3 point = new Vector3(position.x + xMoveAmount * value, height, position.y + yMoveAmount * value);
        
    }
}
