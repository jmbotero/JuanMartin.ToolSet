DELIMITER $$
USE backup;



DROP PROCEDURE IF EXISTS uspGetBackupJobFiles;

CREATE PROCEDURE uspGetBackupJobFiles(IN FolderId BIGINT)
BEGIN

	SELECT files.FileId AS Id, files.Name AS Name, files.Size AS Size, files.ModifiedDtm AS Modified
	FROM tblfiles files
	WHERE files.FolderId = FolderId;
	
END $$

DELIMITER ;
