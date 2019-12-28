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
		public String TargetProp { get; }
		public Boolean IsArray { get; }


		public CrossItem(String targetProp, Boolean isArray)
		{
			TargetProp = targetProp;
			IsArray = isArray;
		}

		public void Add(String propName, ExpandoObject target)
		{
			_list.Add(target);
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
			int _keyCount = _keys.Count;
			foreach (var eo in _list)
			{
				var arr = CreateArray(_keyCount);
				ExpandoObject targetVal = eo.Get<ExpandoObject>(TargetProp);
				foreach (var key in _keys)
				{
					Int32 index = key.Value;
					arr[index] = targetVal.Get<ExpandoObject>(key.Key);
				}
				eo.Set(TargetProp, arr);
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
		public void Add(String key, String targetProp, ExpandoObject target, String propName, Boolean isArray)
		{
			CrossItem crossItem = null;
			if (!TryGetValue(key, out crossItem))
			{
				crossItem = new CrossItem(targetProp, isArray);
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
