using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace ParseRelation
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlTextReader xtr = new XmlTextReader("D:\\PFE\\Xerox\\smdl\\XSMDataWarehouse.dsv");
            xtr.WhitespaceHandling = WhitespaceHandling.None;
            xtr.Read(); // read the XML declaration node, advance to <suite> tag

            while (!xtr.EOF) //load loop
            {
                if (xtr.Name == "appinfo" && !xtr.IsStartElement())
                    break;

                if (xtr.Name == "msdata:Relationship" && xtr.IsStartElement())
                {

                    Console.WriteLine(xtr.GetAttribute("name"));


                    Console.WriteLine(xtr.GetAttribute("msdata:parent"));
                    Console.WriteLine(xtr.GetAttribute("msdata:child"));
                    Console.WriteLine(xtr.GetAttribute("msdata:parentkey"));
                    Console.WriteLine(xtr.GetAttribute("msdata:childkey"));

                    CallToInsert(xtr.GetAttribute("name"), xtr.GetAttribute("msdata:parent"), xtr.GetAttribute("msdata:child"), xtr.GetAttribute("msdata:parentkey"), xtr.GetAttribute("msdata:childkey"));
                }

                try
                {
                    xtr.Read();
                }
                catch (SystemException ex)
                {
                    Console.WriteLine(ex.Message);
                    xtr.Read();
                }
            }
        }


        static void CallToInsert(string RelationshipName, string ParentTable, string ChildTable, string ParentKey, string ChildKey)
        {
            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.connString))
            {
                // Create a SqlCommand, and identify it as a stored procedure.
                using (SqlCommand sqlCommand = new SqlCommand("dbo.usp_insertrelationship", connection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                    // Add input parameter for the stored procedure and specify what to use as its value.
                    sqlCommand.Parameters.Add(new SqlParameter("@RelationshipName", SqlDbType.NVarChar, 150));
                    sqlCommand.Parameters["@RelationshipName"].Value = RelationshipName;

                    sqlCommand.Parameters.Add(new SqlParameter("@ParentTable", SqlDbType.NVarChar, 150));
                    sqlCommand.Parameters["@ParentTable"].Value = ParentTable;

                    sqlCommand.Parameters.Add(new SqlParameter("@ChildTable", SqlDbType.NVarChar, 150));
                    sqlCommand.Parameters["@ChildTable"].Value = ChildTable;

                    sqlCommand.Parameters.Add(new SqlParameter("@ParentKey", SqlDbType.NVarChar, 150));
                    sqlCommand.Parameters["@ParentKey"].Value = ParentKey;

                    sqlCommand.Parameters.Add(new SqlParameter("@ChildKey", SqlDbType.NVarChar, 150));
                    sqlCommand.Parameters["@ChildKey"].Value = ChildKey;

                    // Add the output parameter.

                    try
                    {
                        connection.Open();

                        // Run the stored procedure.
                        sqlCommand.ExecuteNonQuery();
                        
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }

                }
            }
        }
    }
}


