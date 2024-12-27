Imports System.Data.SQLite

Public Class CompanyWindow
    Public Sub New()
        InitializeComponent()
        LoadCompanyData()
    End Sub

    Private Sub LoadCompanyData()
        Dim query As String = "SELECT name, email, phone_number, address, city, postal_code, country, iban, tax_number, managing_director FROM company;"
        Dim reader As SQLiteDataReader = DatabaseHelper.ExecuteReader(query)

        If reader.Read() Then
            NameTextBox.Text = reader("name").ToString()
            EmailTextBox.Text = reader("email").ToString()
            PhoneNumberTextBox.Text = reader("phone_number").ToString()
            AddressTextBox.Text = reader("address").ToString()
            CityTextBox.Text = reader("city").ToString()
            PostalCodeTextBox.Text = reader("postal_code").ToString()
            CountryTextBox.Text = reader("country").ToString()
            IbanTextBox.Text = reader("iban").ToString()
            TaxNumberTextBox.Text = reader("tax_number").ToString()
            ManagingDirectorTextBox.Text = reader("managing_director").ToString()
        End If
    End Sub

    Private Sub EditCompany(sender As Object, e As RoutedEventArgs)
        Dim editWindow As New EditCompanyWindow(
            NameTextBox.Text, EmailTextBox.Text, PhoneNumberTextBox.Text,
            AddressTextBox.Text, CityTextBox.Text, PostalCodeTextBox.Text,
            CountryTextBox.Text, IbanTextBox.Text, TaxNumberTextBox.Text,
            ManagingDirectorTextBox.Text
        )

        If editWindow.ShowDialog() = True Then
            LoadCompanyData()
        End If
    End Sub
End Class