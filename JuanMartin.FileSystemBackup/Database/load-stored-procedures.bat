SET source=C:\Git\JuanMartin.ToolSet\JuanMartin.FileSystemBackup\Database\*.sql
SET mysql=C:\Program Files\MySQL\MySQL Shell 8.0\bin\mysqlsh.exe
SET databasename=backup
SET user=root
SET password=yala
SET output=C:\Git\JuanMartin.ToolSet\JuanMartin.FileSystemBackup\Database\load-procedures.log

FOR %%A IN ("%source%") DO "%mysql%" --user=%user% --password=%password% %databasename% < %%A >%output%