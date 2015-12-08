TvRemoteViewer_VB v1.73


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

	・ptTimer番組表につきまして
		sqlite3.exeをTvRemoteViewer_VB.exeと同じフォルダに設置してください
		sqlite3.exeはググればすぐ見つかると思います


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
	%VIDEOSEEKSECONDS%		ファイル再生時にシークする秒数
	%SELECTVIDEO%			ビデオファイル一覧HTML部品
	%VIDEOFROMDATE%			ビデオファイル一覧を表示した際の一番古いファイルの更新日時「yyyy/MM/dd」
	

	・ViewTV[n].htmlのみで使用できる変数
	%SELECTCH%			ViewTV.html内で番組を選択する<SELECT>を作成する
	%WIDTH%				ビデオの幅
	%HEIGHT%			ビデオの高さ
	%FILEROOT%			.m3u8が存在する相対フォルダ
	%SUBSTR%			Nico2HLSによってニコニコ実況コメント取得中ならば"_s"に変換される
	%JKNUM%				ニコニコ実況のチャンネル文字列（例：jk8)
	%JKVALUE%			ニコニコ実況用接続用文字列


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
	%VIDEOFILE%	ビデオファイルに変換（実際は「-i %VIDEOFILE%」の決め打ちで-iの後ろの文字列がファイル名に変換）
	%VIDEODURATION%	ビデオの長さ(秒)　不明な場合は0

	・StartTv.html呼び出し時のオプション	
	hlsOptAdd	配信時のHLSオプションに動的にパラメーターを追加できます
	hlsOptAdd=[1～2],[1～4],[文字列]
	例：hlsOptAdd=2,2,-map 0,0 -map 0,1
	また、「_-_」で区切ることにより複数の書き換えを行うことができます
	例：hlsOptAdd=2,9,-hls_-_2,2,-map 0,0 -map 0,1	（-hls部分を削除した後に-map～を追加）
	第1パラメータ：	1=HLSオプションの-iより前に文字列を追加します
			2=HLSオプションの-iの後に文字列を追加します
	第2パラメータ：	HLSオプション上に同じパラメータがあった場合にどうするか
			1=変更しない
			2=既存のHLSオプション上のパラメータを破棄し新しく追加します
			3=既存のHLSオプションの要素に追加を試みます（例：-vf a→-vf a,b)
			4=単純に追加
			9=指定パラメータ部分を削除

	・NHKMODE 詳細
		'0=主・副　HLSオプション変更無し
                '1=NHKならば主　NHK以外は主・副
                '2=NHKならば副　NHK以外は主・副
		'3(ini限定)=選択式
                '4=全ての放送局で音声1 -map 0:0 -map 0:1
                '5=全ての放送局で音声2 -map 0:0 -map 0:2
                '6=全ての放送局で音声3 -map 0:0 -map 0:3
		'9=NHKならばVLCで再生
                '11=全ての放送局で主
                '12=全ての放送局で副



■WEBインターフェース（一部　その他はclientのreadme.txt参照のこと）
	
	WI_GET_VIDEOFILES.html	ビデオファイル一覧HTML部品を返す
	WI_GET_VIDEOFILES2.html	ビデオファイル一覧をテキストで返す
		上記２つのインターフェース用パラメーター：
		vl_refresh	1=強制ビデオファイル更新
		vl_startdate	指定日より前のビデオファイルを抽出する
		vl_volume	何件表示するか（最終日付のファイルを追加するので不正確）
		上記パラメーターは%SELECTVIDEO%を変換するSelectVideo.htmlにも有効
	WI_FILE_OPE.html	ファイル読み書き(UTF-8)
		パラメーター：
		fl_cmd		dir, read, write, write_add, delete
		fl_file		フォルダ名又はファイル名（%WWWROOT%からの相対位置）
		fl_text		書き込む内容
		temp		dirの場合のフィルタ(無指定の場合は「*」)　例：「*.jpg」や「mystream*」
		結果：
		0,SUCCESS(+改行[結果])　又は　2,[エラー内容]
	WI_STREAMFILE_EXIST.html?fl_file=[ファイル名]
		ストリームフォルダ内にファイルが存在するかどうか
		例：WI_STREAMFILE_EXIST.html?fl_file=mystream1_thumb.jpg
