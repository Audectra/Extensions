/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;

using SkiaSharp;
using Audectra.Extensions.Sdk.V1;

namespace Audectra.Extensions.Effects
{
    [MinWidth(4)]
    [LandscapeAspectRatio()]
    [EffectExtension("Color Wheel", "Audectra", "1.3.0")]
    class ColorWheel : EffectExtensionBase
    {
        private IEffectApi _api;
        private SKColor _color;
        private IRender _render;

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

        public ColorWheel(IEffectApi effectApi, int width, int height) : base(width, height)
        {
            _api = effectApi;
            _color = new SKColor(0, 128, 128);
            _render = _api.CreateRender();

            _speed = 25;
            _offset = 0;
            _scale = 1;
        }

        public override IRender Render(float dt)
        {
            _offset = (_offset + _speed * dt) % 360;
            _render.Map((x, y) =>
            {
                float hue = ((float) x / Width * 360f * _scale + _offset) % 360;
                return SKColor.FromHsv(hue, 100, 100);
            });

            return _render;
        }

        public override void GenerateSettings(ISettingsBuilder settingsBuilder)
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
    }
}
