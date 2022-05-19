USE `photogallery`;

DELETE FROM `tblkeyword`;
INSERT INTO `tblkeyword` (`id`, `word`) VALUES
	(1, 'bear'),
	(2, 'tree');

DELETE FROM `tbllocation`;

DELETE FROM `tblphotography`;
INSERT INTO `tblphotography` (`id`, `_source`, `_path`, `title`, `filename`, `location_id`) VALUES
	(1, 0, '~\\photos\\negatives', '', '35mmNegatives01_0001 - Copy (2).jpg', NULL),
	(2, 0, '~\\photos\\negatives', '', '35mmNegatives01_0001 - Copy (3).jpg', NULL),
	(3, 0, '~\\photos\\negatives', '', '35mmNegatives01_0001 - Copy (4).jpg', NULL),
	(4, 0, '~\\photos\\negatives', '', '35mmNegatives01_0001 - Copy (5).jpg', NULL),
	(5, 0, '~\\photos\\negatives', '', '35mmNegatives01_0001 - Copy (6).jpg', NULL),
	(6, 0, '~\\photos\\negatives', '', '35mmNegatives01_0001 - Copy (7).jpg', NULL),
	(7, 0, '~\\photos\\negatives', '', '35mmNegatives01_0001 - Copy (8).jpg', NULL),
	(8, 0, '~\\photos\\negatives', '', '35mmNegatives01_0001 - Copy.jpg', NULL),
	(9, 0, '~\\photos\\negatives', '', '35mmNegatives01_0001.jpg', NULL),
	(10, 1, '~\\photos\\slide', '', 'Slide Memories 01_0001 - Copy (2).jpg', NULL),
	(11, 1, '~\\photos\\slide', '', 'Slide Memories 01_0001 - Copy (3).jpg', NULL),
	(12, 1, '~\\photos\\slide', '', 'Slide Memories 01_0001 - Copy (4).jpg', NULL),
	(13, 1, '~\\photos\\slide', '', 'Slide Memories 01_0001 - Copy (5).jpg', NULL),
	(14, 1, '~\\photos\\slide', '', 'Slide Memories 01_0001 - Copy (6).jpg', NULL),
	(15, 1, '~\\photos\\slide', '', 'Slide Memories 01_0001 - Copy (7).jpg', NULL),
	(16, 1, '~\\photos\\slide', '', 'Slide Memories 01_0001 - Copy (8).jpg', NULL),
	(17, 1, '~\\photos\\slide', '', 'Slide Memories 01_0001 - Copy.jpg', NULL),
	(18, 1, '~\\photos\\slide', '', 'Slide Memories 01_0001.jpg', NULL);

DELETE FROM `tblphotographykeywords`;
INSERT INTO `tblphotographykeywords` (`photography_id`, `keyword_id`) VALUES
	(1, 1),
	(1, 2);

DELETE FROM `tblranking`;

DELETE FROM `tbluser`;
INSERT INTO `tbluser` (`id`, `login`, `_password`, `email`) VALUES
	(1, 'juan', 'dc681250f7549ba735dcf6b5d13685c3', 'jbotero@hotmmail.com');

