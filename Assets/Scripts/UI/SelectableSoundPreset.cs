using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[CreateAssetMenu(menuName="Game/Selectable Sound Preset")]
public class SelectableSoundPreset : ScriptableObject
{
    public AudioClip OnPointerEnter;
    [Range(0, 1)]
    public float OnPointerEnterVolume;
    public AudioClip OnPointerExit;
    [Range(0, 1)]
    public float OnPointerExitVolume;

    [Space(5)]
    public AudioClip OnSelect;
    [Range(0, 1)]
    public float OnSelectVolume;
    public AudioClip OnDeselect;
    [Range(0, 1)]
    public float OnDeselectVolume;

    [Space(5)]
    public AudioClip OnSubmit;
    [Range(0, 1)]
    public float OnSubmitVolume;
}
