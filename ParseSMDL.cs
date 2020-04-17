using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Collections;
using System.Data.SqlClient;
using System.Data;

namespace ParseSMDL
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlTextReader xtr = new XmlTextReader("C:\\temp\\smdl\\extract1.xml");
            int fieldcount = 0;
            string entityid=null;
            string entityName=null;
            string baseTableName = null;
            xtr.WhitespaceHandling = WhitespaceHandling.None;
            xtr.Read(); // read the XML declaration node, advance to <suite> tag

            while (!xtr.EOF) //load loop
            {
                if (xtr.Name == "Entities" && !xtr.IsStartElement())
                    break;
                if (xtr.Name == "Table" && xtr.IsStartElement())
                {
                    Console.WriteLine(xtr.GetAttribute("Name"));
                    baseTableName = xtr.GetAttribute("Name");
                    // ToDo Write a logic to update all the record for the EntityID to include the  BaseTableName
                    CallToUpdate(entityid, baseTableName);
                }

                if (xtr.Name == "Entity" && xtr.IsStartElement())
                {
                    Console.WriteLine(xtr.GetAttribute("ID"));//
                    entityid = xtr.GetAttribute("ID");
                    xtr.Read();
                    xtr.Read();
                    Console.WriteLine(xtr.Value);
                    entityName = xtr.Value;
                    xtr.Read(); // advance to Name tag Value

                    while (xtr.Name != "Fields")
                        xtr.Read(); // advance to Fields tag
                    //bool state = true;

                    do
                    {

                        if (xtr.Name == "Fields" && xtr.IsStartElement())
                        {

                            fieldcount++;
                            Console.WriteLine(xtr.LineNumber);
                            Console.WriteLine(fieldcount);
                        }
                        if (xtr.Name == "Fields" && !xtr.IsStartElement())
                            if (fieldcount == 1)
                                break;
                            else
                                fieldcount--;

                        if (xtr.Name == "Role" && xtr.IsStartElement())
                        {
                            while (true)
                            {

                                xtr.Read();
                                if (xtr.Name == "Role" && !xtr.IsStartElement())
                                    break;
                            }
                        }


                        if (xtr.Name == "Attribute" && xtr.IsStartElement())
                        {
                            //Changing the logic to include a function call by reference. 
                            ManipulateEntity(ref xtr, ref entityid,ref entityName ,ref baseTableName);
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

                    } while (true);
                }
                if (xtr.Name == "Entity" && !xtr.IsStartElement())
                {
                    Console.WriteLine("AnotherEntity");
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
                

            } // load loop


        }

        private static void ManipulateEntity(ref XmlTextReader xtr, ref string entityid, ref string entityName, ref string baseTableName)
        {
                string AttributeID = null;
                string AttributeName = null;
                string ColumnName = null;
                string AttributeType = "Dimension";
                Console.WriteLine(xtr.GetAttribute("ID"));
                AttributeID = xtr.GetAttribute("ID");
                xtr.Read();
                while (true)
                {
                    if ((xtr.Name == "Attribute" && !xtr.IsStartElement()))
                    {
                        break;
                    }
                    if ((xtr.Name == "Name" &&  xtr.IsStartElement()))
                    {
                        xtr.Read();
                        Console.WriteLine(xtr.Value);
                        AttributeName = xtr.Value;
                    }
                    if ((xtr.Name == "Column" && xtr.IsStartElement()))
                    {
                        Console.WriteLine(xtr.GetAttribute("Name"));
                        ColumnName = xtr.GetAttribute("Name");
                        Program.CallToInsert(entityid, entityName, baseTableName, AttributeType, AttributeID, AttributeName, ColumnName, null, null);
                    }
                    if ((xtr.Name == "Variations" && xtr.IsStartElement()))
                    {
                        string vAttributeType = "Measure";
                        string vAttributeID = null;
                        string vAttributeName = null;
                        string vFunction = null;
                        string vDepAt = null;
                        xtr.Read();
                        while (true)
                        {
                            if ((xtr.Name =="Variations" && !xtr.IsStartElement()))
                            {
                                break;
                            }
                            if ((xtr.Name == "Attribute" && xtr.IsStartElement()))
                            {
                                Console.WriteLine(xtr.GetAttribute("ID"));
                                vAttributeID = xtr.GetAttribute("ID");
                                xtr.Read();
                            }
                            if ((xtr.Name == "Name" && xtr.IsStartElement()))
                            {
                                xtr.Read();
                                vAttributeName = xtr.Value;
                                Console.WriteLine(xtr.Value);
                            }
                            if ((xtr.Name == "Expression" && xtr.IsStartElement()))
                            {
                                xtr.Read();
                                xtr.Read();
                                xtr.Read();
                                vFunction = xtr.Value;
                                Console.WriteLine(xtr.Value);
                                xtr.Read();
                                xtr.Read();
                                xtr.Read();
                                xtr.Read();
                                xtr.Read();
                                xtr.Read();
                                Console.WriteLine(xtr.Value);
                                vDepAt = xtr.Value;
                                Program.CallToInsert(entityid, entityName, baseTableName, vAttributeType, vAttributeID, vAttributeName, null, vFunction, vDepAt);
                            }
                            xtr.Read();
                        }
                    }
                    if ((xtr.Name == "Expression" && xtr.IsStartElement()))
                    {
                        string Function = null;
                        string DepAt = null;
                        xtr.Read();
                        xtr.Read();
                        xtr.Read();
                        Function = xtr.Value;
                        Console.WriteLine(xtr.Value);
                        xtr.Read();
                        xtr.Read();
                        xtr.Read();
                        xtr.Read();
                        xtr.Read();
                        xtr.Read();
                        Console.WriteLine(xtr.Value);
                        DepAt = xtr.Value;
                        Program.CallToInsert(entityid, entityName, baseTableName, AttributeType, AttributeID, AttributeName, ColumnName, Function, DepAt);
                    }
                    xtr.Read();
                }
                

        }

        static void CallToInsert(string EntityID,string EntityName,string BaseTableName,string Attribute_type, string Att_id, string Att_name,
                                    string ColName,string Expression,string Expres_depend_id)
        {
            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.conString))
            {
                // Create a SqlCommand, and identify it as a stored procedure.
                using (SqlCommand sqlCommand = new SqlCommand("dbo.usp_insertEntities", connection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                    // Add input parameter for the stored procedure and specify what to use as its value.
                    sqlCommand.Parameters.Add(new SqlParameter("@EntityID", SqlDbType.NVarChar,400));
                    sqlCommand.Parameters["@EntityID"].Value = EntityID;

                    sqlCommand.Parameters.Add(new SqlParameter("@EntityName", SqlDbType.NVarChar, 100));
                    sqlCommand.Parameters["@EntityName"].Value = EntityName.ToString();

                    sqlCommand.Parameters.Add(new SqlParameter("@BaseTableName", SqlDbType.NVarChar, 100));
                    if ( string.IsNullOrEmpty(BaseTableName))
                        sqlCommand.Parameters["@BaseTableName"].Value = DBNull.Value;
                    else
                    sqlCommand.Parameters["@BaseTableName"].Value = BaseTableName.ToString();

                    sqlCommand.Parameters.Add(new SqlParameter("@Attribute_type", SqlDbType.NVarChar, 10));
                    sqlCommand.Parameters["@Attribute_type"].Value = Attribute_type.ToString();

                    sqlCommand.Parameters.Add(new SqlParameter("@Att_id", SqlDbType.NVarChar, 400));
                    sqlCommand.Parameters["@Att_id"].Value = Att_id;

                    sqlCommand.Parameters.Add(new SqlParameter("@Att_name", SqlDbType.NVarChar, 100));
                    sqlCommand.Parameters["@Att_name"].Value = Att_name.ToString();

                    sqlCommand.Parameters.Add(new SqlParameter("@ColName", SqlDbType.NVarChar, 100));
                    if (string.IsNullOrEmpty(ColName))
                        sqlCommand.Parameters["@ColName"].Value = DBNull.Value;
                    else
                        sqlCommand.Parameters["@ColName"].Value = ColName.ToString();

                    sqlCommand.Parameters.Add(new SqlParameter("@Expression", SqlDbType.NVarChar, 10));
                    if (string.IsNullOrEmpty(Expression))
                        sqlCommand.Parameters["@Expression"].Value = DBNull.Value;
                    else
                        sqlCommand.Parameters["@Expression"].Value = Expression.ToString();

                    sqlCommand.Parameters.Add(new SqlParameter("@Expres_depend_id", SqlDbType.NVarChar, 400));
                    if (string.IsNullOrEmpty(Expres_depend_id))
                        sqlCommand.Parameters["@Expres_depend_id"].Value = DBNull.Value;
                    else
                        sqlCommand.Parameters["@Expres_depend_id"].Value = Expres_depend_id;

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


        static void CallToUpdate(string EntityID,string BaseTableName)
        {
            //            usp_updateEntities
            //(
            //@EntityID uniqueidentifier,
            //@BaseTableName Nvarchar(100)
            //)

            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.conString))
            {
                // Create a SqlCommand, and identify it as a stored procedure.
                using (SqlCommand sqlCommand = new SqlCommand("dbo.usp_updateEntities", connection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                    // Add input parameter for the stored procedure and specify what to use as its value.
                    sqlCommand.Parameters.Add(new SqlParameter("@EntityID", SqlDbType.NVarChar,100));
                    sqlCommand.Parameters["@EntityID"].Value = EntityID.ToString();

                    sqlCommand.Parameters.Add(new SqlParameter("@BaseTableName", SqlDbType.NVarChar, 100));
                    sqlCommand.Parameters["@BaseTableName"].Value = BaseTableName.ToString();

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
