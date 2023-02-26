-- Dumping structure for view gallery.vwphotographywithranking
DROP VIEW IF EXISTS `vwphotographywithranking`;
-- Removing temporary table and create final VIEW structure
DROP TABLE IF EXISTS `vwphotographywithranking`;
CREATE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `vwphotographywithranking` AS select `p`.`Id` AS `Id`,`p`.`Filename` AS `Filename`,ifnull(`p`.`Location`,'') AS `Location`,`p`.`Path` AS `Path`,(case when (locate('slide',`p`.`Path`) > 0) then 1 else 0 end) AS `Source`,`p`.`Title` AS `Title`,`p`.`Archive` AS `Archive`,ifnull(`p`.`Tags`,'') AS `Tags`,ifnull(`r`.`_rank`,0) AS `Rank`,ifnull(`udfGetAverageRank`(`p`.`Id`),0) AS `AverageRank` from (`vwphotographydetails` `p` left join `tblranking` `r` on(((`r`.`user_id` = `p1`()) and (`r`.`photography_id` = `p`.`Id`))));
