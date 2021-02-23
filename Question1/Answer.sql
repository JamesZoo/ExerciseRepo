CREATE TYPE [dbo].[CustomTableType] AS TABLE  
(  
[Id] [int] NOT NULL,
[Value1] [int] NOT NULL,
[Value2] [int] NULL,
PRIMARY KEY (Id)
);
GO

CREATE FUNCTION dbo.CompareTables(@Table1 dbo.CustomTableType READONLY, @Table2 dbo.CustomTableType READONLY)  
RETURNS int   
AS   
BEGIN
    RETURN
	(
		CASE 
			WHEN EXISTS	(SELECT * FROM @Table1 EXCEPT SELECT * FROM @Table2) THEN 1
			ELSE 0
		END
	);
END
