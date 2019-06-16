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
    [MinDimensions(8, 8)]
    [EffectExtension("Fire", "Audectra", "1.3.0")]
    class Fire : EffectExtensionBase
    {
        private readonly IEffectApi _api;
        private readonly IRender _render;

        private SKColor _beginColor;
        private readonly IParticleSystem _particleSystem;
        private readonly List<IParticleEmitter> _particleEmitters;

        private const float EmberDistance = 4;
        private const float MaxParticleSpeed = 20f;
        private const float MaxParticleLife = 10f;
        private const float MaxEmissionRate = 50f;
        private const float MaxParticleSize = 16f;

        private enum SettingId
        {
            BeginColorValue = 0,
            EmissionRateValue,
            ParticleSpeedValue,
            ParticleLifeValue,
            ParticleBeginSizeValue,
            ParticleEndSizeValue
        }

        public Fire(IEffectApi effectApi, int width, int height) : base(width, height)
        {
            _api = effectApi;
            _render = _api.CreateRender();
            _particleSystem = _api.CreateParticleSystem();

            _beginColor = new SKColor(128, 64, 16);
            var endColor = new SKColor(64, 64, 64);
            _particleEmitters = new List<IParticleEmitter>();

            int numEmbers = Math.Max((int) (Width / EmberDistance), 1);

            for (int i = 0; i < numEmbers; i++)
            {
                var xPos = Width / (numEmbers - 1f) * i;
                var yPos = Height;

                var emitterConfig = new ParticleEmitterConfig
                {
                    Position = new Vector2(xPos, yPos),
                    AxisPosition = new Vector2(xPos, yPos),
                    EmissionRate = 8,
                    Angle = new Range<float>(45, 135),
                    Life = 4,
                    Speed = 4,
                    RadialAcceleration = 0,
                    TangentialAcceleration = 0,
                    BeginColor = _beginColor,
                    EndColor = endColor,
                    BeginSize = 4,
                    EndSize = 4,
                    EnableSizeTransition = true,
                    EnableColorTransition = true,
                };

                var emitter = _particleSystem.AddEmitter(emitterConfig);
                _particleEmitters.Add(emitter);
            }
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

            settingsBuilder.GroupBegin("Emission Rate");
            settingsBuilder.AddBindableSlider(_particleEmitters[0].EmissionRate, 0, MaxEmissionRate, (uint)SettingId.EmissionRateValue);
            settingsBuilder.GroupEnd();

            settingsBuilder.GroupBegin("Velocity");
            settingsBuilder.AddSlider(_particleEmitters[0].Speed.Minimum, 0, MaxParticleSpeed, (uint)SettingId.ParticleSpeedValue);
            settingsBuilder.GroupEnd();

            settingsBuilder.GroupBegin("Life");
            settingsBuilder.AddSlider(_particleEmitters[0].Life.Minimum, 0, MaxParticleLife, (uint)SettingId.ParticleLifeValue);
            settingsBuilder.GroupEnd();
            
            settingsBuilder.AddColorGroup(_beginColor, (uint)SettingId.BeginColorValue);

            settingsBuilder.GroupBegin("Begin Size");
            settingsBuilder.AddSlider(_particleEmitters[0].BeginSize.Minimum, 1, MaxParticleSize, (uint)SettingId.ParticleBeginSizeValue);
            settingsBuilder.GroupEnd();

            settingsBuilder.GroupBegin("End Size");
            settingsBuilder.AddSlider(_particleEmitters[0].EndSize.Minimum, 0, MaxParticleSize, (uint)SettingId.ParticleEndSizeValue);
            settingsBuilder.GroupEnd();

            settingsBuilder.PageEnd();
        }

        public override void OnSettingChanged(uint settingId, SettingValue value)
        {
            switch ((SettingId) settingId)
            {
                case SettingId.BeginColorValue:
                    _particleEmitters.ForEach(emitter => 
                        emitter.BeginColor = value);
                    break;

                case SettingId.EmissionRateValue:
                    _particleEmitters.ForEach(emitter => 
                        emitter.EmissionRate = value);
                    break;

                case SettingId.ParticleSpeedValue:
                    _particleEmitters.ForEach(emitter => 
                        emitter.Speed = value.ToSingle());
                    break;

                case SettingId.ParticleLifeValue:
                    _particleEmitters.ForEach(emitter => 
                        emitter.Life = value.ToSingle());
                    break;

                case SettingId.ParticleBeginSizeValue:
                    _particleEmitters.ForEach(emitter => 
                        emitter.BeginSize = value.ToSingle());
                    break;

                case SettingId.ParticleEndSizeValue:
                    _particleEmitters.ForEach(emitter => 
                        emitter.EndSize = value.ToSingle());
                    break;
            }
        }
    }
}
