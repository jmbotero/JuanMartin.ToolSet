
using MySql.Data.MySqlClient;
using System;

namespace JuanMartin.FileSystemBackup
{
    class Program
    {

        static void Main(string[] args)
        {
            var backup = new Backup();

            backup.Run(args);

            //Ping("localhost", "backup", "root", "yala");
        }
        
        private static void Ping(string Server, string Database, string User, string Password)
        {
            string connectionString = string.Format("SERVER={0};" +
                "DATABASE={1};" +
                    "UID={2};" +
                "PASSWORD={3};", Server, Database, User, Password);

            var connection = new MySqlConnection(connectionString);

            connection.Open();

            Console.WriteLine($"MySQL version : {connection .ServerVersion}");
            var stm = "SELECT VERSION()";
            var cmd = new MySqlCommand(stm, connection);

            var version = cmd.ExecuteScalar().ToString();
            Console.WriteLine($"MySQL version: {version}");

            connection.Close();

        }
    }
}
