using ExcelDataReader;
using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace Swine.Demo.Lib
{
    public class WriterFile
    {
        public static Task Status()
        {
            string createText = "Y" + Environment.NewLine;
            File.WriteAllText("D:\\status.txt", createText);
            return Task.CompletedTask;
        }

        public static DataTable ReadExcel()
        {
            string path = "D:\\result.xlsx";
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IExcelDataReader reader;
                reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                var ds = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });
                reader.Close();
                return ds.Tables[0];
            }

        }
    }
}
