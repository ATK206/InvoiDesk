Public Class EditCustomerWindow
    Private Customer As Customer

    Public Sub New(customer As Customer)
        InitializeComponent()
        Me.Customer = customer

        FirstNameTextBox.Text = customer.FirstName
        LastNameTextBox.Text = customer.LastName
        EmailTextBox.Text = customer.Email
        PhoneNumberTextBox.Text = customer.PhoneNumber
        AddressTextBox.Text = customer.Address
        CityTextBox.Text = customer.City
        PostalCodeTextBox.Text = customer.PostalCode
        CountryTextBox.Text = customer.Country
    End Sub

    Private Sub SaveCustomer(sender As Object, e As RoutedEventArgs)
        Customer.FirstName = FirstNameTextBox.Text
        Customer.LastName = LastNameTextBox.Text
        Customer.Email = EmailTextBox.Text
        Customer.PhoneNumber = PhoneNumberTextBox.Text
        Customer.Address = AddressTextBox.Text
        Customer.City = CityTextBox.Text
        Customer.PostalCode = PostalCodeTextBox.Text
        Customer.Country = CountryTextBox.Text

        Dim updateQuery As String = $"UPDATE customers SET 
                                      first_name = '{Customer.FirstName}', 
                                      last_name = '{Customer.LastName}', 
                                      email = '{Customer.Email}', 
                                      phone_number = '{Customer.PhoneNumber}', 
                                      address = '{Customer.Address}', 
                                      city = '{Customer.City}', 
                                      postal_code = '{Customer.PostalCode}', 
                                      country = '{Customer.Country}' 
                                    WHERE id = {Customer.ID};"

        DatabaseHelper.ExecuteNonQuery(updateQuery)
        Me.DialogResult = True
        Me.Close()
    End Sub

    Private Sub Cancel(sender As Object, e As RoutedEventArgs)
        Me.DialogResult = False
        Me.Close()
    End Sub
End Class