TvRemoteViewer_VB
=================
チャンネル切り替えを素早く行えるWindows上で再生可能なロケーションフリーを目指して開発。  
が、同時4配信でマシンパワー限界でしょぼーん(´・ω・｀)  

特徴  
・複数同時配信（マシンパワー、UDP＆HLSアプリの安定度に依存）  
・複数の機器からの同時視聴  
・複数同時配信によるスムーズなチャンネル切り替え  
・インターネット(地デジ)、TvRock(地デジorBS/CS)、EDCB(地デジ＆BS/CS)から現在放映中の番組を表示し視聴開始することが可能  
・本来対応していないWindows上での再生もできるかも（readme.txt参照）  

このソフトは、  
地デジのロケフリシステムを作るスレに掲載されていたTvRemoteViewerをVBに変換して追加編集したものです。  
TvRemoteViewer無しにスムーズな開発は出来なかったでしょう。  
作者様ありがとうございます。感謝感謝です。  

バイナリは<http://vb45wb5b.seesaa.net/>からどうぞ

#動かないぞゴルァ！ の前に  
　  
####・Windows8以降のOSでは以下のどちらかの操作が必要です。  
起動したとたんに例外エラーで落ちる場合。  
【推奨】 管理者としてコマンドプロンプトを実行し、  
　netsh http add urlacl url=http://+:40003/ user=XXXXX  
と入力します。(XXXXXは実行するユーザー、もしくは Everyone と入力する)  
または、TvRemoteViewer_vb.exeを右クリック、  
　「プロパティ」→「互換性」→「管理者としてこのプログラムを実行する」にチェック  
　  
　  
####・操作はブラウザから行います  
スマホやiPadからは  
http://[TvRemoteViewer_VBが動作しているPCのIPアドレス]:[TCPポート]/  
にアクセスしてください。  
例　http://192.168.1.5:40003/  
外からアクセスする場合はプロバイダから割り当てられたIPを指定します。併せてルーターのポートマッピングでローカルPCを割り当てることも必要です。IPの調べ方、ルーターの設定等はググってください。  
外からアクセスする場合はIDとパスを必ず設定してください。  
　  
　  
####・ファイアウォールに注意  
スマホ等のブラウザからアクセスすると応答無しになってしまう場合。  
TvRemoteViewer_VBを起動しているPCからアクセス（http://127.0.0.1:40003/）するとおｋだが、他のPCやスマホ等からアクセス（http://IPアドレス:40003/）すると応答無しになることがあります。  
そのようなときはファイアウォールに受信許可をしてあげてください。  
Windowsファイアウォールの場合  
　参考：<http://windows.microsoft.com/ja-jp/windows/open-port-windows-firewall>  
このページの手順にしたがって受信の規則にTCPの40003を許可するよう設定してください。  
　  
　  
####・このソフトはタスクトレイからスタートします。  
起動するとタスクトレイに常駐します。タスクトレイに隠れていないか確認してください。  
ダブルクリックすると設定画面が開きます。  
　  
　  
####・起動に時間がかかりすぎた場合、タスクトレイにアイコンが現れない場合があります  
タスクマネージャーでTvRemoteViewer_VB.exeを停止して再起動してあげましょう  
　  
　  
####・Framework3.5とFramework4.0以上が必要です。  
このソフトはVisualStudio2010のVisualBasicで作られています。  
Windows7以降は問題無いはずですが、XPなどで実行する場合はFramework4.0、Windows8以降ではFramework3.5のインストールが必要です。  
詳しく無いので具体的には説明できませんが・・3.5でも4でも4.5でもなんでもかんでもインスコしてしまえばどうでしょう（無責任） 　  
　  
####・RemoteTestと同時起動する場合はRemoteTestで使用するHTTPポートと重複していないか確認してください。  
まずRemoteTestを終了してから起動してみましょう。  
RemoteTestとの同時起動を試みる場合はHTTPポート40003を他の数値に変更してみてください。  
　  
　  
####・初期設定はHLSアプリとしてffmpegを使用することを想定しています  
vlcは複数配信時に不安定なのでお勧めしませんが、  
HLSアプリにvlcを選択した場合は  
HLS_option.txt  
の内容を  
HLS_option_VLC.txt  
の内容に置き換えなければなりません。  
（HLS_option_VLC.txtは削除しないでください。）  
そのうえでフォーム上の解像度選択を行ってください【重要】  
HLSオプションがvlc用のものに切り替わったことを確認してください。  
　  
　  
####・起動に必要なファイルをきちんと配置しましょう  
ffmpegは解凍したままのフォルダ構造で使用しましょう。  
また、同梱の「libx264-ipod640.ffpreset」をffmpegのpresetフォルダにコピーするのを忘れないでください。  
　  
　  
####・なるべく新しいffmpegを使いましょう  
当方のテストでは古いffmpegでは複数同時配信時に不安定です。  
古すぎるものだとエンコードがスタートしないこともあります。  
　  
　  
####・BonDriverの場所がRecTask.exeの場所と異なる場合は注意  
BonDriver PathにはRecTask.iniに記述されているフォルダを指定しましょう  
あるはずのBonDriverが無いということも起こりえます。  
　  
　  
####・特定の放送局でRecTaskが不安性でHLS再起動が繰り返されてしまう  
RecTask.ini　  
Logging.OutputToFile=true　  
としてみてください　  
謎ですがうちでは正常動作となりました。　  
また、UDP追加オプションに「/log /loglevel 5」を加えると正常動作したとのコメントがありました。　  
　  
　  
####・vlcを使用する場合は%WWWROOT%は半角スペースの入らないところに設置しましょう  
vlcのオプション記述の関係からhtmlを配置するフォルダのパスには半角スペースが入らないほうが無難です。  
BS1＆BSプレミアム視聴時にvlcへ切り替える設定をしている場合もご注意ください  
　  
　  
####・(＊)印がついた項目を変更した場合はTvRemoteViewer_VB.exeを再起動してください。  
　  
　  
