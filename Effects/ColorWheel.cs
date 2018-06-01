/**
 * Copyright (c) Audectra e.U. - All Rights Reserved
 * This effect is part of Audectra.
 */

using System;

using Audectra.GUI;
using Audectra.Graphics;
using Audectra.Graphics.Effects;

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

        private enum ValueId
        {
            Speed,
            Scale,
        }

        public ColorWheel() { }

        public ColorWheel(IEffectHelper effectHelper, int height, int width) : base(height, width)
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
            _render.Map((color, x, y) =>
            {
                float hue = ((float) x / Width * 360f * _scale + _offset) % 360;
                return new HsvColor(hue, 1, 1);
            });

            return _render;
        }

        public override void GenerateSettings(ILayerSettingsPanel settingsPanel)
        {
            settingsPanel.GroupBegin("Scale");
            settingsPanel.AddTrackbar(this, _scale, MinScale, MaxScale, (uint)ValueId.Scale);
            settingsPanel.GroupEnd();

            settingsPanel.GroupBegin("Speed");
            settingsPanel.AddBindableTrackbar(this, _speed, 0, MaxSpeed, (uint)ValueId.Speed);
            settingsPanel.GroupEnd();
        }

        public override void ValueChanged(uint valueId, object value)
        {
            switch ((ValueId) valueId)
            {
                case ValueId.Speed:
                    _speed = _helper.ValueToSingle(value);
                    break;

                case ValueId.Scale:
                    _scale = _helper.ValueToSingle(value);
                    break;
            }
        }

        public string GetName()
        {
            return "Color Wheel";
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
