DELIMITER $$
USE backup;

USE backup;

DROP PROCEDURE IF EXISTS uspUpdateBackupJobFile;

CREATE PROCEDURE uspUpdateBackupJobFile(IN JobId BIGINT, IN FileId BIGINT, IN Size FLOAT, IN WriteDtm CHAR(25))
BEGIN

	-- File is not new to the system
	UPDATE tblfiles files
	SET files.Size = Size, files.ModifiedDtm = STR_TO_DATE(WriteDtm,'%m/%d/%Y %h:%i:%s %p')
	WHERE files.FileId = FileId;
	
	UPDATE tblbackupjobfiles jobfiles
	SET jobfiles.IsNew = 0, jobfiles.BackupDtm = NOW()
	WHERE jobfiles.JobId = JobId AND jobfiles.FileId = FileId;

END $$

DELIMITER ;
