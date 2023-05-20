﻿// Copyright © 2022-2023 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;

using A2v10.Data.Interfaces;

namespace A2v10.Data;

internal class KeyComparer : IEqualityComparer<Object>
{
	private const String Id = "Id";
    public new Boolean Equals(Object x, object y)
    {
		if (x == null && y == null) 
			return true;
		if (x == null || y == null) return false;
		if (x is ExpandoObject eox && y is ExpandoObject eoy)
			return eox.Get<Object>(Id) == eoy.Get<Object>(Id);
		return x == y;
    }

    public int GetHashCode(Object obj)
    {
		if (obj == null)
			return 0;
		if (obj is ExpandoObject eo)
			return eo.Get<Object>(Id).GetHashCode();
		return obj.GetHashCode();
    }
}

internal class DynamicGroupItem
{
	private readonly Object _key;
	private readonly Dictionary<Object, DynamicGroupItem> _children = new(new KeyComparer());
	private ExpandoObject _data = new();
	public DynamicGroupItem(Object key = null, String elem = null)
	{
		_key = key;
		if (elem != null)
			_data.Set(elem, key);
	}

	public ExpandoObject ToExpando(String propertyName)
	{
		var e = _data;
		var coll = new List<ExpandoObject>();
		foreach (var c in _children.Values)
			coll.Add(c.ToExpando(propertyName));
		e.Set(propertyName, coll);
		return e;
	}
	public DynamicGroupItem GetOrCreate(Object key, String elem)
	{
		if (_children.TryGetValue(key, out var item))
			return item;
		var newElem = new DynamicGroupItem(key, elem);
		_children.Add(key, newElem);
		return newElem;
	}

	public void SetData(ExpandoObject data)
	{
		_data = data;
	}

	public void Calculate<T>(String propName, Func<T[], T> calc)
	{
		if (_children.Count == 0) 
			return;
		T result = default;
		T[] values = new T[_children.Count];
		var i = 0;
		foreach (var item in _children.Values)
		{
			if (item._children.Count > 0)
				item.Calculate<T>(propName, calc);
			values[i++] = item._data.Get<T>(propName);
		}
		result = calc(values);
		_data.Set(propName, result);
	}
}

internal enum AggregateType
{
	Sum,
	Avg,
	Count
}

internal record AggregateDescriptor
{
	public String Property;
	public AggregateType Type;
}

internal class RecordsetDescriptor
{
	public List<String> Groups = new();
	public List<AggregateDescriptor> Aggregates = new();
	public void AddGroup(String prop)
	{
		Groups.Add(prop);	
	}
	public void AddAggregate(String prop, AggregateType type) 
	{
		Aggregates.Add(new AggregateDescriptor()
		{
			Property = prop,
			Type = type
		});
	}
}

internal class DynamicDataGrouping
{
	private readonly ExpandoObject _root;
	private readonly ExpandoObject _result = new();

	private readonly IDictionary<String, IDataMetadata> _metadata;
	private readonly Dictionary<String, RecordsetDescriptor> _recordsets = new();
	private readonly DataModelReader _modelReader;
    public DynamicDataGrouping(ExpandoObject root, IDictionary<String, IDataMetadata> metadata, DataModelReader modelReader)
	{
		_root = root;
		_metadata = metadata;
        _modelReader = modelReader;
    }

	private RecordsetDescriptor GetOrCreateRSDescriptor(String name)
	{
		if (_recordsets.TryGetValue(name, out var descr))
			return descr;
		var d = new RecordsetDescriptor();
		_recordsets.Add(name, d);
		return d;
	}

	public void AddGrouping(IDataReader rdr)
	{
		var itemName = rdr.GetName(0);
		var fi = new FieldInfo(itemName);
		var rsDescr = GetOrCreateRSDescriptor(fi.PropertyName);
		String funcName = null;
		String propName = null;
		for (var i = 1; i < rdr.FieldCount; i++)
		{
			var fn = rdr.GetName(i);
			switch (fn)
			{
				case "Property":
					propName = rdr.GetString(i);
					break;
				case "Func":
					funcName = rdr.GetString(i);
					break;
			}
		}
		switch (funcName)
		{
			case "Group":
				rsDescr.AddGroup(propName);
				break;
			case "Sum":
				rsDescr.AddAggregate(propName, AggregateType.Sum);
				break;
			case "Avg":
				rsDescr.AddAggregate(propName, AggregateType.Avg);
				break;
			case "Count":
				rsDescr.AddAggregate(propName, AggregateType.Count);
				break;
			default:
				throw new InvalidOperationException($"Invalid Function for grouping: {funcName}");
		}
	}

