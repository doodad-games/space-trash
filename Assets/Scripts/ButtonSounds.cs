using MyLibrary;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSounds : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData) =>
        SoundController.Play("ui-hover");

    public void OnPointerClick(PointerEventData eventData) =>
        SoundController.Play("ui-click");
}
