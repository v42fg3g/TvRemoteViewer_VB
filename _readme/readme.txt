TvRemoteViewer_VB v1.73


�`���[�i�[���������s�N�����ăp�p�b�ƃ`�����l����ύX���悤�Ǝv������4��CPU100%�E�E



��������ĂȂɁH

	���ꂽ�Ƃ�����PC��̃e���r�⓮������邽�߂̃\�t�g�ł�



����

	Windows�����삷��PC
	FrameWork4.0
	
	RecTask
	ffmpeg��������VLC



���N��

	�����Ӂ�
	Windows8�ȍ~�ł͈ȉ��̂ǂ��炩�̑��삪�K�v�ł��B
	�ETvRemoteViewer_vb.exe���E�N���b�N�A
	�@�u�v���p�e�B�v���u�݊����v���u�Ǘ��҂Ƃ��Ă��̃v���O���������s����v�Ƀ`�F�b�N
	�E�R�}���h�v�����v�g����
	�@netsh http add urlacl url=http://+:40003/ user=XXXXX
	�@(XXXXX�͎��s���郆�[�U�[�A�������� Everyone �Ɠ��͂���)


	�ݒu��A�N������ƃ^�X�N�g���C����X�^�[�g���܂��B
	�_�u���N���b�N�Œʏ�̑傫���ɂȂ�܂��B
	�e�p�����[�^�[��readme.jpg�Q�Ƃ̂���
	�v���O���~���O����Ƃ���Form1��WindowState��Normal�ɂ��Ă����ƒʏ�̑傫���ŋN�����܂�����֗��ł��傤�B



������������

	���ۂ̎����⑀���WEB��ōs���܂��B
	��F	PC�iIP�A�h���X 192.168.1.100�@�|�[�g40003)�A��TvRemoteViewer_VB�����s���Ă���ꍇ��
		�����[�g�@�i�X�}�z�Ȃǁj�̃u���E�U��http://192.168.1.100:40003�ɃA�N�Z�X���Ă�������
		���[�J��PC�Ńe�X�g���Ă���ꍇ��http://127.0.0.1:40003�ł�



���ݒu readme.jpg�≺�ɏ������e�X�g�����Q�l�ɂ��Ă�������

	�ERecTask��BonDriver��K�؂ɔz�u


	�Effmpeg����VLC���C���X�g�[��
	�i��ffmpeg���g���ꍇ�͓�����libx264-ipod640.ffpreset��ffmpeg���C���X�g�[�������Ƃ���presets�t�H���_�ɃR�s�[�j


	�E������form_status�`.txt��HLS_option�`.txt��TvRemoteViewer_VB.exe�Ɠ����t�H���_�ɃR�s�[


	�EWWWROOT�iWEB���[�g�t�H���_�j�ƂȂ�t�H���_�ɓ�����HTML�t�H���_���̃t�@�C�����R�s�[
	�i��VLC�g�p�̏ꍇ�͔��p�X�y�[�X������Ȃ��ꏊ�̂ق������S�ł��j


	�E�t�@�C���Đ������邽�߂ɂ́A���炩���ߓ�����VideoPath.txt��ҏW���ē��悪����t�H���_���w�肵�Ă����K�v������܂��B

	�E�n�f�W�ԑg�\�̐ݒ�	VideoPath.txt��ҏW���Ă�������

	�ETvRock�ԑg�\�ɂ��܂���
		1.�u���E�U��
		�@http://[TvRock���ғ����Ă���PC�̃��[�J��IP]:[TvRock�̃|�[�g]/[���[�U�[��]/iphone?md=2&d=0
		�@�i��@http://127.0.0.1:8969/���[�U�[��/iphone?md=2&d=0�j
		�@�ɃA�N�Z�X����B
		�@����ƁAiPhone�p�ԑg�\���\������܂��B
		2. �u�\��\���v�����u�����v�ɂ���B�����ȊO�ł��\���܂��񂪔������x���Ȃ�\��������܂��B
		�@�iFirefox�ł͂��܂��؂�ւ��Ȃ���������܂���j
		�@�����ŕ\�����ꂽ�ԑg�\���f�[�^�Ƃ��Ďg�p����܂��B
		3. �u���E�U�����
		
		���n�f�W�܂���BS/CS�̂ǂ��炩������\���������ꍇ�́A�`���[�i�[��I�����Ă�������

	�EEDCB�ԑg�\�ɂ��܂���
		EpgTimer.exe�����݂���t�H���_�ɂ���EpgTimerSrv.ini���J����[SET]�����
			EnableHttpSrv=1
			HttpPort=5510
		������������EpgTimer���ċN�����Ă�������
		�Q�l�@http://blog.livedoor.jp/kotositu/archives/1923002.html

	�EptTimer�ԑg�\�ɂ��܂���
		sqlite3.exe��TvRemoteViewer_VB.exe�Ɠ����t�H���_�ɐݒu���Ă�������
		sqlite3.exe�̓O�O��΂���������Ǝv���܂�


	�y���萫�����z
	�EUDP,HLS�eexe��z�M�i���o�[���ɈႤexe���g�p�ł���悤�ɂ����B
�@		exe�����݂���t�H���_���ɔz�M�i���o�[��ǋL�����t�H���_����exe���g�p���܂��B
�@		��FHLS�A�v����ffmpeg���g�p���Ă���ꍇ
�@�@		�ʏ�`\bin\ffmpeg.exe���g�p���Ă���Ƃ���
�@�@		�`\bin1\ffmpeg.exe��p�ӂ��Ă����Δz�M1�̂Ƃ��Ɏg�p����悤�ɂȂ�܂��B
�@�@		�`\bin2\ffmpeg.exe��p�ӂ��Ă����Δz�M2�̂Ƃ��Ɏg�p����悤�ɂȂ�܂��B
�@�@		UDP�A�v���ɂ��܂��Ă����l�ł��B



��WEB�f�U�C����ύX�������ꍇ

	�Eindex.html
	index.html����StartTv�i�z�M�J�n�j���Ăяo�����ۂ�WEB���瑗����p�����[�^�[�Ƃ��Ă͍��̂Ƃ���ȉ���z�肵�Ă��܂��B
	WebRemoconvb��Web_Start()����ҏW����ΈႤ�����قȂ�WEB�݌v�ɂ��ł���ł��傤�B
	�p�����[�^�[	value�̗�
	"num"		"1"					�X�g���[���i���o�[
	"BonDriver"	"BonDriver_pt2_t0.dll"			BonDriver�l�[��
	"ServiceID"	"54321"					�T�[�r�XID
	"ChSpace"	"0"�iCS��1)				�`�����l���X�y�[�X
	"resolution"	"640x360"				�𑜓x�i�c���̑g�ݍ��킹�͌��܂��Ă��܂��j
	"Bon_Sid_Ch"	"BonDriver_pt2_t0.dll,54321,0"		��L�R�𓯎��ɐݒ�
	"redirect"	"ViewTV2.html"				�z�M�J�n��W�����v����y�[�W


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



��HLS�A�v���ɂ��܂���

	�Effmpeg�����g�p�̏ꍇ�y�����z
	http://ffmpeg.zeranoe.com/builds/
	�����ł�x64�ŐV�ł��g�p�B�S�܂ł͓����N���m�F�i�Â��o�[�W�����ł�3�ȏ�͕s����ł����j
	ffmpeg�C���X�g�[�����presets�t�H���_���ɓ�����libx264-ipod640.ffpreset���R�s�[���Ă��������B
	�Q�l�Fhttp://frmmpgit.blog.fc2.com/blog-entry-179.html
	640x360�ȊO�̉𑜓x�͎����Ă��܂���B
	�Ɋ�ɐV���ȃt���[�����ǂݍ��܂ꂸm3u8���X�V����Ȃ��Ȃ錻�ۗL��


	�EVLC�����g�p�̏ꍇ
	������HLS_option.txt�̓��e��HLS_option_VLC.txt�̓��e�ɍ����ւ��ăt�H�[����ŉ𑜓x��I���������Ă��������B
	�����̃e�X�g�ɂ��܂��ƁA�����̃v���Z�X���N�����܂���VLC�̋�������ϕs����ɂȂ�܂��B�Q�Ȃ�܂��Ȃ�Ƃ��B
	2.0.5�A2.1.0�Ƃ��N���b�V�������A���X�ƍċN�����J��Ԃ��N�����ɒ�~���邱�Ƃ��B2.0.5�ł͉������Ȃ��Ȃ錻�ۂ�1�x�L��


	�EHLS�I�v�V�����Ŏ��s���ɕϊ������萔
	%UDPPORT%	�\�t�g�Ŏ����I�Ɋ��蓖�Ă�ꂽudp�|�[�g
	%WWWROOT%	WWW��root�t�H���_
	%FILEROOT%	m3u8��ts���쐬�����t�H���_
	%HLSROOT%	HLS�A�v�������݂���t�H���_
	%HLSROOT/../%	HLS�A�v�������݂���t�H���_�̂P��̐e�t�H���_�iffmpeg�𓀎��̃t�H���_�\���ɑΉ��j
	%rc-host%	"127.0.0.1:%UDPPORT%"�ɕϊ�����܂��B
	%NUM%		�X�g���[���i���o�[
	%VIDEOFILE%	�r�f�I�t�@�C���ɕϊ��i���ۂ́u-i %VIDEOFILE%�v�̌��ߑł���-i�̌��̕����񂪃t�@�C�����ɕϊ��j
	%VIDEODURATION%	�r�f�I�̒���(�b)�@�s���ȏꍇ��0

	�EStartTv.html�Ăяo�����̃I�v�V����	
	hlsOptAdd	�z�M����HLS�I�v�V�����ɓ��I�Ƀp�����[�^�[��ǉ��ł��܂�
	hlsOptAdd=[1�`2],[1�`4],[������]
	��FhlsOptAdd=2,2,-map 0,0 -map 0,1
	�܂��A�u_-_�v�ŋ�؂邱�Ƃɂ�蕡���̏����������s�����Ƃ��ł��܂�
	��FhlsOptAdd=2,9,-hls_-_2,2,-map 0,0 -map 0,1	�i-hls�������폜�������-map�`��ǉ��j
	��1�p�����[�^�F	1=HLS�I�v�V������-i���O�ɕ������ǉ����܂�
			2=HLS�I�v�V������-i�̌�ɕ������ǉ����܂�
	��2�p�����[�^�F	HLS�I�v�V������ɓ����p�����[�^���������ꍇ�ɂǂ����邩
			1=�ύX���Ȃ�
			2=������HLS�I�v�V������̃p�����[�^��j�����V�����ǉ����܂�
			3=������HLS�I�v�V�����̗v�f�ɒǉ������݂܂��i��F-vf a��-vf a,b)
			4=�P���ɒǉ�
			9=�w��p�����[�^�������폜

	�ENHKMODE �ڍ�
		'0=��E���@HLS�I�v�V�����ύX����
                '1=NHK�Ȃ�Ύ�@NHK�ȊO�͎�E��
                '2=NHK�Ȃ�Ε��@NHK�ȊO�͎�E��
		'3(ini����)=�I����
                '4=�S�Ă̕����ǂŉ���1 -map 0:0 -map 0:1
                '5=�S�Ă̕����ǂŉ���2 -map 0:0 -map 0:2
                '6=�S�Ă̕����ǂŉ���3 -map 0:0 -map 0:3
		'9=NHK�Ȃ��VLC�ōĐ�
                '11=�S�Ă̕����ǂŎ�
                '12=�S�Ă̕����ǂŕ�



��WEB�C���^�[�t�F�[�X�i�ꕔ�@���̑���client��readme.txt�Q�Ƃ̂��Ɓj
	
	WI_GET_VIDEOFILES.html	�r�f�I�t�@�C���ꗗHTML���i��Ԃ�
	WI_GET_VIDEOFILES2.html	�r�f�I�t�@�C���ꗗ���e�L�X�g�ŕԂ�
		��L�Q�̃C���^�[�t�F�[�X�p�p�����[�^�[�F
		vl_refresh	1=�����r�f�I�t�@�C���X�V
		vl_startdate	�w������O�̃r�f�I�t�@�C���𒊏o����
		vl_volume	�����\�����邩�i�ŏI���t�̃t�@�C����ǉ�����̂ŕs���m�j
		��L�p�����[�^�[��%SELECTVIDEO%��ϊ�����SelectVideo.html�ɂ��L��
	WI_FILE_OPE.html	�t�@�C���ǂݏ���(UTF-8)
		�p�����[�^�[�F
		fl_cmd		dir, read, write, write_add, delete
		fl_file		�t�H���_�����̓t�@�C�����i%WWWROOT%����̑��Έʒu�j
		fl_text		�������ޓ��e
		temp		dir�̏ꍇ�̃t�B���^(���w��̏ꍇ�́u*�v)�@��F�u*.jpg�v��umystream*�v
		���ʁF
		0,SUCCESS(+���s[����])�@���́@2,[�G���[���e]
	WI_STREAMFILE_EXIST.html?fl_file=[�t�@�C����]
		�X�g���[���t�H���_���Ƀt�@�C�������݂��邩�ǂ���
		��FWI_STREAMFILE_EXIST.html?fl_file=mystream1_thumb.jpg
�@�@�@�@�@�@�@�@�@�@WI_STREAMFILE_EXIST.html?fl_file=file_thumbs/����t�@�C����.jpg
		�Ԓl�F�@���݂����1�A���݂��Ȃ���΋�
	WI_GET_PROGRAM_[TVROCK,EDCB,PTTIMER].html(?temp=1-3)
		TVROCK,EDCB����ԑg�\���擾
		�I�v�V���� temp=1�`3 ���w�肷�邱�Ƃɂ�莟�ԑg�����݂���Ε����Ď擾(PTTIMER�ɂ͖��Ή��j
		1:�Ԓl�̊e�ԑg���L�q�͏]���ʂ�
		2:�Ԓl�̊e�ԑg�����̎��ԑg���`���Ɂu[Next]�v��t��
		3:�Ԓl�̊e�ԑg��񖖔��Ɍ��ԑg�u,0�v�����ԑg�u,1�v����t��
		4�ȏ�:�ԑg�I���܂�temp���ȓ������c���Ă��Ȃ��ꍇ�͌��ԑg�̏ڍח��Ɏ��ԑg����\���i�f�[�^�͖��w��Ɠ����j
	WI_GET_CHAPTER.html?temp=�^��t�@�C���t���p�X
		�^��t�@�C����.chapter�t�@�C���̓��e���擾�ichapters�t�H���_�̒��ł��j
	WI_WRITE_CHAPTER.html?temp=num,�������ރ`���v�^�[������
	WI_GET_HTML.html?temp=[HTML�擾���@],[�G���R�[�h],[UserAgent],http://www.google.co.jp/
		HTML�擾���@	1: webbrowser UserAgent�����B�G���[�ɂ�茻��g�p�s��
				2: webclient
				3: HttpWebRequest
		��FWI_GET_HTML.html?temp=2,UTF-8,,http://www.google.co.jp/
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



��Windows��ł�m3u8�Đ��ɂ��܂���

	�u�n�f�W�̃��P�t���V�X�e�������X�� part3�v�ɏ�����Ă܂�



���e�X�g��

	Windows7 x64
	VisualStudio2010

	RecTask�C���X�R	=	D:\TvRemoteViewer\TVTest
	BonDriver	=	D:\TvRemoteViewer\TVTest
	%WWWROOT%	= 	D:\TvRemoteViewer\html
	%FILEROOT%	=	D:\TvRemoteViewer\html
	%HLSROOT%	=	D:\TvRemoteViewer\ffmpeg-20140628-git-4d1fa38-win64-static\bin
	%HLSROOT/../%	=	D:\TvRemoteViewer\ffmpeg-20140628-git-4d1fa38-win64-static
	(%HLSROOT%)	=	D:\TvRemoteViewer\vlc

	iPad(��3����jiOS7 safari�AAndroid(Nexus7��)



���C��������ǉ������肵�ė~�����Ƃ���

	�E�S�ʓI�ɃN���X�Ƃ������̂��킩���ĂȂ��E�E���`�܂�����
	�Effmpeg�̉��ւȏI��



��ptTimer�ɂ�1�X�g���[�������g�p�ł��Ȃ����Ƃւ̑΍�
	1. BonDriver_ptmr.dll��K���Ȗ��O��4�R�s�[���܂�
	2. ���ꂼ���.ch2�t�@�C����BonDriver_ptmr.ch2����R�s�[���Ĉȉ��̒ʂ�ҏW���܂�
	   ch2�t�@�C������4�̃`�����l����ԃu���b�N������܂����A1�Ɍ��肷��悤�ɂ��܂��B
	   �Ⴆ�΁A1�߂́u;#SPACE(1,T0)�v�A2�߂́u;#SPACE(3,T1)�v�A3�߂́u;#SPACE(0,S0)�v�A4�߂́u;#SPACE(2,S1)�v�A
           �Ƃ����ӂ��Ƀ`�����l����Ԃ��P�����ɂ��܂�
	3. �����4�X�g���[���S�Ďg�p���邱�Ƃ��ł��܂�



������

	0.01	���������Ă����@���͔��Ȃ��Ă���
	0.02	�����ȂƂ����C�����ǉ�
	0.03	�R���\�[���\���E��\���̃`�F�b�N�{�b�N�X��ݒu
		�z�M���̃X�g���[���ԍ����t�H�[����ɕ\��
		�z�M���̃X�g���[���ԍ����^�X�N�g���C�}�E�X�I�[�o�[���ɕ\��
		ffmpeg��HLS_option.txt���e���C���i�p�X���u"�v�ň͂��������ł��j
	0.04	��d�N�������݂��Ƃ��ɗ�O�G���[���o��o�O���C��
		�ǂ�html�y�[�W�ł��p�����[�^�[�u�����s���悤�ɂ���
		WEB�C���^�[�t�F�[�X�̏C���iHTML������������K�v�������Ȃ�܂����j
	0.05	ViewTV.html�Ń`�����l���؂�ւ����ɉ𑜓x�������p���悤�ɂ���
		ViewTV.html�̃r�f�I�̕��ƍ�����ϐ���
		ViewTV.html�̏C��
	0.06	UDP�A�v���p�I�v�V��������ǉ�
		UDP�I�v�V�����쐬���̃o�O�C��
	0.07	ffmpeg�g�p���̎���ts�폜���C��
	0.08	�t�@�C���Đ��ɑΉ��iffmpeg�̂݁j
	0.09	BASIC�F�ؑΉ�
		VideoPath.txt�ɃT�u�t�H���_���܂߂邩�ǂ����̃I�v�V������ǉ�
	0.10	�X�g���[�������{�^����ԍ����ɕ��ёւ���悤�ɂ���
		VideoPath.txt�����݂��Ȃ��Ƃ��ɗ�O�G���[���o�Ă����o�O���C��
	0.11	�z�M�X�^�[�g�{�^������������A�����y�[�W�փ��_�C���N�g����悤�ɂ���
		html�̃f�U�C�����_��ɂł���悤����ϐ��̑O����ɕ\������html�^�O���w��ł���悤�ɂ���
		index.html��ViewTV.html�̃f�U�C���C��
	0.12	�𑜓x���t�H�[����̃I�v�V�����g�p�Ŏ������A�`�����l���؂�ւ����Ɉ����p����Ȃ������o�O���C��
		�R���\�[����\���Ɏw�肵�Ă�VLC�̃R���\�[�����\������Ă��܂��o�O���C��
		UDP&HLS�I�v�V���������O�ɕ\������^�C�~���O���C��
	0.13	ffmpeg�g�p���ABS1��BS�v���~�A���������̂�vlc�ɐ؂�ւ���I�v�V������VideoPath.txt�ɒǉ�
	0.14	%FILEROOT%�w�肪�S���@�\���Ă��Ȃ������o�O���C����������ViewTV.html���C������
	0.15	�X�g���[�������{�^���ɕ����ǖ���\������悤�ɂ���
	0.16	BonDriver�ƕ����ǂ��擾����t�@�C���A�N�Z�X���ɗ͌��炷�悤�ɂ����Bhtml�\��������
	0.17	%FILEROOT%���w�肳��Ă���Ƃ���VLC���������ꏊ�Ƀt�@�C�������Ȃ��o�O���C��
		HLS_option_VLC.txt�̏C��
	0.18	�R���\�[����\�����Ȃ��ݒ�ɂ��Ă��Ă�VLC�̑����\������Ă��܂��Ă����o�O���C��
	0.19	ViewTV.html��\�������Ƃ��Ɏ������̕����ǂ��킩��悤�ɂ���
	0.20	���f���̔ԑg���e��\������{�^����ݒu�iVideoPath.txt�ɃI�v�V�����ǉ��j
	0.21	�n�f�W�ԑg�\�Ɏ����{�^����t����
		�n�f�W�ԑg�\�̐ݒ荀�ڂ�VideoPath.txt�ɒǉ�
	0.22	TvRock����ԑg�\���擾�ł���悤�ɂ����i�u���E�U��iPhone�p�ԑg�\����x�����\������K�v�L��j
	0.23	EDCB����ԑg�\���擾�ł���悤�ɂ����iEpgTimerSrv.ini��ҏW����K�v�L��j
	0.24	�킩�肸�炢�̂Őݒ�t�@�C����TvRemoteViewer_VB.ini�ɂ����B
		�iVideoPath.txt�����݂���ꍇ��VideoPath.txt��D�悵�܂��j
		RecTask�𖼑O�w��ŏI�������邩�ǂ����̃I�v�V������TvRemoteViewer_VB.ini�ɒǉ�
		�i�^���RecTask���g�p���Ă�������l���j
	0.25	EDCB�ԑg�\�̕s����C��
	0.26	�����ǖ��ϊ����l�b�g�EEDCB�ETvRock�ŕ�����ini�ɍ��ڂ�ǉ�
		�킩��Â炩�����̂�TvRemoteViewer_VB.ini�̓��e�𐮗�
	0.27	BASIC�F�؂��C��
		NHK�֘A�ŉ��������������Ȃ邱�Ƃւ̑Ώ����@��I���ł���悤�ɂ���
		�iTvRemoteViewer_VB.ini�ɃI�v�V���� NHK_dual_mono_mode ��ǉ��j
		HTML�����X�C��
	0.28	index.html�ɂ�����NHK�֘A�ȊO�͉����I����\�����Ȃ��悤�ɂ���
	0.29	���N�����ɐݒ�r���ŗ�O�G���[���N����o�O���C��
	0.30	�t�@�C���Đ��Ń��_�C���N�g�Ɏ��s���Ă����o�O���C��
	0.31	VLC���w�肳��Ă��Ȃ��Ă�NHK�����I���ɁuVLC�ōĐ��v�ƕ\������Ă����o�O���C��
	0.32	�A�C�R���̒ǉ�
		�R�[�h�̍œK��
	0.33	�ꕔ��CS�ǂ̎����y�[�W�őI�ǖ����Ԉ���ĕ\�������o�O���C��
	0.34	�z�M���~�𗧂đ����ɍs�����ꍇ�ɗ�O�G���[���o�邱�Ƃ��������o�O���C��
	0.35	�W��HLS�A�v����VLC��I��ł���Ƃ��ɂ�NHK�̉������[�h�I�����\������Ă����o�O���C��
	0.36	�z�M�ċN���ɂȂ����ꍇ�ɕ\�������u�z�M����Ă��܂���v�y�[�W�������I�ɍēǂݍ��݂���悤�ɂ���
	0.37	�Đ����J�n�\�Ɣ��f����ts�t�@�C�������w��ł���悤�ɂ���
		MIME TYPE��ݒ肷��I�v�V������ini�ɒǉ�
		RecTask���I�����Ȃ��I�v�V����(Stop_RecTask_at_StartEnd)����p���Ă��Ȃ������o�O���C��
		HLS�I�v�V�������C���i�Đ��ҋ@���Ԃ̏k���j
	0.38	�ݒ�t�@�C����ǂݍ��ލۂ̃J�����g�t�H���_����������w�肷��悤�ɂ���
	0.39	MIME TYPE�̐ݒ���R�����g�A�E�g�����Ƃ��ɗ�O���N�����Ă����o�O���C��
	0.40	�t�@�C���֌W�ŗ�O���N�������ꍇ�̃��O�o�͂ƃG���[������ǉ�
		index.html�ւ̃��_�C���N�g�R�[�h�̏C��
	0.41	���O�t���p�C�v�֘A�ŃG���[���N�������ꍇ�͖��O�t���p�C�v���g�p���Ȃ��悤�ɂ���
	0.42	ViewTVx.html�ɂ����āANHK���[�h�̑I�����ł��Ȃ������s����C��
		����ɔ���ViewTVx.html�ɃX�N���v�g��ǉ�
	0.43	ini�ɖ��O�t���p�C�v���擾����O���v���O�������w��ł���I�v�V������ǉ�
	0.44	0.43�Œǉ������I�v�V�����̔p�~
		�t�H�[����̃{�^����ENTER�L�[�ŉ�����Ȃ��悤�ɑ΍�
	0.45	VLC�Đ������݂�ۂ̃G���[�΍������
	0.46	���O�t���p�C�v�擾���@��ύX�i�����͂������������X�Ɋ��Ӂj
	0.47	���ʂȃG���[�������C��
		�ׂ����o�O�t�B�b�N�X
	0.48	HLS�I�v�V��������%NUM%��ϊ�����悤�ɂ���
		HLS_option.txt�����X�C��
	0.49	ini�ɍő�z�M���𐧌��ł���悤�I�v�V������ǉ�
		�yHTTP�X�g���[�������z
		http�X�g���[���z�M�iVLC���ŃN���C�A���g�\�t�g���g�p�����Ƃ��̂݁j
		�y���萫�����z
		ini�ɋN������UDP,HLS�A�v����CPU�D��x���w�肷��I�v�V������ǉ�
		�y���萫�����z
		UDP,HLS�eexe��z�M�i���o�[���ɈႤexe���g�p�ł���悤�ɂ����B
		exe�����݂���t�H���_���ɔz�M�i���o�[��ǋL�����t�H���_����exe���g�p���܂��B
		��FHLS�A�v����ffmpeg���g�p���肢��ꍇ
		�ʏ�`\bin\ffmpeg.exe���g�p���Ă���Ƃ���
		�`\bin1\ffmpeg.exe��p�ӂ��Ă����Δz�M1�̂Ƃ��Ɏg�p����悤�ɂȂ�܂��B
		�`\bin2\ffmpeg.exe��p�ӂ��Ă����Δz�M2�̂Ƃ��Ɏg�p����悤�ɂȂ�܂��B
		UDP�A�v���ɂ��܂��Ă����l�ł��B
	0.50	ini��VideoPath��()�������Ă����ꍇ�ɓ���ꗗ������Ɏ擾�ł��Ȃ������o�O���C��
	0.51	�z�M�������y�[�W�Ƃ���Waiting.html�A�G���[�y�[�W�Ƃ���ERROR.html��WWW�t�H���_�ɒǉ�
		���݂��Ȃ��ꍇ�͏]���ʂ�v���O���������ŕ\�����܂�
	0.52	�J�X�^�}�C�Y�ł���悤�v���O��������������HTML�t�H�[���v�f��class����t������悤�ɂ���
	0.53	�C���^�[�l�b�g�ԑg�\��ύX�i���������̂ݑΉ��j
	0.54	�C���^�[�l�b�g�ԑg�\��ύX�i�S���Ή��E�k�C���̒n��ԍ���1�̂݁j
	0.55	�ԑg�\�Ǝ����y�[�W�ɉ𑜓x�I����t����
		HTML�ϊ��ɉ𑜓x�I����ǉ�
		SelectVideo.html�̏C��
	0.56	�t�@�C���Đ��y�[�W���C��
		SelectVideo.html�̏C��
	0.57	TvRock��iphone�ԑg�\�ɂė\��0�ȊO���w�肵�Ă���ꍇ�ɔԑg�\�̎��������������Ȃ錻�ۂւ̑Ώ�
		ini�� NHK_dual_mono_mode ��11��12��ǉ��iNHK�ȊO�ł�dual_mono��ݒ肷��Ƃ��Ɏg�p�j
	0.58	�r�f�I�t�@�C���ꗗ�ɕ\������t�@�C���̊g���q���w��ł���悤ini�ɃI�v�V������ǉ�
		WEB���BonDriver�ꗗ�ɕ\���������Ȃ�BonDriver���w��ł���悤ini�ɃI�v�V������ǉ�
	0.59	����BonDriver��ł̃`�����l���ύX�Ȃ�Ζ��O�t���p�C�v���g�p���邱�Ƃɂ���
		��~���[�`���̃o�O�C��
		WEB�C���^�[�t�F�[�X�p�̔ԑg�\API��ǉ�
	0.60	WEB�C���^�[�t�F�[�X�̏C���ƒǉ�
		UDP�A�v�����z�M�J�n���Ă���HLS�A�v�����N������܂łɓ����҂����Ԏw���ini�ɒǉ�
		BonDriver�p�X�̕s����C��
	0.61	�z�M���X�g���[���ꗗ�\���ł�BonDriver�p�X�̕s����C��
		���O�t���p�C�v���g�p����UDP�A�v�������ۂɔz�M���J�n����܂őҋ@����悤�ɂ���
	0.62	0.60�œ��������E�F�C�g�̔p�~�iini����폜�j
		���O�t���p�C�v�ԍ��̎擾���@�̕ύX
	0.63	���O�t���p�C�v�R�[�h�̏C��
	0.64	UDP�A�v�����z�M�J�n���Ă���HLS�A�v�����N������܂łɓ����҂����Ԏw���ini�ɕ���
	0.65	�z�M����UDP�A�v�������HLS�A�v�����N������悤�ɂ���
	0.66	WEB�C���^�[�t�F�[�X��ǉ�
		TvRemoteViewer_VB_client 1.01�ɑΉ�
	0.66b	�z�M�J�n�菇�̂�0.59���̂��̂ɖ߂��������o�[�W�����iini�� UDP2HLS_WAIT = 500�j
	0.67	0.66b���x�[�X�Ƀp�C�v�ԍ��擾���@�ύX��UDP�z�M�m�F��ǉ��iini��UDP2HLS_WAIT=500�����j
		�N�����Ƀ`�����l�������擾���邱�Ƃɂ���
	0.68	WEB�C���^�[�t�F�[�X���C��
		HTML���Łu���[�U�[��:�p�X���[�h@�v�ɕϊ��ł���悤�ɂ����iini�ŋ�����K�v�L��j
	0.69	RecTask�̖��O�t���p�C�v�ɂ��I�������s�����ꍇ�͋����I�������邱�Ƃɂ���
		���O�����̏C��
		�v���Z�X�ċN�����̋������C��
	0.70	���O�t���p�C�v�擾���@���Â������ɖ߂���
		�t�@�C���Đ��ɂ����ăX�y�[�X��؂�(���por�S�p)�ŕ������[�h�ɂ�钊�o�ɑΉ�
		SelectVideo.html��NHK�����I����ǉ�
	0.71	HTTP���N�G�X�g�̔񓯊���
		HTML�y�[�W���ϐ��̕ϊ����C��
	0.72	ffmpeg��HTTP�X�g���[�~���O�z�M�ɑΉ��i�v�N���C�A���g�j
		HLS_option_ffmpeg_http.txt�̏C��
		ini�ɃX�g���[���ؒf���ɔz�M�I���܂ł̕b����ݒ肷�鍀�ڂ�ǉ�
	0.73	�ꕔ�n��ɂ����Ēn�f�W�ԑg�\���\������Ȃ��o�O���C��
	0.74	�C���^�[�l�b�g�ԑg�\��ύX(�n��ԍ����ύX����Ă��܂��̂�ini�����ēK�؂ɐݒ肵�Ă�������)
	0.75	HLS�p�j�R�j�R�����R�����g�擾�\�t�gNico2HLS�ɑΉ�
		ViewTV�`.html�̕ύX
	0.76	TvRock��EDCB�ԑg�\�̕�����NG���[�h��n�f�W�̂��̂��番����ini�ɒǉ�
		EDCB�ԑg�\�ɂ����Ĕԑg�����擾���Ȃ��ݒ��ini�ɒǉ�
	0.77	%FILEROOT%�����R�ɐݒ�ł���悤�ɂ����iRAM�h���C�u�w����l���B�h���C�u���̂��̂��w�肷�邱�Ƃ͂ł��܂���j
		��F%FILEROOT%��Z:\stream�ɐݒ肵���ꍇ�Ahttp://�`:40003/stream/�`�ւ̃A�N�Z�X��Z:\stream�Ɋ���U���܂�
	0.78	�j�R�j�R�����p�ϊ�������Ƃ���%JKNUM%��%JKVALUE%��ǉ�
		ch_sid.txt�̒ǉ�
	0.79	HTML�����R�[�h�̕W����UTF-8�ɂ���(ini�ɕ����R�[�h�w��I�v�V������ǉ��j
		HTML�t�@�C���̕����R�[�h��UTF-8�ɂ���
	0.80	HTTP�z�M(ffmpeg)�̏C��
		ini��ffmpeg�̃o�b�t�@���w�肷��HTTPSTREAM_FFMPEG_BUFFER��ǉ�
	0.81	�z�M��~���ɂ��֘A�t�@�C�����폜���邱�Ƃɂ����i����ڑ����̃X�s�[�h�A�b�v�_���j
		Nico2HLS�ւ̑Ή����I��
		PC�N���C�A���g�̕s��ɂ�菉��z�M�����Ɏ��s�����ꍇ�ɔz�M��~����悤�ɂ���
		�A�C�h�����Ԃ��w�肵�������ɒB����ƑS�ؒf����悤ini��STOP_IDLEMINUTES��ǉ�
	0.82	�t�@�C���Đ�����WI_GET_LIVE_STREAM.html��BonDriver���Ƀt���p�X�t�@�C�������L������悤�ɂ���
		HTTP�z�M�̃t�@�C���Đ��ɂ�����ffmpeg��2�d�ɋN�����Ă��܂��o�O���C��
	0.83	WEB�C���^�[�t�F�[�X�iWI_GET_PROGRAM_NUM�j��ǉ�
	0.84	�t�@�C���Đ��ɂ����ē�����ASS�t�@�C��������Ƃ��͎������n�[�h�T�u����悤�ɂ���
		�iffmpeg�̃t�H���_����ffmpeg-20140628-git-4d1fa38-win64-static�������ꍇ�͋@�\���܂���j
		ffmpeg.exe�����݂���t�H���_��fonts\fonts.conf��ݒu����K�v������܂�
		�Q�l�@http://peace.2ch.net/test/read.cgi/avi/1413523104/779
	0.85	HLS�I�v�V��������-vf�����݂���ꍇ�ł��n�[�h�T�u�ɑΉ�����
	0.86	�t�@�C�����o���ɕ����������Ă����o�O���C��
	0.87	�t�@�C���Đ����ɊJ�n�V�[�N�b���w��ł���悤�ɂ���
		ini�ɕW���V�[�N�b���w�肷��VideoSeekDefault��ǉ�
		SelectVideo.html���C��
	0.88	�t�@�C���Đ����V�[�N�̍�����
		�N�����Ƀt�H���_�ƕK�v�ȃt�@�C���`�F�b�N���s���悤�ɂ���
		�t�@�C���Đ����̃V�[�N���Ɏ�:��:�b�i1:30��1:20:00�j�̎w�肪�ł���悤�ɂ���
	0.89	WEB�C���^�[�t�F�[�X�̖₢���킹�Ƀt�@�C���Đ����ɃV�[�N�����b����Ԃ��悤�ɂ���
		�iWI_GET_LIVE_STREAM��WI_GET_PROGRAM_NUM�̖����Ɂu,�v��؂�Œǉ�����Ă��܂��j
		WI_GET_PROGRAM_NUM�̃o�O���C��
	0.90	HTTP�z�M���̃`�����l���؂�ւ����萫����
		�N������ini��Stop_RecTask_at_StartEnd�����f����Ă��Ȃ������o�O���C��
	0.91	WEB�C���^�[�t�F�[�XWI_GET_VIDEOFILES2��ǉ�
		�r�f�I�t�H���_�����X�V�`�F�b�N�@�\�̒ǉ�
	0.92	�`�����l���؂�ւ����Ɉꕔ�̃`�����l���ňႤ�ǂ��I�ǂ���Ă��܂��s�������
		�r�f�I�t�@�C���ꗗ�쐬�菇�̏C��
		�s�K�v�ȃr�f�I�t�H���_�����X�V�`�F�b�N���s���錻�ۂ�����
	0.93	ini��BonDriver_NGword���@�\���Ă��Ȃ������o�O���C��
		�p�C�v���g�p�����`�����l���؂�ւ����C��
	0.94	ini�̗D��BonDriver(TvProgramD_BonDriver1st,TvProgramS_BonDriver1st)�𕡐��L���\�ɂ���
		WEB�C���^�[�t�F�[�X�iWI_GET_TVRV_STATUS�j�ɗD��BonDriver�\����ǉ�
		ini�ɔԑg�\��̔z�M�i���o�[�𐧌�����TvProgram_SelectUptoNum��ǉ�
	0.95	HLS_option_ffmpeg_file.txt�����݂��Ă���΃t�@�C���Đ�����HLS�I�v�V�����Ƃ��Ďg�p����悤�ɂ���
		SelectVideo.html�ɂ����ăL�[���[�h���o���ł��Ȃ��Ȃ��Ă����s�������
		SelectVideo.html�̃L�[���[�h���o�Ńt�H���_���������Ɋ܂߂�悤�ɂ���
	0.96	WEB�C���^�[�t�F�[�X�iWI_FILE_OPE�j��ǉ�
	0.97	�z�M���ɌÂ��f�Ѓt�@�C��ts���폜���Ȃ��悤�ɂ���I�v�V����(OLDTS_NODELETE)��ini�ɒǉ�
	0.98	0.93�ł̃p�C�v�`�����l���ؑւ��̃o�O���C��
	0.99	SelectVideo�y�[�W�̉𑜓x�Z���N�g��HLS_option_ffmpeg_file.txt���̂��̂ɂ���悤�ɂ���(���݂����)
	1.00	WI_GET_LIVE_STREAM�̖����ɔz�M�t�@�C��URL��ǉ�
	1.01	ffmpeg��HTTP�z�M�ɂ�����ffmpeg�N���Ɏ��Ԃ����������ꍇ���l���������ؒf�ɗ]�T����������悤�ɂ���
	1.02	HTTP�z�M����NHK���[�h�I�����@�\���Ă��Ȃ������o�O���C��
		HTTP�z�M���AHLS_option_[HLS�A�v��]_http.txt���̓Ǝ��𑜓x�I���ɑΉ�
	1.03	�W���Y�t��HLS_option.txt��HLS_option_ffmpeg.txt���C������
		HLS_option_ffmpeg_file.txt��ǉ�
		HLS_option_ffmpeg_file.txt���g�p���A�𑜓x�����w��̏ꍇ�̓t�H�[����̃I�v�V�������𑜓x���g�p����悤�ɂ���
	1.04	NHK�z�M����NHK���[�h11&12���@�\���Ă��Ȃ������o�O���C��
	1.05	HTTP�z�M���AUDP�A�v�����z�M������ԂɂȂ��Ă��Ȃ��Ƃ��ɔz�M�v�����������ꍇ�͒��f����悤�ɂ���
	1.06	HTTP�z�M���A�X�g���[�������f���ꂽ�Ƃ���HLS�v���Z�X���c���Ă����ꍇ�͏I�������邱�Ƃɂ���
		ffmpeg�t�@�C���Đ��ɂ����Ė��ϊ����̓n�[�h�T�u�����Ȃ��悤�ɂ���
	1.07	StartTv.html�p�I�v�V�����Ƃ���nohsub(1=�n�[�h�T�u�֎~�j��ǉ�
	1.08	�N�����ɃR�}���h���C���I�v�V�����u-dok�v��t����Γ�d�N����������悤�ɂ���
	1.09	�p�X���[�h���Í������ĕۑ����邱�Ƃɂ���
	1.10	�N������Bondriver�`�F�b�N�������ɂ���
	1.11	ini��EDCB�ԑg�\���X�J�p�[�v���~�A���p�ł��邱�Ƃ��w�肷��TvProgramEDCB_premium��ǉ�
		ini���ł�TvRock��EDCB�p�`�����l�����}�b�`���O�w��̕K�v���Ȃ�����
		ini�ɃX�J�p�[�v���~�A�����ɗD��I�Ɋ��蓖�Ă�BonDriver���w��ł���悤�ɂ���
		�iTvProgramP_BonDriver1st�j
		WI_GET_TVRV_STATUS.html��TvProgramEDCB_premium��TvProgramP_BonDriver1st��ǉ�
		BS-TBS��QVC�̃T�[�r�XID������ł��邱�ƂɑΏ�
		EDCB�ԑg�\��ł̃}�b�`���O�o�O���C���A�ƂƂ���TSID�ł��`�F�b�N����悤�ɂ���
	1.12	WI�ԑg�����̃J���}��S�p�ɕϊ�����悤�ɂ���
		ini�ɃX�J�p�[�v���~�A��SPHD�������Ɏg�p����RecTask���w��ł���悤�ɂ���(RecTask_SPHD)
	1.13	WI�ԑg���擾�i�n�f�W�ETvRock�j�ɂ�����NG�ǂ��\������Ă��܂��Ă����o�O���C��
	1.14	RecTask���`�����l���؂�ւ��Ɏ��s�������ɔz�M�𑱂��悤�Ƃ��Ă����o�O���C��
		WEB�C���^�[�t�F�[�X�A���O��\��(WI_SHOW_LOG.html�j��ǉ�
	1.15	�ԑg�\����<>���G�X�P�[�v
		TvProgramEDCB_premium�̎d�l�ύX�i0=�S�� 1=SPHD�̂� 2=SPHD�ȊO)
		EDCB��TvRock�̔ԑg�\�̕�����NG�w��ɃT�[�r�XID���g�p�ł���悤�ɂ���(���l�Ȃ�T�[�r�XID)
	1.16	EDCB�̔ԑg�\�ɕ\�����ׂ��ǂ�EDCB��WEB�T�[�r�X����擾����悤�ɂ���
		�ԑg�\���́u&�v�̃G�X�P�[�v
	1.17	ch2�t�@�C������̃`�����l�����擾�o�O���C��
	1.18	ini�Ƀt�@�C���ꗗ�쐬���Ƀt�@�C���T�C�Y0�̂��̂͋L�ڂ��Ȃ��ɂ���VideoSizeCheck��ǉ�
		�T�[�o�[�Ƃ��Ă̓X�g���[���Đ����̓t�@�C���ꗗ�X�V��Ƃ͍s��Ȃ����Ƃɂ���
		ch_sid.txt���������폜
	1.19	WI_GET_PROGRAM_[TVROCK,EDCB]�Ɏ��ԑg���擾����I�v�V������ǉ��i��ɐ������L�q)
		�W���ԑg�\�ɂ����Ĕԑg�I���܂�7����؂��Ă���ꍇ�ɏڍח��Ɏ��ԑg����\��
	1.20	�z�M�v�����ABonDriver���ʃX�g���[���Ŏg�p���ł���΃X�g���[���ԍ���ύX����悤�ɂ���
	1.21	ptTimer�ɑΉ��i�vsqlite3.exe��TvRemoteViewer_VB.exe�Ɠ����t�H���_�ɐݒu�j
		ini��ptTimer_path��TvProgramptTimer_NGword��ǉ�
		ini��BonDriver�̕����X�g���[���g�p��������Allow_BonDriver4Streams��ǉ�
		WEB�C���^�[�t�F�[�X��WI_GET_PROGRAM_PTTIMER��ǉ�
		�W��HTML��index.html���C����TvProgram_ptTimer.html��ǉ�
		�ԑg�\���擾�ł��Ȃ������Ƃ��ɃG���[���o�邱�Ƃ��������o�O���C��
		WI_GET_TVRV_STATUS��ptTimer_path��Allow_BonDriver4Streams��ǉ�
		VideoSizeCheck�o�O�C��
	1.22	ini��Stop_ffmpeg_at_StartEnd��Stop_vlc_at_StartEnd��ǉ�
		�i�ʗp�r��ffmpeg��vlc���g�p���Ă���ꍇ��0�ɂ��Ă��������j
	1.23	ini��EDCB��Velmy�ł�niisaka�łŔԑg�\��\���ł���悤EDCB_Velmy_niisaka��ǉ�
		�iBS����M�ł����ԂȂ�Ύw�肵�Ȃ��Ƃ��������f�ł���Ǝv���܂��j
		ptTimer�ԑg�\�쐬����.ch2�t�@�C���ɋL�ڂ���Ă�����̂̂ݕ\������悤�ɂ���
	1.24	�{���t�@�C���Đ��ɑΉ�
		�W��HTML��SelectVideo.html���C��
	1.25	�{���t�@�C���Đ����̃n�[�h�T�u�ɑΉ�
	1.26	�g�p���̃X�g���[���ɂ����Ď����t���t�@�C���Đ������݂�Ǝ��s���Ă����o�O���C��
	1.27	�t�@�C���Đ�����HLS�A�v�������������ɂȂ����ꍇ�͒�~����悤�ɂ����i�ċN���͖��ʂȉ\����j
	1.28	EDCB�̔ԑg���擾���@��ύX
		EDCB��Velmy�ł�niisaka�ł��Ȃ�ׂ������Ŕ��ʂ���悤�ɂ���
	1.29	�N������CtrlCmdCLI.dl�̑��݃`�F�b�N������悤�ɂ���
		EDCB�̎��ԑg�������X�s�[�h�A�b�v
	1.30	�t�@�C���Đ����s����ɂȂ�悤�Ȃ̂�1.27�ł̕ύX��p������
		�t�@�C���Đ���HLS_option_ffmpeg_file.txt�����݂��Ȃ��ꍇ��HLS�I�v�V���������������C��
	1.31	�σX�s�[�h�t�@�C���Đ����̃n�[�h�T�u�\���ɂ����ăR�����g���d�Ȃ�Ȃ��悤�ɒ���
	1.32	Tvmaid�ɑΉ�
		ini��Tvmaid_url��TvProgramTvmaid_NGword��ǉ�
		WEB�C���^�[�t�F�[�X��WI_GET_PROGRAM_TVMAID��ǉ�
		�W��HTML��index.html���C����TvProgram_Tvmaid.html��ǉ�
	1.33	�W��HTML�ɂ�����NHKMODE=3(�I����)�̏ꍇ��NHK�ȊO���I������悤�ɂ���
		�z�M�X�^�[�g���ɓn�����NHKMODE��ini�̐ݒ���D�悳����悤�ɂ���
		NHKMODE=4(-map 0,0 -map 0,1)��NHKMODE=5(-map 0,0 -map 0,1)��ǉ�
		�z�M�X�^�[�g���ɓn�����p�����[�^�[�Ƃ���hlsOptAdd��ǉ��i��q�j
	1.34	hlsOptAdd�̃o�O�C����-map�����-���t�����l���l��
		�W��HTML�̉���1�`3�̏C��
	1.35	UDP�`�����l���ύX�Ɏ��s�����ꍇ��UDP�v���Z�X����~���Ȃ������o�O���C��
	1.36	NicoJK�̃R�����g�t�@�C����ǂݍ��߂�悤�ɂ���
		ini��NicoJK_path,NicoConvAss_path,NicoJK_first��ǉ�
	1.37	�t�@�C���Đ��ɂ����ăG���R�[�h�I�����m3u8�I�[��������`�F�b�N���邱�Ƃɂ���
		����t�@�C�����ɃT�[�r�XID��񂪊܂܂�Ă���ꍇ��NicoJK�R�����g�@�\�������悤�ɂ���
		�i��F���͂悤�ɂ��[s0x410].ts�@[s1040]�ł��BTVTest�̕ۑ��`���ő}���\�j
	1.38	pt3Timer�ɑΉ�
		�z�M���J�n�����ptTimer�ԑg�\���\������Ȃ��Ȃ�o�O���C��
		�W��HTML��ViewTV�`.html��ptTimer&Tvmaid�ԑg�\�{�^����ǉ�
	1.39	NicoJK�R�����g�Đ��ŃR�����g�J�n���Ԃ����m�łȂ������o�O���C��
		NicoJK�R�����g�J�n���Ԃ𓮉�t�@�C���쐬�����ɂ��Aini��Nico_delay�Ŕ��������邱�Ƃɂ���
		��U�Đ��I���������t�@�C�����Đ��������ꍇ��NicoJK�R�����g������Ȃ������o�O���C��
	1.40	�^��t�@�C����.chapter�t�@�C���̓��e��ǂݏ����ł���悤�ɂ����ichapters�t�H���_�̒��ł��j
		�ǂݍ��݁FWI_GET_CHAPTER.html?temp=�^��t�@�C���t���p�X
		�������݁FWI_WRITE_CHAPTER.html?temp=num,�������ރ`���v�^�[������
	1.41	EDCB���Ǘ�����`�����l���ꗗ�擾��CtrlCmdCLI���g���悤�ɂ���
		ini�ɋ������Ŏ擾����I�v�V����(EDCB_GetCh_method)��ǉ�
	1.42	ini��RecTask���`�����l���ύX���ɑҋ@����ő�b��(RecTask_CH_MaxWait)��ǉ�
	1.43	TSTask�ɑΉ�
	1.44	BonDriver_TSTask.dll���Đ���₩��O����
		FILEROOT��WWWROOT�̎q�f�B���N�g���łȂ��ꍇ�Ƀ\�t�g�T�u���@�\���Ȃ��������ۂ�����
	1.45	NicoJK�R�����g�t�@�C����T�����[�`��������
		�c���Ă�������֘A�������폜
	1.46	����Đ��ɂ����ē���t�@�C����TOT����J�n���Ԃ𒲂ׂ�悤�ɂ���
	1.47	����Đ��ɂ����ĉߋ��ɓ����t�@�C�����Đ����Ă����ꍇ�ɊJ�n���Ԃ����Ⴆ��o�O���C��
	1.48	����Đ��ɂ����Ď������O���g�p���Ȃ��ꍇ��TOT�𒲂ׂȂ��悤�ɂ���
		WI_GET_HTML�ǉ�
	1.49	ini��MAX_STREAM_NUMBER��8���傫���ƃt�@�C���Đ��ŃG���[�ɂȂ�o�O���C��
		ViewTV%NUM%.html�����݂��Ȃ��ꍇ��ViewTV1.html���g�p���Ĕz�M����悤�ɂ���
	1.50	ini��chapter�t�@�C��������30���ȓ��̔ԑg�Ń`���v�^�[�쐬�����݂�I�v�V������ǉ�
		�imake_chapter��chapter_bufsec�j
	1.51	ini�Ƀ`���[�i�[���g�p���Ɉ�U�ʂ̃T�[�r�XID�ɍ��킹��I�v�V������ǉ�(openfix_BonSid)
	1.52	�N���E�I���E�S�z�M��~����RecTask��TSTask�����𖼑O�w��I�������Ă������g�p���̂��̂̂ݏI���Ƃ���
		openfix�Ń_�~�[�T�[�r�XID��chspace���l�����Ă��Ȃ��������Ƃɂ��z�M���s�o�O���C��
	1.53	�֌W�̖���URL�A�N�Z�X��}������悤�ɂ���
	1.54	BonDriver����`�����l�����擾����Ƃ���chspace�̈Ⴂ���l������悤�ɂ����iptTimer�΍�B��q�j
	1.55	�T���l�C���쐬�@�\�AWI_GET_THUMBNAIL�ǉ�
	1.56	WI_FILE_OPE��dir�w��Ńt�B���^�𗘗p�ł���悤�ɂ���
		WI_STREAMFILE_EXIST�ǉ�
		WI_GET_THUMBNAIL�C��
		�t�@�C���ꊇ�폜���኱������
		�t�@�C�����Ɂu,�v�������Ă���t�@�C���Đ��Ɏ��s���Ă������ƂɑΏ�
		�T���l�C���쐬���Ɂu#�v���u���v�Ƃ���Ƃ��낪�u��v�ɂȂ��Ă������Ƃ��C��
	1.57	ini�Ƀ��O�̍ő啶������ݒ�ł���log_size��ǉ��i�W��30000�����j
		WI_WRITE_LOG��ǉ�
		�I������TvRemoteViewr_VB.log�Ƀ��O���o�͂���悤�ɂ���
		ffmpeg�̃V�[�N���@��ύX����t�@�C�����w�肷��@�\���^�X�N�g���C�A�C�R���ɒǉ�
		ch_sid.txt����MX2�T�[�r�XID���C��
	1.58	WI_SHOW_MAKING_PER_THUMB�ǉ�
		30���҂��Ă����Ԋu�T���l�C���̍쐬���I�����Ȃ��ꍇ�̓v���Z�X���I��������悤�ɂ���
		���ʂ�҂����ɃT���l�C�����쐬����thru�w���ǉ�
	1.59	NicoJK�R�����g��T���ۂɁA����J�n���Ԓ�����TOT���g�p����ɂ����i�ȑO�̈ڍs���ɏC���R��j
		chapter�쐬�őS�p�L�[���[�h�����ׂ�悤�ɂ���
	1.60	HTML���M���A�܂�ɃG���[���N�����Ă����̂ő��M���@��ύX���Ă݂�
		����ɔ���ini��HTML_IN_CHARACTER_CODE��HTML_OUT_CHARACTER_CODE�𖳌��ɂ����iUTF-8�Œ�j
	1.61	1.60�łQ���������������������Ă����o�O���C��
	1.62	ini��HTML���s���@���w��ł���悤�ɂ���(html_publish_method)
		�ׂ����C��
	1.63	HTML�e�L�X�g�z�M�G���R�[�h��UTF-8�Œ�Ƃ���
		HTML�e�L�X�g�z�M������1.59�ȑO��W���Ƃ��邱�Ƃɂ����iini��html_publish_method�ŕύX�\�j
	1.64	html_publish_method=0�w�莞�Ɉꕔ�̊��ŕ����������Ă������ۂɑΏ�
	1.65	NicoConvAss��chapter�̂ݍ쐬��I�����Ă���ƃt�@�C���Đ��ŃR�����g������Ȃ��o�O���C��
	1.66	ini�ɓ���̒������擾�L�����w�肷��TOT_get_duration��ǉ��i�W���͎擾����=1�j
		ini��ts�ȊO�̓���̒������擾���邽�߂�WhiteBrowserWB_path��ǉ�
		HTML����%VIDEODURATION%�𓮉�̒���(�b)�ɕϊ�����悤�ɂ����B�킩��Ȃ��ꍇ��0
	1.67	ini��WhiteBrowserWB_path��p�~
		ts�ȊO�̓���̒����擾��ffprobe.exe���g�p����悤�ɂ���(ffmpeg.exe�Ɠ����t�H���_�Ɋ��ɑ��݁j
	1.68	ini��meta�^�ORefresh�L�q��ύX����meta_refresh_fix��ǉ��iAndroid UC�u���E�U�΍�j
	1.69	�T���l�C���쐬�m�F��������ƍs���悤�ɂ���
	1.70	�Z���ԂɘA���ŃT���l�C���쐬���悤�Ƃ���Ǝ��s�������Ă��܂����ۂɑΏ�
	1.71	NicoJK�R�����g�t�@�C���������[�`�����C��
	1.72	EDCB�ԑg�\�擾���Ƀz�X�g������IP�A�h���X�ւ̕ϊ��Ɏ��s���邱�Ƃ��������s����C��
	1.73	�X�g���[���ԍ�10�ȏ�̊֘A�t�@�C��������č폜���Ă��܂��o�O���C��



��ConnectedSelect.js��http://d.hatena.ne.jp/Mars/20071109�̃X�N���v�g���g�p�����Ă��������܂����B
��ch_sid.txt��NicoJKPlayMod��jkch.sh.txt���Q�Ƃ��C�������������̂ł��B��җl���肪�Ƃ��������܂��B
��CtrlCmdCLI.dll��EDCB�ɓY�t����Ă������̂ł��B��җl�y�єh���ł̍�җl���肪�Ƃ��������܂��B
