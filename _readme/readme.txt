TvRemoteViewer_VB v0.09

�`���[�i�[���������s�N�����ăp�p�b�ƃ`�����l����ύX���悤�Ǝv������4��CPU100%�E�E



����


FrameWork4.0



���N��


�ݒu��A�N������ƃ^�X�N�g���C����X�^�[�g���܂��B
�_�u���N���b�N�Œʏ�̑傫���ɂȂ�܂��B
�e�p�����[�^�[��readme.jpg�Q�Ƃ̂���
�v���O���~���O����Ƃ���form1��WindowState��Normal�ɂ��Ă����ƒʏ�̑傫���ŋN�����܂�����֗��ł��傤�B



���ݒu readme.jpg�≺�ɏ������e�X�g�����Q�l�ɂ��Ă�������


�ERecTask��BonDriver��K�؂ɔz�u


�Effmpeg����VLC���C���X�g�[��
�i��ffmpeg���g���ꍇ�͓�����libx264-ipod640.ffpreset��ffmpeg���C���X�g�[�������Ƃ���presets�t�H���_�ɃR�s�[�j


�E������form_status�`.txt��HLS_option�`.txt��TvRemoteViewer_VB.exe�Ɠ����t�H���_�ɃR�s�[


�EWWWROOT�iWEB���[�g�t�H���_�j�ɓ�����HTML�t�H���_���̃t�@�C�����R�s�[
�i��VLC�g�p�̏ꍇ�͔��p�X�y�[�X������Ȃ��ꏊ�̂ق������S�ł��j



��WEB�f�U�C����ύX�������ꍇ


�Eindex.html
index.html����StartTv�i�z�M�J�n�j���Ăяo�����ۂ�WEB���瑗����p�����[�^�[�Ƃ��Ă͍��̂Ƃ���ȉ���z�肵�Ă��܂��B
Web_Start()����ҏW����ΈႤ�����قȂ�WEB�݌v�ɂ��Ή��ł���ł��傤�B
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
%PROCBONLIST%			�z�M���̃X�g���[���i���o�[��BonDriver
%SELECTBONSIDCH%		index.html����BonDriver��ServiceID&ChSpace��I������<SELECT>�Z�b�g���쐬
%SELECTBONSIDCH:[html]%		[html] = <SELECT>��<SELECT>�̊Ԃɂ��܂���html
%VIEWBUTTONS%			�X�g���[����I������<SELECT>���쐬
%VIEWBUTTONS:[html]%		[html] = <SELECT>��<SELECT>�̊Ԃɂ��܂���html
%VIEWBUTTONS:[html1]:[html2]%	[html2] = �����ɕt��������html


�EViewTV[n].html�Ŏg�p�ł���ϐ�
%SELECTCH%			ViewTV.html���Ŕԑg��I������<SELECT>���쐬����
%WIDTH%				�r�f�I�̕�
%HEIGHT%			�r�f�I�̍���



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


�E�uwindows hls �Đ��v�ŃO�O��΂łĂ�����{��y�[�W����Đ��pflash���_�E�����[�h����html��ҏW���Ă������Windows���m3u8���Đ��ł��܂�



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

iPad(��3����jiOS7 safari



���C��������ǉ������肵�ė~�����Ƃ���


�E�S�ʓI��
�N���X�Ƃ������̂��킩���ĂȂ��E�E���`�܂�����


�E�z��O�̃G���[�����B�e�X�g�s��


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



��ConnectedSelect.js��http://d.hatena.ne.jp/Mars/20071109�̃X�N���v�g���g�p�����Ă��������܂����B
��җl���肪�Ƃ��������܂��B
