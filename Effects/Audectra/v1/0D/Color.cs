/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;

using SkiaSharp;
using Audectra.Extensions.Sdk.V1;

/* Your effects need to be in this namesapce. */
namespace Audectra.Extensions.Effects
{
	[EffectExtension("Color", "Audectra", "v0.0.1")]
    class Color : EffectExtensionBase
    {
        private IEffectApi _api;
        private SKColor _color;
        private IRender _render;
		
		/*	Enumeration for each value you want to be configurable in the layer settings. */
		private enum SettingId
		{
			/*	ValueId for the configurable color. */
			ColorValue = 0,
		}

		/*	This constructor will be called when a layer of your effect is being created. */
        public Color(IEffectApi api, int width, int height) : base(width, height)
        {
			/*	Save the effect helper in your class, you will need it. */
            _api = api;
			
			/*	Set the default color. */
            _color = new SKColor(0, 255, 128);
			
			/*	Create a render for your effect using the effect helper. */
            _render = _api.CreateRender();
        }

		/*	In this method you will be able to render your effect. It will be called for 
			each frame of your project, assuming this layer is enabled. */
        public override IRender Render(float dt)
        {
            using (var canvas = _render.CreateCanvas())
                canvas.Clear(_color);

            return _render;
        }

		/*	To allow the user to configure your effect to their likings, you will need 
			to specify what exactly is configureable. In this method you will specify
			what controls you request from Audectra for the layer settings side panel
			of your effect. This method generally only gets called once per layer. */
        public override void GenerateSettings(ISettingsBuilder settingsBuilder)
        {
            settingsBuilder.PageBegin();
			
            /* 	Add a color group to the layer settings of this effect, such that
				the user is able to choose or bind a color. */
            settingsBuilder.AddColorGroup(_color, (uint) SettingId.ColorValue);
            
            settingsBuilder.PageEnd();
        }

		/*	Every time a configuration option you've secified above has changed, either
			due user interaction in the layer settings or due a feature binding, this 
			method will be called, to inform you on which of your values has changed. */
        public override void OnSettingChanged(uint settingId, SettingValue value)
        {
			/*	The color value has been changed either by the user or a binding. Use the 
				effect helper to convert the value to a color. */
            if ((SettingId)settingId == SettingId.ColorValue)
                _color = value;
        }
    }
}
