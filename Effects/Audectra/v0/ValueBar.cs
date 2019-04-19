﻿/**
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
    class ValueBar : EffectBase, IExtension
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

        public ValueBar() { }

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

        public override void GenerateRequirements(ILayerRequirementsBuilder reqBuilder)
        {
            reqBuilder.AddMinimumWidth(4);
            reqBuilder.AddLandscapeAspectRatio();
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

        public string GetName()
        {
            return "Value Bar";
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
