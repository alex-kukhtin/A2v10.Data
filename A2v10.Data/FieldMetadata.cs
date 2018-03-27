// Copyright © 2012-2017 Alex Kukhtin. All rights reserved.

using A2v10.Data.Interfaces;
using System;

namespace A2v10.Data
{
	public enum DataType
	{
		Undefined,
		String,
		Number,
		Date,
		Boolean,
	}

	public enum FieldType
	{
		Scalar,
		Object,
		Array,
		Map,
		Tree,
		Group
	}

	public enum SpecType
	{
		Unknown,
		Id,
		Key,
		Name,
		RefId,
		ParentId,
		RowCount,
		RowNumber,
		HasChildren,
		Items,
		Permissions,
		GroupMarker,
		ReadOnly,
		SortOrder,
		SortDir,
		PageSize,
		Offset,
		Filter
	}

	public class FieldMetadata : IDataFieldMetadata
	{
		public DataType DataType { get; }
		public FieldType ItemType { get; } // for object, array
		public String RefObject { get; } // for object, array
		public Boolean IsLazy { get; }

		public Boolean IsArrayLike { get { return ItemType == FieldType.Object || ItemType == FieldType.Array; } }

		public FieldMetadata(FieldInfo fi, DataType type)
		{
			DataType = type;
			IsLazy = fi.IsLazy;
			ItemType = FieldType.Scalar;
			RefObject = null;
			if (fi.IsObjectLike)
			{
				ItemType = fi.FieldType;
				RefObject = fi.TypeName;
			}
			else if (fi.IsRefId)
			{
				ItemType = FieldType.Object;
				RefObject = fi.TypeName;
			}
		}

		public String GetObjectType(String fieldName)
		{
			switch (ItemType)
			{
				case FieldType.Array:
				case FieldType.Tree:
					return RefObject + "Array";
				case FieldType.Object:
				case FieldType.Group:
				case FieldType.Map:
					return RefObject;
				default:
					if (DataType == DataType.Undefined)
						throw new DataLoaderException($"Invalid data type for '{fieldName}'");
					else
						return DataType.ToString();
			}
		}
	}
}
