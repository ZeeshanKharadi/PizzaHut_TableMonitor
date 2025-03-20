using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;
using TableDependency.SqlClient.Extensions;
using TableMonitor.Class;
using static iTextSharp.awt.geom.Point2D;
using static iTextSharp.text.pdf.events.IndexEvents;

namespace TableMonitor.Forms
{

    public partial class FrmHome : Form
    {
        //public SqlTableDependency<MiddlewareSALESTRANSACTION> people_table_dependency;
        public SqlTableDependency<MiddlewareTransaction> people_table_dependency;
        public SqlTableDependency<SALESTRANSACTION> people_table_dependency_POS;

        public FrmHome()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Purpose of this event is when application starts this function load data in gridview by calling LoadTransactionDataFromDb function
        /// and also it will start dependency when any change occure in data base dependency will triggerd.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        public enum Floor
        {
            Basement = 0,
            GroundFloor = 1,
            FirstFloor = 2,
            SecondFloor = 3,
            RoofTop = 4
        }

        public static Floor GetFloorFromString(string floorName)
        {
            // Convert the floor name to an enum
            switch (floorName.ToLower())
            {
                case "basement":
                    return Floor.Basement;
                case "ground floor":
                    return Floor.GroundFloor;
                case "first floor":
                    return Floor.FirstFloor;
                case "second floor":
                    return Floor.SecondFloor;
                case "roof top":
                    return Floor.RoofTop;
                default:
                    throw new ArgumentException("Invalid floor name");
            }
        }

