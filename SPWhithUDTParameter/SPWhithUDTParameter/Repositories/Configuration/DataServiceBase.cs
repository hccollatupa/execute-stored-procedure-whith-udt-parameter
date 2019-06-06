using log4net;
using Oracle.DataAccess.Client;
using System;
using System.Configuration;
using System.Data;
using SPWhithUDTParameter.Common;

namespace SPWhithUDTParameter.Repositories.Configuration
{
    public class DataServiceBase
    {
        private static readonly ILog logger = LogManager.GetLogger("DataAccess");

        private static string connectionStringName;

        public DataServiceBase(string _connectionStringName)
        {
            connectionStringName = _connectionStringName;
        }

        protected OracleCommand CreateCommand(out OracleCommand cmd, string procName, params IDataParameter[] procParams)
        {
            OracleConnection cnx = null;

            cmd = null;
            string auxLog = string.Empty;

            try
            {
                cmd = new OracleCommand(procName);
                cmd.CommandType = CommandType.StoredProcedure;

                logger.InfoFormat("SP -> {0}", cmd.CommandText);

                if (procParams != null)
                {
                    for (int index = 0; index < procParams.Length; index++)
                    {
                        cmd.Parameters.Add(procParams[index]);
                        if (cmd.Parameters[index].Direction == ParameterDirection.Input)
                            auxLog += (index == 0 ? "" : "|") + cmd.Parameters[index].ParameterName + ":" + cmd.Parameters[index].Value;
                    }
                }

                if (!string.IsNullOrWhiteSpace(auxLog))
                    logger.InfoFormat("P.IN -> {0}", auxLog);

                cnx = new OracleConnection(connectionStringName);
                cmd.Connection = cnx;
                cnx.Open();

            }
            catch (Exception ex)
            {
                logger.Error("CATCH -> ", ex);
                throw;
            }
            return cmd;
        }

        #region Execute DataSet, DataReader and NonQuery
        protected DataSet ExecuteDataSet(string procName, params IDataParameter[] procParams)
        {
            OracleCommand cmd;
            return ExecuteDataSet(out cmd, procName, procParams);
        }

        protected DataSet ExecuteDataSet(out OracleCommand cmd, string procName, params IDataParameter[] procParams)
        {
            OracleConnection cnx = null;
            DataSet ds = new DataSet();
            OracleDataAdapter da = new OracleDataAdapter();
            cmd = null;
            string auxLog = string.Empty;

            try
            {
                cmd = new OracleCommand(procName);
                cmd.CommandType = CommandType.StoredProcedure;

                logger.InfoFormat("SP -> {0}", cmd.CommandText);

                if (procParams != null)
                {
                    for (int index = 0; index < procParams.Length; index++)
                    {
                        cmd.Parameters.Add(procParams[index]);
                        if (cmd.Parameters[index].Direction == ParameterDirection.Input)
                            auxLog += (index == 0 ? "" : "|") + cmd.Parameters[index].ParameterName + ":" + cmd.Parameters[index].Value;
                    }
                }

                if (!string.IsNullOrWhiteSpace(auxLog))
                    logger.InfoFormat("P.IN -> {0}", auxLog);

                da.SelectCommand = (OracleCommand)cmd;

                cnx = new OracleConnection(connectionStringName);
                cmd.Connection = cnx;
                cnx.Open();

                da.Fill(ds);
            }
            catch (Exception ex)
            {
                logger.Error("CATCH -> ", ex);
                throw;
            }
            finally
            {
                if (da != null) da.Dispose();
                cnx.Dispose();
            }
            return ds;
        }

