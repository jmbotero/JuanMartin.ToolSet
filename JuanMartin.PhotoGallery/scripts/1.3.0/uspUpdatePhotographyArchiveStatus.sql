-- Dumping structure for procedure gallery.uspUpdatePhotographyArchiveStatus
DROP PROCEDURE IF EXISTS `uspUpdatePhotographyArchiveStatus`;
DELIMITER //
CREATE PROCEDURE `uspUpdatePhotographyArchiveStatus`(
	IN `UserId` INT,
	IN `PhotographyId` BIGINT,
	IN `ArchiveStatus` VARCHAR(5)
)
BEGIN
	DECLARE _archcive SMALLINT;

	IF(ArchiveStatus = "true") THEN
		SET _archcive=1;
	ELSEIF(ArchiveStatus = "false") THEN
		SET _archcive=0;
	END IF;
	
	SELECT *
	FROM tblUser u
 	WHERE u.id = UserId;

	IF FOUND_ROWS() THEN 	
		UPDATE tblphotography p
			SET p.archive = _archcive
	 	WHERE p.id = PhotographyId;

		CALL uspAddAuditMessage(UserId, CONCAT('Set archive to [', ArchiveStatus, '] for (',PhotographyId,')'),'uspUpdatePhotographyArchiveStatus',0);
	END IF;
END//
DELIMITER ;
