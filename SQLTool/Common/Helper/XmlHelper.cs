using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SQLTool.Common.Helper
{
    public class XmlHelper
    {
        public static void Update(XmlModel model)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(model.XmlPath);
            XmlNode nodel = xml.SelectSingleNode("/connectionStrings/add[@name='connection']");
            nodel.Attributes["connectionString"].Value = model.ConnectionString;
            nodel.Attributes["providerName"].Value = model.ProviderName;
            xml.Save(model.XmlPath);
        }

        public static XmlModel Get()
        {
            XmlModel model = new XmlModel();
            XmlDocument xml = new XmlDocument();
            xml.Load(model.XmlPath);
            XmlNode nodel = xml.SelectSingleNode("/connectionStrings/add[@name='connection']");
            model.ConnectionString = nodel.Attributes["connectionString"].Value;
            model.ProviderName = nodel.Attributes["providerName"].Value;
            return model;
        }
    }

    public class XmlModel
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string XmlPath { get; set; } = "config/connectionString.xml";
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// 数据库Provider
        /// </summary>
        public string ProviderName { get; set; }
    }
}
