/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;

using Audectra.Graphics;
using Audectra.Layers;
using Audectra.Layers.Effects;
using Audectra.Layers.Settings;
using Audectra.Layers.Requirements;

namespace Audectra.Extensions.Effects
{
    class ColorWheel : EffectBase, IExtension
    {
        private IEffectHelper _helper;
        private RgbColor _color;
        private IRgbRender _render;

        private float _offset;

        private float _speed;
        private float _scale;

        private const float MaxSpeed = 100f;
        private const float MinScale = 0.25f;
        private const float MaxScale = 2;

        private enum SettingId
        {
            Speed,
            Scale,
        }

        public ColorWheel() { }

        public ColorWheel(IEffectHelper effectHelper, int width, int height) : base(width, height)
        {
            _helper = effectHelper;
            _color = new RgbColor(0, 0.5f, 0.5f);
            _render = _helper.CreateRender();

            _speed = 25;
            _offset = 0;
            _scale = 1;
        }

        public override IRgbRender Render(float dt)
        {
            _offset = (_offset + _speed * dt) % 360;
            _render.Map((x, y) =>
            {
                float hue = ((float) x / Width * 360f * _scale + _offset) % 360;
                return new HsvColor(hue, 1, 1);
            });

            return _render;
        }

        public override void GenerateRequirements(ILayerRequirementsBuilder reqBuilder)
        {
            reqBuilder.AddMinimumWidth(4);
            reqBuilder.AddLandscapeAspectRatio();
        }

        public override void GenerateSettings(ILayerSettingsBuilder settingsBuilder)
        {
            settingsBuilder.PageBegin();

            settingsBuilder.GroupBegin("Scale");
            settingsBuilder.AddSlider(_scale, MinScale, MaxScale, (uint)SettingId.Scale);
            settingsBuilder.GroupEnd();

            settingsBuilder.GroupBegin("Speed");
            settingsBuilder.AddBindableSlider(_speed, 0, MaxSpeed, (uint)SettingId.Speed);
            settingsBuilder.GroupEnd();

            settingsBuilder.PageEnd();
        }

        public override void OnSettingChanged(uint settingId, SettingValue value)
        {
            switch ((SettingId) settingId)
            {
                case SettingId.Speed:
                    _speed = value;
                    break;

                case SettingId.Scale:
                    _scale = value;
                    break;
            }
        }

        public string GetName()
        {
            return "Color Wheel";
        }

        public string GetVersion()
        {
            return "v1.3.0";
        }

        public string GetAuthor()
        {
            return "Audectra";
        }
    }
}
