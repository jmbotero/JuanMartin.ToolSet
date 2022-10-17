-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server version:               8.0.30 - MySQL Community Server - GPL
-- Server OS:                    Win64
-- HeidiSQL Version:             12.1.0.6537
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Dumping database structure for gallery
DROP DATABASE IF EXISTS `gallery`;
CREATE DATABASE IF NOT EXISTS `gallery` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `gallery`;

-- Dumping structure for function gallery.p1
DROP FUNCTION IF EXISTS `p1`;
DELIMITER //
CREATE FUNCTION `p1`() RETURNS int
    NO SQL
    DETERMINISTIC
return @p1//
DELIMITER ;

-- Dumping structure for table gallery.tblaudit
DROP TABLE IF EXISTS `tblaudit`;
CREATE TABLE IF NOT EXISTS `tblaudit` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int NOT NULL,
  `event_dtm` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `is_error` tinyint(1) NOT NULL DEFAULT '0',
  `message` varchar(250) NOT NULL,
  `_source` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=185 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Data exporting was unselected.

-- Dumping structure for table gallery.tbllocation
DROP TABLE IF EXISTS `tbllocation`;
CREATE TABLE IF NOT EXISTS `tbllocation` (
  `id` int NOT NULL AUTO_INCREMENT,
  `ddd` float DEFAULT NULL,
  `_reference` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Data exporting was unselected.

-- Dumping structure for table gallery.tblorder
DROP TABLE IF EXISTS `tblorder`;
CREATE TABLE IF NOT EXISTS `tblorder` (
  `id` int NOT NULL AUTO_INCREMENT,
  `_number` varchar(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `user_id` int DEFAULT NULL,
  `created_dtm` datetime NOT NULL,
  `_status` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'pending',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Data exporting was unselected.

-- Dumping structure for table gallery.tblorderitem
DROP TABLE IF EXISTS `tblorderitem`;
CREATE TABLE IF NOT EXISTS `tblorderitem` (
  `id` int NOT NULL AUTO_INCREMENT,
  `order_id` int NOT NULL,
  `photography_id` bigint NOT NULL,
  `_index` int NOT NULL DEFAULT '1',
  `add_dtm` datetime NOT NULL,
  `update_dtm` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Data exporting was unselected.

-- Dumping structure for table gallery.tblpasswordreset
DROP TABLE IF EXISTS `tblpasswordreset`;
CREATE TABLE IF NOT EXISTS `tblpasswordreset` (
  `user_id` int NOT NULL,
  `activation_code` varchar(36) NOT NULL DEFAULT '',
  `request_dtm` datetime DEFAULT NULL,
  PRIMARY KEY (`activation_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Data exporting was unselected.

-- Dumping structure for table gallery.tblphotography
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
) ENGINE=InnoDB AUTO_INCREMENT=91 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Data exporting was unselected.

-- Dumping structure for table gallery.tblphotographytags
DROP TABLE IF EXISTS `tblphotographytags`;
CREATE TABLE IF NOT EXISTS `tblphotographytags` (
  `photography_id` bigint DEFAULT NULL,
  `tag_id` int DEFAULT NULL,
  KEY `FK_keyword_photography` (`photography_id`),
  KEY `FK_keyword` (`tag_id`) USING BTREE,
  CONSTRAINT `FK_keyword` FOREIGN KEY (`tag_id`) REFERENCES `tbltag` (`id`),
  CONSTRAINT `FK_keyword_photography` FOREIGN KEY (`photography_id`) REFERENCES `tblphotography` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Data exporting was unselected.

-- Dumping structure for table gallery.tblranking
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
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Data exporting was unselected.

-- Dumping structure for table gallery.tblsession
DROP TABLE IF EXISTS `tblsession`;
CREATE TABLE IF NOT EXISTS `tblsession` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` int NOT NULL,
  `start_dtm` datetime NOT NULL,
  `end_dtm` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=111 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Data exporting was unselected.

-- Dumping structure for table gallery.tblstate
DROP TABLE IF EXISTS `tblstate`;
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

-- Data exporting was unselected.

-- Dumping structure for table gallery.tbltag
DROP TABLE IF EXISTS `tbltag`;
CREATE TABLE IF NOT EXISTS `tbltag` (
  `id` int NOT NULL AUTO_INCREMENT,
  `word` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Data exporting was unselected.

-- Dumping structure for table gallery.tbluser
DROP TABLE IF EXISTS `tbluser`;
CREATE TABLE IF NOT EXISTS `tbluser` (
  `id` int NOT NULL AUTO_INCREMENT,
  `login` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `_password` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `email` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Data exporting was unselected.

-- Dumping structure for function gallery.udfGetAverageRank
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

-- Dumping structure for procedure gallery.uspAddAuditMessage
DROP PROCEDURE IF EXISTS `uspAddAuditMessage`;
DELIMITER //
CREATE PROCEDURE `uspAddAuditMessage`(
	IN `UserID` INT,
	IN `Message` VARCHAR(250),
	IN `_Source` VARCHAR(100),
	IN `IsError` TINYINT(1)
)
BEGIN
	INSERT INTO tblaudit(user_id, event_dtm, message, is_error,_source) VALUES(UserID, CURRENT_TIMESTAMP(), Message, IsError,_Source);
END//
DELIMITER ;

-- Dumping structure for procedure gallery.uspAddOrder
DROP PROCEDURE IF EXISTS `uspAddOrder`;
DELIMITER //
CREATE PROCEDURE `uspAddOrder`(
	IN `UserID` INT
)
BEGIN
	DECLARE id INT;
	DECLARE _uuid VARCHAR(36);
	DECLARE _status VARCHAR(10);
	DECLARE orderExists INT;
	
	SET _status = 'pending';
	SET id = -1;
	SET orderExists = EXISTS(
							SELECT o.*
							FROM tblorder o
							WHERE o.user_id = UserId
							AND o._status = _status);

	IF (orderExists = 0) THEN 	
		SET _uuid = UUID();
		INSERT INTO tblOrder(user_id,_number,created_dtm,_status) VALUE(UserID, _uuid,CURRENT_TIMESTAMP(),_status);
		SET id = LAST_INSERT_ID();

		CALL uspAddAuditMessage(UserId, CONCAT('Add Order (',id,') #',_uuid,')'),'uspAddOrder',0);

	ELSE
		SET id = -2; /* order already exists */
	END IF;
	

 	CALL uspGetOrder(id, UserId, id);  
END//
DELIMITER ;

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
 	WHERE pic.filename = FileName;
	
	IF NOT FOUND_ROWS() THEN 	
		INSERT INTO tblPhotography (_source,filename, _path, title) VALUE(ImageSource,FileName, FilePath, Title);
		SET id=LAST_INSERT_ID();
	END IF;	

	SELECT id;
END//
DELIMITER ;

-- Dumping structure for procedure gallery.uspAddPhotographyToOrder
DROP PROCEDURE IF EXISTS `uspAddPhotographyToOrder`;
DELIMITER //
CREATE PROCEDURE `uspAddPhotographyToOrder`(
	IN `PhotographyId` BIGINT,
	IN `OrderId` INT,
	IN `UserId` INT
)
BEGIN
	DECLARE id INT;
	DECLARE pos INT;
	
	SET id=-1;

	IF(UserId<>-1) THEN
		SELECT p.id
		INTO id
		FROM tblphotography p
	 	WHERE p.id = PhotographyId;
	 	
		IF FOUND_ROWS() THEN 	
	 	
	 		SELECT COUNT(*)
	 		INTO pos
	 		FROM tblorderitem o
		 	WHERE o.order_id = OrderId;
	 		SET pos = pos + 1;
	 		
			SELECT o.id
			FROM tblorder o
		 	WHERE o.id = OrderId;
			
			IF FOUND_ROWS() THEN
				INSERT INTO tblOrderItem(order_id, photography_id, _index, add_dtm) VALUES(OrderId, PhotographyId, pos, CURRENT_TIMESTAMP());
				SET id=LAST_INSERT_ID();
			ELSE
				SET id = -1;
			END IF;
		ELSE
			SET id = -1;
		END IF;
	END IF;	
	
	IF(id > 0) THEN
		CALL uspAddAuditMessage(UserId, CONCAT('Add photography (',PhotographyId,') to Order (',OrderId,')'),'uspAddPhotographyToOrder',0);
	END IF;

	SELECT id;
END//
DELIMITER ;

-- Dumping structure for procedure gallery.uspAddSession
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

-- Dumping structure for procedure gallery.uspAddTag
DROP PROCEDURE IF EXISTS `uspAddTag`;
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
		CALL uspAddAuditMessage(UserID, CONCAT('Add tag ''',Word, ''' for (',PhotographyId,')'),'uspAddTag',0);
	END IF;
	SELECT id;
END//
DELIMITER ;

-- Dumping structure for procedure gallery.uspAddUser
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

-- Dumping structure for procedure gallery.uspConnectUserAndRemoteHost
DROP PROCEDURE IF EXISTS `uspConnectUserAndRemoteHost`;
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

-- Dumping structure for procedure gallery.uspEndSession
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

-- Dumping structure for procedure gallery.uspExecuteSqlStatement
DROP PROCEDURE IF EXISTS `uspExecuteSqlStatement`;
DELIMITER //
CREATE PROCEDURE `uspExecuteSqlStatement`(
	IN `Statement` VARCHAR(500)
)
BEGIN
	SET @qry = Statement;
	
	PREPARE stmt FROM @qry;
	EXECUTE stmt;
	DEALLOCATE PREPARE stmt;
END//
DELIMITER ;

-- Dumping structure for procedure gallery.uspGetAllPhotographies
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
	ORDER BY v.AverageRank DESC, v.Id DESC
	LIMIT rec_take
	OFFSET rec_skip;
END//
DELIMITER ;

-- Dumping structure for procedure gallery.uspGetCurrentActiveOrder
DROP PROCEDURE IF EXISTS `uspGetCurrentActiveOrder`;
DELIMITER //
CREATE PROCEDURE `uspGetCurrentActiveOrder`(
	IN `UserId` INT
)
    NO SQL
BEGIN
 	DECLARE oid INT;
 	
 	SELECT MAX(o.id)
 	INTO oid
 	FROM tblorder o
 	WHERE o.user_id = UserId
 	AND o._status = 'pending'
	ORDER BY o.created_dtm DESC;
	 	
 	IF (oid = NULL) THEN 	
		SET oid = -1;
	END IF;

 	CALL uspGetOrder(oid, UserId, -1);
END//
DELIMITER ;

-- Dumping structure for procedure gallery.uspGetLocations
DROP PROCEDURE IF EXISTS `uspGetLocations`;
DELIMITER //
CREATE PROCEDURE `uspGetLocations`(
	IN `CurrentPage` INT,
	IN `PageSize` INT
)
BEGIN
	DECLARE rec_take INT;
	DECLARE rec_skip INT;
	
	SET rec_take = PageSize;
	SET rec_skip = (CurrentPage - 1) * PageSize;

	SELECT DISTINCT l._reference AS 'Location'
	FROM tblLocation l
	LIMIT rec_take
	OFFSET rec_skip;
END//
DELIMITER ;

-- Dumping structure for procedure gallery.uspGetOrder
DROP PROCEDURE IF EXISTS `uspGetOrder`;
DELIMITER //
CREATE PROCEDURE `uspGetOrder`(
	IN `OrderId` INT,
	IN `UserId` INT,
	IN `ErrorId` INT
)
    COMMENT 'The ErrorId should have a default value but always pass -1 if you don''t want this ErrporId to overwrite the otder id in the response.'
BEGIN
	DECLARE orderExists INT;
	DECLARE cnt INT;

	IF (ErrorId=-1) THEN 		
		SET orderExists = EXISTS(
								SELECT o.id
								FROM tblorder o
								WHERE o.id = OrderId
								AND o.user_id = UserId);
	
		IF (orderExists = 0) THEN 	
			SELECT -1 AS 'Id',
					 '' AS 'Number',
					 UserId AS 'UserId',
					 NOW() AS 'CreatedDtm',
					 0 AS 'Count',
					 '' AS 'Status';
		ELSE
			 		SELECT COUNT(*)
	 		INTO cnt
	 		FROM tblorderitem o
		 	WHERE o.order_id = OrderId;
		 	
			SELECT o.id AS 'Id',
				 o._number AS 'Number',
				 o.user_id  AS 'UserId',
				 o.created_dtm AS 'CreatedDtm',
				 cnt AS 'Count',
				 o._status AS 'Status'
			FROM tblorder o
			WHERE o.id = OrderId
			AND o.user_id = UserId;
		END IF;
	ELSE
		SELECT ErrorId AS 'Id',
				 '' AS 'Number',
				 UserId AS 'UserId',
				 NOW() AS 'CreatedDtm',
					 0 AS 'Count',
				 '' AS 'Status';
	END IF;
END//
DELIMITER ;

-- Dumping structure for procedure gallery.uspGetOrderPhotographies
DROP PROCEDURE IF EXISTS `uspGetOrderPhotographies`;
DELIMITER //
CREATE PROCEDURE `uspGetOrderPhotographies`(
	IN `UserId` INT,
	IN `OrderId` INT,
	IN `CurrentPage` INT,
	IN `PageSize` INT
)
BEGIN
	DECLARE rec_skip INT;
	DECLARE rec_take INT;
	
	SET rec_take = PageSize;
	SET rec_skip = (CurrentPage - 1) * PageSize;

	SELECT v.*
	FROM (SELECT @p1:=UserId p) parm ,tblorderitem o 
	JOIN vwphotographywithranking v 
		ON v.id = o.photography_id
	WHERE o.order_id = OrderId
	ORDER BY o._index ASC
	LIMIT rec_take
	OFFSET rec_skip;
END//
DELIMITER ;

-- Dumping structure for procedure gallery.uspGetPageCount
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

-- Dumping structure for procedure gallery.uspGetPhotographiesBySearch
DROP PROCEDURE IF EXISTS `uspGetPhotographiesBySearch`;
DELIMITER //
CREATE PROCEDURE `uspGetPhotographiesBySearch`(
	IN `UserID` INT,
	IN `SearchQuery` VARCHAR(150),
	IN `CurrentPage` INT,
	IN `PageSize` INT
)
    NO SQL
BEGIN
	DECLARE rec_take INT;
	DECLARE rec_skip INT;
	
	SET rec_take = PageSize;
	SET rec_skip = (CurrentPage - 1) * PageSize;

	(SELECT v.*
	FROM (SELECT @p1:=UserID p) parm ,tblphotographytags pt 
	JOIN vwphotographywithranking v 
		ON v.id = pt.photography_id
	JOIN tbltag t 
		ON t.id = pt.tag_id 
	WHERE t.word REGEXP SearchQuery
	GROUP BY v.Id
	ORDER BY v.AverageRank DESC,v.Id DESC)
	UNION
	(SELECT v.*
	FROM vwphotographywithranking v 
	WHERE v.Location REGEXP SearchQuery
	GROUP BY v.Id
	ORDER BY v.AverageRank DESC,v.Id DESC)
	LIMIT rec_take
	OFFSET rec_skip;
END//
DELIMITER ;

-- Dumping structure for procedure gallery.uspGetPhotographyIdsList
DROP PROCEDURE IF EXISTS `uspGetPhotographyIdsList`;
DELIMITER //
CREATE PROCEDURE `uspGetPhotographyIdsList`(
	IN `UserId` INT,
	IN `SetSource` INT,
	IN `SearchQuery` VARCHAR(150),
	IN `OrderId` INT
)
    NO SQL
    SQL SECURITY INVOKER
BEGIN
	IF (SetSource = 0) THEN /* from all gallery */
	   SELECT IFNULL(GROUP_CONCAT(sub.Id),'') AS 'Ids', COUNT(*) AS 'RowCount'
	   FROM(
				SELECT v.*
				FROM  (SELECT @p1:=UserId p) parm , vwphotographywithranking v
				ORDER BY v.AverageRank DESC, v.Id) sub;
	ELSEIF (SetSource = 1) THEN /* from search query*/
	   SELECT IFNULL(GROUP_CONCAT(sub.Id),'') AS 'Ids', COUNT(*) AS 'RowCount'
		FROM 
			((
				SELECT v.*
				FROM (SELECT @p1:=UserId p) parm ,tblphotographytags pt 
				JOIN vwphotographywithranking v 
					ON v.id = pt.photography_id
				JOIN tbltag t 
					ON t.id = pt.tag_id 
				WHERE t.word REGEXP SearchQuery
				ORDER BY v.AverageRank DESC, v.Id)
			UNION
			(
				SELECT v.*
				FROM vwphotographywithranking v 
				WHERE v.Location REGEXP SearchQuery
				ORDER BY v.AverageRank DESC, v.Id
			))sub;
	ELSEIF (SetSource = 2) THEN /* from order*/
	   SELECT IFNULL(GROUP_CONCAT(sub.Id),'') AS 'Ids', COUNT(*) AS 'RowCount'
	   FROM(
				SELECT v.*
				FROM (SELECT @p1:=UserId p) parm ,tblorderitem o 
				JOIN vwphotographywithranking v 
				ON v.id = o.photography_id
				WHERE o.order_id = OrderId
				ORDER BY o._index ASC
			) sub;
	END IF;
END//
DELIMITER ;

-- Dumping structure for procedure gallery.uspGetPotography
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

-- Dumping structure for procedure gallery.uspGetTags
DROP PROCEDURE IF EXISTS `uspGetTags`;
DELIMITER //
CREATE PROCEDURE `uspGetTags`(
	IN `CurrentPage` INT,
	IN `PageSize` INT
)
BEGIN
	DECLARE rec_take INT;
	DECLARE rec_skip INT;
	
	SET rec_take = PageSize;
	SET rec_skip = (CurrentPage - 1) * PageSize;

	SELECT DISTINCT t.word AS  'Tag'
	FROM tbltag t
	LIMIT rec_take
	OFFSET rec_skip;
END//
DELIMITER ;

-- Dumping structure for procedure gallery.uspGetUser
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

-- Dumping structure for procedure gallery.uspGetUserRedirectInfo
DROP PROCEDURE IF EXISTS `uspGetUserRedirectInfo`;
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

-- Dumping structure for procedure gallery.uspIsPhotographyInOrder
DROP PROCEDURE IF EXISTS `uspIsPhotographyInOrder`;
DELIMITER //
CREATE PROCEDURE `uspIsPhotographyInOrder`(
	IN `OrderId` INT,
	IN `PhotographyId` BIGINT,
	IN `UserId` INT
)
BEGIN
	DECLARE itemExists INT;
	
	SET itemExists = EXISTS(
							SELECT oi.id
							FROM tblorder o
							JOIN tblorderitem oi
							ON oi.order_id = o.id
						 	WHERE o.user_id = UserID
						   AND o.id =  OrderId
							AND oi.photography_id = PhotographyId);
	
	IF (itemExists = 1) THEN 	
		SELECT 'true' AS 'Result';
	END IF;
END//
DELIMITER ;

-- Dumping structure for procedure gallery.uspRemoveOrder
DROP PROCEDURE IF EXISTS `uspRemoveOrder`;
DELIMITER //
CREATE PROCEDURE `uspRemoveOrder`(
	IN `OrderId` INT,
	IN `UserId` INT
)
BEGIN
	DECLARE id INT;
	
	SELECT o.id
	INTO id
	FROM tblorder o
	WHERE o.user_id = UserId
	AND o.id = OrderId;

	IF FOUND_ROWS() THEN 	
		DELETE o.*
		FROM tblorder o
		WHERE o.user_id = UserId
		AND o.id = OrderId;
		
		DELETE oi.*
		FROM tblorderitem oi
		WHERE oi.order_id = OrderId;
	ELSE
		SET id = -1;
	END IF;

	IF(id > 0) THEN
		CALL uspAddAuditMessage(UserId, CONCAT('Remove Order (',OrderId,')'),'uspRemoveOrder',0);
	END IF;
	
	SELECT id;
END//
DELIMITER ;

-- Dumping structure for procedure gallery.uspRemovePhotographyFromOrder
DROP PROCEDURE IF EXISTS `uspRemovePhotographyFromOrder`;
DELIMITER //
CREATE PROCEDURE `uspRemovePhotographyFromOrder`(
	IN `PhotographyId` BIGINT,
	IN `OrderId` INT,
	IN `UserId` INT
)
BEGIN
	DECLARE id INT;
	
	SELECT oi.id
	INTO id
	FROM tblorder o
	JOIN tblorderitem oi
	ON oi.order_id = o.id
 	WHERE o.user_id = UserID
   AND o.id =  OrderId
	AND oi.photography_id = PhotographyId;
	
	IF FOUND_ROWS() THEN 	
		DELETE oi.*
		FROM tblorder o
		JOIN tblorderitem oi
		ON oi.order_id = o.id
	 	WHERE o.user_id = UserID
	   AND o.id =  OrderId
		AND oi.photography_id = PhotographyId;
	ELSE
		SET id=-1;
	END IF;
	
	IF(id > 0) THEN
		CALL uspAddAuditMessage(UserId, CONCAT('Remove photography (',PhotographyId,') from Order (',OrderId,')'),'uspRemovePhotographyFromOrder',0);
	END IF;

	SELECT id;
END//
DELIMITER ;

-- Dumping structure for procedure gallery.uspRemoveTag
DROP PROCEDURE IF EXISTS `uspRemoveTag`;
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
		CALL uspAddAuditMessage(UserID, CONCAT('Remove tag ''',Word,'''  for (',PhotographyId,')'),'uspRemoveTag',0);
	END IF;

	SELECT id;
END//
DELIMITER ;

-- Dumping structure for procedure gallery.uspSetUserRedirectInfo
DROP PROCEDURE IF EXISTS `uspSetUserRedirectInfo`;
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

-- Dumping structure for procedure gallery.uspStoreActivationCode
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

-- Dumping structure for procedure gallery.uspUpdateOrderIndex
DROP PROCEDURE IF EXISTS `uspUpdateOrderIndex`;
DELIMITER //
CREATE PROCEDURE `uspUpdateOrderIndex`(
	IN `UserId` INT,
	IN `OrderId` INT,
	IN `PhotographyId` BIGINT,
	IN `_Index` INT
)
BEGIN
	DECLARE previousIndex INT;
	
	SELECT oi._index
	INTO previousIndex
	FROM tblorderitem oi
	WHERE oi.order_id = OrderId
	AND oi.photography_id = PhotographyId;

	IF(previousIndex <> _Index) THEN
		UPDATE tblorderitem oi
			SET oi._index = _Index,
				 oi.update_dtm = CURRENT_TIMESTAMP()
		WHERE oi.order_id = OrderId
		AND oi.photography_id = PhotographyId;
			
		CALL uspAddAuditMessage(UserID, CONCAT('Changed index of Photography (',PhotographyId,') in order  (',OrderId,') to: ',_Index,'.'),'uspUpdateOrderIndex',0);
	END IF;
END//
DELIMITER ;

-- Dumping structure for procedure gallery.uspUpdatePhotographyDetails
DROP PROCEDURE IF EXISTS `uspUpdatePhotographyDetails`;
DELIMITER //
CREATE PROCEDURE `uspUpdatePhotographyDetails`(
	IN `UserId` INT,
	IN `PhotographyId` BIGINT,
	IN `Location` VARCHAR(50)
)
    NO SQL
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
		SELECT l.id 
		INTO id
		FROM tblLocation l
	 	WHERE l._reference = Location;
		
		IF NOT FOUND_ROWS() THEN 	
			INSERT INTO tblLocation(_reference) VALUE(Location);
			SET id=LAST_INSERT_ID();
		END IF;
		
		IF(PhotographyId<>-1 AND id > 0) THEN
			UPDATE tblphotography p
				SET p.location_id = id
			WHERE p.id = PhotographyId;
		END IF;
	END IF;
	
	IF(id > 0) THEN
		CALL uspAddAuditMessage(UserID, CONCAT('Update location ''', Location , ''' (', id , ') for photography (', PhotographyId ,')'),'uspUpdatePhotographyDetails',0);
	END IF;
	SELECT id;
END//
DELIMITER ;

-- Dumping structure for procedure gallery.uspUpdateRanking
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
	
		IF(id > 0) THEN
		CALL uspAddAuditMessage(UserID, CONCAT('Set rank = ',UserRank, ' for (',PhotographyId,')'),'uspUpdateRanking',0);
	END IF;

	SELECT id;
END//
DELIMITER ;

-- Dumping structure for procedure gallery.uspUpdateUserPassword
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
	
	IF(id > 0) THEN
		CALL uspAddAuditMessage(UserID, CONCAT('Update password for ''', login, ''' (', id ,')'),'uspUpdateUserPassword',0);
	END IF;

	SELECT id;
END//
DELIMITER ;

-- Dumping structure for procedure gallery.uspVerifyActivationCode
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

-- Dumping structure for procedure gallery.uspVerifyEmail
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

-- Dumping structure for view gallery.vwphotographydetails
DROP VIEW IF EXISTS `vwphotographydetails`;
-- Creating temporary table to overcome VIEW dependency errors
CREATE TABLE `vwphotographydetails` (
	`Id` BIGINT(19) NOT NULL,
	`Filename` VARCHAR(50) NULL COLLATE 'utf8mb4_0900_ai_ci',
	`Location` VARCHAR(50) NULL COLLATE 'utf8mb4_0900_ai_ci',
	`Path` VARCHAR(255) NULL COLLATE 'utf8mb4_0900_ai_ci',
	`Title` VARCHAR(100) NULL COLLATE 'utf8mb4_0900_ai_ci',
	`Tags` TEXT NULL COLLATE 'utf8mb4_0900_ai_ci'
) ENGINE=MyISAM;

-- Dumping structure for view gallery.vwphotographytags
DROP VIEW IF EXISTS `vwphotographytags`;
-- Creating temporary table to overcome VIEW dependency errors
CREATE TABLE `vwphotographytags` (
	`photography_id` BIGINT(19) NOT NULL,
	`Taglist` TEXT NULL COLLATE 'utf8mb4_0900_ai_ci'
) ENGINE=MyISAM;

-- Dumping structure for view gallery.vwphotographywithranking
DROP VIEW IF EXISTS `vwphotographywithranking`;
-- Creating temporary table to overcome VIEW dependency errors
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

-- Dumping structure for view gallery.vwphotographydetails
DROP VIEW IF EXISTS `vwphotographydetails`;
-- Removing temporary table and create final VIEW structure
DROP TABLE IF EXISTS `vwphotographydetails`;
CREATE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `vwphotographydetails` AS select `p`.`id` AS `Id`,`p`.`filename` AS `Filename`,`loc`.`_reference` AS `Location`,`p`.`_path` AS `Path`,`p`.`title` AS `Title`,`vw`.`Taglist` AS `Tags` from ((`tblphotography` `p` left join `tbllocation` `loc` on((`loc`.`id` = `p`.`location_id`))) left join `vwphotographytags` `vw` on((`vw`.`photography_id` = `p`.`id`)));

-- Dumping structure for view gallery.vwphotographytags
DROP VIEW IF EXISTS `vwphotographytags`;
-- Removing temporary table and create final VIEW structure
DROP TABLE IF EXISTS `vwphotographytags`;
CREATE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `vwphotographytags` AS select `p`.`id` AS `photography_id`,group_concat(distinct `t`.`word` separator ',') AS `Taglist` from ((`tblphotography` `p` join `tblphotographytags` `pt` on((`pt`.`photography_id` = `p`.`id`))) join `tbltag` `t` on((`t`.`id` = `pt`.`tag_id`))) group by `p`.`id`;

-- Dumping structure for view gallery.vwphotographywithranking
DROP VIEW IF EXISTS `vwphotographywithranking`;
-- Removing temporary table and create final VIEW structure
DROP TABLE IF EXISTS `vwphotographywithranking`;
CREATE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `vwphotographywithranking` AS select `p`.`Id` AS `Id`,`p`.`Filename` AS `Filename`,ifnull(`p`.`Location`,'') AS `Location`,`p`.`Path` AS `Path`,(case when (locate('slide',`p`.`Path`) > 0) then 1 else 0 end) AS `Source`,`p`.`Title` AS `Title`,ifnull(`p`.`Tags`,'') AS `Tags`,ifnull(`r`.`_rank`,0) AS `Rank`,ifnull(`udfGetAverageRank`(`p`.`Id`),0) AS `AverageRank` from (`vwphotographydetails` `p` left join `tblranking` `r` on(((`r`.`user_id` = `p1`()) and (`r`.`photography_id` = `p`.`Id`))));

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
