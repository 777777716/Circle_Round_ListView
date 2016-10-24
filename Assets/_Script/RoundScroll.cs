using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoundScroll : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField]
    protected RectTransform _content;
    [SerializeField]
    private float _elasticity = 0.05f;
    [SerializeField]
    private float _decelerationRate = 0.125f;
    [SerializeField]
    protected int _stepNum = 8;
    protected float _stepValue;

    private float _nowIndex = 0;
    protected float _NowIndex
    {
        get { return _nowIndex; }
        set
        {
            _nowIndex = value;
            if (_onIndexChange != null)
            {
                _onIndexChange(value);
            }
        }
    }
    public event Action<float> _onIndexChange;
    public event Action _onFinishMove;
    public event Action _onStartCtrl;

    private float _velocity;
    private float _currentAngle;
    private float _pointInitAngle;


    protected virtual void Awake()
    {
        _stepValue = 360 / _stepNum;
        moveRoutine = Move(0);
    }
        
    public virtual void InitValue(int index = 0)
    {
        StopAllCoroutines();
        _content.eulerAngles = new Vector3(0, 0, index * _stepValue);
        _NowIndex = index;
    }
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        _velocity = 0;
        _pointInitAngle = GetNowAngle(eventData);
        _currentAngle = 0;
        StopCoroutine(moveRoutine);

        if (!_Dragging && _onStartCtrl != null) { _onStartCtrl(); }
        _Dragging = true;
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (!_Dragging) { return; }
        
        _Dragging = false;
        float deltaAngle = (_velocity > 0 ? 1 : -1) * (1 - Mathf.Abs(_velocity)) / Mathf.Log(_decelerationRate);
        float target = Mathf.Round((deltaAngle + _content.localRotation.eulerAngles.z) / _stepValue) * _stepValue;

        StopCoroutine(moveRoutine);
        moveRoutine = Move(target);
        StartCoroutine(moveRoutine);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerUp(eventData);
    }
    protected bool _Dragging;

    protected IEnumerator moveRoutine;
    protected IEnumerator AnimMove(float target)
    {
        float now = _content.localRotation.eulerAngles.z;
        while (!_Dragging && Mathf.Abs(target - now) > 4f)
        {
            now = Mathf.Lerp(now, target, _elasticity);
            AddAngle((now - _content.localRotation.eulerAngles.z) % 360);
            yield return null;
        }
        float time = 0;
        while (!_Dragging && time < 0.2f)
        {
            time += Time.deltaTime;
            AddAngle((Mathf.Lerp(now, target, time / 0.2f) - _content.localRotation.eulerAngles.z) % 360);
            yield return null;
        }
    }
    protected IEnumerator Move(float target)
    {
        yield return AnimMove(target);
        if (_onFinishMove != null)
        {
            _onFinishMove();
        }
    }
    public virtual void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (_Dragging)
        {
            float angle = GetNowAngle(eventData) - _pointInitAngle;
            if (angle < 0) { angle += 360; }
            _currentAngle = Mathf.SmoothDampAngle(_currentAngle, angle, ref _velocity, _elasticity);
            AddAngle(_velocity * Time.deltaTime);
        }
    }

    protected void AddAngle(float deltaAngle)
    {
        _NowIndex += deltaAngle / _stepValue;
        _content.localRotation = Quaternion.Euler(0, 0, _content.localRotation.eulerAngles.z + deltaAngle);
    }

    private float GetNowAngle(PointerEventData eventData)
    {
        Vector3 worldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(_content, eventData.position, eventData.pressEventCamera, out worldPos);
        Vector2 localPos = (worldPos - _content.position).normalized;
        float angle = Mathf.Acos(localPos.x) * Mathf.Rad2Deg;
        if (localPos.y < 0)
        {
            angle = 360 - angle;
        }
        return angle;
    }

}
