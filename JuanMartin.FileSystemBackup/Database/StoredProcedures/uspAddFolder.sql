DELIMITER $$
use backup$$

DROP PROCEDURE IF EXISTS uspAddFolder$$

CREATE PROCEDURE uspAddFolder(IN JobId BIGINT, IN Name CHAR(200), IN Path TEXT)
BEGIN
	DECLARE FolderId	BIGINT;

	SELECT folder.FolderId
	FROM tblfolders folder
	WHERE folder.Path = Path;
	
	IF NOT FOUND_ROWS() THEN 	
		BEGIN
			INSERT INTO tblfolders (Name, Path) VALUES(Name, Path);
			SET FolderId = LAST_INSERT_ID();

				INSERT INTO tblbackupjobdefinition (JobId, FolderId) VALUES(JobId, FolderId);
        END;
	END IF;
END$$

DELIMITER ;