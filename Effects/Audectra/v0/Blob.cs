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
    class Blob : EffectBase, IExtension
    {
        private IEffectHelper _helper;
        private RgbColor _color;
        private IRgbRender _render;

        private float _x0Pos;
        private float _y0Pos;
        private float _xSize;
        private float _ySize;

        private enum SettingId
        {
            ColorValue = 0,
            XPositionValue,
            YPositionValue,
            XSizeValue,
            YSizeValue
        }

        public Blob() { }

        public Blob(IEffectHelper effectHelper, int width, int height) : base(width, height)
        {
            _helper = effectHelper;
            _color = new RgbColor(0, 0.5f, 0.5f);
            _render = _helper.CreateRender();

            _x0Pos = Width / 2;
            _y0Pos = Height / 2;

            if (Width >= Height && Height > 1)
            {
                _ySize = Height / 2;
                _xSize = _ySize;
            }
            else
            {
                _xSize = Width / 2;
                _ySize = _xSize;
            }
        }

        public override IRgbRender Render(float dt)
        {
            _render.Clear();
            _helper.AddBlob(_render, _x0Pos, _y0Pos, _xSize, _ySize, _color);

            return _render;
        }

        public override void GenerateRequirements(ILayerRequirementsBuilder reqBuilder)
        {
            reqBuilder.AddMinimumNumberOfPixels(8);
        }

        public override void GenerateSettings(ILayerSettingsBuilder settingsBuilder)
        {
            settingsBuilder.PageBegin();
            settingsBuilder.AddColorGroup(_color, (uint)SettingId.ColorValue);

            settingsBuilder.GroupBegin("Position");
            settingsBuilder.AddSlider(_x0Pos, 0, Width, (uint)SettingId.XPositionValue);

            if (Height > 1)
                settingsBuilder.AddSlider(_y0Pos, 0, Height, (uint)SettingId.YPositionValue);

            settingsBuilder.GroupEnd();

            settingsBuilder.GroupBegin("Size");
            settingsBuilder.AddBindableSlider(_xSize, 0, Width, (uint)SettingId.XSizeValue);

            if (Height > 1)
                settingsBuilder.AddBindableSlider(_ySize, 0, Height, (uint) SettingId.YSizeValue);

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

                case SettingId.XPositionValue:
                    _x0Pos = value;
                    break;

                case SettingId.YPositionValue:
                    _y0Pos = value;
                    break;

                case SettingId.XSizeValue:
                    _xSize = value;
                    break;

                case SettingId.YSizeValue:
                    _ySize = value;
                    break;
            }
        }

        public string GetName()
        {
            return "Blob";
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
