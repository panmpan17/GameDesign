using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MPack;
using TMPro;

public class PlayerStatusHUD : MonoBehaviour
{
    [Header("Aim")]
    [SerializeField]
    private Image aimPiece1;
    [SerializeField]
    private Image aimPiece2;
    [SerializeField]
    private Image aimPiece3;
    [SerializeField]
    private float pieceMoveAmount;
    private Vector2 _aimPiecePosition1, _aimPiecePosition2, _aimPiecePosition3;
    private Vector2 _aimPiecePositionTo1, _aimPiecePositionTo2, _aimPiecePositionTo3;

    [SerializeField]
    private Color transparent = new Color(1, 1, 1, 0.1f);
    [SerializeField]
    private Color beforeFull = new Color(1, 1, 1, 0.7f);
    [SerializeField]
    private Color full = new Color(1, 1, 1, 1f);

    [SerializeField]
    private EventReference aimProgressEvent;

    [Header("Inventory")]
    [SerializeField]
    private EventReference inventoryChangeEvent;
    [SerializeField]
    private TextMeshProUGUI itemCountText;

    void Awake()
    {
        aimPiece1.color = transparent;
        aimPiece2.color = transparent;
        aimPiece3.color = transparent;
        _aimPiecePosition1 = aimPiece1.rectTransform.anchoredPosition;
        _aimPiecePosition2 = aimPiece2.rectTransform.anchoredPosition;
        _aimPiecePosition3 = aimPiece3.rectTransform.anchoredPosition;
        _aimPiecePositionTo1 = Vector2.MoveTowards(_aimPiecePosition1, Vector2.zero, pieceMoveAmount);
        _aimPiecePositionTo2 = Vector2.MoveTowards(_aimPiecePosition2, Vector2.zero, pieceMoveAmount);
        _aimPiecePositionTo3 = Vector2.MoveTowards(_aimPiecePosition3, Vector2.zero, pieceMoveAmount);

        inventoryChangeEvent.InvokeIntEvents += ChangeInventory;
        aimProgressEvent.InvokeFloatEvents += ChangeAimProgress;
    }

    void OnDestroy()
    {
        inventoryChangeEvent.InvokeIntEvents -= ChangeInventory;
    }

    void ChangeInventory(int count)
    {
        itemCountText.text = count.ToString();
    }

    void ChangeAimProgress(float progress)
    {
        Color color = progress >= 0.99f ? full : Color.Lerp(transparent, beforeFull, progress);
        aimPiece1.color = color;
        aimPiece2.color = color;
        aimPiece3.color = color;

        aimPiece1.rectTransform.anchoredPosition = Vector2.Lerp(_aimPiecePosition1, _aimPiecePositionTo1, progress);
        aimPiece2.rectTransform.anchoredPosition = Vector2.Lerp(_aimPiecePosition2, _aimPiecePositionTo2, progress);
        aimPiece3.rectTransform.anchoredPosition = Vector2.Lerp(_aimPiecePosition3, _aimPiecePositionTo3, progress);
    }
}
