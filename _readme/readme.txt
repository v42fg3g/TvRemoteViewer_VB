TvRemoteViewer_VB v0.33


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
		　http://[TvRockが稼働しているPCのローカルIP]:[TvRockのポート]/[ユーザー名]/iphone
		　（例　http://127.0.0.1:8969/ユーザー名/iphone）
		　にアクセスする。
		　すると、iPhone用番組表が表示されます。
		1. BS/CSを受信しているチューナーを選択する
		　（Fireworksではうまくチューナーが切り替わらないかもしれません）
		2. 「予約表示」数を「無し」にする。まぁ無し以外でもいいですけど
		　ここで表示された番組表がデータとして使用されます。
		3. ブラウザを閉じる

	・EDCB番組表につきまして
		EpgTimer.exeが存在するフォルダにあるEpgTimerSrv.iniを開いて[SET]直後に
			EnableHttpSrv=1
			HttpPort=5510
		を書き加えてEpgTimerを再起動してください
		参考　http://blog.livedoor.jp/kotositu/archives/1923002.html



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
	html内に以下の変数を記入しておくと呼び出されたときに適切な値に変換されます
	変換前				変換後
	%NUM%				ストリームナンバー
	%SELECTBONSIDCH%		index.html内でBonDriver＆ServiceID&ChSpaceを選択する<SELECT>セットを作成
	%PROCBONLIST%			配信中のストリームナンバーとBonDriverをテキストで表示する
	%VIEWBUTTONS%			ストリームの数だけ視聴ボタンを作成


	・ViewTV[n].htmlのみで使用できる変数
	%SELECTCH%			ViewTV.html内で番組を選択する<SELECT>を作成する
	%WIDTH%				ビデオの幅
	%HEIGHT%			ビデオの高さ
	%FILEROOT%			.m3u8が存在する相対フォルダ


	・なお、%PROCBONLIST%、%SELECTCH%、%VIEWBUTTONS%、%SELECTBONSIDCH%、%SELECTNHKMODE%　に対しては、要素の前中後に表示するhtmlタグを指定できます。要素自体が存在しない場合は表示されません。
	書式	%VIEWBUTTONS:[前方に表示するhtmlタグ]:[ボタンとボタンの間に表示するhtmlタグ]:[後方に表示するhtmlタグ]:[要素が表示されない場合に替わりに表示されるhtmlタグ]%
	例	%VIEWBUTTONS:視聴可能ストリーム<br>:<br>===================<br>:ボタンを押してください<br>%
	結果	視聴可能ストリーム
		[ストリーム1を視聴]
		===================
		[ストリーム2を視聴]
		ボタンを押してください



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
	"%UDPPORT%"	ソフトで自動的に割り当てられたudpポート
	"%WWWROOT%"	WWWのrootフォルダ
	"%FILEROOT%"	m3u8やtsが作成されるフォルダ
	"%HLSROOT%"	HLSアプリが存在するフォルダ
	"%HLSROOT/../%"	HLSアプリが存在するフォルダの１つ上の親フォルダ（ffmpeg解凍時のフォルダ構造に対応）
	"%rc-host%"	"127.0.0.1:%UDPPORT%"に変換されます。



■Windows上でのm3u8再生につきまして

	「windows hls 再生」でググればでてくる日本語ページから再生用flashをダウンロードしてhtmlを編集してあげればWindows上でm3u8が再生できます



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



※ConnectedSelect.jsはhttp://d.hatena.ne.jp/Mars/20071109のスクリプトを使用させていただきました。
