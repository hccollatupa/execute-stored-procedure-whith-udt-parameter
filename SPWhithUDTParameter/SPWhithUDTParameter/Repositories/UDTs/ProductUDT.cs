using System;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using SPWhithUDTParameter.Repositories.Configuration;

namespace SPWhithUDTParameter.Repositories.UDTs
{
    [OracleCustomTypeMapping("PRODUCT_TABLE_UDT")]
    public class ProductInfoList : CustomCollectionTypeBase<ProductInfoList, ProductInfo>
    {
    }

    [OracleCustomTypeMapping("PRODUCT_UDT")]
    public class ProductInfo : CustomTypeBase<ProductInfo>
    {
        [OracleObjectMapping("ID")]
        public long Id;
        [OracleObjectMapping("DESCRIPTION")]
        public string Description;
        [OracleObjectMapping("PRICE")]
        public double Price;

        public override void FromCustomObject(OracleConnection connection, IntPtr pointerUdt)
        {
            OracleUdt.SetValue(connection, pointerUdt, "ID", Id);
            OracleUdt.SetValue(connection, pointerUdt, "DESCRIPTION", Description);
            OracleUdt.SetValue(connection, pointerUdt, "PRICE", Price);
        }

        public override void ToCustomObject(OracleConnection connection, IntPtr pointerUdt)
        {
            Id = (long)OracleUdt.GetValue(connection, pointerUdt, "ID");
            Description = (string)OracleUdt.GetValue(connection, pointerUdt, "DESCRIPTION");
            Price = (long)OracleUdt.GetValue(connection, pointerUdt, "PRICE");
        }
    }
}