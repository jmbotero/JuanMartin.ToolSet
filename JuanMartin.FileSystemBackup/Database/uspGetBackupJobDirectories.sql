DELIMITER $$

DROP PROCEDURE IF EXISTS uspGetBackupJobDirectories;

CREATE PROCEDURE uspGetBackupJobDirectories(IN JobId BIGINT)
BEGIN

	SELECT folder.FolderId AS Id, folder.Name AS Name
	FROM tblbackupjobdefinition job
	LEFT JOIN tblfolders folder
		ON job.FolderId = folder.FolderId
	WHERE job.JobId = JobId;
	
END $$

DELIMITER ;
