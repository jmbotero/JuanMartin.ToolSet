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

DROP FUNCTION IF EXISTS `p1`;
DELIMITER //
CREATE FUNCTION `p1`() RETURNS int
    NO SQL
    DETERMINISTIC
return @p1//
DELIMITER ;

DROP TABLE IF EXISTS `tbllocation`;
CREATE TABLE IF NOT EXISTS `tbllocation` (
  `id` int NOT NULL AUTO_INCREMENT,
  `ddd` float DEFAULT NULL,
  `reference` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

DROP TABLE IF EXISTS `tblpasswordreset`;
CREATE TABLE IF NOT EXISTS `tblpasswordreset` (
  `user_id` int NOT NULL,
  `activation_code` varchar(36) NOT NULL DEFAULT '',
  `request_dtm` datetime DEFAULT NULL,
  PRIMARY KEY (`activation_code`)
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
) ENGINE=InnoDB AUTO_INCREMENT=37 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

DROP TABLE IF EXISTS `tblphotographytags`;
CREATE TABLE IF NOT EXISTS `tblphotographytags` (
  `photography_id` bigint DEFAULT NULL,
  `tag_id` int DEFAULT NULL,
  KEY `FK_keyword_photography` (`photography_id`),
  KEY `FK_keyword` (`tag_id`) USING BTREE,
  CONSTRAINT `FK_keyword` FOREIGN KEY (`tag_id`) REFERENCES `tbltag` (`id`),
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
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

DROP TABLE IF EXISTS `tblsession`;
CREATE TABLE IF NOT EXISTS `tblsession` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int NOT NULL,
  `start_dtm` datetime NOT NULL,
  `end_dtm` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=43 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

DROP TABLE IF EXISTS `tblstate`;
CREATE TABLE IF NOT EXISTS `tblstate` (
  `user_id` int DEFAULT '-1',
  `remote_host` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `redirect_controller` varchar(50) NOT NULL DEFAULT '',
  `redirect_action` varchar(50) NOT NULL DEFAULT '',
  `redirect_route_id` int DEFAULT '0',
  `redirect_routevalues` varchar(100) DEFAULT '',
  PRIMARY KEY (`remote_host`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

DROP TABLE IF EXISTS `tbltag`;
CREATE TABLE IF NOT EXISTS `tbltag` (
  `id` int NOT NULL AUTO_INCREMENT,
  `word` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

DROP TABLE IF EXISTS `tbluser`;
CREATE TABLE IF NOT EXISTS `tbluser` (
  `id` int NOT NULL AUTO_INCREMENT,
  `login` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `_password` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `email` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

DROP FUNCTION IF EXISTS `udfGetAverageRank`;
DELIMITER //
CREATE FUNCTION `udfGetAverageRank`(
	`PhotographyID` BIGINT
) RETURNS float
    READS SQL DATA
    SQL SECURITY INVOKER
BEGIN
	DECLARE AverageRank float;

	SELECT AVG(r._rank)
	INTO AverageRank
	FROM tblRanking r
 	WHERE r.photography_id = PhotographyID;

	IF NOT FOUND_ROWS() THEN
		SET AverageRanK = 0; 				 	
	END IF;
	
	RETURN AverageRank;
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

DROP PROCEDURE IF EXISTS `uspAddSession`;
DELIMITER //
CREATE PROCEDURE `uspAddSession`(
	IN `UserID` INT
)
BEGIN
	DECLARE id INT;
	
	INSERT INTO tblSession(user_id, start_dtm) VALUE(UserID, CURRENT_TIMESTAMP());
	SET id=LAST_INSERT_ID();
	
	SELECT id;
END//
DELIMITER ;

DROP PROCEDURE IF EXISTS `uspAddTag`;
DELIMITER //
CREATE PROCEDURE `uspAddTag`(
	IN `Word` VARCHAR(50),
	IN `PhotographyId` BIGINT
)
BEGIN
	DECLARE id INT;
	DECLARE ImageExists INT;
	DECLARE linkExists INT;
	
	SET imageExists = EXISTS(
	            SELECT p.filename 
					FROM tblphotography p
				 	WHERE p.id = PhotographyId);
					 
	IF (imageExists = 1) THEN 	
		SELECT k.id 
		INTO id
		FROM tbltag k
	 	WHERE k.word = Word;
		
		IF NOT FOUND_ROWS() THEN 	
			INSERT INTO tbltag(word) VALUE(LOWER(Word));
			SET id=LAST_INSERT_ID();
		ELSE
			SET id = -2;
		END IF;
		
		IF(PhotographyId<>-1 AND id > 0) THEN
			SET linkExists = EXISTS(
					SELECT pk.tag_id
					FROM tblphotographytags pk
					WHERE pk.tag_id = id
					AND pk.photography_id = PhotographyId);
			
			IF (linkExists = 0) THEN 	
				SET FOREIGN_KEY_CHECKS = 0;
				INSERT INTO tblPhotographytags(tag_id, photography_id) VALUE(id, PhotographyId);
				SET FOREIGN_KEY_CHECKS = 1;
			ELSE
				SET id = -1;
			END IF;
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
	DECLARE id INT;
	
	SELECT u.id 
	INTO id
	FROM tblUser u
 	WHERE u.login = Login;
	
	IF NOT FOUND_ROWS() THEN 	
		INSERT INTO tblUser(login, _password, email) VALUE(Login, MD5(UserPassword), Email);
		SET id=LAST_INSERT_ID();
	ELSE
		SET id=-2; /* user exists */
	END IF;
	
	SELECT id;
END//
DELIMITER ;

DROP PROCEDURE IF EXISTS `uspConnectUserAndRemoteHost`;
DELIMITER //
CREATE PROCEDURE `uspConnectUserAndRemoteHost`(
	IN `UserID` INT,
	IN `RemoteHost` VARCHAR(50)
)
BEGIN
	UPDATE tblState s
	SET s.user_id = UserID
	WHERE s.remote_host = RemoteHost
	AND s.user_id = -1;
END//
DELIMITER ;

DROP PROCEDURE IF EXISTS `uspEndSession`;
DELIMITER //
CREATE PROCEDURE `uspEndSession`(
	IN `SessionID` INT
)
BEGIN
	DECLARE id INT;
	
	SELECT s.id 
	INTO id
	FROM tblSession s	
 	WHERE s.id = SessionID;
	
	IF FOUND_ROWS() THEN 	
		UPDATE tblSession s
		SET s.end_dtm = CURRENT_TIMESTAMP()
	 	WHERE s.id = SessionID;
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

	SELECT v.* 
	FROM (SELECT @p1:=UserID p) parm , vwphotographywithranking v
	ORDER BY v.Id ASC
	LIMIT rec_take
	OFFSET rec_skip;
END//
DELIMITER ;

DROP PROCEDURE IF EXISTS `uspGetCurrentClientRedirectInfo`;
DELIMITER //
CREATE PROCEDURE `uspGetCurrentClientRedirectInfo`(
	IN `RemoteHost` VARCHAR(50),
	IN `UserID` INT
)
BEGIN
	SELECT 	s.remote_host AS 'RemoteHost',
				s.redirect_controller AS 'Controller',
	    		s.redirect_action AS 'Action',
	    		s.redirect_route_id AS 'RouteID',
	    		s.redirect_routevalues AS 'QueryString'
	FROM tblState s
	WHERE s.remote_host = RemoteHost;
	
	IF NOT FOUND_ROWS() THEN 	
		SELECT '' AS 'RemoteHost', '' AS 'Controller', '' AS 'Action', -1 AS 'RouteID', '' AS 'QueryString';
	END IF; 
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

DROP PROCEDURE IF EXISTS `uspGetPhotographyIdBounds`;
DELIMITER //
CREATE PROCEDURE `uspGetPhotographyIdBounds`()
BEGIN
   SELECT MIN(p.id) AS 'Lower', MAX(p.id) AS 'Upper'
	FROM tblphotography p;
END//
DELIMITER ;

DROP PROCEDURE IF EXISTS `uspGetPotography`;
DELIMITER //
CREATE PROCEDURE `uspGetPotography`(
	IN `Id` BIGINT,
	IN `UserID` INT
)
BEGIN
	SELECT v.* 
	FROM (SELECT @p1:=UserID p) parm , vwphotographywithranking v
	WHERE v.Id = Id;
	
	
	IF NOT FOUND_ROWS() THEN 	
		SELECT -1 AS Id;
	END IF;
END//
DELIMITER ;

DROP PROCEDURE IF EXISTS `uspGetUser`;
DELIMITER //
CREATE PROCEDURE `uspGetUser`(
	IN `UserName` VARCHAR(50),
	IN `UserPassword` VARCHAR(255)
)
BEGIN
	SELECT u.id AS 'Id', u.login AS 'Login', u._password AS 'Password', u.email AS 'Email'
	FROM tbluser u
	WHERE u.login = UserName
	AND u._password = MD5(UserPassword);
	
 	IF NOT FOUND_ROWS() THEN 	
		SELECT -1 AS 'Id', '' AS 'Login','' AS 'Password','' AS 'Email';
	END IF;
END//
DELIMITER ;

DROP PROCEDURE IF EXISTS `uspRemoveTag`;
DELIMITER //
CREATE PROCEDURE `uspRemoveTag`(
	IN `Word` VARCHAR(50),
	IN `PhotographyId` BIGINT
)
BEGIN
	DECLARE id INT;
	DECLARE linkExists INT;

	SET id=-1;

	IF(PhotographyId<>-1) THEN
		SELECT k.id
		INTO id
		FROM tbltag k
	 	WHERE k.word = Word;
	 	
		IF FOUND_ROWS() THEN 	
			DELETE k.*
			FROM tblphotographytags k
		 	WHERE k.tag_id=id
		 	AND k.photography_id = PhotographyId;
	 	
			SET linkExists = EXISTS(
				SELECT pk.tag_id
				FROM tblphotographytags pk
				JOIN tblTag t
				ON t.id = pk.tag_id
				WHERE t.word = Word
				AND pk.photography_id = PhotographyId);
			
			IF (linkExists = 0) THEN
				DELETE t.*
				FROM tblTag t
				WHERE t.word = Word;
			END IF;
		ELSE
			SET id = -1;
		END IF;
		
	END IF;	
	SELECT id;
END//
DELIMITER ;

DROP PROCEDURE IF EXISTS `uspSetCurrentClientRedirectInfo`;
DELIMITER //
CREATE PROCEDURE `uspSetCurrentClientRedirectInfo`(
	IN `UserID` INT,
	IN `RemoteHost` VARCHAR(50),
	IN `Controller` VARCHAR(50),
	IN `ControllerAction` VARCHAR(50),
	IN `RouteID` INT,
	IN `QueryString` VARCHAR(100)
)
BEGIN
	SELECT *
	FROM tblstate s
 	WHERE s.remote_host = RemoteHost;	

	IF FOUND_ROWS() THEN 	
		UPDATE tblstate s
		SET s.user_id = UserID,
			 s.redirect_controller = Controller,
			 s.redirect_action = ControllerAction,
			 s.redirect_route_id = RouteID,
			 s.redirect_routevalues = QueryString
	 	WHERE s.remote_host = RemoteHost;	
	ELSE
		INSERT INTO tblstate(user_id,remote_host, redirect_controller, redirect_action, redirect_route_id, redirect_routevalues) VALUE(UserID,RemoteHost, Controller, ControllerAction, RouteID, QueryString);
	END IF;
END//
DELIMITER ;

DROP PROCEDURE IF EXISTS `uspStoreActivationCode`;
DELIMITER //
CREATE PROCEDURE `uspStoreActivationCode`(
	IN `UserID` INT,
	IN `ActivationCode` VARCHAR(36)
)
BEGIN
	INSERT INTO tblPasswordReset(user_id ,activation_code,  request_dtm) VALUE(UserID, ActivationCode, CURRENT_TIMESTAMP());
END//
DELIMITER ;

DROP PROCEDURE IF EXISTS `uspUpdateRanking`;
DELIMITER //
CREATE PROCEDURE `uspUpdateRanking`(
	IN `UserID` INT,
	IN `PhotographyID` BIGINT,
	IN `UserRank` INT
)
BEGIN
	DECLARE id INT;

	SET id=-1;
	
	SELECT *
	FROM tblUser u
 	WHERE u.id = UserID;

	IF FOUND_ROWS() THEN 	
		SELECT *
		FROM tblphotography p
	 	WHERE p.id = PhotographyID;

		IF FOUND_ROWS() THEN 	
				SELECT r.id
					INTO id
				FROM tblranking r
			 	WHERE r.photography_id = PhotographyID
				AND r.user_id = UserID;	
				
			IF NOT FOUND_ROWS() THEN 				 	
				INSERT INTO tblranking(user_id, photography_id, _rank) VALUE(UserID, PhotographyID, UserRank);
				SET id=LAST_INSERT_ID();
			ELSE
				UPDATE tblRanking
				SET _rank = UserRank
			 	WHERE photography_id = PhotographyID
				AND user_id = UserID;	
			END IF;
		END IF;
	END IF;
	
	SELECT id;
END//
DELIMITER ;

DROP PROCEDURE IF EXISTS `uspUpdateUserPassword`;
DELIMITER //
CREATE PROCEDURE `uspUpdateUserPassword`(
	IN `UserID` INT,
	IN `Login` VARCHAR(50),
	IN `UserPassword` VARCHAR(255)
)
BEGIN
	DECLARE id INT;
	DECLARE login VARCHAR(50);
	DECLARE email VARCHAR(255);
	
	SELECT u.id, u.login, u.email 
	INTO id, login, email
	FROM tblUser u
 	WHERE u.login = Login
 	AND u.id = UserID;
	
	IF FOUND_ROWS() THEN 	
		UPDATE tbluser u 
		SET u._password = MD5(UserPassword)
 	WHERE u.login = Login
 	AND u.id = UserID;

		SELECT id AS 'Id', login AS 'Login', '' AS 'Password', email AS 'Email';
	ELSE
		SELECT -1 AS 'Id', '' AS 'Login','' AS 'Password','' AS 'Email';
	END IF;
	
	SELECT id;
END//
DELIMITER ;

DROP PROCEDURE IF EXISTS `uspVerifyActivationCode`;
DELIMITER //
CREATE PROCEDURE `uspVerifyActivationCode`(
	IN `ActivationCode` VARCHAR(36)
)
BEGIN
	DECLARE VerifyDtm DATETIME;
	DECLARE RequestDtm DATETIME;
	DECLARE errorCode INT;
	DECLARE login VARCHAR(50);
	DECLARE userId INT;
	
	SET verifyDtm = CURRENT_TIMESTAMP();
	SET login = '';
	SET userId = -1;
	
	SELECT r.request_dtm, r.user_id
	INTO requestDtm,userId
	FROM tblpasswordreset r
	WHERE  r.activation_code = ActivationCode;

	SET errorCode = -1; /* code not found */
	IF FOUND_ROWS() THEN 	
		SET errorCode = 1; /* code good */
		SELECT u.login
		INTO login
		FROM tblUser u
	 	WHERE u.id = userId;

		IF TIMESTAMPDIFF(MINUTE,requestDtm, verifyDtm) > 10 THEN
			DELETE r.*
			FROM tblpasswordreset r
			WHERE  r.activation_code = ActivationCode;
	
			SET errorCode = -2; /* code expired */
		END IF;
	END IF;
	
	SELECT userId AS 'Id', login AS 'Login', errorCode AS 'ErrorCode';
END//
DELIMITER ;

DROP PROCEDURE IF EXISTS `uspVerifyEmail`;
DELIMITER //
CREATE PROCEDURE `uspVerifyEmail`(
	IN `Email` VARCHAR(100)
)
BEGIN
	SELECT u.id AS 'Id', u.login AS 'Login'
	FROM tbluser u
	WHERE u.email = Email;
	
 	IF NOT FOUND_ROWS() THEN 	
		SELECT -1 AS 'Id', '' AS 'Login';
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
	`Tags` TEXT NULL COLLATE 'utf8mb4_0900_ai_ci'
) ENGINE=MyISAM;

DROP VIEW IF EXISTS `vwphotographytags`;
CREATE TABLE `vwphotographytags` (
	`PicId` BIGINT(19) NULL,
	`WordId` INT(10) NULL,
	`Wordlist` TEXT NULL COLLATE 'utf8mb4_0900_ai_ci'
) ENGINE=MyISAM;

DROP VIEW IF EXISTS `vwphotographywithranking`;
CREATE TABLE `vwphotographywithranking` (
	`Id` BIGINT(19) NOT NULL,
	`Filename` VARCHAR(50) NULL COLLATE 'utf8mb4_0900_ai_ci',
	`Location` VARCHAR(50) NOT NULL COLLATE 'utf8mb4_0900_ai_ci',
	`Path` VARCHAR(255) NULL COLLATE 'utf8mb4_0900_ai_ci',
	`Source` INT(10) NOT NULL,
	`Title` VARCHAR(100) NULL COLLATE 'utf8mb4_0900_ai_ci',
	`Tags` MEDIUMTEXT NOT NULL COLLATE 'utf8mb4_0900_ai_ci',
	`Rank` BIGINT(19) NOT NULL,
	`AverageRank` FLOAT NOT NULL
) ENGINE=MyISAM;

DROP VIEW IF EXISTS `vwphotographydetails`;
DROP TABLE IF EXISTS `vwphotographydetails`;
CREATE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `vwphotographydetails` AS select `pic`.`id` AS `Id`,`pic`.`filename` AS `Filename`,`loc`.`reference` AS `Location`,`pic`.`_path` AS `Path`,`pic`.`title` AS `Title`,`vw`.`Wordlist` AS `Tags` from ((`tblphotography` `pic` left join `tbllocation` `loc` on((`loc`.`id` = `pic`.`location_id`))) left join `vwphotographytags` `vw` on((`vw`.`PicId` = `pic`.`id`)));

DROP VIEW IF EXISTS `vwphotographytags`;
DROP TABLE IF EXISTS `vwphotographytags`;
CREATE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `vwphotographytags` AS select `pic`.`id` AS `PicId`,`word`.`id` AS `WordId`,group_concat(distinct `word`.`word` separator ',') AS `Wordlist` from ((`tblphotography` `pic` left join `tblphotographytags` `picwords` on((`picwords`.`photography_id` = `pic`.`id`))) left join `tbltag` `word` on((`word`.`id` = `picwords`.`tag_id`)));

DROP VIEW IF EXISTS `vwphotographywithranking`;
DROP TABLE IF EXISTS `vwphotographywithranking`;
CREATE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `vwphotographywithranking` AS select `pic`.`Id` AS `Id`,`pic`.`Filename` AS `Filename`,ifnull(`pic`.`Location`,'') AS `Location`,`pic`.`Path` AS `Path`,(case when (locate('slide',`pic`.`Path`) > 0) then 1 else 0 end) AS `Source`,`pic`.`Title` AS `Title`,ifnull(`pic`.`Tags`,'') AS `Tags`,ifnull(`r`.`_rank`,0) AS `Rank`,ifnull(`udfGetAverageRank`(`pic`.`Id`),0) AS `AverageRank` from (`vwphotographydetails` `pic` left join `tblranking` `r` on(((`r`.`user_id` = `p1`()) and (`r`.`photography_id` = `pic`.`Id`))));

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
