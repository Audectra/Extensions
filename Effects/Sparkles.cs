/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;
using System.Linq;
using System.Collections.Generic;

using Audectra.Gui;
using Audectra.Graphics;
using Audectra.Graphics.Effects;
using Audectra.Graphics.Particles;

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

        public Sparkles(IEffectHelper effectHelper, int height, int width) : base(height, width)
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

        public override void GenerateSettings(ILayerSettingsPanel settingsPanel)
        {
            settingsPanel.AddColorGroup(this, _color, (uint)ValueId.ColorValue);

            settingsPanel.GroupBegin("Quantity");
            settingsPanel.AddBindableTrackbar(this, _numParticles, 0, Width, (uint)ValueId.ParticleQuantityValue);
            settingsPanel.GroupEnd();

            settingsPanel.GroupBegin("Size");
            settingsPanel.AddTrackbar(this, _particleSize, 1, MaxParticleSize, (uint)ValueId.ParticleSizeValue);
            settingsPanel.GroupEnd();

            settingsPanel.GroupBegin("Trigger");
            settingsPanel.AddBindableTrigger(this, (uint)TriggerId.Sparkle);
            settingsPanel.GroupEnd();
        }

        public override void ValueChanged(uint valueId, object value)
        {
            switch ((ValueId) valueId)
            {
                case ValueId.ColorValue:
                    _color = _helper.ValueToColor(value);
                    break;

                case ValueId.ParticleQuantityValue:
                    _numParticles = _helper.ValueToSingle(value);
                    break;

                case ValueId.ParticleSizeValue:
                    _particleSize = _helper.ValueToSingle(value);
                    break;
            }
        }

        public override void Trigger(uint triggerId, bool enable)
        {
            if (!enable)
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

                        particleConfig.SetPosition((float)(Width * rand.NextDouble()), 0);
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
            return "v1.0.0";
        }

        public string GetAuthor()
        {
            return "Audectra";
        }
    }
}
