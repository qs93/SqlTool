using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace SQLTool.Common.Helper
{
    public class WordHelper
    {
        /// <summary>
        /// 生成Word
        /// </summary>
        /// <param name="wordDtList"></param>
        public static void ExportDataListToWord(List<WordModel> wordDtList)
        {
            if (wordDtList.Count == 0)
            {
                MessageBox.Show("没有数据可供导出！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                SaveFileDialog sfile = new SaveFileDialog()
                {
                    FileName = DateTime.Now.ToString("yyyyMMddHHmmsss"),
                    AddExtension = true,
                    DefaultExt = ".doc",
                    Filter = "(*.doc)|*.doc",
                };
                //if (sfile.ShowDialog() == DialogResult.OK)
                //{

                object path = System.IO.Path.GetFullPath("word/" + sfile.FileName + sfile.DefaultExt);
                Object missing = System.Reflection.Missing.Value;
                Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();

                Document document = wordApp.Documents.Add(ref missing, ref missing, ref missing, ref missing);
                object WdLine = WdUnits.wdLine;//换一行;
                try
                {
                    int cellNum = 1;
                    foreach (var wordModel in wordDtList)
                    {
                        System.Data.DataTable srcDgv = wordModel.TableDt;
                        var rowCount = srcDgv.Rows.Count;
                        var columnsCount = srcDgv.Columns.Count;
                        //建立表格
                        Microsoft.Office.Interop.Word.Table table = document.Tables.Add(wordApp.Selection.Range, rowCount + 3, columnsCount, ref missing, ref missing);


                        table.Cell(1, 1).Merge(table.Cell(1, columnsCount));  //合并第一行
                        table.Cell(1, 1).Range.InsertAfter(wordModel.TableName);  //输出表名
                        table.Cell(1, 1).Range.Font.Bold = 2;  //粗体
                        //table.Cell(1, 1).Range.Font.Size = 24; //字体大小

                        for (int i = 0; i < columnsCount; i++)//输出标题
                        {
                            table.Cell(2, i + 1).Range.InsertAfter(srcDgv.Columns[i].ColumnName);

                        }

                        //输出控件中的记录

                        for (int i = 0; i < rowCount; i++)
                        {
                            for (int j = 0; j < columnsCount; j++)
                            {
                                table.Cell(i + 3, j + 1).Range.InsertAfter(srcDgv.Rows[i][j].ToString());
                            }

                        }
                        cellNum += rowCount + 3;
                        table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                        table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;

                        object tcount = cellNum;
                        wordApp.Selection.MoveDown(ref WdLine, ref tcount, ref missing);//移动焦点
                        wordApp.Selection.TypeParagraph();//插入段落
                        System.Threading.Thread.Sleep(500);
                    }

                    document.SaveAs(ref path, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);

                    document.Close();

                    MessageBox.Show("数据已经成功导出到：" + path, "导出完成", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "提示", MessageBoxButtons.OK);
                }

                //}
            }

        }
    }

    public class WordModel
    {
        public System.Data.DataTable TableDt { get; set; }

        public string TableName { get; set; }
    }
}
