// Copyright © 2019-2023 Alex Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.Dynamic;

namespace A2v10.Data;

internal class CrossItem
{
    private readonly Dictionary<Object, ExpandoObject> _items = new();
    private readonly Dictionary<String, Int32> _keys = new();
    public String TargetProp { get; }
    public Boolean IsArray { get; }
    public String CrossType { get; }

    public String KeyName { get; }

    public CrossItem(String targetProp, Boolean isArray, String crossType, String keyName)
    {
        TargetProp = targetProp;
        IsArray = isArray;
        CrossType = crossType;
        KeyName = keyName;
    }

    public void Add(String propName, ExpandoObject target)
    {
        var id = target.Get<Object>("Id");
        if (!_items.ContainsKey(id))
            _items.Add(id, target);
        if (!_keys.ContainsKey(propName))
        {
            _keys.Add(propName, _keys.Count);
        }
    }

    public List<String> GetCross()
    {
        var l = new List<String>();
        for (Int32 i = 0; i < _keys.Count; i++)
            l.Add(null);
        foreach (var x in _keys)
            l[x.Value] = x.Key;
        return l;
    }

    public void Transform()
    {
        if (!IsArray)
            return;
        Int32 _keyCount = _keys.Count;
        foreach (var itm in _items)
        {
            var eo = itm.Value;
            var arr = CreateArray(_keyCount);
            ExpandoObject targetVal = eo.Get<ExpandoObject>(TargetProp);
            if (targetVal == null)
                continue; // already array?
            foreach (var key in _keys)
            {
                Int32 index = key.Value;
                var val = targetVal.Get<ExpandoObject>(key.Key);
					if (val == null)
						val = new ExpandoObject() { { 
							KeyName, key.Key}
						};
                arr[index] = val;
            }
            eo.Set(TargetProp, arr);
        }
    }

    List<ExpandoObject> CreateArray(Int32 cnt)
    {
        var arr = new List<ExpandoObject>();
        foreach (var key in _keys)
            arr.Add(null);
        return arr;
    }

    void SetIntoArray(List<ExpandoObject> array, ExpandoObject obj, int ix)
    {
        while (array.Count <= ix)
            array.Add(null);
        array[ix] = obj;
    }

    public List<ExpandoObject> GetEmptyArray()
    {
        var arr = new List<ExpandoObject>();
        foreach (var key in _keys)
        {
            SetIntoArray(arr, new ExpandoObject() {
                { KeyName, key.Key}}, key.Value
            );

        }
        return arr;
    }
}

internal class CrossMapper : Dictionary<String, CrossItem>
{
    private readonly IDictionary<Tuple<String, String>, List<ExpandoObject>> _parentRecords = new Dictionary<Tuple<String, String>, List<ExpandoObject>>();
    public void AddParentRecord(String typeName, String propName, ExpandoObject record)
    {
        var key = Tuple.Create<String, String>(typeName, propName);
        if (_parentRecords.TryGetValue(key, out List<ExpandoObject> list))
            list.Add(record);
        else
            _parentRecords.Add(key, new List<ExpandoObject>() { record });
    }

    public void Add(String key, String targetProp, ExpandoObject target, String propName, String keyName, FieldInfo rootFI)
    {
        if (!TryGetValue(key, out CrossItem crossItem))
        {
            crossItem = new CrossItem(targetProp, rootFI.IsCrossArray, rootFI.TypeName, keyName);
            Add(key, crossItem);
        }
        // all source elements
        crossItem.Add(propName, target);
    }

    public void Transform()
    {
        foreach (var x in this)
            x.Value.Transform();
        TransformParentRecords();
    }

    void TransformParentRecords()
    {
        foreach (var kv in _parentRecords)
        {
            foreach (var row in kv.Value)
            {
                var arr = row.Get<List<ExpandoObject>>(kv.Key.Item2);
                if (arr == null)
                {
                    var crossKey = $"{kv.Key.Item1}.{kv.Key.Item2}";
                    if (this.TryGetValue(crossKey, out CrossItem crossItem))
                        row.Set(kv.Key.Item2, crossItem.GetEmptyArray());
                }
            }
        }
    }
}
