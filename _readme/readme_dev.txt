TvRemoteViewer_VB v1.89


※1　	%NUM%は配信番号を表します
※2	パラメーターはGET,POSTどちらでも可です（WatchTV.html除く）

■HLS配信開始

	StartTv.html (HLS配信開始　または、HTTP配信準備)
		GET、POSTどちらでも可　　※HTTP配信の場合は録画準備だけで実際の配信はまだされません
		パラメーター	valueの例				説明
		num		1					ストリームナンバー（WatchTVの場合以外必須）
		StreamMode	0					0=HLS 1=HLS動画再生 2=HTTP 3=HTTP動画再生（必須）
		BonDriver	BonDriver_pt2_t0.dll			BonDriverファイル名（テレビ配信時必須）
		ServiceID	54321					サービスID（テレビ配信時必須）
		ChSpace		0					チャンネルスペース（テレビ配信時必須）
		resolution	640x360					解像度（任意）
		Bon_Sid_Ch	BonDriver_pt2_t0.dll,54321,0		上記３つを同時に設定(HLSのみ)
		redirect	ViewTV2.html				配信開始後ジャンプするページ(HLSのみ)
		VideoName	D:\test.ts				動画ファイルのフルパス（ファイル再生時必須）
									UTF-8でURLエンコード無しで送信かな・・？
		VideoSeekSeconds					動画ファイル先頭からのシーク秒数（任意）
		NHKMODE		0					音声選択　0(主副),11(主),12(副),4(音声2)（任意）
			0=主・副　HLSオプション変更無し
                	1=NHKならば主　NHK以外は主・副　-dual_mono_mode main
                	2=NHKならば副　NHK以外は主・副　-dual_mono_mode sub
			3(ini限定)=選択式
                	4=第二音声　-map 0:v:0 -map 0:a -map -0:a:0
                	5=動画主音声 -af pan=stereo|c0=c0|c1=c0
                	6=動画副音声 -af pan=stereo|c0=c1|c1=c1
			9=NHKならばVLCで再生
                	11=全ての放送局で主　-dual_mono_mode main
                	12=全ての放送局で副　-dual_mono_mode sub
		nohsub		0					1=ハードサブしない
									2=ソフトサブ用assファイルをタイムシフトして作成
									3=ソフトサブ　タイムシフトせずにassをコピーのみ
		VideoSpeed	1.5					何倍速で再生するか（任意）
		nicodelay	0					コメントがずれる場合に調整？通常は0（任意）
		hlsAppSelect	ffmpeg					HLSアプリ名を指定(VLC,V,ffmpeg,F,QSVEnc,Q,QSV)

		hlsOptAdd	[1～2],[1～4],[文字列]			HLSソフトに追加するパラメーター（任意）
			第1パラメータ：	1=HLSオプションの-iより前に文字列を追加します
				2=HLSオプションの-iの後に文字列を追加します
			第2パラメータ：	HLSオプション上に同じパラメータがあった場合にどうするか
				1=変更しない
				2=既存のHLSオプション上のパラメータを破棄し新しく追加します
				3=既存のHLSオプションの要素に追加を試みます（例：-vf a→-vf a,b)
				4=単純に追加
				9=指定パラメータ部分を削除
			例：hlsOptAdd=2,2,-map 0,0 -map 0,1
			また、「_-_」で区切ることにより複数の書き換えを行うことができます
			例：hlsOptAdd=2,9,-hls_-_2,2,-map 0,0 -map 0,1	（-hls部分を削除した後に-map～を追加）
		profile		任意の文字列（profile.txtに記入されたプロファイル名と連動）


		例：
		http://127.0.0.1:40003/StartTv.html?BonDriver=BonDriver_PT3_s0.dll&ServiceID=101&ChSpace=0&hlsAppSelect=QSVEnc
		http://127.0.0.1:40003/StartTv.html?VideoName=D:\test.ts&VideoSeekSeconds=30



