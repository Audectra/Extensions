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
using Audectra.Extensions.Sdk.V1;

namespace Audectra.Extensions.Effects
{
    [MinWidthRequirement(4)]
    [LandscapeAspectRatioRequirement()]
    [EffectExtension("Value Bar", "Audectra", "1.3.0")]
    class ValueBar : EffectExtensionBase
    {
        private IEffectHelper _helper;
        private RgbColor _color;
        private IRgbRender _render;

        private float _barValue;

        private enum SettingId
        {
            Color,
            BarValue,
        }

        public ValueBar(IEffectHelper effectHelper, int width, int height) : base(width, height)
        {
            _helper = effectHelper;
            _color = new RgbColor(0, 0.5f, 0.5f);
            _render = _helper.CreateRender();

            _barValue = 0;
        }

        public override IRgbRender Render(float dt)
        {
            _render.Clear();
            _helper.FillBar(_render, 0, (int)(Width * _barValue), _color);

            return _render;
        }

        public override void GenerateSettings(ILayerSettingsBuilder settingsBuilder)
        {
            settingsBuilder.PageBegin();
            settingsBuilder.AddColorGroup(_color, (uint)SettingId.Color);

            settingsBuilder.GroupBegin("Value");
            settingsBuilder.AddBindableSlider(_barValue, 0, 1, (uint)SettingId.BarValue);
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

                case SettingId.BarValue:
                    _barValue = value;
                    break;
            }
        }
    }
}
