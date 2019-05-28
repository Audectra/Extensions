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
    [EffectExtension("Sparkle Explosion", "Audectra", "v0.0.1")]
    class SparkleExplosion : EffectExtensionBase
    {
        private readonly IEffectApi _api;
        private readonly IRender _render;

        private SKColor _color;
        private float _numParticles;
        private float _particleSize;
        private Random _random;
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

        public SparkleExplosion(IEffectApi api, int width, int height) : base(width, height)
        {
            _api = api;

            // Create a render for this effect instance once.
            _render = _api.CreateRender();

            // Create a particle system
            _particleSystem = _api.CreateParticleSystem();

            _random = new Random();
            _color = new SKColor(0, 255, 128);
            _numParticles = Width * Height * 0.25f;
            _particleSize = 2;
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
            settingsBuilder.AddBindableSlider(_numParticles, 0, Width * Height * 0.75f, (uint)ValueId.ParticleQuantityValue);
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
                {
                    for (int i = 0; i < _numParticles; i++)
                    {
                        var nextX = (float) (Width * _random.NextDouble());
                        var nextY = (float) (Height * _random.NextDouble());

                        AddSparkle(nextX, nextY);
                    }
                    break;
                }
            }
        }

        private void AddSparkle(float x, float y)
        {
            var endColor = new SKColor(0, 0, 0);
            var particleConfig = new ParticleConfig
            {
                Angle = 0,
                Life = (float)(MaxParticleLife * _random.NextDouble()),
                Speed = 0,
                Position = new Vector2(x, y),
                AxisPosition = new Vector2(0, 0),
                BeginSize = _particleSize,
                EndSize = _particleSize,
                BeginColor = _color,
                EndColor = endColor
            };

            _particleSystem.AddParticle(particleConfig);
        }

        public string GetName()
        {
            return "Sparkle Explosion";
        }

        public string GetVersion()
        {
            return "v0.0.1";
        }

        public string GetAuthor()
        {
            return "Audectra";
        }
    }
}
