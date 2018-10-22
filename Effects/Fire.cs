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

        private enum ValueId
        {
            BeginColorValue = 0,
            EmissionRateValue,
            ParticleSpeedValue,
            ParticleLifeValue,
            ParticleBeginSizeValue,
            ParticleEndSizeValue
        }

        public Fire() { }

        public Fire(IEffectHelper effectHelper, int height, int width) : base(height, width)
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

        public override void GenerateSettings(ILayerSettingsPanel settingsPanel)
        {
            settingsPanel.GroupBegin("Emission Rate");
            settingsPanel.AddBindableTrackbar(this, _particleEmitters[0].EmissionRate, 0, MaxEmissionRate, (uint)ValueId.EmissionRateValue);
            settingsPanel.GroupEnd();

            settingsPanel.GroupBegin("Velocity");
            settingsPanel.AddTrackbar(this, _particleEmitters[0].MinSpeed, 0, MaxParticleSpeed, (uint)ValueId.ParticleSpeedValue);
            settingsPanel.GroupEnd();

            settingsPanel.GroupBegin("Life");
            settingsPanel.AddTrackbar(this, _particleEmitters[0].MinLife, 0, MaxParticleLife, (uint)ValueId.ParticleLifeValue);
            settingsPanel.GroupEnd();
            
            settingsPanel.AddColorGroup(this, _beginColor, (uint)ValueId.BeginColorValue);

            settingsPanel.GroupBegin("Begin Size");
            settingsPanel.AddTrackbar(this, _particleEmitters[0].BeginConfig.MinSize, 1, MaxParticleSize, (uint)ValueId.ParticleBeginSizeValue);
            settingsPanel.GroupEnd();

            settingsPanel.GroupBegin("End Size");
            settingsPanel.AddTrackbar(this, _particleEmitters[0].EndConfig.MinSize, 0, MaxParticleSize, (uint)ValueId.ParticleEndSizeValue);
            settingsPanel.GroupEnd();
        }

        public override void ValueChanged(uint valueId, object value)
        {
            switch ((ValueId) valueId)
            {
                case ValueId.BeginColorValue:
                    _particleEmitters.ForEach(emitter => 
                        emitter.BeginConfig.SetColor(_helper.ValueToColor(value)));
                    break;

                case ValueId.EmissionRateValue:
                    _particleEmitters.ForEach(emitter => 
                        emitter.SetEmissionRate(_helper.ValueToSingle(value)));
                    break;

                case ValueId.ParticleSpeedValue:
                    _particleEmitters.ForEach(emitter => 
                        emitter.SetSpeed(_helper.ValueToSingle(value)));
                    break;

                case ValueId.ParticleLifeValue:
                    _particleEmitters.ForEach(emitter => 
                        emitter.SetLife(_helper.ValueToSingle(value)));
                    break;

                case ValueId.ParticleBeginSizeValue:
                    _particleEmitters.ForEach(emitter => 
                        emitter.BeginConfig.SetSize(_helper.ValueToSingle(value)));
                    break;

                case ValueId.ParticleEndSizeValue:
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
            return "v1.0.0";
        }

        public string GetAuthor()
        {
            return "Audectra";
        }
    }
}
