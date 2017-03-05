'Vladi氏提供のDVD関連クラス群です。感謝感謝

Imports System                              'Dumpおよびサムネイルを別スレッドで呼ぶため
Imports System.Threading                    '同上
Imports System.Text.RegularExpressions      '文字列抽出のため正規表現を使う。
Imports System.Security.Cryptography        'Dumpファイル名をハッシュで生成するため。
Imports System.Text

Public Delegate Sub Callback1(ByRef dvdInstance As DVDClass)        'コールバックの雛形なのでこれも忘れずに

'
'   ここからDVD操作関連のクラス定義です。
'
Public Class DVDClass
    '公開プロパティエリア
    Public status As Boolean        'DVDオブジェクト作成結果
    Public statusThumb As Boolean   'サムネイル作成結果
    Public streamNum As Integer     'ストリーム番号
    Public isoFileName As String    '再生するISOファイル名(フルパス)
    Public dumpProgress As Integer  'DVDダンプの進捗%。（0～100なら進行中。101以上ならダンプ完了）
    Public dumpFileName As String   'ダンプファイル名（フルパス）
    '　サムネイル用
    Public thumbFolder As String   'サムネイル出力先フォルダ
    Public thumbName As String     'サムネイルファイル名 (.jpgまで指定)　既に同名のファイルが存在したら上書きする。
    '　コンストラクタでセットされる公開プロパティ
    Public titleNum As Integer      '再生するISO本編のタイトル番号。通常はコンストラクタでセットされるが、手動で変更する事も可能。
    Public dvdrc As Integer         '0 なら情報取得成功（最低限MAINTITLE と DURATION がセットされる。） -1 なら情報取得失敗
    Public duration As Integer      '本編の長さ（秒）   
    Public audio As String()        'オーディオトラックの構成。トラック番号0番～n番の言語コード(ja, en など）が入る
    Public subFlg As Boolean        '字幕トラックが存在するならTrue しなければFalse. Falseの時はいかなる場合も字幕を指定してはならない。(ffmpegがエラーする。)。
    Public subLang As String()      '字幕トラックの構成。トラック番号0番～n番の言語コード(ja, enなど)が入る  、存在しなくても0番にNothingが入る
    Public subID As Integer()       '字幕トラック番号のSID変換用配列。トラック番号(0,1,2・・)に対応するSID(1,3,5・・)が入る。
    Public chapters As String       'チャプタが存在すれば、.chapterレコード形式のチャプタ情報が入る。存在しなければ""

    'Dumpの後コールバックで呼ばれる処理を予めセットする。ffmpeg またはQSVEncによるエンコードを想定。Dumpが失敗したり、中止された場合は呼ばなれい。
    Public EncodeAfterDump As Callback1
    'サムネイル作成後コールバックで呼ばれる処理を処理を予めセットする。端末へサムネイル作成が終わった旨の返答処理を想定
    Public PostThumbProc As Callback1

    '■追加　コールバック時にパラメータを置換するため
    Public ISO_hlsOpt As String
    Public ISO_audioLang As String
    Public ISO_audioTrackNum As Integer
    Public ISO_subLang As String
    Public ISO_subTrackNum As Integer
    Public ISO_seek As Integer

    '以下は内部で使用
    Private mplayePath As String
    '■修正　Private ffmpegPath As String
    Public ffmpegPath As String
    Private dumpPath As String       'Dumpファイルを置くフォルダ(フルパス)
    Private p1 As Process, p2 As Process       'プロセス制御用
    Private retcode1 As Integer     '内部で利用するリターンコード
    Private re1 As Object, re2 As Object
    Private regmatch As Match
    Private useDump As Boolean
    'スレッド制御用
    Private thread1 As Thread, thread2 As Thread
    'サムネイル用内部変数
    Private timesec As Integer      'サムネイル指定位置(秒）
    Private width As Integer        'サムネイル幅　省略時は640
    Private height As Integer       'サムネイル高さ　省略時は360
    Private forceM As Integer       '1ならDumpが存在していてもISOからMPlayerでサムネイルを作成する。
    'コンストラクタ
    '■修正　Public Sub New(ByVal isoFile As String, ByVal ffmpeg As String, ByVal mplayer As String, ByVal streamID As Integer, ByVal work As String)
    Public Sub New(ByVal isoFile As String, ByVal ffmpeg As String, ByVal mplayer As String, ByVal streamID As Integer, ByVal work As String, ByVal hlsOpt_str As String, ByVal audioLang_str As String, ByVal audioTrackNum_str As Integer, ByVal subLang_str As String, ByVal subTrackNum_str As Integer, ByVal seek_str As Integer)
        status = True
        isoFileName = isoFile.Trim("""", " ")
        ffmpegPath = ffmpeg.Trim("""", " ")
        mplayePath = mplayer.Trim("""", " ")
        streamNum = streamID
        dumpPath = work.Trim("""", " ").TrimEnd("\")
        If (isoFileName = "") Or (ffmpegPath = "") Or (mplayePath = "") Or (streamNum < 1) Or (dumpPath = "") Then
            status = False
            Return
        End If
        titleNum = 1
        dumpFileName = dumpPath & "\" & GetDumpName(isoFileName) & ".VOB"
        dumpProgress = 0
        retcode1 = 0
        'Dumpメッセージパターン定義
        re1 = New Regex("^dump:.*\(~(?<arg>.+)%.*\).*", RegexOptions.IgnoreCase)
        re2 = New Regex(".*End of file.*", RegexOptions.IgnoreCase)
        'パターン定義終わり
        status = DVDInfo(Me)
        '■追加
        ISO_hlsOpt = hlsOpt_str
        ISO_audioLang = audioLang_str
        ISO_audioTrackNum = audioTrackNum_str
        ISO_subLang = subLang_str
        ISO_subTrackNum = subTrackNum_str
        ISO_seek = seek_str
    End Sub
    '
    'DVDのダンプを出力し、終了後指定のエンコードプロセスを呼び出す関連メソッド群
    'ダンプStart用メソッド
    Public Sub Start()
        dumpProgress = 0
        retcode1 = 0
        'ISOファイルのチェック。存在しなければ何もせずに終了
        If System.IO.File.Exists(isoFileName) Then
            If Not System.IO.File.Exists(dumpFileName) Then     'ダンプファイルが存在しない→ダンプファイルを新規作成
                thread1 = New Thread(New ThreadStart(AddressOf ThreadDumpDVD))
                thread1.Start()
            Else    '既にDumpファイルが存在するので即座に正常終了させ、後続処理へ
                retcode1 = 0
                dumpProgress = 200
                UpdateTimestamp(isoFileName, dumpPath)
                '定義されていれば、コールバックで次処理を呼び出す
                If Not EncodeAfterDump Is Nothing Then
                    EncodeAfterDump(Me)
                End If
            End If
        Else
            retcode1 = -1
        End If
    End Sub
    'Dumpスレッド強制終了するメソッド
    Public Sub AbortDump()
        If Not p1 Is Nothing Then
            retcode1 = -1
            p1.Kill()
        End If
    End Sub
    'Dumpファイルを敢えて削除するメソッド
    Public Sub DeleteDump()
        System.IO.File.Delete(dumpFileName)
        dumpProgress = 0
    End Sub
    'Dumpスレッド本体
    Private Sub ThreadDumpDVD()
        'MPlayerでDVDのダンプをおこなう。
        p1 = New System.Diagnostics.Process()
        'MPlayerの実行ファイルを指定
        p1.StartInfo.FileName = mplayePath
        '標準出力を受け取り、進捗が判るようにする
        p1.StartInfo.UseShellExecute = False
        p1.StartInfo.RedirectStandardOutput = True
        p1.StartInfo.RedirectStandardInput = False
        p1.StartInfo.CreateNoWindow = True
        'OutputDataReceivedイベントハンドラを追加
        AddHandler p1.OutputDataReceived, AddressOf p_OutputDataReceived
        '実行コマンド
        p1.StartInfo.Arguments = "dvd://" & titleNum & " -dvd-device " & """" & isoFileName & """ -dumpstream -dumpfile """ & dumpFileName & "." & streamNum & ".tmp"""       '一旦.temp の拡張子を付けてダンプを生成
        '起動
        p1.Start()
        '非同期で出力の読み取りを開始
        p1.BeginOutputReadLine()
        p1.WaitForExit()
        'Dumpプロセスが終了した。
        If Not p1 Is Nothing Then
            p1.Close()
        End If
        p1 = Nothing
        If retcode1 = 0 Then
            If System.IO.File.Exists(dumpFileName) Then
                '先に同名のダンプファイルが出来ている場合を想定。その時はここで作成したダンプファイルは破棄して続行
                System.IO.File.Delete(dumpFileName & "." & streamNum & ".tmp")
                dumpProgress = 200
            Else
                '生成した.tmpファイルを本来の名前にリネーム
                System.IO.File.Move(dumpFileName & "." & streamNum & ".tmp", dumpFileName)
                dumpProgress = 200
            End If
            UpdateTimestamp(isoFileName, dumpPath)
        Else    'retcode1が0でない = 強制終了された。
            '生成途中の.tmpファイルを削除
            System.IO.File.Delete(dumpFileName & "." & streamNum & ".tmp")
            dumpProgress = -1
        End If
        'Dumpが成功していてもいなくても、とりあえず定義されていればコールバックで次処理を呼び出す
        If Not EncodeAfterDump Is Nothing Then
            EncodeAfterDump(Me)
        End If
    End Sub
    'Dumpで行が出力されるたびに呼び出されるので、進捗（dumpProgres）を読み取って更新する。
    Private Sub p_OutputDataReceived(sender As Object, _
            e As System.Diagnostics.DataReceivedEventArgs)
        If Not String.IsNullOrEmpty(e.Data) Then
            '出力された文字列を検査する
            '進捗パターンにマッチング
            regmatch = re1.Match(e.Data)
            If regmatch.Success Then
                Dim ft1 As Double = CSng(Trim(regmatch.Groups("arg").Value))
                If ft1 > 0 And ft1 <= 100 Then
                    dumpProgress = CInt(ft1)
                End If
            Else        '完了パターンにマッチング
                regmatch = re2.Match(e.Data)
                If regmatch.Success Then
                    dumpProgress = 200
                End If
            End If
        End If
    End Sub

    'サムネイルを作成し、終了後指定の処理を呼び出すメソッド群
    Public Sub DVDThumb(ByVal time As Integer, ByVal dir As String, ByVal filename As String, Optional ByVal w As Integer = 640, Optional ByVal h As Integer = 360, Optional ByVal forceM As Integer = 0)
        'MPlayerまたはffmpegを使ってDVD ISOファイルから指定位置のサムネイルを取得する。
        'time		指定位置(秒）
        'dir    	サムネイル出力先フォルダ
        'filename 	サムネイルファイル名 (.jpgまで指定)　既に同名のファイルが存在したら上書きする。
        'w     		サムネイル幅　省略時は640
        'h     		サムネイル高さ　省略時は360
        'forceM		0または省略時: Dumpファイルが存在すればffmpegでDumpファイルから、存在しなければMPlayerでISOファイルからサムネイル作成 
        '     		1: 常にMPlayerでISOファイルからサムネイルを作成。
        timesec = time
        thumbFolder = dir.Trim("""", " ").TrimEnd("\")
        thumbName = filename.Trim("""", " ")
        width = w
        height = h
        useDump = False
        If ((forceM <> 1) And System.IO.File.Exists(dumpFileName)) Then
            useDump = True    '取得元はDumpファイル
        End If
        statusThumb = 0
        If System.IO.File.Exists(isoFileName) Then
            '先行するthumbnailスレッドがあれば念のため停止
            AbortThumb()
            'threadを作成
            thread2 = New Thread(New ThreadStart(AddressOf ThreadThumbDVD))
            'Processオブジェクト1を作成
            p2 = New System.Diagnostics.Process()
            p2.StartInfo.UseShellExecute = False
            p2.StartInfo.RedirectStandardInput = False
            p2.StartInfo.RedirectStandardOutput = True
            p2.StartInfo.CreateNoWindow = True
            thread2.Start()
        Else
            statusThumb = -1
        End If
    End Sub
    'サムネイル作成スレッド本体
    Private Sub ThreadThumbDVD()
        statusThumb = 0
        '直接取得の場合(MPlayerを使う)
        If Not useDump Then
            '実行ファイル = MPlayerを指定
            p2.StartInfo.FileName = mplayePath
            '出力先フォルダはカレントフォルダ固定なので、コマンドのカレントフォルダを出力先に指定する。、
            p2.StartInfo.WorkingDirectory = thumbFolder
            '実行コマンド）
            p2.StartInfo.Arguments = "-ss " & timesec & " -frames 1 -vf framestep=I,scale=" & width & ":" & height & " -vo jpeg -ao null dvd://" & titleNum & " -dvd-device " & """" & isoFileName & """"
            '起動
            p2.Start()
            'プロセス終了まで待機する
            p2.WaitForExit()
            '指定ファイル名が00000001.jpg以外の時に以下の処理
            If (statusThumb = 0) And (thumbName.ToLower() <> "00000001.jpg") Then
                '生成した00000001.jpgを指定ファイル名にリネーム
                If System.IO.File.Exists(thumbFolder & "\" & "00000001.jpg") Then
                    '既に指定jpegファイルが存在していたら削除
                    System.IO.File.Delete(thumbFolder & "\" & thumbName)
                    System.IO.File.Move(thumbFolder & "\" & "00000001.jpg", thumbFolder & "\" & thumbName)
                Else
                    'ファイルがまだ出来ていない時は0.5秒間（500ミリ秒）wait後、リトライ
                    System.Threading.Thread.Sleep(500)
                    If System.IO.File.Exists(thumbFolder & "\" & "00000001.jpg") Then
                        System.IO.File.Delete(thumbFolder & "\" & thumbName)
                        System.IO.File.Move(thumbFolder & "\" & "00000001.jpg", thumbFolder & "\" & thumbName)
                    End If
                End If
            End If
        Else    '取得元がDumpファイルのとき
            '実行ファイル = ffmpegを指定
            p2.StartInfo.FileName = ffmpegPath
            '実行コマンド）
            p2.StartInfo.Arguments = "-y -ss " & timesec & " -i """ & dumpFileName & """ -vframes 1 -f image2 -s " & width & "x" & height & " """ & thumbFolder & "\" & thumbName & """"
            '起動
            p2.Start()
            'プロセス終了まで待機する
            p2.WaitForExit()
        End If
        If Not p2 Is Nothing Then
            p2.Close()
        End If
        p2 = Nothing
        '終了コードに関係なく定義されていれば、コールバックで次処理を呼び出す
        If Not PostThumbProc Is Nothing Then
            PostThumbProc(Me)
        End If
    End Sub
    'サムネイルスレッドを強制終了するメソッド
    Public Sub AbortThumb()
        If Not p2 Is Nothing Then
            p2.Kill()
            statusThumb = -1
        End If
    End Sub
    '
    'DVD関連の静的関数（Shared）
    '
    '入力したフルパスファイル名からダンプファイル名を返す
    Public Shared Function GetDumpName(ByVal inputFileName As String) As String
        Dim result As String
        Using md5 As MD5 = md5.Create()
            ' MD5ハッシュ値を求める
            Dim md5hash As Byte() = md5.ComputeHash(Encoding.UTF8.GetBytes(inputFileName.Trim().ToUpper()))
            ' 求めたハッシュ値を16進文字列（32字　大文字）に変換する
            result = BitConverter.ToString(md5hash).ToUpper().Replace("-", "")
        End Using
        GetDumpName = result
    End Function
    '現在の日時を14桁の数字文字列で返す
    Public Shared Function CurrentDT() As String
        Dim dtmNow As DateTime
        dtmNow = DateTime.Now
        CurrentDT = dtmNow.ToString("yyyyMMddHHmmss")
    End Function
    '指定したファイルのタイムスタンプファイルを更新（古い物があれば削除し、現時点のタイムスタンプのものを作成）
    Public Shared Sub UpdateTimestamp(fullfileNmae, dir)
        Dim hashName As String = GetDumpName(fullfileNmae)
        Dim timestamp As String = CurrentDT()
        ' ファイル名に .time.txt のファイルを列挙して削除する。
        For Each stFilePath As String In System.IO.Directory.GetFiles(dir & "\", "*." & hashName & ".time.txt")
            If System.IO.File.Exists(stFilePath) Then
                System.IO.File.Delete(stFilePath)
            End If
        Next stFilePath
        Dim sw = System.IO.File.CreateText(dir & "\" & timestamp & "." & hashName & ".time.txt")
        sw.Close()
    End Sub
    '入力が符号なし整数値であるかを判別
    Shared Function IsInt(ByVal exam As Object) As Boolean
        Dim s1 As String = CStr(exam)               '入力を一旦文字列に変換
        Return New Regex("^[0-9]+$").IsMatch(s1)    '数値のみの正規表現とのマッチで判別
    End Function
    '入力が1以上arg2以下の符号なし整数値である場合数値化して返し、それ以外の場合は1を返す。、
    Shared Function CurrentSTNum(ByVal arg1 As Object, ByVal arg2 As Integer) As Integer
        If IsInt(arg1) Then
            Dim it As Integer = CInt(arg1)
            If (it >= 1) And (it <= arg2) Then
                Return it
            Else
                Return 1
            End If
        Else
            Return 1
        End If
    End Function
    '
    'DVD情報取得関数
    '返り値がFalseの場合は正常なDVD ISOファイルではない（Blu-ray ISOなど）ので処理を続行しないこと。
    '
    Public Shared Function DVDInfo(ByRef obj As DVDClass, Optional ByVal titleID As Integer = -1) As Boolean
        Dim titleNum As Integer
        If titleID <= 0 Then
            titleNum = 1        '最初はタイトル1番の情報を読み出す。これで最低限メインタイトルがどれかの情報が取り出せる。
        Else
            titleNum = titleID
        End If
        Dim psInfo As New ProcessStartInfo()
        Dim results As String, workStr As String
        Dim ii As Integer, ij As Integer, ptnNo As Integer
        Dim it1 As Integer
        Dim s1 As String, s2 As String
        Dim f1 As Single, f2 As Single
        Dim ptnStr() As String = {".*No.*stream.*found.*to.*handle",
                                  "^ID_DVD_TITLE_(?<id>.+)_LENGTH=(?<arg>.+)",
                                  "^audio stream:(?<id>.+)format:.+language:(?<arg>.+)aid.+",
                                  "^number of audio channels on disk:(?<arg>.+)",
                                  "^subtitle \( sid \):(?<id>.+)language:(?<arg>.+)",
                                  "^number of subtitles on disk:(?<arg>.+)",
                                  "^CHAPTERS:(?<arg>.+)"}
        Dim re1() As Object
        ReDim re1(UBound(ptnStr))
        For ii = 0 To UBound(re1)
            If ptnStr(ii) <> "" Then
                re1(ii) = New Regex(ptnStr(ii), RegexOptions.IgnoreCase)
            End If
        Next
        Dim regmatch As Match
        'パターン定義終わり
        Dim mainTitle As Integer = -1
        Dim titleLength As Single = 0
        Dim audioLang(0) As String
        Dim subtitleID(0) As Integer
        Dim subFlg As Boolean = False
        Dim subtitleLang(0) As String
        Dim subtitleNum As Integer = 0
        Dim chapters As String = ""
        'MPlayer呼出し
        psInfo.FileName = obj.mplayePath
        psInfo.UseShellExecute = False
        psInfo.RedirectStandardOutput = True
        psInfo.RedirectStandardInput = False
        psInfo.CreateNoWindow = True
        '実行コマンド）
        psInfo.Arguments = "-vo null -ao null -frames 0 -identify dvd://" & titleNum & " -dvd-device " & """" & obj.isoFileName & """"
        '起動
        Dim p As Process = Process.Start(psInfo)
        '出力を読み取る
        results = p.StandardOutput.ReadToEnd()
        'プロセス終了まで待機する
        p.WaitForExit()
        If Not p Is Nothing Then
            p.Close()
        End If
        '出力された文字列を分析
        Dim re As New System.Text.RegularExpressions.Regex("\s+")
        Dim resultArr
        resultArr = Split(results, vbCrLf)
        For ii = 0 To UBound(resultArr)
            workStr = Trim(re.Replace(resultArr(ii), " "))
            If workStr <> "" Then
                ptnNo = -2
                For ij = 0 To UBound(re1)
                    If Not (re1(ij) Is Nothing) Then
                        regmatch = re1(ij).Match(workStr)
                        If regmatch.Success Then
                            ptnNo = ij - 1
                            Exit For
                        End If
                    End If
                Next
                If ptnNo >= 0 Then
                    s2 = Trim(regmatch.Groups("arg").Value)
                ElseIf ptnNo = -1 Then    '異常終了メッセージを検出した
                    obj.dvdrc = -1
                    Return False
                End If
                Select Case ptnNo
                    Case Is < 0
                    Case 0
                        it1 = CInt(Trim(regmatch.Groups("id").Value))
                        If it1 > 0 Then
                            f1 = CSng(s2)
                            If f1 > titleLength Then
                                titleLength = f1
                                mainTitle = it1
                            End If
                        End If
                    Case 1
                        it1 = CInt(Trim(regmatch.Groups("id").Value))
                        If it1 >= 0 And it1 < 100 Then
                            If UBound(audioLang) < it1 Then
                                ReDim Preserve audioLang(it1)
                            End If
                            audioLang(it1) = s2
                        End If
                    Case 2
                    Case 3
                        it1 = CInt(Trim(regmatch.Groups("id").Value))
                        If it1 >= 0 Then
                            subFlg = True
                            ReDim Preserve subtitleID(subtitleNum)
                            ReDim Preserve subtitleLang(subtitleNum)
                            subtitleID(subtitleNum) = it1
                            subtitleLang(subtitleNum) = s2
                            subtitleNum = subtitleNum + 1
                        End If
                    Case 4
                    Case 5
                        chapters = s2
                End Select
            End If
        Next
        '返り値をセット
        If (titleNum = mainTitle) Or (titleID > 0) Then '問い合わせたタイトルNoがメインタイトルと等しいか二回目の問い合わせなら、結果をそのまま返す。
            If mainTitle < 1 Or titleLength < 0 Then
                obj.dvdrc = -1
                Return False
            End If
            obj.dvdrc = 0
            obj.titleNum = mainTitle
            obj.duration = CInt(Math.Floor(titleLength))
            obj.audio = audioLang
            obj.subFlg = subFlg
            obj.subID = subtitleID
            obj.subLang = subtitleLang
            'チャプタ文字列の変換
            If chapters <> "" Then
                Dim chaptArrayI(0) As Integer
                chaptArrayI(0) = 0
                Dim chaptArrayS() As String = Split(chapters, ",")
                Dim chaptCount As Integer = 0
                For ii = 0 To UBound(chaptArrayS)
                    f2 = 0
                    s1 = Trim(chaptArrayS(ii))
                    If s1 <> "" Then
                        Dim timeArray = Split(s1, ":")
                        For ij = 0 To UBound(timeArray)
                            s2 = Trim(timeArray(ij))
                            If s2 <> "" Then
                                f1 = CSng(s2)
                                f2 = f2 * 60 + f1
                            Else
                                f2 = f2 * 60
                            End If
                        Next
                    End If
                    If f2 >= 0 And f2 <= titleLength Then   'タイトル長に収まっていることを確認
                        If UBound(chaptArrayI) < chaptCount Then
                            ReDim Preserve chaptArrayI(chaptCount)
                        End If
                        chaptArrayI(chaptCount) = Math.Floor(f2 * 10)  '100ms単位の整数に成形して格納
                        chaptCount = chaptCount + 1
                    End If
                Next
                Array.Sort(chaptArrayI)
                For ii = UBound(chaptArrayI) To 1 Step -1
                    If chaptArrayI(ii) = chaptArrayI(ii - 1) Then
                        If ii < UBound(chaptArrayI) Then
                            For ij = ii + 1 To UBound(chaptArrayI)
                                chaptArrayI(ij - 1) = chaptArrayI(ij)
                            Next
                        End If
                        ReDim Preserve chaptArrayI(UBound(chaptArrayI) - 1)     '同じ値が連続する場合切り詰める
                    End If
                Next
                chapters = "c-"
                For ii = 0 To UBound(chaptArrayI)
                    chapters = chapters & chaptArrayI(ii) & "d-"
                Next
                chapters = chapters & "c"
            End If
            obj.chapters = chapters
            '最終的な返り値をセット
        ElseIf mainTitle > 0 Then
            Return DVDInfo(obj, mainTitle)   'メインタイトルが1でなければ正しいタイトル番号をセットし直して自分自身を再度呼び出す。
        Else
            obj.dvdrc = -1
            Return False
        End If
        Return True
    End Function
    'トラック番号または言語コードに対応したSubIDを返す。
    Public Function GetSubID(ByVal trackID As Object) As Integer
        If Not subFlg Then Return -1 '字幕トラックが存在しなければ常に-1を返す
        Dim ii As Integer
        If IsInt(trackID) Then
            ii = CInt(trackID)
            If (ii < 0) Or (ii > UBound(subID)) Then Return -1 '範囲外の数字なので-1
            Return subID(ii)
        Else
            ii = Array.IndexOf(subLang, trackID)
            If (ii < 0) Or (ii > UBound(subID)) Then Return -1 '範囲外の数字なので-1
            Return subID(ii)
        End If
        Return -1
    End Function
    'トラック番号または言語コードに対応したAudioIDを返す。
    Public Function GetAudioID(ByVal trackID As Object) As Integer
        Dim ii As Integer
        If IsInt(trackID) Then
            ii = CInt(trackID)
            If (ii < 0) Or (ii > UBound(audio)) Then Return -1 '範囲外の数字なので-1
            Return ii
        Else
            ii = Array.IndexOf(audio, trackID)
            If (ii < 0) Or (ii > UBound(audio)) Then Return -1 '範囲外の数字なので-1
            Return ii
        End If
        Return -1
    End Function
    '
    ' Dump用キャッシュフォルダのクリーンアップ
    '
    Public Shared Sub CleanupDumpCache(ByVal dumpPath As String, ByVal maxNum As Integer, Optional ByVal InitialFlg As Boolean = False)
        Dim workString As String() = System.IO.Directory.GetFiles(dumpPath & "\", "*")
        Dim regmatch As Match
        Dim s1 As String, s2 As String, s3 As String
        Dim re1 As Object = New Regex("^(?<name>[^\.]{32,32})\.VOB$", RegexOptions.IgnoreCase)
        Dim re2 As Object = New Regex("^(?<time>[0-9]+)\.(?<name>[^\.]{32,32})\.time\.txt$", RegexOptions.IgnoreCase)
        Dim re3 As Object = New Regex("^[^\.]{32,32}\.VOB\.[0-9]+\.tmp$", RegexOptions.IgnoreCase)
        Dim ii As Integer
        Dim timeString = CurrentDT()
        Dim dic1 As New Dictionary(Of String, Object)
        Dim deleteList = New ArrayList(), addList = New ArrayList()
        For ii = 0 To UBound(workString)    'まずDumpファイルの一覧と索引を作成
            workString(ii) = workString(ii).Trim(" ")
            s3 = workString(ii).Substring((dumpPath & "\").Length)
            regmatch = re1.Match(s3)   'Dumpファイルのマッチング
            If regmatch.Success Then
                Dim st1 = New DumpItem
                st1.dumpFile = workString(ii)
                st1.name = Trim(regmatch.Groups("name").Value)
                dic1(st1.name) = st1
            End If
        Next
        For ii = 0 To UBound(workString)    '次にタイムスタンプファイルを検索し、上記で作成した索引に入れる。
            s3 = workString(ii).Substring((dumpPath & "\").Length)
            regmatch = re2.Match(s3)   'タイムスタンプファイルのマッチング
            If regmatch.Success Then
                s1 = Trim(regmatch.Groups("time").Value)
                s2 = Trim(regmatch.Groups("name").Value)
                If dic1.ContainsKey(s2) Then
                    If dic1(s2).timeStamp Is Nothing Then
                        dic1(s2).timeFile = workString(ii)
                        dic1(s2).timeStamp = s1
                    Else    'タイムスタンプファイルに重複があれば、古い方を削除する。
                        If s1.CompareTo(dic1(s2).timeStamp) > 0 Then
                            deleteList.Add(dic1(s2).timeFile)
                            dic1(s2).timeFile = workString(ii)
                            dic1(s2).timeStamp = s1
                        Else
                            deleteList.Add(workString(ii))
                        End If
                    End If
                Else    'タイムスタンプファイルがあるのに本体のダンプが無い場合はタイムスタンプファイル削除
                    deleteList.Add(workString(ii))
                End If
            End If
        Next

        'タイムスタンプファイルが無い場合、現時刻付で作成する。
        For Each key In dic1.Keys
            If dic1(key).timeFile Is Nothing Then
                'addList.Add(key)                                           'ここではファイル追加の登録はしない。Dump本体を残すことが判った時点で登録する。
                'dic1(key).timeFile = timeString & "." & key & ".time.txt"  'ファイルが無い事はここがNothingになっていることで判別
                dic1(key).timeStamp = timeString
            End If
        Next
        Dim dicSize As Integer = dic1.Count
        'DVDキャッシュが指定個数を上回る場合は、古いものから削除対象にする。
        If dicSize > maxNum Then
            'ソート用の配列をペアで作る
            Dim SortIndex() As String
            ReDim SortIndex(dicSize - 1)
            ii = 0
            Dim SortObj() As Object
            ReDim SortObj(dicSize - 1)
            For Each key In dic1.Keys
                SortIndex(ii) = dic1(key).timeStamp
                SortObj(ii) = dic1(key)
                ii = ii + 1
            Next
            '日時の降順にソート
            Array.Sort(SortIndex, SortObj)
            Array.Reverse(SortObj)
            '保持分でタイムスタンプファイルの無い物は追加リストに入れる
            For ii = 0 To maxNum - 1
                If SortObj(ii).timeFile Is Nothing Then
                    addList.Add(SortObj(ii).name)
                End If
            Next
            '削除分のファイルは全て削除リストに入れる。
            For ii = maxNum To UBound(SortObj)
                If Not SortObj(ii).timeFile Is Nothing Then deleteList.Add(SortObj(ii).timeFile)
                deleteList.Add(SortObj(ii).dumpFile)
            Next
        Else
            'ダンプファイルが指定個数以内の場合でも、タイムスタンプファイルが無い場合は作成する。
            For Each key In dic1.Keys
                If dic1(key).timeFile Is Nothing Then
                    addList.Add(key)
                End If
            Next
        End If
        '起動時クリーンアップなら、tmpファイルを削除リストに加える
        If InitialFlg Then
            For ii = 0 To UBound(workString)    'tmpフォイルを検索し、削除リストに入れる
                s3 = workString(ii).Substring((dumpPath & "\").Length)
                regmatch = re3.Match(s3)   'Dumpファイルのマッチング
                If regmatch.Success Then
                    deleteList.Add(workString(ii))
                End If
            Next
        End If
        'ファイル追加のループ
        For Each hashname In addList
            Dim sw = System.IO.File.CreateText(dumpPath & "\" & timeString & "." & hashname & ".time.txt")
            sw.Close()
        Next
        'ファイル削除のループ
        For Each delfilename In deleteList
            System.IO.File.Delete(delfilename)
        Next
    End Sub


    '=============================
    ' サムネイル　シングルスレッド
    '=============================

    'サムネイルを作成するメゾッド２
    Public Sub DVDThumb2(ByVal time As Integer, ByVal dir As String, ByVal filename As String, Optional ByVal w As Integer = 640, Optional ByVal h As Integer = 360, Optional ByVal forceM As Integer = 0)
        'MPlayerまたはffmpegを使ってDVD ISOファイルから指定位置のサムネイルを取得する。
        'time		指定位置(秒）
        'dir    	サムネイル出力先フォルダ
        'filename 	サムネイルファイル名 (.jpgまで指定)　既に同名のファイルが存在したら上書きする。
        'w     		サムネイル幅　省略時は640
        'h     		サムネイル高さ　省略時は360
        'forceM		0または省略時: Dumpファイルが存在すればffmpegでDumpファイルから、存在しなければMPlayerでISOファイルからサムネイル作成 
        '     		1: 常にMPlayerでISOファイルからサムネイルを作成。

        Dim r As String = ""

        timesec = time
        thumbFolder = dir.Trim("""", " ").TrimEnd("\")
        thumbName = filename.Trim("""", " ")
        width = w
        height = h
        useDump = False
        If ((forceM <> 1) And System.IO.File.Exists(dumpFileName)) Then
            useDump = True    '取得元はDumpファイル
        End If
        statusThumb = 0
        If System.IO.File.Exists(isoFileName) Then
            'シングルスレッド
            ThreadThumbDVD2()
        Else
            r = ""
        End If

        '終了時にはサムネイルが出来上がっているはず
    End Sub
    'サムネイル作成スレッド本体２
    Private Sub ThreadThumbDVD2()
        p2 = New System.Diagnostics.Process()
        p2.StartInfo.UseShellExecute = False
        p2.StartInfo.RedirectStandardInput = False
        p2.StartInfo.RedirectStandardOutput = True
        p2.StartInfo.CreateNoWindow = True

        statusThumb = 0
        '直接取得の場合(MPlayerを使う)
        If Not useDump Or ISO_ThumbForceM = 1 Then
            '実行ファイル = MPlayerを指定
            p2.StartInfo.FileName = mplayePath
            '出力先フォルダはカレントフォルダ固定なので、コマンドのカレントフォルダを出力先に指定する。、
            p2.StartInfo.WorkingDirectory = thumbFolder
            '実行コマンド）
            p2.StartInfo.Arguments = "-ss " & timesec & " -frames 1 -vf framestep=I,scale=" & width & ":" & height & " -vo jpeg -ao null dvd://" & titleNum & " -dvd-device " & """" & isoFileName & """"
            '起動
            p2.Start()
            'プロセス終了まで待機する
            p2.WaitForExit()
            '指定ファイル名が00000001.jpg以外の時に以下の処理
            If (statusThumb = 0) And (thumbName.ToLower() <> "00000001.jpg") Then
                '生成した00000001.jpgを指定ファイル名にリネーム
                If System.IO.File.Exists(thumbFolder & "\" & "00000001.jpg") Then
                    '既に指定jpegファイルが存在していたら削除
                    System.IO.File.Delete(thumbFolder & "\" & thumbName)
                    System.IO.File.Move(thumbFolder & "\" & "00000001.jpg", thumbFolder & "\" & thumbName)
                Else
                    'ファイルがまだ出来ていない時は0.5秒間（500ミリ秒）wait後、リトライ
                    System.Threading.Thread.Sleep(500)
                    If System.IO.File.Exists(thumbFolder & "\" & "00000001.jpg") Then
                        System.IO.File.Delete(thumbFolder & "\" & thumbName)
                        System.IO.File.Move(thumbFolder & "\" & "00000001.jpg", thumbFolder & "\" & thumbName)
                    End If
                End If
            End If
        Else    '取得元がDumpファイルのとき
            '実行ファイル = ffmpegを指定
            p2.StartInfo.FileName = ffmpegPath
            '実行コマンド）
            p2.StartInfo.Arguments = "-y -ss " & timesec & " -i """ & dumpFileName & """ -vframes 1 -f image2 -s " & width & "x" & height & " """ & thumbFolder & "\" & thumbName & """"
            '起動
            p2.Start()
            'プロセス終了まで待機する
            p2.WaitForExit()
            '■サムネイル作成に失敗し、真っ黒画像ができてしまう
            '★★★デバッグ用
            log1write("==============================")
            log1write("""" & ffmpegPath & """ " & p2.StartInfo.Arguments)
            log1write("■デバッグ用（サムネイル）====")
        End If
        If Not p2 Is Nothing Then
            p2.Close()
        End If
        p2 = Nothing
    End Sub

End Class
'クリーンアップ検査用構造体。中の処理で使いますのでこれも忘れずに。
Public Class DumpItem
    Public name As String
    Public dumpFile As String
    Public timeFile As String
    Public timeStamp As String
End Class
