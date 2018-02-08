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
        public ExpandoObject System { get; }
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
    }
}
