/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;

using Audectra.Graphics;
using Audectra.Layers;
using Audectra.Layers.Effects;
using Audectra.Layers.Settings;
using Audectra.Layers.Requirements;

/* Your effects need to be in this namesapce. */
namespace Audectra.Extensions.Effects
{
	/*	Implement the EffectBase base class and the IExtension interface. */
    class BeatColor : EffectBase, IExtension
    {
        private IEffectHelper _helper;
        private RgbColor _color;
        private IRgbRender _render;
		
		/* 	This empty constructor is neccessary for Audectras extension loader engine. */
        public BeatColor() { }

		/*	This constructor will be called when a layer of your effect is being created. */
        public BeatColor(IEffectHelper effectHelper, int width, int height) : base(width, height)
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
			/*  Create a new color on each beat. */
            if (_helper.IsBeat())
            {
                _color = _helper.CreateRandomColor();
            }
			
			/* 	Map every pixel in the render to the configured color */
            _render.Map((x, y) => _color);
            return _render;
        }

		/*	Return the name of this effect. */
        public string GetName()
        {
            return "Beat Color";
        }

		/*	Return the version of this effect. */
        public string GetVersion()
        {
            return "v1.2.0";
        }

		/*	Return the author of this effect. */
        public string GetAuthor()
        {
            return "Audectra";
        }
    }
}
