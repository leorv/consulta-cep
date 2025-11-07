USE [CEP]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CEP] (
[Id] INT IDENTITY (1, 1) NOT NULL,
[cep] CHAR (9)       NULL,
[logradouro] NVARCHAR (500) NULL,
[complemento] NVARCHAR (500) NULL,
[bairro] NVARCHAR (500) NULL,
[localidade] NVARCHAR (500) NULL,
[uf] CHAR (2) NULL,
[unidade] BIGINT         NULL,
[ibge]        INT            NULL,
[gia]         NVARCHAR (500) NULL
);