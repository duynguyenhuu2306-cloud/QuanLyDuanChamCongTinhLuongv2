/* Chạy một lần trên SQL Server trước khi triển khai API đã cập nhật. */
IF COL_LENGTH('ChamCong', 'IsDeleted') IS NULL
    ALTER TABLE ChamCong ADD IsDeleted bit NOT NULL CONSTRAINT DF_ChamCong_IsDeleted DEFAULT 0;
GO

IF COL_LENGTH('ChamCong', 'DeletedAt') IS NULL
    ALTER TABLE ChamCong ADD DeletedAt datetime NULL;
GO

IF COL_LENGTH('ChamCong', 'LydoTuChoi') IS NULL
    ALTER TABLE ChamCong ADD LydoTuChoi nvarchar(500) NULL;
GO

UPDATE ChamCong SET IsDeleted = 0 WHERE IsDeleted IS NULL;
GO
