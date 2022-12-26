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

    // TODO: apply sound effect volume
    public void OnSelect(BaseEventData eventData)
    {
        if (soundPreset.OnPointerEnter)
            s_audioSource.PlayOneShot(soundPreset.OnPointerEnter, soundPreset.OnPointerEnterVolume * AudioVolumeControl.ins.SFXVolumeSliderValue);
    }
    public void OnDeselect(BaseEventData eventData)
    {
        if (soundPreset.OnPointerExit)
            s_audioSource.PlayOneShot(soundPreset.OnPointerExit, soundPreset.OnPointerExitVolume * AudioVolumeControl.ins.SFXVolumeSliderValue);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (soundPreset.OnSelect)
            s_audioSource.PlayOneShot(soundPreset.OnSelect, soundPreset.OnSelectVolume * AudioVolumeControl.ins.SFXVolumeSliderValue);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (soundPreset.OnDeselect)
            s_audioSource.PlayOneShot(soundPreset.OnDeselect, soundPreset.OnDeselectVolume * AudioVolumeControl.ins.SFXVolumeSliderValue);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (soundPreset.OnSubmit)
            s_audioSource.PlayOneShot(soundPreset.OnSubmit, soundPreset.OnSubmitVolume * AudioVolumeControl.ins.SFXVolumeSliderValue);
    }
    public void OnSubmit(BaseEventData eventData)
    {
        if (soundPreset.OnSubmit)
            s_audioSource.PlayOneShot(soundPreset.OnSubmit, soundPreset.OnSubmitVolume * AudioVolumeControl.ins.SFXVolumeSliderValue);
    }
}
