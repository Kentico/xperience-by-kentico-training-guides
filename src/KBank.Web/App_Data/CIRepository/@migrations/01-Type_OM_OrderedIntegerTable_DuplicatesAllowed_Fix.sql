drop type [Type_OM_OrderedIntegerTable_DuplicatesAllowed]

CREATE TYPE [dbo].[Type_OM_OrderedIntegerTable_DuplicatesAllowed] AS TABLE(
	[Value] [int] NOT NULL,
	PRIMARY KEY CLUSTERED 
(
	[Value] ASC
)WITH (IGNORE_DUP_KEY = ON)
)