        public static string GetFloorCode(Floor floor)
        {
            // Return the integer value of the enum as a string
            return ((int)floor).ToString("00");
        }
        private void FrmHome_Load(object sender, EventArgs e)
        {
            //form load event
            loadTransactionDataFromDbAsync();
            loadTransactionDataFromDbPOSAsync();
            start_people_table_dependency();
        }
        /// <summary>
        // This List Contains Database Items for Receipt PrinterName
        /// <summary>
        private List<string> itemList = new List<string>();
        /// <summary>
        /// Get Data from Database Printer for Reciept
        /// </summary>
        public void GetTableData()
        {
            using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString))
            {
                sqlConnection.Open();

                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "select PrinterName from ItemWisePrinterConfiguration";
                    SqlDataReader dr = sqlCommand.ExecuteReader();
                    while (dr.Read())
                    {
                        itemList.Add(dr["PrinterName"].ToString());
                    }
                }
                sqlConnection.Close();
            }
        }
        /// <summary>
        /// This function will call when FrmHome window close.
        /// purpose of this function is to call dependency close function when user close application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmHome_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                stop_people_table_dependency();
            }
            catch (Exception ex) { log_file(ex.ToString()); }
        }

        // start , stop , error, changed
        private bool start_people_table_dependency()
        {
            try
            {
                people_table_dependency_POS =  new SqlTableDependency<SALESTRANSACTION>(ConfigurationManager.ConnectionStrings["MiddlewareDbConnection"].ConnectionString, "SALESTRANSACTION", "crt");
                //people_table_dependency = new SqlTableDependency<MiddlewareSALESTRANSACTION>(ConfigurationManager.ConnectionStrings["MiddlewareDbConnection"].ConnectionString, "RetailTransactionSalesTrans", "dbo");
                people_table_dependency = new SqlTableDependency<MiddlewareTransaction>(ConfigurationManager.ConnectionStrings["MiddlewareDbConnection"].ConnectionString, "RetailTransaction", "dbo");

                //if any activity is performed in sql table then this dependency will tregger and check the entity type if entity type inserted or change then proceed accordingly
                people_table_dependency.OnChanged += People_table_dependency_OnChanged;
                //people_table_dependency.OnChanged += People_table_dependency_OnChanged;
                people_table_dependency_POS.OnChanged += People_table_dependency_POS_OnChanged;
                //if any error ouccerd during dependency on process it will throwe an exception
                people_table_dependency.OnError += people_table_dependency_OnError;
                //Dependency start function
                people_table_dependency.Start();
                people_table_dependency_POS.Start();
                return true;
            }
            catch (Exception ex)
            {
                log_file(ex.ToString());
            }
            return false;
        }
        /// <summary>
        /// If any activity is performed in the sql table then this dependancy will trigger
        /// and check the entity type if entity type is inserted then proceed accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Obsolete]
        private void People_table_dependency_OnChanged(object sender, RecordChangedEventArgs<MiddlewareTransaction> e)
        //private void People_table_dependency_OnChanged(object sender, RecordChangedEventArgs<MiddlewareSALESTRANSACTION> e)
        {
            try
            {
                var changedEntity = e.Entity;
                Thread.Sleep(2000);
                switch (e.ChangeType)

                {
                    case ChangeType.Update:
                        //{
                        //    log_file($"New Transaction:\t Transaction ID: {changedEntity.TransactionId}\t Created ON:{changedEntity.CreatedOn} \t ");
                        //}
                        {
                            //if (changedEntity.ISSUSPENDED == false )
                            //{
                            //calling log function to store event log. and display.
                            log_file($"Suspended Transaction:\t Transaction ID: {changedEntity.TransactionId}\t Created ON:{changedEntity.CreatedOn} \t");
                            //inialize database connection.
                            using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["MiddlewareDbConnection"].ConnectionString))
                            {
                                sqlConnection.Open();

                                var transactionDataObject = new TransactionData();
                                //Calling Stored Procedure and pass connection in sql command.
                                using (var cmd = new SqlCommand(@"sp_getProductDetailsCustom", sqlConnection))
                                {
                                    //cmd.CommandText = @"select a.ITEMID,erpt.[NAME] AS Description, erpt.PRODUCT,PRODPOOLID , rtt.CREATEDDATETIME,rtt.STAFF,rtt.TRANSACTIONID,rtt.RECEIPTID , a.QTY,a.Comment FROM
                                    //                    [crt].SALESTRANSACTION st
                                    //                    -- Several RTT records will be retrieved for the same ST record, so this is for searching only, not to be used on the UI.
                                    //                    LEFT JOIN [ax].RETAILTRANSACTIONTABLE AS rtt ON st.TRANSACTIONID = rtt.SUSPENDEDTRANSACTIONID
                                    //                    inner join ax.RETAILTRANSACTIONSALESTRANS  a on a.TRANSACTIONID = rtt.TRANSACTIONID
                                    //                        INNER JOIN [ax].INVENTTABLE it WITH (NOLOCK) ON it.ITEMID = a.ITEMID AND it.DATAAREAID = a.DATAAREAID
                                    //                        INNER JOIN [ax].ECORESPRODUCTTRANSLATION erpt ON erpt.PRODUCT = it.PRODUCT AND erpt.LANGUAGEID = 'en-us'
                                    //                        inner join [crt].[SUSPENDEDTRANSACTION] sut on sut.RECEIPTID = a.RECEIPTID
                                    //                        WHERE st.[DELETEDDATETIME] IS NULL and st.TRANSACTIONID = '" + changedEntity.TRANSACTIONID + "' and rtt.ENTRYSTATUS <> 1 AND a.TRANSACTIONSTATUS <> 1 ";


                                    //Specified how a command string is intpreted
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    //Passing Parameter in stored procedure
                                    cmd.Parameters.AddWithValue("@TransactionID", changedEntity.TransactionId);
                                    // Commed Execution
                                    var drCmd = cmd.ExecuteReader();
                                    transactionDataObject = new TransactionData();
                                    string Floor = "";
                                    while (drCmd.Read())
                                    {
                                        if (drCmd.HasRows)
                                        {
                                            transactionDataObject.CREATEDDATETIME = drCmd.IsDBNull(4) ? "" : drCmd.GetDateTime(4).ToString();
                                            transactionDataObject.ThirdPartyOrderId = drCmd.IsDBNull(12) ? "" : drCmd.GetString(12).ToString();
                                            transactionDataObject.TransactionID = drCmd.IsDBNull(6) ? "" : drCmd.GetString(6).ToString();
                                            transactionDataObject.ReceiptID = drCmd.IsDBNull(7) ? "" : drCmd.GetString(7).ToString();
                                            transactionDataObject.TableNumber = drCmd.IsDBNull(13) ? "" : drCmd.GetString(13).ToString();
                                            transactionDataObject.Server = drCmd.IsDBNull(5) ? "" : drCmd.GetString(5).ToString();
                                            transactionDataObject.Floor = drCmd.IsDBNull(10) ? "" : drCmd.GetString(10).ToString();
                                            transactionDataObject.StoreId = GeStoreNameFromDB(sqlConnection, transactionDataObject, 3); //drCmd.GetString(11).ToString();
                                            transactionDataObject.StoreName = GeStoreNameFromDB(sqlConnection, transactionDataObject, 1);
                                            transactionDataObject.TaxReg = GeStoreNameFromDB(sqlConnection, transactionDataObject, 2);
                                            transactionDataObject.DiscountAmount = GeTotalAmountsFromDB(sqlConnection, transactionDataObject, 1);
                                            transactionDataObject.AmountExcl = GeTotalAmountsFromDB(sqlConnection, transactionDataObject, 2);
                                            transactionDataObject.TaxAmount = GeTotalAmountsFromDB(sqlConnection, transactionDataObject, 3);
                                            transactionDataObject.AmountIncl = GeTotalAmountsFromDB(sqlConnection, transactionDataObject, 4);
                                            transactionDataObject.SurveyUrl = GetSurveyURL(sqlConnection, transactionDataObject);
                                            transactionDataObject.PaymentAmount = Math.Round(Math.Abs(drCmd.GetDecimal(14)), 2);
                                            transactionDataObject.FBRInvoiceNo = GetFBRInvoiceNo(sqlConnection, transactionDataObject);
                                            transactionDataObject.QrCode = GenerateQRCode(transactionDataObject);
                                            transactionDataObject.IsFinalize = drCmd.IsDBNull(11) ? false : drCmd.GetBoolean(11);
                                            transactionDataObject.CHANNEL = GeChannelForServerAppFromDB(sqlConnection, transactionDataObject);

                                            transactionDataObject.SalesLines.Add(new SalesLine
                                            {
                                                Description = drCmd.IsDBNull(1) ? "" : drCmd.GetString(1),
                                                PoolId = drCmd.IsDBNull(3) ? "" : drCmd.GetString(3).ToString(),
                                                QTY = drCmd.IsDBNull(8) ? 0 : Convert.ToInt32(drCmd.GetDecimal(8)),
                                                Comment = drCmd.IsDBNull(9) ? "" : drCmd.GetString(9),
                                                ItemInfoCode = "",// GetSubInfoCodesForSaleLine(changedEntity.TRANSACTIONID, sqlConnection)
                                                Unitprice = Math.Round(Math.Abs(drCmd.GetDecimal(15)), 2),
                                                Taxprice = Math.Round(Math.Abs(drCmd.GetDecimal(15)), 2),
                                                Totalprice = Math.Round(Math.Abs(drCmd.GetDecimal(16)), 2)

                                            });
                                            Floor = drCmd.GetString(10);
                                        }
                                    }

                                    //Calling Headprinter To print full receipt.
                                    using (var sqlCommand = sqlConnection.CreateCommand())
                                    {
                                        sqlCommand.CommandText = "SELECT PrinterName, floor FROM ItemWisePrinterConfiguration WHERE PoolId = @PoolId";
                                        sqlCommand.Parameters.AddWithValue("@PoolId", "Master");
                                        var dr = sqlCommand.ExecuteReader();

                                        while (dr.Read())
                                        {
                                            if (dr.HasRows && transactionDataObject.ReceiptID != null)
                                            {
                                                var printername = dr.GetString(0);
                                                var printerfloor = dr.GetString(1);
                                                // print the receipt
                                                PrinterUtility.Print(printername, "Master", transactionDataObject);
                                            }
                                        }
                                    }
                                    sqlConnection.Close();
                                    // distint pools from sale line and store in array
                                    var distinctPools = transactionDataObject.SalesLines.Select(x => x.PoolId).Distinct();
                                    foreach (var pool in distinctPools)
                                    {
                                        string itemfloor = null;
                                        var sortedLines = new List<SalesLine>();
                                        foreach (var line in transactionDataObject.SalesLines)
                                        {
                                            if (line.PoolId == pool)
                                            {
                                                sortedLines.Add(line);
                                            }
                                        }
                                        var data = new TransactionData
                                        {
                                            CREATEDDATETIME = transactionDataObject.CREATEDDATETIME,
                                            TransactionID = transactionDataObject.TransactionID,
                                            ReceiptID = transactionDataObject.ReceiptID,
                                            SalesLines = sortedLines,
                                            StaffId = transactionDataObject.StaffId,
                                            SUSPENDEDTRANSACTIONID = transactionDataObject.SUSPENDEDTRANSACTIONID,
                                            CHANNEL = transactionDataObject.CHANNEL
                                        };
                                        if (pool == "04")
                                        {
                                            itemfloor = Floor;
                                        }
                                        // send only data which belong to pool
                                        PrinterConfiguration(pool, transactionDataObject, itemfloor);
                                    }

                                    //}
                                }
                                //}
                            }
                            // is_Suspended False


                        }
                        break;

                    case ChangeType.Insert:
                        {
                            //if (changedEntity.ISSUSPENDED == false )
                            //{
                            //calling log function to store event log. and display.
                            log_file($"Suspended Transaction:\t Transaction ID: {changedEntity.TransactionId}\t Created ON:{changedEntity.CreatedOn} \t");
                            //inialize database connection.
                            using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["MiddlewareDbConnection"].ConnectionString))
                            {
                                sqlConnection.Open();

                                var transactionDataObject = new TransactionData();
                                //Calling Stored Procedure and pass connection in sql command.
                                using (var cmd = new SqlCommand(@"sp_getProductDetailsCustom", sqlConnection))
                                {
                                    //cmd.CommandText = @"select a.ITEMID,erpt.[NAME] AS Description, erpt.PRODUCT,PRODPOOLID , rtt.CREATEDDATETIME,rtt.STAFF,rtt.TRANSACTIONID,rtt.RECEIPTID , a.QTY,a.Comment FROM
                                    //                    [crt].SALESTRANSACTION st
                                    //                    -- Several RTT records will be retrieved for the same ST record, so this is for searching only, not to be used on the UI.
                                    //                    LEFT JOIN [ax].RETAILTRANSACTIONTABLE AS rtt ON st.TRANSACTIONID = rtt.SUSPENDEDTRANSACTIONID
                                    //                    inner join ax.RETAILTRANSACTIONSALESTRANS  a on a.TRANSACTIONID = rtt.TRANSACTIONID
                                    //                        INNER JOIN [ax].INVENTTABLE it WITH (NOLOCK) ON it.ITEMID = a.ITEMID AND it.DATAAREAID = a.DATAAREAID
                                    //                        INNER JOIN [ax].ECORESPRODUCTTRANSLATION erpt ON erpt.PRODUCT = it.PRODUCT AND erpt.LANGUAGEID = 'en-us'
                                    //                        inner join [crt].[SUSPENDEDTRANSACTION] sut on sut.RECEIPTID = a.RECEIPTID
                                    //                        WHERE st.[DELETEDDATETIME] IS NULL and st.TRANSACTIONID = '" + changedEntity.TRANSACTIONID + "' and rtt.ENTRYSTATUS <> 1 AND a.TRANSACTIONSTATUS <> 1 ";


                                    //Specified how a command string is intpreted
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    //Passing Parameter in stored procedure
                                    cmd.Parameters.AddWithValue("@TransactionID", changedEntity.TransactionId);
                                    // Commed Execution
                                    var drCmd = cmd.ExecuteReader();
                                    transactionDataObject = new TransactionData();
                                    string Floor = "";
                                    while (drCmd.Read())
                                    {
                                        if (drCmd.HasRows)
                                        {
                                            transactionDataObject.CREATEDDATETIME = drCmd.IsDBNull(4) ? "" : drCmd.GetDateTime(4).ToString();
                                            transactionDataObject.ThirdPartyOrderId = drCmd.IsDBNull(12) ? "" : drCmd.GetString(12).ToString();
                                            transactionDataObject.TransactionID = drCmd.IsDBNull(6) ? "" : drCmd.GetString(6).ToString();
                                            transactionDataObject.ReceiptID = drCmd.IsDBNull(7) ? "" : drCmd.GetString(7).ToString();
                                            transactionDataObject.TableNumber = drCmd.IsDBNull(13) ? "" : drCmd.GetString(13).ToString();
                                            transactionDataObject.Server = drCmd.IsDBNull(5) ? "" : drCmd.GetString(5).ToString();
                                            transactionDataObject.Floor = drCmd.IsDBNull(10) ? "" : drCmd.GetString(10).ToString();
                                            transactionDataObject.StoreId = GeStoreNameFromDB(sqlConnection, transactionDataObject, 3); //drCmd.GetString(11).ToString();
                                            transactionDataObject.StoreName = GeStoreNameFromDB(sqlConnection, transactionDataObject, 1);
                                            transactionDataObject.TaxReg = GeStoreNameFromDB(sqlConnection, transactionDataObject, 2);
                                            transactionDataObject.DiscountAmount = GeTotalAmountsFromDB(sqlConnection, transactionDataObject, 1);
                                            transactionDataObject.AmountExcl = GeTotalAmountsFromDB(sqlConnection, transactionDataObject, 2);
                                            transactionDataObject.TaxAmount = GeTotalAmountsFromDB(sqlConnection, transactionDataObject, 3);
                                            transactionDataObject.AmountIncl = GeTotalAmountsFromDB(sqlConnection, transactionDataObject, 4);
                                            transactionDataObject.SurveyUrl = GetSurveyURL(sqlConnection, transactionDataObject);
                                            transactionDataObject.PaymentAmount = Math.Round(Math.Abs(drCmd.GetDecimal(14)), 2);
                                            transactionDataObject.FBRInvoiceNo = GetFBRInvoiceNo(sqlConnection, transactionDataObject);
                                            transactionDataObject.QrCode = GenerateQRCode(transactionDataObject);
                                            transactionDataObject.IsFinalize = drCmd.IsDBNull(11) ? false : drCmd.GetBoolean(11);
                                            transactionDataObject.CHANNEL = GeChannelForServerAppFromDB(sqlConnection, transactionDataObject);

                                            transactionDataObject.SalesLines.Add(new SalesLine
                                            {
                                                Description = drCmd.IsDBNull(1) ? "" : drCmd.GetString(1),
                                                PoolId = drCmd.IsDBNull(3) ? "" : drCmd.GetString(3).ToString(),
                                                QTY = drCmd.IsDBNull(8) ? 0 : Convert.ToInt32(drCmd.GetDecimal(8)),
                                                Comment = drCmd.IsDBNull(9) ? "" : drCmd.GetString(9),
                                                ItemInfoCode = "",// GetSubInfoCodesForSaleLine(changedEntity.TRANSACTIONID, sqlConnection)
                                                Unitprice = Math.Round(Math.Abs(drCmd.GetDecimal(15)), 2),
                                                Taxprice = Math.Round(Math.Abs(drCmd.GetDecimal(15)), 2),
                                                Totalprice = Math.Round(Math.Abs(drCmd.GetDecimal(16)), 2)

                                            });
                                            Floor = drCmd.GetString(10);
                                        }
                                    }

                                        //Calling Headprinter To print full receipt.
                                        using (var sqlCommand = sqlConnection.CreateCommand())
                                        {
                                            sqlCommand.CommandText = "SELECT PrinterName, floor FROM ItemWisePrinterConfiguration WHERE PoolId = @PoolId";
                                            sqlCommand.Parameters.AddWithValue("@PoolId", "Master");
                                            var dr = sqlCommand.ExecuteReader();

                                            while (dr.Read())
                                            {
                                                if (dr.HasRows && transactionDataObject.ReceiptID != null)
                                                {
                                                    var printername = dr.GetString(0);
                                                    var printerfloor = dr.GetString(1);
                                                    // print the receipt
                                                    PrinterUtility.Print(printername, "Master", transactionDataObject);
                                                }
                                            }
                                        }
                                        sqlConnection.Close();
                                        // distint pools from sale line and store in array
                                        var distinctPools = transactionDataObject.SalesLines.Select(x => x.PoolId).Distinct();
                                        foreach (var pool in distinctPools)
                                        {
                                            string itemfloor = null;
                                            var sortedLines = new List<SalesLine>();
                                            foreach (var line in transactionDataObject.SalesLines)
                                            {
                                                if (line.PoolId == pool)
                                                {
                                                    sortedLines.Add(line);
                                                }
                                            }
                                            var data = new TransactionData
                                            {
                                                CREATEDDATETIME = transactionDataObject.CREATEDDATETIME,
                                                TransactionID = transactionDataObject.TransactionID,
                                                ReceiptID = transactionDataObject.ReceiptID,
                                                SalesLines = sortedLines,
                                                StaffId = transactionDataObject.StaffId,
                                                SUSPENDEDTRANSACTIONID = transactionDataObject.SUSPENDEDTRANSACTIONID,
                                                CHANNEL = transactionDataObject.CHANNEL
                                            };
                                            if (pool == "04")
                                            {
                                                itemfloor = Floor;
                                            }
                                            // send only data which belong to pool
                                            PrinterConfiguration(pool, transactionDataObject, itemfloor);
                                        }

                                    //}
                                }
                                //}
                            }
                            // is_Suspended False


                        }
                        break;


                        //case ChangeType.Delete:
                        //    {
                        //        //if (changedEntity.ISSUSPENDED == false)
                        //        //{
                        //            //calling log function to store event log. and display.
                        //            log_file($"Suspended Transaction:\t Transaction ID: {changedEntity.TRANSACTIONID}\t Created ON:{changedEntity.CREATEDDATETIME} \t ISSUSPENDED : {changedEntity.ISSUSPENDED} ");
                        //            //inialize database connection.
                        //            using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString))
                        //            {
                        //                sqlConnection.Open();

                        //                var transactionDataObject = new TransactionData();
                        //                //Calling Stored Procedure and pass connection in sql command.
                        //                using (var cmd = new SqlCommand(@"sp_getProductDetailsfalse", sqlConnection))
                        //                {
                        //                    //cmd.CommandText = @"select a.ITEMID,erpt.[NAME] AS Description, erpt.PRODUCT,PRODPOOLID , rtt.CREATEDDATETIME,rtt.STAFF,rtt.TRANSACTIONID,rtt.RECEIPTID , a.QTY,a.Comment FROM
                        //                    //                    [crt].SALESTRANSACTION st
                        //                    //                    -- Several RTT records will be retrieved for the same ST record, so this is for searching only, not to be used on the UI.
                        //                    //                    LEFT JOIN [ax].RETAILTRANSACTIONTABLE AS rtt ON st.TRANSACTIONID = rtt.SUSPENDEDTRANSACTIONID
                        //                    //                    inner join ax.RETAILTRANSACTIONSALESTRANS  a on a.TRANSACTIONID = rtt.TRANSACTIONID
                        //                    //                        INNER JOIN [ax].INVENTTABLE it WITH (NOLOCK) ON it.ITEMID = a.ITEMID AND it.DATAAREAID = a.DATAAREAID
                        //                    //                        INNER JOIN [ax].ECORESPRODUCTTRANSLATION erpt ON erpt.PRODUCT = it.PRODUCT AND erpt.LANGUAGEID = 'en-us'
                        //                    //                        inner join [crt].[SUSPENDEDTRANSACTION] sut on sut.RECEIPTID = a.RECEIPTID
                        //                    //                        WHERE st.[DELETEDDATETIME] IS NULL and st.TRANSACTIONID = '" + changedEntity.TRANSACTIONID + "' and rtt.ENTRYSTATUS <> 1 AND a.TRANSACTIONSTATUS <> 1 ";


                        //                    //Specified how a command string is intpreted
                        //                    cmd.CommandType = CommandType.StoredProcedure;
                        //                    //Passing Parameter in stored procedure
                        //                    cmd.Parameters.AddWithValue("@TransactionID", changedEntity.TRANSACTIONID);
                        //                    // Commed Execution
                        //                    var drCmd = cmd.ExecuteReader();
                        //                    transactionDataObject = new TransactionData();

                        //                    while (drCmd.Read())
                        //                    {
                        //                        if (drCmd.HasRows)
                        //                        {
                        //                            transactionDataObject.CREATEDDATETIME = drCmd.GetDateTime(4).ToString();
                        //                            transactionDataObject.StaffId = drCmd.GetString(5).ToString();
                        //                            transactionDataObject.TransactionID = drCmd.GetString(6).ToString();
                        //                            transactionDataObject.ReceiptID = drCmd.GetString(7).ToString();
                        //                            transactionDataObject.TableNumber = GetTableNoFromDB(changedEntity.TRANSACTIONID);
                        //                            transactionDataObject.EmployeName = GetEmployeeName(sqlConnection, transactionDataObject);
                        //                            transactionDataObject.SUSPENDEDTRANSACTIONID = drCmd.GetString(10).ToString();
                        //                            transactionDataObject.SalesLines.Add(new SalesLine
                        //                            {
                        //                                Description = drCmd.GetString(1),
                        //                                ProductId = drCmd.GetInt64(2).ToString(),
                        //                                PoolId = drCmd.GetString(3).ToString(),
                        //                                QTY = Convert.ToInt32(Math.Abs(drCmd.GetDecimal(8))),
                        //                                Comment = drCmd.GetString(9),
                        //                                ItemInfoCode = GetSubInfoCodesForSaleLine(changedEntity.TRANSACTIONID, sqlConnection)
                        //                            });
                        //                        }
                        //                    }


                        //                    //Calling Headprinter To print full receipt.
                        //                    using (var sqlCommand = sqlConnection.CreateCommand())
                        //                    {
                        //                        sqlCommand.CommandText = $"select PrinterName from ItemWisePrinterConfiguration where PoolId ='Master'";
                        //                        var dr = sqlCommand.ExecuteReader();

                        //                        while (dr.Read())
                        //                        {
                        //                            if (dr.HasRows)
                        //                            {
                        //                                var printername = dr.GetString(0);
                        //                                if (transactionDataObject.SalesLines.Count > 0)
                        //                                {
                        //                                    // print the receipt
                        //                                    PrinterUtility.Print(printername, "Master", transactionDataObject);
                        //                                }

                        //                            }
                        //                        }
                        //                    }
                        //                    sqlConnection.Close();
                        //                    // distint pools from sale line and store in array
                        //                    var distinctPools = transactionDataObject.SalesLines.Select(x => x.PoolId).Distinct();
                        //                    foreach (var pool in distinctPools)
                        //                    {
                        //                        var sortedLines = new List<SalesLine>();
                        //                        foreach (var line in transactionDataObject.SalesLines)
                        //                        {
                        //                            if (line.PoolId == pool)
                        //                            {
                        //                                sortedLines.Add(line);
                        //                            }
                        //                        }
                        //                        var data = new TransactionData
                        //                        {
                        //                            CREATEDDATETIME = transactionDataObject.CREATEDDATETIME,
                        //                            TransactionID = transactionDataObject.TransactionID,
                        //                            ReceiptID = transactionDataObject.ReceiptID,
                        //                            SalesLines = sortedLines,
                        //                            StaffId = transactionDataObject.StaffId,
                        //                            SUSPENDEDTRANSACTIONID = transactionDataObject.SUSPENDEDTRANSACTIONID,
                        //                        };
                        //                        if (data.SalesLines.Count > 0)
                        //                        {
                        //                            // send only data which belong to pool
                        //                            PrinterConfiguration(pool, transactionDataObject);
                        //                        }

                        //                    }
                        //                }
                        //            }
                        //      //  }
                        //    }
                        //    break;
                };
            }
            catch (Exception ex)
            {
                log_file(ex.ToString());
            }
        }

        [Obsolete]
        private void People_table_dependency_POS_OnChanged(object sender, RecordChangedEventArgs<SALESTRANSACTION> e)
        {
            try
            {
                var changedEntity = e.Entity;

                switch (e.ChangeType)
                {
                    case ChangeType.Insert:
                        {

                            if (changedEntity.COMMENT != null)
                            {
                                if (changedEntity.COMMENT.Contains("ServerApp") == true)
                                {

                                    log_file($"ServerApp Order Des:\t Transaction ID: {changedEntity.COMMENT}\t Created ON:{changedEntity.CREATEDDATETIME} \t TransactionID : {changedEntity.TRANSACTIONID} ");
                                    //inialize database connection.
                                    using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString))
                                    {
                                        //if (sqlConnection.State == ConnectionState.Closed) // sqlCon.Close();
                                        //{
                                        sqlConnection.Open();
                                        //  }

                                        var transactionDataObject = new TransactionData();
                                        //Calling Stored Procedure and pass connection in sql command.
                                        using (var cmd = new SqlCommand(@"sp_getProductDetailsfalse", sqlConnection))
                                        {
                                            //cmd.CommandText = @"select a.ITEMID,erpt.[NAME] AS Description, erpt.PRODUCT,PRODPOOLID , rtt.CREATEDDATETIME,rtt.STAFF,rtt.TRANSACTIONID,rtt.RECEIPTID , a.QTY,a.Comment FROM
                                            //                    [crt].SALESTRANSACTION st
                                            //                    -- Several RTT records will be retrieved for the same ST record, so this is for searching only, not to be used on the UI.
                                            //                    LEFT JOIN [ax].RETAILTRANSACTIONTABLE AS rtt ON st.TRANSACTIONID = rtt.SUSPENDEDTRANSACTIONID
                                            //                    inner join ax.RETAILTRANSACTIONSALESTRANS  a on a.TRANSACTIONID = rtt.TRANSACTIONID
                                            //                        INNER JOIN [ax].INVENTTABLE it WITH (NOLOCK) ON it.ITEMID = a.ITEMID AND it.DATAAREAID = a.DATAAREAID
                                            //                        INNER JOIN [ax].ECORESPRODUCTTRANSLATION erpt ON erpt.PRODUCT = it.PRODUCT AND erpt.LANGUAGEID = 'en-us'
                                            //                        inner join [crt].[SUSPENDEDTRANSACTION] sut on sut.RECEIPTID = a.RECEIPTID
                                            //                        WHERE st.[DELETEDDATETIME] IS NULL and st.TRANSACTIONID = '" + changedEntity.TRANSACTIONID + "' and rtt.ENTRYSTATUS <> 1 AND a.TRANSACTIONSTATUS <> 1 ";


                                            //Specified how a command string is intpreted
                                            cmd.CommandType = CommandType.StoredProcedure;
                                            //Passing Parameter in stored procedure
                                            cmd.Parameters.AddWithValue("@TransactionID", changedEntity.TRANSACTIONID);
                                            // Commed Execution
                                            var drCmd = cmd.ExecuteReader();
                                            transactionDataObject = new TransactionData();

                                            while (drCmd.Read())
                                            {
                                                if (drCmd.HasRows)
                                                {
                                                    transactionDataObject.CREATEDDATETIME = drCmd.GetDateTime(4).ToString();
                                                    transactionDataObject.StaffId = drCmd.GetString(5).ToString();
                                                    transactionDataObject.TransactionID = drCmd.GetString(6).ToString();
                                                    transactionDataObject.ReceiptID = drCmd.GetString(7).ToString();
                                                    transactionDataObject.TableNumber = drCmd.GetString(14).ToString();//GetTableNoFromDB(drCmd.GetString(6).ToString());
                                                    transactionDataObject.EmployeName = GetEmployeeName(sqlConnection, transactionDataObject);
                                                    transactionDataObject.SUSPENDEDTRANSACTIONID = drCmd.GetString(10).ToString();
                                                    transactionDataObject.CHANNEL = GeChannelFromDB(sqlConnection, transactionDataObject);
                                                    transactionDataObject.StoreId = GeStoreNameFromDB(sqlConnection, transactionDataObject, 3); //drCmd.GetString(11).ToString();
                                                    transactionDataObject.StoreName = GeStoreNameFromDB(sqlConnection, transactionDataObject, 1);
                                                    transactionDataObject.TaxReg = GeStoreNameFromDB(sqlConnection, transactionDataObject, 2);
                                                    transactionDataObject.DiscountAmount = GeTotalAmountsFromDB(sqlConnection, transactionDataObject, 1);
                                                    transactionDataObject.AmountExcl = GeTotalAmountsFromDB(sqlConnection, transactionDataObject, 2);
                                                    transactionDataObject.TaxAmount = GeTotalAmountsFromDB(sqlConnection, transactionDataObject, 3);
                                                    transactionDataObject.AmountIncl = GeTotalAmountsFromDB(sqlConnection, transactionDataObject, 4);
                                                    //transactionDataObject.SurveyUrl = GetSurveyURL(sqlConnection, transactionDataObject);
                                                    //transactionDataObject.PaymentAmount = drCmd.IsDBNull(14) ? 0 : Math.Round(Math.Abs(drCmd.GetDecimal(14)), 2);
                                                    //transactionDataObject.PaymentAmount = Math.Round(Math.Abs(drCmd.GetDecimal(14)), 2);
                                                    transactionDataObject.FBRInvoiceNo = GetFBRInvoiceNo(sqlConnection, transactionDataObject);
                                                    transactionDataObject.QrCode = GenerateQRCode(transactionDataObject);
                                                    transactionDataObject.Terminal = drCmd.GetString(15).ToString();
                                                    transactionDataObject.DiscountAmount = Math.Round(Math.Abs(drCmd.GetDecimal(16)), 2);
                                                    transactionDataObject.TaxRatePercent = Math.Round(Math.Abs(drCmd.GetDecimal(18)), 2);
                                                    transactionDataObject.SalesLines.Add(new SalesLine
                                                    {
                                                        Description = drCmd.GetString(1),
                                                        ProductId = drCmd.GetInt64(2).ToString(),
                                                        PoolId = drCmd.GetString(3).ToString(),
                                                        QTY = Convert.ToInt32(Math.Abs(drCmd.GetDecimal(8))),
                                                        Comment = drCmd.GetString(9),
                                                        ItemInfoCode = "",// GetSubInfoCodesForSaleLine(changedEntity.TRANSACTIONID, sqlConnection)
                                                        Unitprice = Math.Round(Math.Abs(drCmd.GetDecimal(11)), 2),
                                                        Taxprice = Math.Round(Math.Abs(drCmd.GetDecimal(12)), 2),
                                                        Totalprice = Math.Round(Math.Abs(drCmd.GetDecimal(13)), 2)

                                                    });
                                                    //transactionDataObject.PaymentLines.Add(new PaymentData
                                                    //{
                                                    //    Tend    ertype = GetPaymentData(sqlConnection, transactionDataObject, 1),
                                                    //    Amount = GetPaymentData(sqlConnection, transactionDataObject, 2)

                                                    //});
                                                }


                                            }

                                            transactionDataObject.PaymentLines = GetPaymentDataV2(sqlConnection, transactionDataObject);

                                            log_file($"After Loop:\t Receipt ID: {transactionDataObject.ReceiptID}");

                                            if (transactionDataObject.TransactionID != "" || transactionDataObject.TransactionID != null)
                                            {
                                                //Calling Headprinter To print full receipt.
                                                using (var sqlCommand = sqlConnection.CreateCommand())
                                                {
                                                    //sqlCommand.CommandText = $"select PrinterName from ItemWisePrinterConfiguration where PoolId ='CustPrint' and terminal = transactionDataObject.Terminal";
                                                    sqlCommand.CommandText = "SELECT PrinterName FROM ItemWisePrinterConfiguration WHERE PoolId = @PoolId AND Terminal = @Terminal";
                                                    sqlCommand.Parameters.AddWithValue("@PoolId", "CustPrint");
                                                    sqlCommand.Parameters.AddWithValue("@Terminal", transactionDataObject.Terminal);

                                                    var dr = sqlCommand.ExecuteReader();

                                                    while (dr.Read())
                                                    {
                                                        if (dr.HasRows)
                                                        {
                                                            var printername = dr.GetString(0);
                                                            // print the receipt
                                                            PrinterUtility.Print(printername, "Master", transactionDataObject, true);
                                                        }
                                                    }
                                                }
                                            }


                                            sqlConnection.Close();

                                        }

                                    }
                                }
                                log_file($"New Transaction:\t Transaction ID: {changedEntity.TRANSACTIONID}\t Created ON:{changedEntity.CREATEDDATETIME} \t ISSUSPENDED : {changedEntity.ISSUSPENDED} ");
                            }
                            
                        }
                        break;

                    case ChangeType.Update:
                        {
                            if (changedEntity.ISSUSPENDED == true)
                            {
                                //calling log function to store event log. and display.
                                log_file($"Suspended Transaction:\t Transaction ID: {changedEntity.TRANSACTIONID}\t Created ON:{changedEntity.CREATEDDATETIME} \t ISSUSPENDED : {changedEntity.ISSUSPENDED} ");
                                //inialize database connection.
                                using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString))
                                {
                                    sqlConnection.Open();

                                    var transactionDataObject = new TransactionData();
                                    
                                        //Calling Stored Procedure and pass connection in sql command.
                                    using (var cmd = new SqlCommand(@"sp_getProductDetails", sqlConnection))
                                    {
                                        string Floor = GetFloorForTransaction(changedEntity.TRANSACTIONID, sqlConnection);
                                        string FloorSubInfoCode = GetFloorIdForTransaction(changedEntity.TRANSACTIONID, sqlConnection);

                                        //cmd.CommandText = @"select a.ITEMID,erpt.[NAME] AS Description, erpt.PRODUCT,PRODPOOLID , rtt.CREATEDDATETIME,rtt.STAFF,rtt.TRANSACTIONID,rtt.RECEIPTID , a.QTY,a.Comment FROM
                                        //                    [crt].SALESTRANSACTION st
                                        //                    -- Several RTT records will be retrieved for the same ST record, so this is for searching only, not to be used on the UI.
                                        //                    LEFT JOIN [ax].RETAILTRANSACTIONTABLE AS rtt ON st.TRANSACTIONID = rtt.SUSPENDEDTRANSACTIONID
                                        //                    inner join ax.RETAILTRANSACTIONSALESTRANS  a on a.TRANSACTIONID = rtt.TRANSACTIONID
                                        //                        INNER JOIN [ax].INVENTTABLE it WITH (NOLOCK) ON it.ITEMID = a.ITEMID AND it.DATAAREAID = a.DATAAREAID
                                        //                        INNER JOIN [ax].ECORESPRODUCTTRANSLATION erpt ON erpt.PRODUCT = it.PRODUCT AND erpt.LANGUAGEID = 'en-us'
                                        //                        inner join [crt].[SUSPENDEDTRANSACTION] sut on sut.RECEIPTID = a.RECEIPTID
                                        //                        WHERE st.[DELETEDDATETIME] IS NULL and st.TRANSACTIONID = '" + changedEntity.TRANSACTIONID + "' and rtt.ENTRYSTATUS <> 1 AND a.TRANSACTIONSTATUS <> 1 ";


                                        //Specified how a command string is intpreted
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        //Passing Parameter in stored procedure
                                        cmd.Parameters.AddWithValue("@TransactionID", changedEntity.TRANSACTIONID);
                                        // Commed Execution
                                        var drCmd = cmd.ExecuteReader();
                                        transactionDataObject = new TransactionData();

                                        while (drCmd.Read())
                                        {
                                            if (drCmd.HasRows)
                                            {
                                                transactionDataObject.CREATEDDATETIME = drCmd.GetDateTime(4).ToString();
                                                transactionDataObject.StaffId = drCmd.GetString(5).ToString();
                                                transactionDataObject.TransactionID = drCmd.GetString(6).ToString();
                                                transactionDataObject.ReceiptID = drCmd.GetString(7).ToString();
                                                transactionDataObject.TableNumber = GetTableNoFromDB(drCmd.GetString(6).ToString());
                                                transactionDataObject.EmployeName = GetEmployeeName(sqlConnection, transactionDataObject);
                                                transactionDataObject.SUSPENDEDTRANSACTIONID = drCmd.GetString(10).ToString();
                                                transactionDataObject.CHANNEL = GeChannelFromDB(sqlConnection, transactionDataObject);
                                                transactionDataObject.Server = GetServerName(changedEntity.TRANSACTIONID, sqlConnection);
                                                transactionDataObject.DeliveryNumber = GetDeliveryNumber(changedEntity.TRANSACTIONID, sqlConnection);
                                                transactionDataObject.Floor = Floor;
                                                transactionDataObject.SalesLines.Add(new SalesLine
                                                {
                                                    Description = drCmd.GetString(1),
                                                    ProductId = drCmd.GetInt64(2).ToString(),
                                                    PoolId = drCmd.GetString(3).ToString(),
                                                    QTY = Convert.ToInt32(Math.Abs(drCmd.GetDecimal(8))),
                                                    Comment = drCmd.GetString(9),
                                                    ItemInfoCode = GetSubInfoCodesForSaleLine(changedEntity.TRANSACTIONID, sqlConnection)
                                                });
                                            }
                                        }


                                        //Calling Headprinter To print full receipt.
                                        // Commenting this as it will be taken out from POS Machines


                                        using (var sqlCommand = sqlConnection.CreateCommand())
                                        {
                                            sqlCommand.CommandText = $"select PrinterName from ItemWisePrinterConfiguration where PoolId ='Master'";
                                            var dr = sqlCommand.ExecuteReader();

                                            while (dr.Read())
                                            {
                                                if (dr.HasRows && transactionDataObject.ReceiptID != null)
                                                {
                                                    var printername = dr.GetString(0);
                                                    // print the receipt
                                                    PrinterUtility.Print(printername, "Master", transactionDataObject);
                                                }
                                            }
                                        }
                                        //sqlConnection.Close();


                                        // distint pools from sale line and store in array
                                        var distinctPools = transactionDataObject.SalesLines.Select(x => x.PoolId).Distinct();
                                        foreach (var pool in distinctPools)
                                        {
                                            var sortedLines = new List<SalesLine>();
                                            foreach (var line in transactionDataObject.SalesLines)
                                            {
                                                if (line.PoolId == pool)
                                                {
                                                    sortedLines.Add(line);
                                                }
                                            }
                                            var data = new TransactionData
                                            {
                                                CREATEDDATETIME = transactionDataObject.CREATEDDATETIME,
                                                TransactionID = transactionDataObject.TransactionID,
                                                ReceiptID = transactionDataObject.ReceiptID,
                                                SalesLines = sortedLines,
                                                StaffId = transactionDataObject.StaffId,
                                                SUSPENDEDTRANSACTIONID = transactionDataObject.SUSPENDEDTRANSACTIONID,
                                                CHANNEL = transactionDataObject.CHANNEL
                                            };
                                            string floorCode = null;
                                            if (pool == "04")
                                            {
                                                floorCode = FloorSubInfoCode.ToString();
                                            }

                                            // send only data which belong to pool
                                            PrinterConfiguration(pool, transactionDataObject, floorCode);
                                        }
                                    }
                                }
                            }
                            // is_Suspended False


                        }
                        break;


                        //case ChangeType.Delete:
                        //    {
                        //        //if (changedEntity.ISSUSPENDED == false)
                        //        //{
                        //            //calling log function to store event log. and display.
                        //            log_file($"Suspended Transaction:\t Transaction ID: {changedEntity.TRANSACTIONID}\t Created ON:{changedEntity.CREATEDDATETIME} \t ISSUSPENDED : {changedEntity.ISSUSPENDED} ");
                        //            //inialize database connection.
                        //            using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString))
                        //            {
                        //                sqlConnection.Open();

                        //                var transactionDataObject = new TransactionData();
                        //                //Calling Stored Procedure and pass connection in sql command.
                        //                using (var cmd = new SqlCommand(@"sp_getProductDetailsfalse", sqlConnection))
                        //                {
                        //                    //cmd.CommandText = @"select a.ITEMID,erpt.[NAME] AS Description, erpt.PRODUCT,PRODPOOLID , rtt.CREATEDDATETIME,rtt.STAFF,rtt.TRANSACTIONID,rtt.RECEIPTID , a.QTY,a.Comment FROM
                        //                    //                    [crt].SALESTRANSACTION st
                        //                    //                    -- Several RTT records will be retrieved for the same ST record, so this is for searching only, not to be used on the UI.
                        //                    //                    LEFT JOIN [ax].RETAILTRANSACTIONTABLE AS rtt ON st.TRANSACTIONID = rtt.SUSPENDEDTRANSACTIONID
                        //                    //                    inner join ax.RETAILTRANSACTIONSALESTRANS  a on a.TRANSACTIONID = rtt.TRANSACTIONID
                        //                    //                        INNER JOIN [ax].INVENTTABLE it WITH (NOLOCK) ON it.ITEMID = a.ITEMID AND it.DATAAREAID = a.DATAAREAID
                        //                    //                        INNER JOIN [ax].ECORESPRODUCTTRANSLATION erpt ON erpt.PRODUCT = it.PRODUCT AND erpt.LANGUAGEID = 'en-us'
                        //                    //                        inner join [crt].[SUSPENDEDTRANSACTION] sut on sut.RECEIPTID = a.RECEIPTID
                        //                    //                        WHERE st.[DELETEDDATETIME] IS NULL and st.TRANSACTIONID = '" + changedEntity.TRANSACTIONID + "' and rtt.ENTRYSTATUS <> 1 AND a.TRANSACTIONSTATUS <> 1 ";


                        //                    //Specified how a command string is intpreted
                        //                    cmd.CommandType = CommandType.StoredProcedure;
                        //                    //Passing Parameter in stored procedure
                        //                    cmd.Parameters.AddWithValue("@TransactionID", changedEntity.TRANSACTIONID);
                        //                    // Commed Execution
                        //                    var drCmd = cmd.ExecuteReader();
                        //                    transactionDataObject = new TransactionData();

                        //                    while (drCmd.Read())
                        //                    {
                        //                        if (drCmd.HasRows)
                        //                        {
                        //                            transactionDataObject.CREATEDDATETIME = drCmd.GetDateTime(4).ToString();
                        //                            transactionDataObject.StaffId = drCmd.GetString(5).ToString();
                        //                            transactionDataObject.TransactionID = drCmd.GetString(6).ToString();
                        //                            transactionDataObject.ReceiptID = drCmd.GetString(7).ToString();
                        //                            transactionDataObject.TableNumber = GetTableNoFromDB(changedEntity.TRANSACTIONID);
                        //                            transactionDataObject.EmployeName = GetEmployeeName(sqlConnection, transactionDataObject);
                        //                            transactionDataObject.SUSPENDEDTRANSACTIONID = drCmd.GetString(10).ToString();
                        //                            transactionDataObject.SalesLines.Add(new SalesLine
                        //                            {
                        //                                Description = drCmd.GetString(1),
                        //                                ProductId = drCmd.GetInt64(2).ToString(),
                        //                                PoolId = drCmd.GetString(3).ToString(),
                        //                                QTY = Convert.ToInt32(Math.Abs(drCmd.GetDecimal(8))),
                        //                                Comment = drCmd.GetString(9),
                        //                                ItemInfoCode = GetSubInfoCodesForSaleLine(changedEntity.TRANSACTIONID, sqlConnection)
                        //                            });
                        //                        }
                        //                    }


                        //                    //Calling Headprinter To print full receipt.
                        //                    using (var sqlCommand = sqlConnection.CreateCommand())
                        //                    {
                        //                        sqlCommand.CommandText = $"select PrinterName from ItemWisePrinterConfiguration where PoolId ='Master'";
                        //                        var dr = sqlCommand.ExecuteReader();

                        //                        while (dr.Read())
                        //                        {
                        //                            if (dr.HasRows)
                        //                            {
                        //                                var printername = dr.GetString(0);
                        //                                if (transactionDataObject.SalesLines.Count > 0)
                        //                                {
                        //                                    // print the receipt
                        //                                    PrinterUtility.Print(printername, "Master", transactionDataObject);
                        //                                }

                        //                            }
                        //                        }
                        //                    }
                        //                    sqlConnection.Close();
                        //                    // distint pools from sale line and store in array
                        //                    var distinctPools = transactionDataObject.SalesLines.Select(x => x.PoolId).Distinct();
                        //                    foreach (var pool in distinctPools)
                        //                    {
                        //                        var sortedLines = new List<SalesLine>();
                        //                        foreach (var line in transactionDataObject.SalesLines)
                        //                        {
                        //                            if (line.PoolId == pool)
                        //                            {
                        //                                sortedLines.Add(line);
                        //                            }
                        //                        }
                        //                        var data = new TransactionData
                        //                        {
                        //                            CREATEDDATETIME = transactionDataObject.CREATEDDATETIME,
                        //                            TransactionID = transactionDataObject.TransactionID,
                        //                            ReceiptID = transactionDataObject.ReceiptID,
                        //                            SalesLines = sortedLines,
                        //                            StaffId = transactionDataObject.StaffId,
                        //                            SUSPENDEDTRANSACTIONID = transactionDataObject.SUSPENDEDTRANSACTIONID,
                        //                        };
                        //                        if (data.SalesLines.Count > 0)
                        //                        {
                        //                            // send only data which belong to pool
                        //                            PrinterConfiguration(pool, transactionDataObject);
                        //                        }

                        //                    }
                        //                }
                        //            }
                        //      //  }
                        //    }
                        //    break;
                };
            }
            catch (Exception ex)
            {
                log_file(ex.ToString());
            }
        }


        /// <summary>
        /// This function get Sub InfoCodes For saleLine from Database
        /// Purpose of this function is hold subinfo code data in transaction data model from database
        /// </summary>
        /// <param name="changedEntity"></param>
        /// <param name="sqlConnection"></param>
        /// <param name="transactionDataObject"></param>
        private static string GetSubInfoCodesForSaleLine(string transactionId, SqlConnection sqlConnection)
        {
            using (var cmd = sqlConnection.CreateCommand())
            {
                cmd.CommandText = @"select INFORMATION,* from ax.RETAILTRANSACTIONINFOCODETRANS a
                                    inner join ax.RETAILTRANSACTIONSALESTRANS b on b.TRANSACTIONID = a.TRANSACTIONID and a.PARENTLINENUM = b.LINENUM
                                    inner join ax.RETAILTRANSACTIONTABLE c on c.TRANSACTIONID = a.TRANSACTIONID
                                    where c.SUSPENDEDTRANSACTIONID ='" + transactionId + "' and SUBINFOCODEID= '1'";

                //cmd.CommandText = @"SELECT INFORMATION  from ax.RETAILTRANSACTIONINFOCODETRANS  as infocode
                //join ax.RETAILINFOCODETABLE M on M.INFOCODEID = infocode.INFOCODEID 
                //join ax.RETAILINFOCODETRANSLATION T on T.INFOCODE = M.RECID 
                //join ax.RETAILTRANSACTIONTABLE rt on rt.TRANSACTIONID = infocode.TRANSACTIONID
                //where M.DESCRIPTION = 'SELECT ANY ONE?' 
                //and rt.SUSPENDEDTRANSACTIONID =  '" + transactionId + "'  ";

                var infoCodes = string.Empty;
                var dr = cmd.ExecuteReader();
                // transactionDataObject = new TransactionData();

                while (dr.Read())
                {
                    if (dr.HasRows)
                    {
                        infoCodes = dr.GetString(0);
                    }
                }
                return infoCodes;
            }
        }

        private static string GetFloorForTransaction(string transactionId, SqlConnection sqlConnection)
        {
            using (var cmd = sqlConnection.CreateCommand())
            {
                //cmd.CommandText = @"select INFORMATION,* from ax.RETAILTRANSACTIONINFOCODETRANS a
                //                    inner join ax.RETAILTRANSACTIONSALESTRANS b on b.TRANSACTIONID = a.TRANSACTIONID and a.PARENTLINENUM = b.LINENUM
                //                    inner join ax.RETAILTRANSACTIONTABLE c on c.TRANSACTIONID = a.TRANSACTIONID
                //                    where c.SUSPENDEDTRANSACTIONID ='" + transactionId + "' and SUBINFOCODEID= '1'";

                cmd.CommandText = @"SELECT INFORMATION  from ax.RETAILTRANSACTIONINFOCODETRANS  as infocode
                join ax.RETAILINFOCODETABLE M on M.INFOCODEID = infocode.INFOCODEID 
                join ax.RETAILINFOCODETRANSLATION T on T.INFOCODE = M.RECID 
                join ax.RETAILTRANSACTIONTABLE rt on rt.TRANSACTIONID = infocode.TRANSACTIONID
                where M.DESCRIPTION = 'SELECT ANY ONE?' 
                and rt.SUSPENDEDTRANSACTIONID =  '" + transactionId + "'  ";

                var infoCodes = string.Empty;
                var dr = cmd.ExecuteReader();
                // transactionDataObject = new TransactionData();

                while (dr.Read())
                {
                    if (dr.HasRows)
                    {
                        infoCodes = dr.GetString(0);
                    }
                }
                return infoCodes;
            }
        }
        private static string GetFloorIdForTransaction(string transactionId, SqlConnection sqlConnection)
        {
            using (var cmd = sqlConnection.CreateCommand())
            {
                //cmd.CommandText = @"select INFORMATION,* from ax.RETAILTRANSACTIONINFOCODETRANS a
                //                    inner join ax.RETAILTRANSACTIONSALESTRANS b on b.TRANSACTIONID = a.TRANSACTIONID and a.PARENTLINENUM = b.LINENUM
                //                    inner join ax.RETAILTRANSACTIONTABLE c on c.TRANSACTIONID = a.TRANSACTIONID
                //                    where c.SUSPENDEDTRANSACTIONID ='" + transactionId + "' and SUBINFOCODEID= '1'";

                cmd.CommandText = @"SELECT infocode.subinfocodeid  from ax.RETAILTRANSACTIONINFOCODETRANS  as infocode
                join ax.RETAILINFOCODETABLE M on M.INFOCODEID = infocode.INFOCODEID 
                join ax.RETAILINFOCODETRANSLATION T on T.INFOCODE = M.RECID 
                join ax.RETAILTRANSACTIONTABLE rt on rt.TRANSACTIONID = infocode.TRANSACTIONID
                where M.DESCRIPTION = 'SELECT ANY ONE?' 
                and rt.SUSPENDEDTRANSACTIONID =  '" + transactionId + "'  ";

                var infoCodes = string.Empty;
                var dr = cmd.ExecuteReader();
                // transactionDataObject = new TransactionData();

                while (dr.Read())
                {
                    if (dr.HasRows)
                    {
                        infoCodes = dr.GetString(0);
                    }
                }
                return infoCodes;
            }
        }
        /// <summary>
        /// This function get EmplyeeName For saleLine from Database
        /// Purpose of this function is hold Employee Name data in transaction data model from database
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="transactionDataObject"></param>
        /// <returns></returns>
        private static string GetEmployeeName(SqlConnection sqlConnection, TransactionData transactionDataObject)
        {
            using (var cmd1 = sqlConnection.CreateCommand())
            {
                var employeeName = string.Empty;
                cmd1.CommandText = @"select b.FIRSTNAME +' ' + b.LASTNAME as staffname from ax.HCMWORKER a inner join ax.DIRPERSONNAME b on a.PERSON= b.PERSON inner join ax.RETAILTRANSACTIONTABLE c on a.PERSONNELNUMBER ='" + transactionDataObject.StaffId + "'";

                var dr1 = cmd1.ExecuteReader();
                // transactionDataObject = new TransactionData();

                while (dr1.Read())
                {
                    if (dr1.HasRows)
                    {
                        employeeName = dr1.GetString(0);
                    }
                }
                return employeeName;
            }
        }
        /// <summary>
        /// This function Get Table Number For saleLine from Database
        /// Purpose of this function is hold TableNo data in transaction data model from database
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        private string GetTableNoFromDB(string transactionId)
        {
            using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString))
            {
                var tableNo = string.Empty;
                sqlConnection.Open();
                using (var cmd = sqlConnection.CreateCommand())
                {
                    //Our Old Query
                    //cmd.CommandText = @"select INFORMATION from ax.RETAILTRANSACTIONINFOCODETRANS a inner join ax.RETAILTRANSACTIONTABLE b on b.TRANSACTIONID = a.TRANSACTIONID
                    //                    where b.SUSPENDEDTRANSACTIONID =  '" + transactionId + "' and INFOCODEID ='Table'";

                    //Moneeb Bhai Provided Query
                    cmd.CommandText = @"SELECT INFORMATION  from ax.RETAILTRANSACTIONINFOCODETRANS  as infocode join ax.RETAILINFOCODETABLE M on M.INFOCODEID = infocode.INFOCODEID join ax.RETAILINFOCODETRANSLATION T on T.INFOCODE = M.RECID where M.DESCRIPTION = 'Tables' and infocode.TRANSACTIONID = '" + transactionId + "'";


                    var dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        if (dr.HasRows)
                        {
                            tableNo = dr.GetString(0);
                            break;
                        }
                    }
                }
                sqlConnection.Close();
                return tableNo;
            }
        }
        private string GeChannelFromDB(SqlConnection sqlConnection, TransactionData transactionDataObject)
        {
            using (var cmd1 = sqlConnection.CreateCommand())
            {
                var employeeName = string.Empty;

                cmd1.CommandText = @"select top 1 f.DESCRIPTION  from ax.RETAILTRANSACTIONSALESTRANS a inner join ax.RETAILTRANSACTIONTABLE b on b.TRANSACTIONID = a.TRANSACTIONID AND a.RECEIPTID = b.RECEIPTID inner join ax.INVENTDIMCOMBINATION c on c.ITEMID = a.ITEMID inner join  ax.EcoResDistinctProductVariant d on d.RECID = c.DISTINCTPRODUCTVARIANT inner join ax.EcoResProductMasterConfiguration e on e.CONFIGPRODUCTMASTER = d.PRODUCTMASTER inner join ax.EcoResProductMasterDimValueTranslation f on f.PRODUCTMASTERDIMENSIONVALUE = e.RECID where b.TRANSACTIONID  = '" + transactionDataObject.TransactionID + "'";


                var dr1 = cmd1.ExecuteReader();
                // transactionDataObject = new TransactionData();

                while (dr1.Read())
                {
                    if (dr1.HasRows)
                    {
                        employeeName = dr1.GetString(0);
                    }
                }
                return employeeName;
            }
        }

        /// <summary>
        /// Purpose of this function is to stop dependency when application is close 
        /// </summary>
        /// <returns></returns>
        private bool stop_people_table_dependency()
        {
            try
            {
                if (people_table_dependency != null)
                {
                    people_table_dependency.Stop();

                    return true;
                }
            }
            catch (Exception ex) { log_file(ex.ToString()); }

            return false;
        }
        /// <summary>
        /// Any type of error which trigger during dependecy will log.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void people_table_dependency_OnError(object sender, ErrorEventArgs e)
        {
            log_file(e.Error.Message);
        }
        /// <summary>
        /// This function is called form form load
        /// Purpose of this function is to only load the transaction data
        /// from the Db to gridview
        /// </summary>
        private void loadTransactionDataFromDbold()
        {
            var sql = "select t.TransactionId, t.CreatedOn , t.comment, t.NetAmountInclTax from [dbo].[RetailtransactionSalesTrans] as t order by CreatedOn desc";
            var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MiddlewareDbConnection"].ConnectionString);
            var da = new SqlDataAdapter(sql, connection);
            var dt = new DataTable();
            BindingSource SBind = new BindingSource();
            SBind.DataSource = dt;
            connection.Open();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            connection.Close();
        }

        private async Task loadTransactionDataFromDbAsync()
        {
            //var sql = "SELECT DISTINCT t.TransactionId, t.CreatedOn FROM [dbo].[RetailtransactionSalesTrans] AS t ORDER BY t.CreatedOn DESC;";
            var sql = "SELECT  t.TransactionId, t.CreatedOn FROM [dbo].[RetailTransaction] AS t  where isPaid != '1' ORDER BY t.CreatedOn DESC;";

            var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MiddlewareDbConnection"].ConnectionString);
            var da = new SqlDataAdapter(sql, connection);

            da.SelectCommand.CommandTimeout = 300; // Increase timeout

            var dt = new DataTable();
            BindingSource SBind = new BindingSource();
            SBind.DataSource = dt;

            try
            {
                await Task.Run(() =>
                {
                    connection.Open();
                    da.Fill(dt);
                });

                dataGridView1.DataSource = dt;
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }



        private void loadTransactionDataFromDbPOSold()
        {
            var sql = "select t.TRANSACTIONID, t.CREATEDDATETIME , t.COMMENT, t.AMOUNT ,t.ISSUSPENDED from [crt].[SALESTRANSACTION] as t order by CREATEDDATETIME desc";
            var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString);
            var da = new SqlDataAdapter(sql, connection);
            var dt = new DataTable();
            BindingSource SBind = new BindingSource();
            SBind.DataSource = dt;
            connection.Open();
            da.Fill(dt);
            dataGridView2.DataSource = dt;
            connection.Close();
        }

        private async Task loadTransactionDataFromDbPOSAsync()
        {
            var sql = "select t.TRANSACTIONID, t.CREATEDDATETIME, t.COMMENT, t.AMOUNT, t.ISSUSPENDED " +
                      "from [crt].[SALESTRANSACTION] as t where isSuspended = '1'" +
                      "order by t.CREATEDDATETIME desc";

            var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString);
            var da = new SqlDataAdapter(sql, connection);

            // Set Command Timeout to a higher value if necessary (e.g., 5 minutes)
            da.SelectCommand.CommandTimeout = 300; // Timeout in seconds

            var dt = new DataTable();
            BindingSource SBind = new BindingSource();
            SBind.DataSource = dt;

            try
            {
                await Task.Run(() =>
                {
                    // Open the connection and fill the DataTable asynchronously
                    connection.Open();
                    da.Fill(dt);
                });

                // After the data is fetched, bind it to the DataGridView
                dataGridView2.DataSource = dt;
            }
            catch (SqlException ex)
            {
                // Log or display the error details
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                // Ensure the connection is closed even in case of an error
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// This function is Generating Logs Export PDF and Print PDF.
        /// This function Also invoke loadTransactionDataFromDb function to refect updated data in datagridview.
        /// </summary>
        /// <param name="logText"></param>
        public void log_file(string logText)
        {
            ThreadSafe(() => richTextBox1.AppendText(DateTime.Now.ToString("HH:mm:ss:fff") + "\t" + logText + Environment.NewLine));
            System.IO.File.AppendAllText(Application.StartupPath + "\\log.txt", logText);

            try
            {
                Invoke(new Action(() => loadTransactionDataFromDbAsync()));
                Invoke(new Action(() => loadTransactionDataFromDbPOSAsync()));
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        /// <summary>
        ///For Multi Thrading Privent to race conditions.
        ///purpose of this function is to privent deadlock condition when multiple process are parallel work.
        /// <summary>
        private void ThreadSafe(MethodInvoker method)
        {
            try
            {
                if (InvokeRequired)
                    Invoke(method);
                else
                    method();
            }
            catch (ObjectDisposedException) { }
        }
        /// <summary>
        ///Navigate to Printer FrmHome to ConfigurationForm.
        ///Purpose of this funcion is to allow user to navigate to printer configration form.
        /// <summary>
        private void Insert_Btn_Click(object sender, EventArgs e)
        {
            AddPrinter AP = new AddPrinter();
            AP.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// This function get the poolId as a parameter and find the assoicated printer
        /// according to the pool id from the database
        /// and then redirect the command to the printing function with printer name.
        /// and also this function find head printer which have no pool id and redirect the command to the printing function with head printer name
        /// </summary>
        /// <param name="poolId"></param>
        ///
        private void PrinterConfiguration(string poolId, TransactionData td, string floor = null)
        {
            using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString))
            {
                sqlConnection.Open();

                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    if (floor == null)
                    {
                        sqlCommand.CommandText = $"select PrinterName from ItemWisePrinterConfiguration where PoolId = '{poolId}'";
                        var dr = sqlCommand.ExecuteReader();

                        while (dr.Read())
                        {
                            if (dr.HasRows)
                            {
                                var printername = dr.GetString(0);
                                // print the receipt
                                //Black Copper BC-85AC (redirected 2)
                                PrinterUtility.Print(printername, poolId, td);
                            }
                        }
                    }

                    else
                    {
                        sqlCommand.CommandText = $"SELECT PrinterName, floor FROM ItemWisePrinterConfiguration WHERE PoolId ='{poolId}' AND Floor = '{floor}'";
                        var dr = sqlCommand.ExecuteReader();

                        while (dr.Read())
                        {
                            if (dr.HasRows)
                            {
                                var printername = dr.GetString(0);
                                var printerfloor = dr.GetString(1);
                                // print the receipt
                                PrinterUtility.Print(printername, poolId, td);
                            }
                        }
                    }
                }
                sqlConnection.Close();
            }
        }
        /// <summary>
        /// Purpose of this function is to minimize the form and visible in system try in form of notify icon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmHome_Resize(object sender, EventArgs e)
        {
            //if the form is minimized
            //hide it from the task bar
            //and show the system tray icon (represented by the NotifyIcon control)
            if (this.WindowState == FormWindowState.Minimized)
            {
                notifyIcon1.Visible = true;
                Hide();
            }
        }
        /// <summary>
        /// Purpose of this function is to Maximized or return or normal the form window size and disable the notify icon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private string GeStoreNameFromDB(SqlConnection sqlConnection, TransactionData transactionDataObject, int type)
        {
            using (var cmd1 = sqlConnection.CreateCommand())
            {
                var storename = string.Empty;
                var taxreg = string.Empty;
                var operno = string.Empty;

                cmd1.CommandText = @"select top 1 op.OMOPERATINGUNITNUMBER,dp.NAME,TaxIdentificationNumber from ax.RETAILSTORETABLE Rst inner join ax.RETAILCHANNELTABLE Rct on Rst.RECID = Rct.RECID inner join ax.OMOPERATINGUNIT op on Rct.OMOPERATINGUNITID = op.RECID inner join ax.DIRPARTYTABLE dp on Rct.OMOPERATINGUNITID = dp.RECID inner join ax.RETAILTRANSACTIONTABLE Rtt on Rst.STORENUMBER = Rtt.STORE where Rtt.TRANSACTIONID = '" + transactionDataObject.TransactionID + "'";


                var dr1 = cmd1.ExecuteReader();
                // transactionDataObject = new TransactionData();

                while (dr1.Read())
                {
                    if (dr1.HasRows)
                    {
                        operno = dr1.GetString(0);
                        storename = dr1.GetString(1);
                        taxreg = dr1.GetString(2);

                    }
                }
                if (type == 1)
                    return storename;

                else if (type == 2)
                    return taxreg;

                else
                    return operno;
                //return storename;
            }
        }

        private decimal GeTotalAmountsFromDB(SqlConnection sqlConnection, TransactionData transactionDataObject, int type)
        {
            decimal amount = 0;
            using (var cmd1 = sqlConnection.CreateCommand())
            {
                //cmd1.CommandText = @"select  isnull(sum(a.DISCAMOUNT),0), isnull(sum(a.NETAMOUNT),0)+isnull(sum(a.TAXAMOUNT),0), isnull(sum(a.TAXAMOUNT),0), isnull(sum(a.NETAMOUNTINCLTAX),0)  from ax.RETAILTRANSACTIONSALESTRANS a inner join ax.RETAILTRANSACTIONTABLE b on b.TRANSACTIONID = a.TRANSACTIONID AND a.RECEIPTID = b.RECEIPTID where b.TRANSACTIONID  = '" + transactionDataObject.TransactionID + "'";
                cmd1.CommandText = @"select  isnull(sum(a.DISCAMOUNT),0), isnull(sum(a.NETAMOUNT),0), isnull(sum(a.TAXAMOUNT),0), isnull(sum(a.NETAMOUNTINCLTAX),0)  from ax.RETAILTRANSACTIONSALESTRANS a inner join ax.RETAILTRANSACTIONTABLE b on b.TRANSACTIONID = a.TRANSACTIONID AND a.RECEIPTID = b.RECEIPTID where b.TRANSACTIONID  = '" + transactionDataObject.TransactionID + "'";

                var dr1 = cmd1.ExecuteReader();
                // transactionDataObject = new TransactionData();

                while (dr1.Read())
                {
                    if (dr1.HasRows)
                    {
                        if (type == 1)
                        {
                            amount = Math.Round(Math.Abs(dr1.GetDecimal(0)), 2);

                        }
                        if (type == 2)
                        {
                            amount = Math.Round(Math.Abs(dr1.GetDecimal(1)), 2);

                        }

                        if (type == 3)
                        {
                            amount = Math.Round(Math.Abs(dr1.GetDecimal(2)), 2);

                        }

                        if (type == 4)
                        {
                            amount = Math.Round(Math.Abs(dr1.GetDecimal(3)), 2);

                        }

                    }
                }

            }
            return amount;
        }
        //private string GetPaymentData(SqlConnection sqlConnection, TransactionData transactionDataObject, int type)
        //{
        //    string payment = string.Empty;
        //    using (var cmd1 = sqlConnection.CreateCommand())
        //    {
        //        cmd1.CommandText = @"select top 1 b.NAME,a.AMOUNTMST from ax.RETAILTRANSACTIONPAYMENTTRANS a inner join ax.RETAILTENDERTYPETABLE b on a.TENDERTYPE = b.TENDERTYPEID where a.CHANGELINE=0 and a.TRANSACTIONID = '" + transactionDataObject.TransactionID + "'";

        //        var dr1 = cmd1.ExecuteReader();
        //        // transactionDataObject = new TransactionData();

        //        while (dr1.Read())
        //        {
        //            if (dr1.HasRows)
        //            {
        //                if (type == 1)
        //                {
        //                    payment = dr1.GetString(0);

        //                }
        //                if (type == 2)
        //                {
        //                    payment = Math.Round(dr1.GetDecimal(1), 2).ToString();

        //                }

        //            }
        //        }

        //    }
        //    return payment;
        //}

        private List<PaymentData> GetPaymentDataV2(SqlConnection sqlConnection, TransactionData transactionDataObject)
        {
            var paymentDataList = new List<PaymentData>();

            using (var cmd1 = sqlConnection.CreateCommand())
            {
                cmd1.CommandText = @"
            SELECT b.NAME, a.AMOUNTMST 
            FROM ax.RETAILTRANSACTIONPAYMENTTRANS a 
            INNER JOIN ax.RETAILTENDERTYPETABLE b ON a.TENDERTYPE = b.TENDERTYPEID 
            WHERE a.CHANGELINE = 0 AND a.TRANSACTIONID = @TransactionID";

                cmd1.Parameters.AddWithValue("@TransactionID", transactionDataObject.TransactionID); // Prevent SQL injection

                using (var dr1 = cmd1.ExecuteReader())
                {
                    while (dr1.Read())
                    {
                        var paymentData = new PaymentData
                        {
                            Tendertype = dr1.GetString(0), // Tender type
                            Amount = Math.Round(dr1.GetDecimal(1), 2).ToString() // Amount with rounding
                        };

                        paymentDataList.Add(paymentData);
                    }
                }
            }

            return paymentDataList;
        }

        private static string GetFBRInvoiceNo(SqlConnection sqlConnection, TransactionData transactionDataObject)
        {
            using (var cmd1 = sqlConnection.CreateCommand())
            {
                var fbrinvno = string.Empty;
                cmd1.CommandText = @"SELECT FBRINVOICENO FROM EXT.MZNFBRINVOICING where TRANSACTIONID ='" + transactionDataObject.TransactionID + "'";

                var dr1 = cmd1.ExecuteReader();
                // transactionDataObject = new TransactionData();

                while (dr1.Read())
                {
                    if (dr1.HasRows)
                    {
                        fbrinvno = dr1.GetString(0);
                    }
                }
                return fbrinvno;
            }
        }
        private string GetSurveyURL(SqlConnection sqlConnection, TransactionData transactionDataObject)
        {
            string channeldesc = transactionDataObject.CHANNEL;
            string fBRNo = string.Empty;
            string returnvalue = string.Empty;

            if (channeldesc == "EAT IN")
            {
                fBRNo = "1";
            }
            else if (channeldesc == "EAT OUT")
            {
                fBRNo = "2";
            }
            else if (channeldesc == "DELIVERY")
            {
                fBRNo = "3";
            }
            else if (channeldesc == "DRIVE THRU")
            {
                fBRNo = "4";
            }
            else
            {
                fBRNo = "0";
            }

            string variable = "https://u.kfcvisit.com/PAK?S=" + transactionDataObject.StoreId + "&D=" + DateTime.Now.ToString("ddMMyy") + "&T=" + DateTime.Now.ToString("HHmm") + "&U=" + transactionDataObject.ReceiptID.Substring(0, 10) + "&Source=QR&O=6" + "&ST=" + transactionDataObject.ReceiptID;


            ////  returnValue = fBRNo;
            var qrcode = GenerateQRCodeVAr(variable);
            returnvalue = qrcode;   //$"<L:{qrcode}>";

            return returnvalue;

        }


        public static string GenerateQRCode(TransactionData transactionDataObject)
        {
            string receiptFieldValue = "Error";
            string FBRID = transactionDataObject.FBRInvoiceNo;

            int qrByteArrayLength = 0;
            int qrCodeByteArrIndex;
            System.Byte[] qrCodeByteArray;

            //
            StringBuilder stringBuilder = new StringBuilder(string.Empty);
            //stringBuilder.Append(salesOrder.ReceiptId);
            stringBuilder.Append(FBRID);

            // Grab the Receipt ID and send it to be encoded as a QR Code bitmap
            // receiptFieldValue = Encode(150, 150, stringBuilder.ToString(), "UTF-8", BarcodeFormat.QR_CODE);

            qrCodeByteArrIndex = 0;
            qrByteArrayLength = stringBuilder.Length;
            qrCodeByteArray = new System.Byte[qrByteArrayLength];

            var encodedString = stringBuilder.ToString(); //Convert.ToBase64String(qrCodeByteArray);

            receiptFieldValue = encodedString;

            return receiptFieldValue;
        }

        public static string GenerateQRCodeVAr(string FBRID)
        {
            string receiptFieldValue = "Error";
            int qrByteArrayLength = 0;
            int qrCodeByteArrIndex;
            System.Byte[] qrCodeByteArray;

            //
            StringBuilder stringBuilder = new StringBuilder(string.Empty);
            //stringBuilder.Append(salesOrder.ReceiptId);
            stringBuilder.Append(FBRID);

            //// Grab the Receipt ID and send it to be encoded as a QR Code bitmap
            //receiptFieldValue = Encode(200, 200, stringBuilder.ToString(), "UTF-8", BarcodeFormat.QR_CODE);


            qrCodeByteArrIndex = 0;
            qrByteArrayLength = stringBuilder.Length;
            qrCodeByteArray = new System.Byte[qrByteArrayLength];

            var encodedString = stringBuilder.ToString();     //Convert.ToBase64String(qrCodeByteArray);

            receiptFieldValue = encodedString;
            return receiptFieldValue;
        }

        private static string GetServerName(string transactionId, SqlConnection sqlConnection)
        {
            using (var cmd = sqlConnection.CreateCommand())
            {
                //cmd.CommandText = @"select INFORMATION,* from ax.RETAILTRANSACTIONINFOCODETRANS a
                //                    inner join ax.RETAILTRANSACTIONSALESTRANS b on b.TRANSACTIONID = a.TRANSACTIONID and a.PARENTLINENUM = b.LINENUM
                //                    inner join ax.RETAILTRANSACTIONTABLE c on c.TRANSACTIONID = a.TRANSACTIONID
                //                    where c.SUSPENDEDTRANSACTIONID ='" + transactionId + "' and SUBINFOCODEID= '1'";

                cmd.CommandText = @"SELECT infocode.information  from ax.RETAILTRANSACTIONINFOCODETRANS  as infocode
                join ax.RETAILINFOCODETABLE M on M.INFOCODEID = infocode.INFOCODEID 
                join ax.RETAILINFOCODETRANSLATION T on T.INFOCODE = M.RECID 
                join ax.RETAILTRANSACTIONTABLE rt on rt.TRANSACTIONID = infocode.TRANSACTIONID
                where (M.DESCRIPTION like 'Take Away Waiters%' or  M.DESCRIPTION like 'Restaurant Waiter%')
                and rt.SUSPENDEDTRANSACTIONID =  '" + transactionId + "'  ";

                var infoCodes = string.Empty;
                var dr = cmd.ExecuteReader();
                // transactionDataObject = new TransactionData();

                while (dr.Read())
                {
                    if (dr.HasRows)
                    {
                        infoCodes = dr.GetString(0);
                    }
                }
                return infoCodes;
            }
        }

        private static string GetDeliveryNumber(string transactionId, SqlConnection sqlConnection)
        {
            using (var cmd = sqlConnection.CreateCommand())
            {
                //cmd.CommandText = @"select INFORMATION,* from ax.RETAILTRANSACTIONINFOCODETRANS a
                //                    inner join ax.RETAILTRANSACTIONSALESTRANS b on b.TRANSACTIONID = a.TRANSACTIONID and a.PARENTLINENUM = b.LINENUM
                //                    inner join ax.RETAILTRANSACTIONTABLE c on c.TRANSACTIONID = a.TRANSACTIONID
                //                    where c.SUSPENDEDTRANSACTIONID ='" + transactionId + "' and SUBINFOCODEID= '1'";

                cmd.CommandText = @"SELECT infocode.information  from ax.RETAILTRANSACTIONINFOCODETRANS  as infocode
                join ax.RETAILINFOCODETABLE M on M.INFOCODEID = infocode.INFOCODEID 
                join ax.RETAILINFOCODETRANSLATION T on T.INFOCODE = M.RECID 
                join ax.RETAILTRANSACTIONTABLE rt on rt.TRANSACTIONID = infocode.TRANSACTIONID
                where M.DESCRIPTION like 'Delivery Order Number%' 
                and rt.SUSPENDEDTRANSACTIONID =  '" + transactionId + "'  ";

                var infoCodes = string.Empty;
                var dr = cmd.ExecuteReader();
                // transactionDataObject = new TransactionData();

                while (dr.Read())
                {
                    if (dr.HasRows)
                    {
                        infoCodes = dr.GetString(0);
                    }
                }
                return infoCodes;
            }
        }
        private string GeChannelForServerAppFromDB(SqlConnection sqlConnection, TransactionData transactionDataObject)
        {
            using (var cmd1 = sqlConnection.CreateCommand())
            {
                var employeeName = string.Empty;

                cmd1.CommandText = @"select top 1 f.DESCRIPTION  from dbo.RetailTransactionSalesTrans a inner join dbo.RetailTransaction b on b.TRANSACTIONID = a.TRANSACTIONID inner join ax.INVENTDIMCOMBINATION c on c.ITEMID = a.ITEMID inner join  ax.EcoResDistinctProductVariant d on d.RECID = c.DISTINCTPRODUCTVARIANT inner join ax.EcoResProductMasterConfiguration e on e.CONFIGPRODUCTMASTER = d.PRODUCTMASTER inner join ax.EcoResProductMasterDimValueTranslation f on f.PRODUCTMASTERDIMENSIONVALUE = e.RECID where b.TRANSACTIONID  = '" + transactionDataObject.TransactionID + "'";


                var dr1 = cmd1.ExecuteReader();
                // transactionDataObject = new TransactionData();

                while (dr1.Read())
                {
                    if (dr1.HasRows)
                    {
                        employeeName = dr1.GetString(0);
                    }
                }
                return employeeName;
            }
        }
    }
}