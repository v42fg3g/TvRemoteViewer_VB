TvRemoteViewer_VB v2.92




��������ĂȂɁH

	���ꂽ�Ƃ�����PC��̃e���r�⓮������邽�߂̃\�t�g�ł�



����

	Windows�����삷��PC
	FrameWork4.5.x
	TVTest�����삷���
	
	UDP�\�t�g�F�@RecTask, TSTask
	HLS�\�t�g�F�@ffmpeg, VLC, QSVEnc,NVEnc



���N��

	�����Ӂ�
	Windows8�ȍ~�ł͈ȉ��̂ǂ��炩�̑��삪�K�v�ł��B
	�Eweb����_�E�N���b�N�ŊǗ��҂Ƃ��Ď��s.bat
	�@���E�N���b�N���ĊǗ��҂Ƃ��Ď��s�iTvmaid����Y�t�̃t�@�C�����ė��p�����Ă�����Ă��܂��B���ӂł��j
	�E�y���������z�R�}���h�v�����v�g����
	�@netsh http add urlacl url=http://+:40003/ user=XXXXX
	�@(XXXXX�͎��s���郆�[�U�[�A�������� Everyone �Ɠ��͂���)
	�@���͗�F�@netsh http add urlacl url=http://+:40003/ user=Everyone
	�E�y�����ł��Ȃ������ꍇ�zTvRemoteViewer_vb.exe���E�N���b�N�A
	�@�u�v���p�e�B�v���u�݊����v���u�Ǘ��҂Ƃ��Ă��̃v���O���������s����v�Ƀ`�F�b�N


	�ݒu��A�N������ƃ^�X�N�g���C����X�^�[�g���܂��B
	�_�u���N���b�N�Œʏ�̑傫���ɂȂ�܂��i�~�ŕ���ƏI�����Ă��܂��܂��j
	�e�p�����[�^�[��readme.jpg�Q�Ƃ̂���



������������

	���ۂ̎����⑀���WEB��ōs���܂��B
	��F	PC�iIP�A�h���X 192.168.1.100�@�|�[�g40003)�A��TvRemoteViewer_VB�����s���Ă���ꍇ��
		�����[�g�@�i�X�}�z�Ȃǁj�̃u���E�U��http://192.168.1.100:40003�ɃA�N�Z�X���Ă�������
		���[�J��PC�Ńe�X�g���Ă���ꍇ��http://127.0.0.1:40003�ł�



���ݒu readme.jpg�≺�ɏ������e�X�g�����Q�l�ɂ��Ă�������

	�f���炵������T�C�g
	http://vladi.cocolog-nifty.com/
	http://vladi.cocolog-nifty.com/blog/2014/10/iphoneandroidpc.html
	��������������

	�ȉ��͂Ă��Ɓ[�ȏ��ł�

	�ERecTask��BonDriver��K�؂ɔz�u
	�Effmpeg����VLC���C���X�g�[��
	�i��ffmpeg���g���ꍇ�͓�����libx264-ipod640.ffpreset��ffmpeg���C���X�g�[�������Ƃ����presets�t�H���_�ɃR�s�[�j
	�E������HLS_option�`.txt��TvRemoteViewer_VB.exe�Ɠ����t�H���_�ɃR�s�[
	�EWWWROOT�iWEB���[�g�t�H���_�j�ƂȂ�t�H���_�ɓ�����HTML�t�H���_���̃t�@�C�����R�s�[
	�i��VLC�g�p�̏ꍇ�͔��p�X�y�[�X������Ȃ��ꏊ�̂ق������S�ł��j
	�E�t�@�C���Đ������邽�߂ɂ́A���炩���ߓ�����TvRemoteViewer_VB.ini��ҏW���ē��悪����t�H���_���w�肵�Ă����K�v������܂��B
	�E�n�f�W�ԑg�\�̐ݒ�	TvRemoteViewer_VB.ini��ҏW���Ă�������
	�ETvRock�ԑg�\�ɂ��܂���
		1.�u���E�U��
		�@http://[TvRock���ғ����Ă���PC�̃��[�J��IP]:[TvRock�̃|�[�g]/[���[�U�[��]/iphone?md=2&d=0
		�@�i��@http://127.0.0.1:8969/���[�U�[��/iphone?md=2&d=0�j
		�@�ɃA�N�Z�X����B
		�@����ƁAiPhone�p�ԑg�\���\������܂��B
		2. �u�\��\���v����3�ȏ�ɂ���B������I�����܂��Ǝ��ԑg���\������܂���
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
		����
	�EptTimer�ԑg�\�ɂ��܂���
		sqlite3.exe��TvRemoteViewer_VB.exe�Ɠ����t�H���_�ɐݒu���Ă�������
		sqlite3.exe�̓O�O��΂���������Ǝv���܂�



��HLS�A�v���ɂ��܂���

	�Effmpeg�����g�p�̏ꍇ�y�����z
	http://ffmpeg.zeranoe.com/builds/
	ffmpeg�C���X�g�[�����presets�t�H���_���ɓ�����libx264-ipod640.ffpreset���R�s�[���Ă��������B
	�Q�l�Fhttp://frmmpgit.blog.fc2.com/blog-entry-179.html


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
	%VIDEOFILE%	"�r�f�I�t�@�C��"�ɕϊ��i���ۂ́u-i %VIDEOFILE%�v�̌��ߑł���-i�̌��̕����񂪃t�@�C�����ɕϊ��j



��HLS�A�v���̌ʎw��ɂ����܂���

	���w����@
	�EHLS_option*.txt�ւ̋L�q�i�C���f�b�N�X�܂���HLS�I�v�V�����{���j
	�EStartTv.html��hlsAppSelect�����ɂ��HLS�A�v�������w��(hlsAppSelect=VLC,V,ffmpeg,F,QSVEnc,Q,QSV,NVEnc,N,NSV,PiprRun,P)

	���ǂ�HLS�I�v�V�������g�p�����̂��H
	�E�����I��HLS�A�v�����ʎw�肳�ꂽ�ꍇ�́A�e�A�v���ɑΉ�����HLS_option_[HLS�A�v��]*.txt����HLS�I�v�V�������D��I�Ɏg�p����܂�
	�@v1.90�`�@���C�u�������𑜓x�C���f�b�N�X����HLS�A�v�����w�肳��Ă���ꍇ�́��̑O��HLS_option.txt�����������܂�
	�EHLS�A�v�����w�肳��Ă��Ȃ����t�@�C���Đ��̏ꍇ��HLS_option_[HLS�A�v��]_file.txt���D��I�Ɏg�p����܂�
	�EHLS�A�v�����w�肳��Ă��Ȃ����𑜓x�w�肪�����HLS_option.txt����HLS�I�v�V�������g�p����܂�
	�EHLS�A�v�����w�肳��Ă��Ȃ����𑜓x�w�肪�����ꍇ�̓t�H�[�����HLS�I�v�V�������g�p����܂��i�� �t�H�[�����Start�{�^���j
	�Ev1.97�`�@�t�H�[����́uHLS�I�v�V����or�𑜓x�𑗂�v�I���̒l�ɂ���āA�𑜓x�w��̖����ꍇ�Ƀt�H�[�����HLS�I�v�V�����̒l��D�悷�邩�AHLS�A�v���ɉ�����HLS_option�`.txt���̒l��D�悷�邩�I�ׂ�悤�ɂȂ�܂���

	��HLS�A�v���̌ʎw����@
	�EHLS�A�v���w�蕶����i�啶���E�������ǂ���ł�OK�j
	VLC: VLC, V
	ffmpeg: ffmpeg, F
	QSVEnc: QSVEnc, QSVEncC, Q, QSV 
	NVEnc: NVEnc, NVEncC, N, NV 
	PipeRun: PipeRun, P

	��HLS_option*.txt�̋L�q
	�ȉ��͂ǂ��exepath_QSVEnc�Ŏw�肳�ꂽQSVEncC.exe���g�p����܂�
	�EStartTv.html?hlsAppSelect=QSVEnc
	�EStartTv.html?hlsAppSelect=Q
	�E[QSVEnc_640x360]�`
	�E[Q_640x360]�`
	�E[640x360_QSVEnc]�`
	�E[640x360_Q]�`
	�E[(QSVEnc)640x360]�`
	�E[(Q)640x360]�`
	�E[640x360(QSVEnc)]�`
	�E[640x360(Q)]�`
	�E[640x360]QSVEnc_�`
	�E[640x360]Q_�`
	�E[640x360](QSVEnc)�`
	�E[640x360](Q)�`

	��HLS�I�v�V�����t�@�C������̑I�΂��
	�Ⴆ�� F_640x360 �Ǝw�肳�ꂽ�ꍇ�A
	HLS_option_ffmpeg.txt
	����A�͂��߂�[F_640x360]�Ƃ������o����T���A������Ȃ��ꍇ[640x360]��T���܂�



��WEB�C���^�[�t�F�[�X

	�N���C�A���g�J���Ҍ������
	readme_dev.txt�Q�Ƃ̂���



���e�X�g��

	Windows7 x64
	VisualStudio2010

	RecTask�C���X�R	=	D:\TvRemoteViewer\TVTest
	BonDriver	=	D:\TvRemoteViewer\TVTest
	%WWWROOT%	= 	D:\TvRemoteViewer\html
	%FILEROOT%	=	D:\TvRemoteViewer\html
	%HLSROOT%	=	D:\TvRemoteViewer\ffmpeg\bin
	%HLSROOT/../%	=	D:\TvRemoteViewer\ffmpeg
	(%HLSROOT%)	=	D:\TvRemoteViewer\vlc

	iPad(��3����jiOS7 safari�AAndroid(Nexus7��)



��ptTimer�ɂ�1�X�g���[�������g�p�ł��Ȃ����Ƃւ̑΍�
	1. BonDriver_ptmr.dll��K���Ȗ��O��4�R�s�[���܂�
	2. ���ꂼ���.ch2�t�@�C����BonDriver_ptmr.ch2����R�s�[���Ĉȉ��̒ʂ�ҏW���܂�
	   ch2�t�@�C������4�̃`�����l����ԃu���b�N������܂����A1�Ɍ��肷��悤�ɂ��܂��B
	   �Ⴆ�΁A1�߂́u;#SPACE(1,T0)�v�A2�߂́u;#SPACE(3,T1)�v�A3�߂́u;#SPACE(0,S0)�v�A4�߂́u;#SPACE(2,S1)�v�A
           �Ƃ����ӂ��Ƀ`�����l����Ԃ��P�����ɂ��܂�
	3. �����4�X�g���[���S�Ďg�p���邱�Ƃ��ł��܂�



��ISO�Đ��̂��߂̏���
1.�ŐV�ł��_�E�����[�h���𓀌�TvRemoteViewer_VB.exe���㏑���R�s�[���܂�
2.�umplayer-svn-35935.7z�v�Ƃ����L�[���[�h�Ō����̂����_�E�����[�h��mplayer.exe��TvRemoteViewer_VB.exe�Ɠ����t�H���_�ɃR�s�[���܂�
�@�i�V����������͓̂��{��t�@�C�����Ńo�O�����邻���ł��Bmplayer-svn-35935.7z�������Ă��������������肪�Ƃ��������܂��j
3.VLC-2.1.2���_�E�����[�h���ēK���ȃt�H���_�ɉ𓀂��܂��i���܂Ŏg�p���Ă������̂Ƃ͕ʃt�H���_�j
�@http://download.videolan.org/pub/videolan/vlc/2.1.2/
4.TvRemoteViewer_VB.ini��exepath_ISO_VLC=(����vlc.exe�ւ̃p�X)�̋L�q��ǉ������OK�ł�


��EDCB��UDP���M����M����ɂ�
�܂��AEDCB�ɂ�UDP���M�̐ݒ���s���Ă�������
����BonDriver_UDP.dll�Ɠ����ꏊ��BonDriver_UDP.ch2���쐬���Ă��������B���e�͗Ⴆ��
;����������=============
; TVTest �`�����l���ݒ�t�@�C��
; ����,�`���[�j���O���,�`�����l��,�����R���ԍ�,�T�[�r�X,�T�[�r�XID,�l�b�g���[�NID,TSID,���
;#SPACE(0,�l�b�g���[�N)
UDP1234,0,0,0,0,48834,0,0,1
UDP1235,0,1,0,0,48835,0,0,1
;�������܂�=============
�̂悤�ɂȂ�܂��B���e�͊m���ł͂���܂���
���TvRemoteFiles�̊Ǘ��^�u����蓮�z�M�Ŏ����ł��܂�
�Ȃ��ABonDriver_RecTask��BonDriver_TSTask�ł̔z�M�͎������Ă��܂���



���C��������ǉ������肵�ė~�����Ƃ���

	�E�S�ʓI�ɃN���X�Ƃ������̂��킩���ĂȂ��E�E���`�܂�����
	�Effmpeg�̉��ւȏI��



��AbemaTV�ԑg�f�[�^�ɂ��܂���
	���݁AAbemaGraph����̂����͂����������Ă���܂��B���̏�����؂肵�Č��\���グ�܂�



���Ɛ�
	��҂͈�؂̐ӔC�𕉂��܂���B��΂ɁI���ȐӔC�Ŏg�p�ł�����݂̂��g����������



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
	1.74	�G���R�ς݃t�@�C�����������ɍċN�����ɃX�g���[�����A������悤�ɂ���
	1.75	�t�@�C���Đ����A�����G���R�[�h���I���������_�ō쐬����悤�ɂ���
	1.76	���ݒ莞�̏���N�����ɗ�O�G���[���o��o�O���C��
	1.77	�n�[�h�T�u�Đ����A�ăV�[�N����NicoJK���O�ϊ������Ă������ʂ��C��
		PC�N���C�A���g�̃t�@�C���Đ��ɂ����čăV�[�N���ꂽ�Ƃ��ɃG���[���o�Ă����o�O���C��
		http�z�M���AWatchTV%NUM%.html��GET�ŃA�N�Z�X���邱�ƂŒ��ڔz�M�J�n�ł���悤�ɂ���
		WI_STOP_STREAM.html?num=-3��ǉ��i�S��~�������G���R�ς݃t�@�C���͍폜���Ȃ��j
	1.78	ini��OPENFIX�̃_�~�[�`�����l���؂�ւ����ɓ����OPENFIX_WAIT��ǉ�
		ini��UDP2HLS_WAIT��0�̏ꍇ��UDP�A�v���������o������HLS�A�v�������s���邱�Ƃɂ���
		�z�M�������ɕ\�������ts�����C���im3u8���쐬����Ă��Ȃ��Ƃ��J�E���g����悤�ɂ����j
	1.79	ptTimer��Tvmaid�̎��ԑg�擾�s����C��
	1.80	Tvmaid YUI�ɑΉ��iini��TvmaidYUI_url��ǉ��j
	1.81	TV�z�M���߂��󂯎�����Ƃ��Ɋ��ɔz�M���Ȃ�ΊY���X�g���[����\������悤�ɂ���
		�����X�g���[���ɒZ���Ԃŕ����̔z�M���߂𑗂����ꍇ�ɋN����s��Ɏb��I�Ώ�
	1.82	TVROCK�ԑg�\�Ń`���[�i�[���w��ł���悤�ɂ���(ini��TvProgram_tvrock_url���g���j
	1.83	num�w�肪����Ă��Ȃ��ꍇ�̑S�z�M��~�����삵�Ă��Ȃ������o�O���C��
		1.81�̏d��������s�\���������̂ŏC������
		WI_GET_LIVE_STREAM.html�ŁA�z�M�������̂��̂��L�ڂ���悤�ɂ���
	1.84	�ő�z�M����傫�������Ƃ���WI_GET_LIVE_STREAM�������ɒx���Ȃ��Ă����s����C��
		NicoJK�R�����g�t�H���_��ass�t�@�C�������݂���΂�������g�p����悤�ɂ���
		ini�ɍ쐬����ass��NicoJK�t�H���_\jk�`�ɃR�s�[����I�v�V������ǉ�(NicoConvAss_copy2NicoJK)
		1���ȓ��̓���ŃR�����g���Đ����悤�Ƃ���Ɨ�O�G���[���N�����Ă����o�O���C��
	1.85	1.81�̓��ǂ̏ꍇ�͔z�M���̃X�g���[���ɗU������@�\���ēǂݍ��݂��l�����Ă��Ȃ������̂ō폜
	1.86	RecTask���z�M���s�����ꍇ�̏I����������������
		1.83�̏d������̕���p��RecTask�G���[���Ƀ_�~�[�̏������X�g���[�����c���Ă��܂��o�O���C��
	1.87	ini�ɃT���l�C���쐬�pffmpeg���w��ł���悤�ɂ���I�v�V������ǉ��ithumbnail_ffmpeg�j
	1.88	QSVEncC�ɑΉ��i�����i�K�j
		ini��QSVEncC�𖼑O�w��ŏI��������I�v�V������ǉ�(Stop_QSVEnc_at_StartEnd)
		ini�Ƀt�@�C���Đ���ffmpeg(thumbnail_ffmpeg)���g�p����I�v�V������ǉ�(video_force_ffmpeg)
		HLS�A�v����HLS_option.txt�̓��e����v���Ȃ��ꍇ�A�ēǍ��𑣂��_�C�A���O��\��
		�t�H�[�����HLS_option*.txt�Q���ēǂݍ��݂���{�^����ݒu
		VideoPath�Ɏw�肳�ꂽ�t�H���_���ł̃T�u�t�H���_�쐬�E�폜�������F������悤�ɂ���
		ffmpeg��HLS�I�v�V�����t�@�C������libvo_aacenc�̋L�q��aac�ɕύX
		HLS_option_QSVEnc.txt��HLS_option_QSVEnc_file.txt��ǉ��i�R�s�[���Ă��������j
	1.89	HLS�A�v���̌ʎw��ɑΉ��iHLS_option*.txt���̋L�q��StartTv.html�����B��q�j
		StartTv.html�ւ̈���hlsAppSelect��ǉ��ireadme_dev.txt�Q�Ɓj
		ini�Ɍʎw��p��exepath_VLC�Aexepath_ffmpeg�Aexepath_QSCEnc��ǉ�
		thumbnail_ffmpeg��exepath_ffmpeg�ɖ��O�ύX�i���̂܂܂ł��g�p�j
	1.90	�𑜓x�C���f�b�N�X����HLS�A�v�����w�肳��Ă���ꍇ��HLS_option.txt����D��I�Ɍ�������悤�ɂ���
	1.91	HLS�A�v���ʑI���ɔ����ׂ����C��
		ini��BS1_hlsApp��exepath_VLC�ɓ����i���̂܂܂ł��g�p�j
		�^�X�N�g���C�A�C�R���ɃJ�[�\�������킹��ƃo�[�W������\������悤�ɂ���
		�ʎw���PipeRun��ǉ��iPipe�o�RQSVEnc�jreadme�Q��
		Pipe�o�RQSVEnc�t�@�C���Đ��ɑΉ��i������PipeRun.exe��TvRemoteViewer.exe�Ɠ����t�H���_�ɃR�s�[���Ă��������j
		Pipe�o�R�Đ��̂��߂ɂ́Aini��video_force_ffmpeg=2,exepath_ffmpeg, exepath_QSVEnc���w�肵�Ă�������
		video_force_ffmpeg�̒l��3��ǉ��i2�ɉ����Đ��t�@�C����ts�ȊO�̏ꍇ��ffmpeg�ōĐ�����j
	1.92	PipeRun.exe���o�R���Ȃ��Ƃ��p�C�v�������ł���悤�ɂ���(PiprRun.exe�͍폜����OK�ł��j
		�R�����g�t�@�C���T�����኱�C��
	1.93	�W��HLS�A�v����ffmpeg�ȊO�̏ꍇ�Avideo_force_ffmpeg���w�肵�Ă��n�[�h�T�u���L���ɂȂ�Ȃ������o�O���C��
		�W��HLS�A�v����ffmpeg�ȊO�̏ꍇ�Aexepath_ffmpeg���w�肳��Ă��Ă�ffprobe���g�p�ł��Ȃ������o�O���C��
		QSVEnc2.34�̍����V�[�N�I�v�V�����u--seek�v�ɑΉ�
		ini��video_force_ffmpeg��4��ǉ��its�ȊO�E�{���E�n�[�h�T�u�̏ꍇffmpeg���g�p�j
	1.94	�t�@�C���Đ���PipeRun��I�������ꍇ�A��--trim�I�v�V�������c���Ă��܂��Ă����o�O���C��
		�n�[�h�T�u���w�肵���ꍇ�A�R�����g�t�@�C���������Ƃ�ffmpeg���I������Ă��܂��Ă����s����C��
		�ׂ����o�O�t�B�b�N�X
		NicoJK���g�p���Ă��Ȃ����Ńt�@�C���Đ��o���Ȃ������o�O���C��
	1.95	HLS�I�v�V�����I���ے��𐮗����Č�������
		�t�@�C���Đ���ʎw�莞�ɉ𑜓x�w�肪�����ꍇ�̓t�H�[����̉𑜓x�I��l���g�p����悤�ɂ���
		�t�H�[���ォ��̎����p��HLS�I�v�V�����𑗂邩�𑜓x�C���f�b�N�X�𑗂邩�̃R���{�{�b�N�X��ݒu
		ini��PipeRun����ffmpeg�ɓn���I�v�V�������w��ł���悤�ɂ����iPipeRun_ffmpeg_option�j
	1.96	�𑜓x�w�肪���������ꍇ�ł��z�M�����X�g�ɉ𑜓x���L�^����悤�ɂ���
		�z�M���X�g�ɂ͏����ȉ𑜓x���L�^����悤�ɂ����i640x360���j
	1.97	�𑜓x�R���{�{�b�N�X�ύX���̃o�O���C��
		�𑜓x�w�肪���������ꍇ�A�t�H�[�����(�𑜓xorHLS�I�v�V�����j�𑗂�I���ɏ]���悤�ɂ���
	1.98	�𑜓x�𑗂邪�w�肳��HLS_option.txt��HLS�I�v�V�����L�q�������ꍇ�̍ēǂݍ��ݎ��s�ɑΏ�
		0x0noenc�I������ffmpeg�̃I�v�V�������ƔF������Ȃ������o�O���C��
	1.99	���C�u�Đ��ŉ𑜓x�C���f�b�N�X�D��w�肪�L���ɂȂ��Ă��Ȃ������o�O���C��
	2.00	ini��video_force_ffmpeg��p�~���t�H�[����Ɉړ�����
		�v���t�@�C���ɂ��Đ��؂�ւ��ɑΉ�
		�i�t�H�[����Łu9 �v���t�@�C��(profile.txt)�ɏ]���čĐ��v���w�肵���ꍇ�j
		profile.txt��ǉ�
		StartTv.html�ւ̃p�����[�^�Ƃ���profile=��ǉ�
		WI_GET_PROFILES.html��ǉ�
		�t�H�[����Ƀv���t�@�C���֌W�̃R���{�{�b�N�X�ƃ{�^����ǉ�
	2.01	VLC�̃t�@�C���Đ��ɑΉ��i�����ؑւƏč��ɂ͖��Ή��j
	2.02	HLS�A�v������������ɂ���
		�v���t�@�C���L�q�Ɂu-�v���w��ȊO��ǉ�
	2.03	�v���t�@�C���L�q�Ɂu_�v���w���ǉ�
	2.04	BonDriver���̑啶�����������ێ�����悤�ɂ���
	2.05	�v���t�@�C���w��ɂ����Ďg�pHLS�A�v�����肪�t�H�[����̃A�v���ɌŒ肳��Ă����o�O���C��
		Waiting.html�ɔz�M���~�{�^����ݒu
		QSVEnc2.46�̎����č��̃e�X�g���s����悤�ɂ����i�vprofile.txt��2�s�ڂ��R�����g�A�E�g�j
	2.06	ts�̓���X�^�[�g�����̉�͂�TOT�����ɂ�PCR�ŕ␳����悤�ɂ���
	2.07	NVEncC�ɑΉ�
		ini��exepath_NVEnc��Stop_NVEnc_at_StartEnd��ǉ�
	2.08	���΃p�X���w��ł���悤�ɂ���
		�uindex.html���J���v�{�^�����֘A�t�����ꂽ�A�v���ŊJ���悤�ɂ���
	2.09	���΃p�X�̃o�O���C��
	2.10	�t�@�C���I�y���[�V�����̈ꕔ�s����C��
		���C�u�Đ����s���̍ċN���񐔂𐧌�����悤�ɂ����i�W��3��j
		���x������waiting�y�[�W���\�����ꂽ�ꍇ�Ƀ��t���b�V���b����L�΂��悤�ɂ����i�W��10��ȏ��4�b�j
	2.11	LTV����ffmpeg http�z�M���ł��Ȃ��Ȃ��Ă����o�O���C��
		�T�[�o�[�ݒ�擾�iWI_GET_TVRV_STATUS�j�ɂ�����BonDriver�p�X���ԈႦ�Ă����o�O���C���i�e�������j
	2.12	HLS�A�v�����ʂŐ��J��ffmpeg�F�����ł��Ă��Ȃ������o�O���C��
		�@�N������ffmpeg-preset�`�F�b�N���X���[����Ă���
		�@ffmpeg-http�z�M�Ńt�@�C���Đ����ł��Ȃ��Ȃ��Ă���
		�@ts�ȊO�̓���̒����������ʂł��Ȃ��Ȃ��Ă���
	2.13	�t�H�[�����HLS�A�v����ffmpeg�ȊO���w�肳��Ă���ꍇ�ɃT���l�C���쐬���ł��Ȃ������o�O���C��
	2.14	�Z�L�����e�B�ɋC�������Ă݂��i�C���j
	2.15	�X�ɃZ�L�����e�B�ɋC�������Ă݂��i�����j
	2.16	2.15�ׂ̍����o�O�t�B�b�N�X
	2.17	�l�b�g�ォ��񐄏��o�[�W�����������o�[�W�������擾����悤�ɂ���
		WI_GET_VERSION.html��ǉ�
		NVEnc���X�g���[�����A����Ȃ��Ȃ���ts�t�@�C���̏�����ǉ��i�t���Y��j
	2.18	NicoJK�R�����g�Đ����Ajk�R�����g�t�H���_�����݂��Ȃ��Ɨ�O�G���[���N�����Ă����o�O���C��
	2.19	�Z�L�����e�B�����e�i���X
	2.20	�A�b�v�f�[�g�`�F�b�N�̊Ԋu��1���Ԃ���6���ԂɕύX
		WI_GET_VERSION.html�ŕԂ����o�[�W�����ԍ��������_2���ŕ\�L����悤�ɂ���
		�A�b�v�f�[�g�̃`�F�b�N�����邩�ǂ����t�H�[����Ŏw��ł���悤�ɂ���
		�`�F�b�N�������N���C�A���g���ɂ܂��܂��ɂȂ�悤�ɂ���
	2.21	�t�@�C�����ɃV���O���R�[�e�[�V�������܂܂�Ă���ƃt�@�C���Đ��Ɏ��s���Ă����s��ɑΏ�
		���΃p�X�w��ɂ�����1�����t�H���_���󂯕t���Ȃ������o�O���C��
	2.22	ini��VideoExtensions�̊g���q�w��ő啶���������̋�ʂ𖳂�����
		ini��Stop_NVEnc_at_StartEnd���@�\���Ă��Ȃ������o�O���C��
		ISO�t�@�C���Đ��ɑΉ�
		ini��ISO�Đ��Ɏg�p����VLC���w�肷�鍀�ڂ�ǉ��iexepath_ISO_VLC�jVLC-2.1.2����
		ISO�t�@�C���Đ��ɂ͌Â߂�mplayer���K�v�ł��i�vexe���v���O�����t�H���_�ɃR�s�[�j
	2.23	mplayer-ISO.exe�܂���mplayer.exe���v���O�����t�H���_�ɂ���ΔF������悤�ɂ���
	2.24	TTRec�܂��͘^��v���O�������N�����Ă��炸�A�ԑg��񂪎擾�ł��Ȃ����p�Ƀ_�~�[�ԑg�\�@�\��ǉ�
		ini��TvProgram_Force_NoRec��ǉ�
	2.25	ISO�Đ��ɂ����ĉ����������w�肪�@�\���Ă��Ȃ������o�O���C��
	2.26	ISO�Đ��ɂ����鎚���w�莞�̃o�O�����C��
		ISO�Đ����̃`���v�^�[�ɑΉ�
	2.27	ffmpeg��HTTP�z�M���Ƀ`�����l����ύX����΂���قǏd���Ȃ��Ă������ۂ�����
		WebM�`���Ńu���E�U��ł̃X�g���[���z�M�Ɏ����Ή��i��ɕ��������p�j
		HLS_option_ffmpeg_webm.txt��ǉ�
	2.28	2���������̔ړ]�X�V�Ɏ��s���Ă����o�O���C��
	2.29	WebVTT(Nico2HLS)�Ή��𕜊��i�ז��ɂ͂Ȃ�Ȃ��͂��j
	2.30	ISO�Đ��̊m����������
		ISO�����w�莞�̃o�O���C��
	2.31	�ݒ�E�B���h�E�̈ʒu�Ƒ傫�����L������悤�ɂ���
		ini�Ń��O�̕ۑ�����w��ł���悤�ɂ����ilog_path�j
	2.32	ISO�����w�莞�̈��萫����
	2.33	ini�Ɂ~�ōŏ�������I�v�V������ǉ��iclose2min�j
	2.34	�N������ViewTV1.html���X�V����Ă��邩�`�F�b�N���i�ł��ԍ��̑傫��ViewTV�`.html�Ɣ�r�j
		�_�C�A���O�ő���ViewTV.html���X�V���邩�I���ł���悤�ɂ���
		�e�t�@�C���͍ł��ԍ��̑傫��ViewTV�`.html�Ɣ�r����A���̓��e�Ɠ���̂��̂������X�V����܂�
		�i��F�����I��ViewTV5.html�����ꕔ�ύX���Ă����ꍇ�ɂ�ViewTV5.html�͍X�V����܂���j
	2.35	�����R����Ƀ��[�J��IP�ȊO���w��ł���悤�ɂ����iini��Remocon_Domains�ɋL�q�j
	2.36	http�X�g���[��(WatchTV)�A�N�Z�X���̃p�����[�^�iBon_Sid_Ch�������Ă����̂Œǉ��j
		webm�z�M��Ƃ���webm_sample.html��ǉ�
	2.37	WEBAPI��ǉ��iWI_GET_JKNUM�AWI_GET_JKVALUE�j
		�X�g���[���ԍ��I���i%SELECTNUM%�j�^�O��id��ǉ�
	2.38	WEBAPI��ǉ��iWI_GET_JKCOMMENT�j
		ISO�Đ��pVLC�̒ǉ��I�v�V�������w��ł���悤�ɂ����iVLC_ISO_option.txt��ǉ��j
	2.39	http�z�M���@�̎����iini��HTTPSTREAM_METHOD��ǉ��j
			ini�ɃA�C�h���}�~�C�x���g���w�肷��STOP_IDLEMINUTES_METHOD��ǉ��i2=2.35�ȑO�j
	2.40	ini�ɉ�ʐ��ڕ��@���w�肷��TVRemoteFilesNEW��ǉ��iTVRemoteFiles1.82�ȏ�j
	2.41	�V����ISO�Đ������ɑΉ��iTVRemoteFiles1.82�ȏ�j�e�X�g�i�K
			ini�ɁiISOPlayNEW�AISO_DumpDirPath�AISO_maxDump�AISO_ThumbForceM��ǉ��j
	2.42	�V����ISO�Đ������̃o�O�C��
			�@ffmpeg�Ŏ������w�肳�ꂽ�Ƃ��̃G���[���C��
			�@�T���l�C���쐬�ő傫���w�肪��������Ă����o�O���C��
			�@ini��ISO_ThumbForceM��p�~
	2.43	VLC_ISO_option.txt�̔p�~
			ini��VLC_ISO_option��ǉ�
	2.44	�VISO�Đ��I���w�ߎ���VOB���̃`�F�b�N���s���悤�ɂ���(ISO_maxDump��1�ȏ㐄���j
	2.45	�VISO�Đ��Ɏ��s�����ꍇ�ɂ��΂炭���X�g���[�����g�p�ł��Ȃ��Ȃ�o�O���C��
	2.46	�t�H�[����Ń��O�ɏo�͂��鍀�ڂ�I���ł���悤�ɂ���
			�N���C�A���gIP�����O�ɕ\��
			�I�����̃��O�o�͂ɕ\�����ړK�pTvRemoteViewer_VB_edited.log��ǉ�
			�z�M�J�n����%FILEROOT%�t�H���_�̑��݂��m�F����悤�ɂ���
	2.47	%FILEROOT%�t�H���_�����������ꍇ�ɗ�O�G���[���N����s��ɑΏ�
	2.48	�t�H�[�����QSV,NV���O�o�̓`�F�b�N��ݒu�����i�X�g���[���t�H���_�ɏo�͂���܂��j
	2.49	�VISO�Đ��F�ꕔ���{��ISO�t�@�C����QSV�ōĐ��ł��Ȃ����ւ̑Ώ�
			�VISO�Đ��F�����w�肪�����̎��ɋN�����s���Ă������̂��A�����Ȃ��ŋN�����邽�߂̃��W�b�N�ύX
	2.50	��ISO�Đ��ɂ�����ini�Ŏw�肳�ꂽQSVEncC��NVEncC��x64�������ꍇ�ɃG���[�ɂȂ��Ă����o�O���C��
	2.51	ISO�Đ����\���ǂ����`�F�b�N������������
			ini��mplayer.exe�ւ̃p�X���w��ł���悤�ɂ����i���w��͏]���ʂ�v���O�����t�H���_��T���j
	2.52	ISO�Đ��p�f�o�b�O�@�\�ǉ��i�t�H�[�����Debug�`�F�b�N�{�b�N�X�j
			HLS_option_NVEnc_file.txt���C���i�u-m hls_list_size:�v��0�ɏC���j
	2.53	�����I�ɊO���v���O�����֒񋟂ł���悤WI�ԑg�\�ɃW�������L�����̋@�\�ǉ�
			�N���C�A���g�ɂ��nicovideo.jp�ւ̃A�N�Z�X����ǉ�
			�ԑg�\�f�[�^���L���b�V�����邱�Ƃɂ��y�[�W�\�����x�̌���
	2.54	�ԑg�\�L���b�V���̃o�O���C��
			���ʂɔԑg�\���쐬���邱�Ƃ��������o�O���C��
			ini�ɃL���b�V��������s��Ȃ��悤�ɂ���I�v�V������ǉ��iNoUseProgramCache�j
	2.55	���ԑg�̃W���������񋟂ɂ��čׂ����C��
	2.56	EDCB�̎��ԑg�W���������s���m�������o�O���C��
	2.57	�V�K�C���X�g�[�����ɋN���G���[�ɂȂ��Ă����o�O���C��
	2.58	TVRVLauncher�p�ɒn�f�W�ԑg�\��AbemaTV��ǉ��i�v�Fini��TvProgramD��801��ǉ��j
			AbemaTV�ԑg���擾���I���ł���悤�ɂ����iini��AvemaTV_data_get_method��ǉ��j
			AbemaTV�ԑg���擾����ʎw��ł���悤�ɂ����iini��AbemaTV_CustomURL��ǉ��j
			�ԑg�\�L���b�V����Next���̈Ⴂ���l�����Ă��Ȃ������s����C��
	2.59	EDCB���̔ԑg�����̉��s�������i���X�j
			�ԑg���L���b�V�����v���ʂ�ɂ����Ȃ��������������������̂ŏC��
	2.60	�擾����AbemaTV�ԑg���f�[�^���ԑg�����܂�ł��邩��������`�F�b�N����悤�ɂ���
	2.61	�����Ԗ���AbemaTV�ԑg���擾���s���Ă��Ȃ������o�O���C��
	2.62	Tvmaid MAYA�ɑΉ�
	2.63	ptTimer�̔ԑg���ɃW��������ǉ�
	2.64	AbemaTV�ԑg���T�C�g�������Ă���ꍇ�ɏ��񋟌����؂�ւ��Ȃ������o�O���C��
	2.65	TVRVLauncher����AbemaTV�ԑg���L���b�V�����N���A�ł���悤�ɂ���
			AbemaTV�ԑg���̃`�F�b�N�������������i�r���܂ł������M����ė��Ȃ��ꍇ�ɑΏ��j
	2.66	�����̔z�M��~���߂������ɏ������ꂽ�ꍇ�ɋN�����Ă����s����C��
			TVRemoteFilesNEW=1�g�p����HTTP�z�M�ւ̉������]���ƈقȂ��Ă����s����C��
			ini�t�@�C����̐ݒ���t�H�[���ォ�瓮�I�ɕύX�ł���悤�ɂ���
			ini�t�@�C���̕W���Y�t��p�~���㏑���Őݒ�𖳂����Ă��܂����̂�h�~����悤�ɂ���
			TvRemoteViwer_VB.ini.data��TvRemoteViwer_VB.ini.default��Y�t
	2.67	ini��close2min�̑I������ǉ�(2=�~�ōŏ�����Alt+Tab�ɔ�\��)
	2.68	�l�b�g�����ǖ��ϊ��w�肪����Ă��Ȃ��ꍇ�̐����@�\��n�f�W�ւ̕ϊ�����ɂ���
			�l�b�g�����ǂ�AbemaTV(801)�P�ƂŎw�肷���AbemaTV�ԑg�\���\������Ȃ��o�O���C��
	2.69	ffmpeg�X�g���[���z�M���ɃX�g���[���ԍ��̃`�F�b�N���X���[���Ă����o�O���C��
			TvRemoteViewer_VB.ini.data�̃R�s�[���Y�ꎞ��ini�X�V�{�^�����g�p�s�Ƃ���
			�N�����̃��O����G���[�ƌx���𒊏o����ini���ڏڍח��ɕ\�����Ē��ӊ��N����悤�ɂ���
	2.70	ini�t�@�C���ւ̐V�K���ڒǉ��ƒl�̃Z�b�g�������ɏo���Ȃ������o�O���C��
			ini�t�@�C���ւ̐V�K���ڒǉ��Ő������ɉ��s�������Ă���ƃR�����g�A�E�g����Ȃ������o�O���C��
			TvRock�ԑg�\�ւ̃W�������ǉ��i�W���F�ȊO�ɃJ�X�^�}�C�Y���Ă���Ȃ��ini��TvRock_genre_color��ǉ��j
	2.71	Tvmaid�ԑg�\�ɂ����Ď��ԑg�̎擾���o���Ȃ��ꍇ���������o�O���C��
			TvRock�ԑg�\�ŗ\�񂪓����Ă���Ɣԑg����������Ă����o�O���C��
			ini�Ɏ��̎��̔ԑg��ԑg�\�ɕ\������I�v�V������ǉ��inext2_minutes�j
	2.72	TvRock�ԑg�\�W�������̐��x�A�b�v���\�񒆔ԑg�̃W�������擾���\�ɂȂ���
			TvRock�W�������F�͎g�p���Ȃ��Ȃ����̂�ini��TvRock_genre_color��p�~
			ini��TvRock�W�������擾���s�������w��ł���悤�ɂ����B�W���͂��Ȃ��iTvRock_genre_ON�j
	2.73	TvRock�ԑg�\�W��������0���܂����Ŕ��ʂł��Ȃ��̂�2.71�ɖ߂��Đ��x�A�b�v�������s����
			��Ƀo�[�W�����`�F�b�N�ŗ�O�G���[���N����o�O���C��
	2.74	TvRock�W�������擾��ԑg�\���������ʂ̗�������擾����悤�ɂ���
			�iini��TvRock_genre_color��TvRock_genre_ON�����߂Đݒu���W�������擾��W���Ƃ����j
			TvRock�ԑg�\�ŗ\�񂪓����Ă���Ɣԑg����������Ă��܂��o�O���܂������Ă��Ȃ������̂ŏC��
			TVRVLauncher����̃L���b�V���N���A�v���ɑ΂��đS�Ă̔ԑg�����N���A����悤�ɂ���
	2.75	TvRemoteViewer_VB.ini.data��MIME_TYPE_DEFAULT�̐��������Ԉ���Ă����̂ŏC��
			TvRemoteViewer_VB.ini.data�ɍ���MIME_TYPE���L�ڂ���Ă��Ȃ������̂Œǉ�
			��L�L�ڃ~�X��ini�ɋL�ڂ���Ă���ꍇ�́uini �X�V&�K�p�v�{�^���������ꂽ�ۂɏC������悤�ɂ���
	2.76	�ԑg�\���̎����\���𓝈ꂵ��(H:mm�j
			ini�ݒ��ʃ��C�A�E�g��ύX���Č��₷���������������C������
	2.77	ini�ݒ��ʏ�Ńt�@�C����t�H���_���_�C�A���O�őI���ł���悤�ɂ���
	2.78	TvRock�ԑg�\�̎擾�Ɏ��s���邱�Ƃ��������o�O���C��
	2.79	TOT�␳�Ɏg�p����PCR�̒l���s���R�ȏꍇ�Ƀt�@�C���쐬�����𓮉�J�n�����Ƃ���悤�ɂ���
	2.80	TvRock�ԑg�\�ŉp�����݂̂̔ԑg���ŃG���[���������Ă����o�O���C��
			AbemaGraph���񂩂�ԑg�\���擾����ۂɃ^�C���X�^���v�𑗂�Ȃ��悤�ɂ���(2.80c)
	2.81	�W�����OBonDriver������BonDriver_UDP��BonDriver_TSTask�����O���Ȃ����Ƃɂ���
			��L�uEDCB��UDP���M����M����ɂ́v���Q�Ƃ̂���
			TvRock�W���������萸�x����i2.81b�j
	2.82	ini��BonDriver_NGword���@�\���Ă��Ȃ������o�O���C��
	2.83	ptTimer�ԑg�\�ɉ��s���܂܂�Ă����o�O���C��
			EDCB�ԑg�\�̓��e���s���S�ȂƂ����������o�O���C��
			TVRVLauncher�p�ɕ����Ǖʔԑg�\��񋟏o����悤�ɂ���
			TvRock�����Ǖʔԑg�\�擾����d:\tvrock.html�Ƃ����f�o�b�O�p�t�@�C�����쐬���Ă��܂��Ă����i�����Ă��������j
			HLS_option�̊e�s���Ɂu;�v���u#�v������Ζ����ɂ���悤�ɂ����i2.83c�j
			�����Ǖʔԑg�\�̗\��`�F�b�N�������ɂ����B����TvRock�i2.83d�j
	2.84	AbemaTV�̕����Ǖʔԑg�\�ɃT���l�C������ǉ�
			TvRock��EDCB�̗\����ɓ���Ǔ��ꎞ�ԂŗL���Ɩ����������ɓo�^����Ă���ꍇ�ɑΏ��i2.84b�j
			TvProgram_tvrock_sch�������ɂȂ��Ă����o�O���C���i2.84b�j
	2.85	�^��t�@�C���ꗗ�擾�ɂ����āA�X�V���ꂽ�t�H���_�̂ݍX�V����悤�ɂ���
			�X�s�[�h�A�b�v�̂��ߘ^��t�@�C���̃T�C�Y�`�F�b�N��p�~�����i2.85b�j
			�^��t�@�C���ꗗ�X�V���ɕ��ёւ���Y��Ă����o�O���C���i2.85c�j
			HLS_option�ǂݍ��݂������ɂ����i2.85d,2.85e�j
			HLS_option�ɂ����āA2.83c�ȍ~�p�����[�^�[�����̉𑜓x�w����󂯕t���Ȃ������s����C���i2.85f�j
	2.86	AbemaTV�̃W�����������ɂĂ��Ɓ[�ɑΉ��iini AbemaTV���擾�� AvemaTV_data_get_method=2�j
			AbemaTV�ԑg�\�����܂��X�V����Ă��邩�`�F�b�N���郋�[�`�����C���i2.86b�j
			TvRock�Ŕԑg�^�C�g�������p�݂̂̏ꍇ�ɏ�肭�W���������肪�ł��Ȃ����Ƃ��������o�O���C���i2.86c�j
			�ݒ�ɂ���Ă�AbemaTV�ԑg�\�擾�Ɏ��s���Ă����o�O���C���i2.86d�j
			Abema�A�j��2�������ԕ\������Ȃ����ۂɑΏ��B����ɔ����D����擾����J�b�p�ɕύX�i2.86e�j
			Windows�̓��t�`�����W���ƈႤ�ꍇ�ɑΏ��i2.86f�j
			EDCB�ԑg���擾��service_type=1�ȊO���܂߂�悤�ɂ����i2.86g�j
			�C���^�[�l�b�g�ԑg�\���\������Ȃ��Ȃ��Ă����s��ɑΉ��i2.86h�j
			SPHD StarDigio�̔ԑg�\�\���ɑΉ��i�ԑg�f�[�^�������ꍇ�͕����ǖ��̂ݕ\������StarDigio_dummy_ON��ini�ɒǉ��j�i2.86i�j
			UDP�A�v����HLS�A�v����CPU�D��x��BelowNormal��AboveNormal��ǉ��i2.86j�j
	2.87	ini��TvmaidYUI_url�Ŗ�����/�������ꍇ�ɋN�����Ă����s����C��
			Tvmaid,ptTimer�ԑg�\��BS-TBS�܂���QVC���\������Ă��Ȃ������s����C��
			�����Ǖʔԑg�\��BS-TBS��QVC�����ʂł��Ă��Ȃ������s����C��
			�ԑg���Ɏ��̎��̔ԑg�W����������t������悤�ɂ����i2.87b�j
			TvRock�ԑg�\�ŃG���^���`�e���̏I�����Ԃ����������Ȃ�s����C���i2.87c�j
			ini�ɍĐ����̃X���[�v��}�~����I�v�V������ǉ��iviewing_NoSleep�j�i2.87d�j
			ini��AbemaTV�ԑg�\�擾�Ԋu�ݒ��ǉ��iAbemaTV_Program_get_interval_min�j�i2.87e�j
			�N�����`�F�b�N��ch2�t�@�C����Shift_JIS�łȂ���΃��O�Ɍx����\������悤�ɂ����i2.87e�j
	2.88	�ݒ��ʏ�ł�ini�K�p���ɕ⏕�v���O���������݂���΍ċN������悤�ɂ���
			ini�K�p����TvRemoteViewer_VB���ċN��������⏕�v���O����TvRemoteViewer_VB_r.exe��Y�t
			ch2�t�@�C���̕����R�[�h��Shift_JIS�łȂ��ꍇ�ɕϊ������݂邩�N�����ɐq�˂�悤�ɂ����i2.88b�j
			ISO�Đ����ɃT���l�C�����\������Ȃ��Ȃ��Ă����o�O���C���i2.88c�j
			udp�A�v����TSTask�̏ꍇ��ch2�����R�[�h�x���_�C�A���O��\�����Ȃ��悤�ɂ����i2.88d�j
			�l�b�g�ԑg�\�\���ɕs�����������\�����C���i2.88e�j
			���ԑg�Ǝ��̎��̔ԑg�̔�����C���i�����Ԃ̔ԑg�x�~�΍�j�i2.88f�j
			ini�Ɋe�r�f�I�t�H���_�Ď��p�o�b�t�@�T�C�Y�w��I�v�V������ǉ��iwatcher_BufferSize�j�i2.88g�j
			�ݒ��ʂ�BonDriver�I�𗓉��ɗD�揇�ʃ{�^����ݒu�����i2.88h�j
			BonDriver�D�揇�ʂ��ċN�����Ȃ��Ɣ��f����Ȃ������o�O���C���i2.88i�j
			TvRock�����Ǖʔԑg�\�ŃG���^���`�e���̕������ԑg���\������Ȃ��s����C���i2.88j�j
			TvRock�ԑg�\ver2���Ȃ�ΐ����ԓ��̗\��ԑg�̃W���������萸�x������i2.88k�j
			2.88k�Ŕ������݂��Ȃ��Ă����s��̏C���i2.88m�j
			TvRock�ԑg�\�W����������̒����ԑΉ������x����i2.88m�j
			TvRock�����Ǖʔԑg�\�̐擪�ԑg�̗\�񐸓x�̌���i2.88n�j
			AbemaTV����e�ʍ팸�̂���zip�Ŏ擾����悤�ɂ����B�i2.88p�j
			ICSharpCode.SharpZipLib.dll��Y�t�Bexe�Ɠ����t�H���_�ɃR�s�[���Ă��������i2.88p�j
			TvRock�W����������̒����i2.88p�j
			�W����������̂��߂�TvRock�ւ̃A�N�Z�X�񐔂��팸�i2.88q�j
	2.89	�����̃R�����g�t�@�C���������ꍇNicoConvAss�ŃR�����g���_�E�����[�h����ass���쐬�ł���悤�ɂ���
			ini�ɃR�����g���_�E�����[�h���邩���w�肷��NicoConvAss_assData_download��ǉ�
			�ݒ��ʂɁuNicoConvAss�ݒ�Z�b�g�v�I�𗓂�ݒu�i2.89b�j
			TvRock�W����������̐��m������i2.89c�j
			ini�ɓ���p�����[�^�[��������L������Ă���ꍇ�̓��O�Ɍx�����L�ڂ���悤�ɂ����i2.89d�j
	2.90	VCEEnc�ɑΉ��iini��exepath_VCEEnc��ǉ��j
			�T�[�o�[PC��Ŏ��s����HLS�A�v���̃v���Z�X���𒲂ׂ�WI_GET_HLS_APP_COUNT��ǉ��i2.90b�j
			%FILEROOT%�̃`�F�b�N���Â������s����C���i2.90c�j
	2.91	�`���v�^�[���ʎ���A�������Ƃ�B����΃`���v�^�[���쐬����悤�ɂ���
			ini��TSID_in_ChSpace��ǉ��i�����j
			�ݒ��ʂ�TvRemoteFiles�ɂ��u�f�[�^�X�V���v�`�F�b�N�{�b�N�X��ݒu�i2.91b�j
			�A�N�Z�X���O�@�\��ǉ��B�^�X�N�g���C�A�C�R���E�N���b�N�ƃt�H�[����Ƀ{�^����ݒu�i2.91b�j
			�A�N�Z�X���O�̃��N�G�X�g���e���ڂ����\������悤�ɂ����i2.91c�j
			�A�N�Z�X���O��UserAgent��\���i2.91d�j
			�N�����ɐݒ��ʂ̑傫�����A�N�Z�X���O�̑傫���ɂȂ��Ă��܂��o�O���C���i2.91d�j
			�`���v�^�[�쐬�̒����i�u����v�����Ȃ��Ȃ�uʼ����v��u�͂��܂��v�������Ȃ������Ƃւ̑Ώ��j�i2.91e�j
			ini��TvRemoteViewer_VB�쐬�`���v�^�[�������̃`���v�^�[���D�悳����chapter_priority��ǉ��i2.91f�j
			ini��chapter�|�C���g�����b���߂邩(-chapter_bufsec)�ɏ����_��ݒ�ł���悤�ɂ����i2.91g�j
			���܂��@�\�Fini��ts���l�[������chapter�t�@�C�������킹�ă��l�[������tsRenameSyncChapter��ǉ��i2.91h�j
			WI_GET_VIDEOFILES2.html�ɋ@�\�ǉ��i2.91i�j
	2.92	WI_GET_VIDEOFILES2.html�ɋ@�\�ǉ�
			2.91i��WI_GET_VIDEOFILES2�Ńt�H���_�쐬�E�폜���ɘ^��t�@�C���ꗗ�ɔ��f����Ȃ������o�O���C��
			WI_GET_VIDEOFILES2�Ńt�H���_���ύX���ɘ^��t�@�C���ꗗ�ɔ��f����Ȃ������o�O���C���i2.92b�j
			WI_GET_TVRV_STATUS��TSID_in_ChSpace���L�ځi2.92c�j
			�I�����TvRemoteViewer.log�ɃA�N�Z�X���O���L�ځi2.92d�j
			�`���v�^�[�쐬��A,B�ǂ��炩�����݂��Ȃ��ꍇ�ɷ�����폜����Ă��܂��o�O�̏C���i2.92e�j
			WI_GET_VIDEOFILES2�̏C���i2.92f�j
			�r�f�I�t�H���_�X�V�̏C���i2.92g�j
			�r�f�I�t�@�C����t�H���_�̍X�V��Ƃ��W�����Ȃ��悤�C���i2.92h�j
			WI_TVRV_STATUS�ŃG���[�ɂȂ�ꍇ������o�O���C���i2.92i�j
			���݂̔ԑg�f�[�^�������ꍇ�ɁA�����̔ԑg�����ݗ��ɕ\������Ă��܂��Ă����o�O���C���i2.92i�j
			�����Ǖʔԑg�\�ɕ����������ԑт�����Ε⊮����悤�ɂ����i2.92j�j
			�����Ǖʔԑg�\���N�G�X�g����TSID���w��ł���悤�ɂ����iEDCB,Tvmaid�j�i2.92j�j
			WI_GET_VIDEOFILES2�ŃJ�����g�t�H���_�̔��肪�ł��Ă��Ȃ������o�O���C���i2.92j�j
			2.92j��TvRock�ԑg�\�Ŏ��Ԃ����ꂽ���̂��\������邱�Ƃ�����o�O���C���i2.92k�j
	2.93	�L���b�V���L�����̐�ǂ݋@�\�������i�y�[�W�\���̍������B�҂�����邱�Ƃ��قڂȂ��Ȃ�܂��j
			�L���b�V���L�����Ƀl�b�g�ԑg�\���ڂ����擾����悤�ɂ���
			�����Ǖʔԑg���̒P���L���b�V���@�\��ǉ�
			TvRock�͐�ǂ݂��Ȃ����Ƃɂ����i���ԑg�̔ԑg�ڍ׃f�[�^���擾�o���Ȃ����߁j�i2.93b�j
			AbemaTV�p�W�������w��ԑg�\(WI_GET_1GENRE_PROGRAM)��ǉ��i2.93c�j
			AbemaTV�p�W�������w��ԑg�\�̃W���������̑��ɑΉ��i2.93d�j
			NicoJK�t�@�C���D�掞�A���R�����g����Ɋ֌W�����p�����^�C�g������̃R�����g������Ă��܂��\�����y���i2.93e�j
			���Ⴂ�ŏC�������i2.92g�r�f�I�t�H���_�X�V�̏C���j�����ɖ߂����i2.93f�j
			�r�f�I�t�H���_�X�V��ƒ��ɃG���[����������ꍇ�����邱�Ƃւ̑Ώ��i2.93f�j
			�J���},���������t�@�C���̓��t�����������Ȃ�o�O���C���i2.93f�j
			�����x�~�`�F�b�N���ɃG���[���L�^����邱�Ƃ��������ӏ����C���i2.93f�j
			�]���ʂ�̒P���t�@�C���ꗗ�擾�v���ɂ�2.92e�ȑO�̋����[�`�����g�p����悤�ɂ����i2.93g�j
			2.92g�ŏC�������^��t�H���_�ǉ����[�`�������ɖ߂����i2.93h�j
			WI_GET_TVRV_STATUS�̍��ږ�TSID_in_ChSpace��t_i_c�ɕύX�i2.93i�j
			�ԑg�\�擾���G���[�ւ̑Ή��i�vFramework4.5.2�j�i2.93j�j
			����vFramework4.5.0�Ɉڍs�i���̂ق����C���X�g�[���ς݂̕����������߁j�i2.93k�j




��ConnectedSelect.js��http://d.hatena.ne.jp/Mars/20071109�̃X�N���v�g���g�p�����Ă��������܂����B
��ch_sid.txt��NicoJKPlayMod��jkch.sh.txt���Q�Ƃ��C�������������̂ł��B��җl���肪�Ƃ��������܂��B
��CtrlCmdCLI.dll��EDCB�ɓY�t����Ă������̂ł��B��җl�y�єh���ł̍�җl���肪�Ƃ��������܂��B
