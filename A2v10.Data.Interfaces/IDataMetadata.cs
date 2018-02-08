﻿// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.


using System;
using System.Collections.Generic;
using System.Text;

namespace A2v10.Data.Interfaces
{
    public interface IDataMetadata
    {
        String Id { get; }
        String Name { get; }
        String RowNumber { get; }
        String HasChildren { get; }
        String Permissions { get; }
        String Items { get; set; }

        IDictionary<String, IDataFieldMetadata> Fields { get; }

        Boolean IsArrayType { get; }
        Boolean IsGroup { get; }
    }
}