■HTTP配信開始

	まず、サーバー側が使用しているHTTP配信アプリ、VLCの配信先頭ポートをWI_GET_TVRV_STATUS.htmlで取得しておきます

	HTTP配信を開始する方法は2通りあります

	A：	【サーバー側のHTTP配信アプリ：VLC, ffmpeg】
		StartTv.htmlにStreamMode=2or3指定でアクセスし配信パラメーターの設定を行った後、
		動画再生アプリケーションからHTTPストリームURLにアクセスする

		例えば
		http://127.0.0.1:40003/StartTv.html?BonDriver=BonDriver_PT3_s0.dll&ServiceID=101&ChSpace=0&StreamMode=2
		にアクセス後

		・サーバー側のHTTP配信アプリがVLCの場合
		　（42464+ストリーム番号の分だけポートを空ける必要があるかもしれません）
		　http://127.0.0.1:42465/mystream1.ts　(ストリーム2の場合はポート42466、以下同様）

		・サーバー側のHTTP配信アプリがffmpegの場合
		　http://127.0.0.1:40003/WatchTV1.ts　（ストリーム2の場合はWatchTV2.ts、以下同様）

		※　HTTPストリームURLは手計算の他、配信準備完了後にWI_GET_LIVE_STREAM.htmlにアクセスすると取得できます

	B：	【サーバー側のHTTP配信アプリ：ffmpeg】
		・サーバー側のHTTP配信アプリがffmpegであることが必要です
		WatchTV%NUM%.tsに直接アクセスして配信開始
		WatchTV%NUM%.tsにGETでStartTv.htmlと同様のパラメーターを与える(numは省略可能）
		VideoNameはURLエンコードしておく必要有り
		例：
		http://127.0.0.1:40003/WatchTV1.ts?BonDriver=BonDriver_Spinel_s0.dll&ServiceID=101&ChSpace=0
                http://127.0.0.1:40003/WatchTV1.ts?VideoName=D%3a%5ctest.ts&VideoSeekSeconds=30
		ちなみに↑のURLをVLCのストリームを開くから参照すると配信が開始されます

		WatchTV1.webmにアクセスすることによりwebmストリームをブラウザ上で再生することも可能
		（実験してみたらできた程度）
		resolution=に対応するオプションをHLS_option_ffmpeg_http.txtに記述
		例：[960x540webm]-i udp://127.0.0.1:%UDPPORT%?pkt_size=262144&fifo_size=1000000&overrun_nonfatal=1 -vcodec libvpx -b 1800k -quality realtime -cpu-used 2 -vf yadif=0:-1:1  -s 960x540 -r 30000/1001 -acodec libvorbis -ab 128k -f webm -
		その後、VLCまたはブラウザで
		http://127.0.0.1:40003/WatchTV1.webm?resolution=960x540webm&VideoName=D:\test.ts
		などとアクセスすれば再生されます。videoタグに埋め込み可


■配信停止

	WI_STOP_STREAM.html
		パラメーター	valueの例	説明
		num		1		1～　各ストリーム停止
						-1=全停止（UDP・HLSソフト名前停止無し）
						-2=全停止（UDP・HLSソフト名前停止。iniでの設定に従う）
						-3=全停止（-2と同様。ただしエンコ済みファイル削除せず）


■HLS_option～.txt オプション内で変換される定数
	%UDPPORT%	ソフトで自動的に割り当てられたudpポート
	%WWWROOT%	WWWのrootフォルダ
	%FILEROOT%	m3u8やtsが作成されるフォルダ
	%HLSROOT%	HLSアプリが存在するフォルダ
	%HLSROOT/../%	HLSアプリが存在するフォルダの１つ上の親フォルダ（ffmpeg解凍時のフォルダ構造に対応）
	%rc-host%	"127.0.0.1:%UDPPORT%"に変換されます。
	%NUM%		ストリームナンバー
	%VIDEOFILE%	ビデオファイルに変換（実際は「-i %VIDEOFILE%」の決め打ちで-iの後ろの文字列がファイル名に変換）
	%VIDEODURATION%	ビデオの長さ(秒)　不明な場合は0


■HTML出力時のパラメータ変換

	自作htmlでWEBデザインを変更したい場合に使用できます

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



■情報取得
	
	WI_GET_TVRV_STATUS.html			サーバーの各種設定を取得


	WI_GET_CHANNELS.html			BonDriverと放送局一覧


	WI_GET_LIVE_STREAM.html			配信中リスト取得
		(内部List番号), ストリーム番号, UDPアプリが仕様するポート, BonDriver名, サービスID, ChSpace, StreamMode, 音声選択, 再起動中なら0以上, 放送局名, HLSアプリexe名, シーク秒数, 配信中URL


    	WI_GET_PROGRAM_NUM.html			配信中の番組情報取得
		ストリーム番号,放送局名,ネット放送局名,サービスID,ChSpace,開始時:分,終了時:分,番組タイトル,番組内容または再生ファイルフルパス,シーク秒数


	WI_GET_ERROR_STREAM			再起動しているストリーム番号を取得
		ストリーム番号（複数の場合は半角スペース区切り）


	WI_GET_RESOLUTION			解像度一覧取得


	WI_GET_TSFILE_COUNT.html?num=%NUM%	できあがったtsファイル数（m3u8が存在するかどうかは関係無し）
	WI_GET_TSFILE_COUNT2.html?num=%NUM%	できあがったtsファイル数（m3u8が存在すれば正の値、存在しなければ負の値）
	※HTTPストリームでは常に0が返ってきます


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


	WI_GET_PROGRAM_[TVROCK,EDCB,PTTIMER,TVMAID].html(?temp=1～3)
		TVROCK,EDCBから現在時刻の番組表を取得
		オプション temp=1～3 を指定することにより次番組が存在すれば併せて取得
		1:返値の各番組情報記述は従来通り
		2:返値の各番組情報内の次番組名冒頭に「[Next]」を付加
		3:返値の各番組情報末尾に現番組「,0」か次番組「,1」かを付加
		4以上:番組終了までtemp分以内しか残っていない場合は現番組の詳細欄に次番組情報を表示（データは無指定と同じ）
		結果：	放送局名,サービスID,ChSpace,開始時:分,終了時:分,番組タイトル,番組内容(次番組)


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


	WI_GET_PARA.html?temp=[変数名]
		video_force_ffmpeg
		HTTPSTREAM_App
		html_publish_method


	WI_SET_PARA.html?temp=[変数名]=[値]
		video_force_ffmpeg=値
		HTTPSTREAM_App=値
		html_publish_method=値

	WI_GET_PROFILES.html
		profile.txt内に記述されたプロファイル名一覧が返されます
