TvRemoteViewer_VB v2.83


��1�@	%NUM%�͔z�M�ԍ���\���܂�
��2	�p�����[�^�[��GET,POST�ǂ���ł��ł��iWatchTV.html�����j

��HLS�z�M�J�n

	StartTv.html (HLS�z�M�J�n�@�܂��́AHTTP�z�M����)
		GET�APOST�ǂ���ł��@�@��HTTP�z�M�̏ꍇ�͘^�揀�������Ŏ��ۂ̔z�M�͂܂�����܂���
		�p�����[�^�[	value�̗�				����
		num		1					�X�g���[���i���o�[�iWatchTV�̏ꍇ�ȊO�K�{�j
		StreamMode	0					0=HLS 1=HLS����Đ� 2=HTTP 3=HTTP����Đ��i�K�{�j
		BonDriver	BonDriver_pt2_t0.dll			BonDriver�t�@�C�����i�e���r�z�M���K�{�j
		ServiceID	54321					�T�[�r�XID�i�e���r�z�M���K�{�j
		ChSpace		0					�`�����l���X�y�[�X�i�e���r�z�M���K�{�j
					ini��TSID_in_ChSpace��1�̏ꍇ�A�l��TSID*100+ChSpace�Ƃ��邱�Ƃ�TSID���܂߂邱�Ƃ��ł��܂�
		resolution	640x360					�𑜓x�i�C�Ӂj
		Bon_Sid_Ch	BonDriver_pt2_t0.dll,54321,0		��L�R�𓯎��ɐݒ�(HLS�̂�)
		redirect	ViewTV2.html				�z�M�J�n��W�����v����y�[�W(HLS�̂�)
		VideoName	D:\test.ts				����t�@�C���̃t���p�X�i�t�@�C���Đ����K�{�j
									UTF-8��URL�G���R�[�h�����ő��M���ȁE�E�H
		VideoSeekSeconds					����t�@�C���擪����̃V�[�N�b���i�C�Ӂj
		NHKMODE		0					�����I���@0(�啛),11(��),12(��),4(����2)�i�C�Ӂj
			0=��E���@HLS�I�v�V�����ύX����
                	1=NHK�Ȃ�Ύ�@NHK�ȊO�͎�E���@-dual_mono_mode main
                	2=NHK�Ȃ�Ε��@NHK�ȊO�͎�E���@-dual_mono_mode sub
			3(ini����)=�I����
                	4=��񉹐��@-map 0:v:0 -map 0:a -map -0:a:0
                	5=����剹�� -af pan=stereo|c0=c0|c1=c0
                	6=���敛���� -af pan=stereo|c0=c1|c1=c1
			9=NHK�Ȃ��VLC�ōĐ�
                	11=�S�Ă̕����ǂŎ�@-dual_mono_mode main
                	12=�S�Ă̕����ǂŕ��@-dual_mono_mode sub
		nohsub		0					1=�n�[�h�T�u���Ȃ�
									2=�\�t�g�T�u�pass�t�@�C�����^�C���V�t�g���č쐬
									3=�\�t�g�T�u�@�^�C���V�t�g������ass���R�s�[�̂�
		VideoSpeed	1.5					���{���ōĐ����邩�i�C�Ӂj
		nicodelay	0					�R�����g�������ꍇ�ɒ����H�ʏ��0�i�C�Ӂj
		hlsAppSelect	ffmpeg					HLS�A�v�������w��(VLC,V,ffmpeg,F,QSVEnc,Q,QSV,NVVEnc,N,NV,VCEEnc,A,VCE)

		hlsOptAdd	[1�`2],[1�`4],[������]			HLS�\�t�g�ɒǉ�����p�����[�^�[�i�C�Ӂj
			��1�p�����[�^�F	1=HLS�I�v�V������-i���O�ɕ������ǉ����܂�
				2=HLS�I�v�V������-i�̌�ɕ������ǉ����܂�
			��2�p�����[�^�F	HLS�I�v�V������ɓ����p�����[�^���������ꍇ�ɂǂ����邩
				1=�ύX���Ȃ�
				2=������HLS�I�v�V������̃p�����[�^��j�����V�����ǉ����܂�
				3=������HLS�I�v�V�����̗v�f�ɒǉ������݂܂��i��F-vf a��-vf a,b)
				4=�P���ɒǉ�
				9=�w��p�����[�^�������폜
			��FhlsOptAdd=2,2,-map 0,0 -map 0,1
			�܂��A�u_-_�v�ŋ�؂邱�Ƃɂ�蕡���̏����������s�����Ƃ��ł��܂�
			��FhlsOptAdd=2,9,-hls_-_2,2,-map 0,0 -map 0,1	�i-hls�������폜�������-map�`��ǉ��j
		profile		�C�ӂ̕�����iprofile.txt�ɋL�����ꂽ�v���t�@�C�����ƘA���j

		�EISO�Đ��I�v�V����
		i_startoffset	�Đ��J�n�b���iVideoSeekSeconds�Ŏw�肵�Ă��ǂ��j
		i_audioLang	�������� ja ��en�@�Ȃ�
		i_audioTrackNum	�����g���b�N�@0�`�@AudioLang�Ƃǂ��炩�̎w�肪����Ηǂ����A�����炪�w�肳��Ă���΂������D��B�������Ă�����Ă��Ȃ���Ή����w�肹���N���B�i�f�t�H���g����ɂȂ�B�j
		i_subLang	��������
		i_subTrackNum	�����g���b�N�BSubLang���w�肳��Ă���΂������D��B�����Ƃ��w�肳��Ă��Ȃ���Ύw��Ȃ��ŋN���i�����Ȃ��j�B

		��F
		http://127.0.0.1:40003/StartTv.html?num=1&BonDriver=BonDriver_PT3_s0.dll&ServiceID=101&ChSpace=0&hlsAppSelect=QSVEnc
		http://127.0.0.1:40003/StartTv.html?num=1&VideoName=D:\test.ts&VideoSeekSeconds=30



��HTTP�z�M�J�n

	�܂��A�T�[�o�[�����g�p���Ă���HTTP�z�M�A�v���AVLC�̔z�M�擪�|�[�g��WI_GET_TVRV_STATUS.html�Ŏ擾���Ă����܂�

	HTTP�z�M���J�n������@��2�ʂ肠��܂�

	A�F	�y�T�[�o�[����HTTP�z�M�A�v���FVLC, ffmpeg�z
		StartTv.html��StreamMode=2or3�w��ŃA�N�Z�X���z�M�p�����[�^�[�̐ݒ���s������A
		����Đ��A�v���P�[�V��������HTTP�X�g���[��URL�ɃA�N�Z�X����

		�Ⴆ��
		http://127.0.0.1:40003/StartTv.html?BonDriver=BonDriver_PT3_s0.dll&ServiceID=101&ChSpace=0&StreamMode=2
		�ɃA�N�Z�X��

		�E�T�[�o�[����HTTP�z�M�A�v����VLC�̏ꍇ
		�@�i42464+�X�g���[���ԍ��̕������|�[�g���󂯂�K�v�����邩������܂���j
		�@http://127.0.0.1:42465/mystream1.ts�@(�X�g���[��2�̏ꍇ�̓|�[�g42466�A�ȉ����l�j

		�E�T�[�o�[����HTTP�z�M�A�v����ffmpeg�̏ꍇ
		�@http://127.0.0.1:40003/WatchTV1.ts�@�i�X�g���[��2�̏ꍇ��WatchTV2.ts�A�ȉ����l�j

		���@ini�̃T�[�o�[��HTTP�z�M�A�v���w��HTTPSTREAM_App�ɌW��炸StartTv.html�Ăяo������HLS�A�v�����w��ł���I�v�V������ǉ�
			httpApp=2	1=VLC 2=ffmpeg
		���@HTTP�X�g���[��URL�͎�v�Z�̑��A�z�M�����������WI_GET_LIVE_STREAM.html�ɃA�N�Z�X����Ǝ擾�ł��܂�

	B�F	�yffmpeg�X�g���[���z
		�E�T�[�o�[����HTTP�z�M�A�v����VLC�ł����Ă�ffmpeg���g�p����܂�
		WatchTV%NUM%.ts�ɒ��ڃA�N�Z�X���Ĕz�M�J�n
		WatchTV%NUM%.ts��GET��StartTv.html�Ɠ��l�̃p�����[�^�[��^����(num�͏ȗ��\�j
		��F
		http://127.0.0.1:40003/WatchTV1.ts?BonDriver=BonDriver_Spinel_s0.dll&ServiceID=101&ChSpace=0
                http://127.0.0.1:40003/WatchTV1.ts?VideoName=D:\test.ts&VideoSeekSeconds=30
		���Ȃ݂Ɂ���URL��VLC�̃X�g���[�����J������Q�Ƃ���Ɣz�M���J�n����܂�

		�yWebM�`���z
		WatchTV�`.webm�ɃA�N�Z�X���邱�Ƃɂ��webm�X�g���[�����u���E�U��ōĐ����邱�Ƃ��\
		resolution=�ɑΉ�����I�v�V������HLS_option_ffmpeg_webm.txt�ɋL�q
		��F[960x540]-i udp://127.0.0.1:%UDPPORT%?pkt_size=262144&fifo_size=1000000&overrun_nonfatal=1 -vcodec libvpx -b 1800k -quality realtime -cpu-used 2 -vf yadif=0:-1:1 -s 640x360 -r 30000/1001 -acodec libvorbis -ab 128k -f webm -
		���̌�AVLC�܂��̓u���E�U��
		http://127.0.0.1:40003/WatchTV1.webm?resolution=640x360&BonDriver=BonDriver_Spinel_s0.dll&ServiceID=101&ChSpace=0
		��
		http://127.0.0.1:40003/WatchTV1.webm?resolution=640x360&VideoName=D:\test.ts
		�ȂǂƃA�N�Z�X����΍Đ�����܂��Bvideo�^�O�ɖ��ߍ��݉�

		GitHub��Ƀu���E�U��ł�WebM�z�M�e�X�g����A�b�v���܂���



���z�M��~

	WI_STOP_STREAM.html
		�p�����[�^�[	value�̗�	����
		num		1		1�`�@�e�X�g���[����~
						-1=�S��~�iUDP�EHLS�\�t�g���O��~�����j
						-2=�S��~�iUDP�EHLS�\�t�g���O��~�Bini�ł̐ݒ�ɏ]���j
						-3=�S��~�i-2�Ɠ��l�B�������G���R�ς݃t�@�C���폜�����j


��HLS_option�`.txt �I�v�V�������ŕϊ������萔
	%UDPPORT%	�\�t�g�Ŏ����I�Ɋ��蓖�Ă�ꂽudp�|�[�g
	%WWWROOT%	WWW��root�t�H���_
	%FILEROOT%	m3u8��ts���쐬�����t�H���_
	%HLSROOT%	HLS�A�v�������݂���t�H���_
	%HLSROOT/../%	HLS�A�v�������݂���t�H���_�̂P��̐e�t�H���_�iffmpeg�𓀎��̃t�H���_�\���ɑΉ��j
	%rc-host%	"127.0.0.1:%UDPPORT%"�ɕϊ�����܂��B
	%NUM%		�X�g���[���i���o�[
	%VIDEOFILE%	�r�f�I�t�@�C���ɕϊ��i���ۂ́u-i %VIDEOFILE%�v�̌��ߑł���-i�̌��̕����񂪃t�@�C�����ɕϊ��j
	%VIDEODURATION%	�r�f�I�̒���(�b)�@�s���ȏꍇ��0


��HTML�o�͎��̃p�����[�^�ϊ�

	����html��WEB�f�U�C����ύX�������ꍇ�Ɏg�p�ł��܂�

	�Eindex.html�AViewTV[n].html�Ŏg�p�ł���ϐ�
	html���Ɉȉ��̕ϐ����L�����Ă����ƌĂяo���ꂽ�Ƃ��ɓK�؂Ȓl�܂���HTML�ɕϊ�����܂�
	�ϊ��O				�ϊ���
	%NUM%				�X�g���[���i���o�[
	%SELECTBONSIDCH%		index.html����BonDriver��ServiceID&ChSpace��I������<SELECT>�Z�b�g���쐬
	%PROCBONLIST%			�z�M���̃X�g���[���i���o�[��BonDriver���e�L�X�g�ŕ\������
	%VIEWBUTTONS%			�X�g���[���̐����������{�^�����쐬
	%SELECTNUM%			�X�g���[���i���o�[�I��
	%SELECTRESOLUTION%		�𑜓x�I��
	%IDPASS%			�u���[�U�[��:�p�X���[�h@�v�ɕϊ��iini��ALLOW_IDPASS2HTML=1�̂Ƃ��j
					�g�p��@http://%IDPASS%" + location.host + "/%FILEROOT%mystream%NUM%.m3u8";
					IE�Ȃǂł̓Z�L�����e�B�ݒ��URL���p�X���[�h�������Ȃ��ƌ���Ȃ��Ȃ�܂�
	%VIDEOSEEKSECONDS%		�t�@�C���Đ����ɃV�[�N����b��
	%SELECTVIDEO%			�r�f�I�t�@�C���ꗗHTML���i
	%VIDEOFROMDATE%			�r�f�I�t�@�C���ꗗ��\�������ۂ̈�ԌÂ��t�@�C���̍X�V�����uyyyy/MM/dd�v
	%VIDEODURATION%	�r�f�I�̒���(�b)�@�s���ȏꍇ��0�i�C�Ӂj


	�EViewTV[n].html�݂̂Ŏg�p�ł���ϐ�
	%SELECTCH%			ViewTV.html���Ŕԑg��I������<SELECT>���쐬����
	%WIDTH%				�r�f�I�̕�
	%HEIGHT%			�r�f�I�̍���
	%FILEROOT%			.m3u8�����݂��鑊�΃t�H���_
	%SUBSTR%			Nico2HLS�ɂ���ăj�R�j�R�����R�����g�擾���Ȃ��"_s"�ɕϊ������
	%JKNUM%				�j�R�j�R�����̃`�����l��������i��Fjk8)
	%JKVALUE%			�j�R�j�R�����p�ڑ��p������


	�E�Ȃ��A%PROCBONLIST%�A%SELECTCH%�A%VIEWBUTTONS%�A%SELECTBONSIDCH%�A%SELECTNHKMODE%�A%SELECTRESOLUTION%�@�ɑ΂��ẮA�v�f�̑O����ɕ\������html�^�O���w��ł��܂��B
	����	%VIEWBUTTONS:[�O���ɕ\������html�^�O]:[�{�^���ƃ{�^���̊Ԃɕ\������html�^�O]:[����ɕ\������html�^�O]:[�v�f���\������Ȃ��ꍇ�ɑւ��ɕ\�������html�^�O]%
	��	%VIEWBUTTONS:�����\�X�g���[��<br>:<br>===================<br>:�{�^���������Ă�������<br>%
	����	�����\�X�g���[��
		[�X�g���[��1������]
		===================
		[�X�g���[��2������]
		�{�^���������Ă�������


	�EWaiting.html
	%NUM%				�X�g���[���i���o�[
	%WAITING%			���b�Z�[�W�i�z�M������ or �z�M����Ă��܂���j


	�EERROR.html
	%NUM%				�X�g���[���i���o�[
	%ERRORTITLE%			�G���[�y�[�W�^�C�g��
	%ERRORMESSAGE%			�G���[���b�Z�[�W
	%ERRORRELOAD%			�v���O��������w�肳�ꂽ�ꍇ�ɍēǍ��{�^����\������



�����擾
	
	WI_GET_TVRV_STATUS.html			�T�[�o�[�̊e��ݒ���擾


	WI_GET_CHANNELS.html			BonDriver�ƕ����ǈꗗ


	WI_GET_LIVE_STREAM.html			�z�M�����X�g�擾
		(����List�ԍ�), �X�g���[���ԍ�, UDP�A�v�����d�l����|�[�g, BonDriver��, �T�[�r�XID, ChSpace, StreamMode, �����I��, �ċN�����Ȃ�0�ȏ�, �����ǖ�, HLS�A�v��exe��, �V�[�N�b��, �z�M��URL


    	WI_GET_PROGRAM_NUM.html			�z�M���̔ԑg���擾
		�X�g���[���ԍ�,�����ǖ�,�l�b�g�����ǖ�,�T�[�r�XID,ChSpace,�J�n��:��,�I����:��,�ԑg�^�C�g��,�ԑg���e�܂��͍Đ��t�@�C���t���p�X,�V�[�N�b��


	WI_GET_ERROR_STREAM			�ċN�����Ă���X�g���[���ԍ����擾
		�X�g���[���ԍ��i�����̏ꍇ�͔��p�X�y�[�X��؂�j


	WI_GET_RESOLUTION			�𑜓x�ꗗ�擾


	WI_GET_TSFILE_COUNT.html?num=%NUM%	�ł���������ts�t�@�C�����im3u8�����݂��邩�ǂ����͊֌W�����j
	WI_GET_TSFILE_COUNT2.html?num=%NUM%	�ł���������ts�t�@�C�����im3u8�����݂���ΐ��̒l�A���݂��Ȃ���Ε��̒l�j
	��HTTP�X�g���[���ł͏��0���Ԃ��Ă��܂�


	WI_GET_VIDEOFILES.html	�r�f�I�t�@�C���ꗗHTML���i��Ԃ�
	WI_GET_VIDEOFILES2.html	�r�f�I�t�@�C���ꗗ���e�L�X�g�ŕԂ�
		��L�Q�̃C���^�[�t�F�[�X�p�p�����[�^�[�F
		vl_refresh	1=�����r�f�I�t�@�C���X�V
		vl_startdate	�w������O�̃r�f�I�t�@�C���𒊏o����
		vl_volume	�����\�����邩�i�ŏI���t�̃t�@�C����ǉ�����̂ŕs���m�j
		videoexword	�P���Ɏw�蕶���񂪊܂܂�Ă���t�@�C�����X�g��Ԃ�(���p�X�y�[�X��؂��AND����)�y�L�q�R��z
		��L�p�����[�^�[��%SELECTVIDEO%��ϊ�����SelectVideo.html�ɂ��L��
		���ȉ���WI_GET_VIDEOFILES2.html�̂ݑΉ��y2.91i�ȍ~�z
		temp	,��؂�ňȉ��̃p�����[�^�[���w��\
				dironly		�t�H���_�\���̂ݕԂ�
				current		�J�����g�t�H���_�̂݃��X�g�A�b�v
		vl_dir	dironly�Ŏ擾�����t�H���_���w��i�O����v�j
		��FWI_GET_VIDEOFILES2.html?temp=dironly,current
		    WI_GET_VIDEOFILES2.html?temp=current&vl_dir=D:\videos


	WI_FILE_OPE.html	�t�@�C���ǂݏ���(UTF-8)
		�p�����[�^�[�F
		fl_cmd		dir, read, write, write_add, delete
		fl_file		�t�H���_�����̓t�@�C�����i%WWWROOT%����̑��Έʒu�j
		fl_text		�������ޓ��e
		temp		dir�̏ꍇ�̃t�B���^(���w��̏ꍇ�́u*�v)�@��F�u*.jpg�v��umystream*�v
		���ʁF
		0,SUCCESS(+���s[����])�@���́@2,[�G���[���e]
		
		2.14�`	�t�@�C������͊g���q .json .m3u .txt�̂ݗL��
			���̑��̃t�@�C���𑀍삵�����ꍇ�̓v���O�����t�H���_��file_ope_allow.txt�Ƃ���
			�t�@�C����ݒu���A���삵�����t�@�C�������t�H���_���ƋL�����邱��
			*.�g���q�ł�OK
			��F	file\abc.log
				*.ini


	WI_STREAMFILE_EXIST.html?fl_file=[�t�@�C����]
		�X�g���[���t�H���_���Ƀt�@�C�������݂��邩�ǂ���
		��FWI_STREAMFILE_EXIST.html?fl_file=mystream1_thumb.jpg
�@�@�@�@�@�@�@�@�@�@WI_STREAMFILE_EXIST.html?fl_file=file_thumbs/����t�@�C����.jpg
		�Ԓl�F�@���݂����1�A���݂��Ȃ���΋�


	WI_GET_PROGRAM_[D,TVROCK,EDCB,PTTIMER,TVMAID].html(?temp=1�`3)
		TVROCK,EDCB���猻�ݎ����̔ԑg�\���擾
		�I�v�V���� temp=1�`3 ���w�肷�邱�Ƃɂ�莟�ԑg�����݂���Ε����Ď擾
		1:�Ԓl�̊e�ԑg���L�q�͏]���ʂ�
		2:�Ԓl�̊e�ԑg�����̎��ԑg���`���Ɂu[Next]�v��t��
		3:�Ԓl�̊e�ԑg��񖖔��Ɍ��ԑg�u,0�v�����ԑg�u,1�v����t��
		4�ȏ�:�ԑg�I���܂�temp���ȓ������c���Ă��Ȃ��ꍇ�͌��ԑg�̏ڍח��Ɏ��ԑg����\���i�f�[�^�͖��w��Ɠ����j
		���ʁF	�����ǖ�,�T�[�r�XID,ChSpace,�J�n��:��,�I����:��,�ԑg�^�C�g��,�ԑg���e(���ԑg)
		�y2.53�ztemp��,��؂��1��^����Ɣԑg���e�ɑ����ăW���������l��t������悤�ɂ���
				��Ftemp=3,1
		�y2.55�z[Next]�\�L�̏ꍇ�A���ԑg�̃W�������́u:�v��؂�Ŋ����W���������l�ɑ����ĕt�������


	WI_GET_CHAPTER.html?temp=�^��t�@�C���t���p�X
		�^��t�@�C����.chapter�t�@�C���̓��e���擾�ichapters�t�H���_�̒��ł��j


	WI_WRITE_CHAPTER.html?temp=num,�������ރ`���v�^�[������


	WI_GET_HTML.html?temp=[HTML�擾���@],[�G���R�[�h],[UserAgent],http://www.google.co.jp/
		HTML�擾���@	1: webbrowser UserAgent�����B�G���[�ɂ�茻��g�p�s��
				2: webclient
				3: HttpWebRequest
		��FWI_GET_HTML.html?temp=2,UTF-8,,http://www.google.co.jp/
		���ӁFnicovideo.jp��2ch.net��read.cgi,subback.html,bbsmenu.html�ȊO�͒e���悤�ɂȂ��Ă��܂�
			  client*.ini�̐ݒu�ɂ��N���C�A���g���ʂɉ����w��ł���悤�ɂȂ��Ă��܂�
			  GitHub��_readme�t�H���_�ɂ���client_sample.ini.zip���Q�l�ɂ��Ă�������


	WI_GET_THUMBNAIL.html?temp=[�쐬�\�[�X],[�b���w��],[��],[�c]
		�t�@�C���Đ�������̃T���l�C�����쐬
		�p�����[�^
			[�쐬�\�[�X]	�z�M���̃X�g���[���i���o�[�A�������͓���t���p�X�t�@�C�����i���[�J���p�X�j
					�t�@�C�����w��̏ꍇ�̓X�g���[���t�H���_����file_thumbs�Ƃ����t�H���_���ɁA
					�t�@�C�������g�p����jpg���쐬����܂�
					���d�v���t�@�C������#(���p)���܂܂�Ă����ꍇ��(�S�p)�ɕϊ�����܂�
						�iURL�A�N�Z�X���ł��Ȃ����߁j
			[�b���w��]	�P�ƁA�u:�v��؂�ŕ����Athru[�b���w��]�Aper[���Ԋu�b��]
					���Ԋu���w�肵���ꍇ�́A���ʂ�҂����ɕԒl���Ԃ���܂�
					�܂��Athru[�b���w��]�Ō��ʂ�҂����ɕԒl���Ԃ���܂�
			[��],[�c]	�c����0���w�肵���ꍇ��ffmpeg�W���̑傫����jpg���쐬����܂�
		�Ԓl
			�P�Ɓ@ [�X�g���[���o�̓t�H���_]/mystream%NUM%_thumb.jpg
			�����@ [�X�g���[���o�̓t�H���_]/mystream%NUM%_thumb.[�b��].jpg�i�u,�v��؂�ŗ񋓁j
			���Ԋu [�X�g���[���o�̓t�H���_]/mystream%NUM%_thumb-%04d.jpg
				���Ԋu�̏ꍇ�A���Ԃ�������̂Ŋ����O�Ɍ��ʗ\�z���Ԃ����i%04d��4���̘A�ԁj
 			���s�܂��͓���X�g���[�����d�����č쐬���悤�Ƃ����ꍇ�͋�
		��
			�E�X�g���[��1��60�b�ڂ�144x108�ŃT���l�C�����쐬����
			�@���́FWI_GET_THUMBNAIL.html?temp=1,60,144,108
		     	�@�Ԓl�F/stream/mystream1_thumb.jpg
			�E�X�g���[��1��60�b�ڂ�b�����݂̃t�@�C���l�[���ō쐬����
			�@���́FWI_GET_THUMBNAIL.html?temp=1,60:,144,108
		     	�@�Ԓl�F/stream/mystream1_thumb.60.jpg
			�E�X�g���[��1��60�b�ڂ�120�b�ڂ�144x108�ŃT���l�C�����쐬����
			�@���́FWI_GET_THUMBNAIL.html?temp=1,60:120,144,108
		     	�@�Ԓl�F/stream/mystream1_thumb.60.jpg,/stream/mystream1_thumb.120.jpg
			�E�X�g���[��1��60�b�ڂ�120�b�ڂ�144x108�ŃT���l�C�����쐬����i���ʂ�҂��Ȃ��j
			�@���́FWI_GET_THUMBNAIL.html?temp=1,thru60:120,144,108
		     	�@�Ԓl�F/stream/mystream1_thumb.60.jpg,/stream/mystream1_thumb.120.jpg
			�E�X�g���[���P��60�b�Ԋu�̃T���l�C�����쐬����
			�@���́FWI_GET_THUMBNAIL.html?temp=1,per60,144,108
		     	�@�Ԓl�F/stream/mystream1_thumb-%04d.jpg
			�@���P�@���Ԋu�̏ꍇ�A�u-�v�ŘA�Ԑ��l�Ƌ�؂��Ă��܂�
			�@���Q�@�Ԋu�� 1/�w��b�� �Ƃ����v�Z���œ�����Ă��܂��̂Ŗ������ɂȂ����ꍇ�A
				�����ȃY�����o��\��������܂��B60�b�Ƃ��E�E�����_10���؂�̂�
			�E���蓮����t�@�C�����w��ŃT���l�C�����쐬����
			�@���́FWI_GET_THUMBNAIL.html?temp=D:\My Videos\�e�X�g #01.ts,60,144,108
			�@�Ԓl�F/stream/file_thumbs/�e�X�g ��01.jpg 
			�@�쐬���ʁF�@/stream/file_thumbs/�e�X�g ��01.jpg 
			�@	�X�g���[�����́umystream%NUM%_thumb�v�̑���Ƀt�@�C�������g�p����A
			�@	����file_thumbs�t�H���_�ɍ쐬����܂�
			�@	�܂��A���͎���#���o�͎��ɂ́��ɕϊ�����č쐬����܂�
				�����ⓙ�Ԋu�������悤�ɍ쐬����܂�


	WI_SHOW_MAKING_PER_THUMB.html
			���Ԋu�T���l�C�����쐬���̓���t���p�X�t�@�C�����ꗗ���Ԃ���܂�


	WI_WRITE_LOG.html?temp=[���O�ɏ������ޕ�����]
		���O���o�́@�Ԓl�F"OK"


	WI_GET_PARA.html?temp=[�ϐ���]
		video_force_ffmpeg
		HTTPSTREAM_App
		html_publish_method


	WI_SET_PARA.html?temp=[�ϐ���]=[�l]
		video_force_ffmpeg=�l
		HTTPSTREAM_App=�l
		html_publish_method=�l

	WI_GET_PROFILES.html
		profile.txt���ɋL�q���ꂽ�v���t�@�C�����ꗗ���Ԃ���܂�

	WI_GET_VERSION.html?temp=[1,2,3,9]
		�o�[�W��������Ԃ��i1���Ԃ�1��X�V����܂��j
		temp	1 : �N�����̃o�[�W����
			2 : �񐄏��o�[�W����
			3 : �����o�[�W����
			9 : �N�����o�[�W����,�񐄏��o�[�W����,�����o�[�W���� 

	WI_GET_JKNUM.html?(num=[�X�g���[���ԍ�] || temp=[�T�[�r�XID])
		num�w�肳�ꂽ�ꍇ�͔z�M���̃j�R�j�R�����`�����l��
		temp�ŃT�[�r�XID���w�肳�ꂽ�ꍇ�̓T�[�r�XID�ɑΉ�����j�R�j�R�����`�����l��
		�Ԓl�F	��Fjk8�@�Y�������`�����l���������ꍇ�́uNoMatch�v
			num�w��Ŕz�M����Ă��Ȃ��ꍇ�́uNoStream�v

	WI_GET_JKVALUE(.html || .json)?(num=[�X�g���[���ԍ�] || temp=[�T�[�r�XID])
		�Ԓl�F	num�w�肳�ꂽ�ꍇ�͔z�M���̃j�R�j�R�����ڑ��p������
			temp�ŃT�[�r�XID���w�肳�ꂽ�ꍇ�̓T�[�r�XID�ɑΉ�����j�R�j�R�����ڑ��p������

	WI_GET_JKCOMMENT.json?temp=[�擾����]
		���߂̃j�R�j�R�����R�����g�f�[�^���擾����JSON�`���ŕԂ��܂�
		�擾�����F
			sn	�X�g���[���ԍ�
			jk	jk�ԍ� �ujk8�v��
			si	�T�[�r�XID
			����L3��ނ̓�����P���w��
			nm	�擾����擪�R�����gNo.�@�������̓}�C�i�X�l�Œ��߂���̎擾�� 0=�����擾
			ms	���ݎ����ő剽�b�k�����R�����g���擾���邩�i30�Ȃ璼��30�b�̃R�����g 0=�������j
		��F	WI_GET_JKCOMMENT.html?temp=jk8
			WI_GET_JKCOMMENT.html?temp=jk8,nm-40
			WI_GET_JKCOMMENT.html?temp=jk8,nm1234
			WI_GET_JKCOMMENT.html?temp=jk8,nm0,ms30
			nm0���w�肵���Ƃ��́A�����Ōp�������f�[�^�������܂�
		�Ԓl�F�@json�`�� unixtime���ɃR�����g���܂Ƃ߂�ꂽ����
			unixtime 32400�b�ڂ̍��ڂƂ��� [�X���b�h�ԍ�,����ꂽ�ŏ��̃R�����gNo.,����ꂽ�ŏ��̃R�����gunixtime,����ꂽ�Ō�̃R�����gNo.,����ꂽ�Ō�̃R�����gunixtime,���߂Ɏ擾�����Ō�̃R�����gNo.,���߂Ɏ擾�����Ō�̃R�����gunixtime]�@���Ԃ���܂�

	WI_CLEAR_ABEMA_CACHE.html
		AbemaTV�ԑg���L���b�V�����폜

	WI_GET_STATION_PROGRAM.html?temp=[�^��\�t�g��],[�T�[�r�XID](,[�����X�^�[�gunixtime],[�����I��unixtime],[TvRock�\��󋵋����X�V])
		�e�^��\�t�g����T�[�r�XID�ɑΉ����������ǔԑg�ꗗ���擾����
		�^��\�t�g��	TvRock,EDCB,Tvmaid�i�����w��̏ꍇ�́u_�v�ŘA���B��������D��I�Ɍ�������j
		���Ԉȍ~���ȗ������ꍇ��6���ԕ������������
		TvRock�\��󋵋����X�V	1	���ꕪ�ł��L���b�V�����g�p�����ɗ\��󋵂��擾����

	WI_GET_HLS_APP_COUNT.html
	WI_GET_HLS_APP_COUNT.html?temp=HLS�A�v�����ivlc, v, ffmpeg, f, qsvenc, qsvencc, q, qsv, nvenc, nvencc, n, nv, vceenc, vceencc, a, vce�̂����ꂩ�j,��؂�ŕ�����
		�T�[�o�[PC�ŉғ�����HLS�A�v�����ꂼ��̃v���Z�X��
		���A�v���ŃG���R�[�h�������l�����A�P���ɃT�[�o�[��Ŏ��s����Ă���HLS�A�v���̃v���Z�X���𒲂ׂ܂�
		exepath���w�肳��Ă��Ȃ�HLS�A�v����-1���Ԃ���܂�
		exepath�̎��s�t�@�C�������W���ƈ���Ă���ꍇ�����̕Ԓl��̂悤�ɗ��̂Ō��ʂ��Ԃ���܂�
		��FWI_GET_HLS_APP_COUNT.html	�i�S�Ă�HLS�A�v���j
			WI_GET_HLS_APP_COUNT.html?temp=qsv,nv,vce	�iqsv,nv,vce�̃v���Z�X���̂ݕԂ��j
		�Ԓl�F
			ffmpeg,0
			qsv,0
			nv,0
			vce,-1
			vlc,0



������_AbemaTV�J�X�^���f�[�^�`��
	�`���F�e�L�X�g
	�G���R�[�h�FUTF-8
	���s�FCRLF �܂��� LF
	1�s�̓��e�FChannelId,�`�����l����,�J�nunixtime,�I��unixtime,�ԑg��,�ԑg���e
	�\�[�g�FChannelId,�J�nunixtime

	�ԑg���Ɣԑg���e�Ɋ܂܂��u,�v�Ɖ��s�R�[�h(\n�̂͂��Ȃ̂ł܂����肦�Ȃ�)�͑S�p�����邩���̕����ɒu�����Ă����K�v���L��܂�
    ��Fhttps://abemagraph.info/timetable/tvr.txt AbemaGraph����Ɋ��ӁB�J�b�p�B������͂������̂͂Ȃ񂩉��s�����������̂ŁE�E
abema-news,AbemaNews�`�����l��,1498575600,1498582800,�ԑg��1,�ԑg���e1
abema-news,AbemaNews�`�����l��,1498582800,1508285600,�ԑg��2,�ԑg���e2
�E�E�E
abema-special,AbemaSPECIAL,1499263200,1499270400,�ԑg��1,�ԑg���e1
abema-special,AbemaSPECIAL,1499270400,1499277600,�ԑg��1,�ԑg���e1
�E�E�E
	���̗l�ȍs��2����������ƕ��сA�܂��A�`�����l�����������ꂪ�J��Ԃ���܂�
	���я��́A�����ǖ����J�nunixtime�Ń\�[�g���Ă���K�v������܂�

	�ق�Ƃ͐�json�ɂ��Ή��ł���̂ł����AVS2010�ł�NuGet��JSON.NET������Ȃ��AVS2015���Ɉڍs���悤�ɂ����̏ꍇ��GitHub�ڍs���悭�킩��Ȃ��Ƃ�����҂̕׋��s���ɂ����̂ł�
	�܂����̔ԑg�f�[�^���擾�ł�����Ȃ�y�ɐ��`�ł���ł��傤

	�����������f�[�^���Ȃ�TvRemoteViwer_VB�{�̂Ŏ擾���Ȃ��̂��Ƃ�����
	�E�悭���Ă܂��񂪁A�ŋ�AbemaTV����֘A�̃c�[���łȂ񂩑������������Ƃ��Ȃ��Ƃ�
	�E���݃I�[�v���\�[�X�Ŏ擾���@�����J���Ă���\�t�g�������i���������Č��J����Ɩ��H
	�E��҂̎��͂ł͉�͂����ڂ��Ȃ��B���������E�E
	�Ƃ����킯�ł��B���������������܂�

