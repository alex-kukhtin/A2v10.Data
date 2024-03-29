﻿// Copyright © 2015-2023 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Threading.Tasks;

namespace A2v10.Data.Interfaces;

public interface IDbContext
{
	String ConnectionString(String source);

	Task<SqlConnection> GetConnectionAsync(String source);
	SqlConnection GetConnection(String source);

	IDataModel LoadModel(String source, String command, Object prms = null, Int32 commandTimeout = 0);
	Task<IDataModel> LoadModelAsync(String source, String command, Object prms = null, Int32 commandTimeout = 0);

	IDataModel SaveModel(String source, String command, ExpandoObject data, Object prms = null);
	Task<IDataModel> SaveModelAsync(String source, String command, ExpandoObject data, Object prms = null, Func<ITableDescription, ExpandoObject> onSetData = null, Int32 commandTimeout = 0);

	T Load<T>(String source, String command, Object prms = null) where T : class;
	Task<T> LoadAsync<T>(String source, String command, Object prms = null) where T : class;

	IList<T> LoadList<T>(String source, String command, Object prms) where T : class;
	Task<IList<T>> LoadListAsync<T>(String source, String command, Object prms) where T : class;

	void Execute<T>(String source, String command, T element) where T : class;
	Task ExecuteAsync<T>(String source, String command, T element) where T : class;
	Task ExecuteExpandoAsync(String source, String command, ExpandoObject element);
	ExpandoObject ExecuteAndLoadExpando(String source, String command, ExpandoObject element);
	Task<ExpandoObject> ExecuteAndLoadExpandoAsync(String source, String command, ExpandoObject element, Int32 commandTimeout = 0);

	TOut ExecuteAndLoad<TIn, TOut>(String source, String command, TIn element) where TIn : class where TOut : class;
	Task<TOut> ExecuteAndLoadAsync<TIn, TOut>(String source, String command, TIn element) where TIn : class where TOut : class;

	void SaveList<T>(String source, String command, Object prms, IEnumerable<T> list) where T : class;
	Task SaveListAsync<T>(String source, String command, Object prms, IEnumerable<T> list) where T : class;
}
