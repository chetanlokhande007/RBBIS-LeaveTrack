using System;

class Program {
    static void Main() {
        string hash1 = BCrypt.Net.BCrypt.HashPassword("manager123");
        string hash2 = BCrypt.Net.BCrypt.HashPassword("hr123");
        Console.WriteLine($"Manager Hash: {hash1}");
        Console.WriteLine($"HR Hash: {hash2}");
    }
}
