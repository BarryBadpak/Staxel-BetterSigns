const SignInfo = {
	hide: () => {
		$('#SignInfo_ColorPicker').colorPicker.destroy();
		$('.option-slider.slider-signscale').slider("destroy");
		$('#SignInfo_Container').fadeOut(250, function () {
			stxl.call('onHideSignInfo', '');
		});
	},

	saveData: () => {
		const colorString = $('#SignInfo_ColorPicker').val();
		const rgb = colorString.match(/\d+/g);
		let alignmentX = 0;
		let alignmentY = 0;

		if ($('#SignInfo_AlignmentOptions .icon.active[data-group="0"]').length) {
			alignmentX = parseInt($('#SignInfo_AlignmentOptions .icon.active[data-group="0"]').data('value'));
		}
		if($('#SignInfo_AlignmentOptions .icon.active[data-group="1"]').length) {
			alignmentY = parseInt($('#SignInfo_AlignmentOptions .icon.active[data-group="1"]').data('value'));
		}

		const alignment = alignmentX | alignmentY;
		const obj = {
			message: $('#SignInfo_Message').val(),
			color: {
				x: parseInt(rgb[0]),
				y: parseInt(rgb[1]),
				z: parseInt(rgb[2])
			},
			scale: parseFloat($('#SignInfo_SizeContainer span.data').text()),
			align: alignment
		}

		stxl.call("onChangeSignInfo", JSON.stringify(obj));
	},

	init: (sliderValue) => {
		$('#SignInfo_ColorPicker').colorPicker({
			animationSpeed: 0,
			margin: '4px -2px 0',
			doRender: 'div div',
			opacity: false,
			cssAddon: // could also be in a css file instead
			'.cp-color-picker{padding:5px 5px 0;}' +
			'.cp-xy-slider{width:100px; height:100px; margin-bottom:3px;}' +
			'.cp-z-slider{height:100px; margin-left:4px;}' +
			'.cp-alpha{margin:4px 0;}' +
			'.cp-xy-slider:active {cursor:none;}'
		});

		$('.option-slider.slider-signscale').slider({
			range: 'min',
			value: sliderValue,
			min: 0.5,
			max: 4,
			step: 0.1,
			change: function (event, ui) {
				if (!isNaN(ui.value)) {
					$(this).prev().children('.data').text(ui.value);
				}
			},
			slide: function (event, ui) {
				$(this).prev().children('.data').text(ui.value);
			},
			stop: function (event, ui) {
				$(this).prev().children('.data').text(ui.value);
			}
		});
	},

	removeSVGClass: (targetGroup, classname) => {
		$('#SignInfo_AlignmentOptions .icon[data-group="' + targetGroup + '"]').each(function () {
			$(this)[0].classList.remove(classname);
		});
	},

	addSVGClass: (targetGroup, value, classname) => {
		$('#SignInfo_AlignmentOptions .icon[data-group="' + targetGroup + '"][data-value="'+value+'"]').each(function () {
			$(this)[0].classList.add(classname);
		});
	}
};

$(document).ready(function () {
	$('#SignInfo_Close').click(function () {
		SignInfo.hide();
		stxl.call('playUISound', 'staxel.sounds.ui.Menu.close');
	});

	$('#SignInfo_Save').click(function () {
		SignInfo.saveData();
	});

	$('#SignInfo_AlignmentOptions .icon').click(function () {
		const group = $(this).data('group');
		const value = $(this).data('value');

		SignInfo.removeSVGClass(group, 'active');
		SignInfo.addSVGClass(group, value, 'active');
	});
});

stxl.showSignInfo = (json) => {
	// Parse data
	const data = JSON.parse(json);
	const message = data.message;
	const scale = Math.round(parseFloat(data.scale) * 10) / 10;
	const color = 'rgb(' + data.color.x + ', ' + data.color.y + ', ' + data.color.z + ')';
	const align = parseInt(data.align);

	const alignXValue = (1 & align) + (2 & align) + (4 & align);
	const alignYValue = (8 & align) + (16 & align) + (32 & align);

	SignInfo.removeSVGClass(0, 'active');
	SignInfo.removeSVGClass(1, 'active');

	if (alignXValue > 0)
		SignInfo.addSVGClass(0, alignXValue, 'active');
	else
		SignInfo.addSVGClass(0, 1, 'active');
	if (alignYValue > 0)
		SignInfo.addSVGClass(1, alignYValue, 'active');
	else
		SignInfo.addSVGClass(1, 8, 'active');

	$('#SignInfo_Message').val(message);
	$('#SignInfo_SizeContainer span.data').text(scale);
	$('#SignInfo_ColorPicker').val(color);

	// Init UI
	SignInfo.init(scale);

	$('#SignInfo_Container').fadeIn(250, function () {
		stxl.call('onShowSignInfo', '');
	});
};
stxl.hideSignInfo = SignInfo.hide;