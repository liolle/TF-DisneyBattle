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
        [email]         [nvarchar](100) NOT NULL,
        [created_at]    [datetime] CONSTRAINT DF_user_created_at DEFAULT GETDATE() ,

        CONSTRAINT PK_user_id PRIMARY KEY CLUSTERED([id]),
        CONSTRAINT U_user_email_id UNIQUE([email])
    );
END;

IF OBJECT_ID('Personages', 'U') IS NULL
BEGIN

    CREATE TABLE [Personages](
        [id]            [int] IDENTITY(1,1) NOT NULL,
        [name]          [nvarchar](100) NOT NULL,
        [base_hp]       [int] NOT NULL,
        [base_atk]      [int] NOT NULL,
        [base_def]      [int] NOT NULL,

        CONSTRAINT PK_personages_id PRIMARY KEY CLUSTERED([id]),
        CONSTRAINT U_personage_name UNIQUE([name])
    );
END

IF OBJECT_ID('Accounts', 'U') IS NULL
BEGIN
    CREATE TABLE [Accounts](
        [id]            [int] IDENTITY(1,1) NOT NULL,
        [provider]      [nvarchar](50) NOT NULL,
        [user_id]		[int],
        [provider_id]	[nvarchar](100),

        CONSTRAINT PK_account_id PRIMARY KEY CLUSTERED([id]),
        CONSTRAINT FK_account_user_id FOREIGN KEY ([user_id]) REFERENCES Users([id])
        	ON DELETE CASCADE 
			ON UPDATE CASCADE,
		CONSTRAINT CK_account_provider_or_user_id CHECK(
			provider_id IS NOT NULL OR user_id IS NOT NULL
		),
		CONSTRAINT CK_account_provider CHECK(
	        [provider] = 'microsoft' OR 
	        [provider] = 'google' OR 
	        [provider] = 'credential'
		)
    );
END;


IF OBJECT_ID('Credentials', 'U') IS NULL
BEGIN
    CREATE TABLE [Credentials](
        [account_id]    [int] NOT NULL,
        [user_name]		[varchar](50),
        [password]      [nvarchar](100) NOT NULL,
        
        CONSTRAINT FK_credential_account_id FOREIGN KEY([account_id]) REFERENCES Accounts([id])
        	ON DELETE CASCADE 
			ON UPDATE CASCADE,
		CONSTRAINT U_credential_user_name UNIQUE([user_name])
    );
END;

IF OBJECT_ID('Roles', 'U') IS NULL
BEGIN
    CREATE TABLE [Roles](
        [account_id]    [int] NOT NULL,
        [role]		    [varchar](100) NOT NULL,

        CONSTRAINT FK_role_account_id FOREIGN KEY([account_id]) REFERENCES Accounts([id])
            ON DELETE CASCADE 
			ON UPDATE CASCADE,
        CONSTRAINT U_role_account_id UNIQUE([role],[account_id]),
        CONSTRAINT CK_role_role CHECK(
	        [role] = 'admin' 
		)
    );
END;