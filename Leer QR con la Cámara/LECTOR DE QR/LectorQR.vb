'Paquetes NuGet necesarios
'Antes de usar la clase, instala estos paquetes en tu proyecto (en Visual Studio 
'Herramientas > Administrador de paquetes NuGet > Consola del Administrador de paquetes)

'Install-Package AForge.Video
'Install-Package AForge.Video.DirectShow
'Install-Package ZXing.Net


Imports AForge.Video
Imports AForge.Video.DirectShow
Imports ZXing
Imports System.Drawing
Imports System.Media   ' <-- Necesario para SystemSounds.Beep

''' <summary>
''' Clase que encapsula la lectura de códigos QR desde la cámara del ordenador.
''' Utiliza AForge.NET para la captura de vídeo y ZXing.NET para la decodificación.
''' </summary>
Public Class LectorQR
    Implements IDisposable

    ' --- Eventos públicos ---
    Public Event QRDetectado(codigo As String)
    Public Event ErrorOcurrido(ex As Exception)

    ' --- Campos privados ---
    Private _dispositivosVideo As FilterInfoCollection
    Private _fuenteVideo As VideoCaptureDevice
    Private ReadOnly _lector As BarcodeReader
    Private _activo As Boolean = False
    Private ReadOnly _lock As New Object()

    ' --- Campos para control del beep ---
    Private _ultimoCodigo As String = String.Empty
    Private _ultimoBeep As DateTime = DateTime.MinValue
    Private _intervaloMinimoBeep As TimeSpan = TimeSpan.FromSeconds(1.5)   ' sin ReadOnly

    ''' <summary>
    ''' Indica si el lector está capturando activamente.
    ''' </summary>
    Public ReadOnly Property Activo As Boolean
        Get
            Return _activo
        End Get
    End Property

    ''' <summary>
    ''' Indica si se debe emitir un beep al detectar un QR correctamente.
    ''' Por defecto está activado.
    ''' </summary>
    Public Property EmitirBeep As Boolean = True

    ''' <summary>
    ''' Intervalo mínimo entre dos beeps consecutivos (anti-rebote).
    ''' Evita que el beep se repita sin parar mientras el QR siga visible.
    ''' </summary>
    Public Property IntervaloMinimoBeep As TimeSpan
        Get
            Return _intervaloMinimoBeep
        End Get
        Set(value As TimeSpan)
            If value < TimeSpan.Zero Then value = TimeSpan.Zero
            _intervaloMinimoBeep = value
        End Set
    End Property

    ' --- Constructor ---
    Public Sub New()
        Dim opciones As New ZXing.Common.DecodingOptions With {
            .PossibleFormats = New List(Of BarcodeFormat) From {BarcodeFormat.QR_CODE},
            .TryHarder = True
        }

        _lector = New BarcodeReader With {
            .Options = opciones,
            .AutoRotate = True,
            .TryInverted = True
        }
    End Sub

    ''' <summary>
    ''' Inicia la captura desde la cámara predeterminada (la primera disponible).
    ''' </summary>
    Public Sub Iniciar()
        If _activo Then Return
        Try
            _dispositivosVideo = New FilterInfoCollection(FilterCategory.VideoInputDevice)
            If _dispositivosVideo.Count = 0 Then
                Throw New InvalidOperationException("No se encontró ninguna cámara conectada.")
            End If

            _fuenteVideo = New VideoCaptureDevice(_dispositivosVideo(0).MonikerString)
            AddHandler _fuenteVideo.NewFrame, AddressOf OnNuevoFotograma

            _fuenteVideo.Start()
            _activo = True
        Catch ex As Exception
            _activo = False
            RaiseEvent ErrorOcurrido(ex)
        End Try
    End Sub

    ''' <summary>
    ''' Inicia la captura desde una cámara específica por índice.
    ''' </summary>
    Public Sub Iniciar(indiceCamara As Integer)
        If _activo Then Return
        Try
            _dispositivosVideo = New FilterInfoCollection(FilterCategory.VideoInputDevice)
            If indiceCamara < 0 OrElse indiceCamara >= _dispositivosVideo.Count Then
                Throw New ArgumentOutOfRangeException(NameOf(indiceCamara), "Índice de cámara no válido.")
            End If

            _fuenteVideo = New VideoCaptureDevice(_dispositivosVideo(indiceCamara).MonikerString)
            AddHandler _fuenteVideo.NewFrame, AddressOf OnNuevoFotograma

            _fuenteVideo.Start()
            _activo = True
        Catch ex As Exception
            _activo = False
            RaiseEvent ErrorOcurrido(ex)
        End Try
    End Sub

    ''' <summary>
    ''' Detiene la captura de la cámara.
    ''' </summary>
    Public Sub Detener()
        If Not _activo Then Return
        Try
            If _fuenteVideo IsNot Nothing AndAlso _fuenteVideo.IsRunning Then
                RemoveHandler _fuenteVideo.NewFrame, AddressOf OnNuevoFotograma
                _fuenteVideo.SignalToStop()
                _fuenteVideo.WaitForStop()
            End If
        Catch ex As Exception
            RaiseEvent ErrorOcurrido(ex)
        Finally
            _activo = False
            _fuenteVideo = Nothing
            _ultimoCodigo = String.Empty
        End Try
    End Sub

    ''' <summary>
    ''' Devuelve la lista de nombres de las cámaras disponibles.
    ''' </summary>
    Public Function ObtenerCamaras() As List(Of String)
        Dim lista As New List(Of String)
        Try
            Dim dispositivos As New FilterInfoCollection(FilterCategory.VideoInputDevice)
            For Each dispositivo As FilterInfo In dispositivos
                lista.Add(dispositivo.Name)
            Next
        Catch ex As Exception
            RaiseEvent ErrorOcurrido(ex)
        End Try
        Return lista
    End Function

    ' --- Manejador del evento NewFrame ---
    Private Sub OnNuevoFotograma(sender As Object, e As NewFrameEventArgs)
        Dim fotograma As Bitmap = Nothing
        SyncLock _lock
            If e.Frame IsNot Nothing Then
                fotograma = DirectCast(e.Frame.Clone(), Bitmap)
            End If
        End SyncLock

        If fotograma Is Nothing Then Return

        Try
            Dim resultado As Result = _lector.Decode(fotograma)

            If resultado IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(resultado.Text) Then

                ' 🔔 Beep con anti-rebote
                If DebeEmitirBeep(resultado.Text) Then
                    ReproducirBeep()
                    _ultimoBeep = DateTime.Now
                    _ultimoCodigo = resultado.Text
                End If

                RaiseEvent QRDetectado(resultado.Text)
            End If
        Catch ex As Exception
            RaiseEvent ErrorOcurrido(ex)
        Finally
            fotograma.Dispose()
        End Try
    End Sub

    ''' <summary>
    ''' Determina si corresponde emitir el beep según el código detectado
    ''' y el tiempo transcurrido desde el último beep.
    ''' </summary>
    Private Function DebeEmitirBeep(codigoActual As String) As Boolean
        If Not EmitirBeep Then Return False

        ' Si es un código distinto al anterior -> siempre suena
        If Not String.Equals(codigoActual, _ultimoCodigo, StringComparison.Ordinal) Then
            Return True
        End If

        ' Si es el mismo código, solo suena si ya pasó el intervalo mínimo
        Return (DateTime.Now - _ultimoBeep) >= _intervaloMinimoBeep
    End Function

    ''' <summary>
    ''' Reproduce el sonido de notificación. Puedes sobrescribirlo en una
    ''' subclase para usar un .wav personalizado o Console.Beep.
    ''' </summary>
    Protected Overridable Sub ReproducirBeep()
        Try
            SystemSounds.Beep.Play()
        Catch
            ' Silenciar cualquier error de audio
        End Try
    End Sub

    ' --- Implementación de IDisposable ---
    Private _disposed As Boolean = False

    Protected Overridable Sub Dispose(disposing As Boolean)
        If _disposed Then Return
        If disposing Then Detener()
        _disposed = True
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
End Class