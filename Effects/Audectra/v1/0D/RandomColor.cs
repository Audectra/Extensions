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
	[EffectExtension("Random Color", "Audectra", "v0.0.1")]
    class RandomColor : EffectExtensionBase
    {
        private IEffectApi _api;
        private SKColor _color;
        private IRender _render;

        private enum TriggerId
        {
            ChangeColorTrigger = 0,
        }

		/*	This constructor will be called when a layer of your effect is being created. */
        public RandomColor(IEffectApi api, int width, int height) : base(width, height)
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
			
            /* Add a bindable trigger to change the color to a new random color. */
            settingsBuilder.GroupBegin("Trigger");
            settingsBuilder.AddBindableTrigger((uint)TriggerId.ChangeColorTrigger);
            settingsBuilder.GroupEnd();
            
            settingsBuilder.PageEnd();
        }

        public override void OnTrigger(uint triggerId, bool risingEdge)
        {
            if (!risingEdge)
                return;

            if ((TriggerId)triggerId == TriggerId.ChangeColorTrigger)
                _color = _api.CreateRandomColor();
        }
    }
}
