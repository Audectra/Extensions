/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;

using SkiaSharp;
using Audectra.Extensions.Sdk.V1;

namespace Audectra.Extensions.Effects
{
    [MinNumberOfPixels(8)]
    [EffectExtension("Directional Plasma", "Audectra", "v0.0.1")]
    class DirectionalPlasma : EffectExtensionBase
    {
        private IEffectApi _api;
        private IRender _render;

        private float _passedTime;
        
        private float _scale;
        private float _angle;

        private float _kx;
        private float _ky;

        private float _cr;
        private float _cg;
        private float _cb;

        private enum SettingId
        {
            RedColorValue = 0,
            GreenColorValue,
            BlueColorValue,
            ScaleValue,
            AngleValue,
        }

        public DirectionalPlasma(IEffectApi api, int width, int height) : base(width, height)
        {
            _api = api;
            _render = _api.CreateRender();

            _cr = 0.0f;
            _cg = 0.5f;
            _cb = 1.0f;

            _scale = 1.0f;
            _angle = 0.0f;

            UpdateDirection();
        }

        private void UpdateDirection()
        {
            _kx = _scale * (float)Math.PI / Width;
            _ky = _kx * (float)Math.Tan(_angle * Math.PI / 180f);
        }

        public override IRender Render(float dt)
        {
            _passedTime = _passedTime + dt;

            _render.Clear();
            _render.Map((x, y) => 
            {
                float v = _kx * x + _ky * y - _passedTime;

                byte r = (byte)((Math.Sin(v + _cr * Math.PI) * 0.5 + 0.5) * 255);
                byte g = (byte)((Math.Sin(v + _cg * Math.PI) * 0.5 + 0.5) * 255);
                byte b = (byte)((Math.Sin(v + _cb * Math.PI) * 0.5 + 0.5) * 255);

                return new SKColor(r, g, b);
            });

            return _render;
        }

        public override void GenerateSettings(ISettingsBuilder settingsBuilder)
        {
            settingsBuilder.PageBegin();

            settingsBuilder.GroupBegin("Color");
            settingsBuilder.AddSlider(_cr, 0f, 1.0f, (uint)SettingId.RedColorValue);
            settingsBuilder.AddSlider(_cg, 0f, 1.0f, (uint)SettingId.GreenColorValue);
            settingsBuilder.AddSlider(_cb, 0f, 1.0f, (uint)SettingId.BlueColorValue);
            settingsBuilder.GroupEnd();

            settingsBuilder.GroupBegin("Scale");
            settingsBuilder.AddSlider(_scale, 0.5f, 1.5f, (uint)SettingId.ScaleValue);
            settingsBuilder.GroupEnd();

            if (Height > 1)
            {
                settingsBuilder.GroupBegin("Angle");
                settingsBuilder.AddSlider(_angle, -60, 60, (uint)SettingId.AngleValue);
                settingsBuilder.GroupEnd();
            }

            settingsBuilder.PageEnd();
        }

        public override void OnSettingChanged(uint settingId, SettingValue value)
        {
            switch ((SettingId) settingId)
            {
                case SettingId.RedColorValue:
                    _cr = value;
                    break;

                case SettingId.GreenColorValue:
                    _cg = value;
                    break;

                case SettingId.BlueColorValue:
                    _cb = value;
                    break;

                case SettingId.ScaleValue:
                    {
                        _scale = value;
                        UpdateDirection();
                        break;
                    }

                case SettingId.AngleValue:
                    {
                        _angle = value;
                        UpdateDirection();
                        break;
                    }
            }
        }
    }
}
