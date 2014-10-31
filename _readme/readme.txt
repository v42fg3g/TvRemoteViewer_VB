TvRemoteViewer_VB v0.77


チューナー数だけ平行起動してパパッとチャンネルを変更しようと思ったが4つでCPU100%・・



■これってなに？

	離れたとこからPC上のテレビや動画を見るためのソフトです



■環境

	Windowsが動作するPC
	FrameWork4.0
	
	RecTask
	ffmpegもしくはVLC



■起動

	★注意★
	Windows8以降では以下のどちらかの操作が必要です。
	・TvRemoteViewer_vb.exeを右クリック、
	　「プロパティ」→「互換性」→「管理者としてこのプログラムを実行する」にチェック
	・コマンドプロンプトから
	　netsh http add urlacl url=http://+:40003/ user=XXXXX
	　(XXXXXは実行するユーザー、もしくは Everyone と入力する)


	設置後、起動するとタスクトレイからスタートします。
	ダブルクリックで通常の大きさになります。
	各パラメーターはreadme.jpg参照のこと
	プログラミングするときはForm1のWindowStateをNormalにしておくと通常の大きさで起動しますから便利でしょう。



■視聴＆操作

	実際の視聴や操作はWEB上で行います。
	例：	PC（IPアドレス 192.168.1.100　ポート40003)、でTvRemoteViewer_VBを実行している場合は
		リモート機（スマホなど）のブラウザでhttp://192.168.1.100:40003にアクセスしてください
		ローカルPCでテストしている場合はhttp://127.0.0.1:40003です



■設置 readme.jpgや下に書いたテスト環境を参考にしてください

	・RecTaskとBonDriverを適切に配置


	・ffmpeg又はVLCをインストール
	（★ffmpegを使う場合は同梱のlibx264-ipod640.ffpresetをffmpegをインストールしたとこのpresetsフォルダにコピー）


	・同梱のform_status～.txtとHLS_option～.txtをTvRemoteViewer_VB.exeと同じフォルダにコピー


	・WWWROOT（WEBルートフォルダ）となるフォルダに同梱のHTMLフォルダ内のファイルをコピー
	（★VLC使用の場合は半角スペースが入らない場所のほうが安心です）


	・ファイル再生させるためには、あらかじめ同梱のVideoPath.txtを編集して動画があるフォルダを指定しておく必要があります。

	・地デジ番組表の設定	VideoPath.txtを編集してください

	・TvRock番組表につきまして
		1.ブラウザで
		　http://[TvRockが稼働しているPCのローカルIP]:[TvRockのポート]/[ユーザー名]/iphone?md=2&d=0
		　（例　http://127.0.0.1:8969/ユーザー名/iphone?md=2&d=0）
		　にアクセスする。
		　すると、iPhone用番組表が表示されます。
		2. 「予約表示」数を「無し」にする。無し以外でも構いませんが反応が遅くなる可能性があります。
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


	【安定性実験】
	・UDP,HLS各exeを配信ナンバー毎に違うexeを使用できるようにした。
　		exeが存在するフォルダ名に配信ナンバーを追記したフォルダ内のexeを使用します。
　		例：HLSアプリにffmpegを使用している場合
　　		通常～\bin\ffmpeg.exeを使用しているときに
　　		～\bin1\ffmpeg.exeを用意しておけば配信1のときに使用するようになります。
　　		～\bin2\ffmpeg.exeを用意しておけば配信2のときに使用するようになります。
　　		UDPアプリにつきましても同様です。



