Imports System
Imports System.Text.RegularExpressions      '文字列抽出のため正規表現を使う。

Module モジュール_ISO
    Public mplayer4ISOPath As String = System.AppDomain.CurrentDomain.BaseDirectory & "\mplayer-ISO.exe"

    Public Structure ISO_para_structure
        Public enabled As Integer
        Public startoffset As Integer
        Public audioLang As String
        Public audioTrackNum As Integer
        Public subLang As String
        Public subTrackNum As Integer
    End Structure

    Private Sub TEST_ISO_Main()
        'ISO情報読み取り用MPlayerのパス。INIファイル等で設定しても良いが、最初からTVRemoteルートフォルダに入れて配布するのがお勧め
        '以下は端末からリクエストされるパラメータの例
        Dim dvdFileName = "Y:\6\movie\007 スペクター ダニエル・クレイグ  クリストフ・ヴァルツ.ISO"
        Dim startoffset As Integer = 1500
        Dim audioLang = "ja"
        Dim audioTrackNum As Integer = 1
        Dim subLang = "ja"
        Dim subTrackNum As Integer = 0
        '以上が端末から送られる指定値とする。

        Dim resultInfo As Object
        If mplayer4ISOPath <> "" And dvdFileName <> "" Then
            'DVD情報取得
            resultInfo = DVDInfo(dvdFileName)
            If resultInfo("RC") = 0 Then
                Dim vlcpath = exepath_ISO_VLC
                Dim trackID = resultInfo("MAINTITLE")
                Dim startcommand = vlcpath
                Dim startTimeParam = ""
                If startoffset > 0 Then      '開始オフセットが定義されている場合セット
                    startTimeParam = " --start-time " & startoffset
                End If
                Dim audioParam = ""         '音声が指定されていれば、言語コード→トラック番号の優先度でセット 
                Dim aaa = Array.IndexOf(resultInfo("AUDIO"), "xxx")
                If audioLang <> "" And Array.IndexOf(resultInfo("AUDIO"), audioLang) >= 0 Then
                    audioParam = " --audio-language=" & audioLang
                ElseIf audioTrackNum >= 0 And audioTrackNum <= UBound(resultInfo("AUDIO")) Then
                    audioParam = " --audio-track=" & audioTrackNum
                End If
                Dim subParam = ""           '字幕が指定されていれば、言語コード→トラック番号の優先度でセット
                If resultInfo("SUBFLG") Then '但しそもそも字幕トラックが無ければ何も指定しない。
                    If subLang <> "" And Array.IndexOf(resultInfo("SUBLANG"), subLang) >= 0 Then
                        subParam = " --sub-language=" & subLang
                    ElseIf subTrackNum >= 0 And subTrackNum <= UBound(resultInfo("SUBLANG")) Then
                        subParam = " --sub-track=" & subTrackNum
                    End If
                End If
                'Dim startparam = "-I dummy --dummy-quiet dvdsimple:///""" & dvdFileName & """/#" & trackID & startTimeParam & " --stop-time " & resultInfo("DURATION") & " --no-repeat vlc://quit" & audioParam & subParam
                '本来stop-timeは上記の指定にするが、ここではデモ用に20秒だけ再生する設定
                Dim startparam = "-I dummy --dummy-quiet dvdsimple:///""" & dvdFileName & """/#" & trackID & startTimeParam & " --stop-time " & (startoffset + 20) & " --no-repeat vlc://quit" & audioParam & subParam
                Dim psInfo As New ProcessStartInfo()
                psInfo.FileName = vlcpath
                '実行コマンド
                psInfo.Arguments = startparam
                '起動
                Dim p As Process = Process.Start(psInfo)
                p.Close()
            End If

        End If
    End Sub

    Public Function DVDInfo(ByVal isoFileName As String, Optional ByVal selTitle As Integer = -1) As Object
        'DVDストリーミング用情報取得る関数。冒頭にある正規表現用ライブラリのImportgaが必要です。
        '通常は DVDInfo "ISOファイル名のフルパス"　で呼び出す。第二パラメータは内部的に使うのでセットしない。
        '返り値は以下をキーとするDictionary型連想配列
        '   キー      値の型     値
        '   RC          Integer  0 なら情報取得成功（最低限MAINTITLE と DURATION がセットされる。） -1 なら情報取得失敗
        '   MAINTITLE   Integer  本編のタイトル番号　1～
        '   DURATION    Integer  本編の長さ（秒）   
        '   AUDIO       String() オーディオトラックの構成。トラック番号0番～n番の言語コード(ja, en など）が入る
        '   SUBFLG      Boolean   字幕トラックが存在するならTrue しなければFalse. Falseの時はいかなる場合も字幕を指定してはならない。(ffmpegがエラーする。)。
        '   SUBLANG     String()  字幕トラックの構成。トラック番号0番～n番の言語コード(ja, enなど)が入る  、存在しなくても0番にNothingが入る
        '   CHAPTER     String  　チャプタが存在すれば、.chapterレコード形式のチャプタ情報が入る。存在しなければ""
        Dim dic1 As New Dictionary(Of String, Object)
        Dim titleNum As Integer = selTitle
        If selTitle <= 0 Then
            titleNum = 1        '最初はタイトル1番の情報を読み出す。これで最低限メインタイトルがどれかの情報が取り出せる。
        End If
        Dim psInfo As New ProcessStartInfo()
        Dim results As String, workStr As String
        Dim ii As Integer, ij As Integer, ptnNo As Integer
        Dim it1 As Integer
        Dim s1 As String = ""
        Dim s2 As String = ""
        Dim f1 As Single, f2 As Single
        Dim ptnStr() As String = {"^ID_DVD_TITLE_(?<id>.+)_LENGTH=(?<arg>.+)",
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
        Dim chapters As String
        chapters = ""
        'MPlayer呼出し
        psInfo.FileName = mplayer4ISOPath
        psInfo.UseShellExecute = False
        psInfo.RedirectStandardOutput = True
        psInfo.RedirectStandardInput = False
        psInfo.CreateNoWindow = True
        '実行コマンド）
        psInfo.Arguments = "-vo null -ao null -frames 0 -identify dvd://" & titleNum & " -dvd-device " & """" & isoFileName & """"
        '起動
        Dim p As Process = Process.Start(psInfo)
        '出力を読み取る
        results = p.StandardOutput.ReadToEnd()
        'プロセス終了まで待機する
        p.WaitForExit()
        p.Close()
        '出力された文字列を分析
        Dim re As New System.Text.RegularExpressions.Regex("\s+")
        Dim resultArr
        resultArr = Split(results, vbCrLf)
        For ii = 0 To UBound(resultArr)
            workStr = Trim(re.Replace(resultArr(ii), " "))
            If workStr <> "" Then
                ptnNo = -1
                For ij = 0 To UBound(re1)
                    If Not (re1(ij) Is Nothing) Then
                        regmatch = re1(ij).Match(workStr)
                        If regmatch.Success Then
                            ptnNo = ij
                            Exit For
                        End If
                    End If
                Next
                If ptnNo >= 0 Then
                    s2 = Trim(regmatch.Groups("arg").Value)
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
        If (titleNum = mainTitle) Or (selTitle > 0) Then '問い合わせたタイトルNoがメインタイトルと等しいか二回目の問い合わせなら、結果をそのまま返す。
            If mainTitle < 0 Or titleLength < 0 Then
                dic1("RC") = -1
            Else
                dic1("RC") = 0
            End If
            dic1("MAINTITLE") = mainTitle
            dic1("DURATION") = CInt(Math.Floor(titleLength))
            dic1("AUDIO") = audioLang
            dic1("SUBFLG") = subFlg
            dic1("SUBID") = subtitleID
            dic1("SUBLANG") = subtitleLang
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
            dic1("CHAPTER") = chapters
            '最終的な返り値をセット
            DVDInfo = dic1
        ElseIf mainTitle > 0 Then
            DVDInfo = DVDInfo(isoFileName, mainTitle)   'メインタイトルが1でなければ正しいタイトル番号をセットし直して自分自身を再度呼び出す。
        Else
            dic1("RC") = -1
            DVDInfo = dic1
        End If
    End Function
End Module
