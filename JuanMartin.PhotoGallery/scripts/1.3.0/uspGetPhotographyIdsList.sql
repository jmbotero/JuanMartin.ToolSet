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
				WHERE v.Archive = 0
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
				AND v.Archive = 0
				ORDER BY v.AverageRank DESC, v.Id)
			UNION
			(
				SELECT v.*
				FROM vwphotographywithranking v 
				WHERE v.Location REGEXP SearchQuery
				AND v.Archive = 0
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
	ELSEIF (SetSource = 3) THEN /* archived from gallery */
	   SELECT IFNULL(GROUP_CONCAT(sub.Id),'') AS 'Ids', COUNT(*) AS 'RowCount'
	   FROM(
				SELECT v.*
				FROM vwphotographywithranking v 
				WHERE v.Location REGEXP SearchQuery
				AND v.Archive = 1
				ORDER BY v.AverageRank DESC, v.Id
			) sub;
	END IF;
END//
DELIMITER ;
