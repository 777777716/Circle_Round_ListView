using UnityEngine;
using System.Collections.Generic;

public class RoundCircleList : RoundScroll
{
    [SerializeField]
    private GameObject item_prefab;
    [SerializeField]
    private float _radius;
    [SerializeField]
    protected int _leftRange = 0;
    [SerializeField]
    protected int _rightRange = 0;

    protected RoundCircleItemData[] _data;
    protected int _GetDataIndex(int index)
    {
        index %= _data.Length;
        if (index < 0) { index += _data.Length; }
        return index;
    }

    private Queue<RoundCircleItem> _pool = new Queue<RoundCircleItem>();

    protected int _left = 0;
    protected RoundCircleItem[] _items;
    protected int _GetItemIndex(int index)
    {
        index %= _items.Length;
        if (index < 0) { index += _items.Length; }
        return index;
    }
    private bool _hasCreate = false;
    private void _Create()
    {
        if (_hasCreate) { return; }
        _hasCreate = true;
        //创建list
        if (_items == null) { _CreateItems(); }

        //创建预制
        if (_pool.Count == 0) { _CreatePrefabs(); }

        _onIndexChange += _OnValueChange;
    }

    private void _CreatePrefabs()
    {
        //创建足够的prefabs
        int num = _leftRange + _rightRange + 3;
        for (int i = 0; i < num; i++)
        {
            RoundCircleItem item = GameObject.Instantiate(item_prefab).GetComponent<RoundCircleItem>();
            item._SetParent(_content);
            item._Hide();
            _pool.Enqueue(item);
        }
    }
    private void _CreateItems()
    {
        _items = new RoundCircleItem[_stepNum];
        _items.Initialize();
    }

    public void _SetData(RoundCircleItemData[] datas)
    {
        //设置数据
        _data = datas;

        InitValue();
    }

    public override void InitValue(int index = 0)
    {
        _Create();
        //还原队列
        for (int i = 0; i < _items.Length; i++)
        {
            if (_items[i] == null) { continue; }
            _items[i]._Hide();
            _pool.Enqueue(_items[i]);
            _items[i] = null;
        }

        //设置边界
        _left = -_leftRange - 1 + index;
        //显示数据
        for (int i = _left; _pool.Count > 0; i++) { _Show(i); }
        base.InitValue(-index);
    }

    private void _OnValueChange(float index)
    {
        int left = Mathf.RoundToInt(-index) - _leftRange - 1;
        int right = left + _leftRange + _rightRange + 2;
        if (left != _left)
        {
            int _right = _left + _leftRange + _rightRange + 2;

            if (left < _left)
            {
                //隐藏右边
                for (int i = right + 1; i <= _right; i++) { _Hide(i); }
                //显示左边
                for (int i = left; i < _left; i++) { _Show(i); }
            }
            else
            {
                //隐藏左边
                for (int i = _left; i < left; i++) { _Hide(i); }
                //显示右边
                for (int i = _right + 1; i <= right; i++) { _Show(i); }
            }
            _left = left;
        }
        //修正角度
        for (int i = left; i <= right; i++) { _FixAngle(i); }
    }

    private void _FixAngle(int index)
    {
        index = _GetItemIndex(index);
        _items[index]._FixAngle();
    }
    private void _Show(int index)
    {
        int dataIndex = _GetDataIndex(index);
        index = _GetItemIndex(index);
        _items[index] = _pool.Dequeue();

        Vector2 pos = _radius * new Vector2(Mathf.Cos(index * _stepValue * Mathf.Deg2Rad), Mathf.Sin(index * _stepValue * Mathf.Deg2Rad));

        _items[index]._Show(pos, _data[dataIndex]);
    }
    private void _Hide(int index)
    {
        index = _GetItemIndex(index);
        if (_items[index] == null) { return; }
        _items[index]._Hide();
        _pool.Enqueue(_items[index]);
        _items[index] = null;
    }
}
