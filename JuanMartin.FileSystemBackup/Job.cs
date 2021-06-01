using System.IO;
using System;
using JuanMartin.Kernel.Messaging;
using JuanMartin.Kernel;
using JuanMartin.Kernel.Processors;
using JuanMartin.Kernel.Adapters;
using JuanMartin.Kernel.Utilities;
using JuanMartin.Kernel.RuleEngine;
using JuanMartin.Kernel.Extesions;
using System.Text;
using Newtonsoft.Json;

namespace JuanMartin.FileSystemBackup
{
    public class Job 
    {
        readonly Task _task;
        readonly IExchangeRequest _logger;

        public Job(ValueHolder Definition, IExchangeRequest Logger) 
        {
            _logger = Logger;
            _task = new Task(Definition);

            IRecordSet job = Load(Definition);
            _task.Parameters.AddAnnotation("Job", job);

            ((ITask)_task).PreTaskHandler += new TaskEventHandler(StatusHandler);
            ((ITask)_task).TaskHandler += new TaskEventHandler(ExecuteHandler);
            ((ITask)_task).PostTaskHandler += new TaskEventHandler(StatusHandler);
        }

        public Task Task
        {
            get { return _task; }
        }

        private IRecordSet Load(ValueHolder Definition)
        {
            AdapterMySql DbAdapter = (AdapterMySql)Definition.GetAnnotation("DbAdapter").Value;
            string name = Definition.Name;
            Message request = new Message("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("Job", string.Format("uspGetBackupJob('{0}')", name)));
            request.AddSender("Load", typeof(Job).ToString());

            DbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)DbAdapter.Receive();

            long jobId = (long)reply.Data.GetAnnotationByValue(1).GetAnnotation("Id").Value;

            ValueHolder definition = LoadDirectories(DbAdapter, jobId);

            if (definition != null)
                reply.Data.AddAnnotation(definition);

            return reply;
        }

        private ValueHolder LoadDirectories(AdapterMySql DbAdapter, long FolderId)
        {
            Message request = new Message("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("Directories", string.Format("uspGetBackupJobDirectories({0})", FolderId)));
            request.AddSender("LoadDirectories", typeof(Job).ToString());

            DbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)DbAdapter.Receive();

            foreach(ValueHolder directory in reply.Data.Annotations)
            {
                if (directory != null)
                {
                    long directoryId = (long)directory.GetAnnotation("Id").Value;
                    ValueHolder files = LoadFiles(DbAdapter, directoryId);

                    if (files != null)
                        directory.AddAnnotation(files);
                }
            }

            return reply.Data;
        }

