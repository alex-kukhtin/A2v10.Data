// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using A2v10.Data.Interfaces;

namespace A2v10.Data
{
    public class SqlDbContext : IDbContext
    {
        const String RET_PARAM_NAME = "@RetId";

        IDataProfiler _profiler;
        IDataConfiguration _config;
        ITenantManager _tenantManager;
        IDataLocalizer _localizer;

        public SqlDbContext(IDataProfiler profiler, IDataConfiguration config, IDataLocalizer localizer, ITenantManager tenantManager = null)
        {
            _profiler = profiler;
            _config = config;
            _localizer = localizer;
            _tenantManager = tenantManager;
            if (_profiler == null)
                throw new ArgumentNullException(nameof(profiler));
            if (_config == null)
                throw new ArgumentNullException(nameof(config));
            if (_localizer == null)
                throw new ArgumentNullException(nameof(localizer));
        }

        #region IDbContext
        public String ConnectionString(String source)
        {
            return _config.ConnectionString(source);
        }

        public TOut Execute<TIn, TOut>(String source, String command, TIn element) where TIn : class where TOut: class
        {
            TOut outValue = null;
            using (var p = _profiler.Start(command))
            {
                using (var cnn = GetConnection(source))
                {
                    using (var cmd = cnn.CreateCommandSP(command))
                    {
                        var retParam = SetParametersFrom(cmd, element);
                        using (var rdr = cmd.ExecuteReader())
                        {
                            var helper = new LoadHelper<TOut>();
                            if (rdr.Read())
                            {
                                outValue = helper.ProcessFields(rdr);

                            }
                        }
                        SetReturnParamResult(retParam, element);
                    }
                }
            }
            return outValue;
        }

        public async Task<TOut> ExecuteAsync<TIn, TOut>(String source, String command, TIn element) where TIn : class where TOut : class
        {
            TOut outValue = null;
            using (var p = _profiler.Start(command))
            {
                using (var cnn = await GetConnectionAsync(source))
                {
                    using (var cmd = cnn.CreateCommandSP(command))
                    {
                        var retParam = SetParametersFrom(cmd, element);
                        using (var rdr = await cmd.ExecuteReaderAsync())
                        {
                            var helper = new LoadHelper<TOut>();
                            if (await rdr.ReadAsync())
                            {
                                outValue = helper.ProcessFields(rdr);

                            }
                        }
                        SetReturnParamResult(retParam, element);
                    }
                }
            }
            return outValue;
        }

        public SqlConnection GetConnection(String source)
        {
            var cnnStr = _config.ConnectionString(source);
            var cnn = new SqlConnection(cnnStr);
            cnn.Open();
            SetTenantId(source, cnn);
            return cnn;
        }

        public async Task<SqlConnection> GetConnectionAsync(String source)
        {
            var cnnStr = _config.ConnectionString(source);
            var cnn = new SqlConnection(cnnStr);
            await cnn.OpenAsync();
            await SetTenantIdAsync(source, cnn);
            return cnn;
        }

