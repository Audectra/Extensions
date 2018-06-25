/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;

using Audectra.GUI;
using Audectra.Graphics;
using Audectra.Graphics.Effects;

/* Your effects need to be in this namesapce. */
namespace Audectra.Extensions.Effects
{
	/*	Implement the EffectBase base class and the IExtension interface. */
    class BeatColor : EffectBase, IExtension
    {
        private IEffectHelper _helper;
        private RgbColor _color;
        private IRgbRender _render;
		
		/* Random number generator to create random colors. */
		private Random _rand;

		/* 	This empty constructor is neccessary for Audectras extension loader engine. */
        public BeatColor() { }

		/*	This constructor will be called when a layer of your effect is being created. */
        public BeatColor(IEffectHelper effectHelper, int height, int width) : base(height, width)
        {
			/*	Save the effect helper in your class, you will need it. */
            _helper = effectHelper;
			
			/*	Set the default color. */
            _color = new RgbColor(0, 0.5f, 0.5f);
			
			/*	Create a render for your effect using the effect helper. */
            _render = _helper.CreateRender();
			
			/* Initialize the random number generator. */
			_rand = new Random();
        }
		
		/*  Create a random color with full saturation and brightness every time
            this method gets called. */
        private RgbColor CreateRandomColor()
        {
            var hueValue = _rand.Next(360);
            var randColor = new HsvColor(hueValue, 1, 1);
            
            /*  The HsvColor will implicitly be converted to RgbColor. */
            return randColor;
        }

		/*	In this method you will be able to render your effect. It will be called for 
			each frame of your project, assuming this layer is enabled. */
        public override IRgbRender Render(float dt)
        {
			/*  Create a new color on each beat. */
            if (_helper.IsBeat())
            {
                _color = CreateRandomColor();
            }
			
			/* 	Map every pixel in the render to the configured color */
            _render.Map((color, x, y) => _color);
            return _render;
        }

		/*	To allow the user to configure your effect to their likings, you will need 
			to specify what exactly is configureable. In this method you will specify
			what controls you request from Audectra for the layer settings side panel
			of your effect. This method generally only gets called once per layer. */
        public override void GenerateSettings(ILayerSettingsPanel settingsPanel)
        {
			/* This effect extension doesn't need any additional settings so far. */
        }

		/*	Every time a configuration option you've secified above has changed, either
			due user interaction in the layer settings or due a feature binding, this 
			method will be called, to inform you on which of your values has changed. */
        public override void ValueChanged(uint valueId, object value)
        {
			
        }

		/*	Return the name of this effect. */
        public string GetName()
        {
            return "Beat Color";
        }

		/*	Return the version of this effect. */
        public string GetVersion()
        {
            return "v1.0.1";
        }

		/*	Return the author of this effect. */
        public string GetAuthor()
        {
            return "Audectra";
        }
    }
}
