USE [BandGap]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


create OR ALTER view [dbo].[SystemInfo] as 
select  3 as [DBID], 
		Substances.[SubstanceId] as SystemId,
		count(SubstanceId) as ElemNumber,
		1 as UpdateStatus, 
		GETDATE() as _date,
		dbo.GetElements(NumElements, El1, El2, El3, El4, [Elements], Compound) as [Elements],
		Compound as SystemInfo,
		null as [Description]
	from Substances
cross apply string_split(trim('-' from dbo.GetElements(NumElements, El1, El2, El3, El4, [Elements], Compound)), '-')
group by SubstanceId, NumElements, El1, El2, El3, El4, [Elements], Compound
GO

create OR ALTER view [dbo].[PropertiesInfo] as 
select  3 as [DBID], 
		[NOMPROP] as PropId,
		[NAZVPROP] as [Name], 
		'' as [Description],
		[HTML] as [WWWTemplatePage], 
	    [UpdateStatus]
	  from _PropertiesConv
GO