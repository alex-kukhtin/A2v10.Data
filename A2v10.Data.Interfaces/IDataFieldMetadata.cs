// Copyright © 2012-2017 Alex Kukhtin. All rights reserved.

using System;

namespace A2v10.Data.Interfaces
{
    public interface IDataFieldMetadata
    {
        Boolean IsLazy { get; }
        String GetObjectType(String fieldName);
    }
}
