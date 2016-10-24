using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour
{
    private RoundCircleList _list;
    void Awake()
    {
        _list = GetComponent<RoundCircleList>();
        if (_list == null)
        {
            Debug.LogError("未获得RoundCircleList组件");
        }
    }

    void Start()
    {
        if (_list == null) { return; }

        RoundCircleItemData[] data = new RoundCircleItemData[100];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = new RoundCircleItemData();
            data[i].index = i;
        }
        _list._SetData(data);

        //初始化指定位置
        // _list.InitValue(80);
    }
}
