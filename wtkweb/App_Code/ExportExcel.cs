using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;

	public static class ExportExcel
	{
		private static string ExcelGetCharecter(int i)
		{
			switch (i)
			{
				case 1:
					{
						return "A";
					}
				case 2:
					{
						return "B";
					}
				case 3:
					{
						return "C";
					}
				case 4:
					{
						return "D";
					}
				case 5:
					{
						return "E";
					}
				case 6:
					{
						return "F";
					}
				case 7:
					{
						return "G";
					}
				case 8:
					{
						return "H";
					}
				case 9:
					{
						return "I";
					}
				case 10:
					{
						return "J";
					}
				default:
					{
						return "Z";
					}
			}
		}

		public static bool Export(string FileName, System.Data.DataTable LvList)
		{
			object m_objOpt = System.Reflection.Missing.Value;
			Excel.Application m_objExcel = new Excel.Application();
			Excel.Workbooks m_objBooks = (Excel.Workbooks)m_objExcel.Workbooks;
			Excel._Workbook m_objBook = (Excel._Workbook)(m_objBooks.Add(m_objOpt));
			Excel.Sheets m_objSheets = (Excel.Sheets)m_objBook.Worksheets;
			Excel._Worksheet m_objSheet = (Excel._Worksheet)(m_objSheets.get_Item(1));
			Excel.Range m_objRange = null;
			try
			{
				// Start a new workbook in Excel.

				// Add data to cells in the first worksheet in the new workbook.
				
				for (int i = 0; i < LvList.Columns.Count; i++)
				{
					m_objRange = m_objSheet.get_Range(ExcelGetCharecter(i + 1) + "1", m_objOpt);
					m_objRange.set_Value(Type.GetType("string"), LvList.Columns[i].Caption);
				}

				for (int i = 0; i < LvList.Rows.Count; i++)
				{
					for (int j = 0; j < LvList.Columns.Count; j++)
					{
						string s = System.Web.HttpUtility.UrlEncode(LvList.Rows[i][j].ToString().Trim(), System.Text.Encoding.UTF8);
						//if ( j == 0 )
						//s = "'" + s;
						m_objRange = m_objSheet.get_Range(ExcelGetCharecter(j + 1) + (i + 2).ToString(), m_objOpt);
						m_objRange.set_Value(Type.GetType("string"), s);
					}
				}
				FileInfo f = new FileInfo(FileName);
				if (f.Exists)
					f.Delete();
				
				m_objBook.SaveAs(FileName, Excel.XlFileFormat.xlWorkbookNormal,
					m_objOpt, m_objOpt, m_objOpt, m_objOpt, Excel.XlSaveAsAccessMode.xlNoChange, m_objOpt, m_objOpt,
					m_objOpt, m_objOpt, m_objOpt);
				

			}
			catch (Exception ex)
			{
				//MessageBox.Show( ex.Message );
				//Response.Write("<script>alert('导出EXCEL出现错误！')</script>");
				return false;
			}
			finally
			{
				m_objBook.Close(false, m_objOpt, m_objOpt);
				m_objBooks.Close();
				m_objExcel.Quit();
				System.Runtime.InteropServices.Marshal.ReleaseComObject(m_objRange);
				System.Runtime.InteropServices.Marshal.ReleaseComObject(m_objSheet);
				System.Runtime.InteropServices.Marshal.ReleaseComObject(m_objSheets);
				System.Runtime.InteropServices.Marshal.ReleaseComObject(m_objBook);
				System.Runtime.InteropServices.Marshal.ReleaseComObject(m_objBooks);
				System.Runtime.InteropServices.Marshal.ReleaseComObject(m_objExcel);
				GC.Collect();
			}
			return true;
		}
	}

