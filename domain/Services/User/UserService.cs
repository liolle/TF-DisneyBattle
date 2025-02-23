using disney_battle.cqs;
using disney_battle.dal.database;
using disney_battle.dal.entities;
using disney_battle.domain.cqs.commands;
using disney_battle.domain.cqs.queries;
using disney_battle.domain.services.models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace disney_battle.domain.services;

public partial class UserService(IDataContext context, IHashService hashService, IJwtService jwt, HttpClient httpClient, IConfiguration configuration) : IUserService
{
    public CommandResult Execute(RegistersUserCommand command)
    {
        try
        {
            using SqlConnection conn = context.CreateConnection();
            string hashedPassword = hashService.HashPassword(command.Password);

            string query = $@"RegisterUserTransaction";

            using SqlCommand cmd = new(query, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Email", command.Email);
            cmd.Parameters.AddWithValue("@UserName", command.UserName);
            cmd.Parameters.AddWithValue("@Password", hashedPassword);

            conn.Open();
            int result = cmd.ExecuteNonQuery();
            if (result < 1)
            {
                return ICommandResult.Failure("User insertion failed.");
            }

            return ICommandResult.Success();
        }
        catch (Exception)
        {
            return ICommandResult.Failure("Server error");
        }
    }

    public QueryResult<CredentialInfoModel?> Execute(UserFromUserNameQuery query)
    {
        try
        {
            using SqlConnection conn = context.CreateConnection();
            string sql_query = "GetUserCredentialInfo";
            using SqlCommand cmd = new(sql_query, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserName", query.UserName);

            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                CredentialInfoModel u = new CredentialInfoModel(
                    (int)reader[nameof(UserEntity.Id)],
                    (string)reader[nameof(UserEntity.Email)],
                    (string)reader[nameof(CredentialEntity.Password)],
                    (DateTime)reader[nameof(UserEntity.Created_At)]
                );
                return IQueryResult<CredentialInfoModel?>.Success(u);
            }
            return IQueryResult<CredentialInfoModel?>.Failure($"Could not find UserName {query.UserName}");
        }
        catch (Exception)
        {
            return IQueryResult<CredentialInfoModel?>.Failure("Server error");
        }
    }

    public QueryResult<string> Execute(CredentialLoginQuery query)
    {
        try
        {
            QueryResult<CredentialInfoModel?> qr = Execute(new UserFromUserNameQuery(query.UserName));
            if (qr.IsFailure || qr.Result is null)
            {
                return IQueryResult<string>.Failure("Invalid credential combination");
            }

            if (!hashService.VerifyPassword(qr.Result.Password, query.Password))
            {
                return IQueryResult<string>.Failure("Invalid credential combination");
            }

            return IQueryResult<string>.Success(jwt.Generate(qr.Result.GetClaims()));
        }
        catch (Exception)
        {
            return IQueryResult<string>.Failure("Server error");
        }
    }

    public QueryResult<AccountEntity> Execute(AccountFromProviderQuery query)
    {
        try
        {
            using SqlConnection conn = context.CreateConnection();
            string sql_query = "GetOrCreateAccount";

            using SqlCommand cmd = new(sql_query, conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", query.User_Id);
            cmd.Parameters.AddWithValue("@ProviderId", query.Provider_id);
            cmd.Parameters.AddWithValue("@Provider", query.Provider.ToString());

            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                AccountEntity acc = AccountEntity.Create(
                    (int)reader[nameof(AccountEntity.Id)],
                    (string)reader[nameof(AccountEntity.Provider)],
                    (int)reader[nameof(AccountEntity.User_Id)],
                    (string)reader[nameof(AccountEntity.Provider_Id)]
                );
                return IQueryResult<AccountEntity>.Success(acc);
            }
            return IQueryResult<AccountEntity>.Failure($"Server error");
        }
        catch (Exception e)
        {
            return IQueryResult<AccountEntity>.Failure("Server error");
        }
    }
}