	void ProcessRecordset(RecordsetDescriptor descr, IDataMetadata itemMeta, GroupMetadata groupMeta,
		DynamicGroupItem dynaroot, List<ExpandoObject> items)
	{
		for (var i = 0; i < descr.Groups.Count; i++)
		{
			var gr = descr.Groups[i];
            groupMeta.AddMarkerMetadata(gr);
		}
        foreach (var dat in items)
		{
			Object elem = null;
			DynamicGroupItem group = dynaroot;
			for (var i = 0; i< descr.Groups.Count; i++)
			{
				var gr = descr.Groups[i];
				elem = dat.Eval<Object>(gr);
				group = group.GetOrCreate(elem, gr);
			}
			group?.SetData(dat);
		}
		foreach (var v in descr.Aggregates)
		{
			if (!itemMeta.Fields.TryGetValue(v.Property, out var dataMeta))
				throw new InvalidOperationException($"Field Metadata {v.Property} not found");
			switch (v.Type)
			{
				case AggregateType.Sum:
					switch (dataMeta.SqlDataType)
					{
						case SqlDataType.Float:
							dynaroot.Calculate<Double>(v.Property, (values) =>
								Sum((a, b) => a + b, values));
							break;
						case SqlDataType.Currency:
							dynaroot.Calculate<Decimal>(v.Property, (values) =>
								Sum((a, b) => a + b, values));
							break;
						default:
							throw new InvalidOperationException($"Sum for {dataMeta.SqlDataType} not yet implemented");
					}
					break;
				case AggregateType.Avg:
					switch (dataMeta.SqlDataType) 
					{
						case SqlDataType.Float:
							dynaroot.Calculate<Double>(v.Property, (values) =>
								Average((a, b) => a + b, (a, b) => a / b, values));
							break;
						case SqlDataType.Currency:
							dynaroot.Calculate<Decimal>(v.Property, (values) =>
								Average((a, b) => a + b, (a, b) => a / b, values));
							break;
						default:
							throw new InvalidOperationException($"Avg for {dataMeta.SqlDataType} not yet implemented");
					}
					break;
				case AggregateType.Count:
                    dynaroot.Calculate<Int32>(v.Property, (values) =>
                        Count(values));
					break;
			}
		}
	}

	public void Process()
	{
		var rootMd = _metadata["TRoot"];
		foreach (var pd in _recordsets)
		{
            if (!rootMd.Fields.TryGetValue(pd.Key, out var fieldMeta))
				throw new InvalidOperationException($"Metadata {pd.Key} not found");
			if (!_metadata.TryGetValue(fieldMeta.RefObject, out var itemMeta))
				throw new InvalidOperationException($"Metadata {fieldMeta.RefObject} not found");
            var gm = _modelReader.GetOrCreateGroupMetadata(fieldMeta.RefObject);
            var list = _root.Get<List<ExpandoObject>>(pd.Key);
			var dr = new DynamicGroupItem();
			ProcessRecordset(pd.Value, itemMeta, gm, dr, list);
			var result = dr.ToExpando(itemMeta.Items);
			_root.Set(pd.Key, result);
            itemMeta.IsGroup = true;
            fieldMeta.ToDynamicGroup();
		}
	}

	T Sum<T>(Func<T, T, T> add, T[] values) where T : struct
	{
		T result = default;
		for (var i = 0; i < values.Length; i++)
			result = add(result, values[i]);
		return result;
	}

	T Average<T>(Func<T, T, T> add, Func<T, Int32, T> div, T[] values) where T : struct
	{
		T result = default;
		for (var i = 0; i < values.Length; i++)
			result = add(result, values[i]);
		return div(result, values.Length);
	}
	Int32 Count<T>(T[] values) where T : struct
	{
		return values.Length;
	}
}