        protected OracleDataReader ExecuteDataReader(out OracleCommand cmd, string procName, params IDataParameter[] procParams)
        {
            CreateCommand(out cmd, procName, procParams);
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        protected void ExecuteNonQuery(string procName, params IDataParameter[] procParams)
        {
            OracleCommand cmd;
            ExecuteNonQuery(out cmd, procName, procParams);
        }

        protected void ExecuteNonQuery(string textsql)
        {
            OracleCommand cmd;
            ExecuteNonQuery(out cmd, textsql);
        }

        protected void ExecuteNonQuery(out OracleCommand cmd, string textsql)
        {
            OracleConnection cnx = null;
            cmd = null;

            try
            {
                cmd = new OracleCommand(textsql);
                cmd.CommandType = CommandType.StoredProcedure;

                logger.InfoFormat("SP -> {0}", cmd.CommandText);

                cnx = new OracleConnection(connectionStringName);
                cmd.Connection = cnx;
                cnx.Open();

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                logger.Error("CATCH -> ", ex);
                throw;
            }
            finally
            {
                cnx.Dispose();
                if (cmd != null) cmd.Dispose();
            }
        }

        protected void ExecuteNonQuery(out OracleCommand cmd, string procName, params IDataParameter[] procParams)
        {
            OracleConnection cnx = null;
            cmd = null;
            string auxLog = string.Empty;
            try
            {
                cmd = new OracleCommand(procName);

                logger.InfoFormat("SP -> {0}", cmd.CommandText);

                cmd.CommandType = CommandType.StoredProcedure;

                for (int index = 0; index < procParams.Length; index++)
                {
                    cmd.Parameters.Add(procParams[index]);
                    if (cmd.Parameters[index].Direction == ParameterDirection.Input)
                        auxLog += (index == 0 ? "" : "|") + cmd.Parameters[index].ParameterName + ":" + cmd.Parameters[index].Value;
                }

                if (!string.IsNullOrWhiteSpace(auxLog))
                    logger.InfoFormat("P.IN -> {0}", auxLog);

                cnx = new OracleConnection(connectionStringName);
                cmd.Connection = cnx;
                cnx.Open();

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                logger.Error("CATCH -> ", ex);
                throw;
            }
            finally
            {
                cnx.Dispose();
                //We do not release the "Command" object in this scope to allow the retrieval of values associated with the output parameters. Therefore, a "Dispose" must be made to the object "Command" after executing this method.
            }
        }
        #endregion

        #region OracleParameter
        protected OracleParameter CreateParameter(string paramName, OracleDbType paramType, object paramValue)
        {
            OracleParameter param = new OracleParameter(paramName, paramType);

            if (paramValue != DBNull.Value)
            {
                switch (paramType)
                {
                    case OracleDbType.Varchar2:
                    case OracleDbType.NVarchar2:
                        break;
                    case OracleDbType.Char:
                    case OracleDbType.NChar:
                    case OracleDbType.Date:
                        paramValue = CheckParamValue((DateTime)paramValue);
                        break;
                    case OracleDbType.Int16:
                        paramValue = CheckParamValue((Int16)paramValue);
                        break;
                    case OracleDbType.Int32:
                        paramValue = CheckParamValue((Int32)paramValue);
                        break;
                    case OracleDbType.Byte:
                        if (paramValue is bool) paramValue = (int)((bool)paramValue ? 1 : 0);
                        if ((int)paramValue < 0 || (int)paramValue > 1) paramValue = Constants.NullInt;
                        paramValue = CheckParamValue((int)paramValue);
                        break;
                    case OracleDbType.Single:
                        paramValue = CheckParamValue(Convert.ToSingle(paramValue));
                        break;
                    case OracleDbType.Decimal:
                        paramValue = CheckParamValue((decimal)paramValue);
                        break;
                    case OracleDbType.Double:
                        paramValue = CheckParamValue((double)paramValue);
                        break;
                }
            }
            param.Value = paramValue;
            return param;
        }

        protected OracleParameter CreateParameter(string paramName, OracleDbType paramType, ParameterDirection direction)
        {
            OracleParameter returnVal = CreateParameter(paramName, paramType, DBNull.Value);
            returnVal.Direction = direction;
            return returnVal;
        }

        protected OracleParameter CreateParameter(string paramName, OracleDbType paramType, object paramValue, ParameterDirection direction)
        {
            OracleParameter returnVal = CreateParameter(paramName, paramType, paramValue);
            returnVal.Direction = direction;
            return returnVal;
        }

        protected OracleParameter CreateParameter(string paramName, OracleDbType paramType, object paramValue, int size)
        {
            OracleParameter returnVal = CreateParameter(paramName, paramType, paramValue);
            returnVal.Size = size;
            return returnVal;
        }

        protected OracleParameter CreateParameter(string paramName, OracleDbType paramType, object paramValue, int size, ParameterDirection direction)
        {
            OracleParameter returnVal = CreateParameter(paramName, paramType, paramValue);
            returnVal.Direction = direction;
            returnVal.Size = size;
            return returnVal;
        }

        protected OracleParameter CreateParameter(string paramName, OracleDbType paramType, object paramValue, int size, byte precision)
        {
            OracleParameter returnVal = CreateParameter(paramName, paramType, paramValue);
            returnVal.Size = size;
            ((OracleParameter)returnVal).Precision = precision;
            return returnVal;
        }

        protected OracleParameter CreateParameter(string paramName, OracleDbType paramType, object paramValue, int size, byte precision, ParameterDirection direction)
        {
            OracleParameter returnVal = CreateParameter(paramName, paramType, paramValue);
            returnVal.Direction = direction;
            returnVal.Size = size;
            returnVal.Precision = precision;
            return returnVal;
        }

        protected static OracleParameter CreateOutputParamForUdtType(string paramName, string udtName)
        {
            return new OracleParameter
            {
                UdtTypeName = udtName.ToUpperInvariant(),
                ParameterName = paramName,
                OracleDbType = OracleDbType.RefCursor,
                Direction = ParameterDirection.Output,
            };
        }

        protected static OracleParameter CreateInputParamForUdtType(string paramName, string udtName, object paramValue)
        {
            return new OracleParameter
            {
                UdtTypeName = udtName.ToUpperInvariant(),
                ParameterName = paramName,
                OracleDbType = OracleDbType.Object,
                Direction = ParameterDirection.Input,
                Value = paramValue
            };
        }

        protected OracleParameter CreateInputParamCollection(string paramName, OracleDbType paramType, object paramValue, int size)
        {
            OracleParameter returnVal = new OracleParameter();
            returnVal.ParameterName = paramName;
            returnVal.OracleDbType = paramType;
            returnVal.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
            returnVal.Direction = ParameterDirection.Input;
            returnVal.Value = paramValue;
            returnVal.Size = size;
            return returnVal;
        }
        #endregion

        #region CheckParamValue
        protected object CheckParamValue(string paramValue)
        {
            if (string.IsNullOrEmpty(paramValue))
            {
                return DBNull.Value;
            }
            else
            {
                return paramValue;
            }
        }

        protected object CheckParamValue(Guid paramValue)
        {
            if (paramValue.Equals(Constants.NullGuid))
            {
                return DBNull.Value;
            }
            else
            {
                return paramValue;
            }
        }

        protected object CheckParamValue(DateTime paramValue)
        {
            if (paramValue.Equals(Constants.NullDateTime))
            {
                return DBNull.Value;
            }
            else
            {
                return paramValue;
            }
        }

        protected object CheckParamValue(double paramValue)
        {
            if (paramValue.Equals(Constants.NullDouble))
            {
                return DBNull.Value;
            }
            else
            {
                return paramValue;
            }
        }

        protected object CheckParamValue(float paramValue)
        {
            if (paramValue.Equals(Constants.NullFloat))
            {
                return DBNull.Value;
            }
            else
            {
                return paramValue;
            }
        }

        protected object CheckParamValue(Decimal paramValue)
        {
            if (paramValue.Equals(Constants.NullDecimal))
            {
                return DBNull.Value;
            }
            else
            {
                return paramValue;
            }
        }

        protected object CheckParamValue(int paramValue)
        {
            if (paramValue.Equals(Constants.NullInt))
            {
                return DBNull.Value;
            }
            else
            {
                return paramValue;
            }
        }
        #endregion
    }
}