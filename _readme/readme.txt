TvRemoteViewer_VB v2.92




■これってなに？

	離れたとこからPC上のテレビや動画を見るためのソフトです



■環境

	Windowsが動作するPC
	FrameWork4.5.x
	TVTestが動作する環境
	
	UDPソフト：　RecTask, TSTask
	HLSソフト：　ffmpeg, VLC, QSVEnc,NVEnc



■起動

	★注意★
	Windows8以降では以下のどちらかの操作が必要です。
	・web許可_右クリックで管理者として実行.bat
	　を右クリックして管理者として実行（Tvmaidさん添付のファイルを再利用させてもらっています。感謝です）
	・【強く推奨】コマンドプロンプトから
	　netsh http add urlacl url=http://+:40003/ user=XXXXX
	　(XXXXXは実行するユーザー、もしくは Everyone と入力する)
	　入力例：　netsh http add urlacl url=http://+:40003/ user=Everyone
	・【↑ができなかった場合】TvRemoteViewer_vb.exeを右クリック、
	　「プロパティ」→「互換性」→「管理者としてこのプログラムを実行する」にチェック


	設置後、起動するとタスクトレイからスタートします。
	ダブルクリックで通常の大きさになります（×で閉じると終了してしまいます）
	各パラメーターはreadme.jpg参照のこと



■視聴＆操作

	実際の視聴や操作はWEB上で行います。
	例：	PC（IPアドレス 192.168.1.100　ポート40003)、でTvRemoteViewer_VBを実行している場合は
		リモート機（スマホなど）のブラウザでhttp://192.168.1.100:40003にアクセスしてください
		ローカルPCでテストしている場合はhttp://127.0.0.1:40003です



■設置 readme.jpgや下に書いたテスト環境を参考にしてください

	素晴らしい解説サイト
	http://vladi.cocolog-nifty.com/
	http://vladi.cocolog-nifty.com/blog/2014/10/iphoneandroidpc.html
	をご覧ください

	以下はてきとーな情報です

	・RecTaskとBonDriverを適切に配置
	・ffmpeg又はVLCをインストール
	（★ffmpegを使う場合は同梱のlibx264-ipod640.ffpresetをffmpegをインストールしたところのpresetsフォルダにコピー）
	・同梱のHLS_option～.txtをTvRemoteViewer_VB.exeと同じフォルダにコピー
	・WWWROOT（WEBルートフォルダ）となるフォルダに同梱のHTMLフォルダ内のファイルをコピー
	（★VLC使用の場合は半角スペースが入らない場所のほうが安心です）
	・ファイル再生させるためには、あらかじめ同梱のTvRemoteViewer_VB.iniを編集して動画があるフォルダを指定しておく必要があります。
	・地デジ番組表の設定	TvRemoteViewer_VB.iniを編集してください
	・TvRock番組表につきまして
		1.ブラウザで
		　http://[TvRockが稼働しているPCのローカルIP]:[TvRockのポート]/[ユーザー名]/iphone?md=2&d=0
		　（例　http://127.0.0.1:8969/ユーザー名/iphone?md=2&d=0）
		　にアクセスする。
		　すると、iPhone用番組表が表示されます。
		2. 「予約表示」数を3以上にする。無しを選択しますと次番組が表示されません
		　（Firefoxではうまく切り替わらないかもしれません）
		　ここで表示された番組表がデータとして使用されます。
		3. ブラウザを閉じる
		
		※地デジまたはBS/CSのどちらかだけを表示したい場合は、チューナーを選択してください
	・EDCB番組表につきまして
		EpgTimer.exeが存在するフォルダにあるEpgTimerSrv.iniを開いて[SET]直後に
			EnableHttpSrv=1
			HttpPort=5510
		を書き加えてEpgTimerを再起動してください
		参考　http://blog.livedoor.jp/kotositu/archives/1923002.html
		↑は
	・ptTimer番組表につきまして
		sqlite3.exeをTvRemoteViewer_VB.exeと同じフォルダに設置してください
		sqlite3.exeはググればすぐ見つかると思います



■HLSアプリにつきまして

	・ffmpegをご使用の場合【推奨】
	http://ffmpeg.zeranoe.com/builds/
	ffmpegインストール先のpresetsフォルダ内に同梱のlibx264-ipod640.ffpresetをコピーしてください。
	参考：http://frmmpgit.blog.fc2.com/blog-entry-179.html


	・VLCをご使用の場合
	同梱のHLS_option.txtの内容をHLS_option_VLC.txtの内容に差し替えてフォーム上で解像度を選択し直してください。
	当方のテストによりますと、複数のプロセスを起動しますとVLCの挙動が大変不安定になります。２つならまぁなんとか。
	2.0.5、2.1.0ともクラッシュ多発、延々と再起動を繰り返し起動時に停止することも。2.0.5では応答しなくなる現象が1度有り


	・HLSオプションで実行時に変換される定数
	%UDPPORT%	ソフトで自動的に割り当てられたudpポート
	%WWWROOT%	WWWのrootフォルダ
	%FILEROOT%	m3u8やtsが作成されるフォルダ
	%HLSROOT%	HLSアプリが存在するフォルダ
	%HLSROOT/../%	HLSアプリが存在するフォルダの１つ上の親フォルダ（ffmpeg解凍時のフォルダ構造に対応）
	%rc-host%	"127.0.0.1:%UDPPORT%"に変換されます。
	%NUM%		ストリームナンバー
	%VIDEOFILE%	"ビデオファイル"に変換（実際は「-i %VIDEOFILE%」の決め打ちで-iの後ろの文字列がファイル名に変換）



■HLSアプリの個別指定についきまして

	○指定方法
	・HLS_option*.txtへの記述（インデックスまたはHLSオプション本文）
	・StartTv.htmlへhlsAppSelect引数によりHLSアプリ名を指定(hlsAppSelect=VLC,V,ffmpeg,F,QSVEnc,Q,QSV,NVEnc,N,NSV,PiprRun,P)

	○どのHLSオプションが使用されるのか？
	・明示的にHLSアプリが個別指定された場合は、各アプリに対応したHLS_option_[HLSアプリ]*.txtからHLSオプションが優先的に使用されます
	　v1.90～　ライブ放送＆解像度インデックス内でHLSアプリが指定されている場合は↑の前にHLS_option.txt内を検索します
	・HLSアプリが指定されていない＆ファイル再生の場合はHLS_option_[HLSアプリ]_file.txtが優先的に使用されます
	・HLSアプリが指定されていない＆解像度指定があればHLS_option.txtからHLSオプションが使用されます
	・HLSアプリが指定されていない＆解像度指定が無い場合はフォーム上のHLSオプションが使用されます（例 フォーム上のStartボタン）
	・v1.97～　フォーム上の「HLSオプションor解像度を送る」選択の値によって、解像度指定の無い場合にフォーム上のHLSオプションの値を優先するか、HLSアプリに応じたHLS_option～.txt内の値を優先するか選べるようになりました

	○HLSアプリの個別指定方法
	・HLSアプリ指定文字列（大文字・小文字どちらでもOK）
	VLC: VLC, V
	ffmpeg: ffmpeg, F
	QSVEnc: QSVEnc, QSVEncC, Q, QSV 
	NVEnc: NVEnc, NVEncC, N, NV 
	PipeRun: PipeRun, P

	○HLS_option*.txtの記述
	以下はどれもexepath_QSVEncで指定されたQSVEncC.exeが使用されます
	・StartTv.html?hlsAppSelect=QSVEnc
	・StartTv.html?hlsAppSelect=Q
	・[QSVEnc_640x360]～
	・[Q_640x360]～
	・[640x360_QSVEnc]～
	・[640x360_Q]～
	・[(QSVEnc)640x360]～
	・[(Q)640x360]～
	・[640x360(QSVEnc)]～
	・[640x360(Q)]～
	・[640x360]QSVEnc_～
	・[640x360]Q_～
	・[640x360](QSVEnc)～
	・[640x360](Q)～

	○HLSオプションファイルからの選ばれ方
	例えば F_640x360 と指定された場合、
	HLS_option_ffmpeg.txt
	から、はじめに[F_640x360]という見出しを探し、見つからない場合[640x360]を探します



■WEBインターフェース

	クライアント開発者向け情報
	readme_dev.txt参照のこと



