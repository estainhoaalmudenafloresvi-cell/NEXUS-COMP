/*
  NEXUS — Expedición Eos
  Esquema de persistencia SQL Server para SqlSaveRepository.
  Incluye perfiles, misiones, partidas, progreso y mejoras.
*/
IF DB_ID(N'NEXUS') IS NULL
    CREATE DATABASE NEXUS;
GO

USE NEXUS;
GO

IF OBJECT_ID(N'dbo.Partidas', N'U') IS NOT NULL
   AND COL_LENGTH(N'dbo.Partidas', N'IdPerfil') IS NULL
BEGIN
    IF OBJECT_ID(N'dbo.Partidas_v1', N'U') IS NULL
        EXEC sp_rename N'dbo.Partidas', N'Partidas_v1';
    ELSE
        THROW 51000, 'Existe una tabla Partidas antigua y Partidas_v1 ya existe. Revisa la migración antes de continuar.', 1;
END;
GO

IF OBJECT_ID(N'dbo.Perfiles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Perfiles
    (
        IdPerfil INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Perfiles PRIMARY KEY,
        Nombre NVARCHAR(40) NOT NULL,
        FechaCreacion DATETIME2(0) NOT NULL CONSTRAINT DF_Perfiles_Fecha DEFAULT SYSDATETIME(),
        CONSTRAINT UQ_Perfiles_Nombre UNIQUE (Nombre)
    );
END;
GO

IF OBJECT_ID(N'dbo.Misiones', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Misiones
    (
        IdMision TINYINT NOT NULL CONSTRAINT PK_Misiones PRIMARY KEY,
        Planeta NVARCHAR(30) NOT NULL,
        Nombre NVARCHAR(80) NOT NULL
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Misiones WHERE IdMision = 1)
    INSERT INTO dbo.Misiones (IdMision, Planeta, Nombre) VALUES (1, N'EOS', N'Jardín de Cristal');
IF NOT EXISTS (SELECT 1 FROM dbo.Misiones WHERE IdMision = 2)
    INSERT INTO dbo.Misiones (IdMision, Planeta, Nombre) VALUES (2, N'NÉBULA', N'Archivo Sumergido');
IF NOT EXISTS (SELECT 1 FROM dbo.Misiones WHERE IdMision = 3)
    INSERT INTO dbo.Misiones (IdMision, Planeta, Nombre) VALUES (3, N'SYLVARA', N'Santuario de las Lunas');
IF NOT EXISTS (SELECT 1 FROM dbo.Misiones WHERE IdMision = 4)
    INSERT INTO dbo.Misiones (IdMision, Planeta, Nombre) VALUES (4, N'ASTRALIS', N'Corazón del Vacío');
GO

IF OBJECT_ID(N'dbo.Partidas', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Partidas
    (
        IdPartida INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Partidas PRIMARY KEY,
        IdPerfil INT NOT NULL,
        IdMisionActual TINYINT NOT NULL,
        Nivel INT NOT NULL CONSTRAINT DF_Partidas_Nivel DEFAULT 1,
        XP INT NOT NULL CONSTRAINT DF_Partidas_XP DEFAULT 0,
        Vida INT NOT NULL,
        MaxVida INT NOT NULL,
        Energia INT NOT NULL,
        MaxEnergia INT NOT NULL,
        Creditos INT NOT NULL CONSTRAINT DF_Partidas_Creditos DEFAULT 0,
        Botiquines INT NOT NULL CONSTRAINT DF_Partidas_Botiquines DEFAULT 0,
        Eliminaciones INT NOT NULL CONSTRAINT DF_Partidas_Eliminaciones DEFAULT 0,
        MejorRacha INT NOT NULL CONSTRAINT DF_Partidas_MejorRacha DEFAULT 0,
        Fragmentos INT NOT NULL CONSTRAINT DF_Partidas_Fragmentos DEFAULT 0,
        Nucleos INT NOT NULL CONSTRAINT DF_Partidas_Nucleos DEFAULT 0,
        InventarioJson NVARCHAR(MAX) NOT NULL CONSTRAINT DF_Partidas_Inventario DEFAULT N'[]',
        FechaGuardado DATETIME2(0) NOT NULL CONSTRAINT DF_Partidas_Fecha DEFAULT SYSDATETIME(),
        CONSTRAINT FK_Partidas_Perfiles FOREIGN KEY (IdPerfil)
            REFERENCES dbo.Perfiles(IdPerfil) ON DELETE CASCADE,
        CONSTRAINT FK_Partidas_Misiones FOREIGN KEY (IdMisionActual)
            REFERENCES dbo.Misiones(IdMision),
        CONSTRAINT CK_Partidas_Nivel CHECK (Nivel >= 1),
        CONSTRAINT CK_Partidas_XP CHECK (XP >= 0),
        CONSTRAINT CK_Partidas_Vida CHECK (MaxVida > 0 AND Vida >= 0 AND Vida <= MaxVida),
        CONSTRAINT CK_Partidas_Energia CHECK (MaxEnergia > 0 AND Energia >= 0 AND Energia <= MaxEnergia),
        CONSTRAINT CK_Partidas_Creditos CHECK (Creditos >= 0),
        CONSTRAINT CK_Partidas_Botiquines CHECK (Botiquines >= 0),
        CONSTRAINT CK_Partidas_Eliminaciones CHECK (Eliminaciones >= 0),
        CONSTRAINT CK_Partidas_MejorRacha CHECK (MejorRacha >= 0),
        CONSTRAINT CK_Partidas_Fragmentos CHECK (Fragmentos >= 0),
        CONSTRAINT CK_Partidas_Nucleos CHECK (Nucleos >= 0),
        CONSTRAINT CK_Partidas_InventarioJson CHECK (ISJSON(InventarioJson) = 1)
    );
END;
GO

IF OBJECT_ID(N'dbo.ProgresoMisiones', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ProgresoMisiones
    (
        IdPartida INT NOT NULL,
        IdMision TINYINT NOT NULL,
        Completada BIT NOT NULL CONSTRAINT DF_ProgresoMisiones_Completada DEFAULT 0,
        CONSTRAINT PK_ProgresoMisiones PRIMARY KEY (IdPartida, IdMision),
        CONSTRAINT FK_ProgresoMisiones_Partidas FOREIGN KEY (IdPartida)
            REFERENCES dbo.Partidas(IdPartida) ON DELETE CASCADE,
        CONSTRAINT FK_ProgresoMisiones_Misiones FOREIGN KEY (IdMision)
            REFERENCES dbo.Misiones(IdMision)
    );
END;
GO

IF OBJECT_ID(N'dbo.MejorasPartida', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.MejorasPartida
    (
        IdPartida INT NOT NULL,
        TipoMejora NVARCHAR(20) NOT NULL,
        Nivel TINYINT NOT NULL CONSTRAINT DF_MejorasPartida_Nivel DEFAULT 0,
        CONSTRAINT PK_MejorasPartida PRIMARY KEY (IdPartida, TipoMejora),
        CONSTRAINT FK_MejorasPartida_Partidas FOREIGN KEY (IdPartida)
            REFERENCES dbo.Partidas(IdPartida) ON DELETE CASCADE,
        CONSTRAINT CK_MejorasPartida_Tipo CHECK (TipoMejora IN (N'Arma', N'Escudo', N'Propulsor', N'Dron')),
        CONSTRAINT CK_MejorasPartida_Nivel CHECK (Nivel BETWEEN 0 AND 10)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Partidas_Perfil_Fecha' AND object_id = OBJECT_ID(N'dbo.Partidas'))
    CREATE INDEX IX_Partidas_Perfil_Fecha ON dbo.Partidas (IdPerfil, FechaGuardado DESC, IdPartida DESC);
GO

SELECT N'Esquema NEXUS listo' AS Resultado;
GO
