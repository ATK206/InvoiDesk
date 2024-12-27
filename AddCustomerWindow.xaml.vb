Imports System.Collections.ObjectModel
Imports System.Data.SQLite
Imports System.IO

Public Class AddCustomerWindow
    Private Customers As ObservableCollection(Of Customer)

    Public Sub New(customers As ObservableCollection(Of Customer))
        InitializeComponent()
        Me.Customers = customers
    End Sub

    Private Sub InsertCustomer(sender As Object, e As RoutedEventArgs)
        Dim firstName As String = FirstNameTextBox.Text
        Dim lastName As String = LastNameTextBox.Text
        Dim email As String = EmailTextBox.Text
        Dim phoneNumber As String = PhoneNumberTextBox.Text
        Dim address As String = AddressTextBox.Text
        Dim city As String = CityTextBox.Text
        Dim postalCode As String = PostalCodeTextBox.Text
        Dim country As String = CountryTextBox.Text

        If String.IsNullOrWhiteSpace(firstName) OrElse
           String.IsNullOrWhiteSpace(lastName) OrElse
           String.IsNullOrWhiteSpace(email) OrElse
           String.IsNullOrWhiteSpace(phoneNumber) OrElse
           String.IsNullOrWhiteSpace(address) OrElse
           String.IsNullOrWhiteSpace(city) OrElse
           String.IsNullOrWhiteSpace(postalCode) OrElse
           String.IsNullOrWhiteSpace(country) Then
            MessageBox.Show("Bitte füllen Sie alle Felder aus.")
            Return
        End If

        Using connection As New SQLiteConnection($"Data Source = {Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database.db")};Version=3;")
            connection.Open()

            Dim insertQuery As String = $"INSERT INTO customers (
                                          first_name, 
                                          last_name, 
                                          email, 
                                          phone_number, 
                                          address, 
                                          city, 
                                          postal_code, 
                                          country
                                        ) VALUES (
                                          '{firstName}',
                                          '{lastName}',
                                          '{email}',
                                          '{phoneNumber}',
                                          '{address}',
                                          '{city}',
                                          '{postalCode}',
                                          '{country}'
                                        );"

            Using command As New SQLiteCommand(insertQuery, connection)
                command.ExecuteNonQuery()
            End Using

            Dim idQuery As String = "SELECT last_insert_rowid();"
            Dim newId As Integer

            Using command As New SQLiteCommand(idQuery, connection)
                newId = Convert.ToInt32(command.ExecuteScalar())
            End Using

            Dim newCustomer As New Customer With {
                .ID = newId,
                .FirstName = firstName,
                .LastName = lastName,
                .Email = email,
                .PhoneNumber = phoneNumber,
                .Address = address,
                .City = city,
                .PostalCode = postalCode,
                .Country = country
            }

            Customers.Add(newCustomer)
        End Using

        MessageBox.Show($"Kunde ""{firstName} {lastName}"" wurde erfolgreich hinzugefügt.")

        Me.Close()
    End Sub

    Private Sub Cancel(sender As Object, e As RoutedEventArgs)
        Me.Close()
    End Sub
End Class