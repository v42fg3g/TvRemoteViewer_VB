TvRemoteViewer_VB v0.33


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
		�@http://[TvRock���ғ����Ă���PC�̃��[�J��IP]:[TvRock�̃|�[�g]/[���[�U�[��]/iphone
		�@�i��@http://127.0.0.1:8969/���[�U�[��/iphone�j
		�@�ɃA�N�Z�X����B
		�@����ƁAiPhone�p�ԑg�\���\������܂��B
		1. BS/CS����M���Ă���`���[�i�[��I������
		�@�iFireworks�ł͂��܂��`���[�i�[���؂�ւ��Ȃ���������܂���j
		2. �u�\��\���v�����u�����v�ɂ���B�܂������ȊO�ł������ł�����
		�@�����ŕ\�����ꂽ�ԑg�\���f�[�^�Ƃ��Ďg�p����܂��B
		3. �u���E�U�����

	�EEDCB�ԑg�\�ɂ��܂���
		EpgTimer.exe�����݂���t�H���_�ɂ���EpgTimerSrv.ini���J����[SET]�����
			EnableHttpSrv=1
			HttpPort=5510
		������������EpgTimer���ċN�����Ă�������
		�Q�l�@http://blog.livedoor.jp/kotositu/archives/1923002.html



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
	html���Ɉȉ��̕ϐ����L�����Ă����ƌĂяo���ꂽ�Ƃ��ɓK�؂Ȓl�ɕϊ�����܂�
	�ϊ��O				�ϊ���
	%NUM%				�X�g���[���i���o�[
	%SELECTBONSIDCH%		index.html����BonDriver��ServiceID&ChSpace��I������<SELECT>�Z�b�g���쐬
	%PROCBONLIST%			�z�M���̃X�g���[���i���o�[��BonDriver���e�L�X�g�ŕ\������
	%VIEWBUTTONS%			�X�g���[���̐����������{�^�����쐬


	�EViewTV[n].html�݂̂Ŏg�p�ł���ϐ�
	%SELECTCH%			ViewTV.html���Ŕԑg��I������<SELECT>���쐬����
	%WIDTH%				�r�f�I�̕�
	%HEIGHT%			�r�f�I�̍���
	%FILEROOT%			.m3u8�����݂��鑊�΃t�H���_


	�E�Ȃ��A%PROCBONLIST%�A%SELECTCH%�A%VIEWBUTTONS%�A%SELECTBONSIDCH%�A%SELECTNHKMODE%�@�ɑ΂��ẮA�v�f�̑O����ɕ\������html�^�O���w��ł��܂��B�v�f���̂����݂��Ȃ��ꍇ�͕\������܂���B
	����	%VIEWBUTTONS:[�O���ɕ\������html�^�O]:[�{�^���ƃ{�^���̊Ԃɕ\������html�^�O]:[����ɕ\������html�^�O]:[�v�f���\������Ȃ��ꍇ�ɑւ��ɕ\�������html�^�O]%
	��	%VIEWBUTTONS:�����\�X�g���[��<br>:<br>===================<br>:�{�^���������Ă�������<br>%
	����	�����\�X�g���[��
		[�X�g���[��1������]
		===================
		[�X�g���[��2������]
		�{�^���������Ă�������



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
	"%UDPPORT%"	�\�t�g�Ŏ����I�Ɋ��蓖�Ă�ꂽudp�|�[�g
	"%WWWROOT%"	WWW��root�t�H���_
	"%FILEROOT%"	m3u8��ts���쐬�����t�H���_
	"%HLSROOT%"	HLS�A�v�������݂���t�H���_
	"%HLSROOT/../%"	HLS�A�v�������݂���t�H���_�̂P��̐e�t�H���_�iffmpeg�𓀎��̃t�H���_�\���ɑΉ��j
	"%rc-host%"	"127.0.0.1:%UDPPORT%"�ɕϊ�����܂��B



��Windows��ł�m3u8�Đ��ɂ��܂���

	�uwindows hls �Đ��v�ŃO�O��΂łĂ�����{��y�[�W����Đ��pflash���_�E�����[�h����html��ҏW���Ă������Windows���m3u8���Đ��ł��܂�



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


	�E�z��O�̃G���[�����B�e�X�g�s��


	�Effmpeg�̉��ւȏI��


	�EiPad�ł̎����Đ�


	�EHLS�A�v���������Ă�ӂ肵�Ȃ����~���Ă���Ƃ��ւ̑Ώ�
	�܂��A�z�M���~�܂�ƌ��Ă�ق����~�܂邩��ēǍ����~���܂���ˁB
	�Ȃ̂łƂ肠�����͑Ώ����Ȃ��Ă��������Ȃƕ��u���Ă܂��B


	�EWindows��ł̊e��j�R�j�R�������\�t�g�Ƃ̘A�g�i��]�j
	�\���^�C�~���O�̒���
	���������`�����l���ς�������������ς��Ƃ�
	�����Ȃ��Ƃ܂�Ȃ��ł�
	�ł��E�E����Ă݂Ďv���܂����B�g�������ƂȂ�����Splashtop����Ԍ����I���ȁ`�E�Eorz



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



��ConnectedSelect.js��http://d.hatena.ne.jp/Mars/20071109�̃X�N���v�g���g�p�����Ă��������܂����B