■WEBデザインを変更したい場合

	・index.html
	index.htmlからStartTv（配信開始）が呼び出される際にWEBから送られるパラメーターとしては今のところ以下を想定しています。
	WebRemoconvb→Web_Start()内を編集すれば違う動作や異なるWEB設計にもできるでしょう。
	パラメーター	valueの例
	"num"		"1"					ストリームナンバー
	"BonDriver"	"BonDriver_pt2_t0.dll"			BonDriverネーム
	"ServiceID"	"54321"					サービスID
	"ChSpace"	"0"（CSは1)				チャンネルスペース
	"resolution"	"640x360"				解像度（縦横の組み合わせは決まっています）
	"Bon_Sid_Ch"	"BonDriver_pt2_t0.dll,54321,0"		上記３つを同時に設定
	"redirect"	"ViewTV2.html"				配信開始後ジャンプするページ


	・index.html、ViewTV[n].htmlで使用できる変数
	html内に以下の変数を記入しておくと呼び出されたときに適切な値またはHTMLに変換されます
	変換前				変換後
	%NUM%				ストリームナンバー
	%SELECTBONSIDCH%		index.html内でBonDriver＆ServiceID&ChSpaceを選択する<SELECT>セットを作成
	%PROCBONLIST%			配信中のストリームナンバーとBonDriverをテキストで表示する
	%VIEWBUTTONS%			ストリームの数だけ視聴ボタンを作成
	%SELECTNUM%			ストリームナンバー選択
	%SELECTRESOLUTION%		解像度選択
	%IDPASS%			「ユーザー名:パスワード@」に変換（iniでALLOW_IDPASS2HTML=1のとき）
					使用例　http://%IDPASS%" + location.host + "/%FILEROOT%mystream%NUM%.m3u8";
					IEなどではセキュリティ設定でURL内パスワードを許可しないと見れなくなります


	・ViewTV[n].htmlのみで使用できる変数
	%SELECTCH%			ViewTV.html内で番組を選択する<SELECT>を作成する
	%WIDTH%				ビデオの幅
	%HEIGHT%			ビデオの高さ
	%FILEROOT%			.m3u8が存在する相対フォルダ
	%SUBSTR%			Nico2HLSによってニコニコ実況コメント取得中ならば"_s"に変換される


	・なお、%PROCBONLIST%、%SELECTCH%、%VIEWBUTTONS%、%SELECTBONSIDCH%、%SELECTNHKMODE%、%SELECTRESOLUTION%　に対しては、要素の前中後に表示するhtmlタグを指定できます。
	書式	%VIEWBUTTONS:[前方に表示するhtmlタグ]:[ボタンとボタンの間に表示するhtmlタグ]:[後方に表示するhtmlタグ]:[要素が表示されない場合に替わりに表示されるhtmlタグ]%
	例	%VIEWBUTTONS:視聴可能ストリーム<br>:<br>===================<br>:ボタンを押してください<br>%
	結果	視聴可能ストリーム
		[ストリーム1を視聴]
		===================
		[ストリーム2を視聴]
		ボタンを押してください


	・Waiting.html
	%NUM%				ストリームナンバー
	%WAITING%			メッセージ（配信準備中 or 配信されていません）


	・ERROR.html
	%NUM%				ストリームナンバー
	%ERRORTITLE%			エラーページタイトル
	%ERRORMESSAGE%			エラーメッセージ
	%ERRORRELOAD%			プログラムから指定された場合に再読込ボタンを表示する



■HLSアプリにつきまして

	・ffmpegをご使用の場合【推奨】
	http://ffmpeg.zeranoe.com/builds/
	うちではx64最新版を使用。４つまでは同時起動確認（古いバージョンでは3つ以上は不安定でした）
	ffmpegインストール先のpresetsフォルダ内に同梱のlibx264-ipod640.ffpresetをコピーしてください。
	参考：http://frmmpgit.blog.fc2.com/blog-entry-179.html
	640x360以外の解像度は試していません。
	極希に新たなフレームが読み込まれずm3u8が更新されなくなる現象有り


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



■Windows上でのm3u8再生につきまして

	「地デジのロケフリシステムを作るスレ part3」に書かれてます



■テスト環境

	Windows7 x64
	VisualStudio2010

	RecTaskインスコ	=	D:\TvRemoteViewer\TVTest
	BonDriver	=	D:\TvRemoteViewer\TVTest
	%WWWROOT%	= 	D:\TvRemoteViewer\html
	%FILEROOT%	=	D:\TvRemoteViewer\html
	%HLSROOT%	=	D:\TvRemoteViewer\ffmpeg-20140628-git-4d1fa38-win64-static\bin
	%HLSROOT/../%	=	D:\TvRemoteViewer\ffmpeg-20140628-git-4d1fa38-win64-static
	(%HLSROOT%)	=	D:\TvRemoteViewer\vlc

	iPad(第3世代）iOS7 safari、Android(Nexus7旧)



■修正したり追加したりして欲しいところ

	・全般的にクラスというものがわかってない・・お～まいがっ


	・想定外のエラー処理。テスト不足


	・ffmpegの穏便な終了


	・iPadでの自動再生


	・HLSアプリが動いてるふりしながら停止しているときへの対処
	まぁ、配信が止まると見てるほうも止まるから再読込や停止しますよね。
	なのでとりあえずは対処しなくてもいいかなと放置してます。


	・Windows上での各種ニコニコ生放送ソフトとの連携（希望）
	表示タイミングの調整
	こっちがチャンネル変えたら向こうも変わるとか
	実況ないとつまらないです
	でも・・作ってみて思いました。使ったことないけどSplashtopが一番現実的かな～・・orz



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



※ConnectedSelect.jsはhttp://d.hatena.ne.jp/Mars/20071109のスクリプトを使用させていただきました。
