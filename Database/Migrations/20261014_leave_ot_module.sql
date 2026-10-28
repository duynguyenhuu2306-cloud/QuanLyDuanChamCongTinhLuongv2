/* Chạy một lần trên SQL Server trước khi triển khai module Nghỉ phép & OT. */
IF COL_LENGTH('NghiPhep', 'Lydo') IS NULL
    ALTER TABLE NghiPhep ADD Lydo nvarchar(500) NULL;
GO

IF COL_LENGTH('NghiPhep', 'LydoTuChoi') IS NULL
    ALTER TABLE NghiPhep ADD LydoTuChoi nvarchar(500) NULL;
GO

UPDATE NghiPhep SET Lydo = N'' WHERE Lydo IS NULL;
GO
