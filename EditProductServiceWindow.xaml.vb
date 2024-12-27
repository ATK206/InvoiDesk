Imports System.Globalization

Public Class EditProductServiceWindow
    Private ProductService As ProductService

    Public Sub New(productService As ProductService)
        InitializeComponent()
        Me.ProductService = productService

        DesignationTextBox.Text = productService.Designation
        DescriptionTextBox.Text = productService.Description
        UnitPriceTextBox.Text = productService.UnitPrice.ToString("N2", CultureInfo.GetCultureInfo("de-DE"))
        CategoryTextBox.Text = productService.Category
    End Sub

    Private Sub SaveProductService(sender As Object, e As RoutedEventArgs)
        ProductService.Designation = DesignationTextBox.Text
        ProductService.Description = DescriptionTextBox.Text

        Dim unitPrice As Decimal
        If Decimal.TryParse(UnitPriceTextBox.Text, NumberStyles.Any, CultureInfo.GetCultureInfo("de-DE"), unitPrice) Then
            ProductService.UnitPrice = unitPrice
        Else
            MessageBox.Show("Ungültiges Format für den Stückpreis.")
            Return
        End If

        ProductService.Category = CategoryTextBox.Text

        Dim updateQuery As String = $"UPDATE products_services SET 
                                      designation = '{ProductService.Designation}', 
                                      description = '{ProductService.Description}', 
                                      unit_price = {ProductService.UnitPrice.ToString(CultureInfo.InvariantCulture)}, 
                                      category = '{ProductService.Category}'
                                    WHERE id = {ProductService.ID};"

        DatabaseHelper.ExecuteNonQuery(updateQuery)
        Me.DialogResult = True
        Me.Close()
    End Sub

    Private Sub Cancel(sender As Object, e As RoutedEventArgs)
        Me.DialogResult = False
        Me.Close()
    End Sub
End Class