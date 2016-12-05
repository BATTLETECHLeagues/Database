USE [Master]
GO

DECLARE @dbname nvarchar(128)
SET @dbname = N'BCAPIDatabase_Dev'

IF (EXISTS (SELECT name 
FROM master.dbo.sysdatabases 
WHERE ('[' + name + ']' = @dbname 
OR name = @dbname)))

	BEGIN
		DROP DATABASE [BCAPIDatabase_Dev]
	END

	CREATE DATABASE [BCAPIDatabase_Dev] ON  PRIMARY 
	( NAME = N'BCAPIDatabase_Dev', FILENAME = N'C:\Development\BCDatabase\data\BCAPIDatabase_Dev.mdf' , 
	SIZE = 128MB , MAXSIZE = 8GB, FILEGROWTH = 1GB )
	LOG ON 
	( NAME = N'BCAPIDatabase_Dev_log', FILENAME = N'C:\Development\BCDatabase\data\BCAPIDatabase_Dev_log.ldf' , 
	SIZE = 128MB , MAXSIZE = 2GB , FILEGROWTH = 10%)
GO