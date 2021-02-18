--SELECT STR_TO_DATE('12/26/2010 5:09:45 PM','%m/%d/%Y %h:%i:%s %p');
--call uspAddBackupEvent(1,'INPROGRESS','12/26/2010 5:09:45 PM');
--call uspAddBackupEvent(1,'COMPLETE','1/1/2010 12:00:00 AM');
--call uspAddBackupJobFile(1,1,'Cultura y Ciudad.doc',22016,'12/17/2004 10:29:32 PM');
--select * from tblbackupjobfiles;
--select * from tblbackupjobexecution;
--select * from tblfolders;
select job.Name from tblbackupjobs job left outer join tblbackupjobexecution exec on job.JobId = exec.JobId where exec.JobId is null or (exec.Status is not null and exec.Status <> 'INPROGRESS');
