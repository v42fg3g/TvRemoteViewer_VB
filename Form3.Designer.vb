<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form3
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
        Me.components = New System.ComponentModel.Container()
        Me.DataGridViewAccessLog = New System.Windows.Forms.DataGridView()
        Me.utime = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.IP = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.domain = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.UserAgent = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.URL = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.CellCopy = New System.Windows.Forms.ToolStripMenuItem()
        Me.LabelAccessLogWarning = New System.Windows.Forms.Label()
        Me.ButtonRefresh = New System.Windows.Forms.Button()
        Me.ButtonClear = New System.Windows.Forms.Button()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        CType(Me.DataGridViewAccessLog, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'DataGridViewAccessLog
        '
        Me.DataGridViewAccessLog.AllowUserToAddRows = False
        Me.DataGridViewAccessLog.AllowUserToDeleteRows = False
        Me.DataGridViewAccessLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridViewAccessLog.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.utime, Me.IP, Me.domain, Me.UserAgent, Me.URL})
        Me.DataGridViewAccessLog.Location = New System.Drawing.Point(0, 24)
        Me.DataGridViewAccessLog.Name = "DataGridViewAccessLog"
        Me.DataGridViewAccessLog.ReadOnly = True
        Me.DataGridViewAccessLog.RowHeadersVisible = False
        Me.DataGridViewAccessLog.RowTemplate.Height = 21
        Me.DataGridViewAccessLog.Size = New System.Drawing.Size(744, 235)
        Me.DataGridViewAccessLog.TabIndex = 0
        '
        'utime
        '
        Me.utime.HeaderText = "最終アクセス日時"
        Me.utime.Name = "utime"
        Me.utime.ReadOnly = True
        Me.utime.Width = 180
        '
        'IP
        '
        Me.IP.HeaderText = "IP"
        Me.IP.Name = "IP"
        Me.IP.ReadOnly = True
        Me.IP.Width = 90
        '
        'domain
        '
        Me.domain.HeaderText = "　"
        Me.domain.Name = "domain"
        Me.domain.ReadOnly = True
        Me.domain.Width = 200
        '
        'UserAgent
        '
        Me.UserAgent.HeaderText = "UserAgent"
        Me.UserAgent.Name = "UserAgent"
        Me.UserAgent.ReadOnly = True
        Me.UserAgent.Width = 200
        '
        'URL
        '
        Me.URL.HeaderText = "最終リクエスト"
        Me.URL.Name = "URL"
        Me.URL.ReadOnly = True
        Me.URL.Width = 300
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CellCopy})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(113, 26)
        '
        'CellCopy
        '
        Me.CellCopy.Name = "CellCopy"
        Me.CellCopy.Size = New System.Drawing.Size(112, 22)
        Me.CellCopy.Text = "コピー"
        '
        'LabelAccessLogWarning
        '
        Me.LabelAccessLogWarning.AutoSize = True
        Me.LabelAccessLogWarning.Font = New System.Drawing.Font("MS UI Gothic", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.LabelAccessLogWarning.ForeColor = System.Drawing.Color.Red
        Me.LabelAccessLogWarning.Location = New System.Drawing.Point(6, 6)
        Me.LabelAccessLogWarning.Name = "LabelAccessLogWarning"
        Me.LabelAccessLogWarning.Size = New System.Drawing.Size(14, 12)
        Me.LabelAccessLogWarning.TabIndex = 1
        Me.LabelAccessLogWarning.Text = "　"
        '
        'ButtonRefresh
        '
        Me.ButtonRefresh.Location = New System.Drawing.Point(695, 0)
        Me.ButtonRefresh.Name = "ButtonRefresh"
        Me.ButtonRefresh.Size = New System.Drawing.Size(75, 23)
        Me.ButtonRefresh.TabIndex = 2
        Me.ButtonRefresh.Text = "更新"
        Me.ButtonRefresh.UseVisualStyleBackColor = True
        Me.ButtonRefresh.Visible = False
        '
        'ButtonClear
        '
        Me.ButtonClear.Location = New System.Drawing.Point(642, 0)
        Me.ButtonClear.Name = "ButtonClear"
        Me.ButtonClear.Size = New System.Drawing.Size(47, 23)
        Me.ButtonClear.TabIndex = 3
        Me.ButtonClear.Text = "クリア"
        Me.ButtonClear.UseVisualStyleBackColor = True
        '
        'Timer1
        '
        Me.Timer1.Interval = 3000
        '
        'Form3
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(984, 561)
        Me.Controls.Add(Me.ButtonClear)
        Me.Controls.Add(Me.ButtonRefresh)
        Me.Controls.Add(Me.LabelAccessLogWarning)
        Me.Controls.Add(Me.DataGridViewAccessLog)
        Me.Name = "Form3"
        Me.Text = "TvRemoteViewer_VB アクセス ログ"
        CType(Me.DataGridViewAccessLog, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents DataGridViewAccessLog As System.Windows.Forms.DataGridView
    Friend WithEvents LabelAccessLogWarning As System.Windows.Forms.Label
    Friend WithEvents ButtonRefresh As System.Windows.Forms.Button
    Friend WithEvents ButtonClear As System.Windows.Forms.Button
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents utime As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents IP As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents domain As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents UserAgent As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents URL As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents CellCopy As System.Windows.Forms.ToolStripMenuItem
End Class
