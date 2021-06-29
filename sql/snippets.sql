--EXEC sp_rename '[BigQuery].[Servel].CreatedQuery', 'TerrID', 'COLUMN';
select *  from VerificaYa.Correos order by CreatedOn desc

--select *  from Smartcob.Pagos order by CreatedOn desc
--delete  from Smartcob.Pagos where CreatedFromProcess = 'Test'

--select *  from Smartcob.Receptores order by CreatedOn desc

--select *  from Smartcob.CPH_PT order by CreatedOn desc

-- select *  from Smartcob.Mensajes order by CreatedOn desc

-- drop TYPE ut_Sinacofi_BienesRaices
--select * from Sinacofi.BienesRaices

--select * from Sinacofi.Vehiculos

--select * from Sinacofi.Direcciones

--alter table BigQuery.Servel add CreatedByUser VARCHAR(80);
