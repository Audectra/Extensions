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
    class Beams : EffectBase, IExtension
    {
        private readonly IEffectHelper _helper;
        private readonly IRgbRender _render;

        private RgbColor _color;
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

        public Beams() { }

        public Beams(IEffectHelper effectHelper, int width, int height) : base(width, height)
        {
            _helper = effectHelper;
            _render = _helper.CreateRender();
            _particleSystem = _helper.CreateParticleSystem();

            _color = new RgbColor(0.0f, 0.5f, 0.5f);
            _particleSpeed = 5;
            _particleSize = 4;
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
            settingsBuilder.AddColorGroup(this, _color, (uint)SettingId.ColorValue);

            settingsBuilder.GroupBegin("Speed");
            settingsBuilder.AddBindableSlider(this, _particleSpeed, 0, MaxParticleSpeed, (uint)SettingId.ParticleSpeedValue);
            settingsBuilder.GroupEnd();

            settingsBuilder.GroupBegin("Size");
            settingsBuilder.AddSlider(this, _particleSize, 0, MaxParticleSize, (uint)SettingId.ParticleSizeValue);
            settingsBuilder.GroupEnd();

            settingsBuilder.GroupBegin("Trigger");
            settingsBuilder.AddBindableTrigger(this, (uint)TriggerId.BeamMeUp);
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
                    };

                    particleConfig.SetPosition(0, 0);
                    particleConfig.SetAxisPosition(0, 0);

                    particleConfig.BeginConfig = new ParticleEndpoint(_particleSize, _color);
                    particleConfig.EndConfig = new ParticleEndpoint(_particleSize, _color);

                    _particleSystem.AddParticle(particleConfig);
                    break;
            }
        }

        public string GetName()
        {
            return "Beams";
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
