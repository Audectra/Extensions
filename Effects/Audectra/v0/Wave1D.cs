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
	[MinWidthRequirement(8)]
    [LandscapeAspectRatioRequirement()]
	[EffectExtension("1D Wave", "Audectra", "1.3.0")]
    class Wave1D : EffectExtensionBase
    {
        private IEffectHelper _helper;
        private RgbColor _color;
        private IRgbRender _render;
		private IWaveSimulation1D _waveSimulation;
		
		/*	Enumeration for each value you want to be configurable in the layer settings. */
		private enum ValueId
		{
			/*	ValueId for the configurable color. */
			ColorValue = 0,
			
			/*	ValueId for configurable wave speed */
			WaveSpeed,
		}
		
		/*	Enumeration for each trigger you want to be configurable in the layer settings. */
		private enum TriggerId
		{
			/*	TriggerId for the add droplet trigger. */
			AddDrop = 0,
		}

		/*	This constructor will be called when a layer of your effect is being created. */
        public Wave1D(IEffectHelper effectHelper, int width, int height) : base(width, height)
        {
			/*	Save the effect helper in your class, you will need it. */
            _helper = effectHelper;
			
			/*	Set the default color. */
            _color = new RgbColor(0, 0.5f, 0.5f);
			
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

			/* 	Add a color group to the layer settings of this effect, such that
				the user is able to choose or bind a color. */
            settingsBuilder.AddColorGroup(_color, (uint) ValueId.ColorValue);
			
			/*	Add a bindable trackbar for the wave speed. */
			settingsBuilder.GroupBegin("Speed");
			settingsBuilder.AddBindableSlider((float)_waveSimulation.Speed, 1.0f, 4.0f, (uint) ValueId.WaveSpeed);
			settingsBuilder.GroupEnd();
			
			/*	Add a trigger for the AddDrop trigger id to the settings group */
			settingsBuilder.GroupBegin("Droplet");
			settingsBuilder.AddBindableTrigger((uint) TriggerId.AddDrop);
			settingsBuilder.GroupEnd();

			settingsBuilder.PageEnd();
        }

		/*	Every time a configuration option you've secified above has changed, either
			due user interaction in the layer settings or due a feature binding, this 
			method will be called, to inform you on which of your values has changed. */
        public override void OnSettingChanged(uint valueId, SettingValue value)
        {
			switch ((ValueId)valueId)
			{
				/*	The color value has been changed either by the user or a binding. Use the 
				effect helper to convert the value to a color. */
				case ValueId.ColorValue:
					_color = value;
					break;
				
				/*	The wave speed has been changed either by the user or a binding. Use the
					effect helper to convert the value to a single. */
				case ValueId.WaveSpeed:
					_waveSimulation.Speed = value;
					break;
			}
        }
		
		/*	Every time a configured trigger is triggered, either manually by the user
			or due to a feature binding, this method will be called.*/
		public override void OnTrigger(uint triggerId, bool risingEdge)
		{
			/*	We only want to be triggerd on rising edges. */
			if (!risingEdge)
				return;
			
			/*	We've got triggered! Lets add a droplet to the wave simulation. */
			if ((TriggerId)triggerId == TriggerId.AddDrop)
			{
				_waveSimulation.AddDrop(Width / 2, _color);
			}
		}
    }
}
