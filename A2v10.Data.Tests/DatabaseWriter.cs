// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;
using System.Dynamic;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using A2v10.Data.Interfaces;
using A2v10.Data.Tests.Configuration;

namespace A2v10.Data.Tests
{
    [TestClass]
    public class DatabaseWriter
    {
        IDbContext _dbContext;

        public DatabaseWriter()
        {
            _dbContext = Starter.Create();
        }

        [TestMethod]
        public async Task WriteSubObjectData()
        {
            // DATA with ROOT
            var jsonData = @"
            {
			    MainObject: {
				    Id : 45,
				    Name: 'MainObjectName',
				    NumValue : 531.55,
				    BitValue : true,
				    SubObject : {
					    Id: 55,
					    Name: 'SubObjectName',
					    SubArray: [
						    {X: 5, Y:6, D:5.1 },
						    {X: 8, Y:9, D:7.23 }
					    ]
				    }		
			    }
            }
			";
            var dataToSave = JsonConvert.DeserializeObject<ExpandoObject>(jsonData.Replace('\'', '"'), new ExpandoObjectConverter());
            IDataModel dm = await _dbContext.SaveModelAsync(null, "a2test.[NestedObject.Update]", dataToSave);

            var dt = new DataTester(dm, "MainObject");
            dt.AreValueEqual(45L, "Id");
            dt.AreValueEqual("MainObjectName", "Name");

            var tdsub = new DataTester(dm, "MainObject.SubObject");
            tdsub.AreValueEqual(55L, "Id");
            tdsub.AreValueEqual("SubObjectName", "Name");

            var tdsubarray = new DataTester(dm, "MainObject.SubObject.SubArray");
            tdsubarray.IsArray(2);

            tdsubarray.AreArrayValueEqual(5, 0, "X");
            tdsubarray.AreArrayValueEqual(6, 0, "Y");
            tdsubarray.AreArrayValueEqual(5.1M, 0, "D");

            tdsubarray.AreArrayValueEqual(8, 1, "X");
            tdsubarray.AreArrayValueEqual(9, 1, "Y");
            tdsubarray.AreArrayValueEqual(7.23M, 1, "D");
        }

        [TestMethod]
        public async Task WriteNewObject()
        {
            // DATA with ROOT
            var jsonData = @"
            {
			    MainObject: {
				    Id : 0,
				    Name: 'MainObjectName',
			    }
            }
			";
            var dataToSave = JsonConvert.DeserializeObject<ExpandoObject>(jsonData.Replace('\'', '"'), new ExpandoObjectConverter());
            IDataModel dm = await _dbContext.SaveModelAsync(null, "a2test.[NewObject.Update]", dataToSave);

            var dt = new DataTester(dm, "MainObject");
            dt.AreValueEqual("Id is null", "Name");
        }
    }
}
