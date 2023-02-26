-- Dumping structure for procedure gallery.uspAddPhotography
DROP PROCEDURE IF EXISTS `uspAddPhotography`;
DELIMITER //
CREATE PROCEDURE `uspAddPhotography`(
	IN `ImageSource` INT,
	IN `FileName` VARCHAR(50),
	IN `FilePath` VARCHAR(255),
	IN `Title` VARCHAR(100)
)
BEGIN
	DECLARE id BIGINT;
	

	SELECT pic.id 
	INTO id
	FROM tblphotography pic
 	WHERE pic.filename = FileName
	AND pic._path = FilePath;
	
	IF NOT FOUND_ROWS() THEN 	
		INSERT INTO tblPhotography (_source,filename, _path, title) VALUE(ImageSource,FileName, FilePath, Title);
		SET id=LAST_INSERT_ID();
	END IF;	

	SELECT id;
END//
DELIMITER ;

