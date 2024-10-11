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
            int index = ParseNumber();
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
            if (contacts.Count == 0)
            {
                Console.WriteLine("No contacts found.");
                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }
            else
            {
                ShowContacts();
                Console.Write("Enter contact number to delete: ");

                while (true)
                {
                    int index = ParseNumber();
                    if (index >= contacts.Count)
                    {
                        Console.WriteLine("Contact not found.");
                        Console.WriteLine();
                        Console.Write("Please enter a valid number: ");
                    }
                    else
                    {
                        contacts.RemoveAt(index);
                        Console.WriteLine();
                        Console.WriteLine("Contact deleted successfully!");
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;
                    }
                }
            }
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

    public class Contacts
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int PhoneNumber { get; set; }
        public string? Email { get; set; }

        public static SqliteConnection InitDatabase()
        {
            SqliteConnection sqliteCon;
            sqliteCon = new SqliteConnection("Data Source=db.sqlite");
            try
            {
                sqliteCon.Open();
            }
            catch (SqliteException e)
            {
                Console.WriteLine(e.Message);
            }
            return sqliteCon;
        }

        public static SqliteConnection CloseDatabase()
        {
            SqliteConnection sqliteCon;
            sqliteCon = new SqliteConnection("Data Source=db.sqlite");
            try
            {
                sqliteCon.Close();
            }
            catch (SqliteException e)
            {
                Console.WriteLine(e.Message);
            }
            return sqliteCon;
        }

        public static void CreateTable(SqliteConnection conn)
        {
            SqliteCommand sqliteCommand;
            string createSQL =
                "CREATE TABLE IF NOT EXISTS Contacts (Id INTEGER PRIMARY KEY, Name TEXT NOT NULL, PhoneNumber INTEGER NOT NULL, Email TEXT NOT NULL)";
            sqliteCommand = conn.CreateCommand();
            sqliteCommand.CommandText = createSQL;
            sqliteCommand.ExecuteNonQuery();
            conn.Close();
        }

        public static int AddContact(
            SqliteConnection conn,
            string name,
            int phoneNumber,
            string email
        )
        {
            SqliteCommand sqliteCommand;
            sqliteCommand = conn.CreateCommand();
            sqliteCommand.CommandText =
                "INSERT INTO Contacts (Name, PhoneNumber, Email) VALUES (@Name, @PhoneNumber, @Email)";
            sqliteCommand.Parameters.AddWithValue("@Name", name);
            sqliteCommand.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
            sqliteCommand.Parameters.AddWithValue("@Email", email);
            sqliteCommand.ExecuteNonQuery();

            sqliteCommand.CommandText = "SELECT last_insert_rowid()";
            int id = Convert.ToInt32(sqliteCommand.ExecuteScalar());
            conn.Close();
            return id;
        }

        public static SqliteDataReader ReadContact(SqliteConnection conn)
        {
            SqliteDataReader sqliteDataReader;
            SqliteCommand sqliteCommand;
            sqliteCommand = conn.CreateCommand();
            sqliteCommand.CommandText = "SELECT * FROM Contacts";
            sqliteDataReader = sqliteCommand.ExecuteReader();
            return sqliteDataReader;
        }

        public static int EditContact(
            SqliteConnection conn,
            int id,
            string name,
            int phoneNumber,
            string email
        )
        {
            SqliteCommand sqliteCommand;
            sqliteCommand = conn.CreateCommand();
            sqliteCommand.CommandText =
                "UPDATE Contacts FROM Id SET Name = @Name, PhoneNumber = @PhoneNumber, Email = @Email";
            sqliteCommand.Parameters.AddWithValue("@Name", name);
            sqliteCommand.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
            sqliteCommand.Parameters.AddWithValue("@Email", email);
            sqliteCommand.ExecuteNonQuery();
            return id;
        }
    }
}
