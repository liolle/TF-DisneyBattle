using disney_battle.cqs;
using disney_battle.dal.database;
using disney_battle.dal.entities;
using disney_battle.domain.cqs.commands;
using disney_battle.domain.cqs.queries;
using Microsoft.Data.SqlClient;

namespace disney_battle.domain.services;

public class UserService(IDataContext context, IHashService hashService, IJwtService jwt) : IUserService
{
    public CommandResult Execute(RegistersUserCommand command)
    {
        try
        {
            using SqlConnection conn = context.CreateConnection();
            string hashedPassword = hashService.HashPassword(command.Password);

            string query = $@"
                INSERT INTO Users (user_name, email, password) 
                VALUES (@UserName,@Email, @Password);";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@Email", command.Email);
            cmd.Parameters.AddWithValue("@UserName", command.UserName);
            cmd.Parameters.AddWithValue("@Password", hashedPassword);

            conn.Open();
            int result = cmd.ExecuteNonQuery();
            if (result != 1)
            {
                return ICommandResult.Failure("User insertion failed.");
            }

            return ICommandResult.Success();
        }
        catch (Exception )
        {
            return ICommandResult.Failure("Server error");
        }
    }

    public QueryResult<UserEntity?> Execute(UserFromUserNameQuery query)
    {
        try
        {
            using SqlConnection conn = context.CreateConnection();
            string sql_query = "SELECT * FROM Users WHERE user_name = @UserName";
            using SqlCommand cmd = new(sql_query, conn);
            cmd.Parameters.AddWithValue("@UserName", query.UserName);

            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                UserEntity u = UserEntity.Create(
                    (int)reader[nameof(UserEntity.Id)],
                    (string)reader["user_name"],
                    (string)reader[nameof(UserEntity.Email)],
                    (string)reader[nameof(UserEntity.Password)],
                    (DateTime)reader["created_at"]
                );
                return IQueryResult<UserEntity?>.Success(u);
            }
            return IQueryResult<UserEntity?>.Failure($"Could not find UserName {query.UserName}");
        }
        catch (Exception e)
        {
            return IQueryResult<UserEntity?>.Failure("Server error");
        }
    }

    public QueryResult<string> Execute(LoginQuery query)
    {
        try
        {
            QueryResult<UserEntity?> qr = Execute(new UserFromUserNameQuery(query.UserName));
            if (qr.IsFailure || qr.Result is null ){
                return IQueryResult<string>.Failure("Invalid credential combination");
            }

            if (!hashService.VerifyPassword(qr.Result.Password,query.Password)){
                return IQueryResult<string>.Failure("Invalid credential combination");
            }

            return IQueryResult<string>.Success(jwt.generate(qr.Result));
        }
        catch (Exception )
        {
            return IQueryResult<string>.Failure("Server error");
        }
    }
}