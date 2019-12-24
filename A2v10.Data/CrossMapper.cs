// Copyright © 2019 Alex Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.Dynamic;

namespace A2v10.Data
{
	internal class CrossItem
	{
		List<ExpandoObject> _list = new List<ExpandoObject>();
		Dictionary<String, Int32> _keys = new Dictionary<String, Int32>();
		String _targetProp;

		public CrossItem(String targetProp)
		{
			_targetProp = targetProp;
		}
		public void Add(String propName, ExpandoObject target)
		{
			_list.Add(target);
			if (!_keys.ContainsKey(propName))
			{
				_keys.Add(propName, _keys.Count);
			}
		}

		public void Transform()
		{
			int _keyCount = _keys.Count;
			foreach (var eo in _list)
			{
				var arr = CreateArray(_keyCount);
				ExpandoObject targetVal = eo.Get<ExpandoObject>(_targetProp);
				foreach (var key in _keys)
				{
					Int32 index = key.Value;
					arr[index] = targetVal.Get<ExpandoObject>(key.Key);
				}
				eo.Set(_targetProp, arr);
			}
		}

		List<ExpandoObject> CreateArray(Int32 cnt)
		{
			var l = new List<ExpandoObject>();
			for (Int32 i = 0; i < cnt; i++)
			{
				l.Add(new ExpandoObject());
			}
			return l;
		}

	}

	internal class CrossMapper : Dictionary<String, CrossItem>
	{
		public void Add(String key, String targetProp, ExpandoObject target, String propName)
		{
			CrossItem crossItem = null;
			if (!TryGetValue(key, out crossItem))
			{
				crossItem = new CrossItem(targetProp);
				Add(key, crossItem);
			}
			// all source elements
			crossItem.Add(propName, target);
		}

		public void Transform()
		{
			foreach (var x in this)
				x.Value.Transform();
		}
	}
}
