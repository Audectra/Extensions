/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;

using Audectra.Graphics;
using Audectra.Layers;
using Audectra.Layers.Effects;
using Audectra.Layers.Settings;


/* Your effects need to be in this namesapce. */
namespace Audectra.Extensions.Effects
{
	/*	Implement the EffectBase base class and the IExtension interface. */
    class SimpleColor : EffectBase, IExtension
    {
        private IEffectHelper _helper;
        private RgbColor _color;
        private IRgbRender _render;
		
		/*	Enumeration for each value you want to be configurable in the layer settings. */
		private enum SettingId
		{
			/*	ValueId for the configurable color. */
			ColorValue = 0,
		}

		/* 	This empty constructor is neccessary for Audectras extension loader engine. */
        public SimpleColor() { }

		/*	This constructor will be called when a layer of your effect is being created. */
        public SimpleColor(IEffectHelper effectHelper, int width, int height) : base(width, height)
        {
			/*	Save the effect helper in your class, you will need it. */
            _helper = effectHelper;
			
			/*	Set the default color. */
            _color = new RgbColor(0, 0.5f, 0.5f);
			
			/*	Create a render for your effect using the effect helper. */
            _render = _helper.CreateRender();
        }

		/*	In this method you will be able to render your effect. It will be called for 
			each frame of your project, assuming this layer is enabled. */
        public override IRgbRender Render(float dt)
        {
			/* 	Map every pixel in the render to the configured color */
            _render.Map((x, y) => _color);
            return _render;
        }

		/*	To allow the user to configure your effect to their likings, you will need 
			to specify what exactly is configureable. In this method you will specify
			what controls you request from Audectra for the layer settings side panel
			of your effect. This method generally only gets called once per layer. */
        public override void GenerateSettings(ILayerSettingsBuilder settingsBuilder)
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

		/*	Return the name of this effect. */
        public string GetName()
        {
            return "Simple Color";
        }

		/*	Return the version of this effect. */
        public string GetVersion()
        {
            return "v1.3.0";
        }

		/*	Return the author of this effect. */
        public string GetAuthor()
        {
            return "Audectra";
        }
    }
}
