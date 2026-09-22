<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormLeer
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.TextBoxResultado = New System.Windows.Forms.TextBox()
        Me.cmdCerrar = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'TextBoxResultado
        '
        Me.TextBoxResultado.Location = New System.Drawing.Point(37, 83)
        Me.TextBoxResultado.Name = "TextBoxResultado"
        Me.TextBoxResultado.Size = New System.Drawing.Size(159, 20)
        Me.TextBoxResultado.TabIndex = 0
        '
        'cmdCerrar
        '
        Me.cmdCerrar.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdCerrar.Location = New System.Drawing.Point(347, 83)
        Me.cmdCerrar.Name = "cmdCerrar"
        Me.cmdCerrar.Size = New System.Drawing.Size(75, 23)
        Me.cmdCerrar.TabIndex = 1
        Me.cmdCerrar.Text = "Cerrar"
        Me.cmdCerrar.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(34, 22)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(388, 41)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Exponga el código QR ante la cámara y espere la lectura." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Esta ventana se cerrará" &
    " cuando la lectura termine."
        '
        'FormLeer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.cmdCerrar
        Me.ClientSize = New System.Drawing.Size(446, 147)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cmdCerrar)
        Me.Controls.Add(Me.TextBoxResultado)
        Me.Name = "FormLeer"
        Me.Text = "Leer"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents TextBoxResultado As TextBox
    Friend WithEvents cmdCerrar As Button
    Friend WithEvents Label1 As Label
End Class
