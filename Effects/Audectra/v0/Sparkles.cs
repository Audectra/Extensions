/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;
using System.Linq;
using System.Collections.Generic;

using Audectra.Graphics;
using Audectra.Graphics.Particles;
using Audectra.Layers;
using Audectra.Layers.Effects;
using Audectra.Layers.Settings;
using Audectra.Layers.Requirements;

namespace Audectra.Extensions.Effects
{
    class Sparkles : EffectBase, IExtension
    {
        private readonly IEffectHelper _helper;
        private readonly IRgbRender _render;

        private RgbColor _color;
        private float _numParticles;
        private float _particleSize;
        private readonly IParticleSystem _particleSystem;

        private const float MaxParticleLife = 0.25f;
        private const float MaxParticleSize = 8f;

        private enum ValueId
        {
            ColorValue = 0,
            ParticleQuantityValue,
            ParticleSizeValue
        }

        private enum TriggerId
        {
            Sparkle = 0,
        }

        public Sparkles() { }

        public Sparkles(IEffectHelper effectHelper, int width, int height) : base(width, height)
        {
            _helper = effectHelper;

            // Create a render for this effect instance once.
            _render = _helper.CreateRender();

            // Create a particle system
            _particleSystem = _helper.CreateParticleSystem();

            _color = new RgbColor(0.0f, 0.5f, 0.5f);
            _numParticles = 3 * Width / 4;
            _particleSize = 1;
        }

        public override IRgbRender Render(float dt)
        {
            _render.Clear();
            _particleSystem.Update(dt);
            _particleSystem.Render(_render);
            
            return _render;
        }

        public override void GenerateRequirements(ILayerRequirementsBuilder reqBuilder)
        {
            reqBuilder.AddMinimumNumberOfPixels(8);
        }

        public override void GenerateSettings(ILayerSettingsBuilder settingsBuilder)
        {
            settingsBuilder.PageBegin();
            settingsBuilder.AddColorGroup(this, _color, (uint)ValueId.ColorValue);

            settingsBuilder.GroupBegin("Quantity");
            settingsBuilder.AddBindableSlider(this, _numParticles, 0, Width, (uint)ValueId.ParticleQuantityValue);
            settingsBuilder.GroupEnd();

            settingsBuilder.GroupBegin("Size");
            settingsBuilder.AddSlider(this, _particleSize, 1, MaxParticleSize, (uint)ValueId.ParticleSizeValue);
            settingsBuilder.GroupEnd();

            settingsBuilder.GroupBegin("Trigger");
            settingsBuilder.AddBindableTrigger(this, (uint)TriggerId.Sparkle);
            settingsBuilder.GroupEnd();

            settingsBuilder.PageEnd();
        }

        public override void OnSettingChanged(uint valueId, SettingValue value)
        {
            switch ((ValueId) valueId)
            {
                case ValueId.ColorValue:
                    _color = value;
                    break;

                case ValueId.ParticleQuantityValue:
                    _numParticles = value;
                    break;

                case ValueId.ParticleSizeValue:
                    _particleSize = value;
                    break;
            }
        }

        public override void OnTrigger(uint triggerId, bool risingEdge)
        {
            if (!risingEdge)
                return;

            switch ((TriggerId) triggerId)
            {
                case TriggerId.Sparkle:
                    var rand = new Random();

                    for (int i = 0; i < _numParticles; i++)
                    {
                        var particleConfig = new ParticleConfig
                        {
                            Angle = 0,
                            Life = (float)(MaxParticleLife * rand.NextDouble()),
                            Speed = 0,
                        };

                        var nextX = (float) (Width * rand.NextDouble());
                        var nextY = (float) (Height * rand.NextDouble());

                        particleConfig.SetPosition(nextX, nextY);
                        particleConfig.SetAxisPosition(0, 0);

                        particleConfig.BeginConfig = new ParticleEndpoint(_particleSize, _color);
                        particleConfig.EndConfig = new ParticleEndpoint(_particleSize, _color);

                        _particleSystem.AddParticle(particleConfig);
                    }
                    break;
            }
        }

        public string GetName()
        {
            return "Sparkles";
        }

        public string GetVersion()
        {
            return "v1.2.0";
        }

        public string GetAuthor()
        {
            return "Audectra";
        }
    }
}
