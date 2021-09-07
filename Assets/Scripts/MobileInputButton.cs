using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileInputButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public static bool active { get; private set; }
    public static IReadOnlyCollection<MobileInputButton> ActiveInputs => _activeInputs;
    static HashSet<MobileInputButton> _activeInputs = new HashSet<MobileInputButton>();

    public Vector2Int Direction => _direction;

#pragma warning disable CS0649
    [SerializeField] Vector2Int _direction;
    [SerializeField] GameObject _enableWhenActive;
#pragma warning restore CS0649

    int _pointerId;
    bool _isActive;

    void Awake() => active = true;
    void OnEnable() =>
        Refresh();
    void OnDisable()
    {
        _isActive = false;
        Refresh();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_isActive)
            return;
        _isActive = true;
        _pointerId = eventData.pointerId;
        Refresh();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (
            _isActive ||
            eventData.pointerId == -1 ||
            !Input.GetMouseButton(eventData.pointerId)
        ) return;
        _isActive = true;
        _pointerId = eventData.pointerId;
        Refresh();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_isActive || eventData.pointerId != _pointerId)
            return;
        _isActive = false;
        Refresh();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isActive || eventData.pointerId != _pointerId)
            return;
        _isActive = false;
        Refresh();
    }

    void Refresh()
    {
        _enableWhenActive.SetActive(_isActive);

        if (_isActive)
            _activeInputs.Add(this);
        else
            _activeInputs.Remove(this);
    }
}
