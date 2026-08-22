using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Linq;

namespace AvaloniaModel.Misc
{
    //FANARAH - 2026-07-01
    //Copyright FANARAH TEAM 2026
    //we got this EXCEL connection string code from stackoverflow, and modified it to fit our needs.
   
    //stored procedures are not supported in excel, so we use the select statement to get the data from the excel sheet.
    //Paul model //https://www.w3schools.com/sql/sql_stored_procedures.asp
    internal class DataAccess
    {
        public DataTable CreateDisAggregatedTable()
        {
            DataTable disAggTable = new("DisAggrTable");

            // Define all the columns at once.
            DataColumn[] cols =
            [
                new DataColumn("CaseNo", typeof(int)),
            new DataColumn("mode", typeof(int)),
            new DataColumn("utilityVal", typeof(double))
            ];
            disAggTable.Columns.AddRange(cols);
            // disAggTable.PrimaryKey = [disAggTable.Columns["CaseNo"]];
            return disAggTable;
        }
        public void InsertDataRows(int CaseNo, int mode, double Utility, DataTable orderTable)
        {
            // Add one row at a time.
            DataRow row1 = orderTable.NewRow();
            row1["CaseNo"] = CaseNo;
            row1["mode"] = mode;
            row1["utilityVal"] = Utility;
            orderTable.Rows.Add(row1);
        }
        

        //ProbFunList method calculates the probability of mode in DisAggregated data.
        public List<string> ProbDisAggrPropFunList(DataTable DisAgrrDataTable)
        {// here mostafa make the group by avg and sum
            var Prob = new List<string>();
            if (DisAgrrDataTable == null || DisAgrrDataTable.Rows.Count == 0) return Prob;

            // Group by CaseNo, compute total utility per case, then compute probabilities per mode
            var rows = DisAgrrDataTable.AsEnumerable();
            var groups = rows.GroupBy(r => r.Field<int>("mode"));

            foreach (var g in groups)
            {
                double total = g.Sum(r => Convert.ToDouble(r["utilityVal"]));
                foreach (var r in g)
                {
                    int mode = r.Field<int>("mode");
                    double util = Convert.ToDouble(r["utilityVal"]);
                    double prob = total == 0 ? 0.0 : util / total;   //her we should devide Avg/total
                    Prob.Add($"{mode}:{Math.Round(prob, 2)}");
                }
                Debug.Write($"Prob total: {total}");
            }

            return Prob;
        }
        //public List<string> ProbDisAggrPropFunList(DataSet DisAgrrDataSet)
        //{
        //    List<string> Prob = new List<string>();
        //    string strSQL =
        //        "SELECT *" +
        //       "  FROM DisAggrTable";// +
        //    //  "  GROUP BY mode;";
        //    //" ORDER BY mode DESC;";

        //    //DataRow[] selectedRows = DisAgrrDatatable.Select(strSQL);
        //    //DataTable filteredTable = DisAgrrDatatable.Select(strSQL).CopyToDataTable();
        //    //Debug.Write($"Length : {selectedRows.Length}");

        //    //  DataSet ds = new DataSet();
        //    OleDbCommand myCommand = new OleDbCommand();
        //    myCommand.CommandText = strSQL;
        //    OleDbDataAdapter myDataAdapter = new OleDbDataAdapter();
        //    // DataTable myDataTable = new DataTable();
        //    int modeRow;
        //    //  try
        //    //  {
        //    myDataAdapter.SelectCommand = myCommand;
        //    myDataAdapter.Fill(DisAgrrDataSet);

        //    //for (int i = 0; i < selectedRows.Length; i++)
        //    //{
        //    //    modeRow = Convert.ToInt32(selectedRows[i]["mode"].ToString());
        //    //    Prob.Add(Convert.ToString(modeRow.ToString() + ":" + Math.Round(Convert.ToDouble(selectedRows[i]["Utility"].ToString()) / Convert.ToDouble(selectedRows[i]["TotalUtility"].ToString()), 2)));
        //    //    Debug.Write($"props:" + Prob[i].ToString() + Environment.NewLine);
        //    //}
        //    //   }
        //    //catch
        //    //{
        //    //    MessageBox.Show("Error probability functiom!! \n" + "error:");
        //    //    myDA.BasicConn().Close();
        //    //}
        //    //finally
        //    //{
        //    //    //cleanup
        //    //    myDA.BasicConn().Close();
        //    //    myDataAdapter.Dispose();
        //    //    DisAgrrDatatable.Dispose();
        //    //}
        //    return Prob;
        //}
    }
}

