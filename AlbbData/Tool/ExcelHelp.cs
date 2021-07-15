using AlbbData.entity;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvitationForBidsCrawler
{
    public class ExcelHelp : IDisposable
    {
        private string fileName = null; //文件名
        private IWorkbook workbook = null;
        private FileStream fs = null;
        private bool disposed;
        public ExcelHelp(string fileName)
        {
            this.fileName = fileName;
            disposed = false;
        }


        public static ICellStyle DateStyle { get; set; }
        private IWorkbook wb;

        public string ExportExcelInfo(string filePath, string sheetName, List<ShopEntity> entityInfos)
        {

            string extension = System.IO.Path.GetExtension(filePath);
            if (wb == null)
            {
                //根据指定的文件格式创建对应的类
                if (extension.Equals(".xls"))
                {
                    wb = new HSSFWorkbook();
                }
                else
                {
                    wb = new XSSFWorkbook();
                }
            }


            ICellStyle dateStyle = wb.CreateCellStyle();//样式
            dateStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;//文字水平对齐方式
            dateStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;//文字垂直对齐方式
            //设置数据显示格式
            IDataFormat dataFormatCustom = wb.CreateDataFormat();
            dateStyle.DataFormat = dataFormatCustom.GetFormat("yyyy-MM-dd HH:mm:ss");
            DateStyle = dateStyle;
           
            CreateSheelAndInsertDate($"{sheetName}", entityInfos, fileName);
            return filePath;
        }
        public string ExportExcelInfo(string filePath, string sheetName, List<String> entityInfos)
        {

            string extension = System.IO.Path.GetExtension(filePath);
            if (wb == null)
            {
                //根据指定的文件格式创建对应的类
                if (extension.Equals(".xls"))
                {
                    wb = new HSSFWorkbook();
                }
                else
                {
                    wb = new XSSFWorkbook();
                }
            }


            ICellStyle dateStyle = wb.CreateCellStyle();//样式
            dateStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;//文字水平对齐方式
            dateStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;//文字垂直对齐方式
            //设置数据显示格式
            IDataFormat dataFormatCustom = wb.CreateDataFormat();
            dateStyle.DataFormat = dataFormatCustom.GetFormat("yyyy-MM-dd HH:mm:ss");
            DateStyle = dateStyle;
         
            CreateSheelAndInsertDate($"{sheetName}（其他）", entityInfos, fileName);
            return filePath;
        }
        private void CreateSheelAndInsertDate(string sheetName, List<ShopEntity> entityInfos, string filePath)
        {
            ISheet sheet = wb.CreateSheet(sheetName);
            SetData(entityInfos, sheet);
            using (FileStream fs = File.OpenWrite(filePath))
            {
                wb.Write(fs);
            }
        }

        private void CreateSheelAndInsertDate(string sheetName, List<String> entityInfos, string filePath)
        {
            ISheet sheet = wb.CreateSheet(sheetName);
            SetData(entityInfos, sheet);
            using (FileStream fs = File.OpenWrite(filePath))
            {
                wb.Write(fs);
            }
        }
        private static void SetData(List<ShopEntity> entityInfos, ISheet sheet)
        {
            var rowCount = entityInfos.Count - 1;
            IRow row;
            ICell cell = null;
            int cellPos = 0;
            row = sheet.CreateRow(0);//创建第i行//这行代码需改
            #region 头部
            cell = row.CreateCell(cellPos);
            SetCellValue(cell, "产品名称");
            cellPos++;
            cell = row.CreateCell(cellPos);
            SetCellValue(cell, "公司名称");
            cellPos++;
            cell = row.CreateCell(cellPos);
            SetCellValue(cell, "联系人");
            cellPos++;
            cell = row.CreateCell(cellPos);
            SetCellValue(cell, "联系电话");
            cellPos++;

            #endregion
            #region 填充内容
            for (int i = 0; i < rowCount; i++)
            {
                int ri = i + 1;
                row = sheet.CreateRow(ri);//创建第i行
                cellPos = 0;
                cell = row.CreateCell(cellPos);
                SetCellValue(cell, entityInfos[i].Title);
                cellPos++;
                cell = row.CreateCell(cellPos);
                SetCellValue(cell, entityInfos[i].ShopName);
                cellPos++;
                cell = row.CreateCell(cellPos);
                SetCellValue(cell, entityInfos[i].ContactName);
                cellPos++;
                cell = row.CreateCell(cellPos);
                SetCellValue(cell, entityInfos[i].ContactPhone);
                cellPos++;
                sheet.AutoSizeColumn(1);
            }
            #endregion

        }


        private static void SetData(List<String> entityInfos, ISheet sheet)
        {
            var rowCount = entityInfos.Count - 1;
            IRow row;
            ICell cell = null;
            int cellPos = 0;
            row = sheet.CreateRow(0);//创建第i行//这行代码需改
            #region 头部
            cell = row.CreateCell(cellPos);
            SetCellValue(cell, "Url");
            cellPos++;
            
            #endregion
            #region 填充内容
            for (int i = 0; i < rowCount; i++)
            {
                int ri = i + 1;
                row = sheet.CreateRow(ri);//创建第i行
                cellPos = 0;
                cell = row.CreateCell(cellPos);
                SetCellValue(cell, entityInfos[i]);
                cellPos++;
                sheet.AutoSizeColumn(1);
            }
            #endregion

        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="obj"></param>
        public static void SetCellValue(ICell cell, object obj)
        {
            if (obj is int)
            {
                cell.SetCellValue((int)obj);
            }
            else if (obj is double)
            {
                cell.SetCellValue((double)obj);
            }
            else if (obj is decimal)
            {
                cell.SetCellValue(Convert.ToDouble(obj));
            }
            else if (obj is IRichTextString)
            {
                cell.SetCellValue((IRichTextString)obj);
            }
            else if (obj is string)
            {
                cell.SetCellValue(obj.ToString());
            }
            else if (obj is DateTime)
            {
                cell.CellStyle = ExcelHelp.DateStyle;
                var temptime = ((DateTime)obj).GetDateTimeFormats('D')[0].ToString();
                cell.SetCellValue(temptime);
            }
            else if (obj is bool)
            {
                cell.SetCellValue((bool)obj);
            }
            else
            {
                cell.SetCellValue(obj.ToString());
            }
        }

        /// <summary>
        /// 将DataTable数据导入到excel中
        /// </summary>
        /// <param name="data">要导入的数据</param>
        /// <param name="isColumnWritten">DataTable的列名是否要导入</param>
        /// <param name="sheetName">要导入的excel的sheet的名称</param>
        /// <returns>导入数据行数(包含列名那一行)</returns>
        public int DataTableToExcel(DataTable data, string sheetName, bool isColumnWritten)
        {
            int i = 0;
            int j = 0;
            int count = 0;
            ISheet sheet = null;

            fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                workbook = new XSSFWorkbook();
            else if (fileName.IndexOf(".xls") > 0) // 2003版本
                workbook = new HSSFWorkbook();

            try
            {
                if (workbook != null)
                {
                    sheet = workbook.CreateSheet(sheetName);
                }
                else
                {
                    return -1;
                }

                if (isColumnWritten == true) //写入DataTable的列名
                {
                    IRow row = sheet.CreateRow(0);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                    }
                    count = 1;
                }
                else
                {
                    count = 0;
                }

                for (i = 0; i < data.Rows.Count; ++i)
                {
                    IRow row = sheet.CreateRow(count);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                    }
                    ++count;
                }
                workbook.Write(fs); //写入到excel
                return count;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return -1;
            }
        }
        /// <summary>
        /// 将excel中的数据导入到DataTable中
        /// </summary>
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <returns>返回的DataTable</returns>
        public DataTable ExcelToDataTable(string sheetName, bool isFirstRowColumn)
        {
            ISheet sheet = null;
            DataTable data = new DataTable();
            int startRow = 0;
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                    workbook = new XSSFWorkbook(fs);
                else if (fileName.IndexOf(".xls") > 0) // 2003版本
                    workbook = new HSSFWorkbook(fs);

                if (sheetName != null)
                {
                    sheet = workbook.GetSheet(sheetName);
                    if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }
                else
                {
                    sheet = workbook.GetSheetAt(0);
                }
                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    if (firstRow == null)
                    {
                        return null;
                    }
                    int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数

                    if (isFirstRowColumn)
                    {
                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                        {
                            ICell cell = firstRow.GetCell(i);


                            if (cell != null)
                            {
                                cell.SetCellType(CellType.String);
                                string cellValue = cell.StringCellValue;
                                if (cellValue != null)
                                {
                                    DataColumn column = new DataColumn(cellValue);
                                    data.Columns.Add(column);
                                }
                            }
                        }
                        startRow = sheet.FirstRowNum + 1;
                    }
                    else
                    {
                        startRow = sheet.FirstRowNum;
                    }

                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　

                        DataRow dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            ICell cell = row.GetCell(j);
                            if (cell != null && cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell))
                                dataRow[j] = cell.DateCellValue.ToString("yyyy/MM/dd");
                            else
                            {
                                //dataRow[j] = row.GetCell(j).ToString();
                                if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                                    dataRow[j] = row.GetCell(j).ToString();
                            }
                        }
                        data.Rows.Add(dataRow);
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (fs != null)
                        fs.Close();
                }

                fs = null;
                disposed = true;
            }
        }

        /// <summary>
        ///  字符串日期转DateTime
        /// </summary>
        public static DateTime TransStrToDateTime(string strDateTime)
        {
            DateTime now;
            string[] format = new string[]
            {
            "yyyyMMddHHmmss", "yyyy-MM-dd HH:mm:ss", "yyyy年MM月dd日 HH时mm分ss秒",
            "yyyyMdHHmmss","yyyy年M月d日 H时mm分ss秒", "yyyy.M.d H:mm:ss", "yyyy.MM.dd HH:mm:ss","yyyy-MM-dd","yyyyMMdd"
            ,"yyyy/MM/dd","yyyy/M/d","yyyy/M/dd","dd-MM月-yyyy"
            };
            if (DateTime.TryParseExact(strDateTime, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out now))
            {
                return now;
            }
            return DateTime.MinValue;
        }

        //循环去除datatable中的空行
        public DataTable removeEmpty(DataTable dt)
        {
            List<DataRow> removelist = new List<DataRow>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                bool rowdataisnull = true;
                for (int j = 0; j < dt.Columns.Count; j++)
                {

                    if (!string.IsNullOrEmpty(dt.Rows[i][j].ToString().Trim()))
                    {

                        rowdataisnull = false;
                    }

                }
                if (rowdataisnull)
                {
                    removelist.Add(dt.Rows[i]);
                }

            }
            for (int i = 0; i < removelist.Count; i++)
            {
                dt.Rows.Remove(removelist[i]);
            }
            return dt;
        }

    }
}
