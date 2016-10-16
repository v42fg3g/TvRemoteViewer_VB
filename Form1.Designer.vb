<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.buttonHlsAppPath = New System.Windows.Forms.Button()
        Me.buttonUdpAppPath = New System.Windows.Forms.Button()
        Me.textBoxHlsOpt2 = New System.Windows.Forms.TextBox()
        Me.labelHlsOpt2 = New System.Windows.Forms.Label()
        Me.textBoxHlsApp = New System.Windows.Forms.TextBox()
        Me.labelHlsApp = New System.Windows.Forms.Label()
        Me.textBoxUdpApp = New System.Windows.Forms.TextBox()
        Me.labelUdpApp = New System.Windows.Forms.Label()
        Me.textHttpPortNumber = New System.Windows.Forms.TextBox()
        Me.labelPortNuber = New System.Windows.Forms.Label()
        Me.ButtonMovieStart = New System.Windows.Forms.Button()
        Me.ButtonMovieStop = New System.Windows.Forms.Button()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.ButtonWebStop = New System.Windows.Forms.Button()
        Me.ButtonWebStart = New System.Windows.Forms.Button()
        Me.ComboBoxNum = New System.Windows.Forms.ComboBox()
        Me.textBoxUdpPort = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TextBoxChSpace = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.ButtonWWWROOT = New System.Windows.Forms.Button()
        Me.TextBoxWWWroot = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TextBoxLog = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.ButtonBonDriverPath = New System.Windows.Forms.Button()
        Me.TextBoxBonDriverPath = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.ComboBoxBonDriver = New System.Windows.Forms.ComboBox()
        Me.ComboBoxServiceID = New System.Windows.Forms.ComboBox()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.ComboBoxResolution = New System.Windows.Forms.ComboBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.ButtonHLSoption = New System.Windows.Forms.Button()
        Me.Button5 = New System.Windows.Forms.Button()
        Me.Button6 = New System.Windows.Forms.Button()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.ButtonFILEROOT = New System.Windows.Forms.Button()
        Me.TextBoxFILEROOT = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.NotifyIcon1 = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.SeekMethodList = New System.Windows.Forms.ToolStripMenuItem()
        Me.quit = New System.Windows.Forms.ToolStripMenuItem()
        Me.CheckBoxShowConsole = New System.Windows.Forms.CheckBox()
        Me.LabelStream = New System.Windows.Forms.Label()
        Me.TextBoxUdpOpt3 = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.TextBoxID = New System.Windows.Forms.TextBox()
        Me.TextBoxPASS = New System.Windows.Forms.TextBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.ComboBoxHLSorHTTP = New System.Windows.Forms.ComboBox()
        Me.ButtonCopy2Clipboard = New System.Windows.Forms.Button()
        Me.Button7 = New System.Windows.Forms.Button()
        Me.ComboBoxRezFormOrCombo = New System.Windows.Forms.ComboBox()
        Me.ComboBoxVideoForce = New System.Windows.Forms.ComboBox()
        Me.Button4 = New System.Windows.Forms.Button()
        Me.Button8 = New System.Windows.Forms.Button()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.LabelVersionCheckDate = New System.Windows.Forms.Label()
        Me.CheckBoxVersionCheck = New System.Windows.Forms.CheckBox()
        Me.LabelVersionWarning = New System.Windows.Forms.Label()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'buttonHlsAppPath
        '
        Me.buttonHlsAppPath.Location = New System.Drawing.Point(491, 254)
        Me.buttonHlsAppPath.Name = "buttonHlsAppPath"
        Me.buttonHlsAppPath.Size = New System.Drawing.Size(25, 19)
        Me.buttonHlsAppPath.TabIndex = 38
        Me.buttonHlsAppPath.TabStop = False
        Me.buttonHlsAppPath.Text = "..."
        Me.buttonHlsAppPath.UseVisualStyleBackColor = True
        '
        'buttonUdpAppPath
        '
        Me.buttonUdpAppPath.Location = New System.Drawing.Point(492, 160)
        Me.buttonUdpAppPath.Name = "buttonUdpAppPath"
        Me.buttonUdpAppPath.Size = New System.Drawing.Size(25, 19)
        Me.buttonUdpAppPath.TabIndex = 23
        Me.buttonUdpAppPath.TabStop = False
        Me.buttonUdpAppPath.Text = "..."
        Me.buttonUdpAppPath.UseVisualStyleBackColor = True
        '
        'textBoxHlsOpt2
        '
        Me.textBoxHlsOpt2.Location = New System.Drawing.Point(102, 279)
        Me.textBoxHlsOpt2.Multiline = True
        Me.textBoxHlsOpt2.Name = "textBoxHlsOpt2"
        Me.textBoxHlsOpt2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.textBoxHlsOpt2.Size = New System.Drawing.Size(415, 96)
        Me.textBoxHlsOpt2.TabIndex = 40
        Me.textBoxHlsOpt2.TabStop = False
        '
        'labelHlsOpt2
        '
        Me.labelHlsOpt2.AutoSize = True
        Me.labelHlsOpt2.Location = New System.Drawing.Point(8, 282)
        Me.labelHlsOpt2.Name = "labelHlsOpt2"
        Me.labelHlsOpt2.Size = New System.Drawing.Size(73, 12)
        Me.labelHlsOpt2.TabIndex = 26
        Me.labelHlsOpt2.Text = "HLS オプション"
        '
        'textBoxHlsApp
        '
        Me.textBoxHlsApp.Location = New System.Drawing.Point(102, 254)
        Me.textBoxHlsApp.Name = "textBoxHlsApp"
        Me.textBoxHlsApp.Size = New System.Drawing.Size(384, 19)
        Me.textBoxHlsApp.TabIndex = 37
        Me.textBoxHlsApp.TabStop = False
        '
        'labelHlsApp
        '
        Me.labelHlsApp.AutoSize = True
        Me.labelHlsApp.Location = New System.Drawing.Point(8, 257)
        Me.labelHlsApp.Name = "labelHlsApp"
        Me.labelHlsApp.Size = New System.Drawing.Size(51, 12)
        Me.labelHlsApp.TabIndex = 29
        Me.labelHlsApp.Text = "HLSアプリ"
        '
        'textBoxUdpApp
        '
        Me.textBoxUdpApp.Location = New System.Drawing.Point(102, 160)
        Me.textBoxUdpApp.Name = "textBoxUdpApp"
        Me.textBoxUdpApp.Size = New System.Drawing.Size(384, 19)
        Me.textBoxUdpApp.TabIndex = 22
        Me.textBoxUdpApp.TabStop = False
        '
        'labelUdpApp
        '
        Me.labelUdpApp.AutoSize = True
        Me.labelUdpApp.Location = New System.Drawing.Point(7, 163)
        Me.labelUdpApp.Name = "labelUdpApp"
        Me.labelUdpApp.Size = New System.Drawing.Size(53, 12)
        Me.labelUdpApp.TabIndex = 27
        Me.labelUdpApp.Text = "UDPアプリ"
        '
        'textHttpPortNumber
        '
        Me.textHttpPortNumber.Location = New System.Drawing.Point(101, 106)
        Me.textHttpPortNumber.Name = "textHttpPortNumber"
        Me.textHttpPortNumber.Size = New System.Drawing.Size(43, 19)
        Me.textHttpPortNumber.TabIndex = 42
        Me.textHttpPortNumber.TabStop = False
        Me.textHttpPortNumber.Text = "40003"
        '
        'labelPortNuber
        '
        Me.labelPortNuber.AutoSize = True
        Me.labelPortNuber.Location = New System.Drawing.Point(5, 109)
        Me.labelPortNuber.Name = "labelPortNuber"
        Me.labelPortNuber.Size = New System.Drawing.Size(80, 12)
        Me.labelPortNuber.TabIndex = 21
        Me.labelPortNuber.Text = "HTTPポート (*)"
        '
        'ButtonMovieStart
        '
        Me.ButtonMovieStart.Location = New System.Drawing.Point(374, 510)
        Me.ButtonMovieStart.Name = "ButtonMovieStart"
        Me.ButtonMovieStart.Size = New System.Drawing.Size(68, 28)
        Me.ButtonMovieStart.TabIndex = 49
        Me.ButtonMovieStart.TabStop = False
        Me.ButtonMovieStart.Text = "Start"
        Me.ButtonMovieStart.UseVisualStyleBackColor = True
        '
        'ButtonMovieStop
        '
        Me.ButtonMovieStop.Location = New System.Drawing.Point(448, 510)
        Me.ButtonMovieStop.Name = "ButtonMovieStop"
        Me.ButtonMovieStop.Size = New System.Drawing.Size(68, 28)
        Me.ButtonMovieStop.TabIndex = 50
        Me.ButtonMovieStop.TabStop = False
        Me.ButtonMovieStop.Text = "Stop"
        Me.ButtonMovieStop.UseVisualStyleBackColor = True
        '
        'Timer1
        '
        Me.Timer1.Interval = 1000
        '
        'ButtonWebStop
        '
        Me.ButtonWebStop.Enabled = False
        Me.ButtonWebStop.Location = New System.Drawing.Point(224, 103)
        Me.ButtonWebStop.Name = "ButtonWebStop"
        Me.ButtonWebStop.Size = New System.Drawing.Size(68, 28)
        Me.ButtonWebStop.TabIndex = 52
        Me.ButtonWebStop.TabStop = False
        Me.ButtonWebStop.Text = "WebStop"
        Me.ButtonWebStop.UseVisualStyleBackColor = True
        Me.ButtonWebStop.Visible = False
        '
        'ButtonWebStart
        '
        Me.ButtonWebStart.Location = New System.Drawing.Point(205, 103)
        Me.ButtonWebStart.Name = "ButtonWebStart"
        Me.ButtonWebStart.Size = New System.Drawing.Size(68, 28)
        Me.ButtonWebStart.TabIndex = 51
        Me.ButtonWebStart.TabStop = False
        Me.ButtonWebStart.Text = "WebStart"
        Me.ButtonWebStart.UseVisualStyleBackColor = True
        Me.ButtonWebStart.Visible = False
        '
        'ComboBoxNum
        '
        Me.ComboBoxNum.FormattingEnabled = True
        Me.ComboBoxNum.Items.AddRange(New Object() {"1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12"})
        Me.ComboBoxNum.Location = New System.Drawing.Point(102, 445)
        Me.ComboBoxNum.Name = "ComboBoxNum"
        Me.ComboBoxNum.Size = New System.Drawing.Size(51, 20)
        Me.ComboBoxNum.TabIndex = 53
        Me.ComboBoxNum.TabStop = False
        Me.ComboBoxNum.Text = "1"
        '
        'textBoxUdpPort
        '
        Me.textBoxUdpPort.Location = New System.Drawing.Point(102, 210)
        Me.textBoxUdpPort.Name = "textBoxUdpPort"
        Me.textBoxUdpPort.Size = New System.Drawing.Size(43, 19)
        Me.textBoxUdpPort.TabIndex = 55
        Me.textBoxUdpPort.TabStop = False
        Me.textBoxUdpPort.Text = "42424"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(6, 213)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(74, 12)
        Me.Label2.TabIndex = 54
        Me.Label2.Text = "UDPポート (*)"
        '
        'TextBoxChSpace
        '
        Me.TextBoxChSpace.Location = New System.Drawing.Point(102, 492)
        Me.TextBoxChSpace.Name = "TextBoxChSpace"
        Me.TextBoxChSpace.Size = New System.Drawing.Size(28, 19)
        Me.TextBoxChSpace.TabIndex = 57
        Me.TextBoxChSpace.TabStop = False
        Me.TextBoxChSpace.Text = "0"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(8, 495)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(47, 12)
        Me.Label3.TabIndex = 56
        Me.Label3.Text = "chspace"
        '
        'ButtonWWWROOT
        '
        Me.ButtonWWWROOT.Location = New System.Drawing.Point(492, 30)
        Me.ButtonWWWROOT.Name = "ButtonWWWROOT"
        Me.ButtonWWWROOT.Size = New System.Drawing.Size(25, 19)
        Me.ButtonWWWROOT.TabIndex = 60
        Me.ButtonWWWROOT.TabStop = False
        Me.ButtonWWWROOT.Text = "..."
        Me.ButtonWWWROOT.UseVisualStyleBackColor = True
        '
        'TextBoxWWWroot
        '
        Me.TextBoxWWWroot.Location = New System.Drawing.Point(102, 30)
        Me.TextBoxWWWroot.Name = "TextBoxWWWroot"
        Me.TextBoxWWWroot.Size = New System.Drawing.Size(384, 19)
        Me.TextBoxWWWroot.TabIndex = 59
        Me.TextBoxWWWroot.TabStop = False
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(7, 33)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(93, 12)
        Me.Label4.TabIndex = 61
        Me.Label4.Text = "%WWWROOT% (*)"
        '
        'TextBoxLog
        '
        Me.TextBoxLog.Location = New System.Drawing.Point(0, 544)
        Me.TextBoxLog.Multiline = True
        Me.TextBoxLog.Name = "TextBoxLog"
        Me.TextBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TextBoxLog.Size = New System.Drawing.Size(529, 141)
        Me.TextBoxLog.TabIndex = 62
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(8, 472)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(56, 12)
        Me.Label5.TabIndex = 65
        Me.Label5.Text = "BonDriver"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(8, 518)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(54, 12)
        Me.Label6.TabIndex = 66
        Me.Label6.Text = "ServiceID"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(8, 448)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(92, 12)
        Me.Label7.TabIndex = 67
        Me.Label7.Text = "ストリーム Number"
        '
        'ButtonBonDriverPath
        '
        Me.ButtonBonDriverPath.Location = New System.Drawing.Point(492, 185)
        Me.ButtonBonDriverPath.Name = "ButtonBonDriverPath"
        Me.ButtonBonDriverPath.Size = New System.Drawing.Size(25, 19)
        Me.ButtonBonDriverPath.TabIndex = 69
        Me.ButtonBonDriverPath.TabStop = False
        Me.ButtonBonDriverPath.Text = "..."
        Me.ButtonBonDriverPath.UseVisualStyleBackColor = True
        '
        'TextBoxBonDriverPath
        '
        Me.TextBoxBonDriverPath.Location = New System.Drawing.Point(102, 185)
        Me.TextBoxBonDriverPath.Name = "TextBoxBonDriverPath"
        Me.TextBoxBonDriverPath.Size = New System.Drawing.Size(384, 19)
        Me.TextBoxBonDriverPath.TabIndex = 68
        Me.TextBoxBonDriverPath.TabStop = False
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(7, 188)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(83, 12)
        Me.Label8.TabIndex = 70
        Me.Label8.Text = "BonDriver Path"
        '
        'ComboBoxBonDriver
        '
        Me.ComboBoxBonDriver.FormattingEnabled = True
        Me.ComboBoxBonDriver.Location = New System.Drawing.Point(102, 468)
        Me.ComboBoxBonDriver.Name = "ComboBoxBonDriver"
        Me.ComboBoxBonDriver.Size = New System.Drawing.Size(210, 20)
        Me.ComboBoxBonDriver.TabIndex = 71
        Me.ComboBoxBonDriver.TabStop = False
        '
        'ComboBoxServiceID
        '
        Me.ComboBoxServiceID.FormattingEnabled = True
        Me.ComboBoxServiceID.Location = New System.Drawing.Point(102, 515)
        Me.ComboBoxServiceID.Name = "ComboBoxServiceID"
        Me.ComboBoxServiceID.Size = New System.Drawing.Size(249, 20)
        Me.ComboBoxServiceID.TabIndex = 72
        Me.ComboBoxServiceID.TabStop = False
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(414, 103)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(102, 28)
        Me.Button2.TabIndex = 73
        Me.Button2.TabStop = False
        Me.Button2.Text = "index.htmlを開く"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(200, 213)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(56, 12)
        Me.Label9.TabIndex = 74
        Me.Label9.Text = "＋Number"
        '
        'ComboBoxResolution
        '
        Me.ComboBoxResolution.FormattingEnabled = True
        Me.ComboBoxResolution.Location = New System.Drawing.Point(264, 379)
        Me.ComboBoxResolution.Name = "ComboBoxResolution"
        Me.ComboBoxResolution.Size = New System.Drawing.Size(87, 20)
        Me.ComboBoxResolution.TabIndex = 75
        Me.ComboBoxResolution.TabStop = False
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(279, 103)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(129, 28)
        Me.Button1.TabIndex = 76
        Me.Button1.TabStop = False
        Me.Button1.Text = "設定ファイルを編集(*)"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'ButtonHLSoption
        '
        Me.ButtonHLSoption.Location = New System.Drawing.Point(355, 376)
        Me.ButtonHLSoption.Name = "ButtonHLSoption"
        Me.ButtonHLSoption.Size = New System.Drawing.Size(102, 25)
        Me.ButtonHLSoption.TabIndex = 77
        Me.ButtonHLSoption.TabStop = False
        Me.ButtonHLSoption.Text = "HLS_option.txt(*)"
        Me.ButtonHLSoption.UseVisualStyleBackColor = True
        '
        'Button5
        '
        Me.Button5.Location = New System.Drawing.Point(147, 103)
        Me.Button5.Name = "Button5"
        Me.Button5.Size = New System.Drawing.Size(52, 25)
        Me.Button5.TabIndex = 79
        Me.Button5.TabStop = False
        Me.Button5.Text = "初期値"
        Me.Button5.UseVisualStyleBackColor = True
        '
        'Button6
        '
        Me.Button6.Location = New System.Drawing.Point(148, 207)
        Me.Button6.Name = "Button6"
        Me.Button6.Size = New System.Drawing.Size(52, 25)
        Me.Button6.TabIndex = 80
        Me.Button6.TabStop = False
        Me.Button6.Text = "初期値"
        Me.Button6.UseVisualStyleBackColor = True
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(312, 9)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(204, 12)
        Me.Label10.TabIndex = 81
        Me.Label10.Text = "(*)変更後はこのアプリを再起動してください"
        '
        'ButtonFILEROOT
        '
        Me.ButtonFILEROOT.Location = New System.Drawing.Point(492, 55)
        Me.ButtonFILEROOT.Name = "ButtonFILEROOT"
        Me.ButtonFILEROOT.Size = New System.Drawing.Size(25, 19)
        Me.ButtonFILEROOT.TabIndex = 83
        Me.ButtonFILEROOT.TabStop = False
        Me.ButtonFILEROOT.Text = "..."
        Me.ButtonFILEROOT.UseVisualStyleBackColor = True
        '
        'TextBoxFILEROOT
        '
        Me.TextBoxFILEROOT.Location = New System.Drawing.Point(102, 55)
        Me.TextBoxFILEROOT.Name = "TextBoxFILEROOT"
        Me.TextBoxFILEROOT.Size = New System.Drawing.Size(384, 19)
        Me.TextBoxFILEROOT.TabIndex = 82
        Me.TextBoxFILEROOT.TabStop = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(7, 58)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(89, 12)
        Me.Label1.TabIndex = 84
        Me.Label1.Text = "%FILEROOT% (*)"
        '
        'NotifyIcon1
        '
        Me.NotifyIcon1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.NotifyIcon1.Icon = CType(resources.GetObject("NotifyIcon1.Icon"), System.Drawing.Icon)
        Me.NotifyIcon1.Text = "TvRemoteViewer_VB"
        Me.NotifyIcon1.Visible = True
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SeekMethodList, Me.quit})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.ShowImageMargin = False
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(265, 48)
        '
        'SeekMethodList
        '
        Me.SeekMethodList.Name = "SeekMethodList"
        Me.SeekMethodList.Size = New System.Drawing.Size(264, 22)
        Me.SeekMethodList.Text = "ffmpeg シーク方法変更ファイルリスト"
        '
        'quit
        '
        Me.quit.Name = "quit"
        Me.quit.Size = New System.Drawing.Size(264, 22)
        Me.quit.Text = "終了"
        '
        'CheckBoxShowConsole
        '
        Me.CheckBoxShowConsole.AutoSize = True
        Me.CheckBoxShowConsole.Location = New System.Drawing.Point(394, 488)
        Me.CheckBoxShowConsole.Name = "CheckBoxShowConsole"
        Me.CheckBoxShowConsole.Size = New System.Drawing.Size(122, 16)
        Me.CheckBoxShowConsole.TabIndex = 86
        Me.CheckBoxShowConsole.TabStop = False
        Me.CheckBoxShowConsole.Text = "コンソールを表示する"
        Me.CheckBoxShowConsole.UseVisualStyleBackColor = True
        '
        'LabelStream
        '
        Me.LabelStream.AutoSize = True
        Me.LabelStream.Location = New System.Drawing.Point(165, 449)
        Me.LabelStream.Name = "LabelStream"
        Me.LabelStream.Size = New System.Drawing.Size(47, 12)
        Me.LabelStream.TabIndex = 88
        Me.LabelStream.Text = "配信中："
        '
        'TextBoxUdpOpt3
        '
        Me.TextBoxUdpOpt3.Location = New System.Drawing.Point(374, 210)
        Me.TextBoxUdpOpt3.Name = "TextBoxUdpOpt3"
        Me.TextBoxUdpOpt3.Size = New System.Drawing.Size(91, 19)
        Me.TextBoxUdpOpt3.TabIndex = 90
        Me.TextBoxUdpOpt3.TabStop = False
        Me.TextBoxUdpOpt3.Text = "/sendservice 1"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(278, 213)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(95, 12)
        Me.Label11.TabIndex = 89
        Me.Label11.Text = "追加UDPオプション"
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(469, 207)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(50, 25)
        Me.Button3.TabIndex = 91
        Me.Button3.TabStop = False
        Me.Button3.Text = "初期値"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'TextBoxID
        '
        Me.TextBoxID.Location = New System.Drawing.Point(121, 80)
        Me.TextBoxID.Name = "TextBoxID"
        Me.TextBoxID.Size = New System.Drawing.Size(91, 19)
        Me.TextBoxID.TabIndex = 92
        Me.TextBoxID.TabStop = False
        '
        'TextBoxPASS
        '
        Me.TextBoxPASS.Location = New System.Drawing.Point(258, 80)
        Me.TextBoxPASS.Name = "TextBoxPASS"
        Me.TextBoxPASS.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.TextBoxPASS.Size = New System.Drawing.Size(91, 19)
        Me.TextBoxPASS.TabIndex = 93
        Me.TextBoxPASS.TabStop = False
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(6, 83)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(81, 12)
        Me.Label12.TabIndex = 94
        Me.Label12.Text = "BASIC認証 (*)"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(102, 83)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(16, 12)
        Me.Label13.TabIndex = 95
        Me.Label13.Text = "ID"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(221, 83)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(34, 12)
        Me.Label14.TabIndex = 96
        Me.Label14.Text = "PASS"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(182, 494)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(53, 12)
        Me.Label15.TabIndex = 98
        Me.Label15.Text = "配信方式"
        '
        'ComboBoxHLSorHTTP
        '
        Me.ComboBoxHLSorHTTP.FormattingEnabled = True
        Me.ComboBoxHLSorHTTP.Items.AddRange(New Object() {"HLS", "HTTP"})
        Me.ComboBoxHLSorHTTP.Location = New System.Drawing.Point(241, 491)
        Me.ComboBoxHLSorHTTP.Name = "ComboBoxHLSorHTTP"
        Me.ComboBoxHLSorHTTP.Size = New System.Drawing.Size(51, 20)
        Me.ComboBoxHLSorHTTP.TabIndex = 97
        Me.ComboBoxHLSorHTTP.TabStop = False
        Me.ComboBoxHLSorHTTP.Text = "HLS"
        '
        'ButtonCopy2Clipboard
        '
        Me.ButtonCopy2Clipboard.Font = New System.Drawing.Font("MS UI Gothic", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonCopy2Clipboard.Location = New System.Drawing.Point(407, 668)
        Me.ButtonCopy2Clipboard.Name = "ButtonCopy2Clipboard"
        Me.ButtonCopy2Clipboard.Size = New System.Drawing.Size(104, 17)
        Me.ButtonCopy2Clipboard.TabIndex = 99
        Me.ButtonCopy2Clipboard.Text = "クリップボードにコピー"
        Me.ButtonCopy2Clipboard.UseVisualStyleBackColor = True
        '
        'Button7
        '
        Me.Button7.Location = New System.Drawing.Point(458, 376)
        Me.Button7.Name = "Button7"
        Me.Button7.Size = New System.Drawing.Size(59, 25)
        Me.Button7.TabIndex = 101
        Me.Button7.TabStop = False
        Me.Button7.Text = "再読込"
        Me.Button7.UseVisualStyleBackColor = True
        '
        'ComboBoxRezFormOrCombo
        '
        Me.ComboBoxRezFormOrCombo.FormattingEnabled = True
        Me.ComboBoxRezFormOrCombo.Items.AddRange(New Object() {"HLSオプション優先使用", "解像度インデックス優先使用"})
        Me.ComboBoxRezFormOrCombo.Location = New System.Drawing.Point(101, 379)
        Me.ComboBoxRezFormOrCombo.Name = "ComboBoxRezFormOrCombo"
        Me.ComboBoxRezFormOrCombo.Size = New System.Drawing.Size(161, 20)
        Me.ComboBoxRezFormOrCombo.TabIndex = 102
        Me.ComboBoxRezFormOrCombo.TabStop = False
        Me.ComboBoxRezFormOrCombo.Text = "HLSオプション優先使用"
        '
        'ComboBoxVideoForce
        '
        Me.ComboBoxVideoForce.FormattingEnabled = True
        Me.ComboBoxVideoForce.Items.AddRange(New Object() {"0 通常再生", "1 ファイル再生にffmpeg使用", "2 Pipe経由 ffmpeg→QSVEnc", "3 Pipe経由+ts以外・倍速等はffmpeg", "4 ts以外・倍速等はffmpegで再生", "9 プロファイル(profile.txt)に従って再生"})
        Me.ComboBoxVideoForce.Location = New System.Drawing.Point(101, 403)
        Me.ComboBoxVideoForce.Name = "ComboBoxVideoForce"
        Me.ComboBoxVideoForce.Size = New System.Drawing.Size(250, 20)
        Me.ComboBoxVideoForce.TabIndex = 103
        Me.ComboBoxVideoForce.TabStop = False
        Me.ComboBoxVideoForce.Text = "0 通常再生"
        '
        'Button4
        '
        Me.Button4.Location = New System.Drawing.Point(458, 400)
        Me.Button4.Name = "Button4"
        Me.Button4.Size = New System.Drawing.Size(59, 25)
        Me.Button4.TabIndex = 105
        Me.Button4.TabStop = False
        Me.Button4.Text = "再読込"
        Me.Button4.UseVisualStyleBackColor = True
        '
        'Button8
        '
        Me.Button8.Location = New System.Drawing.Point(355, 400)
        Me.Button8.Name = "Button8"
        Me.Button8.Size = New System.Drawing.Size(102, 25)
        Me.Button8.TabIndex = 104
        Me.Button8.TabStop = False
        Me.Button8.Text = "profile.txt(*)"
        Me.Button8.UseVisualStyleBackColor = True
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(150, 690)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(90, 12)
        Me.Label16.TabIndex = 106
        Me.Label16.Text = "最終チェック日時："
        '
        'LabelVersionCheckDate
        '
        Me.LabelVersionCheckDate.AutoSize = True
        Me.LabelVersionCheckDate.Location = New System.Drawing.Point(242, 690)
        Me.LabelVersionCheckDate.Name = "LabelVersionCheckDate"
        Me.LabelVersionCheckDate.Size = New System.Drawing.Size(13, 12)
        Me.LabelVersionCheckDate.TabIndex = 107
        Me.LabelVersionCheckDate.Text = "　"
        '
        'CheckBoxVersionCheck
        '
        Me.CheckBoxVersionCheck.AutoSize = True
        Me.CheckBoxVersionCheck.Checked = True
        Me.CheckBoxVersionCheck.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckBoxVersionCheck.Location = New System.Drawing.Point(7, 689)
        Me.CheckBoxVersionCheck.Name = "CheckBoxVersionCheck"
        Me.CheckBoxVersionCheck.Size = New System.Drawing.Size(127, 16)
        Me.CheckBoxVersionCheck.TabIndex = 108
        Me.CheckBoxVersionCheck.TabStop = False
        Me.CheckBoxVersionCheck.Text = "アップデートチェックする"
        Me.CheckBoxVersionCheck.UseVisualStyleBackColor = True
        '
        'LabelVersionWarning
        '
        Me.LabelVersionWarning.AutoSize = True
        Me.LabelVersionWarning.Font = New System.Drawing.Font("MS UI Gothic", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.LabelVersionWarning.ForeColor = System.Drawing.Color.Red
        Me.LabelVersionWarning.Location = New System.Drawing.Point(377, 690)
        Me.LabelVersionWarning.Name = "LabelVersionWarning"
        Me.LabelVersionWarning.Size = New System.Drawing.Size(143, 12)
        Me.LabelVersionWarning.TabIndex = 109
        Me.LabelVersionWarning.Text = "アップデートを行ってください"
        Me.LabelVersionWarning.Visible = False
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(530, 705)
        Me.Controls.Add(Me.LabelVersionWarning)
        Me.Controls.Add(Me.CheckBoxVersionCheck)
        Me.Controls.Add(Me.LabelVersionCheckDate)
        Me.Controls.Add(Me.Label16)
        Me.Controls.Add(Me.Button4)
        Me.Controls.Add(Me.Button8)
        Me.Controls.Add(Me.ComboBoxVideoForce)
        Me.Controls.Add(Me.ComboBoxRezFormOrCombo)
        Me.Controls.Add(Me.Button7)
        Me.Controls.Add(Me.ButtonCopy2Clipboard)
        Me.Controls.Add(Me.Label15)
        Me.Controls.Add(Me.ComboBoxHLSorHTTP)
        Me.Controls.Add(Me.Label14)
        Me.Controls.Add(Me.Label13)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.TextBoxPASS)
        Me.Controls.Add(Me.TextBoxID)
        Me.Controls.Add(Me.Button3)
        Me.Controls.Add(Me.TextBoxUdpOpt3)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.LabelStream)
        Me.Controls.Add(Me.CheckBoxShowConsole)
        Me.Controls.Add(Me.ButtonFILEROOT)
        Me.Controls.Add(Me.TextBoxFILEROOT)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Button6)
        Me.Controls.Add(Me.Button5)
        Me.Controls.Add(Me.ButtonHLSoption)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.ComboBoxResolution)
        Me.Controls.Add(Me.textBoxUdpPort)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.ComboBoxServiceID)
        Me.Controls.Add(Me.ComboBoxBonDriver)
        Me.Controls.Add(Me.ButtonBonDriverPath)
        Me.Controls.Add(Me.TextBoxBonDriverPath)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.TextBoxLog)
        Me.Controls.Add(Me.ButtonWWWROOT)
        Me.Controls.Add(Me.TextBoxWWWroot)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.TextBoxChSpace)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.ComboBoxNum)
        Me.Controls.Add(Me.ButtonWebStop)
        Me.Controls.Add(Me.ButtonWebStart)
        Me.Controls.Add(Me.ButtonMovieStop)
        Me.Controls.Add(Me.ButtonMovieStart)
        Me.Controls.Add(Me.buttonHlsAppPath)
        Me.Controls.Add(Me.buttonUdpAppPath)
        Me.Controls.Add(Me.textBoxHlsOpt2)
        Me.Controls.Add(Me.labelHlsOpt2)
        Me.Controls.Add(Me.textBoxHlsApp)
        Me.Controls.Add(Me.labelHlsApp)
        Me.Controls.Add(Me.textBoxUdpApp)
        Me.Controls.Add(Me.labelUdpApp)
        Me.Controls.Add(Me.textHttpPortNumber)
        Me.Controls.Add(Me.labelPortNuber)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximumSize = New System.Drawing.Size(546, 744)
        Me.MinimumSize = New System.Drawing.Size(200, 50)
        Me.Name = "Form1"
        Me.ShowInTaskbar = False
        Me.Text = "TvRemoteViewer_VB"
        Me.WindowState = System.Windows.Forms.FormWindowState.Minimized
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents buttonHlsAppPath As System.Windows.Forms.Button
    Private WithEvents buttonUdpAppPath As System.Windows.Forms.Button
    Private WithEvents textBoxHlsOpt2 As System.Windows.Forms.TextBox
    Private WithEvents labelHlsOpt2 As System.Windows.Forms.Label
    Private WithEvents textBoxHlsApp As System.Windows.Forms.TextBox
    Private WithEvents labelHlsApp As System.Windows.Forms.Label
    Private WithEvents textBoxUdpApp As System.Windows.Forms.TextBox
    Private WithEvents labelUdpApp As System.Windows.Forms.Label
    Private WithEvents textHttpPortNumber As System.Windows.Forms.TextBox
    Private WithEvents labelPortNuber As System.Windows.Forms.Label
    Friend WithEvents ButtonMovieStart As System.Windows.Forms.Button
    Friend WithEvents ButtonMovieStop As System.Windows.Forms.Button
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents ButtonWebStop As System.Windows.Forms.Button
    Friend WithEvents ButtonWebStart As System.Windows.Forms.Button
    Friend WithEvents ComboBoxNum As System.Windows.Forms.ComboBox
    Private WithEvents textBoxUdpPort As System.Windows.Forms.TextBox
    Private WithEvents Label2 As System.Windows.Forms.Label
    Private WithEvents TextBoxChSpace As System.Windows.Forms.TextBox
    Private WithEvents Label3 As System.Windows.Forms.Label
    Private WithEvents ButtonWWWROOT As System.Windows.Forms.Button
    Private WithEvents TextBoxWWWroot As System.Windows.Forms.TextBox
    Private WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents TextBoxLog As System.Windows.Forms.TextBox
    Private WithEvents Label5 As System.Windows.Forms.Label
    Private WithEvents Label6 As System.Windows.Forms.Label
    Private WithEvents Label7 As System.Windows.Forms.Label
    Private WithEvents ButtonBonDriverPath As System.Windows.Forms.Button
    Private WithEvents TextBoxBonDriverPath As System.Windows.Forms.TextBox
    Private WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents ComboBoxBonDriver As System.Windows.Forms.ComboBox
    Friend WithEvents ComboBoxServiceID As System.Windows.Forms.ComboBox
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Private WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents ComboBoxResolution As System.Windows.Forms.ComboBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents ButtonHLSoption As System.Windows.Forms.Button
    Friend WithEvents Button5 As System.Windows.Forms.Button
    Friend WithEvents Button6 As System.Windows.Forms.Button
    Private WithEvents Label10 As System.Windows.Forms.Label
    Private WithEvents ButtonFILEROOT As System.Windows.Forms.Button
    Private WithEvents TextBoxFILEROOT As System.Windows.Forms.TextBox
    Private WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents NotifyIcon1 As System.Windows.Forms.NotifyIcon
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents quit As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CheckBoxShowConsole As System.Windows.Forms.CheckBox
    Private WithEvents LabelStream As System.Windows.Forms.Label
    Private WithEvents TextBoxUdpOpt3 As System.Windows.Forms.TextBox
    Private WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Private WithEvents TextBoxID As System.Windows.Forms.TextBox
    Private WithEvents TextBoxPASS As System.Windows.Forms.TextBox
    Private WithEvents Label12 As System.Windows.Forms.Label
    Private WithEvents Label13 As System.Windows.Forms.Label
    Private WithEvents Label14 As System.Windows.Forms.Label
    Private WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents ComboBoxHLSorHTTP As System.Windows.Forms.ComboBox
    Friend WithEvents ButtonCopy2Clipboard As System.Windows.Forms.Button
    Friend WithEvents SeekMethodList As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Button7 As System.Windows.Forms.Button
    Friend WithEvents ComboBoxRezFormOrCombo As System.Windows.Forms.ComboBox
    Friend WithEvents ComboBoxVideoForce As System.Windows.Forms.ComboBox
    Friend WithEvents Button4 As System.Windows.Forms.Button
    Friend WithEvents Button8 As System.Windows.Forms.Button
    Private WithEvents Label16 As System.Windows.Forms.Label
    Private WithEvents LabelVersionCheckDate As System.Windows.Forms.Label
    Friend WithEvents CheckBoxVersionCheck As System.Windows.Forms.CheckBox
    Private WithEvents LabelVersionWarning As System.Windows.Forms.Label

End Class
