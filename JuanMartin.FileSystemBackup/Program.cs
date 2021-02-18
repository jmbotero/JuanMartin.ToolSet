namespace JuanMartin.FileSystemBackup
{
    class Program
    {

        static void Main(string[] args)
        {
            var backup = new Backup();

            backup.Run(args);
        }

    }
}
