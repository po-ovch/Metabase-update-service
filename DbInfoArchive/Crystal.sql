USE [Crystal]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create OR ALTER view [dbo].[SystemInfo] as 
select  1 as [DBID], 
		[HeadClue] as SystemId,
		count(HeadClue) as ElemNumber,
		1 as UpdateStatus, 
		GETDATE() as _date,
		[Help] as [Elements],
		[System] as SystemInfo,
        null as [Description]
	from dbo.HeadTabl
cross apply string_split(trim('-' from Help), '-')
group by HeadClue, Help, System
GO

create OR ALTER view [dbo].[PropertiesInfo] as 
select  1 as [DBID], 
		[NOMPROP] as PropId,
		[NAZVPROP] as [Name], 
		'' as [Description],
		[HTML] as [WWWTemplatePage], 
		1 as [UpdateStatus]
	from dbo.Properties
GO