using JuanMartin.Kernel;
using JuanMartin.Kernel.Adapters;
using JuanMartin.Kernel.Messaging;
using JuanMartin.Kernel.Processors;
using JuanMartin.Kernel.RuleEngine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JuanMartin.FileSystemBackup
{
    public class Backup
    {
        private AdapterFileLog _logger;
        public Backup()
        {
            _logger = new AdapterFileLog(new ValueHolder("Backup", Directory.GetCurrentDirectory() + "\\"));
        }

        public void Run(string[] args)
        {
            string cfg_file = string.Empty;
            var cmd = new CommandLine(string.Join(" ", args).ToString());

            if (cmd.Contains("configuration"))
            {
                _logger.Send("Directory configuration...");
                try
                {
                    if (cmd.Contains("configuration"))
                        cfg_file = (string)cmd["configuration"].Value;
                }
                catch (Exception)
                {
                    throw new ArgumentException("'configuration' file of backup, option from command line is not parseable as string.");
                }
                if (cfg_file == string.Empty)
                {
                    // This will get the current WORKING directory (i.e. \bin\Debug)
                    string workingDirectory = Environment.CurrentDirectory;
                    // This will get the current PROJECT directory
                    string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
                    cfg_file = Path.Combine(projectDirectory, "backup_configuration.json");
                }
                Configure(cfg_file);
            }

            if (cmd.Contains("backup"))
            {
                _logger.Send("Performing backup...");

                ValueHolder taskList = GetAllBackupJobs();

                ProcessorTask processor = new ProcessorTask(taskList, 5000);

                processor.Execute();
            }

            _logger.Disconnect();

        }

        private ValueHolder GetAllBackupJobs()
        {
            AdapterMySql DbAdapter = new AdapterMySql("localhost", "backup", "root", "yala");
            ValueHolder jobs = new ValueHolder("Tasks");

            Message request = new Message("Command", System.Data.CommandType.Text.ToString());

            request.AddData(new ValueHolder("Jobs", "select job.Name from tblbackupjobs job left outer join tblbackupjobexecution exec on job.JobId = exec.JobId where exec.JobId is null or (exec.Status is not null and exec.Status <> 'INPROGRESS');"));
            request.AddSender("GetAllBackupJobs", typeof(Job).ToString());

            DbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)DbAdapter.Receive();

            if (reply.Data != null && reply.Data.GetAnnotation("Record") != null)
            {
                foreach (ValueHolder record in reply.Data.Annotations)
                {
                    string name = (string)record.GetAnnotation("Name").Value;

                    ValueHolder definition = new ValueHolder(name);
                    definition.AddAnnotation("DbAdapter", DbAdapter);
                    Job job = new Job(definition, _logger);

                    jobs.AddAnnotation(record.Value.ToString(), job.Task);
                }
            }

            return jobs;
        }

        private long AddBackupJob(AdapterMySql DbAdapter, string Name)
        {
             Message request = new Message("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("Job", string.Format("uspAddBackupJob('{0}','{1}')", Name, @"C:\Temp")));
            request.AddSender("AddBackupJob", typeof(Job).ToString());

            DbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)DbAdapter.Receive();

            var Id = (long)reply.Data.GetAnnotationByValue(1).GetAnnotation("id").Value;

            return Id;
        }

        private void AddBackupFolders(AdapterMySql DbAdapter, long JobId, string BaseFolder)
        {
            DirectoryInfo root = new DirectoryInfo(BaseFolder);

            try
            {
                DirectoryInfo[] directories = root.GetDirectories();

                string name = ExpressionEvaluator.EncodeToken(root.Name);
                string path = ExpressionEvaluator.EncodeToken(root.FullName);

                Message request = new Message("Command", System.Data.CommandType.StoredProcedure.ToString());
                request.AddData(new ValueHolder("Folder", string.Format("uspAddFolder({0},'{1}','{2}')", JobId, name, path)));
                request.AddSender("AddBackupFolders", typeof(Job).ToString());

                DbAdapter.Send(request);
                _logger.Send(string.Format("Adding folder '{0}'.", root.Name));

                foreach (DirectoryInfo directory in directories)
                {
                    AddBackupFolders(DbAdapter, JobId, directory.FullName);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                //Cannot rea d folder so skip it
                _logger.Send(string.Format("Exception: {0}", e.Message));
            }
        }

        private void Configure(string config_file)
        {
            var settings = LoadBackupSettings(config_file);
            string baseFolder = settings.BaseFolder;

            AdapterMySql dbAdapter = new AdapterMySql("localhost", "backup", "root", "yala");
            string[] names = settings.Folders.ToArray(); //{ "Documents", "Downloads" };

            foreach (string name in names)
            {
                AddBackupFolders(dbAdapter, AddBackupJob(dbAdapter, name), Path.Combine(baseFolder, name));
            }
        }

        private BackupSettings LoadBackupSettings(string file_name)
        {
            BackupSettings config = null; 
            var json = string.Empty;
            if (file_name != null && file_name.Length > 0)
            {
                using (var reader = new StreamReader(file_name, Encoding.UTF8))
                {
                    json = reader.ReadToEnd();
                }

                if (json != string.Empty)
                {
                    config = JsonConvert.DeserializeObject<BackupSettings>(json);
                }
                else
                    throw new FileLoadException();
            }

            return config;
        }

    }
}
