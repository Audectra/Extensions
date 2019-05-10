/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;

using SkiaSharp;
using Audectra.Extensions.Sdk.V1;

namespace Audectra.Extensions.Effects
{
    [MinNumberOfPixels(8)]
    [EffectExtension("Sparkles", "Audectra", "1.3.0")]
    class Sparkles : EffectExtensionBase
    {
        private readonly IEffectApi _api;
        private readonly IRender _render;

        private SKColor _color;
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

        public Sparkles(IEffectApi effectApi, int width, int height) : base(width, height)
        {
            _api = effectApi;

            // Create a render for this effect instance once.
            _render = _api.CreateRender();

            // Create a particle system
            _particleSystem = _api.CreateParticleSystem();

            _color = new SKColor(0, 128, 128);
            _numParticles = 3 * Width / 4;
            _particleSize = 1;
        }

        public override IRender Render(float dt)
        {
            _render.Clear();
            _particleSystem.Update(dt);
            _particleSystem.Render(_render);
            
            return _render;
        }

        public override void GenerateSettings(ISettingsBuilder settingsBuilder)
        {
            settingsBuilder.PageBegin();
            settingsBuilder.AddColorGroup(_color, (uint)ValueId.ColorValue);

            settingsBuilder.GroupBegin("Quantity");
            settingsBuilder.AddBindableSlider(_numParticles, 0, Width, (uint)ValueId.ParticleQuantityValue);
            settingsBuilder.GroupEnd();

            settingsBuilder.GroupBegin("Size");
            settingsBuilder.AddSlider(_particleSize, 1, MaxParticleSize, (uint)ValueId.ParticleSizeValue);
            settingsBuilder.GroupEnd();

            settingsBuilder.GroupBegin("Trigger");
            settingsBuilder.AddBindableTrigger((uint)TriggerId.Sparkle);
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
                        var life = (float)(MaxParticleLife * rand.NextDouble());
                        var nextX = (float) (Width * rand.NextDouble());
                        var nextY = (float) (Height * rand.NextDouble());

                        var particleConfig = new ParticleConfig
                        {
                            Angle = 0,
                            Life = life,
                            Speed = 0,
                            Position = new Vector2(nextX, nextY),
                            AxisPosition = new Vector2(0, 0),
                            BeginSize = _particleSize,
                            EndSize = _particleSize,
                            BeginColor = _color,
                            EndColor = _color
                        };

                        _particleSystem.AddParticle(particleConfig);
                    }
                    break;
            }
        }
    }
}
