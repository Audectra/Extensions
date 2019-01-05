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


namespace Audectra.Extensions.Effects
{
    class Fire : EffectBase, IExtension
    {
        private readonly IEffectHelper _helper;
        private readonly IRgbRender _render;

        private RgbColor _beginColor;
        private readonly IParticleSystem _particleSystem;
        private readonly List<ParticleEmitterConfig> _particleEmitters;

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

        public Fire() { }

        public Fire(IEffectHelper effectHelper, int width, int height) : base(width, height)
        {
            _helper = effectHelper;
            _render = _helper.CreateRender();
            _particleSystem = _helper.CreateParticleSystem();

            _beginColor = new RgbColor(0.5f, 0.25f, 0.1f);
            var endColor = new RgbColor(0.25f, 0.25f, 0.25f);
            _particleEmitters = new List<ParticleEmitterConfig>();

            int numEmbers = Math.Max((int) (Width / EmberDistance), 1);

            for (int i = 0; i < numEmbers; i++)
            {
                var xPos = Width / (numEmbers - 1f) * i;
                var yPos = Height;

                var emitConfig = new ParticleEmitterConfig(xPos, yPos, 8);
                var beginConfig = new ParticleEmitterEndpoint();
                var endConfig = new ParticleEmitterEndpoint();

                beginConfig.SetColor(_beginColor);
                beginConfig.SetSize(4);

                endConfig.SetColor(endColor);
                endConfig.SetSize(4);

                emitConfig.SetAngle(45, 135);
                emitConfig.SetLife(4);
                emitConfig.SetSpeed(4);
                emitConfig.SetRadialAccel(0f);
                emitConfig.SetTangentAccel(0f);
                emitConfig.EnableColorShift();
                emitConfig.EnableSizeShift();
                emitConfig.SetEndpoints(beginConfig, endConfig);

                _particleSystem.AddEmitter(emitConfig);
                _particleEmitters.Add(emitConfig);
            }
        }

        public override IRgbRender Render(float dt)
        {
            _render.Clear();
            _particleSystem.Update(dt);
            _particleSystem.Render(_render);
            
            return _render;
        }

        public override void GenerateSettings(ILayerSettingsBuilder settingsBuilder)
        {
            settingsBuilder.PageBegin();

            settingsBuilder.GroupBegin("Emission Rate");
            settingsBuilder.AddBindableSlider(this, _particleEmitters[0].EmissionRate, 0, MaxEmissionRate, (uint)SettingId.EmissionRateValue);
            settingsBuilder.GroupEnd();

            settingsBuilder.GroupBegin("Velocity");
            settingsBuilder.AddSlider(this, _particleEmitters[0].MinSpeed, 0, MaxParticleSpeed, (uint)SettingId.ParticleSpeedValue);
            settingsBuilder.GroupEnd();

            settingsBuilder.GroupBegin("Life");
            settingsBuilder.AddSlider(this, _particleEmitters[0].MinLife, 0, MaxParticleLife, (uint)SettingId.ParticleLifeValue);
            settingsBuilder.GroupEnd();
            
            settingsBuilder.AddColorGroup(this, _beginColor, (uint)SettingId.BeginColorValue);

            settingsBuilder.GroupBegin("Begin Size");
            settingsBuilder.AddSlider(this, _particleEmitters[0].BeginConfig.MinSize, 1, MaxParticleSize, (uint)SettingId.ParticleBeginSizeValue);
            settingsBuilder.GroupEnd();

            settingsBuilder.GroupBegin("End Size");
            settingsBuilder.AddSlider(this, _particleEmitters[0].EndConfig.MinSize, 0, MaxParticleSize, (uint)SettingId.ParticleEndSizeValue);
            settingsBuilder.GroupEnd();

            settingsBuilder.PageEnd();
        }

        public override void OnSettingChanged(uint settingId, object value)
        {
            switch ((SettingId) settingId)
            {
                case SettingId.BeginColorValue:
                    _particleEmitters.ForEach(emitter => 
                        emitter.BeginConfig.SetColor(_helper.ValueToColor(value)));
                    break;

                case SettingId.EmissionRateValue:
                    _particleEmitters.ForEach(emitter => 
                        emitter.SetEmissionRate(_helper.ValueToSingle(value)));
                    break;

                case SettingId.ParticleSpeedValue:
                    _particleEmitters.ForEach(emitter => 
                        emitter.SetSpeed(_helper.ValueToSingle(value)));
                    break;

                case SettingId.ParticleLifeValue:
                    _particleEmitters.ForEach(emitter => 
                        emitter.SetLife(_helper.ValueToSingle(value)));
                    break;

                case SettingId.ParticleBeginSizeValue:
                    _particleEmitters.ForEach(emitter => 
                        emitter.BeginConfig.SetSize(_helper.ValueToSingle(value)));
                    break;

                case SettingId.ParticleEndSizeValue:
                    _particleEmitters.ForEach(emitter => 
                        emitter.EndConfig.SetSize(_helper.ValueToSingle(value)));
                    break;
            }
        }

        public string GetName()
        {
            return "Fire";
        }

        public string GetVersion()
        {
            return "v1.1.0";
        }

        public string GetAuthor()
        {
            return "Audectra";
        }
    }
}
