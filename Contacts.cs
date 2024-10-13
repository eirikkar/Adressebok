using Microsoft.Data.Sqlite;

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

    public static int AddContact(SqliteConnection conn, string name, int phoneNumber, string email)
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
            "UPDATE Contacts SET Name = @Name, PhoneNumber = @PhoneNumber, Email = @Email WHERE Id = @Id";
        sqliteCommand.Parameters.AddWithValue("@Id", id);
        sqliteCommand.Parameters.AddWithValue("@Name", name);
        sqliteCommand.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
        sqliteCommand.Parameters.AddWithValue("@Email", email);
        sqliteCommand.ExecuteNonQuery();
        return id;
    }

    public static int DeleteContact(SqliteConnection conn, int id)
    {
        SqliteCommand sqliteCommand;
        sqliteCommand = conn.CreateCommand();
        sqliteCommand.CommandText = "DELETE FROM Contacts WHERE Id = @Id";
        sqliteCommand.Parameters.AddWithValue("@Id", id);
        sqliteCommand.ExecuteNonQuery();
        conn.Close();
        return id;
    }
}
