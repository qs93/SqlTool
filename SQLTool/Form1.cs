using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using SQLTool.Common.Helper;
using SQLTool.Common.PetaPoco;

namespace SQLTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var providerName = ConfigurationManager.AppSettings["ProviderName"].ToString();
            var providerNameList = providerName.Split('|').ToList();
            providerNameList.ForEach(p =>
            {
                cbxProviderName.Items.Add(p);
            });

            XmlModel model = XmlHelper.Get();
            txtConnectionString.Text = model.ConnectionString;
            cbxProviderName.Text = model.ProviderName;


        }

        /// <summary>
        /// 查询数据库表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            XmlModel model = new XmlModel()
            {
                ConnectionString = txtConnectionString.Text,
                ProviderName = cbxProviderName.Text
            };
            XmlHelper.Update(model);
            try
            {
                Database db = new Database();
                DataTable dt = db.GetDataTable("select name as TableName from sys.tables");

                gvDataTable.DataSource = dt;
            }
            catch
            {
                gvDataTable.DataSource = new DataTable();
                MessageBox.Show("连接失败");
            }
        }

        /// <summary>
        /// 到处Word
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            Database db = new Database();
            var wordList = new List<WordModel>();
            foreach (DataGridViewRow dgvr in gvDataTable.Rows)
            {
                var checkBoxCell = (DataGridViewCheckBoxCell)dgvr.Cells["Selected"];
                var isChecked = Convert.ToBoolean(checkBoxCell.Value);
                if (isChecked)
                {
                    var tableName = dgvr.Cells["TableName"].Value.ToString();
                    DataTable tbDt = db.GetDataTable(@"select col.name as 字段,t.name as 类型,col.max_length as 长度,case when col.is_nullable=0 then 'N' else 'Y' end as 是否为空,ep.value as 说明  
                                        from sys.objects obj 
                                        inner join sys.columns col on obj.object_id=col.object_id 
                                        left join sys.types t on t.user_type_id=col.user_type_id
                                        left join sys.extended_properties ep on ep.major_id=obj.object_id and ep.minor_id=col.column_id
                                        where obj.name=@0", tableName);
                    wordList.Add(new WordModel()
                    {
                        TableDt = tbDt,
                        TableName = tableName
                    });
                }
            }
            if (wordList.Count > 0)
            {
                WordHelper.ExportDataListToWord(wordList);
            }
            else
            {
                MessageBox.Show("没有选中表");
            }
        }

        /// <summary>
        /// 全选&全不选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (gvDataTable.Rows.Count == 0)
            {
                MessageBox.Show("没有数据");
                return;
            }
            bool isAllChecked = button3.Text == "全选";
            button3.Text = isAllChecked ? "全不选" : "全选";
            foreach (DataGridViewRow dgvr in gvDataTable.Rows)
            {
                var checkBoxCell = (DataGridViewCheckBoxCell)dgvr.Cells["Selected"];
                checkBoxCell.Value = isAllChecked;
            }
        }
    }
}
