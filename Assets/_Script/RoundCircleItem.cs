using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RoundCircleItem : MonoBehaviour
{
    private RectTransform _rtf { get { return transform as RectTransform; } }
    public void _FixAngle() { transform.localEulerAngles = -transform.parent.localEulerAngles; }
    public void _SetParent(Transform parent) { transform.SetParent(parent, false); }

    [SerializeField]
    private Text _txt_Num;
    public virtual void _Show(Vector2 pos, RoundCircleItemData data)
    {
        _SetData(data);

        _rtf.anchoredPosition = pos;
        gameObject.SetActive(true);
    }
    public virtual void _SetData(RoundCircleItemData data)
    {
        _txt_Num.text = data.index.ToString();
    }
    public void _Hide()
    {
        transform.localScale = Vector3.one;
        gameObject.SetActive(false);
    }
}
