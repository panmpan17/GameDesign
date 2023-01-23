using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


public class MinimapControl : MonoBehaviour
{
    public static MinimapControl ins {
        get
        {
            if (!_ins)
                _ins = GameObject.Find("MiniMap").GetComponent<MinimapControl>();
            return _ins;
        }
    }
    private static MinimapControl _ins;

    [SerializeField]
    private EventReference enlargeEvent;

    [Header("Camera")]
    [SerializeField]
    private new Camera camera;
    [SerializeField]
    private float enlargeOrthographicSize;
    private float _smallOrthographicSize;

    [Header("Marker")]
    [SerializeField]
    private SpriteRenderer iconPrefab;
    private List<SpriteRenderer> _markers = new List<SpriteRenderer>(10);

    void Awake()
    {
        _ins = this;

        _smallOrthographicSize = camera.orthographicSize;

        enlargeEvent.InvokeBoolEvents += OnEnlarge;
    }

    void OnEnlarge(bool isEnlarge)
    {
        camera.orthographicSize = isEnlarge ? enlargeOrthographicSize : _smallOrthographicSize;
    }

    public void Register(Vector3 position, MinimapMarker marker)
    {
        SpriteRenderer newSpriteRenderer = Instantiate<SpriteRenderer>(iconPrefab, transform);
        newSpriteRenderer.sprite = marker.Icon;
        newSpriteRenderer.transform.localScale = new Vector3(marker.Scale, marker.Scale, marker.Scale);

        position.y = newSpriteRenderer.transform.position.y;
        newSpriteRenderer.transform.position = position;

        newSpriteRenderer.gameObject.SetActive(true);

        _markers.Add(newSpriteRenderer);
    }

    public void UnregisterMarker(MinimapMarker marker)
    {
        foreach (SpriteRenderer spriteRenderer in _markers)
        {
            if (spriteRenderer.sprite == marker.Icon)
            {
                Destroy(spriteRenderer.gameObject);
                return;
            }
        }
    }

    void OnDestroy()
    {
        enlargeEvent.InvokeBoolEvents -= OnEnlarge;
    }
}
