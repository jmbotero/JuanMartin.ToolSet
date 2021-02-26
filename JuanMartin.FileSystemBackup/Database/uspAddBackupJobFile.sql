DELIMITER $$
USE backup;


DROP PROCEDURE IF EXISTS uspAddBackupJobFile;

CREATE PROCEDURE uspAddBackupJobFile(IN JobId BIGINT, IN FolderId BIGINT, IN Name CHAR(100), IN Size FLOAT, IN WriteDtm CHAR(25))
BEGIN

	SELECT files.FileId
	FROM tblfiles files
	WHERE files.Name = Name AND files.FolderId = FolderId;
	
	IF NOT FOUND_ROWS() THEN 
	-- File is new
		INSERT INTO tblfiles (FolderId, Name, Size, ModifiedDtm) VALUES(FolderId, Name, Size, STR_TO_DATE(WriteDtm,'%m/%d/%Y %h:%i:%s %p'));
		
		INSERT INTO tblbackupjobfiles (JobId, FileId, BackupDtm, IsNew) VALUES(JobId, LAST_INSERT_ID(), NOW(), 1);
	END IF;
END $$

DELIMITER ;
