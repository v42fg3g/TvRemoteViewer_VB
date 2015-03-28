'★パスワードで文字列を暗号化する
'http://dobon.net/vb/dotnet/string/encryptstring.html

Module モジュール_crypt
    ''' <summary>
    ''' 文字列を暗号化する
    ''' </summary>
    ''' <param name="sourceString">暗号化する文字列</param>
    ''' <param name="password">暗号化に使用するパスワード</param>
    ''' <returns>暗号化された文字列</returns>
    Public Function EncryptString(ByVal sourceString As String, _
                                         ByVal password As String) As String
        If sourceString.Length > 0 Then
            'RijndaelManagedオブジェクトを作成
            Dim rijndael As New System.Security.Cryptography.RijndaelManaged()

            'パスワードから共有キーと初期化ベクタを作成
            Dim key As Byte(), iv As Byte()
            GenerateKeyFromPassword(password, rijndael.KeySize, key, rijndael.BlockSize, iv)
            rijndael.Key = key
            rijndael.IV = iv

            '文字列をバイト型配列に変換する
            Dim strBytes As Byte() = System.Text.Encoding.UTF8.GetBytes(sourceString)

            '対称暗号化オブジェクトの作成
            Dim encryptor As System.Security.Cryptography.ICryptoTransform = _
                rijndael.CreateEncryptor()
            'バイト型配列を暗号化する
            Dim encBytes As Byte() = encryptor.TransformFinalBlock(strBytes, 0, strBytes.Length)
            '閉じる
            encryptor.Dispose()

            'バイト型配列を文字列に変換して返す
            Return System.Convert.ToBase64String(encBytes)
        Else
            Return ""
        End If
    End Function

    ''' <summary>
    ''' 暗号化された文字列を復号化する
    ''' </summary>
    ''' <param name="sourceString">暗号化された文字列</param>
    ''' <param name="password">暗号化に使用したパスワード</param>
    ''' <returns>復号化された文字列</returns>
    Public Function DecryptString(ByVal sourceString As String, _
                                         ByVal password As String) As String
        Try
            'RijndaelManagedオブジェクトを作成
            Dim rijndael As New System.Security.Cryptography.RijndaelManaged()

            'パスワードから共有キーと初期化ベクタを作成
            Dim key As Byte(), iv As Byte()
            GenerateKeyFromPassword(password, rijndael.KeySize, key, rijndael.BlockSize, iv)
            rijndael.Key = key
            rijndael.IV = iv

            '文字列をバイト型配列に戻す
            Dim strBytes As Byte() = System.Convert.FromBase64String(sourceString)

            '対称暗号化オブジェクトの作成
            Dim decryptor As System.Security.Cryptography.ICryptoTransform = _
                rijndael.CreateDecryptor()
            'バイト型配列を復号化する
            '復号化に失敗すると例外CryptographicExceptionが発生
            Dim decBytes As Byte() = decryptor.TransformFinalBlock(strBytes, 0, strBytes.Length)
            '閉じる
            decryptor.Dispose()

            'バイト型配列を文字列に戻して返す
            Return System.Text.Encoding.UTF8.GetString(decBytes)
        Catch ex As Exception
            'エラーが起こったらそのまま返す
            Return sourceString
        End Try
    End Function

    ''' <summary>
    ''' パスワードから共有キーと初期化ベクタを生成する
    ''' </summary>
    ''' <param name="password">基になるパスワード</param>
    ''' <param name="keySize">共有キーのサイズ（ビット）</param>
    ''' <param name="key">作成された共有キー</param>
    ''' <param name="blockSize">初期化ベクタのサイズ（ビット）</param>
    ''' <param name="iv">作成された初期化ベクタ</param>
    Private Sub GenerateKeyFromPassword(ByVal password As String, _
                                               ByVal keySize As Integer, _
                                               ByRef key As Byte(), _
                                               ByVal blockSize As Integer, _
                                               ByRef iv As Byte())
        'パスワードから共有キーと初期化ベクタを作成する
        'saltを決める
        Dim salt As Byte() = System.Text.Encoding.UTF8.GetBytes("saltは必ず8バイト以上")
        'Rfc2898DeriveBytesオブジェクトを作成する
        Dim deriveBytes As New System.Security.Cryptography.Rfc2898DeriveBytes( _
            password, salt)
        '.NET Framework 1.1以下の時は、PasswordDeriveBytesを使用する
        'Dim deriveBytes As New System.Security.Cryptography.PasswordDeriveBytes( _
        '    password, salt)

        '反復処理回数を指定する デフォルトで1000回
        deriveBytes.IterationCount = 1000

        '共有キーと初期化ベクタを生成する
        key = deriveBytes.GetBytes(keySize \ 8)
        iv = deriveBytes.GetBytes(blockSize \ 8)
    End Sub
End Module
