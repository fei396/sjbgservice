using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileConverter
{
    /// <summary>
    ///PdfConverter 的摘要说明
    /// </summary>

    public class FileConverter
    {
        //
        //TODO: 在此处添加构造函数逻辑
        //


        public static string ToPdf(string filePath,string fileName, string extName)
        {
            switch (extName.ToLower())
            {
                case "doc":
                    return DocToPdf(filePath,fileName);
                case "xls":
                    return XlsToPdf(filePath,fileName);
                default:
                    return "无需转换";
            }
        }
        private static string DocToPdf(string filePath,string fileName)
        {
            try
            {
                Aspose.Words.Document doc = new Aspose.Words.Document(filePath + fileName + ".doc");
                doc.Save(filePath + fileName + ".pdf", Aspose.Words.SaveFormat.Pdf);
                
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "true";
        }

        private static string XlsToPdf(string filePath,string fileName)
        {
            Aspose.Cells.Workbook book = new Aspose.Cells.Workbook(filePath + fileName + ".xls");
            book.Save(filePath + fileName + ".pdf", Aspose.Cells.SaveFormat.Pdf);
            return "true";
        }
    }
}