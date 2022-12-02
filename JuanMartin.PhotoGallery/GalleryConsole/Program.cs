// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
JsonApplicationSettings applicationSettings = new JsonApplicationSettings(@"C:\GitHub\JuanMartin.ToolSet\JuanMartin.PhotoGallery");
var connectionString = applicationSettings.ConnectionString;
//var path = @"C:\GitHub\JuanMartin.ToolSet\JuanMartin.PhotoGallery\wwwroot\photos.lnk";
var path = @"C:\GitHub\JuanMartin.ToolSet\JuanMartin.PhotoGallery\wwwroot\photos\digital";

photoService.LoadPhotographiesWithLocation(connectionString, path, ".jpg,.JPG", false, 1, "East  Africa");
