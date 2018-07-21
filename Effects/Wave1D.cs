/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;

using Audectra.GUI;
using Audectra.Graphics;
using Audectra.Graphics.Effects;
using Audectra.Mathematics;

/* Your effects need to be in this namesapce. */
namespace Audectra.Extensions.Effects
{
	/*	Implement the EffectBase base class and the IExtension interface. */
    class Wave1D : EffectBase, IExtension
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
		}
		
		/*	Enumeration for each trigger you want to be configurable in the layer settings. */
		private enum TriggerId
		{
			/*	TriggerId for the add droplet trigger. */
			AddDrop = 0,
		}

		/* 	This empty constructor is neccessary for Audectras extension loader engine. */
        public Wave1D() { }

		/*	This constructor will be called when a layer of your effect is being created. */
        public Wave1D(IEffectHelper effectHelper, int height, int width) : base(height, width)
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
        public override void GenerateSettings(ILayerSettingsPanel settingsPanel)
        {
			/* 	Add a color group to the layer settings of this effect, such that
				the user is able to choose or bind a color. */
            settingsPanel.AddColorGroup(this, _color, (uint) ValueId.ColorValue);
			
			/*	Add a trigger for the AddDrop trigger id to the settings group */
			settingsPanel.AddBindableTrigger(this, (uint) TriggerId.AddDrop);
        }

		/*	Every time a configuration option you've secified above has changed, either
			due user interaction in the layer settings or due a feature binding, this 
			method will be called, to inform you on which of your values has changed. */
        public override void ValueChanged(uint valueId, object value)
        {
			/*	The color value has been changed either by the user or a binding. Use the 
				effect helper to convert the value to a color. */
            if ((ValueId)valueId == ValueId.ColorValue)
                _color = _helper.ValueToColor(value);
        }
		
		/*	Every time a configured trigger is triggered, either manually by the user
			or due to a feature binding, this method will be called.*/
		public override void Trigger(uint triggerId, bool risingEdge)
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

		/*	Return the name of this effect. */
        public string GetName()
        {
            return "1D Wave";
        }

		/*	Return the version of this effect. */
        public string GetVersion()
        {
            return "v1.0.0";
        }

		/*	Return the author of this effect. */
        public string GetAuthor()
        {
            return "Audectra";
        }
    }
}
