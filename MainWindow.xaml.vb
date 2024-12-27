Imports System.Data.SQLite


Class MainWindow
    Private Sub DatabaseTest(sender As Object, e As RoutedEventArgs)
        Dim query As String = "SELECT * FROM customers;"
        Dim reader As SQLiteDataReader = DatabaseHelper.ExecuteReader(query)

        While reader.Read()
            MessageBox.Show(reader("first_name").ToString())
        End While
    End Sub

    Private Sub OpenCustomerListWindow(sender As Object, e As RoutedEventArgs)
        Dim customerListWindow As New CustomerListWindow()
        customerListWindow.Show()
    End Sub

    Private Sub OpenProductServiceListWindow(sender As Object, e As RoutedEventArgs)
        Dim productServiceListWindow As New ProductServiceListWindow()
        productServiceListWindow.Show()
    End Sub

    Private Sub OpenCompanyWindow(sender As Object, e As RoutedEventArgs)
        Dim companyWindow As New CompanyWindow()
        companyWindow.Show()
    End Sub

    Private Sub OpenCreateInvoiceWindow(sender As Object, e As RoutedEventArgs)
        Dim createInvoiceWindow As New CreateInvoiceWindow()
        createInvoiceWindow.Show()
    End Sub

    Private Sub OpenInvoiceManagementWindow(sender As Object, e As RoutedEventArgs)
        Dim invoiceManagementWindow As New InvoiceManagementWindow()
        invoiceManagementWindow.Show()
    End Sub
End Class