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
using Audectra.Mathematics;
using Audectra.Extensions.Sdk.V1;

/* Your effects need to be in this namesapce. */
namespace Audectra.Extensions.Effects
{
	[MinWidthRequirement(4)]
    [LandscapeAspectRatioRequirement()]
	[EffectExtension("1D Beat Wave", "Audectra", "1.3.0")]
    class BeatWave1D : EffectExtensionBase
    {
        private IEffectHelper _helper;
        private IRgbRender _render;
		private IWaveSimulation1D _waveSimulation;
		
		/*	Enumeration for each value you want to be configurable in the layer settings. */
		private enum SettingId
		{
			/*	ValueId for configurable wave speed */
			WaveSpeed = 0,
		}

		/*	This constructor will be called when a layer of your effect is being created. */
        public BeatWave1D(IEffectHelper effectHelper, int width, int height) : base(width, height)
        {
			/*	Save the effect helper in your class, you will need it. */
            _helper = effectHelper;
			
			/*	Create a render for your effect using the effect helper. */
            _render = _helper.CreateRender();
			
			/*	Create a wave simulation using the effect helper. */
			_waveSimulation = _helper.CreateWaveSimulation1D();
			
			/*	Lets double the wave speeds */
			_waveSimulation.Speed = 2.0;
        }

		/*	In this method you will be able to render your effect. It will be called for 
			each frame of your project, assuming this layer is enabled. */
        public override IRgbRender Render(float dt)
        {
			/*	Add a new droplet with random color on each beat. */
			if (_helper.IsBeat())
			{
				var color = _helper.CreateRandomColor();
				_waveSimulation.AddDrop(Width / 2, color);
			}
			
			/* Reset all pixels on the render. */
			_render.Clear();
			
			/* Update the wave simulation. */
			_waveSimulation.Update(dt);
			
			/* Render the wave simulation */
			_waveSimulation.Render(_render);
			
            return _render;
        }
		
		/*	To allow the user to configure your effect to their likings, you will need 
			to specify what exactly is configureable. In this method you will specify
			what controls you request from Audectra for the layer settings side panel
			of your effect. This method generally only gets called once per layer. */
        public override void GenerateSettings(ILayerSettingsBuilder settingsBuilder)
        {
			settingsBuilder.PageBegin();

			/*	Add a bindable trackbar for the wave speed. */
			settingsBuilder.GroupBegin("Speed");
			settingsBuilder.AddBindableSlider((float)_waveSimulation.Speed, 1.0f, 4.0f, (uint) SettingId.WaveSpeed);
			settingsBuilder.GroupEnd();

			settingsBuilder.PageEnd();
        }

		/*	Every time a configuration option you've secified above has changed, either
			due user interaction in the layer settings or due a feature binding, this 
			method will be called, to inform you on which of your values has changed. */
        public override void OnSettingChanged(uint settingId, SettingValue value)
        {
			switch ((SettingId)settingId)
			{
				/*	The wave speed has been changed either by the user or a binding. Use the
					effect helper to convert the value to a single. */
				case SettingId.WaveSpeed:
					_waveSimulation.Speed = value;
					break;
			}
        }
    }
}
