using MySqlConnector;

namespace SeaDrop
{
    public class Database
    {
        public static string Connect(string text, string server = "localhost", string user = "root", string pass = "password", string database = "data")
        {
            string value = "";

            // 接続文字列
            var connectionString = $"Server={server}; User ID={user}; Password={pass}; Database={database}";

            // 接続やSQL実行に必要なインスタンスの生成
            using (var connection = new MySqlConnection(connectionString))
            using (var command = connection.CreateCommand())
            {
                // 接続の確立
                connection.Open();

                //コマンドの実行
                command.CommandText = text;
                using var reader = command.ExecuteReader();

                // 1行ずつデータを取得
                while (reader.Read())
                {
                    value += reader.ToString();
                }
            }
            return value;
        }
    }
}
