namespace AdresseBok
{
    class AdresseBok
    {
        static void Main(string[] args)
        {
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

        static void ViewContacts()
        {
            Console.Clear();
            Console.WriteLine("Contacts:");
            if (!Utility.ShowContacts())
            {
                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void AddContact()
        {
            Console.Clear();
            Console.WriteLine("Add Contact");
            string name = Utility.WriteName() ?? throw new ArgumentNullException(nameof(name));
            int phoneNumber = Utility.WriteNumber();
            string? email = Utility.WriteEmail() ?? throw new ArgumentNullException(nameof(email));
            int id = Contacts.AddContact(name, phoneNumber, email);
            Console.WriteLine($"Contact added with ID: {id}");
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void EditContact()
        {
            Console.Clear();
            Console.WriteLine("Edit Contact");
            if (!Utility.ShowContacts())
            {
                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }
            Console.Write("Enter contact number to edit: ");

            while (true)
            {
                int index = Utility.ParseNumber() + 1;
                if (index > Contacts.GetCount() || index < 1)
                {
                    Console.Write("Invalid input. Please enter a valid number: ");
                }
                else
                {
                    string name =
                        Utility.WriteName() ?? throw new ArgumentNullException(nameof(name));
                    int phoneNumber = Utility.WriteNumber();
                    string? email =
                        Utility.WriteEmail() ?? throw new ArgumentNullException(nameof(email));
                    int id = Contacts.EditContact(index, name, phoneNumber, email);
                    Console.WriteLine($"Contact added with ID: {id}");

                    Console.WriteLine("Contact edited successfully!");
                    Console.WriteLine();
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    break;
                }
            }
        }

        static void DeleteContact()
        {
            Console.Clear();
            Console.WriteLine("Delete Contact");
            if (!Utility.ShowContacts())
            {
                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }
            Console.Write("Enter contact number to delete: ");

            while (true)
            {
                int index = Utility.ParseNumber() + 1;
                if (index > Contacts.GetCount() || index < 1)
                {
                    Console.Write("Invalid input. Please enter a valid number: ");
                }
                else
                {
                    int id = Contacts.DeleteContact(index);
                    Console.WriteLine($"Contact with ID: {id} deleted successfully!");
                    Console.WriteLine();
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    break;
                }
            }
        }
    }
}
