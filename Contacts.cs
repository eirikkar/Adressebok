using Microsoft.Data.Sqlite;

public class Contacts
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int PhoneNumber { get; set; }
    public string? Email { get; set; }

    public static SqliteConnection InitDatabase()
    {
        var db = new SqliteConnection("Data Source=db.sqlite");
        db.Open();
        using (var cmd = db.CreateCommand())
        {
            cmd.CommandText =
                @"CREATE TABLE IF NOT EXISTS Contacts (
                                id INTEGER PRIMARY KEY,
                                name TEXT NOT NULL,
                                phoneNumber INTEGER NOT NULL,
                                email TEXT NOT NULL
                            );";
            cmd.ExecuteNonQuery();
        }
        return db;
    }

    public static void CreateTable()
    {
        using SqliteConnection sqliteCon = InitDatabase();
        sqliteCon.Open();
        SqliteCommand sqliteCommand;
        string createSQL =
            "CREATE TABLE IF NOT EXISTS Contacts (Id INTEGER PRIMARY KEY, Name TEXT NOT NULL, PhoneNumber INTEGER NOT NULL, Email TEXT NOT NULL)";
        sqliteCommand = sqliteCon.CreateCommand();
        sqliteCommand.CommandText = createSQL;
        sqliteCommand.ExecuteNonQuery();
    }

    public static int AddContact(string name, int phoneNumber, string email)
    {
        using SqliteConnection sqliteCon = InitDatabase();
        sqliteCon.Open();
        using var sqliteCommand = sqliteCon.CreateCommand();
        sqliteCommand.CommandText =
            "INSERT INTO Contacts (Name, PhoneNumber, Email) VALUES (@Name, @PhoneNumber, @Email)";
        sqliteCommand.Parameters.AddWithValue("@Name", name);
        sqliteCommand.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
        sqliteCommand.Parameters.AddWithValue("@Email", email);
        sqliteCommand.ExecuteNonQuery();

        sqliteCommand.CommandText = "SELECT last_insert_rowid()";
        int id = Convert.ToInt32(sqliteCommand.ExecuteScalar());
        return id;
    }

    public static int EditContact(int id, string name, int phoneNumber, string email)
    {
        using SqliteConnection sqliteCon = InitDatabase();
        sqliteCon.Open();
        using var sqliteCommand = sqliteCon.CreateCommand();
        sqliteCommand.CommandText =
            "UPDATE Contacts SET Name = @Name, PhoneNumber = @PhoneNumber, Email = @Email WHERE Id = @Id";
        sqliteCommand.Parameters.AddWithValue("@Id", id);
        sqliteCommand.Parameters.AddWithValue("@Name", name);
        sqliteCommand.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
        sqliteCommand.Parameters.AddWithValue("@Email", email);
        sqliteCommand.ExecuteNonQuery();
        return id;
    }

    public static int DeleteContact(int id)
    {
        using SqliteConnection sqliteCon = InitDatabase();
        sqliteCon.Open();

        using (var sqliteCommand = sqliteCon.CreateCommand())
        {
            sqliteCommand.CommandText = "DELETE FROM Contacts WHERE Id = @Id";
            sqliteCommand.Parameters.AddWithValue("@Id", id);
            sqliteCommand.ExecuteNonQuery();
        }
        ReassignIds();
        return id;
    }

    public static int GetCount()
    {
        using SqliteConnection sqliteCon = InitDatabase();
        sqliteCon.Open();
        using var sqliteCommand = sqliteCon.CreateCommand();
        sqliteCommand.CommandText = "SELECT COUNT(*) FROM Contacts";
        int count = Convert.ToInt32(sqliteCommand.ExecuteScalar());
        return count;
    }

    public static void ReassignIds()
    {
        using var db = InitDatabase();
        db.Open();
        using var transaction = db.BeginTransaction();
        var cmd = new SqliteCommand("SELECT id FROM Contacts ORDER BY id", db, transaction);
        var reader = cmd.ExecuteReader();
        int newId = 1;

        while (reader.Read())
        {
            int oldId = reader.GetInt32(0);
            var updateCmd = new SqliteCommand(
                "UPDATE Contacts SET id = @newId WHERE id = @oldId",
                db,
                transaction
            );
            updateCmd.Parameters.AddWithValue("@newId", newId);
            updateCmd.Parameters.AddWithValue("@oldId", oldId);
            updateCmd.ExecuteNonQuery();
            newId++;
        }

        transaction.Commit();
    }
}
