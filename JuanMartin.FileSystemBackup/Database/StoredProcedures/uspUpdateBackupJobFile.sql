DELIMITER $$
USE backup$$

DROP PROCEDURE IF EXISTS uspUpdateBackupJobFile$$

CREATE PROCEDURE uspUpdateBackupJobFile(IN JobId BIGINT, IN FileId BIGINT, IN Size FLOAT, IN WriteDtm CHAR(25))
BEGIN
	DECLARE id BIGINT;
	
	SELECT _file.FileId 
	INTO id
	FROM tblfiles _file
	WHERE _file.FileId = FileId;
	
	IF FOUND_ROWS() THEN 	
		BEGIN
			-- File is not new to the system
			UPDATE 	 files
				SET files.Size = Size, files.ModifiedDtm = STR_TO_DATE(WriteDtm,'%m/%d/%Y %h:%i:%s %p')
			WHERE files.FileId = FileId;

			UPDATE tblbackupjobfiles jobfiles
				SET jobfiles.IsNew = 0, jobfiles.BackupDtm = NOW()
			WHERE jobfiles.JobId = JobId AND jobfiles.FileId = FileId;
		END;
	ELSE
		SET id = -1;
	END IF;
	
	SELECT @id;
END $$

DELIMITER ;
