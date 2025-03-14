using Microsoft.Data.SqlClient;

namespace disney_battle.dal.database;

public interface IDataContext {
  SqlConnection CreateConnection();
}
