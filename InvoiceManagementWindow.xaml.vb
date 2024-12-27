Imports System.Data.SQLite
Imports System.IO

Class InvoiceManagementWindow
    Private Sub InitializeWindow(sender As Object, e As RoutedEventArgs)
        LoadInvoices()
    End Sub

    Private Sub LoadInvoices()
        Dim query As String = "SELECT id, invoice_number FROM invoices"
        Dim invoiceList As New List(Of Invoice)()

        Using connection As New SQLiteConnection($"Data Source = {Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database.db")};Version=3;")
            connection.Open()
            Using command As New SQLiteCommand(query, connection)
                Using reader As SQLiteDataReader = command.ExecuteReader()
                    While reader.Read()
                        Dim invoiceId As Integer = reader.GetInt32(0)
                        Dim invoiceNumber As String = reader.GetString(1)

                        invoiceList.Add(New Invoice With {
                            .InvoiceId = invoiceId,
                            .InvoiceNumber = invoiceNumber
                        })
                    End While
                End Using
            End Using
        End Using

        InvoiceDataGrid.ItemsSource = invoiceList
    End Sub
End Class

Public Class Invoice
    Public Property InvoiceId As Integer
    Public Property InvoiceNumber As String
    Public Property DownloadCommand As ICommand = New RelayCommand(AddressOf DownloadInvoice)

    Private Sub DownloadInvoice(parameter As Object)
        Dim invoiceId = CType(parameter, Invoice).InvoiceId
        Dim query As String = "SELECT pdf_data, invoice_number FROM invoices WHERE id = @invoiceId"
        Dim pdfData As Byte() = Nothing
        Dim invoiceNumber As String = ""

        Using connection As New SQLiteConnection($"Data Source = {Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database.db")};Version=3;")
            connection.Open()
            Using command As New SQLiteCommand(query, connection)
                command.Parameters.AddWithValue("@invoiceId", invoiceId)
                Using reader As SQLiteDataReader = command.ExecuteReader()
                    If reader.Read() Then
                        pdfData = CType(reader("pdf_data"), Byte())
                        invoiceNumber = reader("invoice_number").ToString()
                    End If
                End Using
            End Using
        End Using

        If pdfData IsNot Nothing Then
            Dim saveDialog As New Microsoft.Win32.SaveFileDialog() With {
            .Filter = "PDF Datei (*.pdf)|*.pdf",
            .FileName = $"Rechnung {invoiceNumber}.pdf"}

            If saveDialog.ShowDialog() = True Then
                Dim savePath As String = saveDialog.FileName
                System.IO.File.WriteAllBytes(savePath, pdfData)
            End If
        End If
    End Sub
End Class