■テスト環境

	Windows7 x64
	VisualStudio2010

	RecTaskインスコ	=	D:\TvRemoteViewer\TVTest
	BonDriver	=	D:\TvRemoteViewer\TVTest
	%WWWROOT%	= 	D:\TvRemoteViewer\html
	%FILEROOT%	=	D:\TvRemoteViewer\html
	%HLSROOT%	=	D:\TvRemoteViewer\ffmpeg\bin
	%HLSROOT/../%	=	D:\TvRemoteViewer\ffmpeg
	(%HLSROOT%)	=	D:\TvRemoteViewer\vlc

	iPad(第3世代）iOS7 safari、Android(Nexus7旧)



■ptTimerにて1ストリームしか使用できないことへの対策
	1. BonDriver_ptmr.dllを適当な名前で4つコピーします
	2. それぞれの.ch2ファイルをBonDriver_ptmr.ch2からコピーして以下の通り編集します
	   ch2ファイル内に4つのチャンネル空間ブロックがありますが、1つに限定するようにします。
	   例えば、1つめは「;#SPACE(1,T0)」、2つめは「;#SPACE(3,T1)」、3つめは「;#SPACE(0,S0)」、4つめは「;#SPACE(2,S1)」、
           というふうにチャンネル空間を１つだけにします
	3. これで4ストリーム全て使用することができます



■ISO再生のための準備
1.最新版をダウンロードし解凍後TvRemoteViewer_VB.exeを上書きコピーします
2.「mplayer-svn-35935.7z」というキーワードで検索のうえダウンロードしmplayer.exeをTvRemoteViewer_VB.exeと同じフォルダにコピーします
　（新しすぎるものは日本語ファイル名でバグがあるそうです。mplayer-svn-35935.7zを教えてくださった方ありがとうございます）
3.VLC-2.1.2をダウンロードして適当なフォルダに解凍します（今まで使用してきたものとは別フォルダ）
　http://download.videolan.org/pub/videolan/vlc/2.1.2/
4.TvRemoteViewer_VB.iniにexepath_ISO_VLC=(↑のvlc.exeへのパス)の記述を追加すればOKです


■EDCBのUDP送信を受信するには
まず、EDCBにてUDP送信の設定を行ってください
次にBonDriver_UDP.dllと同じ場所にBonDriver_UDP.ch2を作成してください。内容は例えば
;▼ここから=============
; TVTest チャンネル設定ファイル
; 名称,チューニング空間,チャンネル,リモコン番号,サービス,サービスID,ネットワークID,TSID,状態
;#SPACE(0,ネットワーク)
UDP1234,0,0,0,0,48834,0,0,1
UDP1235,0,1,0,0,48835,0,0,1
;▲ここまで=============
のようになります。内容は確実ではありません
後はTvRemoteFilesの管理タブから手動配信で視聴できます
なお、BonDriver_RecTaskやBonDriver_TSTaskでの配信は実験していません



■修正したり追加したりして欲しいところ

	・全般的にクラスというものがわかってない・・お～まいがっ
	・ffmpegの穏便な終了



■AbemaTV番組データにつきまして
	現在、AbemaGraphさんのご協力をいただいております。この場をお借りして御礼申し上げます



■免責
	作者は一切の責任を負いません。絶対に！自己責任で使用できる方のみお使いください



■履歴

	0.01	酔っ払ってうｐ　今は反省している
	0.02	いろんなとこを修正＆追加
	0.03	コンソール表示・非表示のチェックボックスを設置
		配信中のストリーム番号をフォーム上に表示
		配信中のストリーム番号をタスクトレイマウスオーバー時に表示
		ffmpegのHLS_option.txt内容を修正（パスを「"」で囲っただけです）
	0.04	二重起動を試みたときに例外エラーが出るバグを修正
		どのhtmlページでもパラメーター置換を行うようにした
		WEBインターフェースの修正（HTMLを書き換える必要が無くなりました）
	0.05	ViewTV.htmlでチャンネル切り替え時に解像度を引き継ぐようにした
		ViewTV.htmlのビデオの幅と高さを変数化
		ViewTV.htmlの修正
	0.06	UDPアプリ用オプション欄を追加
		UDPオプション作成時のバグ修正
	0.07	ffmpeg使用時の自動ts削除を修正
	0.08	ファイル再生に対応（ffmpegのみ）
	0.09	BASIC認証対応
		VideoPath.txtにサブフォルダを含めるかどうかのオプションを追加
	0.10	ストリーム視聴ボタンを番号順に並び替えるようにした
		VideoPath.txtが存在しないときに例外エラーが出ていたバグを修正
	0.11	配信スタートボタンを押した後、視聴ページへリダイレクトするようにした
		htmlのデザインを柔軟にできるよう特定変数の前中後に表示するhtmlタグを指定できるようにした
		index.htmlとViewTV.htmlのデザイン修正
	0.12	解像度をフォーム上のオプション使用で視聴時、チャンネル切り替え時に引き継がれなかったバグを修正
		コンソール非表示に指定してもVLCのコンソールが表示されてしまうバグを修正
		UDP&HLSオプションをログに表示するタイミングを修正
	0.13	ffmpeg使用時、BS1＆BSプレミアム視聴時のみvlcに切り替えるオプションをVideoPath.txtに追加
	0.14	%FILEROOT%指定が全く機能していなかったバグを修正し併せてViewTV.htmlを修正した
	0.15	ストリーム視聴ボタンに放送局名を表示するようにした
	0.16	BonDriverと放送局を取得するファイルアクセスを極力減らすようにした。html表示高速化
	0.17	%FILEROOT%が指定されているときにVLCが正しい場所にファイルを作らないバグを修正
		HLS_option_VLC.txtの修正
	0.18	コンソールを表示しない設定にしていてもVLCの窓が表示されてしまっていたバグを修正
	0.19	ViewTV.htmlを表示したときに視聴中の放送局がわかるようにした
	0.20	放映中の番組内容を表示するボタンを設置（VideoPath.txtにオプション追加）
	0.21	地デジ番組表に視聴ボタンを付けた
		地デジ番組表の設定項目をVideoPath.txtに追加
	0.22	TvRockから番組表を取得できるようにした（ブラウザでiPhone用番組表を一度だけ表示する必要有り）
	0.23	EDCBから番組表を取得できるようにした（EpgTimerSrv.iniを編集する必要有り）
	0.24	わかりずらいので設定ファイルをTvRemoteViewer_VB.iniにした。
		（VideoPath.txtが存在する場合はVideoPath.txtを優先します）
		RecTaskを名前指定で終了させるかどうかのオプションをTvRemoteViewer_VB.iniに追加
		（録画でRecTaskを使用している環境を考慮）
	0.25	EDCB番組表の不具合を修正
	0.26	放送局名変換をネット・EDCB・TvRockで分離しiniに項目を追加
		わかりづらかったのでTvRemoteViewer_VB.iniの内容を整理
	0.27	BASIC認証を修正
		NHK関連で音声がおかしくなることへの対処方法を選択できるようにした
		（TvRemoteViewer_VB.iniにオプション NHK_dual_mono_mode を追加）
		HTMLを少々修正
	0.28	index.htmlにおいてNHK関連以外は音声選択を表示しないようにした
	0.29	初起動時に設定途中で例外エラーが起こるバグを修正
	0.30	ファイル再生でリダイレクトに失敗していたバグを修正
	0.31	VLCが指定されていなくてもNHK音声選択に「VLCで再生」と表示されていたバグを修正
	0.32	アイコンの追加
		コードの最適化
	0.33	一部のCS局の視聴ページで選局名が間違って表示されるバグを修正
	0.34	配信や停止を立て続けに行った場合に例外エラーが出ることがあったバグを修正
	0.35	標準HLSアプリにVLCを選んでいるときにもNHKの音声モード選択が表示されていたバグを修正
	0.36	配信再起動になった場合に表示される「配信されていません」ページを自動的に再読み込みするようにした
	0.37	再生を開始可能と判断するtsファイル数を指定できるようにした
		MIME TYPEを設定するオプションをiniに追加
		RecTaskが終了しないオプション(Stop_RecTask_at_StartEnd)が作用していなかったバグを修正
		HLSオプションを修正（再生待機時間の縮小）
	0.38	設定ファイルを読み込む際のカレントフォルダをきっちり指定するようにした
	0.39	MIME TYPEの設定をコメントアウトしたときに例外が起こっていたバグを修正
	0.40	ファイル関係で例外が起こった場合のログ出力とエラー処理を追加
		index.htmlへのリダイレクトコードの修正
	0.41	名前付きパイプ関連でエラーが起こった場合は名前付きパイプを使用しないようにした
	0.42	ViewTVx.htmlにおいて、NHKモードの選択ができなかった不具合を修正
		それに伴いViewTVx.htmlにスクリプトを追加
	0.43	iniに名前付きパイプを取得する外部プログラムを指定できるオプションを追加
	0.44	0.43で追加したオプションの廃止
		フォーム上のボタンがENTERキーで押されないように対策
	0.45	VLC再生を試みる際のエラー対策をした
	0.46	名前付きパイプ取得方法を変更（ご協力いただいた方々に感謝）
	0.47	無駄なエラー処理を修正
		細かいバグフィックス
	0.48	HLSオプション中の%NUM%を変換するようにした
		HLS_option.txtを少々修正
	0.49	iniに最大配信数を制限できるようオプションを追加
		【HTTPストリーム実験】
		httpストリーム配信（VLC環境でクライアントソフトを使用したときのみ）
		【安定性実験】
		iniに起動するUDP,HLSアプリのCPU優先度を指定するオプションを追加
		【安定性実験】
		UDP,HLS各exeを配信ナンバー毎に違うexeを使用できるようにした。
		exeが存在するフォルダ名に配信ナンバーを追記したフォルダ内のexeを使用します。
		例：HLSアプリにffmpegを使用し手いる場合
		通常～\bin\ffmpeg.exeを使用しているときに
		～\bin1\ffmpeg.exeを用意しておけば配信1のときに使用するようになります。
		～\bin2\ffmpeg.exeを用意しておけば配信2のときに使用するようになります。
		UDPアプリにつきましても同様です。
	0.50	iniのVideoPathに()が入っていた場合に動画一覧が正常に取得できなかったバグを修正
	0.51	配信準備中ページとしてWaiting.html、エラーページとしてERROR.htmlをWWWフォルダに追加
		存在しない場合は従来通りプログラム生成で表示します
	0.52	カスタマイズできるようプログラムが生成するHTMLフォーム要素にclass名を付加するようにした
	0.53	インターネット番組表を変更（東京＆大阪のみ対応）
	0.54	インターネット番組表を変更（全国対応・北海道の地域番号は1のみ）
	0.55	番組表と視聴ページに解像度選択を付けた
		HTML変換に解像度選択を追加
		SelectVideo.htmlの修正
	0.56	ファイル再生ページを修正
		SelectVideo.htmlの修正
	0.57	TvRockのiphone番組表にて予約0以外を指定している場合に番組表の時刻がおかしくなる現象への対処
		iniの NHK_dual_mono_mode に11と12を追加（NHK以外でもdual_monoを設定するときに使用）
	0.58	ビデオファイル一覧に表示するファイルの拡張子を指定できるようiniにオプションを追加
		WEB上のBonDriver一覧に表示したくないBonDriverを指定できるようiniにオプションを追加
	0.59	同一BonDriver上でのチャンネル変更ならば名前付きパイプを使用することにした
		停止ルーチンのバグ修正
		WEBインターフェース用の番組表APIを追加
	0.60	WEBインターフェースの修正と追加
		UDPアプリが配信開始してからHLSアプリが起動するまでに入れる待ち時間指定をiniに追加
		BonDriverパスの不具合を修正
	0.61	配信中ストリーム一覧表示でのBonDriverパスの不具合を修正
		名前付きパイプを使用してUDPアプリが実際に配信を開始するまで待機するようにした
	0.62	0.60で導入したウェイトの廃止（iniから削除）
		名前付きパイプ番号の取得方法の変更
	0.63	名前付きパイプコードの修正
	0.64	UDPアプリが配信開始してからHLSアプリが起動するまでに入れる待ち時間指定をiniに復活
	0.65	配信時にUDPアプリより先にHLSアプリを起動するようにした
	0.66	WEBインターフェースを追加
		TvRemoteViewer_VB_client 1.01に対応
	0.66b	配信開始手順のみ0.59そのものに戻した実験バージョン（iniの UDP2HLS_WAIT = 500）
	0.67	0.66bをベースにパイプ番号取得方法変更とUDP配信確認を追加（iniのUDP2HLS_WAIT=500推奨）
		起動時にチャンネル情報を取得することにした
	0.68	WEBインターフェースを修正
		HTML内で「ユーザー名:パスワード@」に変換できるようにした（iniで許可する必要有り）
	0.69	RecTaskの名前付きパイプによる終了が失敗した場合は強制終了させることにした
		ログ処理の修正
		プロセス再起動時の挙動を修正
	0.70	名前付きパイプ取得方法を古い方式に戻した
		ファイル再生においてスペース区切り(半角or全角)で複数ワードによる抽出に対応
		SelectVideo.htmlにNHK音声選択を追加
	0.71	HTTPリクエストの非同期化
		HTMLページ内変数の変換を修正
	0.72	ffmpegのHTTPストリーミング配信に対応（要クライアント）
		HLS_option_ffmpeg_http.txtの修正
		iniにストリーム切断時に配信終了までの秒数を設定する項目を追加
	0.73	一部地域において地デジ番組表が表示されないバグを修正
	0.74	インターネット番組表を変更(地域番号が変更されていますのでiniを見て適切に設定してください)
	0.75	HLS用ニコニコ実況コメント取得ソフトNico2HLSに対応
		ViewTV～.htmlの変更
	0.76	TvRock＆EDCB番組表の放送局NGワードを地デジのものから分離しiniに追加
		EDCB番組表において番組情報を取得しない設定をiniに追加
	0.77	%FILEROOT%を自由に設定できるようにした（RAMドライブ指定を考慮。ドライブそのものを指定することはできません）
		例：%FILEROOT%をZ:\streamに設定した場合、http://～:40003/stream/～へのアクセスはZ:\streamに割り振られます
	0.78	ニコニコ実況用変換文字列として%JKNUM%と%JKVALUE%を追加
		ch_sid.txtの追加
	0.79	HTML文字コードの標準をUTF-8にした(iniに文字コード指定オプションを追加）
		HTMLファイルの文字コードをUTF-8にした
	0.80	HTTP配信(ffmpeg)の修正
		iniにffmpegのバッファを指定するHTTPSTREAM_FFMPEG_BUFFERを追加
	0.81	配信停止時にも関連ファイルを削除することにした（初回接続時のスピードアップ狙い）
		Nico2HLSへの対応を終了
		PCクライアントの不具合により初回配信準備に失敗した場合に配信停止するようにした
		アイドル時間が指定した分数に達すると全切断するようiniにSTOP_IDLEMINUTESを追加
	0.82	ファイル再生時のWI_GET_LIVE_STREAM.htmlのBonDriver欄にフルパスファイル名を記入するようにした
		HTTP配信のファイル再生においてffmpegが2重に起動してしまうバグを修正
	0.83	WEBインターフェース（WI_GET_PROGRAM_NUM）を追加
	0.84	ファイル再生において同名のASSファイルがあるときは字幕をハードサブするようにした
		（ffmpegのフォルダ名がffmpeg-20140628-git-4d1fa38-win64-static等長い場合は機能しません）
		ffmpeg.exeが存在するフォルダにfonts\fonts.confを設置する必要があります
		参考　http://peace.2ch.net/test/read.cgi/avi/1413523104/779
	0.85	HLSオプション内に-vfが存在する場合でもハードサブに対応した
	0.86	ファイル抽出時に文字化けしていたバグを修正
	0.87	ファイル再生時に開始シーク秒を指定できるようにした
		iniに標準シーク秒を指定するVideoSeekDefaultを追加
		SelectVideo.htmlを修正
	0.88	ファイル再生時シークの高速化
		起動時にフォルダと必要なファイルチェックを行うようにした
		ファイル再生時のシーク欄に時:分:秒（1:30や1:20:00）の指定ができるようにした
	0.89	WEBインターフェースの問い合わせにファイル再生時にシークした秒数を返すようにした
		（WI_GET_LIVE_STREAMとWI_GET_PROGRAM_NUMの末尾に「,」区切りで追加されています）
		WI_GET_PROGRAM_NUMのバグを修正
	0.90	HTTP配信時のチャンネル切り替え安定性向上
		起動時にiniのStop_RecTask_at_StartEndが反映されていなかったバグを修正
	0.91	WEBインターフェースWI_GET_VIDEOFILES2を追加
		ビデオフォルダ自動更新チェック機能の追加
	0.92	チャンネル切り替え時に一部のチャンネルで違う局が選局されてしまう不具合を解消
		ビデオファイル一覧作成手順の修正
		不必要なビデオフォルダ自動更新チェックが行われる現象を解消
	0.93	iniのBonDriver_NGwordが機能していなかったバグを修正
		パイプを使用したチャンネル切り替えを修正
	0.94	iniの優先BonDriver(TvProgramD_BonDriver1st,TvProgramS_BonDriver1st)を複数記入可能にした
		WEBインターフェース（WI_GET_TVRV_STATUS）に優先BonDriver表示を追加
		iniに番組表上の配信ナンバーを制限するTvProgram_SelectUptoNumを追加
	0.95	HLS_option_ffmpeg_file.txtが存在していればファイル再生時にHLSオプションとして使用するようにした
		SelectVideo.htmlにおいてキーワード抽出ができなくなっていた不具合を解消
		SelectVideo.htmlのキーワード抽出でフォルダ名も検索に含めるようにした
	0.96	WEBインターフェース（WI_FILE_OPE）を追加
	0.97	配信中に古い断片ファイルtsを削除しないようにするオプション(OLDTS_NODELETE)をiniに追加
	0.98	0.93でのパイプチャンネル切替えのバグを修正
	0.99	SelectVideoページの解像度セレクトをHLS_option_ffmpeg_file.txt内のものにするようにした(存在すれば)
	1.00	WI_GET_LIVE_STREAMの末尾に配信ファイルURLを追加
	1.01	ffmpegのHTTP配信においてffmpeg起動に時間がかかった場合を考慮し自動切断に余裕を持たせるようにした
	1.02	HTTP配信時にNHKモード選択が機能していなかったバグを修正
		HTTP配信時、HLS_option_[HLSアプリ]_http.txt内の独自解像度選択に対応
	1.03	標準添付のHLS_option.txtとHLS_option_ffmpeg.txtを修正した
		HLS_option_ffmpeg_file.txtを追加
		HLS_option_ffmpeg_file.txtを使用時、解像度が未指定の場合はフォーム上のオプション内解像度を使用するようにした
	1.04	NHK配信時にNHKモード11&12が機能していなかったバグを修正
	1.05	HTTP配信時、UDPアプリが配信準備状態になっていないときに配信要求があった場合は中断するようにした
	1.06	HTTP配信時、ストリームが中断されたときにHLSプロセスが残っていた場合は終了させることにした
		ffmpegファイル再生において無変換時はハードサブを入れないようにした
	1.07	StartTv.html用オプションとしてnohsub(1=ハードサブ禁止）を追加
	1.08	起動時にコマンドラインオプション「-dok」を付ければ二重起動を許可するようにした
	1.09	パスワードを暗号化して保存することにした
	1.10	起動時のBondriverチェックを厳密にした
	1.11	iniにEDCB番組表がスカパープレミアム用であることを指定するTvProgramEDCB_premiumを追加
		ini内でのTvRockとEDCB用チャンネル名マッチング指定の必要をなくした
		iniにスカパープレミアム時に優先的に割り当てるBonDriverを指定できるようにした
		（TvProgramP_BonDriver1st）
		WI_GET_TVRV_STATUS.htmlにTvProgramEDCB_premiumとTvProgramP_BonDriver1stを追加
		BS-TBSとQVCのサービスIDが同一であることに対処
		EDCB番組表上でのマッチングバグを修正、とともにTSIDでもチェックするようにした
	1.12	WI番組情報内のカンマを全角に変換するようにした
		iniにスカパープレミアムSPHD視聴時に使用するRecTaskを指定できるようにした(RecTask_SPHD)
	1.13	WI番組情報取得（地デジ・TvRock）においてNG局が表示されてしまっていたバグを修正
	1.14	RecTaskがチャンネル切り替えに失敗した時に配信を続けようとしていたバグを修正
		WEBインターフェース、ログを表示(WI_SHOW_LOG.html）を追加
	1.15	番組表内の<>をエスケープ
		TvProgramEDCB_premiumの仕様変更（0=全て 1=SPHDのみ 2=SPHD以外)
		EDCBとTvRockの番組表の放送局NG指定にサービスIDを使用できるようにした(数値ならサービスID)
	1.16	EDCBの番組表に表示すべき局をEDCBのWEBサービスから取得するようにした
		番組表内の「&」のエスケープ
	1.17	ch2ファイルからのチャンネル情報取得バグを修正
	1.18	iniにファイル一覧作成時にファイルサイズ0のものは記載しないにするVideoSizeCheckを追加
		サーバーとしてはストリーム再生中はファイル一覧更新作業は行わないことにした
		ch_sid.txtから難視聴を削除
	1.19	WI_GET_PROGRAM_[TVROCK,EDCB]に次番組も取得するオプションを追加（上に説明を記述)
		標準番組表において番組終了まで7分を切っている場合に詳細欄に次番組情報を表示
	1.20	配信要求時、BonDriverが別ストリームで使用中であればストリーム番号を変更するようにした
	1.21	ptTimerに対応（要sqlite3.exeをTvRemoteViewer_VB.exeと同じフォルダに設置）
		iniにptTimer_pathとTvProgramptTimer_NGwordを追加
		iniにBonDriverの複数ストリーム使用を許可するAllow_BonDriver4Streamsを追加
		WEBインターフェースにWI_GET_PROGRAM_PTTIMERを追加
		標準HTMLのindex.htmlを修正しTvProgram_ptTimer.htmlを追加
		番組表が取得できなかったときにエラーが出ることがあったバグを修正
		WI_GET_TVRV_STATUSにptTimer_pathとAllow_BonDriver4Streamsを追加
		VideoSizeCheckバグ修正
	1.22	iniにStop_ffmpeg_at_StartEndとStop_vlc_at_StartEndを追加
		（別用途でffmpegやvlcを使用している場合は0にしてください）
	1.23	iniにEDCBのVelmy版とniisaka版で番組表を表示できるようEDCB_Velmy_niisakaを追加
		（BSが受信できる状態ならば指定しなくとも自動判断できると思います）
		ptTimer番組表作成時に.ch2ファイルに記載されているもののみ表示するようにした
	1.24	倍速ファイル再生に対応
		標準HTMLのSelectVideo.htmlを修正
	1.25	倍速ファイル再生時のハードサブに対応
	1.26	使用中のストリームにおいて字幕付きファイル再生を試みると失敗していたバグを修正
	1.27	ファイル再生時にHLSアプリが応答無しになった場合は停止するようにした（再起動は無駄な可能性大）
	1.28	EDCBの番組情報取得方法を変更
		EDCBのVelmy版とniisaka版をなるべく自動で判別するようにした
	1.29	起動時にCtrlCmdCLI.dlの存在チェックをするようにした
		EDCBの次番組検索をスピードアップ
	1.30	ファイル再生が不安定になるようなので1.27での変更を廃棄した
		ファイル再生時HLS_option_ffmpeg_file.txtが存在しない場合のHLSオプション書き換えを修正
	1.31	可変スピードファイル再生時のハードサブ表示においてコメントが重ならないように調整
	1.32	Tvmaidに対応
		iniにTvmaid_urlとTvProgramTvmaid_NGwordを追加
		WEBインターフェースにWI_GET_PROGRAM_TVMAIDを追加
		標準HTMLのindex.htmlを修正しTvProgram_Tvmaid.htmlを追加
	1.33	標準HTMLにおいてNHKMODE=3(選択式)の場合はNHK以外も選択するようにした
		配信スタート時に渡されるNHKMODEをiniの設定より優先させるようにした
		NHKMODE=4(-map 0,0 -map 0,1)とNHKMODE=5(-map 0,0 -map 0,1)を追加
		配信スタート時に渡されるパラメーターとしてhlsOptAddを追加（上述）
	1.34	hlsOptAddのバグ修正＆-map直後の-が付いた値を考慮
		標準HTMLの音声1～3の修正
	1.35	UDPチャンネル変更に失敗した場合にUDPプロセスが停止しなかったバグを修正
	1.36	NicoJKのコメントファイルを読み込めるようにした
		iniにNicoJK_path,NicoConvAss_path,NicoJK_firstを追加
	1.37	ファイル再生においてエンコード終了後のm3u8終端文字列をチェックすることにした
		動画ファイル名にサービスID情報が含まれている場合にNicoJKコメント機能が働くようにした
		（例：おはようにゃんこ[s0x410].ts　[s1040]でも可。TVTestの保存形式で挿入可能）
	1.38	pt3Timerに対応
		配信を開始するとptTimer番組表が表示されなくなるバグを修正
		標準HTMLのViewTV～.htmlにptTimer&Tvmaid番組表ボタンを追加
	1.39	NicoJKコメント再生でコメント開始時間が正確でなかったバグを修正
		NicoJKコメント開始時間を動画ファイル作成日時にし、iniのNico_delayで微調整することにした
		一旦再生終了し同じファイルを再生させた場合にNicoJKコメントが流れなかったバグを修正
	1.40	録画ファイルの.chapterファイルの内容を読み書きできるようにした（chaptersフォルダの中でも可）
		読み込み：WI_GET_CHAPTER.html?temp=録画ファイルフルパス
		書き込み：WI_WRITE_CHAPTER.html?temp=num,書き込むチャプター文字列
	1.41	EDCBが管理するチャンネル一覧取得をCtrlCmdCLIを使うようにした
		iniに旧方式で取得するオプション(EDCB_GetCh_method)を追加
	1.42	iniにRecTaskがチャンネル変更時に待機する最大秒数(RecTask_CH_MaxWait)を追加
	1.43	TSTaskに対応
	1.44	BonDriver_TSTask.dllを再生候補から外した
		FILEROOTがWWWROOTの子ディレクトリでない場合にソフトサブが機能しなかった現象を解消
	1.45	NicoJKコメントファイルを探すルーチンを改良
		残っていた難視聴関連部分を削除
	1.46	動画再生において動画ファイルのTOTから開始時間を調べるようにした
	1.47	動画再生において過去に同名ファイルを再生していた場合に開始時間を取り違えるバグを修正
	1.48	動画再生において実況ログを使用しない場合はTOTを調べないようにした
		WI_GET_HTML追加
	1.49	iniのMAX_STREAM_NUMBERが8より大きいとファイル再生でエラーになるバグを修正
		ViewTV%NUM%.htmlが存在しない場合はViewTV1.htmlを使用して配信するようにした
	1.50	iniにchapterファイルが無い30分以内の番組でチャプター作成を試みるオプションを追加
		（make_chapterとchapter_bufsec）
	1.51	iniにチューナー初使用時に一旦別のサービスIDに合わせるオプションを追加(openfix_BonSid)
	1.52	起動・終了・全配信停止時にRecTaskとTSTask両方を名前指定終了させていたが使用中のもののみ終了とした
		openfixでダミーサービスIDのchspaceを考慮していなかったことによる配信失敗バグを修正
	1.53	関係の無いURLアクセスを抑制するようにした
	1.54	BonDriverからチャンネルを取得するときにchspaceの違いも考慮するようにした（ptTimer対策。上述）
	1.55	サムネイル作成機能、WI_GET_THUMBNAIL追加
	1.56	WI_FILE_OPEのdir指定でフィルタを利用できるようにした
		WI_STREAMFILE_EXIST追加
		WI_GET_THUMBNAIL修正
		ファイル一括削除を若干高速化
		ファイル名に「,」が入っているファイル再生に失敗していたことに対処
		サムネイル作成時に「#」を「＃」とするところが「♯」になっていたことを修正
	1.57	iniにログの最大文字数を設定できるlog_sizeを追加（標準30000文字）
		WI_WRITE_LOGを追加
		終了時にTvRemoteViewr_VB.logにログを出力するようにした
		ffmpegのシーク方法を変更するファイルを指定する機能をタスクトレイアイコンに追加
		ch_sid.txt内のMX2サービスIDを修正
	1.58	WI_SHOW_MAKING_PER_THUMB追加
		30分待っても等間隔サムネイルの作成が終了しない場合はプロセスを終了させるようにした
		結果を待たずにサムネイルを作成するthru指定を追加
	1.59	NicoJKコメントを探す際に、動画開始時間調査にTOTを使用するにした（以前の移行時に修正漏れ）
		chapter作成で全角キーワードも調べるようにした
	1.60	HTML送信時、まれにエラーが起こっていたので送信方法を変更してみた
		それに伴いiniのHTML_IN_CHARACTER_CODEとHTML_OUT_CHARACTER_CODEを無効にした（UTF-8固定）
	1.61	1.60で２ちゃん実況が文字化けしていたバグを修正
	1.62	iniでHTML発行方法を指定できるようにした(html_publish_method)
		細かい修正
	1.63	HTMLテキスト配信エンコードをUTF-8固定とした
		HTMLテキスト配信方式を1.59以前を標準とすることにした（iniのhtml_publish_methodで変更可能）
	1.64	html_publish_method=0指定時に一部の環境で文字化けしていた現象に対処
	1.65	NicoConvAssでchapterのみ作成を選択しているとファイル再生でコメントが流れないバグを修正
	1.66	iniに動画の長さを取得有無を指定するTOT_get_durationを追加（標準は取得する=1）
		iniにts以外の動画の長さを取得するためのWhiteBrowserWB_pathを追加
		HTML内の%VIDEODURATION%を動画の長さ(秒)に変換するようにした。わからない場合は0
	1.67	iniのWhiteBrowserWB_pathを廃止
		ts以外の動画の長さ取得にffprobe.exeを使用するようにした(ffmpeg.exeと同じフォルダに既に存在）
	1.68	iniにmetaタグRefresh記述を変更するmeta_refresh_fixを追加（Android UCブラウザ対策）
	1.69	サムネイル作成確認をきちんと行うようにした
	1.70	短時間に連続でサムネイル作成しようとすると失敗が続いてしまう現象に対処
	1.71	NicoJKコメントファイル検索ルーチンを修正
	1.72	EDCB番組表取得時にホスト名からIPアドレスへの変換に失敗することがあった不具合を修正
	1.73	ストリーム番号10以上の関連ファイルを誤って削除してしまうバグを修正
	1.74	エンコ済みファイルを消さずに再起動時にストリーム復帰させるようにした
	1.75	ファイル再生復帰情報をエンコードが終了した時点で作成するようにした
	1.76	未設定時の初回起動時に例外エラーが出るバグを修正
	1.77	ハードサブ再生時、再シーク毎にNicoJKログ変換をしていた無駄を修正
		PCクライアントのファイル再生において再シークされたときにエラーが出ていたバグを修正
		http配信時、WatchTV%NUM%.htmlにGETでアクセスすることで直接配信開始できるようにした
		WI_STOP_STREAM.html?num=-3を追加（全停止ただしエンコ済みファイルは削除しない）
	1.78	iniにOPENFIXのダミーチャンネル切り替え時に入れるOPENFIX_WAITを追加
		iniのUDP2HLS_WAITが0の場合はUDPアプリが準備出来次第HLSアプリを実行することにした
		配信準備中に表示されるts数を修正（m3u8が作成されていなくともカウントするようにした）
	1.79	ptTimerとTvmaidの次番組取得不具合を修正
	1.80	Tvmaid YUIに対応（iniにTvmaidYUI_urlを追加）
	1.81	TV配信命令を受け取ったときに既に配信中ならば該当ストリームを表示するようにした
		同じストリームに短時間で複数の配信命令を送った場合に起こる不具合に暫定的対処
	1.82	TVROCK番組表でチューナーを指定できるようにした(iniのTvProgram_tvrock_urlを拡張）
	1.83	num指定がされていない場合の全配信停止が動作していなかったバグを修正
		1.81の重複回避が不十分だったので修正した
		WI_GET_LIVE_STREAM.htmlで、配信準備中のものを記載するようにした
	1.84	最大配信数を大きくしたときにWI_GET_LIVE_STREAMが微妙に遅くなっていた不具合を修正
		NicoJKコメントフォルダにassファイルが存在すればそちらを使用するようにした
		iniに作成したassをNicoJKフォルダ\jk～にコピーするオプションを追加(NicoConvAss_copy2NicoJK)
		1分以内の動画でコメントを再生しようとすると例外エラーが起こっていたバグを修正
	1.85	1.81の同局の場合は配信中のストリームに誘導する機能が再読み込みを考慮していなかったので削除
	1.86	RecTaskが配信失敗した場合の終了処理を見直した
		1.83の重複回避の副作用でRecTaskエラー時にダミーの準備中ストリームが残ってしまうバグを修正
	1.87	iniにサムネイル作成用ffmpegを指定できるようにするオプションを追加（thumbnail_ffmpeg）
	1.88	QSVEncCに対応（実験段階）
		iniにQSVEncCを名前指定で終了させるオプションを追加(Stop_QSVEnc_at_StartEnd)
		iniにファイル再生にffmpeg(thumbnail_ffmpeg)を使用するオプションを追加(video_force_ffmpeg)
		HLSアプリとHLS_option.txtの内容が一致しない場合、再読込を促すダイアログを表示
		フォーム上にHLS_option*.txt群を再読み込みするボタンを設置
		VideoPathに指定されたフォルダ内でのサブフォルダ作成・削除を自動認識するようにした
		ffmpegのHLSオプションファイル内のlibvo_aacencの記述をaacに変更
		HLS_option_QSVEnc.txtとHLS_option_QSVEnc_file.txtを追加（コピーしてください）
	1.89	HLSアプリの個別指定に対応（HLS_option*.txt内の記述＆StartTv.html引数。上述）
		StartTv.htmlへの引数hlsAppSelectを追加（readme_dev.txt参照）
		iniに個別指定用のexepath_VLC、exepath_ffmpeg、exepath_QSCEncを追加
		thumbnail_ffmpegをexepath_ffmpegに名前変更（そのままでも使用可）
	1.90	解像度インデックス内でHLSアプリが指定されている場合はHLS_option.txt内を優先的に検索するようにした
	1.91	HLSアプリ個別選択に伴う細かい修正
		iniのBS1_hlsAppをexepath_VLCに統合（そのままでも使用可）
		タスクトレイアイコンにカーソルを合わせるとバージョンを表示するようにした
		個別指定にPipeRunを追加（Pipe経由QSVEnc）readme参照
		Pipe経由QSVEncファイル再生に対応（同梱のPipeRun.exeをTvRemoteViewer.exeと同じフォルダにコピーしてください）
		Pipe経由再生のためには、iniのvideo_force_ffmpeg=2,exepath_ffmpeg, exepath_QSVEncを指定してください
		video_force_ffmpegの値に3を追加（2に加え再生ファイルがts以外の場合はffmpegで再生する）
	1.92	PipeRun.exeを経由しなくともパイプ処理をできるようにした(PiprRun.exeは削除してOKです）
		コメントファイル探しを若干修正
	1.93	標準HLSアプリがffmpeg以外の場合、video_force_ffmpegを指定してもハードサブが有効にならなかったバグを修正
		標準HLSアプリがffmpeg以外の場合、exepath_ffmpegが指定されていてもffprobeが使用できなかったバグを修正
		QSVEnc2.34の高速シークオプション「--seek」に対応
		iniのvideo_force_ffmpegに4を追加（ts以外・倍速・ハードサブの場合ffmpegを使用）
	1.94	ファイル再生でPipeRunを選択した場合、旧--trimオプションが残ってしまっていたバグを修正
		ハードサブを指定した場合、コメントファイルが無くともffmpegが選択されてしまっていた不具合を修正
		細かいバグフィックス
		NicoJKを使用していない環境でファイル再生出来なかったバグを修正
	1.95	HLSオプション選択課程を整理して見直した
		ファイル再生や個別指定時に解像度指定が無い場合はフォーム上の解像度選択値を使用するようにした
		フォーム上からの実験用にHLSオプションを送るか解像度インデックスを送るかのコンボボックスを設置
		iniでPipeRun時にffmpegに渡すオプションを指定できるようにした（PipeRun_ffmpeg_option）
	1.96	解像度指定が無かった場合でも配信中リストに解像度を記録するようにした
		配信リストには純粋な解像度を記録するようにした（640x360等）
	1.97	解像度コンボボックス変更時のバグを修正
		解像度指定が無かった場合、フォーム上の(解像度orHLSオプション）を送る選択に従うようにした
	1.98	解像度を送るが指定されHLS_option.txtにHLSオプション記述が無い場合の再読み込み失敗に対処
		0x0noenc選択時にffmpegのオプションだと認識されなかったバグを修正
	1.99	ライブ再生で解像度インデックス優先指定が有効になっていなかったバグを修正
	2.00	iniのvideo_force_ffmpegを廃止しフォーム上に移動した
		プロファイルによる再生切り替えに対応
		（フォーム上で「9 プロファイル(profile.txt)に従って再生」を指定した場合）
		profile.txtを追加
		StartTv.htmlへのパラメータとしてprofile=を追加
		WI_GET_PROFILES.htmlを追加
		フォーム上にプロファイル関係のコンボボックスとボタンを追加
	2.01	VLCのファイル再生に対応（音声切替と焼込には未対応）
	2.02	HLSアプリ判定を厳密にした
		プロファイル記述に「-」無指定以外を追加
	2.03	プロファイル記述に「_」無指定を追加
	2.04	BonDriver名の大文字小文字を維持するようにした
	2.05	プロファイル指定において使用HLSアプリ判定がフォーム上のアプリに固定されていたバグを修正
		Waiting.htmlに配信中止ボタンを設置
		QSVEnc2.46の字幕焼込のテストを行えるようにした（要profile.txtの2行目をコメントアウト）
	2.06	tsの動画スタート日時の解析をTOTを元にしPCRで補正するようにした
	2.07	NVEncCに対応
		iniにexepath_NVEncとStop_NVEnc_at_StartEndを追加
	2.08	相対パスを指定できるようにした
		「index.htmlを開く」ボタンを関連付けされたアプリで開くようにした
	2.09	相対パスのバグを修正
	2.10	ファイルオペレーションの一部不具合を修正
		ライブ再生失敗時の再起動回数を制限するようにした（標準3回）
		何度も同じwaitingページが表示された場合にリフレッシュ秒数を伸ばすようにした（標準10回以上で4秒）
	2.11	LTV等でffmpeg http配信ができなくなっていたバグを修正
		サーバー設定取得（WI_GET_TVRV_STATUS）においてBonDriverパスが間違えていたバグを修正（影響無し）
	2.12	HLSアプリ判別で数カ所ffmpeg認識ができていなかったバグを修正
		　起動時のffmpeg-presetチェックがスルーされていた
		　ffmpeg-http配信でファイル再生ができなくなっていた
		　ts以外の動画の長さ等が判別できなくなっていた
	2.13	フォーム上のHLSアプリにffmpeg以外が指定されている場合にサムネイル作成ができなかったバグを修正
	2.14	セキュリティに気を遣ってみた（気分）
	2.15	更にセキュリティに気を遣ってみた（感じ）
	2.16	2.15の細かいバグフィックス
	2.17	ネット上から非推奨バージョン＆推奨バージョンを取得するようにした
		WI_GET_VERSION.htmlを追加
		NVEncがストリーム中、いらなくなったtsファイルの処理を追加（付け忘れ）
	2.18	NicoJKコメント再生時、jkコメントフォルダが存在しないと例外エラーが起こっていたバグを修正
	2.19	セキュリティメンテナンス
	2.20	アップデートチェックの間隔を1時間から6時間に変更
		WI_GET_VERSION.htmlで返されるバージョン番号を小数点2桁で表記するようにした
		アップデートのチェックをするかどうかフォーム上で指定できるようにした
		チェック日時をクライアント毎にまちまちになるようにした
	2.21	ファイル名にシングルコーテーションが含まれているとファイル再生に失敗していた不具合に対処
		相対パス指定において1文字フォルダを受け付けなかったバグを修正
	2.22	iniのVideoExtensionsの拡張子指定で大文字小文字の区別を無くした
		iniのStop_NVEnc_at_StartEndが機能していなかったバグを修正
		ISOファイル再生に対応
		iniにISO再生に使用するVLCを指定する項目を追加（exepath_ISO_VLC）VLC-2.1.2推奨
		ISOファイル再生には古めのmplayerが必要です（要exeをプログラムフォルダにコピー）
	2.23	mplayer-ISO.exeまたはmplayer.exeがプログラムフォルダにあれば認識するようにした
	2.24	TTRecまたは録画プログラムが起動しておらず、番組情報が取得できない環境用にダミー番組表機能を追加
		iniにTvProgram_Force_NoRecを追加
	2.25	ISO再生において音声＆字幕指定が機能していなかったバグを修正
	2.26	ISO再生における字幕指定時のバグをを修正
		ISO再生時のチャプターに対応
	2.27	ffmpegのHTTP配信時にチャンネルを変更すればするほど重くなっていた現象を解消
		WebM形式でブラウザ上でのストリーム配信に試験対応（主に放送視聴用）
		HLS_option_ffmpeg_webm.txtを追加
	2.28	2ちゃん実況の板移転更新に失敗していたバグを修正
	2.29	WebVTT(Nico2HLS)対応を復活（邪魔にはならないはず）
	2.30	ISO再生の確実性を向上
		ISO字幕指定時のバグを修正
	2.31	設定ウィンドウの位置と大きさを記憶するようにした
		iniでログの保存先を指定できるようにした（log_path）
	2.32	ISO字幕指定時の安定性向上
	2.33	iniに×で最小化するオプションを追加（close2min）
	2.34	起動時にViewTV1.htmlが更新されているかチェックし（最も番号の大きいViewTV～.htmlと比較）
		ダイアログで他のViewTV.htmlを更新するか選択できるようにした
		各ファイルは最も番号の大きいViewTV～.htmlと比較され、その内容と同一のものだけが更新されます
		（例：実験的にViewTV5.htmlだけ一部変更していた場合にはViewTV5.htmlは更新されません）
	2.35	リモコン先にローカルIP以外を指定できるようにした（iniのRemocon_Domainsに記述）
	2.36	httpストリーム(WatchTV)アクセス時のパラメータ（Bon_Sid_Chが抜けていたので追加）
		webm配信例としてwebm_sample.htmlを追加
	2.37	WEBAPIを追加（WI_GET_JKNUM、WI_GET_JKVALUE）
		ストリーム番号選択（%SELECTNUM%）タグにidを追加
	2.38	WEBAPIを追加（WI_GET_JKCOMMENT）
		ISO再生用VLCの追加オプションを指定できるようにした（VLC_ISO_option.txtを追加）
	2.39	http配信方法の実験（iniにHTTPSTREAM_METHODを追加）
			iniにアイドル抑止イベントを指定するSTOP_IDLEMINUTES_METHODを追加（2=2.35以前）
	2.40	iniに画面推移方法を指定するTVRemoteFilesNEWを追加（TVRemoteFiles1.82以上）
	2.41	新しいISO再生方式に対応（TVRemoteFiles1.82以上）テスト段階
			iniに（ISOPlayNEW、ISO_DumpDirPath、ISO_maxDump、ISO_ThumbForceMを追加）
	2.42	新しいISO再生方式のバグ修正
			　ffmpegで字幕が指定されたときのエラーを修正
			　サムネイル作成で大きさ指定が無視されていたバグを修正
			　iniのISO_ThumbForceMを廃止
	2.43	VLC_ISO_option.txtの廃止
			iniにVLC_ISO_optionを追加
	2.44	新ISO再生終了指令時にVOB数のチェックを行うようにした(ISO_maxDumpは1以上推奨）
	2.45	新ISO再生に失敗した場合にしばらく同ストリームが使用できなくなるバグを修正
	2.46	フォーム上でログに出力する項目を選択できるようにした
			クライアントIPをログに表示
			終了時のログ出力に表示項目適用TvRemoteViewer_VB_edited.logを追加
			配信開始時に%FILEROOT%フォルダの存在を確認するようにした
	2.47	%FILEROOT%フォルダが消失した場合に例外エラーが起こる不具合に対処
	2.48	フォーム上にQSV,NVログ出力チェックを設置した（ストリームフォルダに出力されます）
	2.49	新ISO再生：一部日本語ISOファイルがQSVで再生できない事への対処
			新ISO再生：字幕指定が無効の時に起動失敗していたものを、字幕なしで起動するためのロジック変更
	2.50	旧ISO再生においてiniで指定されたQSVEncCやNVEncCがx64だった場合にエラーになっていたバグを修正
	2.51	ISO再生が可能かどうかチェックを厳しくした
			iniでmplayer.exeへのパスを指定できるようにした（無指定は従来通りプログラムフォルダを探す）
	2.52	ISO再生用デバッグ機能追加（フォーム上のDebugチェックボックス）
			HLS_option_NVEnc_file.txtを修正（「-m hls_list_size:」を0に修正）
	2.53	将来的に外部プログラムへ提供できるようWI番組表にジャンル記入等の機能追加
			クライアントによるnicovideo.jpへのアクセス許可を追加
			番組表データをキャッシュすることによるページ表示速度の向上
	2.54	番組表キャッシュのバグを修正
			無駄に番組表を作成することがあったバグを修正
			iniにキャッシュ制御を行わないようにするオプションを追加（NoUseProgramCache）
	2.55	次番組のジャンル情報提供について細かい修正
	2.56	EDCBの次番組ジャンルが不正確だったバグを修正
	2.57	新規インストール時に起動エラーになっていたバグを修正
	2.58	TVRVLauncher用に地デジ番組表にAbemaTVを追加（要：iniのTvProgramDに801を追加）
			AbemaTV番組情報取得先を選択できるようにした（iniにAvemaTV_data_get_methodを追加）
			AbemaTV番組情報取得先を個別指定できるようにした（iniにAbemaTV_CustomURLを追加）
			番組表キャッシュがNext分の違いを考慮していなかった不具合を修正
	2.59	EDCB等の番組情報内の改行を処理（今更）
			番組情報キャッシュが思い通りにいかなかった原因が判明したので修正
	2.60	取得したAbemaTV番組情報データが番組情報を含んでいるかしっかりチェックするようにした
	2.61	数時間毎のAbemaTV番組情報取得が行われていなかったバグを修正
	2.62	Tvmaid MAYAに対応
	2.63	ptTimerの番組情報にジャンルを追加
	2.64	AbemaTV番組情報サイトが落ちている場合に情報提供元が切り替わらなかったバグを修正
	2.65	TVRVLauncherからAbemaTV番組情報キャッシュをクリアできるようにした
			AbemaTV番組情報のチェックを厳しくした（途中までしか送信されて来ない場合に対処）
	2.66	複数の配信停止命令が同時に処理された場合に起こっていた不具合を修正
			TVRemoteFilesNEW=1使用時にHTTP配信への応答が従来と異なっていた不具合を修正
			iniファイル上の設定をフォーム上から動的に変更できるようにした
			iniファイルの標準添付を廃止し上書きで設定を無くしてしまう事故を防止するようにした
			TvRemoteViwer_VB.ini.dataとTvRemoteViwer_VB.ini.defaultを添付
	2.67	iniのclose2minの選択肢を追加(2=×で最小化＆Alt+Tabに非表示)
	2.68	ネット放送局名変換指定がされていない場合の推測機能を地デジへの変換限定にした
			ネット放送局にAbemaTV(801)単独で指定するとAbemaTV番組表が表示されないバグを修正
	2.69	ffmpegストリーム配信時にストリーム番号のチェックをスルーしていたバグを修正
			TvRemoteViewer_VB.ini.dataのコピーし忘れ時にini更新ボタンを使用不可とした
			起動時のログからエラーと警告を抽出してini項目詳細欄に表示して注意喚起するようにした
	2.70	iniファイルへの新規項目追加と値のセットが同時に出来なかったバグを修正
			iniファイルへの新規項目追加で説明中に改行が入っているとコメントアウトされなかったバグを修正
			TvRock番組表へのジャンル追加（標準色以外にカスタマイズしているならばiniのTvRock_genre_colorを追加）
	2.71	Tvmaid番組表において次番組の取得が出来ない場合があったバグを修正
			TvRock番組表で予約が入っていると番組が無視されていたバグを修正
			iniに次の次の番組を番組表に表示するオプションを追加（next2_minutes）
	2.72	TvRock番組表ジャンルの精度アップ＆予約中番組のジャンル取得が可能になった
			TvRockジャンル色は使用しなくなったのでiniのTvRock_genre_colorを廃止
			iniにTvRockジャンル取得を行うかを指定できるようにした。標準はしない（TvRock_genre_ON）
	2.73	TvRock番組表ジャンルが0時またぎで判別できないので2.71に戻して精度アップだけを行った
			希にバージョンチェックで例外エラーが起こるバグを修正
	2.74	TvRockジャンル取得を番組表＆検索結果の両方から取得するようにした
			（iniのTvRock_genre_colorとTvRock_genre_ONを改めて設置しジャンル取得を標準とした）
			TvRock番組表で予約が入っていると番組が無視されてしまうバグがまだ直っていなかったので修正
			TVRVLauncherからのキャッシュクリア要求に対して全ての番組情報をクリアするようにした
	2.75	TvRemoteViewer_VB.ini.dataのMIME_TYPE_DEFAULTの説明文が間違っていたので修正
			TvRemoteViewer_VB.ini.dataに項目MIME_TYPEが記載されていなかったので追加
			上記記載ミスがiniに記載されている場合は「ini 更新&適用」ボタンが押された際に修正するようにした
	2.76	番組表内の時刻表示を統一した(H:mm）
			ini設定画面レイアウトを変更して見やすくし説明文等を修正した
	2.77	ini設定画面上でファイルやフォルダをダイアログで選択できるようにした
	2.78	TvRock番組表の取得に失敗することがあったバグを修正
	2.79	TOT補正に使用するPCRの値が不自然な場合にファイル作成日時を動画開始日時とするようにした
	2.80	TvRock番組表で英数字のみの番組名でエラーが発生していたバグを修正
			AbemaGraphさんから番組表を取得する際にタイムスタンプを送らないようにした(2.80c)
	2.81	標準除外BonDriverだったBonDriver_UDPとBonDriver_TSTaskを除外しないことにした
			上記「EDCBのUDP送信を受信するには」を参照のこと
			TvRockジャンル判定精度向上（2.81b）
	2.82	iniのBonDriver_NGwordが機能していなかったバグを修正
	2.83	ptTimer番組表に改行が含まれていたバグを修正
			EDCB番組表の内容が不完全なときがあったバグを修正
			TVRVLauncher用に放送局別番組表を提供出来るようにした
			TvRock放送局別番組表取得時にd:\tvrock.htmlというデバッグ用ファイルを作成してしまっていた（消してください）
			HLS_optionの各行頭に「;」か「#」があれば無効にするようにした（2.83c）
			放送局別番組表の予約チェックを厳密にした。特にTvRock（2.83d）
	2.84	AbemaTVの放送局別番組表にサムネイル情報を追加
			TvRockとEDCBの予約情報に同一局同一時間で有効と無効が同時に登録されている場合に対処（2.84b）
			TvProgram_tvrock_schが無効になっていたバグを修正（2.84b）
	2.85	録画ファイル一覧取得において、更新されたフォルダのみ更新するようにした
			スピードアップのため録画ファイルのサイズチェックを廃止した（2.85b）
			録画ファイル一覧更新時に並び替えを忘れていたバグを修正（2.85c）
			HLS_option読み込みを厳密にした（2.85d,2.85e）
			HLS_optionにおいて、2.83c以降パラメーター無しの解像度指定を受け付けなかった不具合を修正（2.85f）
	2.86	AbemaTVのジャンル分けにてきとーに対応（ini AbemaTV情報取得先 AvemaTV_data_get_method=2）
			AbemaTV番組表がうまく更新されているかチェックするルーチンを修正（2.86b）
			TvRockで番組タイトルが半角のみの場合に上手くジャンル判定ができないことがあったバグを修正（2.86c）
			設定によってはAbemaTV番組表取得に失敗していたバグを修正（2.86d）
			Abemaアニメ2が長時間表示されない現象に対処。それに伴い優先情報取得先をカッパに変更（2.86e）
			Windowsの日付形式が標準と違う場合に対処（2.86f）
			EDCB番組情報取得にservice_type=1以外も含めるようにした（2.86g）
			インターネット番組表が表示されなくなっていた不具合に対応（2.86h）
			SPHD StarDigioの番組表表示に対応（番組データが無い場合は放送局名のみ表示するStarDigio_dummy_ONをiniに追加）（2.86i）
			UDPアプリ＆HLSアプリのCPU優先度にBelowNormalとAboveNormalを追加（2.86j）
	2.87	iniのTvmaidYUI_urlで末尾に/が無い場合に起こっていた不具合を修正
			Tvmaid,ptTimer番組表でBS-TBSまたはQVCが表示されていなかった不具合を修正
			放送局別番組表でBS-TBSとQVCが判別できていなかった不具合を修正
			番組情報に次の次の番組ジャンル情報を付加するようにした（2.87b）
			TvRock番組表でエンタメ～テレの終了時間がおかしくなる不具合を修正（2.87c）
			iniに再生中のスリープを抑止するオプションを追加（viewing_NoSleep）（2.87d）
			iniにAbemaTV番組表取得間隔設定を追加（AbemaTV_Program_get_interval_min）（2.87e）
			起動時チェックでch2ファイルがShift_JISでなければログに警告を表示するようにした（2.87e）
	2.88	設定画面上でのini適用時に補助プログラムが存在すれば再起動するようにした
			ini適用時にTvRemoteViewer_VBを再起動させる補助プログラムTvRemoteViewer_VB_r.exeを添付
			ch2ファイルの文字コードがShift_JISでない場合に変換を試みるか起動時に尋ねるようにした（2.88b）
			ISO再生時にサムネイルが表示されなくなっていたバグを修正（2.88c）
			udpアプリがTSTaskの場合はch2文字コード警告ダイアログを表示しないようにした（2.88d）
			ネット番組表表示に不具合が発生する可能性を修正（2.88e）
			次番組と次の次の番組の判定を修正（長時間の番組休止対策）（2.88f）
			iniに各ビデオフォルダ監視用バッファサイズ指定オプションを追加（watcher_BufferSize）（2.88g）
			設定画面のBonDriver選択欄横に優先順位ボタンを設置した（2.88h）
			BonDriver優先順位が再起動しないと反映されなかったバグを修正（2.88i）
			TvRock放送局別番組表でエンタメ～テレの放送中番組が表示されない不具合を修正（2.88j）
			TvRock番組表ver2環境ならば数時間内の予約番組のジャンル判定精度を向上（2.88k）
			2.88kで反応が鈍くなっていた不具合の修正（2.88m）
			TvRock番組表ジャンル判定の長時間対応＆精度向上（2.88m）
			TvRock放送局別番組表の先頭番組の予約精度の向上（2.88n）
			AbemaTV情報を容量削減のためzipで取得するようにした。（2.88p）
			ICSharpCode.SharpZipLib.dllを添付。exeと同じフォルダにコピーしてください（2.88p）
			TvRockジャンル判定の調整（2.88p）
			ジャンル判定のためのTvRockへのアクセス回数を削減（2.88q）
	2.89	既存のコメントファイルが無い場合NicoConvAssでコメントをダウンロードしてassを作成できるようにした
			iniにコメントをダウンロードするかを指定するNicoConvAss_assData_downloadを追加
			設定画面に「NicoConvAss設定セット」選択欄を設置（2.89b）
			TvRockジャンル判定の正確性向上（2.89c）
			iniに同一パラメーターが複数回記入されている場合はログに警告を記載するようにした（2.89d）
	2.90	VCEEncに対応（iniにexepath_VCEEncを追加）
			サーバーPC上で実行中のHLSアプリのプロセス数を調べるWI_GET_HLS_APP_COUNTを追加（2.90b）
			%FILEROOT%のチェックが甘かった不具合を修正（2.90c）
	2.91	チャプター判別時にAが無くともBがればチャプターを作成するようにした
			iniにTSID_in_ChSpaceを追加（実験）
			設定画面にTvRemoteFilesによる「データ更新許可」チェックボックスを設置（2.91b）
			アクセスログ機能を追加。タスクトレイアイコン右クリックとフォーム上にボタンを設置（2.91b）
			アクセスログのリクエスト内容を詳しく表示するようにした（2.91c）
			アクセスログにUserAgentを表示（2.91d）
			起動時に設定画面の大きさがアクセスログの大きさになってしまうバグを修正（2.91d）
			チャプター作成の調整（「ｷﾀｰ」が少なくなり「ﾊｼﾞﾏﾀ」や「はじまた」が多くなったことへの対処）（2.91e）
			iniにTvRemoteViewer_VB作成チャプターを既存のチャプターより優先させるchapter_priorityを追加（2.91f）
			iniのchapterポイントを何秒早めるか(-chapter_bufsec)に少数点を設定できるようにした（2.91g）
			おまけ機能：iniにtsリネーム時にchapterファイルもあわせてリネームするtsRenameSyncChapterを追加（2.91h）
			WI_GET_VIDEOFILES2.htmlに機能追加（2.91i）
	2.92	WI_GET_VIDEOFILES2.htmlに機能追加
			2.91iのWI_GET_VIDEOFILES2でフォルダ作成・削除時に録画ファイル一覧に反映されなかったバグを修正
			WI_GET_VIDEOFILES2でフォルダ名変更時に録画ファイル一覧に反映されなかったバグを修正（2.92b）
			WI_GET_TVRV_STATUSにTSID_in_ChSpaceを記載（2.92c）
			終了後のTvRemoteViewer.logにアクセスログを記載（2.92d）
			チャプター作成でA,Bどちらかが存在しない場合にｷﾀｰが削除されてしまうバグの修正（2.92e）
			WI_GET_VIDEOFILES2の修正（2.92f）
			ビデオフォルダ更新の修正（2.92g）
			ビデオファイルやフォルダの更新作業が集中しないよう修正（2.92h）
			WI_TVRV_STATUSでエラーになる場合があるバグを修正（2.92i）
			現在の番組データが無い場合に、未来の番組が現在欄に表示されてしまっていたバグを修正（2.92i）
			放送局別番組表に放送無し時間帯があれば補完するようにした（2.92j）
			放送局別番組表リクエスト時にTSIDを指定できるようにした（EDCB,Tvmaid）（2.92j）
			WI_GET_VIDEOFILES2でカレントフォルダの判定ができていなかったバグを修正（2.92j）
			2.92jでTvRock番組表で時間がずれたものが表示されることがあるバグを修正（2.92k）
	2.93	キャッシュ有効時の先読み機能を実装（ページ表示の高速化。待たされることがほぼなくなります）
			キャッシュ有効時にネット番組表を詳しく取得するようにした
			放送局別番組情報の単純キャッシュ機能を追加
			TvRockは先読みしないことにした（次番組の番組詳細データが取得出来ないため）（2.93b）
			AbemaTV用ジャンル指定番組表(WI_GET_1GENRE_PROGRAM)を追加（2.93c）
			AbemaTV用ジャンル指定番組表のジャンルその他に対応（2.93d）
			NicoJKファイル優先時、無コメント動画に関係無い英数字タイトル動画のコメントが流れてしまう可能性を軽減（2.93e）
			勘違いで修正した（2.92gビデオフォルダ更新の修正）を元に戻した（2.93f）
			ビデオフォルダ更新作業中にエラーが発生する場合があることへの対処（2.93f）
			カンマ,が入ったファイルの日付がおかしくなるバグを修正（2.93f）
			放送休止チェック時にエラーが記録されることがあった箇所を修正（2.93f）
			従来通りの単純ファイル一覧取得要求には2.92e以前の旧ルーチンを使用するようにした（2.93g）
			2.92gで修正した録画フォルダ追加ルーチンを元に戻した（2.93h）
			WI_GET_TVRV_STATUSの項目名TSID_in_ChSpaceをt_i_cに変更（2.93i）
			番組表取得時エラーへの対応（要Framework4.5.2）（2.93j）
			環境を要Framework4.5.0に移行（そのほうがインストール済みの方が多いため）（2.93k）




※ConnectedSelect.jsはhttp://d.hatena.ne.jp/Mars/20071109のスクリプトを使用させていただきました。
※ch_sid.txtはNicoJKPlayModのjkch.sh.txtを参照し修正を加えたものです。作者様ありがとうございます。
※CtrlCmdCLI.dllはEDCBに添付されていたものです。作者様及び派生版の作者様ありがとうございます。
