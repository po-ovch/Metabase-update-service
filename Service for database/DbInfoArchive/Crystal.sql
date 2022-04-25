USE [Crystal]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create OR ALTER view [dbo].[MetaProperties] as 
select '1' as [DBID], [NOMPROP] as PropId,
[NAZVPROP] as [Name], '' as [Description],
[HTML] as [WWWTemplatePage], [UpdateStatus]
	  from _PropertiesConv
GO