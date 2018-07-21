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
    class BeatWave1D : EffectBase, IExtension
    {
        private IEffectHelper _helper;
        private IRgbRender _render;
		private IWaveSimulation1D _waveSimulation;
		
		/* 	This empty constructor is neccessary for Audectras extension loader engine. */
        public BeatWave1D() { }

		/*	This constructor will be called when a layer of your effect is being created. */
        public BeatWave1D(IEffectHelper effectHelper, int height, int width) : base(height, width)
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
        public override void GenerateSettings(ILayerSettingsPanel settingsPanel)
        {
			
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
            return "1D Beat Wave";
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
