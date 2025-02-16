IF DB_ID('tf_disney_battle') IS NULL
BEGIN
    CREATE DATABASE tf_disney_battle;
END

GO

USE [tf_disney_battle];

IF OBJECT_ID('Users', 'U') IS NULL
BEGIN

    CREATE TABLE [Users](
        [id]            [int] IDENTITY(1,1) NOT NULL,
        [user_name]     [nvarchar](50) NOT NULL,
        [email]         [nvarchar](100) NOT NULL,
        [password]      [nvarchar](255) NOT NULL,
        [created_at]    [datetime] CONSTRAINT DF_user_created_at DEFAULT GETDATE() ,

        CONSTRAINT PK_user_id PRIMARY KEY CLUSTERED([id]),
        CONSTRAINT U_user_user_name UNIQUE([user_name])
    );
END