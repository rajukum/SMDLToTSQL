using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ParseDSV
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlTextReader xtr = new XmlTextReader("C:\\Temp\\smdl\\XSMDataWarehouse.xml");
            xtr.WhitespaceHandling = WhitespaceHandling.None;
            xtr.Read(); // read the XML declaration node, advance to <suite> tag

            while (!xtr.EOF) //load loop
            {
                if (xtr.Name == "Schema" && !xtr.IsStartElement())
                    break;
                
                if (xtr.Name== "xs:element" && xtr.IsStartElement() && xtr.GetAttribute("msprop:DbTableName") !=null)
                {
                    
                    Console.WriteLine(xtr.GetAttribute("name"));

                    Console.WriteLine(xtr.GetAttribute("msprop:QueryDefinition"));
                    Console.WriteLine(xtr.GetAttribute("msprop:DbTableName"));
                    Console.WriteLine(xtr.GetAttribute("msprop:FriendlyName"));
                    Console.WriteLine(xtr.GetAttribute("msprop:TableType"));
                    CallToInsert(xtr.GetAttribute("name"), xtr.GetAttribute("msprop:QueryDefinition"), xtr.GetAttribute("msprop:DbTableName"), xtr.GetAttribute("msprop:FriendlyName"), xtr.GetAttribute("msprop:TableType"));
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

        static void CallToInsert(string EntityName, string QueryDefination, string TableName, string FriendlyName, string TableType)
        {
            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.connString))
            {
                // Create a SqlCommand, and identify it as a stored procedure.
                using (SqlCommand sqlCommand = new SqlCommand("dbo.usp_insertdsv", connection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                    // Add input parameter for the stored procedure and specify what to use as its value.
                    sqlCommand.Parameters.Add(new SqlParameter("@EntityName", SqlDbType.NVarChar, 100));
                    sqlCommand.Parameters["@EntityName"].Value = EntityName;

                    sqlCommand.Parameters.Add(new SqlParameter("@QueryDefination", SqlDbType.NVarChar, 2000));
                    sqlCommand.Parameters["@QueryDefination"].Value = QueryDefination;

                    sqlCommand.Parameters.Add(new SqlParameter("@TableName", SqlDbType.NVarChar, 100));
                    sqlCommand.Parameters["@TableName"].Value = TableName;

                    sqlCommand.Parameters.Add(new SqlParameter("@FriendlyName", SqlDbType.NVarChar, 100));
                    sqlCommand.Parameters["@FriendlyName"].Value = FriendlyName;

                    sqlCommand.Parameters.Add(new SqlParameter("@TableType", SqlDbType.NVarChar, 100));
                    sqlCommand.Parameters["@TableType"].Value = TableType;

                    // Add the output parameter.

                    try
                    {
                        connection.Open();

                        // Run the stored procedure.
                        sqlCommand.ExecuteNonQuery();
                        // Customer ID is an IDENTITY value from the database.
                        // Put the Customer ID value into the read-only text box.
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
