using System;
var passwords = new[] {
    ("admin",    "Admin@123"),
    ("nurse1",   "Nurse@123"),
    ("doctor1",  "Doctor@123"),
    ("patient1", "Patient@123"),
};
foreach (var (user, pass) in passwords)
{
    var hash = BCrypt.Net.BCrypt.HashPassword(pass, 11);
    Console.WriteLine($"{user}: {hash}");
}