　　　　　　　　　　WI_STREAMFILE_EXIST.html?fl_file=file_thumbs/動画ファイル名.jpg
		返値：　存在すれば1、存在しなければ空白
	WI_GET_PROGRAM_[TVROCK,EDCB,PTTIMER].html(?temp=1-3)
		TVROCK,EDCBから番組表を取得
		オプション temp=1～3 を指定することにより次番組が存在すれば併せて取得(PTTIMERには未対応）
		1:返値の各番組情報記述は従来通り
		2:返値の各番組情報内の次番組名冒頭に「[Next]」を付加
		3:返値の各番組情報末尾に現番組「,0」か次番組「,1」かを付加
		4以上:番組終了までtemp分以内しか残っていない場合は現番組の詳細欄に次番組情報を表示（データは無指定と同じ）
	WI_GET_CHAPTER.html?temp=録画ファイルフルパス
		録画ファイルの.chapterファイルの内容を取得（chaptersフォルダの中でも可）
	WI_WRITE_CHAPTER.html?temp=num,書き込むチャプター文字列
	WI_GET_HTML.html?temp=[HTML取得方法],[エンコード],[UserAgent],http://www.google.co.jp/
		HTML取得方法	1: webbrowser UserAgent無効。エラーにより現状使用不可
				2: webclient
				3: HttpWebRequest
		例：WI_GET_HTML.html?temp=2,UTF-8,,http://www.google.co.jp/
	WI_GET_THUMBNAIL.html?temp=[作成ソース],[秒数指定],[幅],[縦]
		ファイル再生中動画のサムネイルを作成
		パラメータ
			[作成ソース]	配信中のストリームナンバー、もしくは動画フルパスファイル名（ローカルパス）
					ファイル名指定の場合はストリームフォルダ内のfile_thumbsというフォルダ内に、
					ファイル名を使用してjpgが作成されます
					■重要■ファイル名に#(半角)が含まれていた場合＃(全角)に変換されます
						（URLアクセスができないため）
			[秒数指定]	単独、「:」区切りで複数、thru[秒数指定]、per[等間隔秒数]
					等間隔を指定した場合は、結果を待たずに返値が返されます
					また、thru[秒数指定]で結果を待たずに返値が返されます
			[幅],[縦]	縦横に0を指定した場合はffmpeg標準の大きさのjpgが作成されます
		返値
			単独　 [ストリーム出力フォルダ]/mystream%NUM%_thumb.jpg
			複数　 [ストリーム出力フォルダ]/mystream%NUM%_thumb.[秒数].jpg（「,」区切りで列挙）
			等間隔 [ストリーム出力フォルダ]/mystream%NUM%_thumb-%04d.jpg
				等間隔の場合、時間がかかるので完了前に結果予想が返される（%04dは4桁の連番）
 			失敗または同一ストリームを重複して作成しようとした場合は空白
		例
			・ストリーム1の60秒目を144x108でサムネイルを作成する
			　入力：WI_GET_THUMBNAIL.html?temp=1,60,144,108
		     	　返値：/stream/mystream1_thumb.jpg
			・ストリーム1の60秒目を秒数込みのファイルネームで作成する
			　入力：WI_GET_THUMBNAIL.html?temp=1,60:,144,108
		     	　返値：/stream/mystream1_thumb.60.jpg
			・ストリーム1の60秒目と120秒目を144x108でサムネイルを作成する
			　入力：WI_GET_THUMBNAIL.html?temp=1,60:120,144,108
		     	　返値：/stream/mystream1_thumb.60.jpg,/stream/mystream1_thumb.120.jpg
			・ストリーム1の60秒目と120秒目を144x108でサムネイルを作成する（結果を待たない）
			　入力：WI_GET_THUMBNAIL.html?temp=1,thru60:120,144,108
		     	　返値：/stream/mystream1_thumb.60.jpg,/stream/mystream1_thumb.120.jpg
			・ストリーム１の60秒間隔のサムネイルを作成する
			　入力：WI_GET_THUMBNAIL.html?temp=1,per60,144,108
		     	　返値：/stream/mystream1_thumb-%04d.jpg
			　※１　等間隔の場合、「-」で連番数値と区切られています
			　※２　間隔は 1/指定秒数 という計算式で導かれていますので無理数になった場合、
				微妙なズレが出る可能性があります。60秒とか・・小数点10桁切り捨て
			・特定動画をファイル名指定でサムネイルを作成する
			　入力：WI_GET_THUMBNAIL.html?temp=D:\My Videos\テスト #01.ts,60,144,108
			　返値：/stream/file_thumbs/テスト ♯01.jpg 
			　作成結果：　/stream/file_thumbs/テスト ♯01.jpg 
			　	ストリーム時の「mystream%NUM%_thumb」の代わりにファイル名が使用され、
			　	かつfile_thumbsフォルダに作成されます
			　	また、入力時の#が出力時には＃に変換されて作成されます
				複数や等間隔も同じように作成されます
	WI_SHOW_MAKING_PER_THUMB.html
			等間隔サムネイルを作成中の動画フルパスファイル名一覧が返されます
	WI_WRITE_LOG.html?temp=[ログに書き込む文字列]
		ログを出力　返値："OK"



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
	・ffmpegの穏便な終了



■ptTimerにて1ストリームしか使用できないことへの対策
	1. BonDriver_ptmr.dllを適当な名前で4つコピーします
	2. それぞれの.ch2ファイルをBonDriver_ptmr.ch2からコピーして以下の通り編集します
	   ch2ファイル内に4つのチャンネル空間ブロックがありますが、1つに限定するようにします。
	   例えば、1つめは「;#SPACE(1,T0)」、2つめは「;#SPACE(3,T1)」、3つめは「;#SPACE(0,S0)」、4つめは「;#SPACE(2,S1)」、
           というふうにチャンネル空間を１つだけにします
	3. これで4ストリーム全て使用することができます



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



※ConnectedSelect.jsはhttp://d.hatena.ne.jp/Mars/20071109のスクリプトを使用させていただきました。
※ch_sid.txtはNicoJKPlayModのjkch.sh.txtを参照し修正を加えたものです。作者様ありがとうございます。
※CtrlCmdCLI.dllはEDCBに添付されていたものです。作者様及び派生版の作者様ありがとうございます。
