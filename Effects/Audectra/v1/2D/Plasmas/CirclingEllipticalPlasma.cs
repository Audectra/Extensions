/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;

using SkiaSharp;
using Audectra.Extensions.Sdk.V1;

namespace Audectra.Extensions.Effects
{
    [MinDimensions(8, 8)]
    [EffectExtension("Circling Elliptical Plasma", "Audectra", "v0.0.1")]
    class CirclingEllipticalPlasma : EffectExtensionBase
    {
        private IEffectApi _api;
        private IRender _render;

        private float _passedTime;

        private float _xScale;
        private float _yScale;

        private float _cr;
        private float _cg;
        private float _cb;

        private enum SettingId
        {
            RedColorValue = 0,
            GreenColorValue,
            BlueColorValue,
            XScaleValue,
            YScaleValue,
        }

        public CirclingEllipticalPlasma(IEffectApi api, int width, int height) : base(width, height)
        {
            _api = api;
            _render = _api.CreateRender();

            _cr = 0.0f;
            _cg = 0.5f;
            _cb = 1.0f;

            _xScale = 1.0f;
            _yScale = 1.0f;
        }

        public override IRender Render(float dt)
        {
            _passedTime = _passedTime + dt;

            _render.Clear();
            _render.Map((x, y) => 
            {
                float xx = ((float)x / Width - 0.5f * (float)Math.Sin(_passedTime / 2) - 0.5f);
                float yy = ((float)y / Height - 0.5f * (float)Math.Cos(_passedTime / 2) - 0.5f);
                float v = (float)Math.Sqrt(_xScale * xx * xx + _yScale * yy * yy) - _passedTime;

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
            settingsBuilder.AddSlider(_xScale, 1f, 100f, (uint)SettingId.XScaleValue);
            settingsBuilder.AddSlider(_yScale, 1f, 100f, (uint)SettingId.YScaleValue);
            settingsBuilder.GroupEnd();

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

                case SettingId.XScaleValue:
                    _xScale = value;
                    break;

                case SettingId.YScaleValue:
                    _yScale = value;
                    break;
            }
        }
    }
}
