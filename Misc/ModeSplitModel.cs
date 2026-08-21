using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvaloniaModel.Models;
using CsvHelper;
using CsvHelper.Configuration;

namespace AvaloniaModel.Misc
{
    public class ModeSplitModel<T> where T : new()
    {
        DataAccess myDA = new DataAccess();

        /// <summary>
        /// Read CSV file with "," delimiter and put it in list
        /// </summary>
        /// <param name="filePath">CSV file path</param>
        /// <returns></returns>
        public List<T> readCSV(string filePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HeaderValidated = null,
                MissingFieldFound = null
            };

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<T>();
                return records.ToList();
            }
        }

        /// <summary>
        /// Save CSV from list
        /// </summary>
        /// <param name="list">list of objects</param>
        /// <param name="filename">CSV file that will be saved</param>
        /// <returns></returns>
        public bool saveCSV(List<T> list, string filename)
        {
            try
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                    HeaderValidated = null,
                    MissingFieldFound = null
                };

                using (var writer = new StreamWriter(Directory.GetCurrentDirectory() + "\\" + filename + ".csv"))
                using (var csv = new CsvWriter(writer, config))
                {
                    csv.WriteRecords(list);
                }
            }
            catch { return false; }
            return true;
        }

        //ExpAggrFunList method calculates the exponential of utility functions for Aggregated Data.
        public void ExpAggrFunList(ref List<Aggregated> UtilityArr)
        {
            UtilityArr.ForEach(u => u.Exp = Math.Exp(u.gx.GetValueOrDefault(0)));
        }
        //ExpDisAggrFunTble method calculates the exponential of utility functions for Disaggregated Data and returns a DataTable.
        public List<string> ExpDisAggrFunLst(List<string> UtilityArr)
        {
            double Utility;
            int CaseNo, mode;
            List<string> ExpLst = new List<string>();
            DataTable expDisAggDataTable = new DataTable();

            expDisAggDataTable = myDA.CreateDisAggregatedTable();

            for (int i = 0; i < UtilityArr.Count; i++)
            {
                CaseNo = Convert.ToInt32(UtilityArr[i].ToString().Split(':')[0]);
                mode = Convert.ToInt32(UtilityArr[i].ToString().Split(':')[1]);
                Utility = Convert.ToDouble(UtilityArr[i].ToString().Split(':')[2]);
                myDA.InsertDataRows(CaseNo, mode, Math.Exp(Utility), expDisAggDataTable);
                Debug.Write($"CaseNo:" + CaseNo + "mode: " + mode + " Exp:" + Math.Exp(Utility).ToString() + Environment.NewLine);
            }
            ExpLst = myDA.ProbDisAggrPropFunList(expDisAggDataTable);
            return ExpLst;
        }
        //PropFunList method calculates the probability of mode in Aggregated data.
        public void PropFunList(ref List<Aggregated> ExpArr)
        {
            var sum = ExpArr.Sum(e => e.Exp);
            ExpArr.ForEach(e => e.Prop = e.Exp / sum);
        }

    }
}
