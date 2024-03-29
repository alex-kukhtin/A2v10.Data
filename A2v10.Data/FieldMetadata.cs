﻿// Copyright © 2012-2020 Alex Kukhtin. All rights reserved.

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
		Blob
	}

	public enum FieldType
	{
		Scalar,
		Object,
		Array,
		Map,
		Tree,
		Group,
		MapObject,
		Json,
		CrossArray,
		CrossObject,
		Lookup
	}

	public enum SpecType
	{
		Unknown,
		Id,
		Key,
		Name,
		UtcDate,
		RefId,
		ParentId,
		RowCount,
		RowNumber,
		HasChildren,
		Items,
		Expanded,
		Permissions,
		GroupMarker,
		ReadOnly,
		Copy,
		SortOrder,
		SortDir,
		PageSize,
		Offset,
		GroupBy,
		Filter,
		HasRows,
		Json,
		Utc,
		Token
	}

	public class FieldMetadata : IDataFieldMetadata
	{
		public DataType DataType { get; }
		public FieldType ItemType { get; set; } // for object, array
		public String RefObject { get; private set; } // for object, array
		public Boolean IsLazy { get; }
		public Int32 Length { get; }
		public Boolean IsJson { get; set; }
		public SqlDataType SqlDataType { get; }

		public Boolean IsArrayLike 
		{ 
			get 
			{ 
				return 
					ItemType == FieldType.Object || 
					ItemType == FieldType.Array || 
					ItemType == FieldType.Map || 
					ItemType == FieldType.Lookup; 
			} 
		}

		public FieldMetadata(FieldInfo fi, DataType type, SqlDataType sqlDataType, Int32 length)
		{
			DataType = type;
			SqlDataType = sqlDataType;
			Length = length;
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

		public void ToDynamicGroup()
		{
			ItemType = FieldType.Group;			
		}

		public String GetObjectType(String fieldName)
		{
			switch (ItemType)
			{
				case FieldType.Array:
				case FieldType.Tree:
				case FieldType.Map:
				case FieldType.CrossArray:
					return RefObject + "Array";
				case FieldType.Object:
				case FieldType.CrossObject:
				case FieldType.Group:
					return RefObject;
				case FieldType.MapObject:
				case FieldType.Lookup:
					return RefObject + "Map";
				case FieldType.Json:
					return "Json";
				default:
					if (DataType == DataType.Undefined)
						throw new DataLoaderException($"Invalid data type for '{fieldName}'");
					else
						return DataType.ToString();
			}
		}

		public void SetType(String type)
		{
			RefObject = type;
		}

		public String TypeForValidate
		{
			get
			{
				return ItemType switch
				{
					FieldType.Array or FieldType.Tree or FieldType.Map or FieldType.MapObject or FieldType.Lookup => RefObject + "[]",
					FieldType.Object or FieldType.Group => RefObject,
					_ => DataType.ToString(),
				};
			}
		}

		public String TypeScriptName => ItemType switch
		{
			FieldType.Scalar => DataType switch
			{
				DataType.Number or DataType.String or DataType.Boolean => DataType.ToString().ToLowerInvariant(),
				DataType.Date => "Date",
				_ => DataType.ToString(),
			},
			FieldType.Array or FieldType.Tree => $"IElementArray<{RefObject}>",
			FieldType.Map or FieldType.MapObject or FieldType.Lookup => RefObject + "[]",
			FieldType.Object or FieldType.Group => RefObject,
			_ => DataType.ToString(),
		};

	}
}
