using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class SelectableSound : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IPointerClickHandler, ISubmitHandler
{
    private static AudioSource s_audioSource;

    [SerializeField]
    private SelectableSoundPreset soundPreset;


    void Awake()
    {
        if (!s_audioSource)
        {
            GameObject gameObject = new GameObject("UI AudioSource");
            s_audioSource = gameObject.AddComponent<AudioSource>();
            s_audioSource.playOnAwake = false;
        }
    }

    public void OnSelect(BaseEventData eventData) => s_audioSource.PlayOneShot(soundPreset.OnPointerEnter, soundPreset.OnPointerEnterVolume);
    public void OnDeselect(BaseEventData eventData) => s_audioSource.PlayOneShot(soundPreset.OnPointerExit, soundPreset.OnPointerExitVolume);

    public void OnPointerEnter(PointerEventData eventData) => s_audioSource.PlayOneShot(soundPreset.OnSelect, soundPreset.OnSelectVolume);
    public void OnPointerExit(PointerEventData eventData) => s_audioSource.PlayOneShot(soundPreset.OnDeselect, soundPreset.OnDeselectVolume);

    public void OnPointerClick(PointerEventData eventData) => s_audioSource.PlayOneShot(soundPreset.OnSubmit, soundPreset.OnSubmitVolume);
    public void OnSubmit(BaseEventData eventData) => s_audioSource.PlayOneShot(soundPreset.OnSubmit, soundPreset.OnSubmitVolume);
}
