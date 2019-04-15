using Npgsql;

namespace RKSI.EduPractice
{
    class DatabaseContext
    {
        public string DbConnectionString { get; }
        public NpgsqlConnection Connection { get; }

        /// <summary>
        /// Конструктор для создания подключения
        /// </summary>
        /// <param name="connectionString">Строка подключения</param>
        public DatabaseContext(string connectionString)
        {
            DbConnectionString = connectionString;
            Connection = new NpgsqlConnection(DbConnectionString);
        }
    }
}
