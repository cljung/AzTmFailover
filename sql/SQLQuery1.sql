create table TM_ReadWrite_test
(
	webserver		varchar(64)			not null
,	probetimeutc	datetime			not null	
)
go


select * from TM_readWrite_test

delete from TM_readWrite_test
where datediff( minute, probetimeutc, getutcdate() ) > 60


insert into TM_ReadWrite_test ( webserver, probetimeutc ) select 'localhost', GETUTCDATE()
delete from TM_readWrite_test where datediff( minute, probetimeutc, getutcdate() ) > 60

begin transaction
delete from TM_ReadWrite_test

rollback transaction
