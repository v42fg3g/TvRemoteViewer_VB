<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form2
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
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

    'Windows フォーム デザイナーで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
    'Windows フォーム デザイナーを使用して変更できます。  
    'コード エディターを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.textBoxSeekFileList = New System.Windows.Forms.TextBox()
        Me.labelHlsOpt2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'textBoxSeekFileList
        '
        Me.textBoxSeekFileList.Location = New System.Drawing.Point(0, 119)
        Me.textBoxSeekFileList.Multiline = True
        Me.textBoxSeekFileList.Name = "textBoxSeekFileList"
        Me.textBoxSeekFileList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.textBoxSeekFileList.Size = New System.Drawing.Size(454, 423)
        Me.textBoxSeekFileList.TabIndex = 41
        Me.textBoxSeekFileList.TabStop = False
        '
        'labelHlsOpt2
        '
        Me.labelHlsOpt2.AutoSize = True
        Me.labelHlsOpt2.Location = New System.Drawing.Point(7, 8)
        Me.labelHlsOpt2.Name = "labelHlsOpt2"
        Me.labelHlsOpt2.Size = New System.Drawing.Size(250, 12)
        Me.labelHlsOpt2.TabIndex = 42
        Me.labelHlsOpt2.Text = "ffmpeg　-i 後置　シーク(-ss)　ファイル名マッチリスト"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(7, 36)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(368, 12)
        Me.Label1.TabIndex = 43
        Me.Label1.Text = "通常でシークに失敗するファイル名（一部）を記入してください　(1行に1ファイル)"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(400, 31)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(54, 23)
        Me.Button1.TabIndex = 46
        Me.Button1.Text = "閉じる"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(7, 55)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(250, 12)
        Me.Label2.TabIndex = 47
        Me.Label2.Text = "ただし後方へ行けばいくほどシーク時間が長くなります"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(7, 74)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(265, 12)
        Me.Label4.TabIndex = 48
        Me.Label4.Text = "実用的には冒頭マージンを飛ばす程度にしか使えません"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(7, 93)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(181, 12)
        Me.Label5.TabIndex = 49
        Me.Label5.Text = "※サムネイル作成には適用されません"
        '
        'Form2
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(461, 543)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.labelHlsOpt2)
        Me.Controls.Add(Me.textBoxSeekFileList)
        Me.MaximumSize = New System.Drawing.Size(477, 580)
        Me.Name = "Form2"
        Me.Text = "ffmpegにおいてシーク方法を変更するファイルリスト"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents textBoxSeekFileList As System.Windows.Forms.TextBox
    Private WithEvents labelHlsOpt2 As System.Windows.Forms.Label
    Private WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Private WithEvents Label2 As System.Windows.Forms.Label
    Private WithEvents Label4 As System.Windows.Forms.Label
    Private WithEvents Label5 As System.Windows.Forms.Label
End Class
