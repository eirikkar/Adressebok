using System.Net.Mail;
using Microsoft.Data.Sqlite;

public class Utility
{
    public static int WriteNumber()
    {
        while (true)
        {
            Console.Write("Enter phone number: ");
            string? phoneNumberString = Console.ReadLine();
            if (
                int.TryParse(phoneNumberString, out int phoneNumber)
                && phoneNumberString.Length == 8
                && phoneNumber > 0
            )
            {
                return phoneNumber;
            }
            else
            {
                Console.WriteLine("Phone number is invalid. Please enter a 8-digit phone number.");
            }
        }
    }

    public static string? WriteEmail()
    {
        while (true)
        {
            Console.Write("Enter email: ");
            string? email = Console.ReadLine();
            if (!string.IsNullOrEmpty(email))
            {
                try
                {
                    email = new MailAddress(email).Address;
                    return email;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Email is invalid. Please enter a valid email.");
                }
            }
            else
            {
                Console.WriteLine("You must enter something.");
            }
        }
    }

    public static string? WriteName()
    {
        while (true)
        {
            Console.Write("Enter name: ");
            string? name = Console.ReadLine();
            if (string.IsNullOrEmpty(name) || name.Length < 2 || !name.All(char.IsLetter))
            {
                Console.WriteLine("Name is invalid. Please enter a valid name.");
            }
            else
            {
                return name;
            }
        }
    }

    public static int ParseNumber()
    {
        while (true)
        {
            string? input = Console.ReadLine();
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out int index) || index < 1)
            {
                Console.Write("Invalid input. Please enter a valid number: ");
            }
            else
            {
                return index - 1;
            }
        }
    }

    public static bool ShowContacts()
    {
        using var db = Contacts.InitDatabase();
        try
        {
            db.Open();
            using var cmd = new SqliteCommand("SELECT * FROM Contacts", db);
            using var reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var contact = new Contacts
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        PhoneNumber = reader.GetInt32(2),
                        Email = reader.GetString(3),
                    };
                    Console.WriteLine(
                        $"ID: {contact.Id}, Name: {contact.Name}, Phone: {contact.PhoneNumber}, Email: {contact.Email}"
                    );
                }
                return true;
            }
            else
            {
                Console.WriteLine("No contacts in storage.");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
        finally
        {
            db.Close();
        }
    }
}
