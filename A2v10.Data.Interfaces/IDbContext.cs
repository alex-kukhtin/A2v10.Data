// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace A2v10.Data.Interfaces
{
    public interface IDbContext
    {
        String ConnectionString(String source);
        Task<SqlConnection> GetConnectionAsync(String source);

        IDataModel LoadModel(String source, String command, Object prms = null);
        Task<IDataModel> LoadModelAsync(String source, String command, Object prms = null);

        IDataModel SaveModel(String source, String command, Object data, Object prms = null);
        Task<IDataModel> SaveModelAsync(String source, String command, Object data, Object prms = null);

        T Load<T>(String source, String command, Object prms = null) where T : class;
        Task<T> LoadAsync<T>(String source, String command, Object prms = null) where T : class;

        IList<T> LoadList<T>(String source, String command, Object prms) where T : class;
        Task<IList<T>> LoadListAsync<T>(String source, String command, Object prms) where T : class;

        TOut Execute<TIn, TOut>(String source, String command, TIn element) where TIn : class where TOut: class;
        Task<TOut> ExecuteAsync<TIn, TOut>(String source, String command, TIn element) where TIn : class where TOut : class;

        void SaveList<T>(String source, String command, Object prms, IEnumerable<T> list) where T : class;
        Task SaveListAsync<T>(String source, String command, Object prms, IEnumerable<T> list) where T : class;
    }
}
