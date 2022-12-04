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
    private FillBarControl aimProgressBar;
    [SerializeField]
    private EventReference aimProgressEvent;

    [HideInInspector, SerializeField] private Image aimPiece1, aimPiece2, aimPiece3;
    [HideInInspector, SerializeField] private float pieceMoveAmount;
    private Vector2 _aimPiecePosition1, _aimPiecePosition2, _aimPiecePosition3;
    private Vector2 _aimPiecePositionTo1, _aimPiecePositionTo2, _aimPiecePositionTo3;
    [HideInInspector, SerializeField]
    private Color transparent = new Color(1, 1, 1, 0.1f), beforeFull = new Color(1, 1, 1, 0.7f), full = new Color(1, 1, 1, 1f), extraBeforeFull = new Color(1, 1, 1, 1f), extrafull = new Color(1, 1, 1, 1f);

    [Header("Inventory")]
    [SerializeField]
    private EventReference inventoryChangeEvent;
    [SerializeField]
    private TextMeshProUGUI itemCountText;

    void Awake()
    {
        aimProgressBar.SetFillAmount(0);

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
        aimProgressEvent.InvokeFloatEvents -= ChangeAimProgress;
    }

    void ChangeInventory(int count)
    {
        itemCountText.text = count.ToString();
    }

    void ChangeAimProgress(float progress)
    {
        aimProgressBar.SetFillAmount(progress / 2);
    }

    private void UpdateOldHUD(float progress)
    {
        Color color;

        if (progress <= 0.99f)
            color = Color.Lerp(transparent, beforeFull, progress);
        else if (progress <= 1.99f)
            color = Color.Lerp(full, extraBeforeFull, Mathf.Max(progress - 1, 0));
        else
            color = extrafull;

        aimPiece1.color = color;
        aimPiece2.color = color;
        aimPiece3.color = color;

        aimPiece1.rectTransform.anchoredPosition = Vector2.Lerp(_aimPiecePosition1, _aimPiecePositionTo1, progress / 2);
        aimPiece2.rectTransform.anchoredPosition = Vector2.Lerp(_aimPiecePosition2, _aimPiecePositionTo2, progress / 2);
        aimPiece3.rectTransform.anchoredPosition = Vector2.Lerp(_aimPiecePosition3, _aimPiecePositionTo3, progress / 2);
    }
}
