
IF OBJECT_ID('dbo.RegisterUserTransaction', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.RegisterUserTransaction;
END

GO;

CREATE PROCEDURE RegisterUserTransaction
    @UserName VARCHAR(50),
    @Email [nvarchar](100),
    @Password [nvarchar](100)
AS
BEGIN
    DECLARE @NewUserId INT;
    DECLARE @NewAccountId INT;
    BEGIN TRY
        BEGIN TRANSACTION;

            -- Create User
            INSERT INTO [Users]
        ([Email])
    VALUES
        (@Email);

            -- retrieves the last identity value generated in the current session and scope
            SET @NewUserId = SCOPE_IDENTITY();

            -- Create Account
            INSERT INTO [Accounts]
        ([provider],[user_id])
    VALUES
        ('credential', @NewUserId);

            SET @NewAccountId = SCOPE_IDENTITY();

            -- Create Credential
            INSERT INTO [Credentials]
        ([user_name], [account_id], [password])
    VALUES
        (@UserName, @NewAccountId, @Password);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW; -- Re-throw the error
    END CATCH
END;


IF OBJECT_ID('dbo.GetUserCredentialInfo', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.GetUserCredentialInfo;
END

GO;

CREATE PROCEDURE GetUserCredentialInfo
    @UserName VARCHAR(50)
AS
BEGIN
    SELECT 
        u.id,
        u.email,
        u.created_at,
        cred.password
    FROM [Accounts] acc
    LEFT JOIN [Credentials] cred ON cred.account_id = acc.id 
    LEFT JOIN [Users] u ON u.id = acc.user_id
    WHERE cred.user_name = @UserName

END;


IF OBJECT_ID('dbo.GetOrCreateAccount', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.GetOrCreateAccount;
END

GO;

CREATE PROCEDURE GetOrCreateAccount
    @Provider NVARCHAR(50),
    @UserId INT,
    @ProviderId NVARCHAR(100)
AS
BEGIN
    DECLARE @AccountId INT;

    SELECT @AccountId = id 
    FROM Accounts 
    WHERE provider = @Provider
      AND (
          (@Provider = 'credential' AND user_id = @UserId) 
          OR (@Provider != 'credential' AND provider_id = @ProviderId)
      );

    IF @AccountId IS NULL
    BEGIN
        INSERT INTO Accounts ([provider], [provider_id], [user_id])
        VALUES (@Provider, @ProviderId, @UserId);

        SET @AccountId = SCOPE_IDENTITY();
    END

    SELECT * FROM Accounts WHERE id = @AccountId;
END;