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

DELIMITER //
CREATE FUNCTION `p1`() RETURNS int
    NO SQL
    DETERMINISTIC
return @p1//
DELIMITER ;

CREATE TABLE IF NOT EXISTS `tblaudit` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int NOT NULL,
  `event_dtm` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `message` varchar(250) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=50 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS `tbllocation` (
  `id` int NOT NULL AUTO_INCREMENT,
  `ddd` float DEFAULT NULL,
  `reference` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS `tblpasswordreset` (
  `user_id` int NOT NULL,
  `activation_code` varchar(36) NOT NULL DEFAULT '',
  `request_dtm` datetime DEFAULT NULL,
  PRIMARY KEY (`activation_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

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

CREATE TABLE IF NOT EXISTS `tblphotographytags` (
  `photography_id` bigint DEFAULT NULL,
  `tag_id` int DEFAULT NULL,
  KEY `FK_keyword_photography` (`photography_id`),
  KEY `FK_keyword` (`tag_id`) USING BTREE,
  CONSTRAINT `FK_keyword` FOREIGN KEY (`tag_id`) REFERENCES `tbltag` (`id`),
  CONSTRAINT `FK_keyword_photography` FOREIGN KEY (`photography_id`) REFERENCES `tblphotography` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

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
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS `tblsession` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int NOT NULL,
  `start_dtm` datetime NOT NULL,
  `end_dtm` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=71 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS `tblstate` (
  `remote_host` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `user_id` int NOT NULL DEFAULT '-1',
  `redirect_controller` varchar(50) NOT NULL DEFAULT '',
  `redirect_action` varchar(50) NOT NULL DEFAULT '',
  `redirect_route_id` int NOT NULL DEFAULT '0',
  `redirect_routevalues` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '',
  `event_dtm` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`remote_host`,`user_id`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS `tbltag` (
  `id` int NOT NULL AUTO_INCREMENT,
  `word` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS `tbluser` (
  `id` int NOT NULL AUTO_INCREMENT,
  `login` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `_password` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `email` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

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

DELIMITER //
CREATE PROCEDURE `uspAddAuditMessage`(
	IN `UserID` INT,
	IN `Message` VARCHAR(250)
)
BEGIN
	INSERT INTO tblaudit(user_id, event_dtm, message) VALUES(UserID, CURRENT_TIMESTAMP(), Message);
END//
DELIMITER ;

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
 	WHERE pic.filename = FileName;
	
	IF NOT FOUND_ROWS() THEN 	
		INSERT INTO tblPhotography (_source,filename, _path, title) VALUE(ImageSource,FileName, FilePath, Title);
		SET id=LAST_INSERT_ID();
	END IF;	

	SELECT id;
END//
DELIMITER ;

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

DELIMITER //
CREATE PROCEDURE `uspAddTag`(
	IN `UserID` INT,
	IN `Word` VARCHAR(50),
	IN `PhotographyId` BIGINT
)
BEGIN
	DECLARE id INT;
	DECLARE imageExists INT;
	DECLARE linkExists INT;
	
	SET imageExists = EXISTS(
	            SELECT p.filename 
					FROM tblphotography p
				 	WHERE p.id = PhotographyId);

	SET id = -1;
					 
	IF (imageExists = 1) THEN 	
		SELECT k.id 
		INTO id
		FROM tbltag k
	 	WHERE k.word = Word;
		
		IF NOT FOUND_ROWS() THEN 	
			INSERT INTO tbltag(word) VALUE(LOWER(Word));
			SET id=LAST_INSERT_ID();
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
				SET id = -2;
			END IF;
		END IF;
	END IF;
	
	IF(id > 0) THEN
		CALL uspAddAuditMessage(UserID, CONCAT('Add tag ''',Word, ''' for (',PhotographyId,')'));
	END IF;
	SELECT id;
END//
DELIMITER ;

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

DELIMITER //
CREATE PROCEDURE `uspConnectUserAndRemoteHost`(
	IN `UserID` INT,
	IN `RemoteHost` VARCHAR(50)
)
BEGIN
	IF (UserID = 1) THEN
      DELETE s.*
		FROM tblstate s 
		WHERE  s.remote_host = RemoteHost AND s.user_id = UserID;
	END IF;

	UPDATE tblState s
	SET s.user_id = UserID
	WHERE s.remote_host = RemoteHost
	AND s.user_id = -1;
END//
DELIMITER ;

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

DELIMITER //
CREATE PROCEDURE `uspGetPhotographiesByTags`(
	IN `UserID` INT,
	IN `Wordlist` VARCHAR(150),
	IN `CurrentPage` INT,
	IN `PageSize` INT
)
    NO SQL
BEGIN
	SET @rec_take = PageSize;
	SET @rec_skip = (CurrentPage - 1) * PageSize;

	SET @qry = CONCAT('SELECT v.* FROM tblphotographytags pt JOIN vwphotographywithranking v ON v.id = pt.photography_id JOIN tbltag t ON t.id = pt.tag_id WHERE t.word REGEXP ''', Wordlist, '''  ORDER BY v.Id ASC LIMIT ', @rec_take, ' OFFSET ',@rec_skip);
	PREPARE stmt FROM @qry;
	EXECUTE stmt;
	DEALLOCATE PREPARE stmt;
END//
DELIMITER ;

DELIMITER //
CREATE PROCEDURE `uspGetPhotographyIdBounds`(
	IN `HasQuery` INT,
	IN `SearchQuery` VARCHAR(150)
)
BEGIN
	IF (HasQuery = 0) THEN
	   SELECT IFNULL(MIN(p.id),0) AS 'Lower', IFNULL(MAX(p.id),0) AS 'Upper', COUNT(*) AS 'RowCount'
		FROM tblphotography p
		ORDER BY p.id ASC;
	ELSE
		SELECT IFNULL(MIN(v.id),0) AS 'Lower', IFNULL(MAX(v.id),0) AS 'Upper', COUNT(*) AS 'RowCount' 
		FROM tblphotographytags pt 
		JOIN vwphotographywithranking v 
			ON v.id = pt.photography_id
		JOIN tbltag t 
			ON t.id = pt.tag_id 
		WHERE t.word REGEXP SearchQuery
		ORDER BY v.Id ASC;
	END IF;
END//
DELIMITER ;

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

DELIMITER //
CREATE PROCEDURE `uspGetUserRedirectInfo`(
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
	WHERE s.remote_host = RemoteHost
	AND s.user_id = UserID;
/*	AND s.event_dtm = (SELECT MAX (s.event_dtm) FROM tblstate s);*/
	 
	IF NOT FOUND_ROWS() THEN 	
		SELECT '' AS 'RemoteHost', '' AS 'Controller', '' AS 'Action', -1 AS 'RouteID', '' AS 'QueryString';
	END IF; 
END//
DELIMITER ;

DELIMITER //
CREATE PROCEDURE `uspRemoveTag`(
	IN `UserID` INT,
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
	
	IF(id > 0) THEN
		CALL uspAddAuditMessage(UserID, CONCAT('Remove tag ''',Word,'''  for (',PhotographyId,')'));
	END IF;

	SELECT id;
END//
DELIMITER ;

DELIMITER //
CREATE PROCEDURE `uspSetUserRedirectInfo`(
	IN `UserID` INT,
	IN `RemoteHost` VARCHAR(50),
	IN `Controller` VARCHAR(50),
	IN `ControllerAction` VARCHAR(50),
	IN `RouteID` INT,
	IN `QueryString` VARCHAR(100)
)
BEGIN
	REPLACE INTO tblstate(remote_host, user_id, redirect_controller, redirect_action, redirect_route_id, redirect_routevalues, event_dtm) VALUES(RemoteHost, UserID, Controller, ControllerAction, RouteID, QueryString, CURRENT_TIMESTAMP());
END//
DELIMITER ;

DELIMITER //
CREATE PROCEDURE `uspStoreActivationCode`(
	IN `UserID` INT,
	IN `ActivationCode` VARCHAR(36)
)
BEGIN
	INSERT INTO tblPasswordReset(user_id ,activation_code,  request_dtm) VALUE(UserID, ActivationCode, CURRENT_TIMESTAMP());
END//
DELIMITER ;

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
	
		IF(id > 0) THEN
		CALL uspAddAuditMessage(UserID, CONCAT('Set rank = ',UserRank, ' for (',PhotographyId,')'));
	END IF;

	SELECT id;
END//
DELIMITER ;

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
	
	IF(id > 0) THEN
		CALL uspAddAuditMessage(UserID, CONCAT('Update password for ''', login, ''' (', id ,')'));
	END IF;

	SELECT id;
END//
DELIMITER ;

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

CREATE TABLE `vwphotographydetails` (
	`Id` BIGINT(19) NOT NULL,
	`Filename` VARCHAR(50) NULL COLLATE 'utf8mb4_0900_ai_ci',
	`Location` VARCHAR(50) NULL COLLATE 'utf8mb4_0900_ai_ci',
	`Path` VARCHAR(255) NULL COLLATE 'utf8mb4_0900_ai_ci',
	`Title` VARCHAR(100) NULL COLLATE 'utf8mb4_0900_ai_ci',
	`Tags` TEXT NULL COLLATE 'utf8mb4_0900_ai_ci'
) ENGINE=MyISAM;

CREATE TABLE `vwphotographytags` (
	`photography_id` BIGINT(19) NOT NULL,
	`Taglist` TEXT NULL COLLATE 'utf8mb4_0900_ai_ci'
) ENGINE=MyISAM;

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

DROP TABLE IF EXISTS `vwphotographydetails`;
CREATE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `vwphotographydetails` AS select `p`.`id` AS `Id`,`p`.`filename` AS `Filename`,`loc`.`reference` AS `Location`,`p`.`_path` AS `Path`,`p`.`title` AS `Title`,`vw`.`Taglist` AS `Tags` from ((`tblphotography` `p` left join `tbllocation` `loc` on((`loc`.`id` = `p`.`location_id`))) left join `vwphotographytags` `vw` on((`vw`.`photography_id` = `p`.`id`)));

DROP TABLE IF EXISTS `vwphotographytags`;
CREATE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `vwphotographytags` AS select `p`.`id` AS `photography_id`,group_concat(distinct `t`.`word` separator ',') AS `Taglist` from ((`tblphotography` `p` join `tblphotographytags` `pt` on((`pt`.`photography_id` = `p`.`id`))) join `tbltag` `t` on((`t`.`id` = `pt`.`tag_id`))) group by `p`.`id`;

DROP TABLE IF EXISTS `vwphotographywithranking`;
CREATE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `vwphotographywithranking` AS select `p`.`Id` AS `Id`,`p`.`Filename` AS `Filename`,ifnull(`p`.`Location`,'') AS `Location`,`p`.`Path` AS `Path`,(case when (locate('slide',`p`.`Path`) > 0) then 1 else 0 end) AS `Source`,`p`.`Title` AS `Title`,ifnull(`p`.`Tags`,'') AS `Tags`,ifnull(`r`.`_rank`,0) AS `Rank`,ifnull(`udfGetAverageRank`(`p`.`Id`),0) AS `AverageRank` from (`vwphotographydetails` `p` left join `tblranking` `r` on(((`r`.`user_id` = `p1`()) and (`r`.`photography_id` = `p`.`Id`))));

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
