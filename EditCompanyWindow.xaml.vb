Public Class EditCompanyWindow
    Public Sub New(name As String, email As String, phoneNumber As String,
                   address As String, city As String, postalCode As String,
                   country As String, iban As String, taxNumber As String,
                   managingDirector As String)
        InitializeComponent()

        NameTextBox.Text = name
        EmailTextBox.Text = email
        PhoneNumberTextBox.Text = phoneNumber
        AddressTextBox.Text = address
        CityTextBox.Text = city
        PostalCodeTextBox.Text = postalCode
        CountryTextBox.Text = country
        IbanTextBox.Text = iban
        TaxNumberTextBox.Text = taxNumber
        ManagingDirectorTextBox.Text = managingDirector
    End Sub

    Private Sub SaveCompany(sender As Object, e As RoutedEventArgs)
        If String.IsNullOrWhiteSpace(NameTextBox.Text) OrElse
           String.IsNullOrWhiteSpace(EmailTextBox.Text) OrElse
           String.IsNullOrWhiteSpace(PhoneNumberTextBox.Text) OrElse
           String.IsNullOrWhiteSpace(AddressTextBox.Text) OrElse
           String.IsNullOrWhiteSpace(CityTextBox.Text) OrElse
           String.IsNullOrWhiteSpace(PostalCodeTextBox.Text) OrElse
           String.IsNullOrWhiteSpace(CountryTextBox.Text) OrElse
           String.IsNullOrWhiteSpace(IbanTextBox.Text) OrElse
           String.IsNullOrWhiteSpace(TaxNumberTextBox.Text) OrElse
           String.IsNullOrWhiteSpace(ManagingDirectorTextBox.Text) Then

            MessageBox.Show("Bitte füllen Sie alle Felder aus.")
            Return
        End If

        Dim selectQuery As String = "SELECT COUNT(*) FROM company;"
        Dim recordCount As Integer = Convert.ToInt32(DatabaseHelper.ExecuteScalar(selectQuery))

        Dim sqlCommand As String

        If recordCount = 0 Then
            sqlCommand = $"INSERT INTO company (
                            name, 
                            email, 
                            phone_number,
                            address,
                            city,
                            postal_code,
                            country,
                            iban,
                            tax_number,
                            managing_director
                          ) VALUES (
                            '{NameTextBox.Text}',
                            '{EmailTextBox.Text}',
                            '{PhoneNumberTextBox.Text}',
                            '{AddressTextBox.Text}',
                            '{CityTextBox.Text}',
                            '{PostalCodeTextBox.Text}',
                            '{CountryTextBox.Text}',
                            '{IbanTextBox.Text}',
                            '{TaxNumberTextBox.Text}',
                            '{ManagingDirectorTextBox.Text}'
                          );"
        Else
            sqlCommand = $"UPDATE company SET
                            name = '{NameTextBox.Text}',
                            email = '{EmailTextBox.Text}',
                            phone_number = '{PhoneNumberTextBox.Text}',
                            address = '{AddressTextBox.Text}',
                            city = '{CityTextBox.Text}',
                            postal_code = '{PostalCodeTextBox.Text}',
                            country = '{CountryTextBox.Text}',
                            iban = '{IbanTextBox.Text}',
                            tax_number = '{TaxNumberTextBox.Text}',
                            managing_director = '{ManagingDirectorTextBox.Text}';"
        End If

        DatabaseHelper.ExecuteNonQuery(sqlCommand)
        Me.DialogResult = True
        Me.Close()
    End Sub

    Private Sub Cancel(sender As Object, e As RoutedEventArgs)
        Me.DialogResult = False
        Me.Close()
    End Sub
End Class