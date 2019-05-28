/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;

using SkiaSharp;
using Audectra.Extensions.Sdk.V1;

namespace Audectra.Extensions.Effects
{
    [MinWidth(8)]
    [LandscapeAspectRatio]
    [EffectExtension("Bouncing Blob", "Audectra", "v0.0.1")]
    class BouncingBlob : EffectExtensionBase
    {
        private IEffectApi _api;
        private SKColor _color;
        private IRender _render;

        private float _xCenterPos;
        private float _size;
        private float _speed;
        private float _direction;

        private enum SettingId
        {
            Color,
            Size,
            Speed
        }

        private enum TriggerId
        {
            Bounce,
        }

        public BouncingBlob(IEffectApi api, int width, int height) : base(width, height)
        {
            _api = api;
            _color = new SKColor(0, 255, 128);
            _render = _api.CreateRender();

            _xCenterPos = width / 2f;
            _size = width / 4f;
            _speed = width / 8f;
            _direction = 1;
        }

        public override IRender Render(float dt)
        {
            UpdateBlob(dt);

            return RenderBlob();
        }

        private void UpdateBlob(float dt)
        {
            _xCenterPos += _direction * _speed * dt;

            if (_xCenterPos < _size / 2f)
                _direction = 1;
            
            else if (_xCenterPos > Width - _size / 2f)
                _direction = -1;
        }

        private IRender RenderBlob()
        {
            using (var canvas = _render.CreateCanvas())
            {
                canvas.Clear();

                float x = _xCenterPos;
                float y = Height / 2f;

                var paint = new SKPaint
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill,
                    Shader = SKShader.CreateRadialGradient(
                        new SKPoint(x, y),
                        _size, new[] {_color, SKColors.Black},
                        null, SKShaderTileMode.Clamp),
                };

                canvas.DrawOval(x, y, _size, _size, paint);
                paint.Dispose();
            }

            return _render;
        }

        public override void GenerateSettings(ISettingsBuilder settingsBuilder)
        {
            settingsBuilder.PageBegin();
            settingsBuilder.AddColorGroup(_color, (uint)SettingId.Color);

            settingsBuilder.GroupBegin("Size");
            settingsBuilder.AddBindableSlider(_size, 0, Width / 2f, (uint)SettingId.Size);
            settingsBuilder.GroupEnd();

            settingsBuilder.GroupBegin("Speed");
            settingsBuilder.AddBindableSlider(_speed, 0, Width / 2f, (uint)SettingId.Speed);
            settingsBuilder.GroupEnd();

            settingsBuilder.GroupBegin("Bounce");
            settingsBuilder.AddBindableTrigger((uint)TriggerId.Bounce);
            settingsBuilder.GroupEnd();

            settingsBuilder.PageEnd();
        }

        public override void OnSettingChanged(uint settingId, SettingValue value)
        {
            switch ((SettingId) settingId)
            {
                case SettingId.Color:
                    _color = value;
                    break;

                case SettingId.Size:
                    _size = value;
                    break;

                case SettingId.Speed:
                    _speed = value;
                    break;
            }
        }

        public override void OnTrigger(uint triggerId, bool risingEdge)
        {
            if (!risingEdge)
                return;

            if ((TriggerId)triggerId == TriggerId.Bounce)
                _direction = -_direction;
        }
    }
}
