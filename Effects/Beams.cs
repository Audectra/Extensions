/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;
using System.Linq;
using System.Collections.Generic;

using Audectra.GUI;
using Audectra.Graphics;
using Audectra.Graphics.Effects;
using Audectra.Graphics.Particles;

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

        private enum ValueId
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

        public Beams(IEffectHelper effectHelper, int height, int width) : base(height, width)
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

        public override void GenerateSettings(ILayerSettingsPanel settingsPanel)
        {
            settingsPanel.AddColorGroup(this, _color, (uint)ValueId.ColorValue);

            settingsPanel.GroupBegin("Speed");
            settingsPanel.AddBindableTrackbar(this, _particleSpeed, 0, MaxParticleSpeed, (uint)ValueId.ParticleSpeedValue);
            settingsPanel.GroupEnd();

            settingsPanel.GroupBegin("Size");
            settingsPanel.AddTrackbar(this, _particleSize, 0, MaxParticleSize, (uint)ValueId.ParticleSizeValue);
            settingsPanel.GroupEnd();

            settingsPanel.GroupBegin("Trigger");
            settingsPanel.AddBindableTrigger(this, (uint)TriggerId.BeamMeUp);
            settingsPanel.GroupEnd();
        }

        public override void ValueChanged(uint valueId, object value)
        {
            switch ((ValueId) valueId)
            {
                case ValueId.ColorValue:
                    _color = _helper.ValueToColor(value);
                    break;

                case ValueId.ParticleSpeedValue:
                    _particleSpeed = _helper.ValueToSingle(value);
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
            return "v1.0.0";
        }

        public string GetAuthor()
        {
            return "Audectra";
        }
    }
}
