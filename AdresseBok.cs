using System.Net.Mail;
using Microsoft.Data.Sqlite;

namespace AdresseBok
{
    class AdresseBok
    {
        static List<Contacts> contacts = [];

        static void Main(string[] args)
        {
            Contacts.CreateTable(Contacts.InitDatabase());
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("Address Book");
                Console.WriteLine("1. View Contacts");
                Console.WriteLine("2. Add Contact");
                Console.WriteLine("3. Edit Contact");
                Console.WriteLine("4. Delete Contact");
                Console.WriteLine("5. Exit");

                switch (Console.ReadLine())
                {
                    case "1":
                        ViewContacts();
                        break;
                    case "2":
                        AddContact();
                        break;
                    case "3":
                        EditContact();
                        break;
                    case "4":
                        DeleteContact();
                        break;
                    case "5":
                        exit = true;
                        break;
                }
            }
        }

        static void AddContact()
        {
            Console.Clear();
            Console.WriteLine("Add Contact");
            string name = WriteName() ?? throw new ArgumentNullException(nameof(name));
            int phoneNumber = WriteNumber();
            string? email = WriteEmail() ?? throw new ArgumentNullException(nameof(email));
            int id = Contacts.AddContact(Contacts.InitDatabase(), name, phoneNumber, email);
            Console.WriteLine($"Contact added with ID: {id}");
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static string? WriteName()
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

        private static int WriteNumber()
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
                    Console.WriteLine(
                        "Phone number is invalid. Please enter a 8-digit phone number."
                    );
                }
            }
        }

        private static string? WriteEmail()
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

        static void ViewContacts()
        {
            Console.Clear();
            Console.WriteLine("Contacts:");
            ShowContacts();
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void EditContact()
        {
            Console.Clear();
            Console.WriteLine("Edit Contact");
            ShowContacts();
            Console.Write("Enter contact number to edit: ");
            int index = ParseNumber() + 1;
            string name = WriteName() ?? throw new ArgumentNullException(nameof(name));
            int phoneNumber = WriteNumber();
            string? email = WriteEmail() ?? throw new ArgumentNullException(nameof(email));
            int id = Contacts.EditContact(Contacts.InitDatabase(), index, name, phoneNumber, email);
            Console.WriteLine($"Contact added with ID: {id}");

            Console.WriteLine("Contact edited successfully!");
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static int ParseNumber()
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

        static void DeleteContact()
        {
            Console.Clear();
            Console.WriteLine("Delete Contact");
            ShowContacts();
            Console.Write("Enter contact number to delete: ");

            int index = ParseNumber() + 1;
            int id = Contacts.DeleteContact(Contacts.InitDatabase(), index);
            Console.WriteLine($"Contact with ID: {id} deleted successfully!");
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void ShowContacts()
        {
            SqliteDataReader contacts = Contacts.ReadContact(Contacts.InitDatabase());
            while (contacts.Read())
            {
                Console.WriteLine(
                    $"ID: {contacts.GetInt32(0)}, Name: {contacts.GetString(1)}, Phone Number: {contacts.GetInt32(2)}, Email: {contacts.GetString(3)}"
                );
            }
        }
    }
}
