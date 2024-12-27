Imports System.Collections.ObjectModel
Imports System.Data.SQLite
Imports System.Globalization
Imports System.IO

Public Class AddProductsServicesWindow
    Private ProductServices As ObservableCollection(Of ProductService)

    Public Sub New(productServices As ObservableCollection(Of ProductService))
        InitializeComponent()
        Me.ProductServices = productServices
    End Sub

    Private Sub InsertProductService(sender As Object, e As RoutedEventArgs)
        Dim designation As String = DesignationTextBox.Text
        Dim description As String = DescriptionTextBox.Text
        Dim unitPrice As Decimal
        If Not Decimal.TryParse(UnitPriceTextBox.Text, unitPrice) Then
            MessageBox.Show("Bitte geben Sie einen gültigen Preis ein.")
            Return
        End If
        Dim category As String = CategoryTextBox.Text

        If String.IsNullOrWhiteSpace(designation) OrElse String.IsNullOrWhiteSpace(unitPrice) Then
            MessageBox.Show("Bitte füllen Sie alle Pflichtfelder aus.")
            Return
        End If

        Using connection As New SQLiteConnection($"Data Source = {Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database.db")};Version=3;")
            connection.Open()

            Dim insertQuery As String = $"INSERT INTO products_services (
                                          designation, 
                                          description, 
                                          unit_price, 
                                          category
                                        ) VALUES (
                                          '{designation}',
                                          '{description}',
                                          {unitPrice.ToString(CultureInfo.InvariantCulture)},
                                          '{category}'
                                        );"

            Using command As New SQLiteCommand(insertQuery, connection)
                command.ExecuteNonQuery()
            End Using

            Dim idQuery As String = "SELECT last_insert_rowid();"
            Dim newId As Integer

            Using command As New SQLiteCommand(idQuery, connection)
                newId = Convert.ToInt32(command.ExecuteScalar())
            End Using

            Dim newProductService As New ProductService With {
                .ID = newId,
                .Designation = designation,
                .Description = description,
                .UnitPrice = unitPrice,
                .Category = category
            }

            ProductServices.Add(newProductService)
        End Using

        MessageBox.Show($"Produkt/Service ""{designation}"" wurde erfolgreich hinzugefügt.")

        Me.Close()
    End Sub

    Private Sub Cancel(sender As Object, e As RoutedEventArgs)
        Me.Close()
    End Sub
End Class