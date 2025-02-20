using disney_battle.cqs;
using disney_battle.dal.database;
using disney_battle.dal.entities;
using Microsoft.Data.SqlClient;

namespace disney_battle.domain.services;

public class GameService(IDataContext context) : IGameService
{
    public QueryResult<ICollection<PersonageEntity>> Execute(AllPersonages query)
    {
        List<PersonageEntity> personages = [];
        try
        {
            using SqlConnection conn = context.CreateConnection();
            string sql_query = "SELECT * FROM Personages";
            using SqlCommand cmd = new(sql_query, conn);

            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                PersonageEntity p = PersonageEntity.Create(
                    (int)reader[nameof(UserEntity.Id)],
                    (string)reader[nameof(PersonageEntity.Name)],
                    (int)reader["base_hp"],
                    (int)reader["base_atk"],
                    (int)reader["base_def"]
                );
                personages.Add(p);
            }
            return IQueryResult<ICollection<PersonageEntity>>.Success(personages);
        }
        catch (Exception)
        {
            return IQueryResult<ICollection<PersonageEntity>>.Failure("Server error");
        }
    }
}