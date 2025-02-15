using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace disney_battle.dal.database;

public class DataContext(IConfiguration configuration) : IDataContext
{
    private readonly string _connectionString = configuration["DB_CONNECTION_STRING"]!;

    public SqlConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }

    public int ExecuteNonQuery(string query, SqlParameter[] parameters)
    {
        using SqlConnection conn = CreateConnection();
        using SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddRange(parameters);
        conn.Open();
        return cmd.ExecuteNonQuery(); 
    }

    public DataTable ExecuteQuery(string query, SqlParameter[] parameters)
    {
        using SqlConnection conn = CreateConnection();
        using SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddRange(parameters);
        conn.Open();

        using SqlDataAdapter adapter = new(cmd);
        DataTable resultTable = new();
        adapter.Fill(resultTable);
        return resultTable;
    }
}