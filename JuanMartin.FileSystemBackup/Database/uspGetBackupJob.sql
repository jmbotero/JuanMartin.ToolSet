DELIMITER $$
USE backup;


DROP PROCEDURE IF EXISTS uspGetBackupJob;

CREATE PROCEDURE uspGetBackupJob(IN JobName VARCHAR(50))
BEGIN

	SELECT JobId AS Id, Target AS Target
	FROM tblbackupjobs
	WHERE Name = JobName;
END $$

DELIMITER ;
