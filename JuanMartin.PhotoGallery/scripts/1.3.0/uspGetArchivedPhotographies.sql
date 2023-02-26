-- Dumping structure for procedure gallery.uspGetArchivedPhotographies
DROP PROCEDURE IF EXISTS `uspGetArchivedPhotographies`;
DELIMITER //
CREATE PROCEDURE `uspGetArchivedPhotographies`(
	IN `UserId` INT,
	IN `CurrentPage` INT,
	IN `PageSize` INT
)
BEGIN
	DECLARE rec_take INT;
	DECLARE rec_skip INT;
	
	SET rec_take = PageSize;
	SET rec_skip = (CurrentPage - 1) * PageSize;

	SELECT DISTINCT v.*
	FROM vwphotographywithranking v 
	WHERE v.Archive = 1
	GROUP BY v.Id
	ORDER BY v.AverageRank DESC,v.Id DESC
	LIMIT rec_take
	OFFSET rec_skip;
END//
DELIMITER ;
