// See https://aka.ms/new-console-template for more information
using Shallow.SQL;
using Shallow.SQL.Systems;
using Sharpquent.Tester.Models;

/*SQL.System = SystemType.MySQL;
SQL.Server = "localhost";
SQL.Database = "sharpquent";
SQL.User = "root";*/

SQL.System = SystemType.SQLite;
SQL.Database = $"{Environment.CurrentDirectory}\\..\\..\\..\\..\\Shallow.SQL.Test\\Shallow.SQL.db";

User[] users = User.Query.All();

foreach (User user in users)
{
    Console.WriteLine($"{user.ID} {user.Name} {user.Email} - Phone Number: {user.PhoneNumber}");
    foreach (Product product in user.Products)
        Console.WriteLine($"\t{product.ID} {product.Name} {product.Description} {product.Price} - Same User: {(product.Users[0].ID == user.ID ? "true" : "false")}");
}

User user2;
string originalName;
string tempName;

user2 = User.Query.FindById(2).First();
#pragma warning disable CS8600 // Se va a convertir un literal nulo o un posible valor nulo en un tipo que no acepta valores NULL
originalName = user2.Name;
#pragma warning restore CS8600 // Se va a convertir un literal nulo o un posible valor nulo en un tipo que no acepta valores NULL
user2.Name = "Alcachofi";
user2.Update();
Console.WriteLine($"Name changed from '{originalName}' to '{user2.Name}'");

user2 = User.Query.FindById(2).First();
#pragma warning disable CS8600 // Se va a convertir un literal nulo o un posible valor nulo en un tipo que no acepta valores NULL
tempName = user2.Name;
#pragma warning restore CS8600 // Se va a convertir un literal nulo o un posible valor nulo en un tipo que no acepta valores NULL
user2.Name = originalName;
user2.Update();
Console.WriteLine($"Name reversed from '{tempName}' to '{originalName}");

Console.WriteLine("--------------------------------------------------------------");
Console.ReadKey();