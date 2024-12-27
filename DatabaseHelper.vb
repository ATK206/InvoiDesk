Imports System.Data
Imports System.Data.SQLite
Imports System.IO

Module DatabaseHelper
    Public connectionString As String = $"Data Source = {Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database.db")};Version=3;"

    Public Function ExecuteReader(query As String) As SQLiteDataReader
        Dim connection As New SQLiteConnection(connectionString)
        connection.Open()
        Dim command As New SQLiteCommand(query, connection)
        Return command.ExecuteReader(CommandBehavior.CloseConnection)
    End Function

    Public Sub ExecuteNonQuery(query As String)
        Using connection As New SQLiteConnection(connectionString)
            connection.Open()
            Using command As New SQLiteCommand(query, connection)
                command.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Function ExecuteScalar(query As String) As Object
        Using connection As New SQLiteConnection($"Data Source = {Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database.db")};Version=3;")
            connection.Open()
            Using command As New SQLiteCommand(query, connection)
                Return command.ExecuteScalar()
            End Using
        End Using
    End Function
End Module