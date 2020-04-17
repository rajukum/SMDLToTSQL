# SMDLToTSQL.  
Convert SMDL to T-SQL (Work in progress). I have not organized the solution completely and it is rough implementation of my thought process. 

SMDL query specs
https://docs.microsoft.com/en-us/openspecs/sql_data_portability/ms-smdl/266461ea-ff84-4ed8-8916-ceca77e409f0

If we look at the Query we can see two type of query, one which is grouping which are mostly the query behind the parameters and another where you will see Details which are for the Tablix in the reports. As an example we can see the following. 

Parameter based query :-- 
<SemanticQuery xmlns="http://schemas.microsoft.com/sqlserver/2004/10/semanticmodeling" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:qd="http://schemas.microsoft.com/sqlserver/2004/11/semanticquerydesign" xmlns:rb="http://schemas.microsoft.com/sqlserver/2004/11/reportbuilder">
  <Hierarchies>
    <Hierarchy>
      <BaseEntity>
        <!--SomeTable-->
        <EntityID>Gcdc54da2-9df1-4cf7-b2e9-18a3cb523036</EntityID>
      </BaseEntity>
<Groupings>
<Grouping Name="Some Name">
          <Expression Name="Some Name">
            <AttributeRef>
              <!--Some Name-->
              <AttributeID>G8908eaff-7455-4695-ab45-896d6e94329f</AttributeID>
            </AttributeRef>
          </Expression>
 </Grouping>
 <Grouping Name="Some Other Name">
          <Expression Name="Some Other Name">
            <AttributeRef>
              <!--Some Other Name-->
              <AttributeID>G02769611-5665-48f8-9ee1-898c261ea05f</AttributeID>
            </AttributeRef>
          </Expression>
</Grouping>

Detail based query :-- 
<SemanticQuery xmlns="http://schemas.microsoft.com/sqlserver/2004/10/semanticmodeling" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:qd="http://schemas.microsoft.com/sqlserver/2004/11/semanticquerydesign" xmlns:rb="http://schemas.microsoft.com/sqlserver/2004/11/reportbuilder">
  <Hierarchies>
    <Hierarchy>
      <BaseEntity>
        <!--Some Table-->
        <EntityID>Gd6c95314-28e1-4b11-a96f-41620f487925</EntityID>
      </BaseEntity>
<Groupings>
        <Grouping Name="Some Column">
          <Expression Name="Some Column">
            <EntityRef>
              <!--Some Column-->
              <EntityID>Gd6c95314-28e1-4b11-a96f-41620f487925</EntityID>
            </EntityRef>
          </Expression>
          <Details>
            <Expression Name="Some Column">
              <AttributeRef>
                <!--Some Column-->
                <AttributeID>Gd872745a-2aac-41bd-8206-6f839523363f</AttributeID>
              </AttributeRef>
            </Expression>

First thing was to get a repository created with all the entity and the corresponding Attribute ID. When we look at the SMDL its an xml file, if we write a program which can look at the SMDL XML content and identify all the attribute that woudl be handy while looking at the SMDL we can see 3 different data which we can extract which can help. 

a) All the Entity and their corresponding attribute name and ID. 
b) DSV parsing where we can understand what was the original table name used in the SMDL. 
c) What is the relationship between the tables we should use to relate in case a Detail data set uses more than one table. 

I have created 3 solutions which takes the xml file as an input and then write them to 3 SQL Tables 
a) Entity

CREATE TABLE [dbo].[Entities](
	[EntityID] [nvarchar](100) NULL,
	[EntityName] [nvarchar](100) NULL,
	[BaseTableName] [nvarchar](100) NULL,
	[Attribute_type] [nvarchar](10) NULL,
	[Att_id] [nvarchar](100) NULL,
	[Att_name] [nvarchar](100) NULL,
	[ColName] [nvarchar](100) NULL,
	[Expression] [nvarchar](10) NULL,
	[Expres_depend_id] [nvarchar](100) NULL
) ON [PRIMARY]
GO

b) DSV

CREATE TABLE [dbo].[dsv](
	[EntityName] [nvarchar](100) NULL,
	[QueryDefination] [nvarchar](2000) NULL,
	[TableName] [nvarchar](100) NULL,
	[FriendlyName] [nvarchar](100) NULL,
	[TableType] [nvarchar](100) NULL
) ON [PRIMARY]
GO

c) Relationship 

CREATE TABLE [dbo].[relationship](
	[RelationshipName] [nvarchar](150) NULL,
	[ParentTable] [nvarchar](150) NULL,
	[ChildTable] [nvarchar](150) NULL,
	[ParentKey] [nvarchar](150) NULL,
	[ChildKey] [nvarchar](150) NULL
) ON [PRIMARY]
GO

Once we have the tables pupulated we can then generate the final dataset parsing by doing lookup. I have started working on it but have not finished it yet. 
