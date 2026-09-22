Public Class FormInicio

    Private Sub FormInicio_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        ' Crear el formulario de lectura y mostrarlo como diálogo modal.
        ' Cuando se detecte un QR, FormLeer se cerrará solo (DialogResult.OK)
        ' y ShowDialog() retornará, dejando disponible la propiedad CodigoQR.
        Dim codigoQR As String

        Using formLeer As New FormLeer()
            formLeer.ShowDialog()
            codigoQR = formLeer.CodigoQR
        End Using

        ' Mostrar el resultado
        If Not String.IsNullOrEmpty(codigoQR) Then
            'MessageBox.Show("Código QR leído: " & codigoQR)
            TextBox1.Text = codigoQR
        Else
            MessageBox.Show("No se leyó ningún código QR.")
        End If

    End Sub
End Class