/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

DROP DATABASE IF EXISTS `photogallery`;
CREATE DATABASE IF NOT EXISTS `photogallery` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `photogallery`;

DROP TABLE IF EXISTS `tblkeyword`;
CREATE TABLE IF NOT EXISTS `tblkeyword` (
  `id` int NOT NULL AUTO_INCREMENT,
  `word` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

DROP TABLE IF EXISTS `tbllocation`;
CREATE TABLE IF NOT EXISTS `tbllocation` (
  `id` int NOT NULL AUTO_INCREMENT,
  `ddd` float DEFAULT NULL,
  `reference` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

DROP TABLE IF EXISTS `tblphotography`;
CREATE TABLE IF NOT EXISTS `tblphotography` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `_source` int NOT NULL DEFAULT '0',
  `_path` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `title` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT '',
  `filename` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `location_id` int DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  KEY `FK_photography_locations` (`location_id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

DROP TABLE IF EXISTS `tblphotographykeywords`;
CREATE TABLE IF NOT EXISTS `tblphotographykeywords` (
  `photography_id` bigint DEFAULT NULL,
  `keyword_id` int DEFAULT NULL,
  KEY `FK_keyword` (`keyword_id`),
  KEY `FK_keyword_photography` (`photography_id`),
  CONSTRAINT `FK_keyword` FOREIGN KEY (`keyword_id`) REFERENCES `tblkeyword` (`id`),
  CONSTRAINT `FK_keyword_photography` FOREIGN KEY (`photography_id`) REFERENCES `tblphotography` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

DROP TABLE IF EXISTS `tblranking`;
CREATE TABLE IF NOT EXISTS `tblranking` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int DEFAULT NULL,
  `photography_id` bigint DEFAULT NULL,
  `_rank` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `FK_ranking_photography` (`photography_id`),
  KEY `FK_ranking_user` (`user_id`),
  CONSTRAINT `FK_ranking_photography` FOREIGN KEY (`photography_id`) REFERENCES `tblphotography` (`id`),
  CONSTRAINT `FK_ranking_user` FOREIGN KEY (`user_id`) REFERENCES `tbluser` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

DROP TABLE IF EXISTS `tbluser`;
CREATE TABLE IF NOT EXISTS `tbluser` (
  `id` int NOT NULL AUTO_INCREMENT,
  `login` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `_password` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `email` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

DROP PROCEDURE IF EXISTS `uspAddKeyword`;
DELIMITER //
CREATE PROCEDURE `uspAddKeyword`(
	IN `Word` VARCHAR(50),
	IN `LinkToPhotographyId` BIGINT
)
BEGIN
	DECLARE id INT;
	
	SELECT k.id 
	INTO id
	FROM tblkeyword k
 	WHERE k.word = Word;
	
	IF NOT FOUND_ROWS() THEN 	
		INSERT INTO tblkeyword(word) VALUE(LOWER(Word));
		SET id=LAST_INSERT_ID();
	ELSE
		SET id=-1;
	END IF;
	
	IF(LinkToPhotographyId<>-1) THEN
		INSERT INTO tblPhotographykeywords(keyword_id, photography_id) VALUE(id, LinkToPhotographyId);
	END IF;
	
	SELECT id;
END//
DELIMITER ;

DROP PROCEDURE IF EXISTS `uspAddPhotography`;
DELIMITER //
CREATE PROCEDURE `uspAddPhotography`(
	IN `Source` INT,
	IN `Filename` VARCHAR(50),
	IN `Path` VARCHAR(255),
	IN `Title` VARCHAR(100)
)
BEGIN
	DECLARE id BIGINT;
	

	SELECT pic.id 
	INTO id
	FROM tblphotography pic
 	WHERE pic.filename = Filename;
	
	IF NOT FOUND_ROWS() THEN 	
		INSERT INTO tblPhotography (_source,filename, _path, title) VALUE('Source',Filename, 'Path', Title);
		SET id=LAST_INSERT_ID();
	END IF;	

	SELECT id;
END//
DELIMITER ;

DROP PROCEDURE IF EXISTS `uspAddRanking`;
DELIMITER //
CREATE PROCEDURE `uspAddRanking`(
	IN `UserID` INT,
	IN `PhotographyID` BIGINT,
	IN `Rank` INT
)
BEGIN
	DECLARE id INT;
	DECLARE pid BIGINT;

	SET id=-1;
	
	SELECT u.id 
	INTO id
	FROM tblUser u
 	WHERE u.id = UserID;

	IF NOT FOUND_ROWS() THEN 	
		SELECT p.id 
		INTO pid
		FROM tblphotography P
	 	WHERE p.id = PhotographyID;

		IF NOT FOUND_ROWS() THEN 	
			INSERT INTO tblranking(user_id, photography_id, _rank) VALUE(UserID, PhotographyID, 'Rank');
			SET id=LAST_INSERT_ID();
		END IF;
	END IF;
	
	SELECT id;
END//
DELIMITER ;

DROP PROCEDURE IF EXISTS `uspAddUser`;
DELIMITER //
CREATE PROCEDURE `uspAddUser`(
	IN `Login` VARCHAR(50),
	IN `UserPassword` VARCHAR(255),
	IN `Email` VARCHAR(100)
)
BEGIN
	DECLARE id BIGINT;
	
	SELECT u.id 
	INTO id
	FROM tblUser u
 	WHERE u.login = Login;
	
	IF NOT FOUND_ROWS() THEN 	
		INSERT INTO tblUser(login, _password, email) VALUE(Login, MD5(UserPassword), Email);
		SET id=LAST_INSERT_ID();
	ELSE
		SET id=-1;
	END IF;
	
	SELECT id;
END//
DELIMITER ;

DROP PROCEDURE IF EXISTS `uspGetAllPhotographies`;
DELIMITER //
CREATE PROCEDURE `uspGetAllPhotographies`(
	IN `CurrentPage` INT,
	IN `PageSize` INT,
	IN `UserID` INT
)
BEGIN
	DECLARE rec_take INT;
	DECLARE rec_skip INT;
	
	SET rec_take = PageSize;
	SET rec_skip = (CurrentPage - 1) * PageSize;
	
 	SELECT 	pic.Id,
				pic.Filename,
				IFNULL(pic.Location,'') AS 'Location',
				pic.Path AS 'Path',
				CASE WHEN LOCATE('slide',pic.Path) > 0
				 THEN 1 /* Photography.PhysicalSource.slide */
				 ELSE 0 /* Photography.PhysicalSource.negative */
				END AS 'Source',
				pic.Title,
				IFNULL(pic.Keywords,'') AS 'Keywords',
				IFNULL(r._rank,0) AS 'Rank'
	FROM vwPhotographyDetails pic
	LEFT JOIN tblRanking r
	ON r.user_id=UserID
	AND r.photography_id=pic.id
	LIMIT rec_take
	OFFSET rec_skip;
END//
DELIMITER ;

DROP PROCEDURE IF EXISTS `uspGetPageCount`;
DELIMITER //
CREATE PROCEDURE `uspGetPageCount`(
	IN `PageSize` INT
)
BEGIN
	DECLARE photoCount INT;
	DECLARE pageCount INT;
	
	SELECT COUNT(*)
	INTO photoCount
	FROM  vwphotographydetails;
	
	SET pageCount = (photoCount / PageSize) + 1;
	
	SELECT pageCount;
END//
DELIMITER ;

DROP PROCEDURE IF EXISTS `uspGetPotography`;
DELIMITER //
CREATE PROCEDURE `uspGetPotography`(
	IN `Id` BIGINT
)
BEGIN
 	SELECT 	pic.Id,
				pic.Filename,
				IFNULL(pic.Location,'') AS 'Location',
				pic.Path AS 'Path',
				CASE WHEN LOCATE('slide',pic.Path) > 0
				 THEN 1
				 ELSE 0
				END AS 'Source',
				pic.Title,
				pic.Keywords,
				IFNULL(r._rank,0) AS 'Rank'
	FROM vwPhotographyDetails pic
	LEFT JOIN tblRanking r
	ON r.user_id=UserID
	AND r.photography_id=pic.id
	WHERE pic.Id = Id;
	
	
	IF NOT FOUND_ROWS() THEN 	
		SELECT -1 AS Id;
	END IF;
END//
DELIMITER ;

DROP VIEW IF EXISTS `vwphotographydetails`;
CREATE TABLE `vwphotographydetails` (
	`Id` BIGINT(19) NOT NULL,
	`Filename` VARCHAR(50) NULL COLLATE 'utf8mb4_0900_ai_ci',
	`Location` VARCHAR(50) NULL COLLATE 'utf8mb4_0900_ai_ci',
	`Path` VARCHAR(255) NULL COLLATE 'utf8mb4_0900_ai_ci',
	`Title` VARCHAR(100) NULL COLLATE 'utf8mb4_0900_ai_ci',
	`Keywords` TEXT NULL COLLATE 'utf8mb4_0900_ai_ci'
) ENGINE=MyISAM;

DROP VIEW IF EXISTS `vwphotographykeywords`;
CREATE TABLE `vwphotographykeywords` (
	`PicId` BIGINT(19) NULL,
	`WordId` INT(10) NULL,
	`Wordlist` TEXT NULL COLLATE 'utf8mb4_0900_ai_ci'
) ENGINE=MyISAM;

DROP VIEW IF EXISTS `vwphotographydetails`;
DROP TABLE IF EXISTS `vwphotographydetails`;
CREATE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `vwphotographydetails` AS select `pic`.`id` AS `Id`,`pic`.`filename` AS `Filename`,`loc`.`reference` AS `Location`,`pic`.`_path` AS `Path`,`pic`.`title` AS `Title`,`vw`.`Wordlist` AS `Keywords` from ((`tblphotography` `pic` left join `tbllocation` `loc` on((`loc`.`id` = `pic`.`location_id`))) left join `vwphotographykeywords` `vw` on((`vw`.`PicId` = `pic`.`id`)));

DROP VIEW IF EXISTS `vwphotographykeywords`;
DROP TABLE IF EXISTS `vwphotographykeywords`;
CREATE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `vwphotographykeywords` AS select `pic`.`id` AS `PicId`,`word`.`id` AS `WordId`,group_concat(distinct `word`.`word` separator ',') AS `Wordlist` from ((`tblphotography` `pic` left join `tblphotographykeywords` `picwords` on((`picwords`.`photography_id` = `pic`.`id`))) left join `tblkeyword` `word` on((`word`.`id` = `picwords`.`keyword_id`)));

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
