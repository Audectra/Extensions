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
    [MinWidth(8)]
    [LandscapeAspectRatio()]
    [EffectExtension("Beams", "Audectra", "1.3.0")]
    class Beams : EffectExtensionBase
    {
        private readonly IEffectApi _api;
        private readonly IRender _render;

        private SKColor _color;
        private float _particleSpeed;
        private float _particleSize;
        private readonly IParticleSystem _particleSystem;

        private const float MaxParticleSpeed = 100f;
        private const float MaxParticleSize = 16f;

        private enum SettingId
        {
            ColorValue = 0,
            ParticleSpeedValue,
            ParticleSizeValue
        }

        private enum TriggerId
        {
            BeamMeUp = 0,
        }

        public Beams(IEffectApi effectApi, int width, int height) : base(width, height)
        {
            _api = effectApi;
            _render = _api.CreateRender();
            _particleSystem = _api.CreateParticleSystem();

            _color = new SKColor(0, 128, 128);
            _particleSpeed = 5;
            _particleSize = 4;
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
            settingsBuilder.AddColorGroup(_color, (uint)SettingId.ColorValue);

            settingsBuilder.GroupBegin("Speed");
            settingsBuilder.AddBindableSlider(_particleSpeed, 0, MaxParticleSpeed, (uint)SettingId.ParticleSpeedValue);
            settingsBuilder.GroupEnd();

            settingsBuilder.GroupBegin("Size");
            settingsBuilder.AddSlider(_particleSize, 0, MaxParticleSize, (uint)SettingId.ParticleSizeValue);
            settingsBuilder.GroupEnd();

            settingsBuilder.GroupBegin("Trigger");
            settingsBuilder.AddBindableTrigger((uint)TriggerId.BeamMeUp);
            settingsBuilder.GroupEnd();

            settingsBuilder.PageEnd();
        }

        public override void OnSettingChanged(uint settingId, SettingValue value)
        {
            switch ((SettingId) settingId)
            {
                case SettingId.ColorValue:
                    _color = value;
                    break;

                case SettingId.ParticleSpeedValue:
                    _particleSpeed = value;
                    break;

                case SettingId.ParticleSizeValue:
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
                case TriggerId.BeamMeUp:
                    var particleConfig = new ParticleConfig
                    {
                        Angle = 0,
                        Life = float.MaxValue,
                        Speed = _particleSpeed,
                        Position = new Vector2(0, 0),
                        AxisPosition = new Vector2(0 ,0),
                        BeginSize = _particleSize,
                        EndSize = _particleSize,
                        BeginColor = _color,
                        EndColor = _color
                    };

                    _particleSystem.AddParticle(particleConfig);
                    break;
            }
        }
    }
}
