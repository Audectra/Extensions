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
	[EffectExtension("Beat Color", "Audectra", "1.3.0")]
    class BeatColor : EffectExtensionBase
    {
        private IEffectApi _api;
        private SKColor _color;
        private IRender _render;
        private IAudioFeatureCache _featureCache;

		/*	This constructor will be called when a layer of your effect is being created. */
        public BeatColor(IEffectApi effectApi, int width, int height) : base(width, height)
        {
			/*	Save the effect api in your class, you will need it. */
            _api = effectApi;
            _featureCache = _api.CreateAudioFeatureCache();
			
			/*	Set the default color. */
            _color = new SKColor(0, 128, 128);
			
			/*	Create a render for your effect using the effect api. */
            _render = _api.CreateRender();
        }

		/*	In this method you will be able to render your effect. It will be called for 
			each frame of your project, assuming this layer is enabled. */
        public override IRender Render(float dt)
        {
			/*  Create a new color on each beat. */
            if (_featureCache.IsBeat())
            {
                _color = _api.CreateRandomColor();
            }
			
			/* 	Map every pixel in the render to the configured color */
            using (var canvas = _api.CreateCanvas(_render))
                canvas.Clear(_color);

            return _render;
        }
    }
}
