Imports System.Data.SQLite
Imports System.Collections.ObjectModel
Imports System.Globalization

Public Class ProductServiceListWindow
    Public Property ProductServices As ObservableCollection(Of ProductService)

    Public Sub New()
        InitializeComponent()
        ProductServices = New ObservableCollection(Of ProductService)
        ProductServiceListView.ItemsSource = ProductServices
        LoadProductServicesFromDatabase()
    End Sub

    Private Sub LoadProductServicesFromDatabase()
        Dim selectQuery As String = "SELECT id, designation, description, unit_price, category FROM products_services;"
        Dim reader As SQLiteDataReader = DatabaseHelper.ExecuteReader(selectQuery)

        While reader.Read()
            ProductServices.Add(New ProductService With {
                .ID = Convert.ToInt32(reader("id")),
                .Designation = reader("designation").ToString(),
                .Description = reader("description").ToString(),
                .UnitPrice = Convert.ToDecimal(reader("unit_price"), CultureInfo.GetCultureInfo("de-DE")),
                .Category = reader("category").ToString()
            })
        End While
    End Sub

    Private Sub AddProductService(sender As Object, e As RoutedEventArgs)
        Dim addProductServiceWindow As New AddProductsServicesWindow(ProductServices)
        addProductServiceWindow.Show()
    End Sub

    Private Sub EditProductService(sender As Object, e As RoutedEventArgs)
        Dim selectedProductService As ProductService = CType(ProductServiceListView.SelectedItem, ProductService)
        If selectedProductService IsNot Nothing Then
            Dim editWindow As New EditProductServiceWindow(selectedProductService)
            If editWindow.ShowDialog() = True Then
                ProductServices.Clear()
                LoadProductServicesFromDatabase()
            End If
        Else
            MessageBox.Show("Bitte wählen Sie einen Eintrag aus, den Sie bearbeiten möchten.")
        End If
    End Sub

    Private Sub DeleteProductService(sender As Object, e As RoutedEventArgs)
        Dim selectedProductService As ProductService = CType(ProductServiceListView.SelectedItem, ProductService)
        If selectedProductService IsNot Nothing Then
            Dim result As MessageBoxResult = MessageBox.Show("Möchten Sie diesen Eintrag wirklich löschen?", "Bestätigung", MessageBoxButton.YesNo)
            If result = MessageBoxResult.Yes Then
                Dim deleteQuery As String = $"DELETE FROM products_services WHERE id = {selectedProductService.ID};"
                DatabaseHelper.ExecuteNonQuery(deleteQuery)
                ProductServices.Remove(selectedProductService)
            End If
        Else
            MessageBox.Show("Bitte wählen Sie einen Eintrag aus, den Sie löschen möchten.")
        End If
    End Sub
End Class

Public Class ProductService
    Public Property ID As Integer
    Public Property Designation As String
    Public Property Description As String
    Public Property UnitPrice As Decimal
    Public Property Category As String

    Public ReadOnly Property UnitPriceFormatted As String
        Get
            Return UnitPrice.ToString("N2", CultureInfo.GetCultureInfo("de-DE")) & " €"
        End Get
    End Property
End Class