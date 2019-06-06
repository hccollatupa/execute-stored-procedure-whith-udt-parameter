using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Configuration;
using SPWhithUDTParameter.Repositories.Configuration;
using SPWhithUDTParameter.Entities;
using SPWhithUDTParameter.Repositories.UDTs;

namespace SPWhithUDTParameter.Repositories
{
    public class CategoryRepository : DataServiceBase
    {
        private static string connectionStringName = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

        public CategoryRepository()
            : base(connectionStringName)
        { }

        public long Create(Category entity)
        {
            #region Parse UDT
            ProductInfoList productInfoList = new ProductInfoList();
            ProductInfo[] productInfoArray = entity.Products.Select(x => new ProductInfo()
            {
                Id = x.Id,
                Description = x.Description,
                Price = x.Price
            }).ToArray();
            productInfoList.Values = productInfoArray;
            #endregion

            OracleCommand cmd;
            ExecuteNonQuery(out cmd,
                "PKG_TEST_01.CREATE_CATEGORY",
                CreateParameter("P_DESCRIPTION", OracleDbType.Varchar2, entity.Description),
                CreateInputParamForUdtType("P_PRODUCTS", "PRODUCT_TABLE_UDT", productInfoList),
                CreateParameter("P_ID", OracleDbType.Long, null, 255, ParameterDirection.Output)
               );
            object result = cmd.Parameters["P_ID"].Value;
            cmd.Dispose();
            return long.Parse(result.ToString());
        }
    }
}
