Public Class FormLeer

    Private WithEvents _lectorQR As LectorQR

    ' --- Propiedad pública que leerá FormInicio ---
    Public Property CodigoQR As String = String.Empty

    Private Sub FormLeer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _lectorQR = New LectorQR()

        Dim camaras = _lectorQR.ObtenerCamaras()
        If camaras.Count = 0 Then
            MessageBox.Show("No hay cámaras conectadas.")
            Me.DialogResult = DialogResult.Cancel
            Me.Close()
            Return
        End If

        _lectorQR.Iniciar(0)
    End Sub

    ' Manejador del evento QRDetectado
    Private Sub LectorQR_QRDetectado(codigo As String) Handles _lectorQR.QRDetectado

        ' Siempre nos aseguramos de ejecutar en el hilo de UI
        If Me.InvokeRequired Then
            Me.BeginInvoke(New Action(Sub() LectorQR_QRDetectado(codigo)))
            Return
        End If

        ' 1) Guardar el código SIEMPRE (antes estaba solo en el Else)
        CodigoQR = codigo
        'no se muestra el codigo en pantalla
        'TextBoxResultado.Text = codigo

        ' 2) Detener la cámara para liberar el dispositivo antes de cerrar
        _lectorQR.Detener()

        ' 3) Cerrar el formulario -> ShowDialog() retorna en FormInicio
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    ' Manejador de errores
    Private Sub LectorQR_ErrorOcurrido(ex As Exception) Handles _lectorQR.ErrorOcurrido
        If Me.InvokeRequired Then
            Me.BeginInvoke(Sub() MessageBox.Show(ex.Message, "Error"))
        Else
            MessageBox.Show(ex.Message, "Error")
        End If
    End Sub

    Private Sub FormLeer_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        _lectorQR?.Dispose()
    End Sub

    Private Sub cmdCerrar_Click(sender As Object, e As EventArgs) Handles cmdCerrar.Click
        Me.Close()
    End Sub
End Class