        public T Load<T>(String source, String command, System.Object prms = null) where T : class
        {
            using (var p = _profiler.Start(command))
            {
                var helper = new LoadHelper<T>();
                using (var cnn = GetConnection(source))
                {
                    using (var cmd = cnn.CreateCommandSP(command))
                    {
                        SqlExtensions.SetFromDynamic(cmd.Parameters, prms);
                        using (var rdr = cmd.ExecuteReader())
                        {
                            helper.ProcessRecord(rdr);
                            if (rdr.Read())
                            {
                                T result = helper.ProcessFields(rdr);
                                return result;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public async Task<T> LoadAsync<T>(String source, String command, System.Object prms = null) where T : class
        {
            using (var p = _profiler.Start(command))
            {
                var helper = new LoadHelper<T>();
                using (var cnn = await GetConnectionAsync(source))
                {
                    using (var cmd = cnn.CreateCommandSP(command))
                    {
                        SqlExtensions.SetFromDynamic(cmd.Parameters, prms);
                        using (var rdr = await cmd.ExecuteReaderAsync())
                        {
                            helper.ProcessRecord(rdr);
                            if (await rdr.ReadAsync())
                            {
                                T result = helper.ProcessFields(rdr);
                                return result;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public IList<T> LoadList<T>(String source, String command, Object prms) where T : class
        {
            using (var token = _profiler.Start(command))
            {
                var listLoader = new ListLoader<T>();
                using (var cnn = GetConnection(source))
                {
                    using (var cmd = cnn.CreateCommandSP(command))
                    {
                        SqlExtensions.SetFromDynamic(cmd.Parameters, prms);
                        using (var rdr = cmd.ExecuteReader())
                        {
                            listLoader.ProcessFields(rdr);
                            while (rdr.Read())
                            {
                                listLoader.ProcessRecord(rdr);
                            }
                        }
                    }
                }
                return listLoader.Result;
            }
        }

        public async Task<IList<T>> LoadListAsync<T>(String source, String command, System.Object prms) where T : class
        {
            using (var token = _profiler.Start(command))
            {
                var listLoader = new ListLoader<T>();
                using (var cnn = await GetConnectionAsync(source))
                {
                    using (var cmd = cnn.CreateCommandSP(command))
                    {
                        SqlExtensions.SetFromDynamic(cmd.Parameters, prms);
                        using (var rdr = await cmd.ExecuteReaderAsync())
                        {
                            listLoader.ProcessFields(rdr);
                            while (await rdr.ReadAsync())
                            {
                                listLoader.ProcessRecord(rdr);
                            }
                        }
                    }
                }
                return listLoader.Result;
            }
        }

        public IDataModel LoadModel(String source, String command, System.Object prms = null)
        {
            var modelReader = new DataModelReader(_localizer);
            using (var p = _profiler.Start(command))
            {
                ReadData(source, command,
                    (prm) =>
                    {
                        modelReader.SetParameters(prm, prms);
                    },
                    (no, rdr) =>
                    {
                        modelReader.ProcessOneRecord(rdr);
                    },
                    (no, rdr) =>
                    {
                        modelReader.ProcessOneMetadata(rdr);
                    });
            }
            modelReader.PostProcess();
            return modelReader.DataModel;
        }

        public async Task<IDataModel> LoadModelAsync(String source, String command, System.Object prms = null)
        {
            var modelReader = new DataModelReader(_localizer);
            using (var p = _profiler.Start(command))
            {
                await ReadDataAsync(source, command,
                    (prm) =>
                    {
                        modelReader.SetParameters(prm, prms);
                    },
                    (no, rdr) =>
                    {
                        modelReader.ProcessOneRecord(rdr);
                    },
                    (no, rdr) =>
                    {
                        modelReader.ProcessOneMetadata(rdr);
                    });
            }
            modelReader.PostProcess();
            return modelReader.DataModel;
        }

        public void SaveList<T>(String source, String command, System.Object prms, IEnumerable<T> list) where T : class
        {
            throw new System.NotImplementedException();
        }

        public Task SaveListAsync<T>(String source, String command, System.Object prms, IEnumerable<T> list) where T : class
        {
            throw new System.NotImplementedException();
        }

        public IDataModel SaveModel(String source, String command, System.Object data, System.Object prms = null)
        {
            var dataReader = new DataModelReader(_localizer);
            var dataWriter = new DataModelWriter();
            using (var p = _profiler.Start(command))
            {
                var metadataCommand = command.Update2Metadata();
                using (var cnn = GetConnection(source))
                {
                    using (var cmd = cnn.CreateCommandSP(metadataCommand))
                    {
                        using (var rdr = cmd.ExecuteReader())
                        {
                            do
                            {
                                dataWriter.ProcessOneMetadata(rdr);
                            }
                            while (rdr.NextResult());
                        }
                    }
                    using (var cmd = cnn.CreateCommandSP(command))
                    {
                        SqlCommandBuilder.DeriveParameters(cmd);
                        dataWriter.SetTableParameters(cmd, data, prms);
                        using (var rdr = cmd.ExecuteReader())
                        {
                            do
                            {
                                // metadata is not needed (exclude aliases)
                                dataReader.ProcessMetadataAliases(rdr);
                                while (rdr.Read())
                                {
                                    dataReader.ProcessOneRecord(rdr);
                                }
                            }
                            while (rdr.NextResult());
                        }
                    }
                }
                dataReader.PostProcess();
                return dataReader.DataModel;
            }
        }

        public async Task<IDataModel> SaveModelAsync(String source, String command, System.Object data, System.Object prms = null)
        {
            var dataReader = new DataModelReader(_localizer);
            var dataWriter = new DataModelWriter();
            using (var p = _profiler.Start(command))
            {
                var metadataCommand = command.Update2Metadata();
                using (var cnn = await GetConnectionAsync(source))
                {
                    using (var cmd = cnn.CreateCommandSP(metadataCommand))
                    {
                        using (var rdr = await cmd.ExecuteReaderAsync())
                        {
                            do
                            {
                                dataWriter.ProcessOneMetadata(rdr);
                            }
                            while (await rdr.NextResultAsync());
                        }
                    }
                    using (var cmd = cnn.CreateCommandSP(command))
                    {
                        SqlCommandBuilder.DeriveParameters(cmd);
                        dataWriter.SetTableParameters(cmd, data, prms);
                        using (var rdr = await cmd.ExecuteReaderAsync())
                        {
                            do
                            {
                                // metadata is not needed (exclude aliases)
                                dataReader.ProcessMetadataAliases(rdr);
                                while (await rdr.ReadAsync())
                                {
                                    dataReader.ProcessOneRecord(rdr);
                                }
                            }
                            while (await rdr.NextResultAsync());
                        }
                    }
                }
                dataReader.PostProcess();
                return dataReader.DataModel;
            }
        }
        #endregion

        SqlParameter SetParametersFrom<T>(SqlCommand cmd, T element)
        {
            Type retType = typeof(T);
            var props = retType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            SqlCommandBuilder.DeriveParameters(cmd);
            var sqlParams = cmd.Parameters;
            SqlParameter retParam = null;
            if (cmd.Parameters.Contains(RET_PARAM_NAME))
            {
                retParam = cmd.Parameters[RET_PARAM_NAME];
                retParam.Value = DBNull.Value;
            }
            foreach (var p in props)
            {
                var paramName = "@" + p.Name;
                if (sqlParams.Contains(paramName))
                {
                    var sqlParam = sqlParams[paramName];
                    var sqlVal = p.GetValue(element);
                    if (sqlParam.SqlDbType == SqlDbType.VarBinary)
                    {
                        var stream = sqlVal as Stream;
                        if (stream == null)
                            throw new IndexOutOfRangeException("Stream expected");
                        sqlParam.Value = new SqlBytes(stream);
                    }
                    else
                    {
                        sqlParam.Value = SqlExtensions.ConvertTo(sqlVal, sqlParam.SqlDbType.ToType());
                    }
                }
            }
            return retParam;
        }

        void SetReturnParamResult(SqlParameter retParam, Object element)
        {
            if (retParam == null)
                return;
            if (retParam.Value == DBNull.Value)
                return;
            var idProp = element.GetType().GetProperty("Id");
            if (idProp != null)
                idProp.SetValue(element, retParam.Value);
        }

        async Task ReadDataAsync(String source, String command,
            Action<SqlParameterCollection> setParams,
            Action<Int32, IDataReader> onRead,
            Action<Int32, IDataReader> onMetadata)
        {
            using (var cnn = await GetConnectionAsync(source))
            {
                Int32 rdrNo = 0;
                using (var cmd = cnn.CreateCommandSP(command))
                {
                    if (setParams != null)
                        setParams(cmd.Parameters);
                    using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                    {
                        do
                        {
                            if (onMetadata != null)
                                onMetadata(rdrNo, rdr);
                            while (await rdr.ReadAsync())
                            {
                                if (onRead != null)
                                    onRead(rdrNo, rdr);
                            }
                            rdrNo += 1;
                        } while (await rdr.NextResultAsync());
                    }
                }
            }
        }

        void ReadData(String source, String command,
            Action<SqlParameterCollection> setParams,
            Action<Int32, IDataReader> onRead,
            Action<Int32, IDataReader> onMetadata)
        {
            using (var cnn = GetConnection(source))
            {
                Int32 rdrNo = 0;
                using (var cmd = cnn.CreateCommandSP(command))
                {
                    if (setParams != null)
                        setParams(cmd.Parameters);
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        do
                        {
                            if (onMetadata != null)
                                onMetadata(rdrNo, rdr);
                            while (rdr.Read())
                            {
                                if (onRead != null)
                                    onRead(rdrNo, rdr);
                            }
                            rdrNo += 1;
                        } while (rdr.NextResult());
                    }
                }
            }
        }

        async Task SetTenantIdAsync(String source, SqlConnection cnn)
        {
            if (_tenantManager == null)
                return;
            await _tenantManager.SetTenantIdAsync(cnn, source);
        }

        void SetTenantId(String source, SqlConnection cnn)
        {
            if (_tenantManager == null)
                return;
            _tenantManager.SetTenantId(cnn, source);
        }
    }
}
