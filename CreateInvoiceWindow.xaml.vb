Imports System.Data.SQLite
Imports System.Windows.Controls
Imports System.IO
Imports iText.Kernel.Pdf
Imports iText.Layout
Imports iText.Layout.Element
Imports iText.Layout.Properties
Imports iText.Kernel.Pdf.Canvas

Public Class CreateInvoiceWindow
    Public Class Company
        Public Property Name As String
        Public Property Email As String
        Public Property PhoneNumber As String
        Public Property Address As String
        Public Property City As String
        Public Property PostalCode As String
        Public Property Country As String
        Public Property IBAN As String
        Public Property TaxNumber As String
        Public Property ManagingDirector As String
    End Class

    Public Class Customer
        Public Property ID As Integer
        Public Property Name As String
        Public Property Address As String
        Public Property City As String
        Public Property PostalCode As String
        Public Property Country As String
    End Class
    Public Class ProductService
        Public Property ID As Integer
        Public Property Name As String
        Public Property UnitPrice As Decimal
    End Class

    Private Function GetCompanyData() As Company
        Dim query As String = "SELECT name, email, phone_number, address, city, postal_code, country, iban, tax_number, managing_director FROM company;"
        Dim reader As SQLiteDataReader = DatabaseHelper.ExecuteReader(query)

        If reader.Read() Then
            Return New Company With {
                .Name = reader("name").ToString(),
                .Email = reader("email").ToString(),
                .PhoneNumber = reader("phone_number").ToString(),
                .Address = reader("address").ToString(),
                .City = reader("city").ToString(),
                .PostalCode = reader("postal_code").ToString(),
                .Country = reader("country").ToString(),
                .IBAN = reader("iban").ToString(),
                .TaxNumber = reader("tax_number").ToString(),
                .ManagingDirector = reader("managing_director").ToString()
            }
        End If

        Return Nothing
    End Function

    Private Function GetCustomers() As List(Of Customer)
        Dim customers As New List(Of Customer)
        Dim query As String = "SELECT id, first_name, last_name, address, city, postal_code, country FROM customers;"

        Dim reader As SQLiteDataReader = DatabaseHelper.ExecuteReader(query)
        While reader.Read()
            Dim customer = New Customer With {
            .ID = Convert.ToInt32(reader("id")),
            .Name = $"{reader("first_name")} {reader("last_name")}".Trim(),
            .Address = reader("address").ToString(),
            .City = reader("city").ToString(),
            .PostalCode = reader("postal_code").ToString(),
            .Country = reader("country").ToString()}
            customers.Add(customer)
        End While

        Return customers
    End Function

    Private Sub InitializeWindow(sender As Object, e As RoutedEventArgs)
        Dim customers As List(Of Customer) = GetCustomers()
        CustomerComboBox.ItemsSource = customers
    End Sub

    Private Function GetCustomerDataById(customerId As Integer) As Customer
        Dim query As String = $"SELECT id, first_name, last_name, address, city, postal_code, country FROM customers WHERE id = {customerId};"
        Dim reader As SQLiteDataReader = DatabaseHelper.ExecuteReader(query)

        If reader.Read() Then
            Return New Customer With {
            .ID = Convert.ToInt32(reader("id")),
            .Name = $"{reader("first_name")} {reader("last_name")}".Trim(),
            .Address = reader("address").ToString(),
            .City = reader("city").ToString(),
            .PostalCode = reader("postal_code").ToString(),
            .Country = reader("country").ToString()}
        End If

        Return Nothing
    End Function

    Private Function GetProductsServices() As List(Of ProductService)
        Dim products As New List(Of ProductService)
        Dim query As String = "SELECT id, designation, unit_price FROM products_services;"

        Dim reader As SQLiteDataReader = DatabaseHelper.ExecuteReader(query)
        While reader.Read()
            Dim product = New ProductService With {
            .ID = Convert.ToInt32(reader("id")),
            .Name = reader("designation").ToString(),
            .UnitPrice = Convert.ToDecimal(reader("unit_price"))}
            products.Add(product)
        End While

        Return products
    End Function

    Private Sub AddProductService(sender As Object, e As RoutedEventArgs)
        Dim rowGrid As New Grid() With {.Margin = New Thickness(0, 5, 0, 5)}

        rowGrid.ColumnDefinitions.Add(New ColumnDefinition() With {.Width = New GridLength(0.8, GridUnitType.Star)})
        rowGrid.ColumnDefinitions.Add(New ColumnDefinition() With {.Width = New GridLength(0.2, GridUnitType.Star)})

        Dim productComboBox As New ComboBox() With {
        .DisplayMemberPath = "Name",
        .SelectedValuePath = "ID",
        .ItemsSource = GetProductsServices(),
        .Margin = New Thickness(0, 0, 10, 0)}

        Grid.SetColumn(productComboBox, 0)
        rowGrid.Children.Add(productComboBox)

        Dim quantityTextBox As New TextBox() With {
        .Text = "1",
        .Width = 50,
        .HorizontalAlignment = HorizontalAlignment.Center}

        Grid.SetColumn(quantityTextBox, 1)
        rowGrid.Children.Add(quantityTextBox)

        ProductStackPanel.Children.Add(rowGrid)
    End Sub

    Private Function GetSelectedProductsServices() As List(Of (ProductService, Integer))
        Dim selectedProductsServices As New List(Of (ProductService, Integer))

        For Each child As UIElement In ProductStackPanel.Children
            If TypeOf child Is Grid Then
                Dim rowGrid = CType(child, Grid)
                Dim productComboBox = CType(rowGrid.Children(0), ComboBox)
                Dim selectedProductService As ProductService = CType(productComboBox.SelectedItem, ProductService)
                Dim quantityTextBox = CType(rowGrid.Children(1), TextBox)
                Dim quantity As Integer

                If selectedProductService IsNot Nothing AndAlso Integer.TryParse(quantityTextBox.Text, quantity) AndAlso quantity > 0 Then
                    selectedProductsServices.Add((selectedProductService, quantity))
                End If
            End If
        Next

        Return selectedProductsServices
    End Function

    Private Function ValidateInvoiceInput(companyData As Company, selectedCustomerId As Integer, deliveryDate As Date?, selectedProductsServices As List(Of (ProductService, Integer))) As String
        If companyData Is Nothing Then Return "Keine Unternehmensdaten vorhanden."
        If selectedCustomerId = 0 Then Return "Bitte wählen Sie einen Kunden aus."
        If deliveryDate Is Nothing Then Return "Bitte wählen Sie ein Lieferdatum aus."
        If selectedProductsServices Is Nothing OrElse selectedProductsServices.Count = 0 Then Return "Bitte wählen Sie mindestens ein Produkt oder eine Dienstleistung aus."
        Return Nothing
    End Function

    Private Function GetNextInvoiceNumber(currentYear As Integer) As Integer
        Dim query As String = "SELECT MAX(CAST(SUBSTR(invoice_number, INSTR(invoice_number, '-') + 1) AS INTEGER)) " &
                      "FROM invoices WHERE SUBSTR(invoice_number, 1, 4) = @year"
        Dim nextInvoiceNumber As Integer = 1

        Using connection As New SQLiteConnection($"Data Source={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database.db")};Version=3;")
            connection.Open()
            Using command As New SQLiteCommand(query, connection)
                command.Parameters.AddWithValue("@year", currentYear.ToString())
                Dim result = command.ExecuteScalar()

                If result IsNot DBNull.Value Then
                    nextInvoiceNumber = Convert.ToInt32(result) + 1
                End If
            End Using
        End Using

        Return nextInvoiceNumber
    End Function

    Private Sub SaveInvoiceToDatabase(invoiceNumberString As String, pdfData As Byte())
        Dim query As String = "INSERT INTO invoices (invoice_number, created_at, pdf_data) VALUES (@invoiceNumber, @createdAt, @pdfData)"
        Using connection As New SQLiteConnection($"Data Source={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database.db")};Version=3;Pooling=True;Max Pool Size=100;Journal Mode=WAL;")
            connection.Open()
            Using command As New SQLiteCommand(query, connection)
                command.Parameters.AddWithValue("@invoiceNumber", invoiceNumberString)
                command.Parameters.AddWithValue("@createdAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                command.Parameters.AddWithValue("@pdfData", pdfData)
                command.ExecuteNonQuery()
            End Using
        End Using
    End Sub


    Private Sub CreateInvoice(sender As Object, e As RoutedEventArgs)
        Dim companyData As Company = GetCompanyData()
        Dim selectedCustomerId As Integer = Convert.ToInt32(CustomerComboBox.SelectedValue)
        Dim customerData As Customer = GetCustomerDataById(selectedCustomerId)
        Dim deliveryDate As Date? = DeliveryDatePicker.SelectedDate
        Dim selectedProductsServices = GetSelectedProductsServices()
        Dim errorMessage = ValidateInvoiceInput(companyData, selectedCustomerId, deliveryDate, selectedProductsServices)

        If errorMessage IsNot Nothing Then
            MessageBox.Show(errorMessage)
            Return
        End If

        Dim currentYear As Integer = DateTime.Now.Year
        Dim invoiceNumberInt As Integer = GetNextInvoiceNumber(currentYear)
        Dim invoiceNumberString As String = $"{currentYear}-{invoiceNumberInt:D3}"

        Dim pdfData As Byte()
        Using memoryStream As New System.IO.MemoryStream()
            Using writer As New iText.Kernel.Pdf.PdfWriter(memoryStream)
                Using pdf As New iText.Kernel.Pdf.PdfDocument(writer)
                    Dim document As New iText.Layout.Document(pdf)
                    AddInvoiceHeader(document, companyData, customerData, deliveryDate.Value, invoiceNumberString)
                    AddInvoiceItems(document, selectedProductsServices)
                    AddInvoiceComment(document, companyData)
                    AddInvoiceFooter(pdf, companyData)
                    document.Close()
                End Using
            End Using

            pdfData = memoryStream.ToArray()
        End Using

        Dim fileName As String = $"Rechnung {invoiceNumberString}.pdf"
        Dim saveDialog As New Microsoft.Win32.SaveFileDialog() With {
        .Filter = "PDF Datei (*.pdf)|*.pdf",
        .FileName = fileName}

        If saveDialog.ShowDialog() = True Then
            Dim filePath As String = saveDialog.FileName
            System.IO.File.WriteAllBytes(filePath, pdfData)
        End If

        SaveInvoiceToDatabase(invoiceNumberString, pdfData)
    End Sub

    Private Sub AddInvoiceHeader(document As Document, company As Company, customer As Customer, deliveryDate As Date, invoiceNumberString As String)
        document.Add(New Paragraph($"{company.Name} - {company.Address} - {company.PostalCode} {company.City}").
                 SetFontSize(8).
                 SetMarginBottom(20))
        document.Add(New Paragraph($"{customer.Name}").
                 SetFontSize(10).
                 SetFixedLeading(6))
        document.Add(New Paragraph(customer.Address).
                 SetFontSize(10).
                 SetFixedLeading(6))
        document.Add(New Paragraph($"{customer.PostalCode} {customer.City}").
                 SetFontSize(10).
                 SetFixedLeading(6))
        document.Add(New Paragraph(customer.Country).
                 SetFontSize(10).
                 SetFixedLeading(6))
        document.Add(New Paragraph($"Rechnungsdatum: {DateTime.Now.ToString("dd.MM.yyyy")}").
                 SetFontSize(8).
                 SetFixedLeading(4).
                 SetTextAlignment(TextAlignment.RIGHT))
        document.Add(New Paragraph($"Lieferdatum: {deliveryDate:dd.MM.yyyy}").
                 SetFontSize(8).
                 SetFixedLeading(4).
                 SetTextAlignment(TextAlignment.RIGHT))
        document.Add(New Paragraph($"Kundennummer: {customer.ID}").
                 SetFontSize(8).
                 SetFixedLeading(4).
                 SetTextAlignment(TextAlignment.RIGHT).
                 SetMarginBottom(10))
        document.Add(New Paragraph($"Rechnungsnummer: {invoiceNumberString}").
                 SetFontSize(12).
                 SetBold().
                 SetMarginBottom(20))
        document.Add(New Paragraph($"Sehr geehrte Damen und Herren,").
                 SetFontSize(10))
        document.Add(New Paragraph($"vielen Dank für Ihr Vertrauen in die {company.Name}. Wir stellen Ihnen hiermit folgende Leistungen in Rechnung:").
                 SetFontSize(10).
                 SetTextAlignment(TextAlignment.LEFT).
                 SetMarginBottom(20))
    End Sub

    Private Sub AddInvoiceItems(document As Document, selectedProducts As List(Of (ProductService, Integer)))
        Dim table As New Table(UnitValue.CreatePercentArray({50, 10, 20, 20}))
        table.UseAllAvailableWidth()
        table.SetMarginBottom(20)
        table.AddHeaderCell(New Cell().Add(New Paragraph("Beschreibung").
              SetFontSize(10).
              SetBold().
              SetTextAlignment(TextAlignment.LEFT)))
        table.AddHeaderCell(New Cell().Add(New Paragraph("Menge").
              SetFontSize(10).
              SetBold().
              SetTextAlignment(TextAlignment.CENTER)))
        table.AddHeaderCell(New Cell().Add(New Paragraph("Einzelpreis").
              SetFontSize(10).
              SetBold().
              SetTextAlignment(TextAlignment.RIGHT)))
        table.AddHeaderCell(New Cell().Add(New Paragraph("Gesamtpreis").
              SetFontSize(10).
              SetBold().
              SetTextAlignment(TextAlignment.RIGHT)))

        For Each position In selectedProducts
            Dim total = position.Item1.UnitPrice * position.Item2

            table.AddCell(New Cell().Add(New Paragraph(position.Item1.Name)).
                  SetFontSize(10).
                  SetTextAlignment(TextAlignment.LEFT))
            table.AddCell(New Cell().Add(New Paragraph(position.Item2.ToString())).
                  SetFontSize(10).
                  SetTextAlignment(TextAlignment.CENTER))
            table.AddCell(New Cell().Add(New Paragraph($"{position.Item1.UnitPrice:C}")).
                  SetFontSize(10).
                  SetTextAlignment(TextAlignment.RIGHT))
            table.AddCell(New Cell().Add(New Paragraph($"{total:C}")).
                  SetFontSize(10).
                  SetTextAlignment(TextAlignment.RIGHT))
        Next

        document.Add(table)

        Dim totalAmount As Decimal = selectedProducts.Sum(Function(p) p.Item1.UnitPrice * p.Item2)
        Dim taxRate As Decimal = 0.19D
        Dim taxAmount As Decimal = totalAmount * taxRate
        Dim totalWithTax As Decimal = totalAmount + taxAmount

        document.Add(New Paragraph($"Summe Netto: {totalAmount:C}").
                 SetFontSize(10).
                 SetTextAlignment(TextAlignment.RIGHT))
        document.Add(New Paragraph($"MwSt (19 %): {taxAmount:C}").
                 SetFontSize(10).
                 SetTextAlignment(TextAlignment.RIGHT))
        document.Add(New Paragraph($"Gesamtsumme: {totalWithTax:C}").
                 SetFontSize(12).
                 SetBold().
                 SetTextAlignment(TextAlignment.RIGHT))
    End Sub

    Private Sub AddInvoiceComment(document As Document, company As Company)
        document.Add(New Paragraph($"Bitte überweisen Sie den Betrag bis zum {DateTime.Now.AddDays(14):dd.MM.yyyy} an die unten angegebene Bankverbindung.").
                 SetFontSize(10).
                 SetMarginTop(20))
    End Sub

    Private Sub AddInvoiceFooter(pdf As PdfDocument, company As Company)
        Dim totalPages = pdf.GetNumberOfPages()
        Dim font As iText.Kernel.Font.PdfFont = iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA)

        Const FONT_SIZE = 8
        Const LINE_HEIGHT = 10
        Const Y_START = 80
        Const X_ADDRESS = 40
        Const X_CONTACT = 240
        Const X_IBAN = 430

        For i As Integer = 1 To totalPages
            Dim page = pdf.GetPage(i)
            Dim pdfCanvas As New PdfCanvas(page)
            Dim pageSize = page.GetPageSize()

            pdfCanvas.BeginText()
            pdfCanvas.SetFontAndSize(font, FONT_SIZE)

            TextBlock(pdfCanvas, X_ADDRESS, Y_START, {
            company.Name,
            company.Address,
            $"{company.PostalCode} {company.City}",
            company.Country}, LINE_HEIGHT)

            TextBlock(pdfCanvas, X_CONTACT, Y_START, {
            $"Telefon: {company.PhoneNumber}",
            $"E-Mail: {company.Email}",
            $"IBAN: {company.IBAN}"}, LINE_HEIGHT)

            TextBlock(pdfCanvas, X_IBAN, Y_START, {
            $"Steuernummer: {company.TaxNumber}",
            $"Geschäftsführer: {company.ManagingDirector}"}, LINE_HEIGHT)

            Dim pageNumberText = $"Seite {i} von {totalPages}"
            Dim textWidth = font.GetWidth(pageNumberText, FONT_SIZE)
            Dim xPageNumber = (pageSize.GetWidth() - textWidth) / 2
            pdfCanvas.SetTextMatrix(xPageNumber, Y_START - LINE_HEIGHT * 5)
            pdfCanvas.ShowText(pageNumberText)

            pdfCanvas.EndText()
        Next
    End Sub

    Private Sub TextBlock(canvas As PdfCanvas, x As Single, y As Single, lines As String(), lineHeight As Single)
        For i As Integer = 0 To lines.Length - 1
            canvas.SetTextMatrix(x, y - i * lineHeight)
            canvas.ShowText(lines(i))
        Next
    End Sub

    Private Sub GeneratePDF(filePath As String, company As Company, customer As Customer, selectedProducts As List(Of (ProductService, Integer)), deliveryDate As Date, invoiceNumberString As String)
        Using writer As New PdfWriter(filePath)
            Using pdf As New PdfDocument(writer)
                Dim document As New Document(pdf)

                AddInvoiceHeader(document, company, customer, deliveryDate, invoiceNumberString)
                AddInvoiceItems(document, selectedProducts)
                AddInvoiceComment(document, company)
                AddInvoiceFooter(pdf, company)

                document.Close()
            End Using
        End Using
    End Sub
End Class