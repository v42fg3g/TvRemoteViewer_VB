Imports System.Text
Imports System.IO

Module モジュール_プロファイル
    'HLSアプリ　解像度インデックス　チェック用文字列
    Public hlschkstr As String = ":vlc:v:ffmpeg:f:qsvenc:qsvencc:q:qsv:piperun:p:"
    Public hlschkstr_vlc As String = ":vlc:v:"
    Public hlschkstr_ffmpeg As String = ":ffmpeg:f:"
    Public hlschkstr_qsvenc As String = ":qsvenc:qsvencc:q:qsv:"
    Public hlschkstr_piperun As String = ":piperun:p:"

    Public Function get_hlsApp_and_resolution_from_profiles(ByVal profile As String, ByVal StreamMode As Integer, ByVal hlsAppSelect As String, ByVal resolution As String, ByVal filename As String, ByVal voice As String, ByVal speed As String, ByVal hardsub_on As String, ByRef video_force_ffmpeg_temp As Integer) As String()
        'video_force_ffmpeg_tempはByRefで返す
        Dim file_ext As String = Path.GetExtension(filename)
        Dim file_instr As String = Path.GetFileName(filename)
        Dim hlsAppName As String = Path.GetFileName(hlsAppSelect)

        Dim r() As String = Nothing
        Dim j As Integer = 0
        If profiletxt.Length > 0 Then
            Dim line() As String = Split(profiletxt, vbCrLf)
            If line IsNot Nothing Then
                log1write("項目：プロファイル名   種別  HLSアプリ  解像度   拡張子 ファイル名 音声  倍速  焼込")
                For i As Integer = 0 To line.Length - 1
                    Dim sp As Integer = line(i).IndexOf(";")
                    If sp >= 0 Then
                        'コメント削除
                        line(i) = line(i).Substring(0, sp)
                    End If
                    Dim e() As String = Split(line(i), "→")
                    If e.Length = 2 Then
                        Dim result As String = ""
                        Dim src() As String = Split(e(0), vbTab)
                        Dim dst() As String = Split(e(1), vbTab)
                        If src.Length >= 9 And dst.Length >= 4 Then
                            log1write("Profile：" & line(i))
                            If isMatch_kamma(profile, src(0)) = 1 Then
                                'profileがマッチ
                                result &= "o" & vbTab
                                src(1) = src(1).Replace("ライブ", "0").Replace("ライブ", "0").Replace("ファイル", "1").Replace("ファイル再生", "1")
                                If isMatch_kamma(StreamMode, src(1)) = 1 Then
                                    'StreamModeがマッチ
                                    result &= "o" & vbTab
                                    'hlsApp名調整
                                    hlsAppName = modify_hlsAppName(hlsAppName)
                                    src(2) = modify_hlsAppName(src(2))
                                    If isMatch_kamma(hlsAppName, src(2)) = 1 Then
                                        'hlsAppNameがマッチ
                                        result &= "o" & vbTab
                                        'hlsApp & 解像度をチェック
                                        Dim rez() As String = get_resolution_and_hlsApp(Trim(resolution)) 'resolutionから解像度とhlsAppを取得
                                        Dim resolution_value As String = rez(0) '純粋な解像度文字列
                                        Dim errchk As Integer = 0
                                        If rez(1).Length > 0 Then
                                            If isMatch_kamma(rez(1), src(2)) = 0 Then 'hlsAppがマッチしているか
                                                '駄目
                                                errchk = 1
                                            End If
                                        End If
                                        If isMatch_kamma(rez(0), src(3)) = 1 And errchk = 0 Then '解像度がマッチしているか
                                            '解像度がマッチ
                                            result &= "o" & vbTab
                                            If isMatch_kamma(file_ext, src(4)) = 1 Then
                                                result &= "o" & vbTab
                                                If isMatch_file_instr(file_instr, src(5)) = 1 Then
                                                    result &= "o" & vbTab
                                                    '音声調整
                                                    src(6) = src(6).Replace("主・副", "0").Replace("主", "11").Replace("副", "12").Replace("第二音声", "4").Replace("第二", "4").Replace("第2", "4").Replace("第２", "4")
                                                    If isMatch_kamma(voice, src(6)) = 1 Then
                                                        result &= "o" & vbTab
                                                        src(7) = src(7).Replace("等速", "1")
                                                        If isMatch_kamma(speed, src(7)) = 1 Or (speed <> 1 And src(7) <> "1") Then
                                                            result &= "o" & vbTab
                                                            src(8) = src(8).Replace("ソフト", 0).Replace("ハード", 1)
                                                            If isMatch_kamma(hardsub_on, src(8)) = 1 Then
                                                                result &= "○ Match!" & vbTab
                                                                'OK
                                                                ReDim Preserve r(2)
                                                                If Trim(dst(1)).Length > 0 And Trim(dst(1)) <> "*" Then
                                                                    r(0) = modify_hlsAppName(Trim(dst(1))) 'hlsAppSelect
                                                                    If isMatch_HLS(r(0), "piperun") = 1 Then
                                                                        '指定がPipeRunならば
                                                                        r(0) = "QSVEnc"
                                                                        video_force_ffmpeg_temp = 2
                                                                    End If
                                                                Else
                                                                    r(0) = hlsAppSelect
                                                                End If
                                                                r(0) = Trim(dst(1)).ToLower 'hlsApp
                                                                If Trim(dst(2)).Length > 0 And Trim(dst(2)) <> "*" Then
                                                                    r(1) = Trim(dst(2)) 'resolution
                                                                Else
                                                                    r(1) = resolution
                                                                End If
                                                                If IsNumeric(Trim(dst(3))) Then
                                                                    dst(3) = dst(3).Replace("主・副", "0").Replace("主", "11").Replace("副", "12").Replace("第二音声", "4").Replace("第二", "4").Replace("第2", "4").Replace("第２", "4")
                                                                    r(2) = Val(Trim(dst(3))) 'audio NHKMODE
                                                                Else
                                                                    r(2) = voice
                                                                End If
                                                                log1write("判定：" & result)
                                                                Exit For
                                                            Else
                                                                result &= "x焼込(" & hardsub_on & ")" & vbTab
                                                            End If
                                                        Else
                                                            result &= "x倍速(" & speed & ")" & vbTab
                                                        End If
                                                    Else
                                                        result &= "x音声(" & voice & ")" & vbTab
                                                    End If
                                                Else
                                                    result &= "xファイル名に含まれる文字" & vbTab
                                                End If
                                            Else
                                                result &= "x拡張子" & vbTab
                                            End If
                                        Else
                                            result &= "x解像度(" & rez(0) & ")" & vbTab
                                        End If
                                    Else
                                        result &= "xHLSアプリ(" & hlsAppName & ")" & vbTab
                                    End If
                                Else
                                    result &= "xライブorファイル再生" & vbTab
                                End If
                            Else
                                result &= "xプロファイル" & vbTab
                            End If
                            log1write("判定：" & result)
                        End If
                    End If
                Next
            End If
        End If

        Return r
    End Function

    'HLSアプリ判定
    Public Function isMatch_HLS(ByVal fullpath As String, ByVal s As String) As Integer
        Dim r As Integer = 0

        Dim p As String = Path.GetFileNameWithoutExtension(fullpath).ToLower
        Dim d() As String = s.Split("|")
        For i As Integer = 0 To d.Length - 1
            If d(i).Length > 0 Then
                If p.IndexOf(d(i).ToLower) >= 0 Then
                    r = 1
                    Exit For
                End If
            End If
        Next

        Return r
    End Function

    '統一した形でhlsApp名を返す
    Public Function modify_hlsAppName(ByVal s As String) As String
        Dim r As String = ""
        If s.Length > 0 Then
            Try
                r = Path.GetFileNameWithoutExtension(s)
            Catch ex As Exception
            End Try
            If r.Length > 0 Then
                s = ":" & r.ToLower & ":"
                If hlschkstr_ffmpeg.IndexOf(s) >= 0 Then
                    r = "ffmpeg"
                ElseIf hlschkstr_qsvenc.IndexOf(s) >= 0 Then
                    r = "qsvenc"
                ElseIf hlschkstr_vlc.IndexOf(s) >= 0 Then
                    r = "vlc"
                ElseIf hlschkstr_piperun.IndexOf(s) >= 0 Then
                    r = "piperun" 'ありえないと思うが
                End If
            End If
            Return r
        End If
        Return r
    End Function

    'カンマ区切りの文字列中に該当文字列があるかどうか
    Public Function isMatch_file_instr(ByVal str As String, ByVal s2 As String) As Integer
        Dim r As Integer = 0
        If s2.IndexOf("*") >= 0 Or s2.Length = 0 Then
            '全てにマッチ
            r = 1
        ElseIf s2 = "-" And str.Length > 0 Then
            '無指定でなければマッチ
            r = 1
        ElseIf s2 = "_" And str.Length = 0 Then
            '無指定ならばマッチ
            r = 1
        ElseIf str.Length = 0 Then
            '比べるものがなければ
            r = 0
        Else
            Dim d() As String = s2.Split(",")
            For i As Integer = 0 To d.Length - 1
                d(i) = Trim(d(i))
                If d(i).Length > 0 Then
                    If str.IndexOf(d(i)) >= 0 Then
                        r = 1
                        Exit For
                    End If
                End If
            Next
        End If
        Return r
    End Function

    'カンマ区切りの文字列中に該当文字列があるかどうか
    Public Function isMatch_kamma(ByVal str As String, ByVal s2 As String) As Integer
        Dim r As Integer = 0
        If s2.IndexOf("*") >= 0 Or s2.Length = 0 Then
            '全てにマッチ
            r = 1
        ElseIf str.Length = 0 Then
            '比べるものがなければ
            r = 0
        Else
            Dim d() As String = s2.Split(",")
            For i As Integer = 0 To d.Length - 1
                d(i) = Trim(d(i))
            Next
            If Array.IndexOf(d, str) >= 0 Then
                r = 1
            End If
        End If
        Return r
    End Function

    '解像度インデックス文字列からhlsAppと解像度を分離して返す
    Public Function get_resolution_and_hlsApp(ByVal resolution As String) As String()
        Dim r(1) As String
        Dim i As Integer = 0

        r(0) = "" '640x360
        r(1) = "" 'hlsApp

        resolution = Trim(resolution).ToLower

        Dim hlsAppSelect As String = ""
        Dim resolution_value As String = Trim(resolution)

        If resolution.IndexOf("_") > 0 Then
            Dim sp As Integer = resolution.IndexOf("_")
            hlsAppSelect = Trim(resolution.Substring(0, sp))
            Try
                resolution_value = Trim(resolution.Substring(sp + 1))
            Catch ex As Exception
            End Try
            If hlschkstr.IndexOf(":" & hlsAppSelect & ":") < 0 Then
                '後ろに指定があった場合
                sp = resolution.LastIndexOf("_")
                resolution_value = resolution.Substring(0, sp)
                Try
                    hlsAppSelect = Trim(resolution.Substring(sp + 1))
                Catch ex As Exception
                End Try
                If hlschkstr.IndexOf(":" & hlsAppSelect & ":") < 0 Then
                    'HLSアプリ指定は含まれていない
                    hlsAppSelect = ""
                    resolution_value = resolution
                End If
            End If
        End If
        If resolution.ToLower.IndexOf("(v)") >= 0 Or resolution.ToLower.IndexOf("(vlc)") >= 0 Then
            hlsAppSelect = "VLC"
            resolution_value = resolution.ToLower.Replace("(v)", "").Replace("(vlc)", "")
        ElseIf resolution.ToLower.IndexOf("(f)") >= 0 Or resolution.ToLower.IndexOf("(ffmpeg)") >= 0 Then
            hlsAppSelect = "ffmpeg"
            resolution_value = resolution.ToLower.Replace("(f)", "").Replace("(ffmpeg)", "")
        ElseIf resolution.ToLower.IndexOf("(q)") >= 0 Or resolution.ToLower.IndexOf("(qsv)") >= 0 Or resolution.ToLower.IndexOf("(qsvenc)") >= 0 Or resolution.ToLower.IndexOf("(qsvencc)") >= 0 Then
            hlsAppSelect = "QSVEnc"
            resolution_value = resolution.ToLower.Replace("(q)", "").Replace("(qsv)", "").Replace("(qsvenc)", "").Replace("(qsvencc)", "")
        ElseIf resolution.ToLower.IndexOf("(p)") >= 0 Or resolution.ToLower.IndexOf("(piperun)") >= 0 Then
            hlsAppSelect = "PipeRun"
            resolution_value = resolution.ToLower.Replace("(p)", "").Replace("(piperun)", "")
        End If

        r(0) = Trim(resolution_value)
        r(1) = Trim(hlsAppSelect)

        Return r
    End Function

    'プロファイル一覧を返す
    Public Function WI_GET_PROFILES() As String
        Dim r As String = ""

        Dim profiles() As String = Nothing

        Dim line() As String = Split(profiletxt, vbCrLf)
        If line IsNot Nothing Then
            Dim j As Integer = 0
            For i As Integer = 0 To line.Length - 1
                Dim sp As Integer = line(i).IndexOf(";")
                If sp >= 0 Then
                    'コメント削除
                    line(i) = line(i).Substring(0, sp)
                End If
                Dim d() As String = Split(line(i), vbTab)
                If d.Length >= 9 Then
                    If Trim(d(0)) <> "*" Then
                        If profiles Is Nothing Then
                            ReDim Preserve profiles(j)
                            profiles(j) = Trim(d(0))
                            r &= Trim(d(0)) & vbCrLf
                            j += 1
                        Else
                            If Array.IndexOf(profiles, Trim(d(0))) < 0 Then
                                ReDim Preserve profiles(j)
                                profiles(j) = Trim(d(0))
                                r &= Trim(d(0)) & vbCrLf
                                j += 1
                            End If
                        End If
                    End If
                End If
            Next
        End If

        Return r
    End Function
End Module
