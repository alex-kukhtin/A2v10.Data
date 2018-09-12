// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.Dynamic;
using A2v10.Data.Interfaces;

namespace A2v10.Data
{
	public class DynamicDataModel : IDataModel
	{

		#region IDataModel
		public ExpandoObject Root { get; }
		public ExpandoObject System { get; set; }
		public IDictionary<String, IDataMetadata> Metadata { get; }
		#endregion

		public DynamicDataModel(IDictionary<String, IDataMetadata> metadata, ExpandoObject root, ExpandoObject system)
		{
			Root = root;
			System = system;
			Metadata = metadata;
		}

		public T Eval<T>(String expression)
		{
			T fallback = default(T);
			return (this.Root).Eval<T>(expression, fallback);
		}

		public T Eval<T>(ExpandoObject root, String expression)
		{
			T fallback = default(T);
			return (root).Eval<T>(expression, fallback);
		}

		public String CreateScript(IDataScripter scripter)
		{
			if (scripter == null)
				throw new ArgumentNullException(nameof(scripter));
			var sys = System as IDictionary<String, Object>;
			var meta = Metadata as IDictionary<String, IDataMetadata>;
			return scripter.CreateScript(sys, meta);
		}

		public IDictionary<String, dynamic> GetDynamic()
		{
			return ObjectBuilder.BuildObject(Root as ExpandoObject);
		}

		public void SetReadOnly()
		{
			if (System == null)
				System = new ExpandoObject();
			System.Set("ReadOnly", true);
			System.Set("StateReadOnly", true);
		}

		public Boolean IsReadOnly
		{
			get
			{
				if (System != null)
					return System.Get<Boolean>("ReadOnly");
				return false;
			}
		}

		public void Merge(IDataModel src)
		{
			var trgMeta = Metadata as IDictionary<String, IDataMetadata>;
			var srcMeta = src.Metadata as IDictionary<String, IDataMetadata>;
			var trgRoot = Root;
			var srcRoot = src.Root as IDictionary<String, Object>;
			var rootObj = trgMeta["TRoot"];
			var srcSystem = src.System as IDictionary<String, Object>;
			var trgSystem = System;
			foreach (var sm in srcMeta)
			{
				if (sm.Key != "TRoot")
				{
					if (trgMeta.ContainsKey(sm.Key))
						trgMeta[sm.Key] = sm.Value;
					else
						trgMeta.Add(sm.Key, sm.Value);
				}
				else
				{
					foreach (var f in sm.Value.Fields)
						rootObj.Fields.Add(f.Key, f.Value);
				}
			}
			foreach (var sr in srcRoot)
			{
				if (!trgRoot.AddChecked(sr.Key, sr.Value))
					throw new DataLoaderException($"DataModel.Merge. Item with '{sr.Key}' already has been added");
			}
			foreach (var sys in srcSystem)
				trgSystem.AddChecked(sys.Key, sys.Value);
		}

	}
}
