Imports System.Data.SQLite
Imports System.Collections.ObjectModel

Public Class CustomerListWindow

    Public Property Customers As ObservableCollection(Of Customer)

    Public Sub New()
        InitializeComponent()
        Customers = New ObservableCollection(Of Customer)
        CustomerListView.ItemsSource = Customers
        LoadCustomersFromDatabase()
    End Sub

    Private Sub LoadCustomersFromDatabase()
        Dim selectQuery As String = "SELECT id, first_name, last_name, email, phone_number, address, city, postal_code, country FROM customers;"
        Dim reader As SQLiteDataReader = DatabaseHelper.ExecuteReader(selectQuery)

        While reader.Read()
            Customers.Add(New Customer With {
                .ID = Convert.ToInt32(reader("id")),
                .FirstName = reader("first_name").ToString(),
                .LastName = reader("last_name").ToString(),
                .Email = reader("email").ToString(),
                .PhoneNumber = reader("phone_number").ToString(),
                .Address = reader("address").ToString(),
                .City = reader("city").ToString(),
                .PostalCode = reader("postal_code").ToString(),
                .Country = reader("country").ToString()
            })
        End While
    End Sub

    Private Sub AddCustomer(sender As Object, e As RoutedEventArgs)
        Dim addWindow As New AddCustomerWindow(Customers)
        addWindow.ShowDialog()
    End Sub

    Private Sub EditCustomer(sender As Object, e As RoutedEventArgs)
        Dim selectedCustomer As Customer = CType(CustomerListView.SelectedItem, Customer)
        If selectedCustomer IsNot Nothing Then
            Dim editWindow As New EditCustomerWindow(selectedCustomer)
            If editWindow.ShowDialog() = True Then
                Customers.Clear()
                LoadCustomersFromDatabase()
            End If
        Else
            MessageBox.Show("Bitte wählen Sie einen Eintrag aus, den Sie bearbeiten möchten.")
        End If
    End Sub

    Private Sub DeleteCustomer(sender As Object, e As RoutedEventArgs)
        Dim selectedCustomer As Customer = CType(CustomerListView.SelectedItem, Customer)
        If selectedCustomer IsNot Nothing Then
            Dim result As MessageBoxResult = MessageBox.Show("Möchten Sie diesen Eintrag wirklich löschen?", "Bestätigung", MessageBoxButton.YesNo)
            If result = MessageBoxResult.Yes Then
                Dim deleteQuery As String = $"DELETE FROM customers WHERE id = {selectedCustomer.ID};"
                DatabaseHelper.ExecuteNonQuery(deleteQuery)
                Customers.Remove(selectedCustomer)
            End If
        Else
            MessageBox.Show("Bitte wählen Sie einen Eintrag aus, den Sie löschen möchten.")
        End If
    End Sub

End Class

Public Class Customer
    Public Property ID As Integer
    Public Property FirstName As String
    Public Property LastName As String
    Public Property Email As String
    Public Property PhoneNumber As String
    Public Property Address As String
    Public Property City As String
    Public Property PostalCode As String
    Public Property Country As String
End Class