DELIMITER $$

DROP PROCEDURE IF EXISTS uspAddBackupJob;

CREATE PROCEDURE uspAddBackupJob(IN Name CHAR(255), IN Path CHAR(255))
BEGIN
	DECLARE id BIGINT;
	
	SELECT job.JobId 
	INTO id
	FROM tblbackupjobs job
	WHERE job.Name = Name;
	
	IF NOT FOUND_ROWS() THEN 	
		INSERT INTO tblbackupjobs (Name, Target) VALUE(Name, Path);
		SET id=LAST_INSERT_ID();
	END IF;
	
	SELECT id;
END $$

DELIMITER ;
