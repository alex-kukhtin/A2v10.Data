// Copyright © 2018 Alex Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;

namespace A2v10.Data.Generator;

public class Solution
{
	public IDictionary<String, Table> _tables;

	public Solution(JsonModule module)
	{
		_tables = new Dictionary<String, Table>();
	}

	public void AddTable(String key, JsonTable table)
	{
		var t = new Table(this, key, table);
		_tables.Add(key, t);
	}

	public void CreateFields()
	{
		foreach (var t in _tables)
			t.Value.CreateFields();
	}

	public Table FindTable(String name)
	{
		if (_tables.TryGetValue(name, out Table table))
			return table;
		throw new DataCreatorException($"Table not found. '{name}'");
	}

	public void MakeTables(ModelBuilder builder)
	{
		foreach (var t in _tables)
		{
			builder.WriteDivider();
			t.Value.BuildCreate(builder);
		}
	}
}