        private ValueHolder LoadFiles(AdapterMySql DbAdapter, long FolderId)
        {
            Message request = new Message("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("Files", string.Format("uspGetBackupJobFiles({0})", FolderId)));
            request.AddSender("LoadFiles", typeof(Job).ToString());

            DbAdapter.Send(request);
            IRecordSet reply = (IRecordSet)DbAdapter.Receive();

            return reply.Data;
        }

        private void ExecuteHandler(Object sender, TaskEventArgs e)
        {
            Execute(e.Parameters);
        }

        public void Execute(ValueHolder Parameters) 
        {
            IRecordSet job = (IRecordSet)Parameters.GetAnnotation("Job").Value;
            AdapterMySql dbAdapter = (AdapterMySql)Parameters.GetAnnotation("DbAdapter").Value;
            long jobId = (long)job.Data.GetAnnotationByValue(1).GetAnnotation("Id").Value;
            string baseTargetDirectory = (string)job.Data.GetAnnotationByValue(1).GetAnnotation("Target").Value;

            if (jobId == -1)
                throw new Exception(string.Format("No job was found with name '{0}'.",Parameters.Name));

            _logger.Send(string.Format("Backup '{0}' starting...", Parameters.Name));

            foreach (ValueHolder directory in job.Data.GetAnnotation("Directories").Annotations)
            {
                string path = (string)directory.GetAnnotation("Path").Value;
                long directoryId = (long)directory.GetAnnotation("Id").Value;
                
                if (Directory.Exists(path))
                {
                    _logger.Send(string.Format("Processing directory '{0}'", path));
                    String[] files = Directory.GetFiles(path);

                    if (directory.GetAnnotation("Files") != null)
                    {
                        foreach (ValueHolder file in directory.GetAnnotation("Files").Annotations)
                        {
                            long fileId = (long)file.GetAnnotation("Id").Value;
                            string name = (string)file.GetAnnotation("Name").Value;
                            long size = (long)file.GetAnnotation("Size").Value;
                            DateTime modifiedDtm = (DateTime)file.GetAnnotation("Modified").Value;

                            
                            //Backup the file
                            String pathName = Path.Combine(path, name);
                            if (File.Exists(pathName))
                            {
                                //Remove file from folder's files because is not a new file
                                files.Remove(pathName);

                                FileInfo info = new FileInfo(pathName);
                                //Workaround because milliseconds are not persisted by the AdapterMySql
                                DateTime fileDtm = DateTime.Parse(info.LastWriteTime.ToString("MM/dd/yyyy HH:mm:ss tt"));

                                //Get target file name and location
                                string targetDirectory = Path.Combine(baseTargetDirectory, Path.GetDirectoryName(pathName).Substring(3));
                                string newName = Path.Combine(targetDirectory, name);

                                //If the file exists in database but not in backup location or if size of original
                                //file and backup file are different or updated timestamps are different then re-copy it
                                if (!File.Exists(newName) || size == 0 || info.Length != size || fileDtm != modifiedDtm)
                                {
                                    //Create target path if it does not exist
                                    Directory.CreateDirectory(targetDirectory);

                                    //Do backup
                                    _logger.Send(string.Format("Refreshing file {0}", name));
                                    File.Copy(pathName, newName, true);

                                    //Update file info in the database
                                    Message request = new Message("Command", System.Data.CommandType.StoredProcedure.ToString());

                                    request.AddData(new ValueHolder("File", string.Format("uspUpdateBackupJobFile({0},{1},{2},'{3}')", jobId, fileId, info.Length, fileDtm)));
                                    request.AddSender("UpdateFile", typeof(Job).ToString());

                                    dbAdapter.Send(request);
                                }
                            }
                        }
                    }

                    //Now add to backup definitions any new files
                    foreach (String pathName in files)
                    {
                        //Backup the file
                        String name = Path.GetFileName(pathName);

                        if (File.Exists(pathName))
                        {
                            FileInfo info = new FileInfo(pathName);

                            //Create target path if it does not exist
                            string targetDirectory = Path.Combine(baseTargetDirectory, Path.GetDirectoryName(pathName).Substring(3));
                            Directory.CreateDirectory(targetDirectory);

                            //Do backup
                            _logger.Send(string.Format("Copying file {0}", name));
                            string newName = Path.Combine(targetDirectory, name);
                            File.Copy(pathName, newName, true);

                            //Add file info in the database
                            Message request = new Message("Command", System.Data.CommandType.StoredProcedure.ToString());

                            //Encode file name because sproc calls in mysql adapter use the evaluator to parse the sql command
                            request.AddData(new ValueHolder("File", string.Format("uspAddBackupJobFile({0},{1},'{2}',{3},'{4}')",jobId , directoryId, ExpressionEvaluator.EncodeToken(name), info.Length, info.LastWriteTime)));
                            request.AddSender("UpdateJob", typeof(Job).ToString());

                            dbAdapter.Send(request);
                        }
                    }
                }
            }

            _logger.Send(string.Format("Backup '{0}' complete.", Parameters.Name));
        }

        private void StatusHandler(Object sender, TaskEventArgs e)
        {
            AdapterMySql DbAdapter = (AdapterMySql)e.Parameters.GetAnnotation("DbAdapter").Value;
            long jobId = (long)((IRecordSet)e.Parameters.GetAnnotation("Job").Value).Data.GetAnnotationByValue(1).GetAnnotation("Id").Value;

            UpdateJobStatus(DbAdapter, jobId, e.Status.ToString(), DateTime.Now);            
        }

        public void UpdateJobStatus(AdapterMySql DbAdapter, long JobId, string Status, DateTime EventDtm)
        {
            Message request = new Message("Command", System.Data.CommandType.StoredProcedure.ToString());

            request.AddData(new ValueHolder("Job", string.Format("uspAddBackupEvent({0},'{1}','{2}')", JobId, Status, EventDtm)));
            request.AddSender("UpdateJobStatus", typeof(Job).ToString());

            DbAdapter.Send(request);
        }
    }
}
    